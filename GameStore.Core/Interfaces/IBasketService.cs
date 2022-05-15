using System;
using System.Threading.Tasks;
using GameStore.Core.Models.Baskets;

namespace GameStore.Core.Interfaces;

public interface IBasketService
{
    public Task FillWithDetailsAsync(Basket basket);
    public Task AddToBasketAsync(Basket basket, Guid gameId, int quantity);
}