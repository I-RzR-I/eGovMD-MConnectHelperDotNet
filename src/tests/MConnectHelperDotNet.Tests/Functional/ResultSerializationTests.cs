using MConnectHelperDotNet.Helpers;
using Newtonsoft.Json;
using RzR.ResultMessage;
using Xunit;

namespace MConnectHelperDotNet.Tests.Functional
{
    public class ResultSerializationTests
    {
        [Fact]
        public void FailureResult_SerializesMessageCodeNotKey()
        {
            var result = Result<string>.Failure(DefaultMessages.InternalErrorCallMConnectApi);

            var json = JsonConvert.SerializeObject(result);

            Assert.Contains("\"Code\"", json);
            Assert.DoesNotContain("\"Key\"", json);
        }
    }
}
