using System;
using System.Collections.Generic;
using FluentAssertions;
using GameStore.Web.Models.Baskets;
using GameStore.Web.Services;
using Microsoft.AspNetCore.Http;
using Moq;
using Newtonsoft.Json;
using Xunit;

namespace GameStore.Web.Tests.Services;

public class BasketCookieServiceTests
{
    private readonly BasketCookieService _basketCookieService;
    
    public BasketCookieServiceTests()
    {
        _basketCookieService = new BasketCookieService();
    }

    [Fact]
    public void GetBasketFromCookie_CookiesContainsBasket_ReturnsBasket()
    {
        var expectedResult = new BasketCookieModel
        {
            Items = new List<BasketItemCookieModel>
            {
                new()
                {
                    GameId = Guid.NewGuid(),
                    Quantity = 1
                },
                new()
                {
                    GameId = Guid.NewGuid(),
                    Quantity = 2
                }
            }
        };
        var expectedJson = JsonConvert.SerializeObject(expectedResult);

        var requestCookiesMock = new Mock<IRequestCookieCollection>();
        requestCookiesMock.Setup(collection => collection.TryGetValue(It.IsAny<string>(), out expectedJson))
                          .Returns(true);

        var actualResult = _basketCookieService.GetBasketFromCookie(requestCookiesMock.Object);
        
        expectedResult.Should().BeEquivalentTo(actualResult);
    }

    [Fact]
    public void GetBasketFromCookie_CookieDoesntContainBasket_ReturnsEmptyBasket()
    {
        var expectedResult = new BasketCookieModel();
        var expectedJson = "";
        
        var requestCookiesMock = new Mock<IRequestCookieCollection>();
        requestCookiesMock.Setup(collection => collection.TryGetValue(It.IsAny<string>(), out expectedJson))
                          .Returns(false);
        
        var actualResult = _basketCookieService.GetBasketFromCookie(requestCookiesMock.Object);
        
        expectedResult.Should().BeEquivalentTo(actualResult);
    }

    [Fact]
    public void AppendBasketCookie_CorrectValues_CookieContainsBasketAsJson()
    {
        var basket = new BasketCookieModel
        {
            Items = new List<BasketItemCookieModel>
            {
                new()
                {
                    GameId = Guid.NewGuid(),
                    Quantity = 1
                }
            }
        };

        var expectedResult = JsonConvert.SerializeObject(basket);

        var responseCookieMock = new Mock<IResponseCookies>();
        responseCookieMock.Setup(cookies => cookies.Append(It.IsAny<string>(), expectedResult, It.IsAny<CookieOptions>()))
                          .Verifiable();
        
        _basketCookieService.AppendBasketCookie(responseCookieMock.Object, basket);
        
        responseCookieMock.Verify(cookies => cookies.Append(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CookieOptions>()), Times.Once);
    }
}