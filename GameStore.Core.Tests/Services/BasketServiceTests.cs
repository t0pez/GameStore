using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using GameStore.Core.Exceptions;
using GameStore.Core.Interfaces;
using GameStore.Core.Models.Baskets;
using GameStore.Core.Models.Games;
using GameStore.Core.Services;
using Moq;
using Xunit;

namespace GameStore.Core.Tests.Services;

public class BasketServiceTests
{
    private readonly BasketService _basketService;
    private readonly Mock<IGameService> _gameServiceMock;

    public BasketServiceTests()
    {
        _gameServiceMock = new Mock<IGameService>();
        _basketService = new BasketService(_gameServiceMock.Object);
    }

    [Fact]
    public async void FillWithDetailsAsync_CorrectBasketItems_BasketContainsGameDetails()
    {
        var gameId = Guid.NewGuid();
        var basket = new Basket
        {
            Items = new List<BasketItem>
            {
                new()
                {
                    Game = new Game
                    {
                        Id = gameId
                    }
                }
            }
        };

        const string expectedGameName = "Game 1";

        _gameServiceMock.Setup(service => service.GetByIdAsync(gameId))
                        .ReturnsAsync(new Game { Id = gameId, Name = expectedGameName });

        await _basketService.FillWithDetailsAsync(basket);
        
        var basketItem = basket.Items.First(item => item.Game.Id == gameId);
        basketItem.Game.Name.Should().Be(expectedGameName);
    }
    
    [Fact]
    public async void FillWithDetailsAsync_BasketContainsNotExistingGameId_BasketExcludesWrongData()
    {
        var existingGameId = Guid.NewGuid();
        var notExistingGameId = Guid.NewGuid();
        var basket = new Basket
        {
            Items = new List<BasketItem>
            {
                new()
                {
                    Game = new Game
                    {
                        Id = existingGameId
                    }
                },
                new()
                {
                    Game = new Game
                    {
                        Id = notExistingGameId
                    }
                }
            }
        };

        const string expectedGameName = "Game 1";
        const int expectedCount = 1;

        _gameServiceMock.Setup(service => service.GetByIdAsync(existingGameId))
                        .ReturnsAsync(new Game { Id = existingGameId, Name = expectedGameName });
        _gameServiceMock.Setup(service => service.GetByIdAsync(notExistingGameId))
                        .ThrowsAsync(new ItemNotFoundException());

        await _basketService.FillWithDetailsAsync(basket);
        
        var basketItem = basket.Items.First(item => item.Game.Id == existingGameId);
        basketItem.Game.Name.Should().Be(expectedGameName);
        basket.Items.Should().HaveCount(expectedCount);
    }

    [Fact]
    public async void AddToBasket_BasketNotContainsGame_AddsGameInBasket()
    {
        var basket = new Basket();

        var gameId = Guid.NewGuid();
        var quantity = 2;

        const int expectedGamesCount = 1;

        await _basketService.AddToBasketAsync(basket, gameId, quantity);

        basket.Items.Should().HaveCount(expectedGamesCount)
              .And.Contain(item => item.Game.Id == gameId);
    }
    
    [Fact]
    public async void AddToBasket_BasketContainsGame_IncreaseGameQuantity()
    {
        var gameId = Guid.NewGuid();
        var newQuantity = 2;
        
        const int defaultQuantity = 1;
        var item = new BasketItem
        {
            Game = new Game { Id = gameId },
            Quantity = defaultQuantity
        };
        var basket = new Basket
        {
            Items = new List<BasketItem>
            {
                item
            }
        };

        const int expectedGamesCount = 1;
        const int expectedGameQuantity = 3;

        await _basketService.AddToBasketAsync(basket, gameId, newQuantity);

        var basketItem = basket.Items.First(item => item.Game.Id == gameId);
        basket.Items.Should().HaveCount(expectedGamesCount)
              .And.Contain(basketItem => basketItem.Game.Id == gameId);
        basketItem.Quantity.Should().Be(expectedGameQuantity);
    }
}