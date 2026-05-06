using Contracts.Exceptions;
using Microsoft.AspNetCore.Mvc;
using System.Numerics;

namespace API_Layer.Middleware
{
    public class GlobalExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;

        public GlobalExceptionHandlerMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlerMiddleware> logger)
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
                var traceId = context.TraceIdentifier;

                _logger.LogError(ex, "An unhandled exception. TraceID: {traceID}", traceId);

                var problem = MapToProblemDetails(ex, context);

                context.Response.StatusCode = problem.Status ?? StatusCodes.Status500InternalServerError;
                context.Response.ContentType = "application/json";

                await context.Response.WriteAsJsonAsync(problem);
            }
        }

        private ProblemDetails MapToProblemDetails(Exception ex, HttpContext context)
        {
            return ex switch
            {
                DuplicateRecordException => new ProblemDetails
                {
                    Title = "Duplicate Resource",
                    Status = 409,
                    Detail = "Resource already exists",
                    Type = "/errors/duplicate",
                    Instance = context.Request.Path
                },

                InvalidReferenceTypeException => new ProblemDetails
                {
                    Title = "Invalid Reference",
                    Status = 400,
                    Detail = "Related entity not found",
                    Type = "/errors/invalid-reference",
                    Instance = context.Request.Path
                },

                _ => new ProblemDetails
                {
                    Title = "Server Error",
                    Status = 500,
                    Detail = ex.Message,
                    Type = "/errors/server-error",
                    Instance = context.Request.Path
                }
            };
        }
    }
}