namespace Common.Constants
{
    public static class Constants
    {
        public const string PasswordRegexPattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,15}$";
        public const string CommaDelimiter = ",";
    }
}
