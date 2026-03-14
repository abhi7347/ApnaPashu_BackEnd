using APNAPASHU.DataContract.Models;
using APNAPASHU.Common.Exceptions;
using System.Net;

namespace APNAPASHU.API.Middlewares
{
    /// <summary>
    /// Global Error Handling Middleware
    /// </summary>
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlingMiddleware> _logger;

        public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception in middleware");
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            var response = new JsonModel<object>();

            if (exception is CustomException customException)
            {
                context.Response.StatusCode = customException.StatusCode;
                response = new JsonModel<object>(
                    null,
                    customException.Message,
                    customException.StatusCode,
                    customException.ErrorCode
                );
            }
            else if (exception is UnauthorizedAccessException)
            {
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                response = new JsonModel<object>(
                    null,
                    "Unauthorized",
                    401,
                    "UNAUTHORIZED"
                );
            }
            else if (exception is ArgumentException || exception is ArgumentNullException)
            {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                response = new JsonModel<object>(
                    null,
                    exception.Message,
                    400,
                    "BAD_REQUEST"
                );
            }
            else
            {
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                response = new JsonModel<object>(
                    null,
                    "An internal error occurred",
                    500,
                    "INTERNAL_ERROR"
                );
            }

            return context.Response.WriteAsJsonAsync(response);
        }
    }

    /// <summary>
    /// Extension method to add error handling middleware
    /// </summary>
    public static class ErrorHandlingMiddlewareExtensions
    {
        public static IApplicationBuilder UseErrorHandlingMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ErrorHandlingMiddleware>();
        }
    }
}