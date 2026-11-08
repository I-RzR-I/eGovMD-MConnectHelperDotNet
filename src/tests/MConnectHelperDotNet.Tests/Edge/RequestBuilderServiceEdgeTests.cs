using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MConnectHelperDotNet.Helpers;
using MConnectHelperDotNet.Models.DTO.Request;
using MConnectHelperDotNet.Services;
using MConnectHelperDotNet.Tests.TestInfrastructure;
using Microsoft.Extensions.Logging;
using RzR.ResultMessage.Enums;
using Xunit;

namespace MConnectHelperDotNet.Tests.Edge
{
    public class RequestBuilderServiceEdgeTests
    {
        public enum HeaderField
        {
            CallingEntity,
            CallingUser,
            CallBasis,
            CallReason
        }

        public enum RequiredValue
        {
            EndpointUrl,
            ServiceCertificatePath,
            CallingEntity,
            CallingUser,
            CallBasis,
            CallReason
        }

        private const string RequestValue = "REQ-VALUE";
        private const string CfgCertificatePath = "cfg-service.cer";
        private const string CfgEntity = "CFG-ENTITY";
        private const string CfgUser = "CFG-USER";
        private const string CfgBasis = "CFG-BASIS";
        private const string CfgReason = "CFG-REASON";

        public static IEnumerable<object[]> FallbackCases()
        {
            foreach (var field in (HeaderField[])Enum.GetValues(typeof(HeaderField)))
            {
                yield return new object[] { field, RequestValue, true };
                yield return new object[] { field, string.Empty, false };
                yield return new object[] { field, "   ", false };
                yield return new object[] { field, null, false };
            }
        }

        public static IEnumerable<object[]> MissingCases()
        {
            foreach (var value in (RequiredValue[])Enum.GetValues(typeof(RequiredValue)))
            {
                yield return new object[] { value, null };
                yield return new object[] { value, string.Empty };
                yield return new object[] { value, "   " };
            }
        }

        [Theory]
        [MemberData(nameof(FallbackCases))]
        public async Task GetHeaderRequestDataAsync_RequestFieldVariant_UsesRequestValueOnlyWhenNotBlank(
            HeaderField field, string requestValue, bool expectRequestValue)
        {
            var request = new GenerateRequestHeaderDto { RequestEndPointUrl = TestValues.BaseUrl };
            switch (field)
            {
                case HeaderField.CallingEntity: request.CallingEntityName = requestValue; break;
                case HeaderField.CallingUser: request.CallingUserIdentifierCode = requestValue; break;
                case HeaderField.CallBasis: request.CallBasis = requestValue; break;
                case HeaderField.CallReason: request.CallReason = requestValue; break;
            }

            var sut = new RequestBuilderService(BuildConfig(), new SpyLogger<RequestBuilderService>());

            var result = await sut.GetHeaderRequestDataAsync(request);

            Assert.True(result.IsSuccess);
            Assert.Equal(Pick(HeaderField.CallingEntity, field, requestValue, expectRequestValue, CfgEntity), result.Response.CallingEntity);
            Assert.Equal(Pick(HeaderField.CallingUser, field, requestValue, expectRequestValue, CfgUser), result.Response.CallingUser);
            Assert.Equal(Pick(HeaderField.CallBasis, field, requestValue, expectRequestValue, CfgBasis), result.Response.CallBasis);
            Assert.Equal(Pick(HeaderField.CallReason, field, requestValue, expectRequestValue, CfgReason), result.Response.CallReason);
        }

        [Theory]
        [MemberData(nameof(MissingCases))]
        public async Task GetHeaderRequestDataAsync_RequiredValueMissing_ReturnsMissingHeaderVariable(
            RequiredValue missing, string missingValue)
        {
            var request = new GenerateRequestHeaderDto
            {
                RequestEndPointUrl = missing == RequiredValue.EndpointUrl ? missingValue : TestValues.BaseUrl
            };
            var config = BuildConfig(
                missing == RequiredValue.ServiceCertificatePath ? missingValue : CfgCertificatePath,
                missing == RequiredValue.CallingEntity ? missingValue : CfgEntity,
                missing == RequiredValue.CallingUser ? missingValue : CfgUser,
                missing == RequiredValue.CallBasis ? missingValue : CfgBasis,
                missing == RequiredValue.CallReason ? missingValue : CfgReason);
            var sut = new RequestBuilderService(config, new SpyLogger<RequestBuilderService>());

            var result = await sut.GetHeaderRequestDataAsync(request);

            Assert.False(result.IsSuccess);
            Assert.Equal(DefaultMessages.MissingHeaderVariable, result.GetFirstMessage());
        }

        [Fact]
        public async Task GetHeaderRequestDataAsync_ConfigurationThrows_ReturnsConstantMessageWithoutExceptionText()
        {
            var config = BuildConfig();
            config.ThrowOnRead = true;
            var thrown = Assert.Throws<InvalidOperationException>(() => config["any:key"]);
            var thrownText = thrown.Message;
            Assert.StartsWith(FakeConfiguration.ThrowMarker, thrownText);

            var logger = new SpyLogger<RequestBuilderService>();
            var sut = new RequestBuilderService(config, logger);

            var result = await sut.GetHeaderRequestDataAsync(new GenerateRequestHeaderDto { RequestEndPointUrl = TestValues.BaseUrl });

            Assert.False(result.IsSuccess);
            Assert.Equal(DefaultMessages.InternalErrorOnGetMConnectHeader, result.GetFirstMessage());
            Assert.Equal(DefaultMessages.InternalErrorOnGetMConnectHeader, EdgeText.Info(result, 0));
            Assert.DoesNotContain(thrownText, result.GetFirstMessage());
            Assert.DoesNotContain(FakeConfiguration.ThrowMarker, result.GetFirstMessage());
            Assert.DoesNotContain(FakeConfiguration.ThrowMarker, EdgeText.FirstEntry(result));
            Assert.Equal(2, result.Messages.Count);
            Assert.Equal(MessageType.Exception, result.Messages.ElementAt(1).MessageType);
            Assert.Equal(1, logger.ErrorCount);

            var logged = Assert.Single(logger.Entries);
            Assert.Equal(LogLevel.Error, logged.Level);
            Assert.Equal(DefaultMessages.InternalErrorOnGetMConnectHeader, logged.Message);
            Assert.IsType<InvalidOperationException>(logged.Exception);
            Assert.StartsWith(FakeConfiguration.ThrowMarker, logged.Exception.Message);
        }

        private static string Pick(HeaderField target, HeaderField field, string requestValue, bool expectRequestValue, string configValue)
            => target == field && expectRequestValue ? requestValue : configValue;

        private static FakeConfiguration BuildConfig(
            string certificatePath = CfgCertificatePath,
            string callingEntity = CfgEntity,
            string callingUser = CfgUser,
            string callBasis = CfgBasis,
            string callReason = CfgReason)
        {
            var config = new FakeConfiguration();
            SetOrOmit(config, "MConnectOptions:ServiceCertificate:Path", certificatePath);
            SetOrOmit(config, "MConnectOptions:RequestHeaders:CallingEntity", callingEntity);
            SetOrOmit(config, "MConnectOptions:RequestHeaders:CallingUser", callingUser);
            SetOrOmit(config, "MConnectOptions:RequestHeaders:CallBasis", callBasis);
            SetOrOmit(config, "MConnectOptions:RequestHeaders:CallReason", callReason);

            return config;
        }

        private static void SetOrOmit(FakeConfiguration config, string key, string value)
        {
            if (value != null) config.Set(key, value);
        }
    }
}
