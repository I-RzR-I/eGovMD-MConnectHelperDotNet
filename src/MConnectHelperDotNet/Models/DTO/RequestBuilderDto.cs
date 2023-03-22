// ***********************************************************************
//  Assembly         : RzR.Shared.eGovMD.MConnectHelperDotNet
//  Author           : RzR
//  Created On       : 2023-03-20 19:49
// 
//  Last Modified By : RzR
//  Last Modified On : 2023-03-21 17:21
// ***********************************************************************
//  <copyright file="RequestBuilderDto.cs" company="">
//   Copyright (c) RzR. All rights reserved.
//  </copyright>
// 
//  <summary>
//  </summary>
// ***********************************************************************

#region U S A G E S

using System.Text;

#endregion

namespace MConnectHelperDotNet.Models.DTO
{
    /// <summary>
    ///     Request header builder
    /// </summary>
    public class RequestBuilderDto
    {
        /// <summary>
        ///     Endpoint URL
        /// </summary>
        public string EndpointUrl { get; set; }

        /// <summary>
        ///     Service certificate path
        /// </summary>
        public string ServiceCertificatePath { get; set; }

        /// <summary>
        ///     Calling entity
        /// </summary>
        public string CallingEntity { get; set; }

        /// <summary>
        ///     Calling user
        /// </summary>
        public string CallingUser { get; set; }

        /// <summary>
        ///     Calling basis
        /// </summary>
        public string CallBasis { get; set; }

        /// <summary>
        ///     Calling reason
        /// </summary>
        public string CallReason { get; set; }

        /// <summary>
        ///     Format current request data to string
        /// </summary>
        /// <returns>Get data as string</returns>
        public string FormatToString()
        {
            var sbRequestHeaders = new StringBuilder();
            sbRequestHeaders.AppendLine("CallingEntity:" + CallingEntity);
            sbRequestHeaders.AppendLine("CallingUser:" + CallingUser);
            sbRequestHeaders.AppendLine("CallBasis:" + CallBasis);
            sbRequestHeaders.AppendLine("CallReason:" + CallReason);

            return sbRequestHeaders.ToString();
        }
    }
}