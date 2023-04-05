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

namespace UserManagementService.Controllers
{
    
    [Authorize(Roles = nameof(RolesEnum.Admin))]
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/user")]
    public class UserController : ControllerBase
    {
        private readonly IUserBusinessHandler _userBusinessHandler;
        private readonly IValidator<LoginRequestDTO> _loginRequestDTOValidator;
        private readonly IValidator<CreateUserRequestDTO> _createUserRequestDTOValidator;

        public UserController(IUserBusinessHandler userBusinessHandler,
            IValidator<LoginRequestDTO> loginRequestDTOValidator,
            IValidator<CreateUserRequestDTO> createUserRequestDTOValidator)
        {
            _userBusinessHandler = userBusinessHandler;
            _loginRequestDTOValidator = loginRequestDTOValidator;
            _createUserRequestDTOValidator = createUserRequestDTOValidator;
        }

        [AllowAnonymous]
        [HttpPost("authenticate")]
        public async Task<GenericResponseObject<TokenResponse>> Login([FromBody] LoginRequestDTO loginRequestDTO)
        {
            // Validating the request object
            ValidationResult validationResult = await _loginRequestDTOValidator.ValidateAsync(loginRequestDTO);
            if (!validationResult.IsValid)
            {
                var errorMessages = validationResult.Errors.Select(x => x.ErrorMessage).ToList();
                throw new BadRequestException(string.Join(Constants.CommaDelimiter, errorMessages));
            }

            var result = await _userBusinessHandler.VerifyLoginAsync(loginRequestDTO);
            return new GenericResponseObject<TokenResponse> { Data = result };
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<GenericResponseObject<UserResponseDTO>> CreateUser([FromBody] CreateUserRequestDTO createUserRequestDTO)
        {

            // Validating the request object
            ValidationResult validationResult = await _createUserRequestDTOValidator.ValidateAsync(createUserRequestDTO);
            if (!validationResult.IsValid)
            {
                var errorMessages = validationResult.Errors.Select(x => x.ErrorMessage).ToList();
                throw new BadRequestException(string.Join(Constants.CommaDelimiter, errorMessages));
            }

            var result = await _userBusinessHandler.RegisterUserAsync(createUserRequestDTO);
            
            return new GenericResponseObject<UserResponseDTO> { Data = result };
        }

        [HttpGet("all")]
        public async Task<GenericResponseObject<IEnumerable<UserResponseDTO>>> GetAllUsers()
        {
            var result = await _userBusinessHandler.GetAllUsersAsync();
            return new GenericResponseObject<IEnumerable<UserResponseDTO>> { Data = result };
        }
    }
}
