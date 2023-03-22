// ***********************************************************************
//  Assembly         : RzR.Shared.eGovMD.MConnectHelperDotNet
//  Author           : RzR
//  Created On       : 2023-03-20 19:48
// 
//  Last Modified By : RzR
//  Last Modified On : 2023-03-21 17:21
// ***********************************************************************
//  <copyright file="ResponseSettingDto.cs" company="">
//   Copyright (c) RzR. All rights reserved.
//  </copyright>
// 
//  <summary>
//  </summary>
// ***********************************************************************

#region U S A G E S

using System.Security.Cryptography.X509Certificates;

#endregion

// ReSharper disable ClassNeverInstantiated.Global

namespace MConnectHelperDotNet.Models.DTO
{
    /// <summary>
    ///     Response settings DRO
    /// </summary>
    public class ResponseSettingDto
    {
        /// <summary>
        ///     Validate signed message
        /// </summary>
        public bool ValidateSignedMessage { get; set; }

        /// <summary>
        ///     Is current message signed
        /// </summary>
        public bool IsMessageSigned { get; set; }

        /// <summary>
        ///     Service certificate
        /// </summary>
        public X509Certificate2 ServiceCertificate { get; set; }
    }
}