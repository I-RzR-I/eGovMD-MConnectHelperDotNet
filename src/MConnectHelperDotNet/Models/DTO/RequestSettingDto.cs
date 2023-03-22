// ***********************************************************************
//  Assembly         : RzR.Shared.eGovMD.MConnectHelperDotNet
//  Author           : RzR
//  Created On       : 2023-03-20 19:48
// 
//  Last Modified By : RzR
//  Last Modified On : 2023-03-21 17:21
// ***********************************************************************
//  <copyright file="RequestSettingDto.cs" company="">
//   Copyright (c) RzR. All rights reserved.
//  </copyright>
// 
//  <summary>
//  </summary>
// ***********************************************************************

#region U S A G E S

using System;
using System.Net.Http;

#endregion

// ReSharper disable once ClassNeverInstantiated.Global

namespace MConnectHelperDotNet.Models.DTO
{
    /// <summary>
    ///     Request setting DRO
    /// </summary>
    public class RequestSettingDto
    {
        /// <summary>
        ///     HTTP method
        /// </summary>
        public HttpMethod Method { get; set; }

        /// <summary>
        ///     URI
        /// </summary>
        public Uri Uri { get; set; }

        /// <summary>
        ///     Request headers
        /// </summary>
        public string RequestHeaders { get; set; }

        /// <summary>
        ///     SOAP action
        /// </summary>
        public string SoapAction { get; set; }

        /// <summary>
        ///     Content headers
        /// </summary>
        public string ContentHeaders { get; set; }

        /// <summary>
        ///     Content
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        ///     Sign current message
        /// </summary>
        public bool SignMessage { get; set; }
    }
}