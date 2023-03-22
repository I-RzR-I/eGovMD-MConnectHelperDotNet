// ***********************************************************************
//  Assembly         : RzR.Shared.eGovMD.MConnectHelperDotNet
//  Author           : RzR
//  Created On       : 2023-03-20 22:39
// 
//  Last Modified By : RzR
//  Last Modified On : 2023-03-21 17:14
// ***********************************************************************
//  <copyright file="ProcessResponseAsyncDto.cs" company="">
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
    ///     Process response DTO
    /// </summary>
    public class ProcessResponseAsyncDto
    {
        /// <summary>
        ///     Response settings
        /// </summary>
        public ResponseSettingDto ResponseSetting { get; set; }

        /// <summary>
        ///     HTTP response message
        /// </summary>
        public HttpResponseMessage Response { get; set; }
    }
}