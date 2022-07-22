using GameStore.Core.Models.Dto;
using GameStore.Core.PagedResult;
using GameStore.Web.Models.Game;

namespace GameStore.Web.ViewModels.Games;

public class GamesGetAllViewModel
{
    public PagedResult<ProductDto> GamesPaged { get; set; }
    public GamesFilterRequestModel Filter { get; set; } = new();
}