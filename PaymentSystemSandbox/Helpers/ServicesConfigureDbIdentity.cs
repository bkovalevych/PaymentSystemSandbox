using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using PaymentSystemSandbox.Models;

namespace PaymentSystemSandbox.Helpers
{
    public static class ServicesConfigureDbIdentity
    {
        public static async Task ConfigureAsync(IServiceProvider serviceProvider)
        {
            await ConfigureRoles(serviceProvider);
            await ConfigureAdmin(serviceProvider);
        }

        private static async Task ConfigureRoles(IServiceProvider provider)
        {
            var roles = provider.GetRequiredService<RoleManager<IdentityRole>>();
            if (!await roles.RoleExistsAsync(Constants.Roles.Admin))
            {
                await roles.CreateAsync(new IdentityRole(Constants.Roles.Admin));
            }

            if (!await roles.RoleExistsAsync(Constants.Roles.RegularUser))
            {
                await roles.CreateAsync(new IdentityRole(Constants.Roles.RegularUser));
            }
        }

        private static async Task ConfigureAdmin(IServiceProvider provider)
        {
            var settings = provider.GetRequiredService<IOptions<AdminSettings>>();
            var userManager = provider.GetRequiredService<UserManager<IdentityUser>>();
            var adminSettings = settings.Value;
            var admin = new IdentityUser(adminSettings.UserName);
            if (await userManager.Users.AnyAsync(it => it.UserName == adminSettings.UserName))
            {
                return;
            }
            var result = await userManager.CreateAsync(admin, adminSettings.Password);
            if (!result.Succeeded)
            {
                throw new Exception($"Admin was not created. {string.Join('\n', result.Errors)}");
            }

            result = await userManager.AddToRoleAsync(admin, Constants.Roles.Admin);

            if (!result.Succeeded)
            {
                throw new Exception($"Admin was not created. {string.Join('\n', result.Errors)}");
            }
        }
    }
}
