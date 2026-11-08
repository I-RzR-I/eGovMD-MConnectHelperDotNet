using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using MConnectHelperDotNet.Abstractions;
using MConnectHelperDotNet.Models.DTO;
using MConnectHelperDotNet.Models.DTO.Request.Base;
using Newtonsoft.Json.Linq;
using RzR.ResultMessage;
using RzR.ResultMessage.Abstractions;

namespace MConnectHelperDotNet.Tests.TestInfrastructure
{
    public sealed class FakeEndpointClient : IEndpointClient
    {
        private int _buildRequestCount;
        private int _sendRequestCount;
        private int _processResponseAsyncCount;
        private int _processResponseBodyCount;
        private int _getResponseBodyCount;

        public Func<RequestSettingDto, IResult<HttpRequestMessage>> BuildRequestHandler { get; set; } =
            _ => Result<HttpRequestMessage>.Success(new HttpRequestMessage(HttpMethod.Post, TestValues.BaseUrl));

        public Func<SendRequestDto, IResult<HttpResponseMessage>> SendRequestHandler { get; set; } =
            _ => Result<HttpResponseMessage>.Success(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(string.Empty) });

        public Func<ProcessResponseAsyncDto, IResult<JToken>> ProcessResponseAsyncHandler { get; set; } =
            _ => Result<JToken>.Success(new JObject());

        public Func<ProcessResponseBodyDto, IResult<JToken>> ProcessResponseBodyHandler { get; set; } =
            _ => Result<JToken>.Success(new JObject());

        public Func<GetResponseBodyDto, IResult<XmlNode>> GetResponseBodyHandler { get; set; } =
            _ => Result<XmlNode>.Success(new XmlDocument().CreateElement("response"));

        public int BuildRequestCount => Volatile.Read(ref _buildRequestCount);

        public int SendRequestCount => Volatile.Read(ref _sendRequestCount);

        public int ProcessResponseAsyncCount => Volatile.Read(ref _processResponseAsyncCount);

        public int ProcessResponseBodyCount => Volatile.Read(ref _processResponseBodyCount);

        public int GetResponseBodyCount => Volatile.Read(ref _getResponseBodyCount);

        public RequestSettingDto LastBuildRequest { get; private set; }

        public SendRequestDto LastSendRequest { get; private set; }

        public ProcessResponseBodyDto LastProcessResponseBody { get; private set; }

        public IResult<HttpRequestMessage> BuildRequest(RequestSettingDto requestSettings)
        {
            Interlocked.Increment(ref _buildRequestCount);
            LastBuildRequest = requestSettings;

            return BuildRequestHandler(requestSettings);
        }

        public Task<IResult<HttpResponseMessage>> SendRequestAsync(SendRequestDto request,
            CancellationToken cancellationToken = default)
        {
            Interlocked.Increment(ref _sendRequestCount);
            LastSendRequest = request;

            return Task.FromResult(SendRequestHandler(request));
        }

        public Task<IResult<JToken>> ProcessResponseAsync(ProcessResponseAsyncDto request,
            CancellationToken cancellationToken = default)
        {
            Interlocked.Increment(ref _processResponseAsyncCount);

            return Task.FromResult(ProcessResponseAsyncHandler(request));
        }

        public IResult<JToken> ProcessResponseBody(ProcessResponseBodyDto request)
        {
            Interlocked.Increment(ref _processResponseBodyCount);
            LastProcessResponseBody = request;

            return ProcessResponseBodyHandler(request);
        }

        public IResult<XmlNode> GetResponseBody(GetResponseBodyDto request)
        {
            Interlocked.Increment(ref _getResponseBodyCount);

            return GetResponseBodyHandler(request);
        }
    }
}
