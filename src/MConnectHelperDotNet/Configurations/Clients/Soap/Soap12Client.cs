// ***********************************************************************
//  Assembly         : RzR.Shared.eGovMD.MConnectHelperDotNet
//  Author           : RzR
//  Created On       : 2023-03-21 07:55
// 
//  Last Modified By : RzR
//  Last Modified On : 2023-03-21 07:57
// ***********************************************************************
//  <copyright file="Soap12Client.cs" company="">
//   Copyright (c) RzR. All rights reserved.
//  </copyright>
// 
//  <summary>
//  </summary>
// ***********************************************************************

#region U S A G E S

using System.Net.Http;
using MConnectHelperDotNet.Models.Client;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
// ReSharper disable NotAccessedField.Local

#endregion

namespace MConnectHelperDotNet.Configurations.Clients.Soap
{
    /// <summary>
    ///     SOAP12 client
    /// </summary>
    public sealed class Soap12Client : BaseSoapClient
    {
        /// <summary>
        ///     Logger
        /// </summary>
#pragma warning disable IDE0052 // Remove unread private members
        private readonly ILogger<Soap12Client> _logger;
#pragma warning restore IDE0052 // Remove unread private members

        /// <summary>
        ///     SOAP12 client
        /// </summary>
        /// <param name="clientFactory">Client factory</param>
        /// <param name="optionsAccessor">Option accessor</param>
        /// <param name="logger">Logger</param>
        public Soap12Client(IHttpClientFactory clientFactory, IOptionsMonitor<ClientCertificateOptions> optionsAccessor, 
            ILogger<Soap12Client> logger)
            : base(clientFactory, optionsAccessor, "http://www.w3.org/2003/05/soap-envelope", "application/soap+xml",
                logger)
            => _logger = logger;
    }
}