using Ardalis.ApiEndpoints;
using GameStore.Core.Interfaces;
using GameStore.Core.Models.Games;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;

namespace GameStore.Web.Endpoints.Games
{
    public class Update : EndpointBaseAsync.WithRequest<Game>.WithActionResult
    {

        private readonly IGameService _gameService;

        public Update(IGameService gameService)
        {
            _gameService = gameService;
        }

        [HttpPost("games/update")]
        public override async Task<ActionResult> HandleAsync(Game game, CancellationToken cancellationToken = default)
        {
            await _gameService.UpdateAsync(game);
            return Ok();
        }
    }
}
