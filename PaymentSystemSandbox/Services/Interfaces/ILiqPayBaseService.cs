using PaymentSystemSandbox.Services.PaymentService.LiqPay.Models;
namespace PaymentSystemSandbox.Services.Interfaces
{
    public interface ILiqPayBaseService
    {
        string ApiUrl { get; }

        void FillWithConfiguredValues(BaseLiqPayCommand command);

        LiqPayRequest EncryptApiPayload<TPayload>(TPayload payload);

        public bool IsValid(LiqPayRequest liqPay);

        public TPayload DecryptApiPayload<TPayload>(LiqPayRequest liqPay);
    }
}
