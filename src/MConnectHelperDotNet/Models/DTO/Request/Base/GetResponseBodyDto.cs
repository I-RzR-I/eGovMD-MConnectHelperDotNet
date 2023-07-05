// ***********************************************************************
//  Assembly         : RzR.Shared.eGovMD.MConnectHelperDotNet
//  Author           : RzR
//  Created On       : 2023-03-20 22:35
// 
//  Last Modified By : RzR
//  Last Modified On : 2023-03-21 17:14
// ***********************************************************************
//  <copyright file="GetResponseBodyDto.cs" company="">
//   Copyright (c) RzR. All rights reserved.
//  </copyright>
// 
//  <summary>
//  </summary>
// ***********************************************************************

// ReSharper disable ClassNeverInstantiated.Global

// ReSharper disable UnusedAutoPropertyAccessor.Global
namespace MConnectHelperDotNet.Models.DTO.Request.Base
{
    /// <summary>
    ///     Get response body DTO
    /// </summary>
    public class GetResponseBodyDto
    {
        /// <summary>
        ///     Response settings
        /// </summary>
        public ResponseSettingDto ResponseSetting { get; set; }

        /// <summary>
        ///     Response bode
        /// </summary>
        public string ResponseBody { get; set; }
    }
}