// ***********************************************************************
//  Assembly         : RzR.Shared.eGovMD.MConnectHelperDotNet
//  Author           : RzR
//  Created On       : 2023-03-20 20:09
// 
//  Last Modified By : RzR
//  Last Modified On : 2023-03-21 17:08
// ***********************************************************************
//  <copyright file="RequestBuilderService.cs" company="">
//   Copyright (c) RzR. All rights reserved.
//  </copyright>
// 
//  <summary>
//  </summary>
// ***********************************************************************

#region U S A G E S

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using AggregatedGenericResultMessage;
using AggregatedGenericResultMessage.Abstractions;
using AggregatedGenericResultMessage.Extensions.Result;
using DomainCommonExtensions.DataTypeExtensions;
using MConnectHelperDotNet.Abstractions.Services;
using MConnectHelperDotNet.Helpers;
using MConnectHelperDotNet.Models.DTO;
using MConnectHelperDotNet.Models.DTO.Request;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

#endregion

namespace MConnectHelperDotNet.Services
{
    /// <inheritdoc cref="IRequestBuilderService" />
    public class RequestBuilderService : IRequestBuilderService
    {
        /// <summary>
        ///     Configuration
        /// </summary>
        private readonly IConfiguration _configuration;

        /// <summary>
        ///     Logger
        /// </summary>
        private readonly ILogger<RequestBuilderService> _logger;

        /// <summary>
        ///     Header request builder service
        /// </summary>
        /// <param name="configuration">Application configuration</param>
        /// <param name="logger">Application logger</param>
        public RequestBuilderService(IConfiguration configuration, ILogger<RequestBuilderService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        /// <inheritdoc />
        public async Task<IResult<RequestBuilderDto>> GetHeaderRequestDataAsync(GenerateRequestHeaderDto request, CancellationToken cancellationToken = default)
        {
            try
            {
                var serviceCertificatePath = _configuration["MConnectOptions:ServiceCertificate:Path"];
                var callingEntity = _configuration["MConnectOptions:RequestHeaders:CallingEntity"];
                var callingUser = request.CallingUserIdentifierCode.IsNullOrEmpty() || string.IsNullOrWhiteSpace(request.CallingUserIdentifierCode)
                    ? _configuration["MConnectOptions:RequestHeaders:CallingUser"]
                    : request.CallingUserIdentifierCode;
                var callBasis = _configuration["MConnectOptions:RequestHeaders:CallBasis"];
                var callReason = _configuration["MConnectOptions:RequestHeaders:CallReason"];

                if (string.IsNullOrWhiteSpace(request.RequestEndPointUrl) ||
                    string.IsNullOrWhiteSpace(serviceCertificatePath) || string.IsNullOrWhiteSpace(callingEntity) ||
                    string.IsNullOrWhiteSpace(callingUser) || string.IsNullOrWhiteSpace(callBasis) ||
                    string.IsNullOrWhiteSpace(callReason)
                )
                {
                    return await Task.FromResult(
                        Result<RequestBuilderDto>.Failure(DefaultMessages.MissingHeaderVariable));
                }

                var result = new RequestBuilderDto
                {
                    CallBasis = callBasis,
                    CallReason = callReason,
                    CallingEntity = callingEntity,
                    CallingUser = callingUser,
                    EndpointUrl = request.RequestEndPointUrl,
                    ServiceCertificatePath = Path.Combine(AppContext.BaseDirectory, serviceCertificatePath)
                };

                return await Task.FromResult(Result<RequestBuilderDto>.Success(result));
            }
            catch (Exception e)
            {
                const string message = DefaultMessages.InternalErrorOnGetMConnectHeader;

                _logger.LogError(e, message);

                return Result<RequestBuilderDto>.Failure(message)
                    .WithError(e);
            }
        }
    }
}