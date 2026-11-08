using System;
using System.Security.Cryptography.X509Certificates;
using Jose;
using MConnectHelperDotNet.Configurations.Clients.Rest;
using MConnectHelperDotNet.Models.Client;
using MConnectHelperDotNet.Models.DTO;
using MConnectHelperDotNet.Models.DTO.Request.Base;
using MConnectHelperDotNet.Tests.TestInfrastructure;
using Newtonsoft.Json.Linq;
using Xunit;

namespace MConnectHelperDotNet.Tests.Functional
{
    public sealed class RestClientTests : IDisposable
    {
        private const string PayloadJson = "{\"answer\":\"42\"}";

        private readonly TestCertificateFactory _certificates = new TestCertificateFactory();
        private readonly StubHttpMessageHandler _handler = StubHttpMessageHandler.Responding(string.Empty);

        public void Dispose()
        {
            _handler.Dispose();
            _certificates.Dispose();
        }

        private RestClient CreateClient() => new RestClient(
            new FakeHttpClientFactory(_handler),
            FakeOptionsMonitor<ClientCertificateOptions>.ForClientCertificate(),
            new SpyLogger<RestClient>());

        [Fact]
        public void ProcessResponseBody_UnsignedValidJson_ReturnsParsedPayload()
        {
            var client = CreateClient();
            var dto = new ProcessResponseBodyDto
            {
                ResponseBody = PayloadJson,
                ResponseSetting = new ResponseSettingDto { IsMessageSigned = false }
            };

            var result = client.ProcessResponseBody(dto);

            Assert.True(result.IsSuccess);
            Assert.True(JToken.DeepEquals(JToken.Parse(PayloadJson), result.Response));
        }

        [Fact]
        public void ProcessResponseBody_JwsSignedWithServiceCertificate_ReturnsDecodedPayload()
        {
            var signingCertificate = _certificates.Create();
            using var privateKey = signingCertificate.GetRSAPrivateKey();
            var jws = JWT.Encode(PayloadJson, privateKey, JwsAlgorithm.RS256);
            var client = CreateClient();
            var dto = new ProcessResponseBodyDto
            {
                ResponseBody = jws,
                ResponseSetting = new ResponseSettingDto { IsMessageSigned = true, ServiceCertificate = signingCertificate }
            };

            var result = client.ProcessResponseBody(dto);

            Assert.True(result.IsSuccess);
            Assert.True(JToken.DeepEquals(JToken.Parse(PayloadJson), result.Response));
        }
    }
}
