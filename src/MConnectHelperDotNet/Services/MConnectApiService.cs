// ***********************************************************************
//  Assembly         : RzR.Shared.eGovMD.MConnectHelperDotNet
//  Author           : RzR
//  Created On       : 2023-03-21 17:30
// 
//  Last Modified By : RzR
//  Last Modified On : 2023-03-21 20:10
// ***********************************************************************
//  <copyright file="MConnectApiService.cs" company="">
//   Copyright (c) RzR. All rights reserved.
//  </copyright>
// 
//  <summary>
//  </summary>
// ***********************************************************************

#region U S A G E S

using System;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using AggregatedGenericResultMessage;
using AggregatedGenericResultMessage.Abstractions;
using MConnectHelperDotNet.Abstractions;
using MConnectHelperDotNet.Abstractions.Services;
using MConnectHelperDotNet.Enums;
using MConnectHelperDotNet.Helpers;
using MConnectHelperDotNet.Models.DTO;
using MConnectHelperDotNet.Models.DTO.Request;
using MConnectHelperDotNet.Models.DTO.Request.Base;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

#endregion

namespace MConnectHelperDotNet.Services
{
    /// <inheritdoc cref="IMConnectApiService" />
    public sealed class MConnectApiService : IMConnectApiService
    {
        /// <summary>
        ///     Client factory
        /// </summary>
        private readonly Func<EndpointType, IEndpointClient> _clientFactory;
        
        /// <summary>
        ///     Application logger
        /// </summary>
        private readonly ILogger<MConnectApiService> _logger;
        
        /// <summary>
        ///     Header request builder service
        /// </summary>
        private readonly IRequestBuilderService _requestBuilderService;

        /// <summary>
        ///     Initialize MConnect API service
        /// </summary>
        /// <param name="clientFactory">Client factory</param>
        /// <param name="requestBuilderService">Header request builder service</param>
        /// <param name="logger">Logger</param>
        public MConnectApiService(Func<EndpointType, IEndpointClient> clientFactory,
            IRequestBuilderService requestBuilderService, ILogger<MConnectApiService> logger)
        {
            _clientFactory = clientFactory;
            _requestBuilderService = requestBuilderService;
            _logger = logger;
        }

        /// <inheritdoc />
        public async Task<IResult<string>> SendRequestAsync(MConnectRequestDto request,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var endpointClient = _clientFactory(EndpointType.SOAP11);
                var requestHeader = await _requestBuilderService.GetHeaderRequestDataAsync(
                    new GenerateRequestHeaderDto { CallingUserIdentifierCode = request.CallingUserIdentifierCode, RequestEndPointUrl = request.RequestEndPointUrl }, cancellationToken);
                if (!requestHeader.IsSuccess)
                    return Result<string>.Failure(requestHeader.GetFirstMessage());

                var requestApi = endpointClient.BuildRequest(new RequestSettingDto
                {
                    Uri = new Uri(requestHeader.Response.EndpointUrl),
                    RequestHeaders = requestHeader.Response.FormatToString(),
                    SoapAction = request.SoapAction,
                    ContentHeaders = request.ContentHeaders,
                    Content = request.Content,
                    SignMessage = request.SignMessage
                });
                if (!requestApi.IsSuccess)
                    return Result<string>.Failure(requestApi.GetFirstMessage());

                var httpResponse = await endpointClient.SendRequestAsync(new SendRequestDto { TimeOut = request.TimeOut, Request = requestApi.Response }, cancellationToken);
                if (!httpResponse.IsSuccess)
                    return Result<string>.Failure(httpResponse.GetFirstMessage());

                var response = await httpResponse.Response.Content.ReadAsStringAsync();
                var doc = XDocument.Parse(response);
                XNamespace xmlns = "http://schemas.xmlsoap.org/soap/envelope/";
                var orderNode = doc.Descendants(xmlns + "Fault");

                var faultError = "";
                foreach (var element in orderNode)
                {
                    if (element.Name.LocalName != "Fault") continue;
                    faultError = element.Value;
                    break;
                }

                if (!string.IsNullOrWhiteSpace(faultError))
                    return Result<string>.Failure(faultError);

                var json = endpointClient.ProcessResponseBody(new ProcessResponseBodyDto
                {
                    ResponseBody = response,
                    ResponseSetting = new ResponseSettingDto
                    {
                        IsMessageSigned = request.SignMessage,
                        ValidateSignedMessage = request.ValidateSignedMessage,
                        ServiceCertificate = new X509Certificate2(requestHeader.Response.ServiceCertificatePath)
                    }
                });
                if (!json.IsSuccess)
                    return Result<string>.Failure(json.GetFirstMessage());

                var responseBody = JsonConvert.SerializeObject(json.Response, Formatting.Indented);

                return Result<string>.Success(responseBody);
            }
            catch (Exception e)
            {
                _logger.LogError(e, DefaultMessages.InternalErrorCallMConnectApi);

                return Result<string>.Failure(DefaultMessages.InternalErrorCallMConnectApi);
            }
        }
    }
}