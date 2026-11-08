using System;
using System.IO;
using MConnectHelperDotNet.Configurations.Certificate;
using MConnectHelperDotNet.Tests.TestInfrastructure;
using Xunit;

namespace MConnectHelperDotNet.Tests.Edge
{
    public sealed class CertificateLoaderEdgeTests : IDisposable
    {
        private readonly TestCertificateFactory _certificates = new TestCertificateFactory();

        public void Dispose() => _certificates.Dispose();

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Public_NullOrBlankPath_ReturnsNull(string path)
        {
            using var result = CertificateLoader.Public(path);

            Assert.Null(result);
        }

        [Fact]
        public void Public_DirectoryWithoutTlsCrt_ReturnsNull()
        {
            using var result = CertificateLoader.Public(_certificates.DirectoryPath);

            Assert.Null(result);
        }

        [Fact]
        public void Public_PathThatDoesNotExist_ReturnsNull()
        {
            var missing = Path.Combine(_certificates.DirectoryPath, "missing-service.cer");

            using var result = CertificateLoader.Public(missing);

            Assert.Null(result);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("   ")]
        public void Private_NullOrBlankPath_ReturnsNull(string path)
        {
            using var result = CertificateLoader.Private(path, TestValues.Marker);

            Assert.Null(result);
        }
    }
}
