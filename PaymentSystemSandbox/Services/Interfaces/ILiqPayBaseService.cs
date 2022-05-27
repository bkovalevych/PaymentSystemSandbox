using PaymentSystemSandbox.Services.PaymentService.LiqPay.ConfigurationModels;
using PaymentSystemSandbox.Services.PaymentService.LiqPay.Models;
namespace PaymentSystemSandbox.Services.Interfaces
{
    public interface ILiqPayBaseService
    {
        string ApiUrl { get; }

        public LiqPayCommandSettings CommandSettings { get; }

        void FillWithConfiguredValues(BaseLiqPayCommand command);

        LiqPayRequest EncryptApiPayload<TPayload>(TPayload payload);

        bool IsValid(LiqPayRequest liqPay);

        TPayload DecryptApiPayload<TPayload>(LiqPayRequest liqPay);
    }
}
