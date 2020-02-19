using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Junto.Application.Models.Validator
{
    public class UpdateUserValidator : AbstractValidator<UpdateUser>
    {
        public UpdateUserValidator()
        {
            RuleFor(x => x.Id).NotNull().GreaterThan(0);
            RuleFor(x => x.Login).MinimumLength(5).When(x => !string.IsNullOrEmpty(x.Login));
            RuleFor(x => x.Name).MinimumLength(3).When(x => !string.IsNullOrEmpty(x.Name));
        }
    }
}
