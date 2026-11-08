using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;
using MConnectHelperDotNet.Abstractions;
using MConnectHelperDotNet.Enums;
using MConnectHelperDotNet.Helpers;
using MConnectHelperDotNet.Models.DTO;
using MConnectHelperDotNet.Models.DTO.Request;
using MConnectHelperDotNet.Services;
using MConnectHelperDotNet.Tests.TestInfrastructure;
using RzR.ResultMessage;
using RzR.ResultMessage.Abstractions;
using RzR.ResultMessage.Extensions.Result;
using Xunit;

namespace MConnectHelperDotNet.Tests.Edge
{
    public class MConnectApiServiceEdgeTests
    {
        public enum InjectedFailure
        {
            FactoryThrows,
            ResponseBodyIsNotXml,
            EndpointUrlIsNotAUri
        }

        private const string FaultText = "FAULT-X";
        private const string LeakPathText = "secret-path";
        private const string StackFramePrefix = "   at ";

        [Fact]
        public async Task SendRequestAsync_InnerSendFailsWithExceptionDetail_SurfacesOnlyConstantMessage()
        {
            var innerException = new Exception(LeakPathText +" C:\\x " + TestValues.Marker);
            var endpoint = new FakeEndpointClient
            {
                SendRequestHandler = _ => Result<HttpResponseMessage>
                    .Failure(DefaultMessages.InternalErrorProcessResponse)
                    .WithError(innerException)
            };
            var logger = new SpyLogger<MConnectApiService>();
            var sut = new MConnectApiService(_ => endpoint, new FakeRequestBuilderService { NextResult = HeaderSuccess(TestValues.BaseUrl) },
                logger);

            var result = await sut.SendRequestAsync(ValidRequest());

            var everything = EdgeText.Everything(result);
            Assert.False(result.IsSuccess);
            Assert.NotEqual(DefaultMessages.InternalErrorCallMConnectApi, DefaultMessages.InternalErrorProcessResponse);
            Assert.Equal(DefaultMessages.InternalErrorProcessResponse, result.GetFirstMessage());
            Assert.Equal(0, logger.ErrorCount);
            Assert.DoesNotContain(TestValues.Marker, everything);
            Assert.DoesNotContain(LeakPathText, everything);
            Assert.DoesNotContain(StackFramePrefix, everything);
            Assert.Equal(1, endpoint.SendRequestCount);
            Assert.Equal(0, endpoint.ProcessResponseBodyCount);
        }

        [Fact]
        public async Task SendRequestAsync_ResponseContainsSoapFault_ReturnsFaultValueWithoutProcessingBody()
        {
            var envelope = TestSoapEnvelopes.Fault(FaultText);
            var expectedFaultValue = XDocument.Parse(envelope)
                .Descendants(XName.Get("Fault", TestSoapEnvelopes.SoapNamespace))
                .First()
                .Value;
            Assert.Contains(FaultText, expectedFaultValue);

            using var response = new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(envelope) };
            var endpoint = new FakeEndpointClient
            {
                SendRequestHandler = _ => Result<HttpResponseMessage>.Success(response)
            };
            var sut = new MConnectApiService(_ => endpoint, new FakeRequestBuilderService { NextResult = HeaderSuccess(TestValues.BaseUrl) },
                new SpyLogger<MConnectApiService>());

            var result = await sut.SendRequestAsync(ValidRequest());

            Assert.False(result.IsSuccess);
            Assert.Equal(expectedFaultValue, result.GetFirstMessage());
            Assert.Equal(0, endpoint.ProcessResponseBodyCount);
        }

        [Theory]
        [InlineData(InjectedFailure.FactoryThrows)]
        [InlineData(InjectedFailure.ResponseBodyIsNotXml)]
        [InlineData(InjectedFailure.EndpointUrlIsNotAUri)]
        public async Task SendRequestAsync_InternalFailure_ReturnsConstantMessageWithoutLeakAndLogsOnce(InjectedFailure failure)
        {
            using var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("<<not-xml " + TestValues.Marker)
            };
            var endpoint = new FakeEndpointClient
            {
                SendRequestHandler = _ => Result<HttpResponseMessage>.Success(response)
            };
            var endpointUrl = failure == InjectedFailure.EndpointUrlIsNotAUri
                ? "not a valid uri " + TestValues.Marker
                : TestValues.BaseUrl;
            var logger = new SpyLogger<MConnectApiService>();

            IEndpointClient Resolve(EndpointType _)
            {
                if (failure == InjectedFailure.FactoryThrows)
                    throw new InvalidOperationException(TestValues.Marker);

                return endpoint;
            }

            var sut = new MConnectApiService(Resolve, new FakeRequestBuilderService { NextResult = HeaderSuccess(endpointUrl) }, logger);

            var result = await sut.SendRequestAsync(ValidRequest());

            Assert.False(result.IsSuccess);
            Assert.Equal(DefaultMessages.InternalErrorCallMConnectApi, result.GetFirstMessage());
            Assert.DoesNotContain(TestValues.Marker, EdgeText.Everything(result));
            Assert.Equal(1, logger.ErrorCount);
            Assert.Equal(0, endpoint.ProcessResponseBodyCount);
        }

        private static IResult<RequestBuilderDto> HeaderSuccess(string endpointUrl)
            => Result<RequestBuilderDto>.Success(new RequestBuilderDto
            {
                EndpointUrl = endpointUrl,
                ServiceCertificatePath = "unused-service.cer",
                CallingEntity = TestValues.CallingEntity,
                CallingUser = TestValues.CallingUser,
                CallBasis = TestValues.CallBasis,
                CallReason = TestValues.CallReason
            });

        private static MConnectRequestDto ValidRequest()
            => new MConnectRequestDto
            {
                RequestEndPointUrl = TestValues.BaseUrl,
                SoapAction = "x",
                Content = "<a/>",
                SignMessage = false,
                TimeOut = 5
            };
    }
}
