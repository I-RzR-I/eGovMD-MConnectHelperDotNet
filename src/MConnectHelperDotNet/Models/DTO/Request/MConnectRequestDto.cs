// ***********************************************************************
//  Assembly         : RzR.Shared.eGovMD.MConnectHelperDotNet
//  Author           : RzR
//  Created On       : 2023-03-21 17:54
// 
//  Last Modified By : RzR
//  Last Modified On : 2023-03-21 17:54
// ***********************************************************************
//  <copyright file="MConnectRequestDto.cs" company="">
//   Copyright (c) RzR. All rights reserved.
//  </copyright>
// 
//  <summary>
//  </summary>
// ***********************************************************************

namespace MConnectHelperDotNet.Models.DTO.Request
{
    /// <summary>
    ///     Send request DTO
    /// </summary>
    public class MConnectRequestDto
    {
        /// <summary>
        ///     Calling user identifier code (IDNP)
        /// </summary>
        public string CallingUserIdentifierCode { get; set; }

        /// <summary>
        ///     Request endpoint
        /// </summary>
        public string RequestEndPointUrl { get; set; }
        
        /// <summary>
        ///     Method SOAP action
        /// </summary>
        public string SoapAction { get; set; }
        
        /// <summary>
        ///     Content headers
        /// </summary>
        public string ContentHeaders { get; set; }
        
        /// <summary>
        ///     Sign current message on request
        /// </summary>
        public bool SignMessage { get; set; } = true;
        
        /// <summary>
        ///     XML content data
        /// </summary>
        public string Content { get; set; }
        
        /// <summary>
        ///     Validate received response from MConnect
        /// </summary>
        public bool ValidateSignedMessage { get; set; }
        
        /// <summary>
        ///     Calling waiting timeout 
        /// </summary>
        public int TimeOut { get; set; } = 60;
    }
}