// ***********************************************************************
//  Assembly         : RzR.Shared.eGovMD.MConnectHelperDotNet
//  Author           : RzR
//  Created On       : 2023-03-20 22:35
// 
//  Last Modified By : RzR
//  Last Modified On : 2023-03-21 17:14
// ***********************************************************************
//  <copyright file="SendRequestDto.cs" company="">
//   Copyright (c) RzR. All rights reserved.
//  </copyright>
// 
//  <summary>
//  </summary>
// ***********************************************************************

#region U S A G E S

using System.Net.Http;

#endregion

// ReSharper disable ClassNeverInstantiated.Global

namespace MConnectHelperDotNet.Models.DTO.Request.Base
{
    /// <summary>
    ///     Send request DTO
    /// </summary>
    public class SendRequestDto
    {
        /// <summary>
        ///     Request message
        /// </summary>
        public HttpRequestMessage Request { get; set; }

        /// <summary>
        ///     API timeout
        /// </summary>
        public int TimeOut { get; set; } = 45;
    }
}