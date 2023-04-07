#region References
using AutoMapper;
using Business.Implementations;
using Business.Interfaces;
using Business.Mappers;
using Common.Configurations;
using Common.Exceptions;
using Common.Models.Requests;
using Common.Models.Responses;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using Repository.Interfaces;
using Repository.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#endregion References

namespace Business.Tests
{
    [TestFixture]
    public class UserBusinessHandlerTests
    {
        private Mock<ILogger<UserBusinessHandler>> _loggerMock;
        private Mock<IUserRepository> _userRepositoryMock;
        private Mock<IRoleRepository> _roleRepositoryMock;
        private Mock<IMapper> _mapperMock;
        private Mock<IJWTHelper> _jwthelperMock;
        private UserBusinessHandler _userBusinessHandler;

        [SetUp]
        public void SetUp()
        {
            _loggerMock = new Mock<ILogger<UserBusinessHandler>>();
            _userRepositoryMock = new Mock<IUserRepository>();
            _roleRepositoryMock = new Mock<IRoleRepository>();
            _mapperMock = new Mock<IMapper>();
            _jwthelperMock = new Mock<IJWTHelper>();
            _userBusinessHandler = new UserBusinessHandler(
                _loggerMock.Object,
                _mapperMock.Object,
                _userRepositoryMock.Object,
                _roleRepositoryMock.Object,
                _jwthelperMock.Object);
        }

        [Test]
        public async Task VerifyLoginAsync_ValidCredentials_ReturnsTokenResponseDTO()
        {
            // Arrange
            var email = "john@example.com";
            var password = "mypassword";
            var loginRequest = new LoginRequestDTO { Email = email, Password = password };
            var user = new User { Email = email, PasswordHash = "passwordhash", FirstName = "John", LastName = "Doe", };
            _userRepositoryMock.Setup(repo => repo.GetUserByEmailAsync(email)).ReturnsAsync(user);
            _jwthelperMock.Setup(jwt => jwt.VerifyPasswordHash(password, user.PasswordHash)).Returns(true);

            // Act
            var result = await _userBusinessHandler.VerifyLoginAsync(loginRequest);

            // Assert
            Assert.IsInstanceOf<TokenResponseDTO>(result);
            Assert.IsNotEmpty(result.AccessToken);
        }

        [Test]
        public void VerifyLoginAsync_UserNotFound_ThrowsRecordNotFoundException()
        {
            // Arrange
            var email = "john@example.com";
            var password = "mypassword";
            var loginRequest = new LoginRequestDTO { Email = email, Password = password };
            _userRepositoryMock.Setup(repo => repo.GetUserByEmailAsync(email)).ReturnsAsync(null as User);

            // Act & Assert
            Assert.ThrowsAsync<RecordNotFoundException>(() => _userBusinessHandler.VerifyLoginAsync(loginRequest));
        }

        [Test]
        public void VerifyLoginAsync_InvalidPassword_ThrowsInvalidCredentialsException()
        {
            // Arrange
            var email = "john@example.com";
            var password = "mypassword";
            var loginRequest = new LoginRequestDTO { Email = email, Password = password };
            var user = new User { Email = email, PasswordHash = "passwordhash", FirstName = "John", LastName = "Doe", };
            _userRepositoryMock.Setup(repo => repo.GetUserByEmailAsync(email)).ReturnsAsync(user);
            _jwthelperMock.Setup(jwt => jwt.VerifyPasswordHash(password, user.PasswordHash)).Returns(false);

            // Act & Assert
            Assert.ThrowsAsync<InvalidCredentialsException>(() => _userBusinessHandler.VerifyLoginAsync(loginRequest));
        }

        [Test]
        public void RegisterUserAsync_ShouldThrowRecordAlreadyExistsException_WhenUserAlreadyExists()
        {
            // Arrange
            var createUserRequestDTO = new CreateUserRequestDTO
            {
                Email = "testemail@example.com",
                Password = "testpassword",
                FirstName = "John",
                LastName = "Doe"
            };
            var existingUser = new User {
                Email = "testemail@example.com",
                PasswordHash = "testpassword",
                FirstName = "John",
                LastName = "Doe"
            };

            _userRepositoryMock.Setup(repo => repo.GetUserByEmailAsync(createUserRequestDTO.Email))
                .ReturnsAsync(existingUser);

            
            // Act + Assert
            var ex = Assert.ThrowsAsync<RecordAlreadyExistsException>(
                async () => await _userBusinessHandler.RegisterUserAsync(createUserRequestDTO));
            Assert.AreEqual("User with email: testemail@example.com already exists",ex.Message);
        }

        [Test]
        public async Task GetAllUsersAsync_ReturnsEmptyCollection_WhenNoUsersFound()
        {
            // Arrange
            _userRepositoryMock.Setup(repo => repo.GetAllUsersAsync())
                                .ReturnsAsync(new List<User>());
            
            // Act
            var result = await _userBusinessHandler.GetAllUsersAsync();

            // Assert
            Assert.IsEmpty(result);
        }

        [Test]
        public void GetAllUsersAsync_ThrowsException_WhenRepositoryThrowsException()
        {
            // Arrange
            _userRepositoryMock.Setup(repo => repo.GetAllUsersAsync())
                                .ThrowsAsync(new Exception("Database connection failed"));
            
            // Act & Assert
            Assert.ThrowsAsync<Exception>(() => _userBusinessHandler.GetAllUsersAsync());
        }
    }
}
