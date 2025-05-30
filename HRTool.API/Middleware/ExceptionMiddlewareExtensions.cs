using System.Net;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace HRTool.API.Middleware
{
    /// <summary>
    /// Middleware for global exception handling. Returns standardized error responses and logs exceptions.
    /// </summary>
    public static class ExceptionMiddlewareExtensions
    {
        public static void UseCustomExceptionHandler(this IApplicationBuilder app, ILoggerFactory loggerFactory)
        {
            var env = app.ApplicationServices.GetService<IWebHostEnvironment>();
            app.UseExceptionHandler(errorApp =>
            {
                errorApp.Run(async context =>
                {
                    var logger = loggerFactory.CreateLogger("GlobalExceptionHandler");
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    context.Response.ContentType = "application/json";

                    var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
                    var error = exceptionHandlerPathFeature?.Error;

                    logger.LogError(error, "Unhandled exception occurred: {Message}", error?.Message);

                    var problem = new ProblemDetails
                    {
                        Status = StatusCodes.Status500InternalServerError,
                        Title = "An unexpected error occurred.",
                        Detail = env != null && env.IsDevelopment() ? error?.ToString() : null
                    };
                    await context.Response.WriteAsJsonAsync(problem);
                });
            });
        }
    }
}
