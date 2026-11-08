using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace MConnectHelperDotNet.Tests.TestInfrastructure
{
    public sealed class TestCertificateFactory : IDisposable
    {
        private const string PemBeginCertificate = "-----BEGIN CERTIFICATE-----";
        private const string PemEndCertificate = "-----END CERTIFICATE-----";
        private const int PemLineLength = 64;
        private const int KeySizeInBits = 2048;

        private readonly List<IDisposable> _owned = new List<IDisposable>();
        private bool _disposed;

        public TestCertificateFactory()
        {
            DirectoryPath = Path.Combine(Path.GetTempPath(), "mconnect-tests-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(DirectoryPath);
        }

        public string DirectoryPath { get; }

        public X509Certificate2 Create(string cn = "MConnect-Test")
        {
            ThrowIfDisposed();

            var transferSecret = RandomSecret();
            using var rsa = RSA.Create(KeySizeInBits);
            var request = new CertificateRequest("CN=" + cn, rsa, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
            var now = DateTimeOffset.UtcNow;
            using var selfSigned = request.CreateSelfSigned(now.AddDays(-1), now.AddDays(1));

            var flags = X509KeyStorageFlags.Exportable;
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                flags |= X509KeyStorageFlags.EphemeralKeySet;

            var loaded = new X509Certificate2(selfSigned.Export(X509ContentType.Pfx, transferSecret), transferSecret, flags);
            _owned.Add(loaded);

            return loaded;
        }

        public string WritePfx(X509Certificate2 certificate, out string password)
        {
            ThrowIfDisposed();

            password = RandomSecret();
            var path = Path.Combine(DirectoryPath, "client-" + Guid.NewGuid().ToString("N") + ".pfx");
            File.WriteAllBytes(path, certificate.Export(X509ContentType.Pfx, password));

            return path;
        }

        public string WriteCer(X509Certificate2 certificate)
        {
            ThrowIfDisposed();

            var path = Path.Combine(DirectoryPath, "service-" + Guid.NewGuid().ToString("N") + ".cer");
            File.WriteAllBytes(path, certificate.Export(X509ContentType.Cert));

            return path;
        }

        public string WritePemDirectory(X509Certificate2 certificate)
        {
            ThrowIfDisposed();

            var directory = Path.Combine(DirectoryPath, "pem-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(directory);

            var base64 = Convert.ToBase64String(certificate.RawData);
            var builder = new StringBuilder();
            builder.Append(PemBeginCertificate).Append('\n');
            for (var offset = 0; offset < base64.Length; offset += PemLineLength)
                builder.Append(base64, offset, Math.Min(PemLineLength, base64.Length - offset)).Append('\n');
            builder.Append(PemEndCertificate).Append('\n');

            File.WriteAllText(Path.Combine(directory, "tls.crt"), builder.ToString());

            return directory;
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;

            foreach (var item in _owned)
                item.Dispose();
            _owned.Clear();

            try
            {
                Directory.Delete(DirectoryPath, true);
            }
            catch (IOException)
            {
            }
            catch (UnauthorizedAccessException)
            {
            }
        }

        private static string RandomSecret() => Guid.NewGuid().ToString("N");

        private void ThrowIfDisposed()
        {
            if (_disposed) throw new ObjectDisposedException(nameof(TestCertificateFactory));
        }
    }
}
