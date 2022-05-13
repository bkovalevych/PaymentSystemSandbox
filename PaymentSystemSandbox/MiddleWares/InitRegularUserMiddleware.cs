using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PaymentSystemSandbox.Data;
using PaymentSystemSandbox.Models;
using PaymentSystemSandbox.Services.Interfaces;
using System.Security.Claims;

namespace PaymentSystemSandbox.Middlewares
{
    public class InitRegularUserMiddleware : IMiddleware
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IWalletService _walletService;

        public InitRegularUserMiddleware(IServiceProvider serviceProvider)
        {
            _context = serviceProvider.GetRequiredService<ApplicationDbContext>();
            _userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();
            _walletService = serviceProvider.GetRequiredService<IWalletService>();
        }
        public async Task Invoke(HttpContext httpContext)
        {
            var userId = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return;
            }
            await InitWallet(userId);
            await InitRoles(userId);
        }

        public async Task InitWallet(string userId)
        {
            if (!await _context.Wallets.AnyAsync(it => it.UserId == userId))
            {
                await _walletService.InitiateWalletForUserAsync(userId);
            }
        }

        public async Task InitRoles(string userId)
        {
            if (await _userManager.Users.FirstOrDefaultAsync(it => it.Id == userId) 
                is IdentityUser user &&
                !await _userManager.IsInRoleAsync(user, Constants.Roles.Admin) &&
                !await _userManager.IsInRoleAsync(user, Constants.Roles.RegularUser))
            {
                await _userManager.AddToRoleAsync(user, Constants.Roles.RegularUser);
            }
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            await Invoke(context);
            await next(context);
        }
    }
}
