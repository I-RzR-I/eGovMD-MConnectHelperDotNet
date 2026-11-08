using System;
using System.Net.Http;
using MConnectHelperDotNet.Models.DTO;
using MConnectHelperDotNet.Models.DTO.Request;
using MConnectHelperDotNet.Models.DTO.Request.Base;

namespace MConnectHelperDotNet.Tests.TestInfrastructure
{
    public static class TestRequests
    {
        private const string DefaultContent = "<Ping/>";

        public static RequestSettingDto RequestSettings(string content = DefaultContent, bool signMessage = false,
            string soapAction = "x")
        {
            return new RequestSettingDto
            {
                Method = HttpMethod.Post,
                Uri = new Uri(TestValues.BaseUrl),
                Content = content,
                SignMessage = signMessage,
                SoapAction = soapAction
            };
        }

        public static SendRequestDto SendRequest(HttpRequestMessage request = null, int timeOut = 5)
        {
            return new SendRequestDto
            {
                Request = request ?? new HttpRequestMessage(HttpMethod.Post, TestValues.BaseUrl),
                TimeOut = timeOut
            };
        }

        public static MConnectRequestDto MConnectRequest(string content = DefaultContent)
        {
            return new MConnectRequestDto
            {
                CallingEntityName = TestValues.CallingEntity,
                CallingUserIdentifierCode = TestValues.CallingUser,
                CallBasis = TestValues.CallBasis,
                CallReason = TestValues.CallReason,
                RequestEndPointUrl = TestValues.BaseUrl,
                SoapAction = "x",
                Content = content,
                SignMessage = false,
                ValidateSignedMessage = false,
                TimeOut = 5
            };
        }

        public static GenerateRequestHeaderDto GenerateRequestHeader(string endpointUrl = TestValues.BaseUrl,
            string callingUser = null, string callingEntity = null, string callBasis = null, string callReason = null)
        {
            return new GenerateRequestHeaderDto
            {
                RequestEndPointUrl = endpointUrl,
                CallingUserIdentifierCode = callingUser,
                CallingEntityName = callingEntity,
                CallBasis = callBasis,
                CallReason = callReason
            };
        }
    }
}
