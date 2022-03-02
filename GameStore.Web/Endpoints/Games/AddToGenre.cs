using Ardalis.ApiEndpoints;
using GameStore.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;

namespace GameStore.Web.Endpoints.Games
{
    public class AddToGenre : EndpointBaseAsync
        .WithRequest<AddToGenreRequest>
        .WithActionResult
    {
        private readonly IGameService _gameService;

        public AddToGenre(IGameService gameService)
        {
            _gameService = gameService;
        }

        [HttpPost("games/add_to_genre")]
        public override async Task<ActionResult> HandleAsync(AddToGenreRequest request, CancellationToken cancellationToken = default)
        {
            await _gameService.ApplyGenreAsync(request.GameId, request.GenreId);

            return Ok();
        }
    }
}
