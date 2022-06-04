using System;
using FluentAssertions;
using GameStore.Web.Services;
using Microsoft.AspNetCore.Http;
using Moq;
using Xunit;

namespace GameStore.Web.Tests.Services;

public class ActiveOrderCookieServiceTests
{
    private readonly ActiveOrderCookieService _activeOrderCookieService;

    public ActiveOrderCookieServiceTests()
    {
        _activeOrderCookieService = new ActiveOrderCookieService();
    }

    [Fact]
    public void IsCookieContainsActiveOrder_CookieContainsOrder_ReturnsTrue()
    {
        const bool expectedResult = true;

        var requestCookiesMock = new Mock<IRequestCookieCollection>();
        requestCookiesMock.Setup(collection => collection.ContainsKey(It.IsAny<string>()))
                          .Returns(expectedResult);

        var actualResult = _activeOrderCookieService.IsCookieContainsActiveOrder(requestCookiesMock.Object);

        actualResult.Should().Be(expectedResult);
    }

    [Fact]
    public void IsCookieContainsActiveOrder_CookieNotContainsOrder_ReturnsFalse()
    {
        const bool expectedResult = false;

        var requestCookiesMock = new Mock<IRequestCookieCollection>();
        requestCookiesMock.Setup(collection => collection.ContainsKey(It.IsAny<string>()))
                          .Returns(expectedResult);

        var actualResult = _activeOrderCookieService.IsCookieContainsActiveOrder(requestCookiesMock.Object);

        actualResult.Should().Be(expectedResult);
    }

    [Fact]
    public void TryGetActiveOrderId_CookieContainsOrder_ReturnsCorrectOrderId()
    {
        const bool expectedResult = true;
        var expectedOrderId = Guid.NewGuid();
        var expectedCookieValue = expectedOrderId.ToString();

        var requestCookiesMock = new Mock<IRequestCookieCollection>();
        requestCookiesMock.Setup(collection => collection.TryGetValue(It.IsAny<string>(), out expectedCookieValue))
                          .Returns(true);

        var actualResult =
            _activeOrderCookieService.TryGetActiveOrderId(requestCookiesMock.Object, out var actualOrderId);


        actualResult.Should().Be(expectedResult);
        actualOrderId.Should().Be(expectedOrderId.ToString());
    }

    [Fact]
    public void TryGetActiveOrderId_CookieNotContainsOrder_ReturnsFalseAndEmptyGuid()
    {
        const bool expectedResult = false;
        var expectedCookieValue = "";

        var requestCookiesMock = new Mock<IRequestCookieCollection>();
        requestCookiesMock.Setup(collection => collection.TryGetValue(It.IsAny<string>(), out expectedCookieValue))
                          .Returns(expectedResult);

        var actualResult =
            _activeOrderCookieService.TryGetActiveOrderId(requestCookiesMock.Object, out var actualOrderId);
        
        actualResult.Should().Be(expectedResult);
        actualOrderId.Should().Be(Guid.Empty);
    }

    [Fact]
    public void AppendActiveOrder_CookieNotContainsOrder_Returns()
    {
        var expectedOrderId = Guid.NewGuid();

        var responseCookies = new Mock<IResponseCookies>();
        responseCookies.Setup(collection =>
                                  collection.Append(It.IsAny<string>(), expectedOrderId.ToString(),
                                                    It.IsAny<CookieOptions>()))
                       .Verifiable();

        _activeOrderCookieService.AppendActiveOrder(responseCookies.Object, expectedOrderId);

        responseCookies.Verify(
            cookies => cookies.Append(It.IsAny<string>(), expectedOrderId.ToString(), It.IsAny<CookieOptions>()));
    }
}