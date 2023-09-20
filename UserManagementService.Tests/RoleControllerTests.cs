using Business.Interfaces;
using Common.Exceptions;
using Common.Models;
using Common.Models.Requests;
using Common.Models.Responses;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UserManagementService.Controllers;

namespace UserManagementService.Tests
{


    namespace UserManagementService.Tests.Controllers
    {
        //[TestFixture]
        //public class RolesControllerTests
        //{
        //    private Mock<IRoleBusinessHandler> _roleBusinessHandlerMock;
        //    private Mock<IValidator<CreateRoleRequestDTO>> _createRoleRequestDTOValidatorMock;
        //    private Mock<ILogger<RoleController>> _loggerMock;
        //    private RoleController _roleController;

        //    [SetUp]
        //    public void Setup()
        //    {
        //        _roleBusinessHandlerMock = new Mock<IRoleBusinessHandler>();
        //        _createRoleRequestDTOValidatorMock = new Mock<IValidator<CreateRoleRequestDTO>>();
        //        _loggerMock = new Mock<ILogger<RoleController>>();
        //        _roleController = new RoleController(_loggerMock.Object, _roleBusinessHandlerMock.Object, _createRoleRequestDTOValidatorMock.Object);
        //    }

        //    [Test]
        //    public async Task DeleteRoleById_ValidRoleId_ReturnsTrue()
        //    {
        //        // Arrange
        //        int roleId = 1;
        //        _roleBusinessHandlerMock.Setup(x => x.DeleteRoleByIdAsync(roleId)).ReturnsAsync(true);

        //        // Act
        //        var result = await _roleController.DeleteRoleById(roleId);

        //        // Assert
        //        Assert.IsNotNull(result);
        //        Assert.AreEqual(true, result.Data);
        //        Assert.AreEqual(StatusCodes.Status200OK, ((ObjectResult)result).StatusCode);
        //    }

        //    [Test]
        //    public async Task DeleteRoleById_InvalidRoleId_ReturnsNotFound()
        //    {
        //        // Arrange
        //        int roleId = 0;
        //        _roleBusinessHandlerMock.Setup(x => x.DeleteRoleByIdAsync(roleId)).ReturnsAsync(false);

        //        // Act
        //        var result = await _roleController.DeleteRoleById(roleId);

        //        // Assert
        //        Assert.IsNotNull(result);
        //        Assert.AreEqual(StatusCodes.Status404NotFound, ((ObjectResult)result).StatusCode);
        //    }

        //    [Test]
        //    public async Task DeleteRoleById_Unauthorized_ReturnsForbidden()
        //    {
        //        // Arrange
        //        int roleId = 1;
        //        _roleBusinessHandlerMock.Setup(x => x.DeleteRoleByIdAsync(roleId)).Throws(new UnauthorizedAccessException());

        //        // Act
        //        var result = await _roleController.DeleteRoleById(roleId);

        //        // Assert
        //        Assert.IsNotNull(result);
        //        Assert.AreEqual(StatusCodes.Status403Forbidden, ((ObjectResult)result).StatusCode);
        //    }

        //    [Test]
        //    public async Task DeleteRoleById_InternalServerError_ReturnsInternalServerError()
        //    {
        //        // Arrange
        //        int roleId = 1;
        //        _roleBusinessHandlerMock.Setup(x => x.DeleteRoleByIdAsync(roleId)).Throws(new Exception());

        //        // Act
        //        var result = await _roleController.DeleteRoleById(roleId);

        //        // Assert
        //        Assert.IsNotNull(result);
        //        Assert.AreEqual(StatusCodes.Status500InternalServerError, ((ObjectResult)result).StatusCode);
        //    }
        //}

    }

}