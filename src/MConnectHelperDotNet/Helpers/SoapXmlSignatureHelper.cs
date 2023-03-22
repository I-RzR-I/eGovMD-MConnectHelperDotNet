// ***********************************************************************
//  Assembly         : RzR.Shared.eGovMD.MConnectHelperDotNet
//  Author           : RzR
//  Created On       : 2023-03-21 00:15
// 
//  Last Modified By : RzR
//  Last Modified On : 2023-03-21 01:10
// ***********************************************************************
//  <copyright file="SoapXmlSignatureHelper.cs" company="">
//   Copyright (c) RzR. All rights reserved.
//  </copyright>
// 
//  <summary>
//  </summary>
// ***********************************************************************

#region U S A G E S

using System;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Xml;
using AggregatedGenericResultMessage;
using AggregatedGenericResultMessage.Abstractions;
using AggregatedGenericResultMessage.Extensions.Result;
using MConnectHelperDotNet.Models.DTO.SoapXmlSignature;

#endregion

namespace MConnectHelperDotNet.Helpers
{
    /// <summary>
    ///     Soap XML signature helper
    /// </summary>
    internal static class SoapXmlSignatureHelper
    {
        private const string NamespaceUri = "http://www.w3.org/2000/09/xmldsig#";

        /// <summary>
        ///     Create XML message
        /// </summary>
        /// <param name="request">Request data</param>
        /// <returns></returns>
        /// <remarks></remarks>
        internal static IResult<string> CreateXmlMessage(CreateXmlMessageDto request)
        {
            if (!request.SignMessage)
            {
                return
                    Result<string>.Success(
                        $@"<soap:Envelope xmlns:soap=""{request.SoapNamespace}"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance""><soap:Body>{request.RequestBody}</soap:Body></soap:Envelope>");
            }

            var doc = new XmlDocument();
            var envelope = doc.CreateElement("soap", "Envelope", request.SoapNamespace);
            envelope.SetAttribute("xmlns:xsi", "http://www.w3.org/2001/XMLSchema-instance");
            doc.AppendChild(envelope);

            // Create Security Header
            var id = Guid.NewGuid().ToString("N");
            var TimestampNs = "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd";

            var security = doc.CreateElement("Security", "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd");
            var msAtt = doc.CreateAttribute("soap", "mustUnderstand", request.SoapNamespace);
            msAtt.InnerText = "1";
            security.Attributes.Append(msAtt);

            var timestamp = doc.CreateElement("Timestamp", TimestampNs);
            timestamp.SetAttribute("Id", "TS-" + id);
            security.AppendChild(timestamp);

            var created = doc.CreateElement("Created", TimestampNs);
            created.InnerText = XmlConvert.ToString(DateTimeOffset.UtcNow);
            timestamp.AppendChild(created);

            var expires = doc.CreateElement("Expires", TimestampNs);
            expires.InnerText = XmlConvert.ToString(DateTimeOffset.UtcNow.AddMinutes(15));
            timestamp.AppendChild(expires);

            var header = doc.CreateElement("soap", "Header", request.SoapNamespace);
            header.AppendChild(security);
            envelope.AppendChild(header);

            // Create Body
            var body = doc.CreateElement("soap", "Body", request.SoapNamespace);
            envelope.AppendChild(body);
            var bodyDocument = new XmlDocument();

            try
            {
                bodyDocument.LoadXml(request.RequestBody);
            }
            catch (Exception e)
            {
                return Result<string>
                    .Failure(DefaultMessages.InvalidXmlForSoap)
                    .WithError(e);
            }

            body.AppendChild(body.OwnerDocument?.ImportNode(bodyDocument.DocumentElement!, true)!);
            body.SetAttribute("Id", "MS-" + id);

            return ApplySignature(new ApplySignatureDto
            {
                Id = id,
                Header = header,
                Body = body,
                Document = doc,
                Options = request.Options
            });
        }

        /// <summary>
        ///     Apply signature on message/XML
        /// </summary>
        /// <param name="request">Request data</param>
        /// <returns></returns>
        /// <remarks></remarks>
        private static IResult<string> ApplySignature(ApplySignatureDto request)
        {
            request.Body.SetAttribute("Id", "MS-" + request.Id);

            var keyInfo = new KeyInfo();
            keyInfo.AddClause(new KeyInfoX509Data(request.Options.ClientCertificate));
            var signedXml = new SignedXml(request.Document) { KeyInfo = keyInfo, SigningKey = request.Options.ClientCertificate.PrivateKey };

            signedXml.SignedInfo.CanonicalizationMethod = SignedXml.XmlDsigExcC14NTransformUrl;
            var bodyReference = new Reference { Uri = "#MS-" + request.Id };
            bodyReference.AddTransform(new XmlDsigExcC14NTransform()); // required to match doc
            signedXml.AddReference(bodyReference);

            var tsReference = new Reference { Uri = "#TS-" + request.Id };
            tsReference.AddTransform(new XmlDsigExcC14NTransform()); // required to match doc
            signedXml.AddReference(tsReference);

            signedXml.ComputeSignature();
            var signedElement = signedXml.GetXml();

            request.Header.FirstChild.AppendChild(signedElement);

            return Result<string>.Success(request.Document.InnerXml);
        }

        /// <summary>
        ///     Validate signature
        /// </summary>
        /// <param name="doc">XML document</param>
        /// <param name="serviceCertificate">X509 service certificate</param>
        /// <remarks></remarks>
        public static IResult ValidateSignature(XmlDocument doc, X509Certificate2 serviceCertificate)
        {
            var signatureNodes = doc.GetElementsByTagName("Signature", NamespaceUri);
            if (signatureNodes.Count != 1)
                return Result.Failure(DefaultMessages.NoExistOrMoreThanOneSignature);

            var sDocument = new SignedSoapXml(doc.DocumentElement);
            sDocument.LoadXml((XmlElement)signatureNodes[0]);

            return sDocument.CheckSignature(serviceCertificate, true).Equals(false) 
                ? Result.Failure(DefaultMessages.InvalidSoapSignature) 
                : Result.Success();
        }

        private class SignedSoapXml : SignedXml
        {
            public SignedSoapXml(XmlElement elem) : base(elem)
            {
            }

            /// <summary>
            ///     Get element
            /// </summary>
            /// <param name="document">XML document</param>
            /// <param name="idValue">Id</param>
            /// <returns></returns>
            public override XmlElement GetIdElement(XmlDocument document, string idValue)
            {
                var nodes = document.SelectNodes("//*[@*[local-name()='Id' and .='" + idValue + "']]");
                
                return nodes == null || nodes.Count != 1 
                    ? null 
                    : nodes[0] as XmlElement;
            }
        }
    }
}