using PaymentSystemSandbox.Services.Interfaces;
using PaymentSystemSandbox.Services.PaymentService.LiqPay.ConfigurationModels;

namespace PaymentSystemSandbox.Services.PaymentService.LiqPay.Extensions
{
    public static class ServicesExtensions
    {
        public static IServiceCollection AddLiqPay(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<LiqPayCommandSettings>(opt =>
                configuration.GetSection(nameof(LiqPayCommandSettings)).Bind(opt));
            services.Configure<LiqPaySettings>(opt =>
                configuration.GetSection(nameof(LiqPaySettings)).Bind(opt));

            services.AddScoped<ILiqPayBaseService, LiqPayBaseService>();
            services.AddScoped<LiqpayPaymentCheckoutAdapter>();
            return services;
        }
    }
}
