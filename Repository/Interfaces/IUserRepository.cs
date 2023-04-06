using Repository.Models.Entities;

namespace Repository.Interfaces
{
    public interface IUserRepository
    {
        /// <summary>
        /// Gets the user based on the email
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public Task<User> GetUserByEmailAsync(string email);

        /// <summary>
        /// Creates the new user in database
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Task<User> CreateUserAsync(User user);

        /// <summary>
        /// Gets all the users from the database
        /// </summary>
        /// <returns></returns>
        public Task<IEnumerable<User>> GetAllUsersAsync();

        /// <summary>
        /// Gets the user by id name
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Task<User> GetUserByIdAsync(int id);

        /// <summary>
        /// Assign the role to the user
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public Task<UserRole> AssignRoleAsync(int userId, int roleId);

        /// <summary>
        /// Will delete all the roles assigned to the user
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public Task<bool> DeleteAllUserRoles(int userId);
    }
}
