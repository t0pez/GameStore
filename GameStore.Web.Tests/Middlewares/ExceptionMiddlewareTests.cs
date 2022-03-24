using System;
using System.Net;
using GameStore.Core.Exceptions;
using GameStore.Web.Middlewares;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace GameStore.Web.Tests.Middlewares;

public class ExceptionMiddlewareTests
{
    private readonly Mock<RequestDelegate> _requestDelegateMock;
    private readonly Mock<HttpContext> _httpContextMock;
    private readonly Mock<ILogger<ExceptionMiddleware>> _loggerMock;

    public ExceptionMiddlewareTests()
    {
        _loggerMock = new Mock<ILogger<ExceptionMiddleware>>();
        _httpContextMock = new Mock<HttpContext>();
        _httpContextMock.Setup(context => context.Response.StatusCode)
                        .Verifiable();
        _requestDelegateMock = new Mock<RequestDelegate>();
    }

    [Fact]
    public void Invoke_ThrowsItemNotFound_AddsCorrectStatusCode()
    {
        _requestDelegateMock.Setup(rd => rd.Invoke(It.IsAny<HttpContext>()))
                            .ThrowsAsync(new ItemNotFoundException());

        var exceptionMiddleware = new ExceptionMiddleware(_requestDelegateMock.Object, _loggerMock.Object);
        exceptionMiddleware.Invoke(_httpContextMock.Object);
        
        _httpContextMock.VerifySet(context => context.Response.StatusCode = (int)HttpStatusCode.NotFound);
    }
    
    [Fact]
    public void Invoke_ThrowsInvalidOperationException_AddsCorrectStatusCode()
    {
        _requestDelegateMock.Setup(rd => rd.Invoke(It.IsAny<HttpContext>()))
                            .ThrowsAsync(new InvalidOperationException());

        var exceptionMiddleware = new ExceptionMiddleware(_requestDelegateMock.Object, _loggerMock.Object);
        exceptionMiddleware.Invoke(_httpContextMock.Object);
        
        _httpContextMock.VerifySet(context => context.Response.StatusCode = (int)HttpStatusCode.BadGateway);
    } 
    
    [Fact]
    public void Invoke_ThrowsArgumentException_AddsCorrectStatusCode()
    {
        _requestDelegateMock.Setup(rd => rd.Invoke(It.IsAny<HttpContext>()))
                            .ThrowsAsync(new ArgumentException());

        var exceptionMiddleware = new ExceptionMiddleware(_requestDelegateMock.Object, _loggerMock.Object);
        exceptionMiddleware.Invoke(_httpContextMock.Object);
        
        _httpContextMock.VerifySet(context => context.Response.StatusCode = (int)HttpStatusCode.BadRequest);
    }
    
    [Fact]
    public void Invoke_ThrowsUnsupportedException_AddsCorrectStatusCode()
    {
        _requestDelegateMock.Setup(rd => rd.Invoke(It.IsAny<HttpContext>()))
                            .ThrowsAsync(new Exception());

        var exceptionMiddleware = new ExceptionMiddleware(_requestDelegateMock.Object, _loggerMock.Object);
        exceptionMiddleware.Invoke(_httpContextMock.Object);
        
        _httpContextMock.VerifySet(context => context.Response.StatusCode = (int)HttpStatusCode.InternalServerError);
    }
}