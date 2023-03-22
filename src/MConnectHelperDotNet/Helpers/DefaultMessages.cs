// ***********************************************************************
//  Assembly         : RzR.Shared.eGovMD.MConnectHelperDotNet
//  Author           : RzR
//  Created On       : 2023-03-20 22:45
// 
//  Last Modified By : RzR
//  Last Modified On : 2023-03-20 22:45
// ***********************************************************************
//  <copyright file="DefaultMessages.cs" company="">
//   Copyright (c) RzR. All rights reserved.
//  </copyright>
// 
//  <summary>
//  </summary>
// ***********************************************************************

namespace MConnectHelperDotNet.Helpers
{
    /// <summary>
    ///     Default result message
    /// </summary>
    internal static class DefaultMessages
    {
        internal const string InvalidCertificatePathOrPassword = "Invalid service certificate path or password!";

        internal const string InternalErrorCallMConnectApi = "Internal error on call MConnect API!";

        internal const string InternalErrorProcessResponse = "Internal error on process Response from MConnect!";

        internal const string InternalErrorOnFillRequestHeaders = "Internal error on fill request headers!";

        internal const string InvalidSoapSignature = "Invalid SOAP Signature!";

        internal const string NoExistOrMoreThanOneSignature = "No or more than one SOAP Signature in response!";

        internal const string NoExistOrMoreThanOneBodyInResponse = "No or more than one SOAP Body in response!";

        internal const string InvalidXmlForSoap = "Invalid XML for SOAP Body!";

        internal const string NoChildInBody = "No child in SOAP Body!";

        internal const string InvalidSoapMessageInResponse = "Invalid SOAP Message in response!";

        internal const string InternalErrorBuildRequest = "Internal error on build request for MConnect API!";

        internal const string InternalErrorBuildRequestString = "Internal error on build string request!";

        internal const string InvalidJsonMessage = "Invalid JSON message!";

        internal const string InvalidJsonResponse = "Invalid JSON in response!";

        internal const string InternalErrorOnGetMConnectHeader = "Internal error on getting MConnect request header!";

        internal const string MissingHeaderVariable = "Some of MConnect settings (EndpointUrl, SoapAction, ServiceCertificatePath, CallingEntity, CallingUser, CallBasis or CallReason) are empty!";
    }
}