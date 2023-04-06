using Common.Models.Requests;
using FluentValidation;

namespace UserManagementService.Validators
{
    public class CreateRoleRequestDTOValidator: AbstractValidator<CreateRoleRequestDTO>
    {
        public CreateRoleRequestDTOValidator()
        {
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.Name).MaximumLength(50);
        }
    }
}
