using GameStore.Core.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace GameStore.Web.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch(ItemNotFoundException e) // TODO: Extract method and keep dry
            {
                _logger.LogError(e, "ItemNotFound");
                context.Response.StatusCode = 404;
                context.Response.Headers.Add("exception", "ItemNotFound");
                context.Response.Headers.Add("exceptionMessage", e.Message);
            }
            catch(InvalidOperationException e)
            {
                _logger.LogError(e, "InvalidOperation");
                context.Response.StatusCode = 502;
                context.Response.Headers.Add("exception", "InvalidOperation");
                context.Response.Headers.Add("exceptionMessage", e.Message);
            }
            catch(ArgumentException e)
            {
                _logger.LogError(e, "Argument");
                context.Response.StatusCode = 400;
                context.Response.Headers.Add("exception", "ArgumentException");
                context.Response.Headers.Add("exceptionMessage", e.Message);
            }
            catch(Exception e)
            {
                _logger.LogWarning($"Unhandled exception");
                context.Response.StatusCode = 500;
                context.Response.Headers.Add("exception", "Unknown");
                context.Response.Headers.Add("exceptionMessage", e.Message);
            }
        }
    }
}
