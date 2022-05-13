using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentSystemSandbox.Services.PaymentService.Commands
{
    public class PayCommand
    {
        public string CardNumber { get; internal set; }
        public string ExpirationDate { get; internal set; }
        public string CardCode { get; internal set; }
    }
}
