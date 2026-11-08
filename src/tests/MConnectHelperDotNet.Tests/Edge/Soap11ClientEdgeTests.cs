using System;
using System.Linq;
using System.Net.Http;
using MConnectHelperDotNet.Configurations.Clients.Soap;
using MConnectHelperDotNet.Helpers;
using MConnectHelperDotNet.Models.Client;
using MConnectHelperDotNet.Models.DTO;
using MConnectHelperDotNet.Models.DTO.Request.Base;
using MConnectHelperDotNet.Tests.TestInfrastructure;
using Xunit;

namespace MConnectHelperDotNet.Tests.Edge
{
    public sealed class Soap11ClientEdgeTests : IDisposable
    {
        public enum RejectedEnvelope
        {
            TwoBodies,
            NoBody,
            EmptyBody,
            UnsignedWhenValidationRequired
        }

        private const string ServiceBodyXml = "<m:Ping xmlns:m=\"urn:test\"><m:Value>1</m:Value></m:Ping>";

        private readonly TestCertificateFactory _certificates = new TestCertificateFactory();
        private readonly StubHttpMessageHandler _handler = StubHttpMessageHandler.Responding(string.Empty);

        public void Dispose()
        {
            _handler.Dispose();
            _certificates.Dispose();
        }

        [Theory]
        [InlineData(null, "\"\"")]
        [InlineData("", "\"\"")]
        [InlineData(" ", "\" \"")]
        public void BuildRequest_SoapActionNullOrBlank_WritesQuotedHeaderValue(string soapAction, string expectedHeader)
        {
            var sut = CreateSut(new SpyLogger<Soap11Client>());
            var settings = new RequestSettingDto
            {
                Method = HttpMethod.Post,
                Uri = new Uri(TestValues.BaseUrl),
                SoapAction = soapAction,
                Content = "<a/>",
                SignMessage = false
            };

            var result = sut.BuildRequest(settings);

            Assert.True(result.IsSuccess);
            using var request = result.Response;
            Assert.Equal(expectedHeader, request.Headers.GetValues("SOAPAction").Single());
        }

        [Fact]
        public void BuildRequest_SignedWithInvalidXmlBody_ReturnsConstantMessageWithoutLeak()
        {
            var logger = new SpyLogger<Soap11Client>();
            var sut = CreateSut(logger);
            var settings = new RequestSettingDto
            {
                Method = HttpMethod.Post,
                Uri = new Uri(TestValues.BaseUrl),
                SoapAction = "x",
                Content = "<unclosed " + TestValues.Marker,
                SignMessage = true
            };

            var result = sut.BuildRequest(settings);

            Assert.False(result.IsSuccess);
            Assert.Equal(DefaultMessages.InternalErrorBuildRequest, result.GetFirstMessage());
            Assert.DoesNotContain(TestValues.Marker, result.GetFirstMessage());
            Assert.Equal(1, logger.ErrorCount);
        }

        [Theory]
        [InlineData(RejectedEnvelope.TwoBodies, DefaultMessages.NoExistOrMoreThanOneBodyInResponse)]
        [InlineData(RejectedEnvelope.NoBody, DefaultMessages.NoExistOrMoreThanOneBodyInResponse)]
        [InlineData(RejectedEnvelope.EmptyBody, DefaultMessages.NoChildInBody)]
        [InlineData(RejectedEnvelope.UnsignedWhenValidationRequired, DefaultMessages.NoExistOrMoreThanOneSignature)]
        public void ProcessResponseBody_RejectedEnvelope_ReturnsMatchingConstant(RejectedEnvelope envelope, string expectedMessage)
        {
            var sut = CreateSut(new SpyLogger<Soap11Client>());
            var request = new ProcessResponseBodyDto
            {
                ResponseBody = Envelope(envelope),
                ResponseSetting = SettingFor(envelope)
            };

            var result = sut.ProcessResponseBody(request);

            Assert.False(result.IsSuccess);
            Assert.Equal(expectedMessage, result.GetFirstMessage());
        }

        [Theory]
        [InlineData(RejectedEnvelope.TwoBodies, DefaultMessages.NoExistOrMoreThanOneBodyInResponse)]
        [InlineData(RejectedEnvelope.NoBody, DefaultMessages.NoExistOrMoreThanOneBodyInResponse)]
        [InlineData(RejectedEnvelope.EmptyBody, DefaultMessages.NoChildInBody)]
        [InlineData(RejectedEnvelope.UnsignedWhenValidationRequired, DefaultMessages.NoExistOrMoreThanOneSignature)]
        public void GetResponseBody_RejectedEnvelope_ReturnsMatchingConstant(RejectedEnvelope envelope, string expectedMessage)
        {
            var sut = CreateSut(new SpyLogger<Soap11Client>());
            var request = new GetResponseBodyDto
            {
                ResponseBody = Envelope(envelope),
                ResponseSetting = SettingFor(envelope)
            };

            var result = sut.GetResponseBody(request);

            Assert.False(result.IsSuccess);
            Assert.Equal(expectedMessage, result.GetFirstMessage());
        }

        [Fact]
        public void ProcessResponseBody_MalformedXml_ReturnsInvalidSoapMessageWithExceptionEntry()
        {
            var logger = new SpyLogger<Soap11Client>();
            var sut = CreateSut(logger);
            var request = new ProcessResponseBodyDto
            {
                ResponseBody = TestSoapEnvelopes.Malformed(),
                ResponseSetting = new ResponseSettingDto()
            };

            var result = sut.ProcessResponseBody(request);

            Assert.False(result.IsSuccess);
            Assert.Equal(DefaultMessages.InvalidSoapMessageInResponse, result.GetFirstMessage());
            Assert.Equal(2, result.Messages.Count);
            Assert.Equal(1, logger.ErrorCount);
        }

        [Fact]
        public void GetResponseBody_MalformedXml_ReturnsInvalidSoapMessageWithExceptionEntry()
        {
            var logger = new SpyLogger<Soap11Client>();
            var sut = CreateSut(logger);
            var request = new GetResponseBodyDto
            {
                ResponseBody = TestSoapEnvelopes.Malformed(),
                ResponseSetting = new ResponseSettingDto()
            };

            var result = sut.GetResponseBody(request);

            Assert.False(result.IsSuccess);
            Assert.Equal(DefaultMessages.InvalidSoapMessageInResponse, result.GetFirstMessage());
            Assert.Equal(2, result.Messages.Count);
            Assert.Equal(1, logger.ErrorCount);
        }

        [Fact]
        public void ProcessResponseBody_SignedBodyTampered_ReturnsInvalidSoapSignature()
        {
            var serviceCertificate = _certificates.Create("MConnect-Test-Service");
            var tampered = TestSoapEnvelopes.Tamper(TestSoapEnvelopes.SignedViaLibrary(serviceCertificate, ServiceBodyXml));
            var sut = CreateSut(new SpyLogger<Soap11Client>());
            var request = new ProcessResponseBodyDto
            {
                ResponseBody = tampered,
                ResponseSetting = new ResponseSettingDto
                {
                    IsMessageSigned = true,
                    ValidateSignedMessage = true,
                    ServiceCertificate = serviceCertificate
                }
            };

            var result = sut.ProcessResponseBody(request);

            Assert.False(result.IsSuccess);
            Assert.Equal(DefaultMessages.InvalidSoapSignature, result.GetFirstMessage());
        }

        [Fact]
        public void GetResponseBody_SignedBodyTampered_ReturnsInvalidSoapSignature()
        {
            var serviceCertificate = _certificates.Create("MConnect-Test-Service");
            var tampered = TestSoapEnvelopes.Tamper(TestSoapEnvelopes.SignedViaLibrary(serviceCertificate, ServiceBodyXml));
            var sut = CreateSut(new SpyLogger<Soap11Client>());
            var request = new GetResponseBodyDto
            {
                ResponseBody = tampered,
                ResponseSetting = new ResponseSettingDto
                {
                    IsMessageSigned = true,
                    ValidateSignedMessage = true,
                    ServiceCertificate = serviceCertificate
                }
            };

            var result = sut.GetResponseBody(request);

            Assert.False(result.IsSuccess);
            Assert.Equal(DefaultMessages.InvalidSoapSignature, result.GetFirstMessage());
        }

        private static string Envelope(RejectedEnvelope envelope)
        {
            switch (envelope)
            {
                case RejectedEnvelope.TwoBodies: return TestSoapEnvelopes.TwoBodies();
                case RejectedEnvelope.NoBody: return TestSoapEnvelopes.NoBody();
                case RejectedEnvelope.EmptyBody: return TestSoapEnvelopes.EmptyBody();
                default: return TestSoapEnvelopes.Ok(ServiceBodyXml);
            }
        }

        private static ResponseSettingDto SettingFor(RejectedEnvelope envelope)
            => envelope == RejectedEnvelope.UnsignedWhenValidationRequired
                ? new ResponseSettingDto { IsMessageSigned = true, ValidateSignedMessage = true }
                : new ResponseSettingDto();

        private Soap11Client CreateSut(SpyLogger<Soap11Client> logger)
            => new Soap11Client(new FakeHttpClientFactory(_handler), FakeOptionsMonitor<ClientCertificateOptions>.ForClientCertificate(), logger);
    }
}
