using System;
using System.Linq;
using System.Threading.Tasks;
using GameStore.Core.Interfaces;
using GameStore.Core.Models.Baskets;
using GameStore.Core.Models.Games;

namespace GameStore.Core.Services;

public class BasketService : IBasketService
{
    private readonly IGameService _gameService;

    public BasketService(IGameService gameService)
    {
        _gameService = gameService;
    }

// TODO: In this methods exception can be thrown.
// It means that all times when trying to invoke this ExceptionMiddleware will catch it and basket would be broken until cookie alive
    public async Task FillWithDetailsAsync(Basket basket)
    {
        foreach (var basketItem in basket.Items)
        {
            var game = await _gameService.GetByIdAsync(basketItem.Game.Id);
            basketItem.Game = game;
        }
    }

    public void AddToBasket(Basket basket, Guid gameId, int quantity)
    {
        if (IsBasketContainGame(basket, gameId))
        {
            IncreaseQuantity(basket, gameId, quantity); // TODO: Can be changed by PO
        }
        else
        {
            AddNewGame(basket, gameId, quantity);
        }
    }

    private static void AddNewGame(Basket basket, Guid gameId, int quantity)
    {
        var item = new BasketItem
        {
            Game = new Game { Id = gameId },
            Quantity = quantity
        };

        basket.Items.Add(item);
    }

    private void IncreaseQuantity(Basket basket, Guid gameId, int quantity)
    {
        var existingGame = basket.Items.First(model => model.Game.Id == gameId);
        existingGame.Quantity += quantity;
    }

    private bool IsBasketContainGame(Basket basket, Guid gameId)
    {
        return basket.Items.Any(model => model.Game.Id == gameId);
    }
}