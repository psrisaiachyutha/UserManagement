#region References
using Business.Interfaces;
using Common.Constants;
using Common.Exceptions;
using Common.ExtensionMethods;
using Common.Models;
using Common.Models.Requests;
using Common.Models.Responses;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;
#endregion References

namespace UserManagementService.Controllers
{
    /// <summary>
    /// Role controller
    /// </summary>
    //[Authorize(Roles = nameof(RolesEnum.Admin))]
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/role")]
    [Produces(MediaTypeNames.Application.Json)]
    public class RoleController : ControllerBase
    {
        #region Declarations
        private readonly IRoleBusinessHandler _roleBusinessHandler;
        private readonly IValidator<CreateRoleRequestDTO> _createRoleRequestDTOValidator;
        private readonly ILogger<RoleController> _logger;
        #endregion Declarations

        #region Constructor
        /// <summary>
        /// RoleController constructor is created with its dependents
        /// </summary>
        /// <param name="logger">logger object</param>
        /// <param name="roleBusinessHandler">for handling the bussiness for RoleController</param>
        /// <param name="createRoleRequestDTOValidator">validator for createRoleRequestDTO</param>
        public RoleController(
            ILogger<RoleController> logger,
            IRoleBusinessHandler roleBusinessHandler,
            IValidator<CreateRoleRequestDTO> createRoleRequestDTOValidator)
        {
            _logger = logger;
            _roleBusinessHandler = roleBusinessHandler;
            _createRoleRequestDTOValidator = createRoleRequestDTOValidator;
        }
        #endregion Constructor

        #region Public Methods

        /// <summary>
        /// Api to get all the roles in the database
        /// </summary>        
        /// <returns>IEnumerable of roles</returns>
        /// <response code="200">If request went well and successful</response>
        /// <response code="403">If the user is unauthorized</response>
        /// <response code="500">If any internal server error due to the database or any other issue</response>
        [HttpGet("all", Name = nameof(GetAllRoles))]
        [ProducesResponseType(typeof(ApiResponseObject<IEnumerable<RoleResponseDTO>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorObject), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiErrorObject), StatusCodes.Status500InternalServerError)]
        public async Task<ApiResponseObject<IEnumerable<RoleResponseDTO>>> GetAllRoles()
        {
            _logger.LogInformation("{functionName} api is triggered.",nameof(GetAllRoles));
            
            var result = await _roleBusinessHandler.GetAllRolesAsync();
            
            _logger.LogInformation("fetched {count} number of roles.", result.Count());
            
            return new ApiResponseObject<IEnumerable<RoleResponseDTO>> { Data = result };
        }

        /// <summary>
        /// Api to create a new role
        /// </summary>        
        /// <returns>Created role</returns>
        /// <response code="201">If request went well and successful created</response>
        /// <response code="400">If request object is not valid</response>
        /// <response code="403">If the user is unauthorized</response>
        /// <response code="500">If any internal server error due to the database or any other issue</response>
        /// <param name="createRoleRequestDTO">create role request object</param>
        /// <exception cref="BadRequestException">bad request exception is thrown when request object is not valid</exception>
        [HttpPost(Name = nameof(CreateRole))]
        [ProducesResponseType(typeof(ApiResponseObject<IEnumerable<RoleResponseDTO>>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiErrorObject), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiErrorObject), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiErrorObject), StatusCodes.Status500InternalServerError)]
        [Consumes(MediaTypeNames.Application.Json)]
        public async Task<ApiResponseObject<RoleResponseDTO>> CreateRole(
            [FromBody] CreateRoleRequestDTO createRoleRequestDTO)
        {
            _logger.LogInformation("{functionName} api is triggered with request body : {request}", 
                nameof(CreateRole), createRoleRequestDTO.JsonSerialize());
            
            ValidationResult validationResult = await _createRoleRequestDTOValidator.ValidateAsync(createRoleRequestDTO);
            if (!validationResult.IsValid)
            {
                var errorMessages = validationResult.Errors.Select(x => x.ErrorMessage).ToList();
                _logger.LogError("Model validation failed for the request");
                throw new BadRequestException(string.Join(Constants.CommaDelimiter, errorMessages));
            }

            var result = await _roleBusinessHandler.CreateRoleAsync(createRoleRequestDTO);

            _logger.LogInformation("Successfully created the role");

            return new ApiResponseObject<RoleResponseDTO> { Data = result };
        }

        /// <summary>
        /// Api to delete a role in database
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns>boolean value whether data is deleted or not</returns>
        /// <response code="200">If the role object is found and deleted successfully</response>
        /// <response code="404">If request object is not found in database</response>
        /// <response code="403">If the user is unauthorized</response>
        /// <response code="500">If any internal server error due to the database or any other issue</response>
        [HttpDelete("{roleId:int}", Name = nameof(DeleteRoleById))]
        [ProducesResponseType(typeof(ApiResponseObject<IEnumerable<RoleResponseDTO>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorObject), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiErrorObject), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiErrorObject), StatusCodes.Status500InternalServerError)]
        public async Task<ApiResponseObject<bool>> DeleteRoleById(int roleId)
        {
            _logger.LogInformation("{functionName} api is triggered.", nameof(DeleteRoleById));

            var result = await _roleBusinessHandler.DeleteRoleByIdAsync(roleId);

            _logger.LogInformation("Required role is successfully deleted.");

            return new ApiResponseObject<bool> { Data = result };
        }

        #endregion Public Methods   
    }
}
