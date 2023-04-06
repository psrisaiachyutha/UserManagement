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
    [Consumes(MediaTypeNames.Application.Json)]
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

        [AllowAnonymous]
        [HttpPost("authenticate")]
        public async Task<ApiResponseObject<TokenResponseDTO>> Login([FromBody] LoginRequestDTO loginRequestDTO)
        {
            
            ValidationResult validationResult = await _loginRequestDTOValidator.ValidateAsync(loginRequestDTO);
            if (!validationResult.IsValid)
            {
                var errorMessages = validationResult.Errors.Select(x => x.ErrorMessage).ToList();
                throw new BadRequestException(string.Join(Constants.CommaDelimiter, errorMessages));
            }

            var result = await _userBusinessHandler.VerifyLoginAsync(loginRequestDTO);
            return new ApiResponseObject<TokenResponseDTO> { Data = result };
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<ApiResponseObject<UserResponseDTO>> CreateUser([FromBody] CreateUserRequestDTO createUserRequestDTO)
        {

            // Validating the request object
            ValidationResult validationResult = await _createUserRequestDTOValidator.ValidateAsync(createUserRequestDTO);
            if (!validationResult.IsValid)
            {
                var errorMessages = validationResult.Errors.Select(x => x.ErrorMessage).ToList();
                throw new BadRequestException(string.Join(Constants.CommaDelimiter, errorMessages));
            }

            var result = await _userBusinessHandler.RegisterUserAsync(createUserRequestDTO);
            
            return new ApiResponseObject<UserResponseDTO> { Data = result };
        }

        [AllowAnonymous]
        [HttpGet("all")]
        public async Task<ApiResponseObject<IEnumerable<UserResponseDTO>>> GetAllUsers()
        {
            var result = await _userBusinessHandler.GetAllUsersAsync();
            return new ApiResponseObject<IEnumerable<UserResponseDTO>> { Data = result };
        }

        [AllowAnonymous]
        [HttpPost("assignrole")]
        public async Task<ApiResponseObject<AssignRoleResponseDTO>> CreateRoleForUser(
            [FromBody] AssignRoleRequestDTO assignRoleRequestDTO)
        {
            // Validating the request object
            ValidationResult validationResult = await _assignRoleRequestDTOValidator.ValidateAsync(assignRoleRequestDTO);
            if (!validationResult.IsValid)
            {
                var errorMessages = validationResult.Errors.Select(x => x.ErrorMessage).ToList();
                throw new BadRequestException(string.Join(Constants.CommaDelimiter, errorMessages));
            }

            var result = await _userBusinessHandler.AssignRoleForUserAsync(assignRoleRequestDTO);
            return new ApiResponseObject<AssignRoleResponseDTO> { Data = result };
        }

        [AllowAnonymous]
        [HttpDelete("{userId:int}/roles")]
        public async Task<ApiResponseObject<bool>> DeleteAllRolesForUser(int userId)
        {
            var result = await _userBusinessHandler.DeleteAllRoles(userId);
            return new ApiResponseObject<bool> { Data = result };
        }
    }
}
