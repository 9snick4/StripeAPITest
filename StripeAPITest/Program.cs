using StripeAPITest.DataAccessLayer;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi.Models;
using StripeAPITest.Shared.Settings;
using Microsoft.AspNetCore.Authorization;
using System.Text;
using StripeAPITest.DataAccessLayer.Entities;
using Stripe;
using StripeAPITest.BusinessLayer.StartupTasks;
using StripeAPITest.BusinessLayer.Services;
using FluentValidation.AspNetCore;
using StripeAPITest.Shared.Models;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

var Configuration = builder.Configuration;

// Add services to the container.
var jwtSettings = Configure<JwtSettings>(nameof(JwtSettings));
var stripeSettings = Configure<StripeSettings>(nameof(StripeSettings));
StripeConfiguration.ApiKey = stripeSettings.TestApiKey;

builder.Services.AddControllers()
    .AddFluentValidation(config =>
    {
        config.RegisterValidatorsFromAssembly(typeof(TransactionValidator).Assembly);
    });
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "MVCBaseAuth", Version = "v1" });

    options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Insert the Bearer Token",
        Name = HeaderNames.Authorization,
        Type = SecuritySchemeType.ApiKey
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference= new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = JwtBearerDefaults.AuthenticationScheme
                }
            },
            Array.Empty<string>()
        }
    });
});

services.AddDbContext<StripeContext>(options =>
{
    var connectionString = Configuration.GetConnectionString("SqlConnection");
    options.UseSqlServer(connectionString);
});

services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
{
    options.User.RequireUniqueEmail = true;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;

})
.AddEntityFrameworkStores<StripeContext>()
.AddDefaultTokenProviders();

services.AddAuthentication(options =>
{
    options.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = jwtSettings.Issuer,
        ValidateAudience = true,
        ValidAudience = jwtSettings.Audience,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecurityKey)),
        RequireExpirationTime = true,
        ClockSkew = TimeSpan.Zero
    };
});

services.AddAuthorization(options =>
{
    var policyBuilder = new AuthorizationPolicyBuilder().RequireAuthenticatedUser();
    options.FallbackPolicy = options.DefaultPolicy = policyBuilder.Build();
});



services.AddHostedService<AuthenticationStartupTask>();
services.AddScoped<IIdentityService, IdentityService>();
services.AddScoped<ITransactionService, TransactionService>();

var app = builder.Build();



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseRouting();

app.UseAuthorization();

app.MapControllers();

app.Run();



T Configure<T>(string sectionName) where T : class
{
    var section = Configuration.GetSection(sectionName);
    var settings = section.Get<T>();
    services.Configure<T>(section);

    return settings;
}
