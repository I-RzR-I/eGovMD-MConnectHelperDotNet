// ***********************************************************************
//  Assembly         : RzR.Shared.eGovMD.MConnectHelperDotNet
//  Author           : RzR
//  Created On       : 2023-03-20 22:38
// 
//  Last Modified By : RzR
//  Last Modified On : 2023-03-21 17:14
// ***********************************************************************
//  <copyright file="ProcessResponseBodyDto.cs" company="">
//   Copyright (c) RzR. All rights reserved.
//  </copyright>
// 
//  <summary>
//  </summary>
// ***********************************************************************

namespace MConnectHelperDotNet.Models.DTO.Request.Base
{
    /// <summary>
    ///     Process response body DTO
    /// </summary>
    public class ProcessResponseBodyDto
    {
        /// <summary>
        ///     Response settings
        /// </summary>
        public ResponseSettingDto ResponseSetting { get; set; }

        /// <summary>
        ///     Response body
        /// </summary>
        public string ResponseBody { get; set; }
    }
}