using Junto.Api.Infra;
using Junto.Application.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Junto.Test.Infra
{
    public class FilterTest
    {
        [Fact]
        public void ExceptionFilterTestException()
        {
            var exception = new Exception("testException");
            var actionContext = new ActionContext(new DefaultHttpContext(), new RouteData(), new ActionDescriptor());
            var exceptionContext = new ExceptionContext(actionContext, new List<IFilterMetadata>());
            exceptionContext.Exception = exception;
            var filter = new CustomExceptionFilter();
            filter.OnException(exceptionContext);

            Assert.Null(exceptionContext.Exception);
            var result = Assert.IsType<BadRequestObjectResult>(exceptionContext.Result);
            Assert.Equal(result.Value.ToString(), exception.Message);
        }

        [Fact]
        public void ExceptionFilterTestInvalidUserException()
        {
            var exception = new InvalidUserException("testException");
            var actionContext = new ActionContext(new DefaultHttpContext(), new RouteData(), new ActionDescriptor());
            var exceptionContext = new ExceptionContext(actionContext, new List<IFilterMetadata>());
            exceptionContext.Exception = exception;
            var filter = new CustomExceptionFilter();
            filter.OnException(exceptionContext);

            Assert.Null(exceptionContext.Exception);
            var result = Assert.IsType<BadRequestObjectResult>(exceptionContext.Result);
            Assert.Equal(result.Value.ToString(), exception.Message);
        }

        [Fact]
        public void ExceptionFilterTestNotFoundException()
        {
            var exception = new NotFoundException("testException");
            var actionContext = new ActionContext(new DefaultHttpContext(), new RouteData(), new ActionDescriptor());
            var exceptionContext = new ExceptionContext(actionContext, new List<IFilterMetadata>());
            exceptionContext.Exception = exception;
            var filter = new CustomExceptionFilter();
            filter.OnException(exceptionContext);

            Assert.Null(exceptionContext.Exception);
            var result = Assert.IsType<NotFoundObjectResult>(exceptionContext.Result);
            Assert.Equal(result.Value.ToString(), exception.Message);
        }

        [Fact]
        public void ExceptionFilterTestUnexpectedUserException()
        {
            var exception = new UnexpectedUserException("testException");
            var actionContext = new ActionContext(new DefaultHttpContext(), new RouteData(), new ActionDescriptor());
            var exceptionContext = new ExceptionContext(actionContext, new List<IFilterMetadata>());
            exceptionContext.Exception = exception;
            var filter = new CustomExceptionFilter();
            filter.OnException(exceptionContext);

            Assert.Null(exceptionContext.Exception);
            var result = Assert.IsType<BadRequestObjectResult>(exceptionContext.Result);
            Assert.Equal(result.Value.ToString(), exception.Message);
        }
    }
}
