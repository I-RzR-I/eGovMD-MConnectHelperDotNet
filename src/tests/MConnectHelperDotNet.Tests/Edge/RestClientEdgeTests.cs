using System;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Jose;
using MConnectHelperDotNet.Configurations.Clients.Rest;
using MConnectHelperDotNet.Helpers;
using MConnectHelperDotNet.Models.Client;
using MConnectHelperDotNet.Models.DTO;
using MConnectHelperDotNet.Models.DTO.Request.Base;
using MConnectHelperDotNet.Tests.TestInfrastructure;
using RzR.ResultMessage.Enums;
using Xunit;

namespace MConnectHelperDotNet.Tests.Edge
{
    public sealed class RestClientEdgeTests : IDisposable
    {
        private const string JsonPayload = "{\"ok\":true}";

        private readonly TestCertificateFactory _certificates = new TestCertificateFactory();
        private readonly StubHttpMessageHandler _handler = StubHttpMessageHandler.Throwing(new HttpRequestException(TestValues.Marker));

        public void Dispose()
        {
            _handler.Dispose();
            _certificates.Dispose();
        }

        [Fact]
        public async Task SendRequestAsync_HandlerThrowsWithMarker_ReturnsConstantFirstMessageAndExceptionEntry()
        {
            var logger = new SpyLogger<RestClient>();
            var sut = CreateSut(logger);
            using var outgoing = new HttpRequestMessage(HttpMethod.Post, TestValues.BaseUrl);

            var result = await sut.SendRequestAsync(new SendRequestDto { Request = outgoing, TimeOut = 5 });

            Assert.False(result.IsSuccess);
            Assert.Equal(1, _handler.CallCount);
            Assert.Equal(DefaultMessages.InternalErrorCallMConnectApi, result.GetFirstMessage());
            Assert.Equal(DefaultMessages.InternalErrorCallMConnectApi, EdgeText.Info(result, 0));
            Assert.DoesNotContain(TestValues.Marker, result.GetFirstMessage());
            Assert.DoesNotContain(TestValues.Marker, EdgeText.FirstEntry(result));
            Assert.Equal(2, result.Messages.Count);
            Assert.Equal(MessageType.Exception, result.Messages.ElementAt(1).MessageType);
            Assert.Equal(1, logger.ErrorCount);
        }

        [Fact]
        public void BuildRequest_SignedWithInvalidJson_ReturnsInvalidJsonMessageWithExceptionEntry()
        {
            var sut = CreateSut(new SpyLogger<RestClient>());
            var settings = new RequestSettingDto
            {
                Method = HttpMethod.Post,
                Uri = new Uri(TestValues.BaseUrl),
                SignMessage = true,
                Content = "{ not json " + TestValues.Marker
            };

            var result = sut.BuildRequest(settings);

            Assert.False(result.IsSuccess);
            Assert.Equal(DefaultMessages.InvalidJsonMessage, result.GetFirstMessage());
            Assert.DoesNotContain(TestValues.Marker, result.GetFirstMessage());
            Assert.Equal(2, result.Messages.Count);
        }

        [Fact]
        public void ProcessResponseBody_UnsignedInvalidJson_ReturnsInvalidJsonResponseAndLogsOnce()
        {
            var logger = new SpyLogger<RestClient>();
            var sut = CreateSut(logger);
            var body = new ProcessResponseBodyDto
            {
                ResponseBody = "{ not json " + TestValues.Marker,
                ResponseSetting = new ResponseSettingDto { IsMessageSigned = false }
            };

            var result = sut.ProcessResponseBody(body);

            Assert.False(result.IsSuccess);
            Assert.Equal(DefaultMessages.InvalidJsonResponse, result.GetFirstMessage());
            Assert.DoesNotContain(TestValues.Marker, result.GetFirstMessage());
            Assert.Equal(1, logger.ErrorCount);
        }

        [Fact]
        public void ProcessResponseBody_SignedByDifferentCertificate_Fails()
        {
            var signer = _certificates.Create("MConnect-Test-Signer");
            var otherService = _certificates.Create("MConnect-Test-Service");
            using var signingKey = signer.GetRSAPrivateKey();
            var jws = JWT.Encode(JsonPayload, signingKey, JwsAlgorithm.RS256);
            var logger = new SpyLogger<RestClient>();
            var sut = CreateSut(logger);
            var body = new ProcessResponseBodyDto
            {
                ResponseBody = jws,
                ResponseSetting = new ResponseSettingDto { IsMessageSigned = true, ServiceCertificate = otherService }
            };

            var result = sut.ProcessResponseBody(body);

            Assert.False(result.IsSuccess);
            Assert.Null(result.Response);
            Assert.Equal(DefaultMessages.InvalidJsonResponse, result.GetFirstMessage());
            Assert.Single(result.Messages);
            Assert.Equal(1, logger.ErrorCount);
        }

        private RestClient CreateSut(SpyLogger<RestClient> logger)
            => new RestClient(new FakeHttpClientFactory(_handler), FakeOptionsMonitor<ClientCertificateOptions>.ForClientCertificate(), logger);
    }
}
