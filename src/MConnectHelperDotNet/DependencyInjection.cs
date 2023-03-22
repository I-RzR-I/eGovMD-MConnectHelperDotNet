// ***********************************************************************
//  Assembly         : RzR.Shared.eGovMD.MConnectHelperDotNet
//  Author           : RzR
//  Created On       : 2023-03-20 17:11
// 
//  Last Modified By : RzR
//  Last Modified On : 2023-03-21 21:13
// ***********************************************************************
//  <copyright file="DependencyInjection.cs" company="">
//   Copyright (c) RzR. All rights reserved.
//  </copyright>
// 
//  <summary>
//  </summary>
// ***********************************************************************

#if NETSTANDARD2_0_OR_GREATER || NET || NETCOREAPP3_1_OR_GREATER

#region U S A G E S

using System;
using MConnectHelperDotNet.Abstractions;
using MConnectHelperDotNet.Abstractions.Services;
using MConnectHelperDotNet.Configurations.ClientCertificateOption;
using MConnectHelperDotNet.Configurations.Clients.Rest;
using MConnectHelperDotNet.Configurations.Clients.Soap;
using MConnectHelperDotNet.Enums;
using MConnectHelperDotNet.Models.Client;
using MConnectHelperDotNet.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

#endregion

namespace MConnectHelperDotNet
{
    /// <summary>
    ///     MConnect service DI
    /// </summary>
    /// <remarks></remarks>
    public static class DependencyInjection
    {
        /// <summary>
        ///     Add MConnect
        /// </summary>
        /// <param name="services">Service collection</param>
        /// <param name="configuration">Application configuration</param>
        /// <returns></returns>
        public static void AddMConnect(this IServiceCollection services, IConfiguration configuration)
        {
            //MConnect interconnection builder
            var clientCertificate = configuration.GetSection("MConnectOptions:ClientCertificate");
            services.Configure<ClientCertificateOptions>(clientCertificate);
            services.AddEndpointClients();

            //Consuming MConnect API service
            services.TryAddScoped<IMConnectApiService, MConnectApiService>();
        }

        /// <summary>
        ///     Add endpoint clients configurations
        /// </summary>
        /// <param name="services">Service collection</param>
        /// <returns></returns>
        private static void AddEndpointClients(this IServiceCollection services)
        {
            services.AddOptions();
            services.AddSingleton<IPostConfigureOptions<ClientCertificateOptions>, ClientCertificateOptionsPostConfigure>();

            services.AddHttpClient();

            services.AddSingleton<Soap11Client>();
            services.AddSingleton<Soap12Client>();
            services.AddSingleton<RestClient>();

            services.AddSingleton<Func<EndpointType, IEndpointClient>>(sp => endpointType =>
            {
                return endpointType switch
                {
                    EndpointType.SOAP11 => sp.GetRequiredService<Soap11Client>(),
                    EndpointType.SOAP12 => sp.GetRequiredService<Soap12Client>(),
                    EndpointType.REST => sp.GetRequiredService<RestClient>(),
                    _ => throw new NotImplementedException($"No implementation for endpoint type: {endpointType}")
                };
            });

            services.AddScoped<IRequestBuilderService, RequestBuilderService>();
        }
    }
}
#endif