#region References
using AutoMapper;
using Business.Implementations;
using Business.Interfaces;
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
    public class UserBusinessHandlerTests
    {
        private readonly Mock<ILogger<UserBusinessHandler>> _loggerMock;
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IRoleRepository> _roleRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IOptions<ApplicationSettings>> _optionsMock;
        private readonly UserBusinessHandler _userBusinessHandler;

        public UserBusinessHandlerTests()
        {
            _loggerMock = new Mock<ILogger<UserBusinessHandler>>();
            _userRepositoryMock = new Mock<IUserRepository>();
            _roleRepositoryMock = new Mock<IRoleRepository>();
            _mapperMock = new Mock<IMapper>();
            _optionsMock = new Mock<IOptions<ApplicationSettings>>();
            _userBusinessHandler = new UserBusinessHandler(_loggerMock.Object, _mapperMock.Object, _userRepositoryMock.Object, _roleRepositoryMock.Object, _optionsMock.Object);
        }

        [Ignore("need to move the code")]
        [Test]
        public async Task VerifyLoginAsync_ValidCredentials_ReturnsTokenResponseDTO()
        {
            /*
            // Arrange
            var loginRequestDTO = new LoginRequestDTO { Email = "test@test.com", Password = "password123" };
            var user = new User { Email = loginRequestDTO.Email, PasswordHash = _userBusinessHandler.CreatePasswordHash(loginRequestDTO.Password) };
            _userRepositoryMock.Setup(x => x.GetUserByEmailAsync(loginRequestDTO.Email)).ReturnsAsync(user);
            var tokenResponseDTO = new TokenResponseDTO { AccessToken = "accessToken" };
            _mapperMock.Setup(x => x.Map<TokenResponseDTO>(It.IsAny<object>())).Returns(tokenResponseDTO);

            // Act
            var result = await _userBusinessHandler.VerifyLoginAsync(loginRequestDTO);

            // Assert
            Assert.AreEqual(tokenResponseDTO, result);
            */
        }

        [Test]
        public void VerifyLoginAsync_UserNotFound_ThrowsRecordNotFoundException()
        {
            // Arrange
            var loginRequestDTO = new LoginRequestDTO { Email = "test@test.com", Password = "password123" };
            _userRepositoryMock.Setup(x => x.GetUserByEmailAsync(loginRequestDTO.Email)).ReturnsAsync((User)null);

            // Act + Assert
            Assert.ThrowsAsync<RecordNotFoundException>(() => _userBusinessHandler.VerifyLoginAsync(loginRequestDTO));
        }

        [Ignore("need to move the code")]
        [Test]
        public void VerifyLoginAsync_InvalidCredentials_ThrowsInvalidCredentialsException()
        {
            // Arrange
            var loginRequestDTO = new LoginRequestDTO { Email = "test@test.com", Password = "password123" };
            var user = new User { Email = loginRequestDTO.Email, PasswordHash = "wrongPasswordHash" };
            _userRepositoryMock.Setup(x => x.GetUserByEmailAsync(loginRequestDTO.Email)).ReturnsAsync(user);

            // Act + Assert
            Assert.ThrowsAsync<InvalidCredentialsException>(() => _userBusinessHandler.VerifyLoginAsync(loginRequestDTO));
        }
    }
}