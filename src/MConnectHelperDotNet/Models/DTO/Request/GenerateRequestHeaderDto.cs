// ***********************************************************************
//  Assembly         : RzR.Shared.eGovMD.MConnectHelperDotNet
//  Author           : RzR
//  Created On       : 2023-03-20 20:05
// 
//  Last Modified By : RzR
//  Last Modified On : 2023-03-21 17:15
// ***********************************************************************
//  <copyright file="GenerateRequestHeaderDto.cs" company="">
//   Copyright (c) RzR. All rights reserved.
//  </copyright>
// 
//  <summary>
//  </summary>
// ***********************************************************************

// ReSharper disable ClassNeverInstantiated.Global

namespace MConnectHelperDotNet.Models.DTO.Request
{
    /// <summary>
    ///     Generate request header DTO
    /// </summary>
    public class GenerateRequestHeaderDto
    {
        /// <summary>
        ///     Request endpoint URL
        /// </summary>
        public string RequestEndPointUrl { get; set; }

        /// <summary>
        ///     Calling user (Personal code)
        /// </summary>
        public string CallingUserIdentifierCode { get; set; }

        /// <summary>
        ///     Gets or sets the name of the calling entity.
        /// </summary>
        /// <value>
        ///     The name of the calling entity.
        /// </value>
        public string CallingEntityName { get; set; }

        /// <summary>
        ///     Gets or sets the call basis.
        /// </summary>
        /// <value>
        ///     The call basis.
        /// </value>
        public string CallBasis { get; set; }

        /// <summary>
        ///     Gets or sets the call reason.
        /// </summary>
        /// <value>
        ///     The call reason.
        /// </value>
        public string CallReason { get; set; }
    }
}