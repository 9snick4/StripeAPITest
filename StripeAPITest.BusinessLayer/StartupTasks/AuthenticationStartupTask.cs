using StripeAPITest.DataAccessLayer;
using StripeAPITest.DataAccessLayer.Entities;
using StripeAPITest.Shared.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace StripeAPITest.BusinessLayer.StartupTasks
{
    public class AuthenticationStartupTask : IHostedService
    {
        private readonly IServiceProvider serviceProvider;

        public AuthenticationStartupTask(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using var scope = serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<StripeContext>();
            await dbContext.Database.MigrateAsync();

            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
            var roleNames = new string[] { RoleNames.Administrator, RoleNames.PowerUser, RoleNames.User };

            foreach (var roleName in roleNames)
            {
                var roleExists = await roleManager.RoleExistsAsync(roleName);
                if (!roleExists)
                {
                    await roleManager.CreateAsync(new ApplicationRole(roleName));
                }
            }

            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var user = await userManager.FindByEmailAsync("admin@test.com");
            if (user == null)
            {
                await userManager.CreateAsync(new ApplicationUser
                {
                    UserName = "admin",
                    Email = "admin@test.com",
                    FirstName = "Admin",
                    LastName = "Admin",
                    CreatedDate = DateTime.Now
                }, "Test-123!");
            }
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
