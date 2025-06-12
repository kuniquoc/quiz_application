using quiz_application.Application.Exceptions;
using System.Net;
using System.Text.Json;

namespace quiz_application.Api.Middleware
{
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
                await HandleExceptionAsync(context, ex, _logger);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception exception, ILogger logger)
        {
            var code = HttpStatusCode.InternalServerError; // 500 by default
            var message = "An unexpected error occurred.";

            switch (exception)
            {
                case BadRequestException badRequestException:
                    code = HttpStatusCode.BadRequest; // 400
                    message = badRequestException.Message;
                    break;
                case NotFoundException notFoundException:
                    code = HttpStatusCode.NotFound; // 404
                    message = notFoundException.Message;
                    break;
                case UnauthorizedException unauthorizedException:
                    code = HttpStatusCode.Unauthorized; // 401
                    message = unauthorizedException.Message;
                    break;
                case BusinessRuleViolationException businessRuleViolationException:
                    code = HttpStatusCode.UnprocessableEntity; // 422
                    message = businessRuleViolationException.Message;
                    break;
                case Application.Exceptions.ApplicationException applicationException:
                    code = HttpStatusCode.BadRequest;
                    message = applicationException.Message;
                    break;
                default:
                    logger.LogError(exception, "An unhandled exception occurred.");
                    message = "An unexpected error occurred.";
                    break;
            }

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)code;

            var result = JsonSerializer.Serialize(new { error = message });
            await context.Response.WriteAsync(result);
        }
    }
}
