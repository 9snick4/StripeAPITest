using StripeAPITest.Shared.Models;
using Microsoft.AspNetCore.Identity;

namespace StripeAPITest.BusinessLayer.Services
{
    public interface IIdentityService
    {
        Task<User> GetMeAsync(Guid userId);
        Task<List<User>> GetUsersAsync();
        Task<AuthResponse> LoginAsync(ILoginRequest request, bool rememberMe);
        Task<AuthResponse> RefreshTokenAsync(RefreshTokenRequest request);
        Task<RegisterResponse> RegisterAsync(RegisterRequest request);
        Task<NewUser> InsertUserAsync(NewUser item);

    }
}