namespace MyAdmin.ApiHost;

public static class ApiEndpoints
{
    private const string ApiBase = "api";

    public static class Test
    {
        private const string GroupName = $"{ApiBase}/test";

        public const string TestMethod = $"{GroupName}/test";
        public const string TestMethod2 = $"{GroupName}/test2";
    }
    public static class Testv2{
        private const string GroupName = $"{ApiBase}/testv2";

        public const string TestMethod = $"{GroupName}/test";
        public const string TestMethod2 = $"{GroupName}/test2";
    }
}
