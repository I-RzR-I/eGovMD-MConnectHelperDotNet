using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using MConnectHelperDotNet.Models.DTO;
using MConnectHelperDotNet.Services;
using MConnectHelperDotNet.Tests.TestInfrastructure;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RzR.ResultMessage;
using Xunit;

namespace MConnectHelperDotNet.Tests.Functional
{
    public sealed class MConnectApiServiceTests : IDisposable
    {
        private const string HeaderSentinel = "HDR-X";
        private const string BuildSentinel = "BLD-X";
        private const string SendSentinel = "SND-X";
        private const string ProcessSentinel = "PRB-X";
        private const string BodyXml = "<Pong>ok</Pong>";

        private readonly TestCertificateFactory _certificates = new TestCertificateFactory();
        private readonly FakeRequestBuilderService _builder = new FakeRequestBuilderService();
        private readonly FakeEndpointClient _client = new FakeEndpointClient();
        private readonly SpyLogger<MConnectApiService> _logger = new SpyLogger<MConnectApiService>();

        public void Dispose() => _certificates.Dispose();

        private MConnectApiService CreateService() => new MConnectApiService(_ => _client, _builder, _logger);

        private void UseHeaderWithServiceCertificate()
        {
            var certificate = _certificates.Create();
            _builder.NextResult = Result<RequestBuilderDto>.Success(new RequestBuilderDto
            {
                EndpointUrl = TestValues.BaseUrl,
                CallingEntity = TestValues.CallingEntity,
                CallingUser = TestValues.CallingUser,
                CallBasis = TestValues.CallBasis,
                CallReason = TestValues.CallReason,
                ServiceCertificatePath = _certificates.WriteCer(certificate)
            });
        }

        [Fact]
        public async Task SendRequestAsync_HeaderFailure_SurfacesSentinelAndSkipsLaterSteps()
        {
            _builder.NextResult = Result<RequestBuilderDto>.Failure(HeaderSentinel);
            var service = CreateService();

            var result = await service.SendRequestAsync(TestRequests.MConnectRequest());

            Assert.False(result.IsSuccess);
            Assert.Equal(HeaderSentinel, result.GetFirstMessage());
            Assert.Equal(1, _builder.CallCount);
            Assert.Equal(0, _client.BuildRequestCount);
            Assert.Equal(0, _client.SendRequestCount);
            Assert.Equal(0, _client.ProcessResponseBodyCount);
        }

        [Fact]
        public async Task SendRequestAsync_BuildRequestFailure_SurfacesSentinelAndSkipsLaterSteps()
        {
            _client.BuildRequestHandler = _ => Result<HttpRequestMessage>.Failure(BuildSentinel);
            var service = CreateService();

            var result = await service.SendRequestAsync(TestRequests.MConnectRequest());

            Assert.False(result.IsSuccess);
            Assert.Equal(BuildSentinel, result.GetFirstMessage());
            Assert.Equal(1, _client.BuildRequestCount);
            Assert.Equal(0, _client.SendRequestCount);
            Assert.Equal(0, _client.ProcessResponseBodyCount);
        }

        [Fact]
        public async Task SendRequestAsync_SendRequestFailure_SurfacesSentinelAndSkipsProcessing()
        {
            _client.SendRequestHandler = _ => Result<HttpResponseMessage>.Failure(SendSentinel);
            var service = CreateService();

            var result = await service.SendRequestAsync(TestRequests.MConnectRequest());

            Assert.False(result.IsSuccess);
            Assert.Equal(SendSentinel, result.GetFirstMessage());
            Assert.Equal(1, _client.BuildRequestCount);
            Assert.Equal(1, _client.SendRequestCount);
            Assert.Equal(0, _client.ProcessResponseBodyCount);
        }

        [Fact]
        public async Task SendRequestAsync_ProcessResponseBodyFailure_SurfacesSentinel()
        {
            UseHeaderWithServiceCertificate();
            using var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(TestSoapEnvelopes.Ok(BodyXml))
            };
            _client.SendRequestHandler = _ => Result<HttpResponseMessage>.Success(response);
            _client.ProcessResponseBodyHandler = _ => Result<JToken>.Failure(ProcessSentinel);
            var service = CreateService();

            var result = await service.SendRequestAsync(TestRequests.MConnectRequest());
            using var serviceCertificate = _client.LastProcessResponseBody?.ResponseSetting?.ServiceCertificate;

            Assert.False(result.IsSuccess);
            Assert.Equal(ProcessSentinel, result.GetFirstMessage());
            Assert.Equal(1, _client.ProcessResponseBodyCount);
        }

        [Fact]
        public async Task SendRequestAsync_HappyPath_ReturnsIndentedJsonWithoutMessages()
        {
            UseHeaderWithServiceCertificate();
            var payload = new JObject { ["answer"] = "42" };
            using var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(TestSoapEnvelopes.Ok(BodyXml))
            };
            _client.SendRequestHandler = _ => Result<HttpResponseMessage>.Success(response);
            _client.ProcessResponseBodyHandler = _ => Result<JToken>.Success(payload);
            var service = CreateService();

            var result = await service.SendRequestAsync(TestRequests.MConnectRequest());
            using var serviceCertificate = _client.LastProcessResponseBody?.ResponseSetting?.ServiceCertificate;

            Assert.True(result.IsSuccess);
            Assert.Equal(JsonConvert.SerializeObject(payload, Formatting.Indented), result.Response);
            Assert.Empty(result.Messages);
            Assert.Equal(1, _client.ProcessResponseBodyCount);
        }
    }
}
