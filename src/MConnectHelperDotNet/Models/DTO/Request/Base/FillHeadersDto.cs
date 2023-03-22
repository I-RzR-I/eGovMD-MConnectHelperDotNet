// ***********************************************************************
//  Assembly         : RzR.Shared.eGovMD.MConnectHelperDotNet
//  Author           : RzR
//  Created On       : 2023-03-20 22:59
// 
//  Last Modified By : RzR
//  Last Modified On : 2023-03-21 17:14
// ***********************************************************************
//  <copyright file="FillHeadersDto.cs" company="">
//   Copyright (c) RzR. All rights reserved.
//  </copyright>
// 
//  <summary>
//  </summary>
// ***********************************************************************

#region U S A G E S

using System.Net.Http.Headers;

#endregion

namespace MConnectHelperDotNet.Models.DTO.Request.Base
{
    /// <summary>
    ///     Fill header DTO
    /// </summary>
    public class FillHeadersDto
    {
        /// <summary>
        ///     Raw header
        /// </summary>
        public string RawHeaders { get; set; }

        /// <summary>
        ///     HTTP header
        /// </summary>
        public HttpHeaders Headers { get; set; }
    }
}