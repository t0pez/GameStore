using System;
using AutoMapper;
using FluentAssertions;
using GameStore.Core.Interfaces;
using GameStore.Core.Models.Baskets;
using GameStore.Web.Controllers;
using GameStore.Web.Interfaces;
using GameStore.Web.Models.Baskets;
using GameStore.Web.ViewModels.Baskets;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace GameStore.Web.Tests.Controllers;

public class BasketControllerTests
{
    private readonly BasketController _basketController;
    private readonly Mock<IBasketService> _basketServiceMock;
    private readonly Mock<IBasketCookieService> _basketCookieServiceMock;
    private readonly Mock<IActiveOrderCookieService> _activeOrderCookieService;
    private readonly Mock<IMapper> _mapperMock;

    public BasketControllerTests()
    {
        _basketServiceMock = new Mock<IBasketService>();
        _basketCookieServiceMock = new Mock<IBasketCookieService>();
        _activeOrderCookieService = new Mock<IActiveOrderCookieService>();
        _mapperMock = new Mock<IMapper>();

        _basketController = new BasketController(_basketServiceMock.Object, _basketCookieServiceMock.Object,
                                                 _activeOrderCookieService.Object, _mapperMock.Object);
        
        _basketController.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };
    }

    [Fact]
    public async void GetCurrentBasketAsync_NoParameters_ReturnsView()
    {
        var basketCookieModel = new BasketCookieModel();
        _basketCookieServiceMock.Setup(service => service.GetBasketFromCookie(It.IsAny<IRequestCookieCollection>()))
                                .Returns(basketCookieModel);
        var basket = new Basket();
        _mapperMock.Setup(mapper => mapper.Map<Basket>(basketCookieModel));

        var basketViewModel = new BasketViewModel();
        _mapperMock.Setup(mapper => mapper.Map<BasketViewModel>(basket));

        var actualResult = await _basketController.GetCurrentBasketAsync();

        actualResult.Should().BeAssignableTo<ViewResult>();
    }
    
    [Fact]
    public async void AddToBasketAsync_CorrectValues_ReturnsRedirect()
    {
        const int quantity = 1;
        var gameId = Guid.NewGuid();

        var basketCookieModel = new BasketCookieModel();
        _basketCookieServiceMock.Setup(service => service.GetBasketFromCookie(It.IsAny<IRequestCookieCollection>()))
                                .Returns(basketCookieModel);
        var basket = new Basket();
        _mapperMock.Setup(mapper => mapper.Map<Basket>(basketCookieModel));
        
        var actualResult = await _basketController.AddToBasketAsync(gameId, quantity);

        actualResult.Should().BeAssignableTo<RedirectToActionResult>();
    }
}