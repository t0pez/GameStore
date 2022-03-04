using Ardalis.ApiEndpoints;
using GameStore.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
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
        [SwaggerOperation(
            Summary = "Adds game to genre",
            OperationId = "Games.AddToGenre",
            Tags = new[] { "Games" })]
        public override async Task<ActionResult> HandleAsync([FromBody] AddToGenreRequest request, CancellationToken cancellationToken = default)
        {
            try
            {
                await _gameService.ApplyGenreAsync(request.GameId, request.GenreId);

                return Ok();
            }
            catch
            {
                return BadRequest();
            }
        }
    }
}
