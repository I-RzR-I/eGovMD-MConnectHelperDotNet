// ***********************************************************************
//  Assembly         : RzR.Shared.eGovMD.MConnectHelperDotNet
//  Author           : RzR
//  Created On       : 2023-03-21 08:06
// 
//  Last Modified By : RzR
//  Last Modified On : 2023-03-21 16:46
// ***********************************************************************
//  <copyright file="RestClient.cs" company="">
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
using System.Xml;
using AggregatedGenericResultMessage;
using AggregatedGenericResultMessage.Abstractions;
using AggregatedGenericResultMessage.Extensions.Result;
using MConnectHelperDotNet.Helpers;
using MConnectHelperDotNet.Models.Client;
using MConnectHelperDotNet.Models.DTO;
using MConnectHelperDotNet.Models.DTO.Request.Base;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;

#endregion

// ReSharper disable NotAccessedField.Local

namespace MConnectHelperDotNet.Configurations.Clients.Rest
{
    /// <summary>
    ///     REST client
    /// </summary>
    public sealed class RestClient : BaseEndpointClient
    {
        /// <summary>
        ///     Logger
        /// </summary>
        private readonly ILogger<RestClient> _logger;

        /// <summary>
        ///     Rest client
        /// </summary>
        /// <param name="clientFactory">Client factory</param>
        /// <param name="optionsAccessor">Option accessor</param>
        /// <param name="logger">Logger</param>
        public RestClient(
            IHttpClientFactory clientFactory, IOptionsMonitor<ClientCertificateOptions> optionsAccessor,
            ILogger<RestClient> logger)
            : base(clientFactory, optionsAccessor, logger)
            => _logger = logger;

        /// <inheritdoc />
        public override IResult<HttpRequestMessage> BuildRequest(RequestSettingDto requestSettings)
        {
            var httpRequestMessage = new HttpRequestMessage { RequestUri = requestSettings.Uri, Method = requestSettings.Method };
            var headerFill = FillHeaders(new FillHeadersDto { RawHeaders = requestSettings.RequestHeaders, Headers = httpRequestMessage.Headers });
            if (!headerFill.IsSuccess) return Result<HttpRequestMessage>.Failure(headerFill.ToBase().GetFirstMessage());

            var content = requestSettings.Content;
            if (requestSettings.SignMessage)
            {
                try
                {
                    JToken.Parse(content);
                }
                catch (Exception ex)
                {
                    return Result<HttpRequestMessage>.Failure(DefaultMessages.InvalidJsonMessage)
                        .WithError(ex);
                }

                content = RestSignatureHelper.ApplySignature(Options, requestSettings.Content);
            }

            httpRequestMessage.Content = new StringContent(content, Encoding.UTF8, "application/json");
            var headerFill2 = FillHeaders(new FillHeadersDto { Headers = httpRequestMessage.Content.Headers, RawHeaders = requestSettings.ContentHeaders });
            
            return !headerFill2.IsSuccess 
                ? Result<HttpRequestMessage>.Failure(headerFill2.ToBase().GetFirstMessage()) 
                : Result<HttpRequestMessage>.Success(httpRequestMessage);
        }

        /// <inheritdoc />
        public override IResult<JToken> ProcessResponseBody(ProcessResponseBodyDto request)
        {
            try
            {
                if (request.ResponseSetting.IsMessageSigned)
                {
                    var validation = RestSignatureHelper.ValidateSignature(request.ResponseBody, request.ResponseSetting.ServiceCertificate);

                    return Result<JToken>.Success(JToken.Parse(validation.Response));
                }

                return Result<JToken>.Success(JToken.Parse(request.ResponseBody));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, DefaultMessages.InvalidJsonResponse);

                return Result<JToken>.Failure(DefaultMessages.InvalidJsonResponse);
            }
        }

        /// <inheritdoc />
        public override IResult<XmlNode> GetResponseBody(GetResponseBodyDto request) => throw new NotImplementedException();
    }
}