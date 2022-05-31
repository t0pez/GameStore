using GameStore.Core.Models.Games;
using GameStore.Core.PagedResult;
using GameStore.Web.Models.Game;

namespace GameStore.Web.ViewModels.Games;

public class GamesGetAllViewModel
{
    public PagedResult<Game> GamesPaged { get; set; }
    public GamesFilterRequestModel Filter { get; set; } = new();
}