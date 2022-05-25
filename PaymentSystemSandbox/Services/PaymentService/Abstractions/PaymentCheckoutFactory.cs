using System.Reflection;

namespace PaymentSystemSandbox.Services.PaymentService.Abstractions
{
    public class PaymentCheckoutFactory
    {
        private Dictionary<string, IAbstractPaymentCheckoutAdapter> _services;
        public PaymentCheckoutFactory(IServiceProvider provider)
        {
            _services = Assembly.GetExecutingAssembly().GetTypes()
               .Where(t => !t.IsAbstract
               && typeof(IAbstractPaymentCheckoutAdapter).IsAssignableFrom(t))
               .Select(it => provider.GetService(it) as IAbstractPaymentCheckoutAdapter)
               .Where(it => it != null)
               .ToDictionary(it => it.PaymentName, it => it)
               as Dictionary<string, IAbstractPaymentCheckoutAdapter>;
        }

        public List<string> AvailablePayments => _services.Keys.ToList();

        public IAbstractPaymentCheckoutAdapter GetServiceByName(string name)
        {
            return _services[name];
        }
    }
}
