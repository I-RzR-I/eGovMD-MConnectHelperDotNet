using System;
using System.IO;
using System.Threading.Tasks;
using MConnectHelperDotNet.Services;
using MConnectHelperDotNet.Tests.TestInfrastructure;
using Xunit;

namespace MConnectHelperDotNet.Tests.Functional
{
    public class RequestBuilderServiceTests
    {
        private const string ConfiguredCertificatePath = "certs/service.cer";

        [Fact]
        public async Task GetHeaderRequestDataAsync_AllConfigPresentAndRequestFieldsNull_UsesConfigValues()
        {
            var configuration = new FakeConfiguration()
                .Set("MConnectOptions:ServiceCertificate:Path", ConfiguredCertificatePath)
                .Set("MConnectOptions:RequestHeaders:CallingEntity", TestValues.CallingEntity)
                .Set("MConnectOptions:RequestHeaders:CallingUser", TestValues.CallingUser)
                .Set("MConnectOptions:RequestHeaders:CallBasis", TestValues.CallBasis)
                .Set("MConnectOptions:RequestHeaders:CallReason", TestValues.CallReason);
            var service = new RequestBuilderService(configuration, new SpyLogger<RequestBuilderService>());
            var request = TestRequests.GenerateRequestHeader();

            var result = await service.GetHeaderRequestDataAsync(request);

            Assert.True(result.IsSuccess);
            Assert.Equal(TestValues.CallingEntity, result.Response.CallingEntity);
            Assert.Equal(TestValues.CallingUser, result.Response.CallingUser);
            Assert.Equal(TestValues.CallBasis, result.Response.CallBasis);
            Assert.Equal(TestValues.CallReason, result.Response.CallReason);
            Assert.Equal(request.RequestEndPointUrl, result.Response.EndpointUrl);
            Assert.Equal(Path.Combine(AppContext.BaseDirectory, ConfiguredCertificatePath),
                result.Response.ServiceCertificatePath);
        }
    }
}
