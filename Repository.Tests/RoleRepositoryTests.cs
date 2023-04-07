#region References
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using Repository.Implementations;
using Repository.Interfaces;
using Repository.Models.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
#endregion References

namespace Repository.Tests
{

    [TestFixture]
    public class RoleRepositoryTests
    {
        private ApplicationDbContext _dbContext;
        private ILogger<RoleRepository> _logger;
        private IRoleRepository _roleRepository;

        [SetUp]
        public void SetUp()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb")
                .Options;
            _dbContext = new ApplicationDbContext(options);
            _logger = new Mock<ILogger<RoleRepository>>().Object;
            _roleRepository = new RoleRepository(_logger, _dbContext);
        }

        [Test]
        public async Task GetRoleByNameAsync_ShouldReturnRole_WhenRoleExists()
        {
            // Arrange
            var roleName = "TestRole";
            var role = new Role { Name = roleName };
            _dbContext.Roles.Add(role);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _roleRepository.GetRoleByNameAsync(roleName);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Name, Is.EqualTo(roleName));
        }

        [Test]
        public async Task GetRoleByNameAsync_ShouldReturnNull_WhenRoleDoesNotExist()
        {
            // Arrange
            var roleName = "TestRole";

            // Act
            var result = await _roleRepository.GetRoleByNameAsync(roleName);

            // Assert
            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task GetAllRolesAsync_ShouldReturnListOfRoles()
        {
            // Arrange
            var roles = new List<Role>
            {
                new Role { Name = "Role1" },
                new Role { Name = "Role2" },
                new Role { Name = "Role3" }
            };
            _dbContext.Roles.AddRange(roles);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _roleRepository.GetAllRolesAsync();

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.EqualTo(3));
            Assert.That(result.Select(x => x.Name), Is.EquivalentTo(roles.Select(x => x.Name)));
        }

        [Test]
        public async Task CreateRoleAsync_ShouldAddRoleToDatabase()
        {
            // Arrange
            var roleName = "TestRole";

            // Act
            var result = await _roleRepository.CreateRoleAsync(roleName);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Name, Is.EqualTo(roleName));
            Assert.That(_dbContext.Roles.Any(x => x.Name == roleName), Is.True);
        }

        [Test]
        public async Task GetRoleByIdAsync_ShouldReturnRole_WhenRoleExists()
        {
            // Arrange
            var roleId = 1;
            var role = new Role { RoleId = roleId, Name = "TestRole1" };
            _dbContext.Roles.Add(role);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _roleRepository.GetRoleByIdAsync(roleId);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.RoleId, Is.EqualTo(roleId));
        }

        [Test]
        public async Task GetRoleByIdAsync_ShouldReturnNull_WhenRoleDoesNotExist()
        {
            // Arrange
            var roleId = 1;

            // Act
            var result = await _roleRepository.GetRoleByIdAsync(roleId);

            // Assert
            Assert.That(result, Is.Null);
        }

        [TearDown]
        public void TearDown()
        {
            //_dbContext.Dispose();
            _dbContext.Database.EnsureDeleted();
        }

        [Test]
        public async Task DeleteRoleAsync_ShouldDeleteRoleFromDatabase()
        {
            // Arrange
            var role = new Role { Name = "TestRole" };
            _dbContext.Roles.Add(role);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _roleRepository.DeleteRoleAsync(role);

            // Assert
            Assert.IsTrue(result);
            Assert.IsNull(_dbContext.Roles.Find(role.RoleId));
        }
    }
}