// ***********************************************************************
//  Assembly         : RzR.Shared.eGovMD.MConnectHelperDotNet
//  Author           : RzR
//  Created On       : 2023-03-21 08:21
// 
//  Last Modified By : RzR
//  Last Modified On : 2023-03-21 16:40
// ***********************************************************************
//  <copyright file="RestSignatureHelper.cs" company="">
//   Copyright (c) RzR. All rights reserved.
//  </copyright>
// 
//  <summary>
//  </summary>
// ***********************************************************************

#region U S A G E S

using System;
using System.Security.Cryptography.X509Certificates;
using AggregatedGenericResultMessage;
using AggregatedGenericResultMessage.Abstractions;
using AggregatedGenericResultMessage.Extensions.Result;
using Jose;
using MConnectHelperDotNet.Models.Client;

#endregion

namespace MConnectHelperDotNet.Helpers
{
    /// <summary>
    ///     REST client signature helper
    /// </summary>
    internal static class RestSignatureHelper
    {
        /// <summary>
        ///     Apply signature on message
        /// </summary>
        /// <param name="options"></param>
        /// <param name="content">Data</param>
        /// <returns></returns>
        /// <remarks></remarks>
        internal static string ApplySignature(ClientCertificateOptions options, string content)
        {
            var privateKey = options.ClientCertificate.GetRSAPrivateKey();

            return JWT.Encode(content, privateKey, JwsAlgorithm.RS256);
        }

        /// <summary>
        ///     Verify signature
        /// </summary>
        /// <param name="responseBody">Response body</param>
        /// <param name="serviceCertificate">Service certificate</param>
        /// <returns></returns>
        internal static IResult<string> ValidateSignature(string responseBody, X509Certificate2 serviceCertificate)
        {
            var publicKey = serviceCertificate.GetRSAPublicKey();
            try
            {
                return Result<string>.Success(JWT.Decode(responseBody, publicKey));
            }
            catch (Exception ex)
            {
                return Result<string>.Failure(ex.Message)
                    .WithError(ex);
            }
        }
    }
}