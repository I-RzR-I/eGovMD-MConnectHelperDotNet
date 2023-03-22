// ***********************************************************************
//  Assembly         : RzR.Shared.eGovMD.MConnectHelperDotNet
//  Author           : RzR
//  Created On       : 2023-03-20 19:38
// 
//  Last Modified By : RzR
//  Last Modified On : 2023-03-20 22:40
// ***********************************************************************
//  <copyright file="IEndpointClient.cs" company="">
//   Copyright (c) RzR. All rights reserved.
//  </copyright>
// 
//  <summary>
//  </summary>
// ***********************************************************************

#region U S A G E S

using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using AggregatedGenericResultMessage.Abstractions;
using MConnectHelperDotNet.Models.DTO;
using MConnectHelperDotNet.Models.DTO.Request.Base;
using Newtonsoft.Json.Linq;

#endregion

namespace MConnectHelperDotNet.Abstractions
{
    /// <summary>
    ///     End point client
    /// </summary>
    public interface IEndpointClient
    {
        /// <summary>
        ///     Build request
        /// </summary>
        /// <param name="requestSettings">Response settings</param>
        /// <returns></returns>
        /// <remarks></remarks>
        IResult<HttpRequestMessage> BuildRequest(RequestSettingDto requestSettings);

        /// <summary>
        ///     Send request to API
        /// </summary>
        /// <param name="request">Send request data</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        Task<IResult<HttpResponseMessage>> SendRequestAsync(SendRequestDto request, 
            CancellationToken cancellationToken = default);

        /// <summary>
        ///     Process body async
        /// </summary>
        /// <param name="request">Request data</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        Task<IResult<JToken>> ProcessResponseAsync(ProcessResponseAsyncDto request, 
            CancellationToken cancellationToken = default);

        /// <summary>
        ///     Process response body
        /// </summary>
        /// <param name="request">Request data</param>
        /// <returns></returns>
        /// <remarks></remarks>
        IResult<JToken> ProcessResponseBody(ProcessResponseBodyDto request);

        /// <summary>
        ///     Get response data
        /// </summary>
        /// <param name="request">Request data</param>
        /// <returns></returns>
        /// <remarks></remarks>
        IResult<XmlNode> GetResponseBody(GetResponseBodyDto request);
    }
}