using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace MConnectHelperDotNet.Tests.TestInfrastructure
{
    public sealed class StubHttpMessageHandler : HttpMessageHandler
    {
        private readonly Func<HttpRequestMessage, HttpResponseMessage> _respond;
        private int _callCount;

        public StubHttpMessageHandler(Func<HttpRequestMessage, HttpResponseMessage> respond)
        {
            _respond = respond ?? throw new ArgumentNullException(nameof(respond));
        }

        public HttpRequestMessage LastRequest { get; private set; }

        public int CallCount => Volatile.Read(ref _callCount);

        public static StubHttpMessageHandler Throwing(Exception exception)
        {
            if (exception == null) throw new ArgumentNullException(nameof(exception));

            return new StubHttpMessageHandler(_ => throw exception);
        }

        public static StubHttpMessageHandler Responding(string body, HttpStatusCode status = HttpStatusCode.OK)
        {
            return new StubHttpMessageHandler(_ => new HttpResponseMessage(status) { Content = new StringContent(body ?? string.Empty) });
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Interlocked.Increment(ref _callCount);
            LastRequest = request;

            try
            {
                return Task.FromResult(_respond(request));
            }
            catch (Exception exception)
            {
                return Task.FromException<HttpResponseMessage>(exception);
            }
        }
    }
}
