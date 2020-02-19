using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Junto.Api.Infra
{
    public static class FluentValidation
    {
        public static IActionResult FormatResponse(ActionContext context)
        {
            var errors = context.ModelState.Values.SelectMany(x => x.Errors.Select(e => e.ErrorMessage))
                        .ToList();

            var result = new FluentValidationModelStateErrors
            {
                Message = "Validation Errors",
                Errors = errors
            };

            return new BadRequestObjectResult(result);
        } 
    }
}
