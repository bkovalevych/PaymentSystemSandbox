namespace PaymentSystemSandbox.Services.PaymentService.LiqPay.Models
{
    public class PayCommandBuilder
    {
        private readonly PayCommand _command;

        public PayCommandBuilder()
        {
            _command = new PayCommand()
            {
                Version = "3",
                Action = "pay",
                Currency = "UAH",
                Description = "Default description",
                Language = "en",
                OrderId = Guid.NewGuid().ToString(),
                Sandbox = 1
            };
        }

        public PayCommandBuilder WithAmount(decimal amount)
        {
            _command.Amount = amount;
            return this;
        }

        public PayCommandBuilder WithResultUrl(string resultUrl)
        {
            _command.ResultUrl = resultUrl;
            return this;
        }

        public PayCommand Build()
        {
            return _command;
        }
    }
}
