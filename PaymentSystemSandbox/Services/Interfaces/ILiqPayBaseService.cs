using PaymentSystemSandbox.Services.PaymentService.LiqPay.Models;
namespace PaymentSystemSandbox.Services.Interfaces
{
    public interface ILiqPayBaseService
    {
        string ApiUrl { get; }

        void FillWithConfiguredValues(BaseLiqPayCommand command);

        LiqPayRequest EncryptApiPayload<TPayload>(TPayload payload);

        bool IsValid(LiqPayRequest liqPay);

        TPayload DecryptApiPayload<TPayload>(LiqPayRequest liqPay);
    }
}
