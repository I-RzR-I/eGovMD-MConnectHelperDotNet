using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Security;
using MConnectHelperDotNet.Configurations.Clients.Soap;
using MConnectHelperDotNet.Models.Client;

namespace MConnectHelperDotNet.Tests.TestInfrastructure
{
    public static class TestSoapEnvelopes
    {
        public const string SoapNamespace = "http://schemas.xmlsoap.org/soap/envelope/";

        private const string EnvelopeStart = "<soap:Envelope xmlns:soap=\"" + SoapNamespace + "\">";
        private const string EnvelopeEnd = "</soap:Envelope>";

        public static string Ok(string bodyXml) => EnvelopeStart + "<soap:Body>" + bodyXml + "</soap:Body>" + EnvelopeEnd;

        public static string Fault(string text) =>
            Ok("<soap:Fault><faultstring>" + SecurityElement.Escape(text) + "</faultstring></soap:Fault>");

        public static string EmptyBody() => EnvelopeStart + "<soap:Body/>" + EnvelopeEnd;

        public static string TwoBodies() =>
            EnvelopeStart + "<soap:Body><First/></soap:Body><soap:Body><Second/></soap:Body>" + EnvelopeEnd;

        public static string NoBody() => EnvelopeStart + "<soap:Header/>" + EnvelopeEnd;

        public static string Malformed() => EnvelopeStart + "<soap:Body><Open>";

        public static string SignedViaLibrary(X509Certificate2 clientCertificate, string bodyXml)
        {
            using var handler = StubHttpMessageHandler.Responding(string.Empty);
            var client = new Soap11Client(
                new FakeHttpClientFactory(handler),
                FakeOptionsMonitor<ClientCertificateOptions>.ForClientCertificate(clientCertificate),
                new SpyLogger<Soap11Client>());

            var settings = TestRequests.RequestSettings(bodyXml, true);
            var built = client.BuildRequest(settings);
            if (!built.IsSuccess)
                throw new InvalidOperationException("The library could not build the signed request: " + built.GetFirstMessage());

            using var request = built.Response;
            using var stream = request.Content.ReadAsStream();
            using var reader = new StreamReader(stream);

            return reader.ReadToEnd();
        }

        public static string Tamper(string signedEnvelope)
        {
            var bodyStart = signedEnvelope.IndexOf("<soap:Body", StringComparison.Ordinal);
            if (bodyStart < 0)
                throw new InvalidOperationException("The envelope has no Body.");

            var inTag = false;
            for (var index = signedEnvelope.IndexOf('>', bodyStart) + 1; index < signedEnvelope.Length; index++)
            {
                var current = signedEnvelope[index];
                if (current == '<') inTag = true;
                else if (current == '>') inTag = false;
                else if (!inTag && char.IsLetterOrDigit(current))
                {
                    var replacement = current == 'x' ? 'y' : 'x';

                    return signedEnvelope.Substring(0, index) + replacement + signedEnvelope.Substring(index + 1);
                }
            }

            throw new InvalidOperationException("The signed Body has no text to change.");
        }
    }
}
