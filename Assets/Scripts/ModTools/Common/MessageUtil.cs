namespace Common
{
    public static class MessageUtil
    {
        private static string _appName = "UNLOCALIZED";

        public static void SetAppName(string appName)
        {
            _appName = appName;
        }
    }
}
