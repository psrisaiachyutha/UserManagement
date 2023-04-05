﻿using Common.Models.Requests;
using Common.Constants;
using FluentValidation;

namespace UserManagementService.Validators
{
    public class LoginRequestDTOValidator : AbstractValidator<LoginRequestDTO>
    {
        public LoginRequestDTOValidator()
        {
            RuleFor(x => x.Email).EmailAddress();
            RuleFor(x => x.Email).NotEmpty();

            RuleFor(x => x.Password).Matches(Constants.PasswordRegexPattern)
                .WithMessage(ErrorMessages.PasswordRegexErrorMessage);
            RuleFor(x => x.Password).NotEmpty();

        }
    }
}
