using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Junto.Application.Models.Validator
{
    public class CreateUserValidator : AbstractValidator<CreateUser>
    {
        public CreateUserValidator()
        {
            RuleFor(x => x.Login).NotNull().MinimumLength(5);
            RuleFor(x => x.Name).NotNull().MinimumLength(3);
            RuleFor(x => x.Password).NotNull().MinimumLength(5);
        }
    }
}
