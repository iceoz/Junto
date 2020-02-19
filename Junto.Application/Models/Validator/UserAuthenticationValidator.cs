using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Junto.Application.Models.Validator
{
    public class UserAuthenticationValidator : AbstractValidator<UserAuthentication>
    {
        public UserAuthenticationValidator()
        {
            RuleFor(x => x.Login).NotNull().MinimumLength(5);
            RuleFor(x => x.Password).NotNull().MinimumLength(5);
        }
    }
}
