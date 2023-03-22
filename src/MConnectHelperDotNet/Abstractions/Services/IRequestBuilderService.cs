// ***********************************************************************
//  Assembly         : RzR.Shared.eGovMD.MConnectHelperDotNet
//  Author           : RzR
//  Created On       : 2023-03-20 19:50
// 
//  Last Modified By : RzR
//  Last Modified On : 2023-03-20 21:22
// ***********************************************************************
//  <copyright file="IRequestBuilderService.cs" company="">
//   Copyright (c) RzR. All rights reserved.
//  </copyright>
// 
//  <summary>
//  </summary>
// ***********************************************************************

#region U S A G E S

using System.Threading;
using System.Threading.Tasks;
using AggregatedGenericResultMessage.Abstractions;
using MConnectHelperDotNet.Models.DTO;
using MConnectHelperDotNet.Models.DTO.Request;

#endregion

namespace MConnectHelperDotNet.Abstractions.Services
{
    /// <summary>
    ///     Header request builder service
    /// </summary>
    public interface IRequestBuilderService
    {
        /// <summary>
        ///     Build request header data for MConnect calling API
        /// </summary>
        /// <param name="request">Input request data</param>
        /// <param name="cancellationToken">Optional. The default value is default.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        Task<IResult<RequestBuilderDto>> GetHeaderRequestDataAsync(GenerateRequestHeaderDto request, 
            CancellationToken cancellationToken = default);
    }
}