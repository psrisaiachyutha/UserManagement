using AutoMapper;
using Business.Implementations;
using Common.Constants;
using Common.Exceptions;
using Common.Models.Requests;
using Common.Models.Responses;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using Repository.Interfaces;
using Repository.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Business.Tests
{
    [TestFixture]
    public class RoleBusinessHandlerTests
    {
        private RoleBusinessHandler _roleBusinessHandler;
        private Mock<IRoleRepository> _roleRepositoryMock;
        private Mock<IMapper> _mapperMock;
        private Mock<ILogger<RoleBusinessHandler>> _loggerMock;

        [SetUp]
        public void Setup()
        {
            _roleRepositoryMock = new Mock<IRoleRepository>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILogger<RoleBusinessHandler>>();

            _roleBusinessHandler = new RoleBusinessHandler(
                _loggerMock.Object,
                _mapperMock.Object,
                _roleRepositoryMock.Object);
        }

        [Test]
        public async Task GetAllRolesAsync_ReturnsListOfRoles()
        {
            // Arrange
            var roles = new List<Role>();
            _roleRepositoryMock.Setup(x => x.GetAllRolesAsync()).ReturnsAsync(roles);

            var roleResponseDTOs = new List<RoleResponseDTO>();
            _mapperMock.Setup(x => x.Map<IEnumerable<RoleResponseDTO>>(roles)).Returns(roleResponseDTOs);

            // Act
            var result = await _roleBusinessHandler.GetAllRolesAsync();

            // Assert
            Assert.That(result, Is.EqualTo(roleResponseDTOs));
        }

        [Test]
        public async Task CreateRoleAsync_ValidInput_ReturnsCreatedRole()
        {
            // Arrange
            var createRoleRequestDTO = new CreateRoleRequestDTO
            {
                Name = "Test Role"
            };
            var createdRole = new Role
            {
                RoleId = 1,
                Name = "Test Role"
            };
            var responseDto = new RoleResponseDTO
            {
                RoleId = 1,
                Name = "Test Role"
            };
            _roleRepositoryMock.Setup(x => x.GetRoleByNameAsync(createRoleRequestDTO.Name)).ReturnsAsync((Role)null);
            _roleRepositoryMock.Setup(x => x.CreateRoleAsync(createRoleRequestDTO.Name)).ReturnsAsync(createdRole);
            _mapperMock.Setup(x => x.Map<RoleResponseDTO>(createdRole)).Returns(responseDto);

            // Act
            var result = await _roleBusinessHandler.CreateRoleAsync(createRoleRequestDTO);

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(createdRole.RoleId, result.RoleId);
            Assert.AreEqual(createdRole.Name, result.Name);
        }

        [Test]
        public void CreateRoleAsync_RoleAlreadyExists_ThrowsRecordAlreadyExistsException()
        {
            // Arrange
            var createRoleRequestDTO = new CreateRoleRequestDTO
            {
                Name = "Test Role"
            };
            var existingRole = new Role
            {
                RoleId = 1,
                Name = "Test Role"
            };
            _roleRepositoryMock.Setup(x => x.GetRoleByNameAsync(createRoleRequestDTO.Name)).ReturnsAsync(existingRole);

            // Act & Assert
            Assert.ThrowsAsync<RecordAlreadyExistsException>(async () => await _roleBusinessHandler.CreateRoleAsync(createRoleRequestDTO));
        }

        [Test]
        public async Task DeleteRoleByIdAsync_WhenRoleExists_ReturnsTrue()
        {
            // Arrange
            int roleId = 1;
            var role = new Role() { RoleId = roleId, Name = "Test Role" };
            _roleRepositoryMock.Setup(x => x.GetRoleByIdAsync(roleId)).ReturnsAsync(role);
            _roleRepositoryMock.Setup(x => x.DeleteRoleAsync(role)).ReturnsAsync(true);

            // Act
            var result = await _roleBusinessHandler.DeleteRoleByIdAsync(roleId);

            // Assert
            Assert.IsTrue(result);
            _roleRepositoryMock.Verify(x => x.GetRoleByIdAsync(roleId), Times.Once);
            _roleRepositoryMock.Verify(x => x.DeleteRoleAsync(role), Times.Once);
        }

        [Test]
        public void DeleteRoleByIdAsync_WhenRoleDoesNotExist_ThrowsRecordNotFoundException()
        {
            // Arrange
            int roleId = 1;
            _roleRepositoryMock.Setup(x => x.GetRoleByIdAsync(roleId)).ReturnsAsync(null as Role);

            // Act & Assert
            var ex = Assert.ThrowsAsync<RecordNotFoundException>(() => _roleBusinessHandler.DeleteRoleByIdAsync(roleId));
            Assert.AreEqual(ErrorMessages.RoleByIdNotFound(roleId), ex.Message);
            _roleRepositoryMock.Verify(x => x.GetRoleByIdAsync(roleId), Times.Once);
            _roleRepositoryMock.Verify(x => x.DeleteRoleAsync(It.IsAny<Role>()), Times.Never);
        }
    }

}