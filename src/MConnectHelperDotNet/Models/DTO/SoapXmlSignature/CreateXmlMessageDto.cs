// ***********************************************************************
//  Assembly         : RzR.Shared.eGovMD.MConnectHelperDotNet
//  Author           : RzR
//  Created On       : 2023-03-21 00:52
// 
//  Last Modified By : RzR
//  Last Modified On : 2023-03-21 17:25
// ***********************************************************************
//  <copyright file="CreateXmlMessageDto.cs" company="">
//   Copyright (c) RzR. All rights reserved.
//  </copyright>
// 
//  <summary>
//  </summary>
// ***********************************************************************

#region U S A G E S

using MConnectHelperDotNet.Models.Client;

#endregion

namespace MConnectHelperDotNet.Models.DTO.SoapXmlSignature
{
    /// <summary>
    ///     Create XML message DTO
    /// </summary>
    internal class CreateXmlMessageDto
    {
        /// <summary>
        ///     Request body
        /// </summary>
        internal string RequestBody { get; set; }

        /// <summary>
        ///     SOAP namespace
        /// </summary>
        internal string SoapNamespace { get; set; }

        /// <summary>
        ///     Sign current message
        /// </summary>
        internal bool SignMessage { get; set; }

        /// <summary>
        ///     Client certificate option
        /// </summary>
        internal ClientCertificateOptions Options { get; set; }
    }
}