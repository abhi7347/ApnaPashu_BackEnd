using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using APNAPASHU.DataContract.Models;
using APNAPASHU.Common.Exceptions;

namespace APNAPASHU.API.Filters
{
    /// <summary>
    /// Global API Exception Filter
    /// </summary>
    public class ApiExceptionFilterAttribute : ExceptionFilterAttribute
    {
        private readonly ILogger<ApiExceptionFilterAttribute> _logger;

        public ApiExceptionFilterAttribute(ILogger<ApiExceptionFilterAttribute> logger)
        {
            _logger = logger;
        }

        public override void OnException(ExceptionContext context)
        {
            var exception = context.Exception;
            _logger.LogError(exception, "Global exception caught");

            var response = new JsonModel<object>();

            if (exception is CustomException customException)
            {
                response = new JsonModel<object>(
                    null,
                    customException.Message,
                    customException.StatusCode,
                    customException.ErrorCode
                );
                context.HttpContext.Response.StatusCode = customException.StatusCode;
            }
            else if (exception is ArgumentNullException || exception is ArgumentException)
            {
                response = new JsonModel<object>(
                    null,
                    "Invalid argument provided",
                    400,
                    "INVALID_ARGUMENT"
                );
                context.HttpContext.Response.StatusCode = 400;
            }
            else if (exception is UnauthorizedAccessException)
            {
                response = new JsonModel<object>(
                    null,
                    "Unauthorized access",
                    401,
                    "UNAUTHORIZED"
                );
                context.HttpContext.Response.StatusCode = 401;
            }
            else
            {
                response = new JsonModel<object>(
                    null,
                    "An unexpected error occurred",
                    500,
                    exception.GetType().Name
                );
                context.HttpContext.Response.StatusCode = 500;
            }

            context.Result = new JsonResult(response);
            base.OnException(context);
        }
    }
}