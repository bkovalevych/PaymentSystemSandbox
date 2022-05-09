using Microsoft.AspNetCore.Identity;

namespace PaymentSystemSandbox.Data.Entities
{
    public class Wallet
    {
        public int Id { get; set; }

        public string UserId { get; set; }

        public IdentityUser User { get; set; }

        public decimal Balance { get; set; }
    }
}
