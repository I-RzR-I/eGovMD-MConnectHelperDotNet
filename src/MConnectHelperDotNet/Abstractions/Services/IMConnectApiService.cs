// ***********************************************************************
//  Assembly         : RzR.Shared.eGovMD.MConnectHelperDotNet
//  Author           : RzR
//  Created On       : 2023-03-21 17:30
// 
//  Last Modified By : RzR
//  Last Modified On : 2023-03-21 20:08
// ***********************************************************************
//  <copyright file="IMConnectApiService.cs" company="">
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
using MConnectHelperDotNet.Enums;
using MConnectHelperDotNet.Models.DTO.Request;

#endregion

namespace MConnectHelperDotNet.Abstractions.Services
{
    /// <summary>
    ///     MConnect API service
    /// </summary>
    public interface IMConnectApiService
    {
        /// <summary>
        ///     Send request
        /// </summary>
        /// <param name="request">Required. Request data</param>
        /// <param name="cancellationToken">Optional. The default value is default.</param>
        /// <returns>Return response body as JSON</returns>
        /// <remarks></remarks>
        Task<IResult<string>> SendRequestAsync(MConnectRequestDto request, 
            CancellationToken cancellationToken = default);
    }
}