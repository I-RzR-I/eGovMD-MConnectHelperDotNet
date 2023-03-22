// ***********************************************************************
//  Assembly         : RzR.Shared.eGovMD.MConnectHelperDotNet
//  Author           : RzR
//  Created On       : 2023-03-21 07:46
// 
//  Last Modified By : RzR
//  Last Modified On : 2023-03-21 07:54
// ***********************************************************************
//  <copyright file="Soap11Client.cs" company="">
//   Copyright (c) RzR. All rights reserved.
//  </copyright>
// 
//  <summary>
//  </summary>
// ***********************************************************************

#region U S A G E S

using System;
using System.Net.Http;
using AggregatedGenericResultMessage;
using AggregatedGenericResultMessage.Abstractions;
using AggregatedGenericResultMessage.Extensions.Result;
using DomainCommonExtensions.DataTypeExtensions;
using MConnectHelperDotNet.Helpers;
using MConnectHelperDotNet.Models.Client;
using MConnectHelperDotNet.Models.DTO;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

#endregion

namespace MConnectHelperDotNet.Configurations.Clients.Soap
{
    /// <summary>
    ///     SOAP11 client
    /// </summary>
    public sealed class Soap11Client : BaseSoapClient
    {
        /// <summary>
        ///     Logger
        /// </summary>
        private readonly ILogger<Soap11Client> _logger;

        /// <summary>
        ///     SOAP11 client
        /// </summary>
        /// <param name="clientFactory">Client factory</param>
        /// <param name="optionsAccessor">Option accessor</param>
        /// <param name="logger">Logger</param>
        public Soap11Client(
            IHttpClientFactory clientFactory, IOptionsMonitor<ClientCertificateOptions> optionsAccessor,
            ILogger<Soap11Client> logger) : base(
            clientFactory, optionsAccessor, "http://schemas.xmlsoap.org/soap/envelope/", "application/xml", logger)
            => _logger = logger;

        /// <inheritdoc />
        public override IResult<HttpRequestMessage> BuildRequest(RequestSettingDto requestSettings)
        {
            try
            {
                var httpRequestMessage = base.BuildRequest(requestSettings);
                if (!httpRequestMessage.IsSuccess)
                    return Result<HttpRequestMessage>.Failure(httpRequestMessage.GetFirstMessage());
                requestSettings.SoapAction =
                    "\"" + (requestSettings.SoapAction.IsNullOrEmpty() ? null : requestSettings.SoapAction) + "\"";
                httpRequestMessage.Response.Headers.Add("SOAPAction", requestSettings.SoapAction);

                return Result<HttpRequestMessage>.Success(httpRequestMessage.Response);
            }
            catch (Exception e)
            {
                _logger.LogError(e, DefaultMessages.InternalErrorBuildRequestString);

                return Result<HttpRequestMessage>.Failure(DefaultMessages.InternalErrorBuildRequestString)
                    .WithError(e);
            }
        }
    }
}