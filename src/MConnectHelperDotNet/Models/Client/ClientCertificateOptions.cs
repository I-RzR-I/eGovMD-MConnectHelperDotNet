// ***********************************************************************
//  Assembly         : RzR.Shared.eGovMD.MConnectHelperDotNet
//  Author           : RzR
//  Created On       : 2023-03-20 19:41
// 
//  Last Modified By : RzR
//  Last Modified On : 2023-03-21 21:31
// ***********************************************************************
//  <copyright file="ClientCertificateOptions.cs" company="">
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

namespace MConnectHelperDotNet.Models.Client
{
    /// <summary>
    ///     Client certificate options
    /// </summary>
    public class ClientCertificateOptions
    {
        /// <summary>
        ///     Client certificate path
        /// </summary>
        public string ClientCertificatePath { get; set; }

        /// <summary>
        ///     Client certificate password
        /// </summary>
        public string ClientCertificatePassword { get; set; }

        /// <summary>
        ///     Client X509 certificate
        /// </summary>
        public X509Certificate2 ClientCertificate { get; set; }
    }
}