using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentSystemSandbox.Services.PaymentService.LiqPay.Models
{
    public class BaseLiqPayCommand
    {
        public string Version { get; set; }

        public string PublicKey { get; set; }

        public string Currency { get; set; }

        public string ResultUrl { get; set; }

        public string ServerUrl { get; set; }

        public int Sandbox { get; set; }
    }
}
