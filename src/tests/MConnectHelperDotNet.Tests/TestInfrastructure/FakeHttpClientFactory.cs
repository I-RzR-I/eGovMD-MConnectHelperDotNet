using System;
using System.Net.Http;

namespace MConnectHelperDotNet.Tests.TestInfrastructure
{
    public sealed class FakeHttpClientFactory : IHttpClientFactory
    {
        private readonly HttpMessageHandler _handler;

        public FakeHttpClientFactory(HttpMessageHandler handler)
        {
            _handler = handler ?? throw new ArgumentNullException(nameof(handler));
        }

        public int CreateCount { get; private set; }

        public string LastName { get; private set; }

        public HttpClient CreateClient(string name)
        {
            CreateCount++;
            LastName = name;

            return new HttpClient(_handler, false);
        }
    }
}
