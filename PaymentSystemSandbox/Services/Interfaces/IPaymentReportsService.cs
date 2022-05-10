using PaymentSystemSandbox.Models;

namespace PaymentSystemSandbox.Services.Interfaces
{
    public interface IPaymentReportsService
    {
        Task<TransactionReport> GetReportAsync(int? top = 0, int? offset = 20);

        TransactionReport GetReport(int? top = 0, int? offset = 20);
    }
}
