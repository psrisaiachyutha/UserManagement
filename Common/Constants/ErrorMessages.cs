namespace Common.Constants
{
    public static class ErrorMessages
    {
        public const string PasswordRegexErrorMessage = "Password must be between 8 and 20 characters and contain one uppercase letter, one lowercase letter, one digit and one special character.";
        public const string EmailOrPasswordIncorrect = "Email or Password is incorrect";
        public static string UserNotFound(string email) => $"User with email: {email} doesn't exists.";
        public static string UserAlreadyExists(string email) => $"User with email: {email} already exists";
    }
}
