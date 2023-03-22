// ***********************************************************************
//  Assembly         : RzR.Shared.eGovMD.MConnectHelperDotNet
//  Author           : RzR
//  Created On       : 2023-03-21 01:03
// 
//  Last Modified By : RzR
//  Last Modified On : 2023-03-21 17:25
// ***********************************************************************
//  <copyright file="ApplySignatureDto.cs" company="">
//   Copyright (c) RzR. All rights reserved.
//  </copyright>
// 
//  <summary>
//  </summary>
// ***********************************************************************

#region U S A G E S

using System.Xml;
using MConnectHelperDotNet.Models.Client;

#endregion

namespace MConnectHelperDotNet.Models.DTO.SoapXmlSignature
{
    /// <summary>
    ///     Apply signature DTO
    /// </summary>
    internal class ApplySignatureDto
    {
        /// <summary>
        ///     XML document
        /// </summary>
        internal XmlDocument Document { get; set; }

        /// <summary>
        ///     XML element header
        /// </summary>
        internal XmlElement Header { get; set; }

        /// <summary>
        ///     XML element body
        /// </summary>
        internal XmlElement Body { get; set; }

        /// <summary>
        ///     XML sign id
        /// </summary>
        internal string Id { get; set; }

        /// <summary>
        ///     Client certificate option
        /// </summary>
        internal ClientCertificateOptions Options { get; set; }
    }
}