using Common.Models.Requests;
using FluentValidation;

namespace UserManagementService.Validators
{
    public class AssignRoleRequestDTOValidator : AbstractValidator<AssignRoleRequestDTO>
    {
        public AssignRoleRequestDTOValidator()
        {
            RuleFor(x => x.Email).EmailAddress();
            RuleFor(x => x.Email).NotEmpty();

            RuleFor(x => x.RoleName).NotEmpty();
            RuleFor(x => x.RoleName).MaximumLength(50);
        }
    }
}
