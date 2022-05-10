#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PaymentSystemSandbox.Data;
using PaymentSystemSandbox.Data.Entities;
using PaymentSystemSandbox.Models;
using PaymentSystemSandbox.Services.Interfaces;

namespace PaymentSystemSandbox.Pages.Admin
{
    public class ReportTransactionsModel : PageModel
    {
        private readonly IPaymentReportsService _paymentReportsService;

        public ReportTransactionsModel(IPaymentReportsService paymentReportsService)
        {
            _paymentReportsService = paymentReportsService;
        }

        public TransactionReport TransactionReport { get;set; }

        public string NextDisabled { get; set; } = "";
        public int NextTopIndex { get; set; } = 0;

        public string PrevDisabled { get; set; } = "";
        public int PrevTopIndex { get; set; } = 0;

        public async Task OnGetAsync(int? top, int? offset)
        {
            TransactionReport = await _paymentReportsService.GetReportAsync(top, offset);
            if (TransactionReport.Top + TransactionReport.Offset > TransactionReport.TotalCount)
            {
                NextDisabled = "disabled";
            }
            if (TransactionReport.Top == 0)
            {
                PrevDisabled = "disabled";
            }

            NextTopIndex = TransactionReport.Top + TransactionReport.Offset;
            PrevTopIndex = Math.Max(0, TransactionReport.Top - TransactionReport.Offset);
        }
    }
}
