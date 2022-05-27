using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using PaymentSystemSandbox.Services.Interfaces;
using PaymentSystemSandbox.Services.PaymentService.LiqPay.ConfigurationModels;
using PaymentSystemSandbox.Services.PaymentService.LiqPay.Models;
using System.Text;
using XSystem.Security.Cryptography;

namespace PaymentSystemSandbox.Services.PaymentService.LiqPay
{
    public class LiqPayBaseService : ILiqPayBaseService
    {
        private readonly LiqPaySettings _settings;
        private readonly JsonSerializerSettings _serializationSettings;

        public LiqPayBaseService(IOptions<LiqPaySettings> liqPaySettings, 
            IOptions<LiqPayCommandSettings> liqPayCommandSettings)
        {
            _settings = liqPaySettings.Value;
            CommandSettings = liqPayCommandSettings.Value;
            _serializationSettings = new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new SnakeCaseNamingStrategy { ProcessDictionaryKeys = true }
                },
                DateFormatString = "yyyy-mm-dd hh:mm:ss",
                Formatting = Formatting.None
            };
            ApiUrl = CommandSettings.ApiUrl;
        }


        public LiqPayCommandSettings CommandSettings { get; }

        public string ApiUrl { get; }

        public void FillWithConfiguredValues(BaseLiqPayCommand command)
        {
            command.Version = CommandSettings.Version;
            command.Currency = CommandSettings.Currency;
            command.ServerUrl = CommandSettings.ServerUrl;
            command.PublicKey = _settings.PublicKey;
        }

        public LiqPayRequest EncryptApiPayload<TPayload>(TPayload payload)
        {
            var dataInJson = JsonConvert.SerializeObject(payload, _serializationSettings);
            var dataBytes = Encoding.UTF8.GetBytes(dataInJson);
            var dataInBase64 = Convert.ToBase64String(dataBytes);

            var signString = _settings.PrivateKey + dataInBase64 + _settings.PrivateKey;
            var signBytes = Encoding.UTF8.GetBytes(signString);
            using var sha1 = new SHA1Managed();
            var signatureBytes = sha1.ComputeHash(signBytes);
            var signatureInBase64 = Convert.ToBase64String(signatureBytes);

            return new LiqPayRequest()
            {
                Data = dataInBase64,
                Signature = signatureInBase64,
            };
        }

        public bool IsValid(LiqPayRequest liqPay)
        {
            var signString = _settings.PrivateKey + liqPay.Data + _settings.PrivateKey;
            var signBytes = Encoding.UTF8.GetBytes(signString);
            using var sha1 = new SHA1Managed();
            var signatureBytes = sha1.ComputeHash(signBytes);
            var signatureInBase64 = Convert.ToBase64String(signatureBytes);

            return liqPay.Signature == signatureInBase64;
        }

        public TPayload DecryptApiPayload<TPayload>(LiqPayRequest liqPay)
        {
            var rowBytes = Convert.FromBase64String(liqPay.Data);
            var rawJson = Encoding.UTF8.GetString(rowBytes);
            var payload = JsonConvert.DeserializeObject<TPayload>(rawJson, _serializationSettings);

            return payload;
        }
    }
}
