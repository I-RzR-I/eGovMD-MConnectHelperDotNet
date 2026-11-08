using System;
using System.Globalization;
using MConnectHelperDotNet.Configurations.Certificate;
using Xunit;

namespace MConnectHelperDotNet.Tests.Edge
{
    public class CertificateConverterEdgeTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void ConvertFrom_NullOrBlankText_ThrowsNotSupportedException(string text)
        {
            var sut = new CertificateConverter();

            Assert.Throws<NotSupportedException>(() => sut.ConvertFrom(null, CultureInfo.InvariantCulture, text));
        }
    }
}
