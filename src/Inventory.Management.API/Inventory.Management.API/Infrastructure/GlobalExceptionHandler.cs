using Inventory.Management.Domain.Common;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Inventory.Management.API.Infrastructure
{
    internal sealed class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
        {
            logger.LogError(exception, "Unhandled exception occurred");

            var details = new ProblemDetails();

            if (exception is DomainException)
            {
                details.Title = "One or more validation errors ocurred";
                details.Status = StatusCodes.Status422UnprocessableEntity;
                details.Type = "UnprocessableEntity";
                details.Detail = exception!.Message;
            }
            else
            {
                details.Title = "Server failure";
                details.Status = StatusCodes.Status500InternalServerError;
                details.Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.6.1";
            }

            httpContext.Response.StatusCode = details.Status.Value;

            await httpContext.Response.WriteAsJsonAsync(details, cancellationToken);

            return true;
        }
    }
}
