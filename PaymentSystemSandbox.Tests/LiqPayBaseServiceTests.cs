using Microsoft.Extensions.Options;
using Moq;
using PaymentSystemSandbox.Services.PaymentService.LiqPay;
using PaymentSystemSandbox.Services.PaymentService.LiqPay.ConfigurationModels;
using PaymentSystemSandbox.Services.PaymentService.LiqPay.Models;
using System.Collections.Generic;
using System.Text.Json;
using Xunit;

namespace PaymentSystemSandbox.Tests
{
    public class LiqPayBaseServiceTests
    {
        private readonly LiqPayBaseService _liqPayBaseService;

        public LiqPayBaseServiceTests()
        {
            var fakeOptions = new Mock<IOptions<LiqPaySettings>>();
            fakeOptions.SetupGet(it => it.Value)
                .Returns(new LiqPaySettings()
                {
                    PrivateKey = "this_is_private_key",
                    PublicKey = "this_is_public_key"
                });
            var fakeCommandOptions = new Mock<IOptions<LiqPayCommandSettings>>();
            fakeCommandOptions.SetupGet(it => it.Value)
                .Returns(new LiqPayCommandSettings());
            
            _liqPayBaseService = new LiqPayBaseService(fakeOptions.Object, fakeCommandOptions.Object);
        }

        [Fact]
        public void EncryptApiPayload_WithUserData_ShouldReturnLiqPayRequest()
        {
            // Arrange

            var data = JsonSerializer.Deserialize<Dictionary<string, string>>("{\"foo\":\"bar\"}");
            
            // Act

            var result = _liqPayBaseService.EncryptApiPayload(data);

            // Assert

            Assert.Equal("eyJmb28iOiJiYXIifQ==", result.Data);

            Assert.Equal("HPPX0ubp3VSI733I28gXDWapJWE=", result.Signature);
        }

        [Fact]
        public void DeccryptApiPayload_WithUserData_ShouldReturnJsonString()
        {
            // Arrange

            var data = JsonSerializer.Deserialize<Dictionary<string, string>>("{\"foo\":\"bar\"}");

            // Act

            var result = _liqPayBaseService.DecryptApiPayload<Dictionary<string, string>>(new LiqPayRequest()
            {
                Data = "eyJmb28iOiJiYXIifQ=="
            });

            // Assert

            Assert.Equal("bar", result["foo"]);
        }

        [Fact]
        public void IsValid_withWrongData_ShouldReturnFalse()
        {
            // Arrange

            var data = JsonSerializer.Deserialize<Dictionary<string, string>>("{\"foo\":\"very bad\"}");
            var payload = _liqPayBaseService.EncryptApiPayload(data);
            payload.Signature = "HPPX0ubp3VSI733I28gXDWapJWE=";

            // Act

            var result = _liqPayBaseService.IsValid(payload);

            // Assert

            Assert.False(result);
        }

        [Fact]
        public void IsValid_withData_ShouldReturnTrue()
        {
            // Arrange

            var data = JsonSerializer.Deserialize<Dictionary<string, string>>("{\"foo\":\"bar\"}");
            var payload = _liqPayBaseService.EncryptApiPayload(data);

            // Act

            var result = _liqPayBaseService.IsValid(payload);

            // Assert

            Assert.True(result);
        }
    }
}