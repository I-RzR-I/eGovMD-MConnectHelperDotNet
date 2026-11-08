using System.Linq;
using Newtonsoft.Json;
using RzR.ResultMessage.Abstractions;

namespace MConnectHelperDotNet.Tests.Edge
{
    internal static class EdgeText
    {
        internal static string Info(IResult result, int index) => result.Messages.ElementAt(index).Message?.Info;

        internal static string FirstEntry(IResult result) => JsonConvert.SerializeObject(result.Messages.First());

        internal static string Everything(IResult result) => JsonConvert.SerializeObject(result);
    }
}
