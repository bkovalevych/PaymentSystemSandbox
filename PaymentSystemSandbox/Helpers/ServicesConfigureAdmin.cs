using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using PaymentSystemSandbox.Models;

namespace PaymentSystemSandbox.Helpers
{
    public static class ServicesConfigureAdmin
    {
        public static async Task ConfigureAsync(IServiceProvider serviceProvider)
        {
            var settings = serviceProvider.GetRequiredService<IOptions<AdminSettings>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

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

            if (!await roleManager.Roles.AnyAsync(it => it.Name == Constants.Roles.Admin))
            {
                await roleManager.CreateAsync(new IdentityRole(Constants.Roles.Admin));
            }

            result = await userManager.AddToRoleAsync(admin, Constants.Roles.Admin);

            if (!result.Succeeded)
            {
                throw new Exception($"Admin was not created. {string.Join('\n', result.Errors)}");
            }
        }
    }
}
