using FluentAssertions;
using Junto.Api.Infra;
using Junto.Test.Helper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing;
using System.Linq;
using Xunit;

namespace Junto.Test.Infra
{
    public class FluentValidatorFormatTest
    {
        [Fact]
        public void TestInvalidModelByFluentValidation()
        {
            var errors = new ModelStateDictionary();
            errors.AddModelError("Name", "name cant be empty");
            errors.AddModelError("Login", "login cant be empty");

            var actionContext = new ActionContext(new DefaultHttpContext(), new RouteData(), new ActionDescriptor(), errors);
            var response = Api.Infra.FluentValidation.FormatResponse(actionContext);
            var requestResult = Assert.IsType<BadRequestObjectResult>(response);
            var result = requestResult.Deserialize<FluentValidationModelStateErrors>();
            result.Should().NotBeNull().And.BeOfType<FluentValidationModelStateErrors>().Which.Errors.Should().HaveCount(2);
        }
    }
}
