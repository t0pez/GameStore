using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameStore.Core.Exceptions;
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
    
    public async Task FillWithDetailsAsync(Basket basket)
    {
        var itemsToDelete = new List<BasketItem>();
        foreach (var basketItem in basket.Items)
        {
            var currentGameId = basketItem.Game.Id;
            try
            {
                var game = await _gameService.GetByIdAsync(currentGameId);
                basketItem.Game = game;
            }
            catch (ItemNotFoundException)
            {
                itemsToDelete.Add(basketItem);
            }
        }
        basket.Items = basket.Items.Except(itemsToDelete).ToList();
    }

    public async Task AddToBasketAsync(Basket basket, Guid gameId, int quantity)
    {
        await AssertGameExists(gameId);

        if (IsBasketContainGame(basket, gameId))
        {
            IncreaseQuantity(basket, gameId, quantity);
        }
        else
        {
            AddNewGame(basket, gameId, quantity);
        }
    }

    private void AddNewGame(Basket basket, Guid gameId, int quantity)
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

    private async Task AssertGameExists(Guid gameId)
    {
        _ = await _gameService.GetByIdAsync(gameId);
    }
}