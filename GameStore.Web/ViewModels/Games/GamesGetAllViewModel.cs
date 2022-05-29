using System.Collections.Generic;
using GameStore.Web.Models.Game;

namespace GameStore.Web.ViewModels.Games;

public class GamesGetAllViewModel
{
    public IEnumerable<GameListViewModel> Games { get; set; }
    public GamesFilterRequestModel Filter { get; set; } = new();
}