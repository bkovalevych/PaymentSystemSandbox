using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using PaymentSystemSandbox.Data;
using PaymentSystemSandbox.Models;

namespace PaymentSystemSandbox.Helpers.Extensions
{
    public static class ServicesIdentityHelper
    {
        public static IServiceCollection AddIDentity(this IServiceCollection services, IConfiguration configuration)
        {
            services
                .AddDefaultIdentity<IdentityUser>(options =>
                {
                    options.SignIn.RequireConfirmedAccount = true;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequireUppercase = false;
                    options.Password.RequireDigit = false;
                })
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();

            services
                .AddAuthorization(opt =>
                {
                    var regularUserPolicy = new AuthorizationPolicyBuilder()
                        .RequireAuthenticatedUser()
                        .Build();
                    opt.AddPolicy(Constants.Roles.RegularUser, regularUserPolicy);

                    var adminUserPolicy = new AuthorizationPolicyBuilder()
                        .RequireAuthenticatedUser()
                        .RequireRole(Constants.Roles.Admin)
                        .Build();
                    opt.AddPolicy(Constants.Roles.Admin, adminUserPolicy);
                });

            return services;
        }
    }
}
