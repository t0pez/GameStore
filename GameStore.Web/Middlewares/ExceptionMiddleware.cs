using System;
using System.Net;
using System.Threading.Tasks;
using GameStore.Core.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace GameStore.Web.Middlewares;

public class ExceptionMiddleware : IMiddleware
{
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(ILogger<ExceptionMiddleware> logger)
    {
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        int? statusCode = null;
        string message = default;

        try
        {
            await next(context);
        }
        catch (ItemNotFoundException e)
        {
            _logger.LogError(e, e.Message);

            statusCode = (int)HttpStatusCode.NotFound;
            message = e.Message;
        }
        catch (InvalidOperationException e)
        {
            _logger.LogError(e, e.Message);

            statusCode = (int)HttpStatusCode.BadGateway;
            message = e.Message;
        }
        catch (ArgumentException e)
        {
            _logger.LogError(e, e.Message);

            statusCode = (int)HttpStatusCode.BadRequest;
            message = e.Message;
        }
        catch (UnauthorizedAccessException e)
        {
            _logger.LogError(e, e.Message);

            statusCode = (int)HttpStatusCode.Unauthorized;
            message = e.Message;
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);

            statusCode = (int)HttpStatusCode.InternalServerError;
            message = e.Message;
        }
        finally
        {
            if (statusCode.HasValue)
            {
                context.Response.Redirect($"/error?statusCode={statusCode}&message={message}");
            }
        }
    }
}