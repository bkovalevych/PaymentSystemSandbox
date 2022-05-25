using PaymentSystemSandbox.Services.PaymentService.Stripe.ConfigurationModels;

namespace PaymentSystemSandbox.Services.PaymentService.Stripe.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddStripe(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<StripeCommandSettings>(opt =>
                configuration.GetSection(nameof(StripeCommandSettings)).Bind(opt));
            services.Configure<StripeSettings>(opt =>
                configuration.GetSection(nameof(StripeSettings)).Bind(opt));

            services.AddScoped<StripeBaseService>();
            services.AddScoped<StripePaymentCheckoutAdapter>();

            return services;
        }
    }
}
