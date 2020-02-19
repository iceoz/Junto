using Junto.Application.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Junto.Api.Infra
{
    public class CustomExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            switch (context.Exception)
            {
                case NotFoundException _:
                    {
                        context.Result = new NotFoundObjectResult(context.Exception.Message);
                        break;
                    }
                default:
                    {
                        context.Result = new BadRequestObjectResult(context.Exception.Message);
                        break;
                    }
            }

            context.Exception = null;
            return;
        }
    }
}