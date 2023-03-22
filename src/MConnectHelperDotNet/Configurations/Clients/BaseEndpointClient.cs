// ***********************************************************************
//  Assembly         : RzR.Shared.eGovMD.MConnectHelperDotNet
//  Author           : RzR
//  Created On       : 2023-03-20 19:39
// 
//  Last Modified By : RzR
//  Last Modified On : 2023-03-20 23:01
// ***********************************************************************
//  <copyright file="BaseEndpointClient.cs" company="">
//   Copyright (c) RzR. All rights reserved.
//  </copyright>
// 
//  <summary>
//  </summary>
// ***********************************************************************

#region U S A G E S

using System;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using AggregatedGenericResultMessage;
using AggregatedGenericResultMessage.Abstractions;
using AggregatedGenericResultMessage.Extensions.Result;
using MConnectHelperDotNet.Abstractions;
using MConnectHelperDotNet.Helpers;
using MConnectHelperDotNet.Models.Client;
using MConnectHelperDotNet.Models.DTO;
using MConnectHelperDotNet.Models.DTO.Request.Base;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;

#endregion

namespace MConnectHelperDotNet.Configurations.Clients
{
    /// <inheritdoc cref="IEndpointClient" />
    public abstract class BaseEndpointClient : IEndpointClient
    {
        /// <summary>
        ///     HTTP client factory
        /// </summary>
        private readonly IHttpClientFactory _clientFactory;

        /// <summary>
        ///     Logger
        /// </summary>
        private readonly ILogger<BaseEndpointClient> _logger;

        /// <summary>
        ///     Client certificate option
        /// </summary>
        protected ClientCertificateOptions Options;
        
        /// <summary>
        ///     Base endpoint client
        /// </summary>
        /// <param name="clientFactory">Client factory</param>
        /// <param name="optionsAccessor">Option accessor</param>
        /// <param name="logger">Logger</param>
        protected BaseEndpointClient(IHttpClientFactory clientFactory, IOptionsMonitor<ClientCertificateOptions> optionsAccessor, 
            ILogger<BaseEndpointClient> logger)
        {
            _clientFactory = clientFactory;
            _logger = logger;
            Options = optionsAccessor.CurrentValue;
            optionsAccessor.OnChange((options, name) => Options = options);
        }

        /// <inheritdoc />
        public abstract IResult<HttpRequestMessage> BuildRequest(RequestSettingDto requestSettings);

        /// <inheritdoc />
        public async Task<IResult<HttpResponseMessage>> SendRequestAsync(SendRequestDto request, 
            CancellationToken cancellationToken = default)
        {
            try
            {
                var client = _clientFactory.CreateClient();
                client.Timeout = TimeSpan.FromSeconds(request.TimeOut);

                var sendRequest = await client.SendAsync(request.Request, cancellationToken);

                return Result<HttpResponseMessage>.Success(sendRequest);
            }
            catch (Exception e)
            {
                _logger.LogError(e, DefaultMessages.InternalErrorCallMConnectApi);

                return Result<HttpResponseMessage>.Failure(DefaultMessages.InternalErrorCallMConnectApi)
                    .WithError(e);
            }
        }

        /// <inheritdoc />
        public async Task<IResult<JToken>> ProcessResponseAsync(ProcessResponseAsyncDto request, 
            CancellationToken cancellationToken = default)
        {
            try
            {
                var responseBody = await request.Response.Content.ReadAsStringAsync();

                var processResult = ProcessResponseBody(new ProcessResponseBodyDto {ResponseBody = responseBody, ResponseSetting = request.ResponseSetting});

                return processResult;
            }
            catch (Exception e)
            {
                _logger.LogError(e, DefaultMessages.InternalErrorProcessResponse);

                return Result<JToken>.Failure(DefaultMessages.InternalErrorProcessResponse)
                    .WithError(e);
            }
        }

        /// <inheritdoc />
        public abstract IResult<JToken> ProcessResponseBody(ProcessResponseBodyDto request);

        /// <inheritdoc />
        public abstract IResult<XmlNode> GetResponseBody(GetResponseBodyDto request);

        /// <summary>
        ///     Fill header
        /// </summary>
        /// <param name="request">Request header data</param>
        /// <remarks></remarks>
        protected static IResult FillHeaders(FillHeadersDto request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.RawHeaders)) return Result.Success();

                foreach (var rawHeader in request.RawHeaders.Split(new[] {Environment.NewLine},
                    StringSplitOptions.RemoveEmptyEntries))
                {
                    var headerText =
                        Encoding.ASCII.GetString(
                            Encoding.ASCII.GetBytes(rawHeader));
                    var indexOfColon = headerText.IndexOf(':');
                    if (indexOfColon <= 0) continue;
                    request.Headers.TryAddWithoutValidation(headerText.Substring(0, indexOfColon).Trim(),
                        headerText.Substring(indexOfColon + 1).Trim());
                }

                return Result.Success();
            }
            catch (Exception e)
            {
                return Result.Failure(DefaultMessages.InternalErrorOnFillRequestHeaders)
                    .WithError(e);
            }
        }
    }
}