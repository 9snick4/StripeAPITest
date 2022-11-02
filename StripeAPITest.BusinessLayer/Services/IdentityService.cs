using StripeAPITest.BusinessLayer.Extensions;
using StripeAPITest.DataAccessLayer;
using StripeAPITest.DataAccessLayer.Entities;
using StripeAPITest.Shared.Enums;
using StripeAPITest.Shared.Models;
using StripeAPITest.Shared.Settings;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using StripeAPITest.BusinessLayer.Services;

namespace StripeAPITest.BusinessLayer.Services
{
    public class IdentityService : IIdentityService
    {
        private readonly JwtSettings jwtSettings;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly ILogger<IdentityService> logger;
        private readonly StripeContext context;


        public IdentityService(IOptions<JwtSettings> jwtSettingsOptions,
            UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager,
            ILogger<IdentityService> logger, StripeContext context)
        {
            jwtSettings = jwtSettingsOptions.Value;
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.logger = logger;
            this.context = context;

        }

        public async Task<User> GetMeAsync(Guid userId)
        {
            var entity = await context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            return new User()
            {
                Email = entity.Email,
                FirstName = entity.FirstName,
                LastName = entity.LastName
            };
        }

        public async Task<List<User>> GetUsersAsync()
        {
            var query = context.Users.AsQueryable();
            var res = query.Select(item => new User {
                Email = item.Email,
                FirstName = item.FirstName,
                LastName = item.LastName
            });
            return await res.ToListAsync();
        }


        public async Task<AuthResponse> LoginAsync(ILoginRequest request, bool rememberMe)
        {
            var signInResult = await MakePasswordSignin(request);
            if (!signInResult.Succeeded)
                return null;


            var user = await userManager.FindByNameAsync(request.Username);
            var userRoles = await userManager.GetRolesAsync(user);

            var claims = new List<Claim>()
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, request.Username),
                    new Claim(ClaimTypes.GivenName, user.FirstName),
                    new Claim(ClaimTypes.Surname, user.LastName ?? string.Empty),
                    new Claim(ClaimTypes.Email, user.Email)
                }.Union(userRoles.Select(role => new Claim(ClaimTypes.Role, role)));

            var loginResponse = CreateToken(claims);

            user.RefreshToken = loginResponse.RefreshToken;
            user.RefreshTokenExpirationDate = DateTime.UtcNow.AddMinutes(jwtSettings.RefreshTokenExpirationMinutes);

            await userManager.UpdateAsync(user);

            return loginResponse;
        }

        public async Task<AuthResponse> RefreshTokenAsync(RefreshTokenRequest request)
        {
            var user = ValidateAccessToken(request.AccessToken);
            if (user != null)
            {
                var userId = user.GetId();
                var dbUser = await userManager.FindByIdAsync(userId.ToString());

                if (dbUser?.RefreshToken == null || dbUser?.RefreshTokenExpirationDate < DateTime.UtcNow || dbUser?.RefreshToken != request.RefreshToken)
                    return null;

                var loginResponse = CreateToken(user.Claims);

                dbUser.RefreshToken = loginResponse.RefreshToken;
                dbUser.RefreshTokenExpirationDate = DateTime.UtcNow.AddMinutes(jwtSettings.RefreshTokenExpirationMinutes);

                await userManager.UpdateAsync(dbUser);

                return loginResponse;
            }

            return null;
        }

        public async Task<RegisterResponse> RegisterAsync(RegisterRequest request)
        {
            var user = new ApplicationUser
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                UserName = request.Email
            };

            var result = await userManager.CreateAsync(user, request.Password);
            if (result.Succeeded)
            {
                result = await userManager.AddToRoleAsync(user, RoleNames.User);
            }

            var response = new RegisterResponse
            {
                Succeeded = result.Succeeded,
                Errors = result.Errors.Select(e => e.Description)
            };

            return response;
        }

        public async Task<NewUser> InsertUserAsync(NewUser item)
        {
            try
            {
                var entity = new ApplicationUser
                {
                    FirstName = item.FirstName,
                    LastName = item.LastName,
                    Email = item.Email,
                    NormalizedEmail = item.Email.ToUpper(),
                    UserName = item.Username,
                    NormalizedUserName = item.Username.ToUpper(),
                    Active = true,
                    CreatedDate = DateTime.Now,
                    PhoneNumber = item.PhoneNumber
                };
                await context.Users.AddAsync(entity);
                await context.SaveChangesAsync();
                item.Id = entity.Id;

                var userRole = new ApplicationUserRole
                {
                    UserId = item.Id.Value,
                    RoleId = item.RoleId
                };
                await context.UserRoles.AddAsync(userRole);
                await context.SaveChangesAsync();

                return item;
            }
            catch (Exception e)
            {
                item.Id = null;
                return item;
            }
        }

        public async Task<(List<BaseLookupWithGuid> items, int count)> GetUsersRolesAsync()
        {
            var query = context.Roles.Where(w => w.Active == true).AsQueryable().OrderBy(o => o.Name);

            var itemsCount = query.Count();

            List<BaseLookupWithGuid> items;
            items = await query.Select(item => new BaseLookupWithGuid
            {
                Id = item.Id,
                Code = null,
                Description = item.Name
            }).AsNoTracking().ToListAsync();

            return (items, itemsCount);
        }

        #region Helpers
        private async Task<SignInResult> MakePasswordSignin(ILoginRequest request, bool rememberMe = false)
        {
            
            return await signInManager.PasswordSignInAsync(request.Username, request.Password, rememberMe, false);
        }

        private AuthResponse CreateToken(IEnumerable<Claim> claims)
        {
            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecurityKey));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            var jwtSecurityToken = new JwtSecurityToken(jwtSettings.Issuer, jwtSettings.Audience, claims,
                DateTime.UtcNow, DateTime.UtcNow.AddMinutes(jwtSettings.AccessTokenExpirationMinutes), signingCredentials);

            var accessToken = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);

            var response = new AuthResponse
            {
                AccessToken = accessToken,
                RefreshToken = GenerateRefreshToken()
            };

            return response;

            static string GenerateRefreshToken()
            {
                var randomNumber = new byte[256];
                using var generator = RandomNumberGenerator.Create();
                generator.GetBytes(randomNumber);

                return Convert.ToBase64String(randomNumber);
            }
        }

        private ClaimsPrincipal ValidateAccessToken(string accessToken)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = jwtSettings.Issuer,
                ValidateAudience = true,
                ValidAudience = jwtSettings.Audience,
                ValidateLifetime = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecurityKey)),
                RequireExpirationTime = true,
                ClockSkew = TimeSpan.Zero
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            try
            {
                var user = tokenHandler.ValidateToken(accessToken, tokenValidationParameters, out var securityToken);
                if (securityToken is JwtSecurityToken jwtSecurityToken && jwtSecurityToken.Header.Alg == SecurityAlgorithms.HmacSha256)
                {
                    return user;
                }
            }
            catch
            {
            }

            return null;
        }
        #endregion
    }
}
