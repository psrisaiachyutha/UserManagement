#region References
using Business.Interfaces;
using Common.Constants;
using Common.Constants.Enums;
using Common.Exceptions;
using Common.Models;
using Common.Models.Requests;
using Common.Models.Responses;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;
#endregion References

namespace UserManagementService.Controllers
{
    /// <summary>
    /// User controller
    /// </summary>
    [Authorize(Roles = nameof(RolesEnum.Admin))]
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/user")]
    [Produces(MediaTypeNames.Application.Json)]
    public class UserController : ControllerBase
    {
        #region Declarations
        private readonly IUserBusinessHandler _userBusinessHandler;
        private readonly IValidator<LoginRequestDTO> _loginRequestDTOValidator;
        private readonly IValidator<CreateUserRequestDTO> _createUserRequestDTOValidator;
        private readonly IValidator<AssignRoleRequestDTO> _assignRoleRequestDTOValidator;
        private readonly ILogger<UserController> _logger;
        #endregion Declarations

        #region Constructor
        /// <summary>
        /// UserController constructor is created with its dependents
        /// </summary>
        /// <param name="logger">logger for the logs related to class user controller</param>
        /// <param name="userBusinessHandler">bussiness handler for the user controller</param>
        /// <param name="loginRequestDTOValidator">validotor for login request</param>
        /// <param name="createUserRequestDTOValidator">validator for the create new user request</param>
        /// <param name="assignRoleRequestDTOValidator">validator for the assign role request</param>
        public UserController(
            ILogger<UserController> logger,
            IUserBusinessHandler userBusinessHandler,
            IValidator<LoginRequestDTO> loginRequestDTOValidator,
            IValidator<CreateUserRequestDTO> createUserRequestDTOValidator,
            IValidator<AssignRoleRequestDTO> assignRoleRequestDTOValidator)
        {
            _logger = logger;
            _userBusinessHandler = userBusinessHandler;
            _loginRequestDTOValidator = loginRequestDTOValidator;
            _createUserRequestDTOValidator = createUserRequestDTOValidator;
            _assignRoleRequestDTOValidator = assignRoleRequestDTOValidator;
        }
        #endregion Constructor


        /// <summary>
        /// This api is used to authenticate the user
        /// </summary>
        /// <param name="loginRequestDTO">request payload for login to the application</param>
        /// <returns>Token if user successfully login</returns>
        /// <exception cref="BadRequestException">When the request payload is invalid </exception>
        /// <response code="200">If the user successfully login to the application</response>
        /// <response code="400">If the requested payload is invalid</response>
        /// <response code="404">IF the user record does not found</response>
        /// <response code="401">If the user credentials are invalid unauthorized</response>
        /// <response code="500">If any internal server error due to the database or any other issue</response>
        [AllowAnonymous]
        [HttpPost("login", Name = nameof(Login))]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(ApiResponseObject<AssignRoleResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorObject), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiErrorObject), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiErrorObject), StatusCodes.Status500InternalServerError)]
        public async Task<ApiResponseObject<TokenResponseDTO>> Login([FromBody] LoginRequestDTO loginRequestDTO)
        {
            _logger.LogInformation("{functionName} api is triggered.", nameof(Login));

            ValidationResult validationResult = await _loginRequestDTOValidator.ValidateAsync(loginRequestDTO);
            if (!validationResult.IsValid)
            {
                _logger.LogError("Passed payload for loging the user is not valid");
                var errorMessages = validationResult.Errors.Select(x => x.ErrorMessage).ToList();
                throw new BadRequestException(string.Join(Constants.CommaDelimiter, errorMessages));
            }

            var result = await _userBusinessHandler.VerifyLoginAsync(loginRequestDTO);
            _logger.LogInformation("successfully logged in to the application");

            return new ApiResponseObject<TokenResponseDTO> { Data = result };
        }

        /// <summary>
        /// This api is to create new user in database
        /// </summary>
        /// <param name="createUserRequestDTO">request payload for creating new user</param>
        /// <returns>Newly created user information record</returns>
        /// <exception cref="BadRequestException">When the request payload is invalid to store in database</exception>
        /// <response code="201">If the user object created successfully</response>
        /// <response code="400">If the requested payload is invalid</response>
        /// <response code="409">IF the user record already exists with the email</response>
        /// <response code="401">If the user is unauthorized</response>
        /// <response code="500">If any internal server error due to the database or any other issue</response>
        [HttpPost("register", Name = nameof(CreateUser))]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(ApiResponseObject<AssignRoleResponseDTO>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiErrorObject), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiErrorObject), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ApiErrorObject), StatusCodes.Status500InternalServerError)]
        public async Task<ApiResponseObject<UserResponseDTO>> CreateUser([FromBody] CreateUserRequestDTO createUserRequestDTO)
        {
            _logger.LogInformation("{functionName} api is triggered.", nameof(CreateUser));
            ValidationResult validationResult = await _createUserRequestDTOValidator.ValidateAsync(createUserRequestDTO);
            if (!validationResult.IsValid)
            {
                _logger.LogError("Passed payload for creating the user is not valid");
                var errorMessages = validationResult.Errors.Select(x => x.ErrorMessage).ToList();
                throw new BadRequestException(string.Join(Constants.CommaDelimiter, errorMessages));
            }

            var result = await _userBusinessHandler.RegisterUserAsync(createUserRequestDTO);
            _logger.LogInformation("successfully created the user in database");

            Response.StatusCode = 201;
            return new ApiResponseObject<UserResponseDTO> { Data = result };
        }

        /// <summary>
        /// Api to get all the users in the database
        /// </summary>        
        /// <returns>IEnumerable of users</returns>
        /// <response code="200">If request went well and successful</response>
        /// <response code="401">If the user is unauthorized</response>
        /// <response code="500">If any internal server error due to the database or any other issue</response>
        [HttpGet("all", Name = nameof(GetAllUsers))]
        [ProducesResponseType(typeof(ApiResponseObject<IEnumerable<UserResponseDTO>>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiErrorObject), StatusCodes.Status500InternalServerError)]        
        public async Task<ApiResponseObject<IEnumerable<UserResponseDTO>>> GetAllUsers()
        {
            _logger.LogInformation("{functionName} api is triggered.", nameof(GetAllUsers));

            var result = await _userBusinessHandler.GetAllUsersAsync();

            _logger.LogInformation("Fetched {count} number of users from database", result.Count());

            return new ApiResponseObject<IEnumerable<UserResponseDTO>> { Data = result };
        }

        /// <summary>
        /// This api is used to create role for the user
        /// </summary>
        /// <param name="assignRoleRequestDTO">request payload for assginng role to user</param>
        /// <returns>Newly assgined role to user information record</returns>
        /// <exception cref="BadRequestException">When the request payload is invalid to store in database</exception>
        /// <response code="201">If the user object is found and role assigned to user successfully</response>
        /// <response code="400">If the requested payload is invalid</response>
        /// <response code="404">If user object is not found in database</response>
        /// <response code="401">If the user is unauthorized</response>
        /// <response code="500">If any internal server error due to the database or any other issue</response>        
        [HttpPost("assignrole", Name = nameof(CreateRoleForUser))]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(ApiResponseObject<AssignRoleResponseDTO>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiErrorObject), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiErrorObject), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiErrorObject), StatusCodes.Status500InternalServerError)]
        public async Task<ApiResponseObject<AssignRoleResponseDTO>> CreateRoleForUser(
            [FromBody] AssignRoleRequestDTO assignRoleRequestDTO)
        {
            _logger.LogInformation("{functionName} api is triggered.", nameof(CreateRoleForUser));

            ValidationResult validationResult = await _assignRoleRequestDTOValidator.ValidateAsync(assignRoleRequestDTO);
            if (!validationResult.IsValid)
            {
                _logger.LogError("Passed payload for assigning role for the user is not valid");
                var errorMessages = validationResult.Errors.Select(x => x.ErrorMessage).ToList();
                throw new BadRequestException(string.Join(Constants.CommaDelimiter, errorMessages));
            }

            var result = await _userBusinessHandler.AssignRoleForUserAsync(assignRoleRequestDTO);
            _logger.LogInformation("Successfully assigned the role for the user");

            Response.StatusCode = 201;
            return new ApiResponseObject<AssignRoleResponseDTO> { Data = result };
        }

        /// <summary>
        /// This api will delete all the roles related to a user
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        /// <response code="200">If the user object is found and deleted all roles related to user successfully</response>        
        /// <response code="404">If request object is not found in database</response>
        /// <response code="401">If the user is unauthorized</response>
        /// <response code="500">If any internal server error due to the database or any other issue</response>
        [HttpDelete("{userId:int}/roles", Name = nameof(DeleteAllRolesForUser))]
        [ProducesResponseType(typeof(ApiResponseObject<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiErrorObject), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiErrorObject), StatusCodes.Status500InternalServerError)]
        public async Task<ApiResponseObject<bool>> DeleteAllRolesForUser(int userId)
        {
            _logger.LogInformation("{functionName} api is triggered.", nameof(DeleteAllRolesForUser));

            var result = await _userBusinessHandler.DeleteAllRoles(userId);

            _logger.LogInformation("Successfully deleted all the roles for user");

            return new ApiResponseObject<bool> { Data = result };
        }
    }
}
