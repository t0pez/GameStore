using GameStore.Core.Exceptions;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace GameStore.Web.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch(ItemNotFoundException e)
            {
                context.Response.StatusCode = 404;
                context.Response.Headers.Add("exception", "ItemNotFound");
                context.Response.Headers.Add("exceptionMessage", e.Message);
            }
            catch(InvalidOperationException e)
            {
                context.Response.StatusCode = 502;
                context.Response.Headers.Add("exception", "ItemNotFound");
                context.Response.Headers.Add("exceptionMessage", e.Message);
            }
            catch(ArgumentException e)
            {
                context.Response.StatusCode = 400;
                context.Response.Headers.Add("exception", "ArgumentException");
                context.Response.Headers.Add("exceptionMessage", e.Message);
            }
        }
    }
}
