using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Junto.Application.Models.Validator
{
    public class UpdateUserPasswordValidator : AbstractValidator<UpdateUserPassword>
    {
        public UpdateUserPasswordValidator()
        {
            RuleFor(x => x.Password).NotNull().MinimumLength(5);
        }
    }
}
