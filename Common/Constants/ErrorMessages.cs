namespace Common.Constants
{
    public static class ErrorMessages
    {
        public const string PasswordRegexErrorMessage = "Password must be between 8 and 20 characters and contain one uppercase letter, one lowercase letter, one digit and one special character.";
        public const string EmailOrPasswordIncorrect = "Email or Password is incorrect";
        public static string UserNotFound(string email) => $"User with email: {email} doesn't exists.";
        public static string RoleNotFound(string roleName) => $"Role with name: {roleName} doesn't exists.";
        public static string RoleByIdNotFound(int roleId) => $"Role with id: {roleId} doesn't exists.";
        public static string UserNotFoundWithId(int userId) => $"User with id: {userId} doesn't exists.";
        public static string NoUserRolesFound(int userId) => $"User with id: {userId} doesn't contain any roles";
        public static string UserAlreadyExists(string email) => $"User with email: {email} already exists";
        public static string RoleAlreadyExists(string name) => $"Role with name: {name} already exists";
        public static string UserAlreadyContainsRole(string email,string roleName) => $"User with email: {email} already contains role: {roleName}";
    }
}
