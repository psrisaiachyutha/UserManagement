#region References
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using Repository.Implementations;
using Repository.Interfaces;
using Repository.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
#endregion References

namespace Repository.Tests
{
    [TestFixture]
    public class UserRepositoryTests
    {
        private ApplicationDbContext _dbContext;
        private ILogger<UserRepository> _logger;
        private IUserRepository _userRepository;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "testDb")
                .Options;

            _dbContext = new ApplicationDbContext(options);
            _logger = Mock.Of<ILogger<UserRepository>>();
            _userRepository = new UserRepository(_logger, _dbContext);            
        }

        [TearDown]
        public void TearDown()
        {
            //_dbContext.Dispose();
            _dbContext.Database.EnsureDeleted();
        }

        [Test]
        public async Task GetUserByEmailAsync_ShouldReturnUser_WhenUserExists()
        {
            // Arrange
            var email = "test1@example.com";
            _dbContext.Users.Add(new User
            {
                FirstName = "John",
                LastName = "Wick",
                Email = "test1@example.com",
                PasswordHash = "password123"
            });
            
            _dbContext.SaveChanges();

            // Act
            var result = await _userRepository.GetUserByEmailAsync(email);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(email, result.Email);
        }

        [Test]
        public async Task GetUserByEmailAsync_ShouldReturnNull_WhenUserDoesNotExist()
        {
            // Arrange
            var email = "nonexistent@example.com";

            // Act
            var result = await _userRepository.GetUserByEmailAsync(email);

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public async Task CreateUserAsync_ShouldCreateNewUser()
        {
            // Arrange
            var newUser = new User
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "johndoe@example.com",
                PasswordHash = "password123"
            };

            // Act
            var createdUser = await _userRepository.CreateUserAsync(newUser);

            // Assert
            Assert.NotNull(createdUser);
            Assert.AreEqual(newUser.FirstName, createdUser.FirstName);
            Assert.AreEqual(newUser.LastName, createdUser.LastName);
            Assert.AreEqual(newUser.Email, createdUser.Email);
            Assert.AreEqual(newUser.PasswordHash, createdUser.PasswordHash);
            Assert.IsTrue(createdUser.UserId > 0);
        }

        [Test]
        public async Task CreateUserAsync_ShouldThrowException_WhenUserIsNull()
        {
            // Arrange
            User user = null;

            // Act and Assert
            var ex = Assert.ThrowsAsync<ArgumentNullException>(() => _userRepository.CreateUserAsync(user));
            Assert.That(ex.Message, Is.EqualTo("Value cannot be null. (Parameter 'entity')"));
        }

        [Ignore("need to check the code")]
        [Test]
        public async Task CreateUserAsync_ShouldThrowException_WhenEmailAlreadyExists()
        {
            // Arrange
            var existingUser = new User
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "johndoe@example.com",
                PasswordHash = "password123"
            };

            var newUser = new User
            {
                FirstName = "Jane",
                LastName = "Doe",
                Email = "johndoe@example.com",
                PasswordHash = "password123"
            };

            _dbContext.Users.Add(existingUser);
            _dbContext.SaveChanges();

            //await _userRepository.CreateUserAsync(newUser);
            // Act and Assert
            var ex = Assert.ThrowsAsync<InvalidOperationException>(async () => await _userRepository.CreateUserAsync(newUser));
            //Assert.That(ex.Message, Is.EqualTo($"A user with email {newUser.Email} already exists."));
        }

        [Test]
        public async Task GetAllUsersAsync_ShouldReturnAllUsers()
        {
            // Arrange
            var users = new List<User>
        {
            new User { UserId = 1, Email = "user1@example.com", FirstName = "user", LastName = "1",PasswordHash = "password123" },
            new User { UserId = 2, Email = "user2@example.com", FirstName = "user", LastName = "2",PasswordHash = "password123" },
            new User { UserId = 3, Email = "user3@example.com", FirstName = "user", LastName = "3",PasswordHash = "password123" }
        };
            await _dbContext.Users.AddRangeAsync(users);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _userRepository.GetAllUsersAsync();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(users.Count, result.Count());
            Assert.IsTrue(users.All(u => result.Any(r => r.UserId == u.UserId && r.Email == u.Email)));
        }

        [Test]
        public async Task GetAllUsersAsync_ShouldReturnEmptyList_WhenNoUsersExist()
        {
            // Act
            var result = await _userRepository.GetAllUsersAsync();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count());
        }

        [Test]
        public async Task GetUserByIdAsync_ShouldReturnCorrectUser()
        {
            // Arrange
            var userId = 1;
            await _dbContext.Users.AddAsync(new User { UserId = 1, PasswordHash = "password123", Email = "user1@example.com", FirstName = "user", LastName = "1" });
            await _dbContext.SaveChangesAsync();

            // Act
            var user = await _userRepository.GetUserByIdAsync(userId);

            // Assert
            Assert.NotNull(user);
            Assert.AreEqual(userId, user.UserId);
            Assert.AreEqual("user1@example.com", user.Email);            
        }

        [Test]
        public async Task GetUserByIdAsync_ShouldReturnNull_WhenUserDoesNotExist()
        {
            // Arrange
            var userId = 4;

            // Act
            var user = await _userRepository.GetUserByIdAsync(userId);

            // Assert
            Assert.Null(user);
        }

        [Test]
        public async Task AssignRoleAsync_ShouldAssignRoleToUser()
        {
            // Arrange
            await _dbContext.Users.AddAsync(new User { PasswordHash = "password123", UserId = 1, Email = "user1@example.com", FirstName = "user", LastName = "1" });
            await _dbContext.Roles.AddAsync(new Role { RoleId = 1, Name = "Admin" });
            await _dbContext.SaveChangesAsync();
            var roleId = 1;
            var userId = 1;

            // Act
            var result = await _userRepository.AssignRoleAsync(userId, roleId);

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(roleId, result.RoleId);
            Assert.AreEqual(userId, result.UserId);
        }

        [Ignore("need to check the code")]
        [Test]
        public async Task AssignRoleAsync_ShouldNotAssignRoleToNonExistingUser()
        {
            // Arrange
            var nonExistingUserId = 999;
            var roleId = 1;
            await _dbContext.Roles.AddAsync(new Role { RoleId = 1, Name = "Admin" });
            await _dbContext.SaveChangesAsync();

            // Act
            var ex = Assert.ThrowsAsync<InvalidOperationException>(
                () => _userRepository.AssignRoleAsync(nonExistingUserId, roleId));

            // Assert
            Assert.That(ex.Message, Is.EqualTo($"Sequence contains no elements. The source may be empty."));
        }

        [Ignore("need to check the code")]
        [Test]
        public async Task AssignRoleAsync_ShouldNotAssignInvalidRoleId()
        {
            // Arrange
            var user = new User { UserId = 1, Email = "user1@example.com", FirstName = "user", LastName = "1", PasswordHash = "password123" };
            await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync();
            var invalidRoleId = 999;

            // Act
            var ex = Assert.ThrowsAsync<DbUpdateException>(
                () => _userRepository.AssignRoleAsync(user.UserId, invalidRoleId));

            // Assert
            Assert.That(ex.InnerException.Message, Is.EqualTo($"The INSERT statement conflicted with the FOREIGN KEY constraint \"FK_UserRoles_Roles_RoleId\". The conflict occurred in database \"TestDatabase\", table \"dbo.Roles\", column 'RoleId'. The statement has been terminated."));
        }
    }

}
