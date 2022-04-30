using System.Threading.Tasks;
using GameStore.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GameStore.Web.Components;

[ViewComponent(Name = "TotalGamesCount")]
public class TotalGamesCountViewComponent : ViewComponent
{
    private readonly IGameService _gameService;

    public TotalGamesCountViewComponent(IGameService gameService)
    {
        _gameService = gameService;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var gamesCount = await _gameService.GetTotalCountAsync();

        return View(gamesCount);
    }
}