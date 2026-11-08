using System;
using MConnectHelperDotNet.Configurations.Clients.Soap;
using MConnectHelperDotNet.Models.Client;
using MConnectHelperDotNet.Models.DTO;
using MConnectHelperDotNet.Models.DTO.Request.Base;
using MConnectHelperDotNet.Tests.TestInfrastructure;
using Newtonsoft.Json.Linq;
using Xunit;

namespace MConnectHelperDotNet.Tests.Functional
{
    public sealed class Soap11ClientTests : IDisposable
    {
        private const string PayloadXml = "<Pong>ok</Pong>";

        private readonly TestCertificateFactory _certificates = new TestCertificateFactory();
        private readonly StubHttpMessageHandler _handler = StubHttpMessageHandler.Responding(string.Empty);

        public void Dispose()
        {
            _handler.Dispose();
            _certificates.Dispose();
        }

        private Soap11Client CreateClient() => new Soap11Client(
            new FakeHttpClientFactory(_handler),
            FakeOptionsMonitor<ClientCertificateOptions>.ForClientCertificate(),
            new SpyLogger<Soap11Client>());

        [Fact]
        public void BuildRequest_SoapActionSet_AddsQuotedSoapActionHeader()
        {
            const string soapAction = "x";
            var client = CreateClient();
            var settings = TestRequests.RequestSettings(soapAction: soapAction);

            var result = client.BuildRequest(settings);
            using var request = result.Response;

            Assert.True(result.IsSuccess);
            var header = Assert.Single(request.Headers.GetValues("SOAPAction"));
            Assert.Equal("\"" + soapAction + "\"", header);
        }

        [Fact]
        public void ProcessResponseBody_UnsignedOkEnvelope_ReturnsBodyPayload()
        {
            var client = CreateClient();
            var dto = new ProcessResponseBodyDto
            {
                ResponseBody = TestSoapEnvelopes.Ok(PayloadXml),
                ResponseSetting = new ResponseSettingDto { IsMessageSigned = false, ValidateSignedMessage = false }
            };

            var result = client.ProcessResponseBody(dto);

            Assert.True(result.IsSuccess);
            Assert.True(JToken.DeepEquals(new JObject { ["Pong"] = "ok" }, result.Response));
        }

        [Fact]
        public void ProcessResponseBody_SignedEnvelopeBuiltByLibrary_ValidatesAndReturnsBodyPayload()
        {
            var signingCertificate = _certificates.Create();
            var envelope = TestSoapEnvelopes.SignedViaLibrary(signingCertificate, PayloadXml);
            var client = CreateClient();
            var dto = new ProcessResponseBodyDto
            {
                ResponseBody = envelope,
                ResponseSetting = new ResponseSettingDto
                {
                    IsMessageSigned = true,
                    ValidateSignedMessage = true,
                    ServiceCertificate = signingCertificate
                }
            };

            var result = client.ProcessResponseBody(dto);

            Assert.True(result.IsSuccess);
            Assert.True(JToken.DeepEquals(new JObject { ["Pong"] = "ok" }, result.Response));
        }
    }
}
