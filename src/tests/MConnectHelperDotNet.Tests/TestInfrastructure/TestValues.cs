namespace MConnectHelperDotNet.Tests.TestInfrastructure
{
    public static class TestValues
    {
        public const string BaseUrl = "https://mconnect.example.invalid/";

        public const string CallingEntity = "TEST-ENTITY";

        public const string CallingUser = "TEST-USER";

        public const string CallBasis = "TEST-BASIS";

        public const string CallReason = "TEST-REASON";

        public const string Marker = "MARKER-7F3A";

        public const char PipeChar = '|';

        public static string JoinPathAndPassword(string path, string password) => string.Concat(path, PipeChar, password);
    }
}
