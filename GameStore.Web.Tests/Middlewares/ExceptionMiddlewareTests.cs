using System;
using System.Net;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using GameStore.Core.Exceptions;
using GameStore.Tests.Infrastructure.Attributes;
using GameStore.Web.Middlewares;
using Microsoft.AspNetCore.Http;
using Moq;
using Xunit;

namespace GameStore.Web.Tests.Middlewares;

public class ExceptionMiddlewareTests
{
    [Theory]
    [AutoMoqData]
    public async Task Invoke_ThrowsItemNotFound_AddsCorrectStatusCode(
        [Frozen] Mock<HttpContext> httpContextMock,
        [Frozen] Mock<RequestDelegate> requestDelegateMock,
        ExceptionMiddleware sut)
    {
        requestDelegateMock.Setup(rd => rd.Invoke(It.IsAny<HttpContext>()))
                           .ThrowsAsync(new ItemNotFoundException());

        await sut.InvokeAsync(httpContextMock.Object, requestDelegateMock.Object);

        httpContextMock.Verify(context => context.Response.Redirect(It.Is<string>(s => s.Contains(
                                                                        ((int)HttpStatusCode
                                                                           .NotFound).ToString()))));
    }

    [Theory]
    [AutoMoqData]
    public async Task Invoke_ThrowsInvalidOperationException_AddsCorrectStatusCode(
        [Frozen] Mock<HttpContext> httpContextMock,
        [Frozen] Mock<RequestDelegate> requestDelegateMock,
        ExceptionMiddleware sut)
    {
        requestDelegateMock.Setup(rd => rd.Invoke(It.IsAny<HttpContext>()))
                           .ThrowsAsync(new InvalidOperationException());

        await sut.InvokeAsync(httpContextMock.Object, requestDelegateMock.Object);

        httpContextMock.Verify(context => context.Response.Redirect(It.Is<string>(s => s.Contains(
                                                                        ((int)HttpStatusCode
                                                                           .BadGateway).ToString()))));
    }

    [Theory]
    [AutoMoqData]
    public async Task Invoke_ThrowsArgumentException_AddsCorrectStatusCode(
        [Frozen] Mock<HttpContext> httpContextMock,
        [Frozen] Mock<RequestDelegate> requestDelegateMock,
        ExceptionMiddleware sut)
    {
        requestDelegateMock.Setup(rd => rd.Invoke(It.IsAny<HttpContext>()))
                           .ThrowsAsync(new ArgumentException());

        await sut.InvokeAsync(httpContextMock.Object, requestDelegateMock.Object);

        httpContextMock.Verify(context => context.Response.Redirect(It.Is<string>(s => s.Contains(
                                                                        ((int)HttpStatusCode
                                                                           .BadRequest).ToString()))));
    }

    [Theory]
    [AutoMoqData]
    public async Task Invoke_ThrowsUnsupportedException_AddsCorrectStatusCode(
        [Frozen] Mock<HttpContext> httpContextMock,
        [Frozen] Mock<RequestDelegate> requestDelegateMock,
        ExceptionMiddleware sut)
    {
        requestDelegateMock.Setup(rd => rd.Invoke(It.IsAny<HttpContext>()))
                           .ThrowsAsync(new Exception());

        await sut.InvokeAsync(httpContextMock.Object, requestDelegateMock.Object);

        httpContextMock.Verify(context => context.Response.Redirect(It.Is<string>(s => s.Contains(
                                                                        ((int)HttpStatusCode
                                                                           .InternalServerError).ToString()))));
    }
}