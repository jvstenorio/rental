using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Rental.Domain.Errors;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace Rental.Api.Middlewares
{
    public class ErrorHandlerMiddleware
    {
        private readonly RequestDelegate _requestDelegate;
        private readonly ILogger<ErrorHandlerMiddleware> _logger;

        public ErrorHandlerMiddleware(RequestDelegate requestDelegate, ILogger<ErrorHandlerMiddleware> logger)
        {
            _requestDelegate = requestDelegate;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context) 
        {
            try
            {
                await _requestDelegate(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }
        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            _logger.LogError(exception, "Unexpected Error");
            context.Response.StatusCode = exception is ValidationException ? 
                StatusCodes.Status400BadRequest : 
                StatusCodes.Status500InternalServerError;

            var errorModel = new ErrorModel
            {
                Message = exception.Message
            };
            await context.Response.WriteAsync(JsonConvert.SerializeObject(errorModel));
        }
    }
}
