using System;
using System.Security.Cryptography.X509Certificates;
using MConnectHelperDotNet.Configurations.Certificate;
using MConnectHelperDotNet.Tests.TestInfrastructure;
using Xunit;

namespace MConnectHelperDotNet.Tests.Functional
{
    public sealed class CertificateConverterTests : IDisposable
    {
        private readonly TestCertificateFactory _certificates = new TestCertificateFactory();

        public void Dispose() => _certificates.Dispose();

        [Fact]
        public void ConvertFrom_PathOnly_LoadsPublicCertificate()
        {
            var source = _certificates.Create();
            var path = _certificates.WriteCer(source);
            var converter = new CertificateConverter();

            using var loaded = Assert.IsType<X509Certificate2>(converter.ConvertFrom(path));

            Assert.Equal(source.Thumbprint, loaded.Thumbprint);
        }

        [Fact]
        public void ConvertFrom_PathPipePassword_LoadsPrivateCertificate()
        {
            var source = _certificates.Create();
            var path = _certificates.WritePfx(source, out var pfxSecret);
            var input = TestValues.JoinPathAndPassword(path, pfxSecret);
            var converter = new CertificateConverter();

            using var loaded = Assert.IsType<X509Certificate2>(converter.ConvertFrom(input));

            Assert.Equal(source.Thumbprint, loaded.Thumbprint);
            Assert.True(loaded.HasPrivateKey);
        }
    }
}
