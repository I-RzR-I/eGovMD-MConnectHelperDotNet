// ***********************************************************************
//  Assembly         : RzR.Shared.eGovMD.MConnectHelperDotNet
//  Author           : RzR
//  Created On       : 2023-03-21 00:04
// 
//  Last Modified By : RzR
//  Last Modified On : 2023-03-21 06:53
// ***********************************************************************
//  <copyright file="BaseSoapClient.cs" company="">
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
using MConnectHelperDotNet.Models.DTO.SoapXmlSignature;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

#endregion

namespace MConnectHelperDotNet.Configurations.Clients.Soap
{
    /// <summary>
    ///     SOAP client
    /// </summary>
    public abstract class BaseSoapClient : BaseEndpointClient
    {
        /// <summary>
        ///     Content type
        /// </summary>
        private readonly string _contentType;

        /// <summary>
        ///     Logger
        /// </summary>
        private readonly ILogger<BaseSoapClient> _logger;

        /// <summary>
        ///     SOAP namespace
        /// </summary>
        private readonly string _soapNamespace;

        /// <summary>
        ///     SOAP client
        /// </summary>
        /// <param name="clientFactory">HTTP client factory</param>
        /// <param name="optionsAccessor">Option accessor</param>
        /// <param name="soapNamespace">Call namespace</param>
        /// <param name="contentType">Call content type</param>
        /// <param name="logger">Logger</param>
        protected BaseSoapClient(
            IHttpClientFactory clientFactory,
            IOptionsMonitor<ClientCertificateOptions> optionsAccessor, string soapNamespace, string contentType,
            ILogger<BaseSoapClient> logger)
            : base(clientFactory, optionsAccessor, logger)
        {
            _soapNamespace = soapNamespace;
            _contentType = contentType;
            _logger = logger;
        }

        /// <inheritdoc />
        public override IResult<HttpRequestMessage> BuildRequest(RequestSettingDto requestSettings)
        {
            try
            {
                var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, new Uri(requestSettings.Uri.ToString()));
                FillHeaders(new FillHeadersDto { Headers = httpRequestMessage.Headers, RawHeaders = requestSettings.RequestHeaders });

                var xml = SoapXmlSignatureHelper.CreateXmlMessage(new CreateXmlMessageDto
                {
                    SignMessage = requestSettings.SignMessage,
                    SoapNamespace = _soapNamespace,
                    Options = Options,
                    RequestBody = requestSettings.Content
                });
                httpRequestMessage.Content = new StringContent(xml.Response, Encoding.UTF8, _contentType);

                FillHeaders(new FillHeadersDto { Headers = httpRequestMessage.Content.Headers, RawHeaders = requestSettings.ContentHeaders });

                return Result<HttpRequestMessage>.Success(httpRequestMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, DefaultMessages.InternalErrorBuildRequest);

                return Result<HttpRequestMessage>.Failure(DefaultMessages.InternalErrorBuildRequest)
                    .WithError(ex);
            }
        }

        /// <inheritdoc />
        public override IResult<JToken> ProcessResponseBody(ProcessResponseBodyDto request)
        {
            try
            {
                var xmlDocument = new XmlDocument { PreserveWhitespace = true };
                xmlDocument.LoadXml(request.ResponseBody);
                var bodyNodes = xmlDocument.GetElementsByTagName("Body", _soapNamespace);
                if (bodyNodes.Count != 1)
                    return Result<JToken>.Failure(DefaultMessages.NoExistOrMoreThanOneBodyInResponse);

                var body = bodyNodes[0];
                if (body.FirstChild == null)
                    return Result<JToken>.Failure(DefaultMessages.NoChildInBody);
                if (request.ResponseSetting.IsMessageSigned.Equals(true) && request.ResponseSetting.ValidateSignedMessage.Equals(true))
                {
                    var validation =
                        SoapXmlSignatureHelper.ValidateSignature(xmlDocument, request.ResponseSetting.ServiceCertificate);
                    if (!validation.IsSuccess)
                        return Result<JToken>.Failure(validation.ToBase().GetFirstMessage());
                }

                var obj = JObject.Parse(JsonConvert.SerializeXmlNode(body.FirstChild));

                return Result<JToken>.Success(obj);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, DefaultMessages.InvalidSoapMessageInResponse);

                return Result<JToken>.Failure(DefaultMessages.InvalidSoapMessageInResponse)
                    .WithError(ex);
            }
        }

        /// <inheritdoc />
        public override IResult<XmlNode> GetResponseBody(GetResponseBodyDto request)
        {
            try
            {
                var xmlDocument = new XmlDocument { PreserveWhitespace = true };
                xmlDocument.LoadXml(request.ResponseBody);
                var bodyNodes = xmlDocument.GetElementsByTagName("Body", _soapNamespace);
                if (bodyNodes.Count != 1)
                    return Result<XmlNode>.Failure(DefaultMessages.NoExistOrMoreThanOneBodyInResponse);

                var body = bodyNodes[0];
                if (body.FirstChild == null)
                    return Result<XmlNode>.Failure(DefaultMessages.NoChildInBody);
                if (request.ResponseSetting.IsMessageSigned.Equals(true) && request.ResponseSetting.ValidateSignedMessage.Equals(true))
                {
                    var validation =
                        SoapXmlSignatureHelper.ValidateSignature(xmlDocument, request.ResponseSetting.ServiceCertificate);
                    if (!validation.IsSuccess)
                        return Result<XmlNode>.Failure(validation.ToBase().GetFirstMessage());
                }

                return Result<XmlNode>.Success(body.FirstChild); /*return JObject.Parse(JsonConvert.SerializeXmlNode(body.FirstChild));*/
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, DefaultMessages.InvalidSoapMessageInResponse);

                return Result<XmlNode>.Failure(DefaultMessages.InvalidSoapMessageInResponse)
                    .WithError(ex);
            }
        }
    }
}