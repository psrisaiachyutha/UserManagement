#region References
using Common.Models.Requests;
using Common.Constants;
using FluentValidation;
#endregion References

namespace UserManagementService.Validators
{
    public class CreateUserRequestDTOValidator : AbstractValidator<CreateUserRequestDTO>
    {
        public CreateUserRequestDTOValidator()
        {
            RuleFor(x => x.Email).EmailAddress();
            RuleFor(x => x.Email).NotEmpty();

            RuleFor(x => x.Password).Matches(Constants.PasswordRegexPattern)
                .WithMessage(ErrorMessages.PasswordRegexErrorMessage);
            RuleFor(x => x.Password).NotEmpty();

            RuleFor(x => x.FirstName).NotEmpty();
            RuleFor(x => x.LastName).NotEmpty();
        }
    }
}
