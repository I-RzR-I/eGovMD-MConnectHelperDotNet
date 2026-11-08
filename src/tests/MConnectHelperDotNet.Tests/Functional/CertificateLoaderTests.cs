using System;
using MConnectHelperDotNet.Configurations.Certificate;
using MConnectHelperDotNet.Tests.TestInfrastructure;
using Xunit;

namespace MConnectHelperDotNet.Tests.Functional
{
    public sealed class CertificateLoaderTests : IDisposable
    {
        private readonly TestCertificateFactory _certificates = new TestCertificateFactory();

        public void Dispose() => _certificates.Dispose();

        [Fact]
        public void Public_DerCertificateFile_ReturnsCertificateWithSameThumbprint()
        {
            var source = _certificates.Create();
            var path = _certificates.WriteCer(source);

            using var loaded = CertificateLoader.Public(path);

            Assert.NotNull(loaded);
            Assert.Equal(source.Thumbprint, loaded.Thumbprint);
        }

        [Fact]
        public void Public_PemDirectoryWithSingleCertificate_ReturnsCertificateWithSameThumbprint()
        {
            var source = _certificates.Create();
            var directory = _certificates.WritePemDirectory(source);

            using var loaded = CertificateLoader.Public(directory);

            Assert.NotNull(loaded);
            Assert.Equal(source.Thumbprint, loaded.Thumbprint);
        }

        [Fact]
        public void Private_PasswordProtectedPfx_ReturnsCertificateWithSameThumbprintAndPrivateKey()
        {
            var source = _certificates.Create();
            var path = _certificates.WritePfx(source, out var pfxSecret);

            using var loaded = CertificateLoader.Private(path, pfxSecret);

            Assert.NotNull(loaded);
            Assert.Equal(source.Thumbprint, loaded.Thumbprint);
            Assert.True(loaded.HasPrivateKey);
        }
    }
}
