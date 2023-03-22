// ***********************************************************************
//  Assembly         : RzR.Shared.eGovMD.MConnectHelperDotNet
//  Author           : RzR
//  Created On       : 2023-03-21 21:29
// 
//  Last Modified By : RzR
//  Last Modified On : 2023-03-21 21:46
// ***********************************************************************
//  <copyright file="ClientCertificateOptionsPostConfigure.cs" company="">
//   Copyright (c) RzR. All rights reserved.
//  </copyright>
// 
//  <summary>
//  </summary>
// ***********************************************************************

#region U S A G E S

using System;
using System.IO;
using MConnectHelperDotNet.Configurations.Certificate;
using MConnectHelperDotNet.Helpers;
using MConnectHelperDotNet.Models.Client;
using Microsoft.Extensions.Options;

#endregion

namespace MConnectHelperDotNet.Configurations.ClientCertificateOption
{
    /// <summary>
    ///     Client certificate POST configuration options
    /// </summary>
    public class ClientCertificateOptionsPostConfigure : IPostConfigureOptions<ClientCertificateOptions>
    {
        /// <summary>
        ///     POST configuration
        /// </summary>
        /// <param name="name"></param>
        /// <param name="options">Client certificate option</param>
        public void PostConfigure(string name, ClientCertificateOptions options)
        {
            if (options.ClientCertificate != null) return;

            var clientCertificatePath = options.ClientCertificatePath;
            var clientCertificatePassword = options.ClientCertificatePassword;
            var certificate = CertificateLoader.Private(Path.Combine(AppContext.BaseDirectory, clientCertificatePath), clientCertificatePassword);

            options.ClientCertificate = certificate ?? throw new ApplicationException(DefaultMessages.InvalidCertificatePathOrPassword);
        }
    }
}