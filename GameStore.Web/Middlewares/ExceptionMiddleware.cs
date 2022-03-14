using GameStore.Core.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Threading.Tasks;

namespace GameStore.Web.Middlewares;

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
        catch (ItemNotFoundException e)
        {
            _logger.LogError(e, e.Message);

            context.Response.StatusCode = (int)HttpStatusCode.NotFound;
            AddErrorToResponse(context, e);
        }
        catch (InvalidOperationException e)
        {
            _logger.LogError(e, e.Message);

            context.Response.StatusCode = (int)HttpStatusCode.BadGateway;
            AddErrorToResponse(context, e);
        }
        catch (ArgumentException e)
        {
            _logger.LogError(e, e.Message);

            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            AddErrorToResponse(context, e);
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);

            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            AddErrorToResponse(context, e);
        }
    }

    private static void AddErrorToResponse(HttpContext context, Exception e)
    {
        var exceptionName = e.GetType().Name;
        
        context.Response.Headers.Add("exception", exceptionName);
        context.Response.Headers.Add("exceptionMessage", e.Message);
    }
}