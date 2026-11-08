using System;
using System.Linq;
using MConnectHelperDotNet.Abstractions;
using MConnectHelperDotNet.Abstractions.Services;
using MConnectHelperDotNet.Configurations.Clients.Rest;
using MConnectHelperDotNet.Configurations.Clients.Soap;
using MConnectHelperDotNet.Enums;
using MConnectHelperDotNet.Models.Client;
using MConnectHelperDotNet.Services;
using MConnectHelperDotNet.Tests.TestInfrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Xunit;

namespace MConnectHelperDotNet.Tests.Functional
{
    public sealed class DependencyInjectionTests : IDisposable
    {
        private readonly TestCertificateFactory _certificates = new TestCertificateFactory();

        public void Dispose() => _certificates.Dispose();

        [Fact]
        public void AddMConnect_RegistersServicesAndEndpointClientFactory()
        {
            var source = _certificates.Create();
            var pfxPath = _certificates.WritePfx(source, out var pfxSecret);
            var configuration = new FakeConfiguration()
                .Set("MConnectOptions:ClientCertificate:ClientCertificatePath", pfxPath)
                .Set("MConnectOptions:ClientCertificate:ClientCertificatePassword", pfxSecret)
                .Set("MConnectOptions:ServiceCertificate:Path", "service.cer")
                .Set("MConnectOptions:RequestHeaders:CallingEntity", TestValues.CallingEntity)
                .Set("MConnectOptions:RequestHeaders:CallingUser", TestValues.CallingUser)
                .Set("MConnectOptions:RequestHeaders:CallBasis", TestValues.CallBasis)
                .Set("MConnectOptions:RequestHeaders:CallReason", TestValues.CallReason);
            var services = new ServiceCollection();
            services.AddLogging();
            services.AddSingleton<IConfiguration>(configuration);

            services.AddMConnect(configuration);
            using var provider = services.BuildServiceProvider();
            using var scope = provider.CreateScope();
            var apiDescriptor = services.Single(descriptor => descriptor.ServiceType == typeof(IMConnectApiService));
            var apiService = scope.ServiceProvider.GetRequiredService<IMConnectApiService>();
            var requestBuilder = scope.ServiceProvider.GetRequiredService<IRequestBuilderService>();
            var clientFactory = provider.GetRequiredService<Func<EndpointType, IEndpointClient>>();
            using var loadedCertificate = provider.GetRequiredService<IOptionsMonitor<ClientCertificateOptions>>()
                .CurrentValue.ClientCertificate;

            Assert.Equal(ServiceLifetime.Scoped, apiDescriptor.Lifetime);
            Assert.IsType<MConnectApiService>(apiService);
            Assert.IsType<RequestBuilderService>(requestBuilder);
            Assert.IsType<Soap11Client>(clientFactory(EndpointType.SOAP11));
            Assert.IsType<Soap12Client>(clientFactory(EndpointType.SOAP12));
            Assert.IsType<RestClient>(clientFactory(EndpointType.REST));
            Assert.Throws<NotImplementedException>(() => clientFactory((EndpointType)999));
            Assert.NotNull(loadedCertificate);
            Assert.Equal(source.Thumbprint, loadedCertificate.Thumbprint);
        }
    }
}
