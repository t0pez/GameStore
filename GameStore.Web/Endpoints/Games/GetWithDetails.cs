using Ardalis.ApiEndpoints;
using GameStore.Core.Interfaces;
using GameStore.Core.Models.Games;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace GameStore.Web.Endpoints.Games
{
    public class GetWithDetails : EndpointBaseAsync.WithRequest<string>.WithActionResult<Game>
    {

        private readonly IGameService _gameService;

        public GetWithDetails(IGameService gameService)
        {
            _gameService = gameService;
        }

        [HttpGet("games/{gameKey}")]
        [SwaggerOperation(
            Summary = "Gets game with details by key",
            OperationId = "Games.GetWithDetails",
            Tags = new[] { "Games" })]
        public override async Task<ActionResult<Game>> HandleAsync([FromRoute(Name = "gameKey")] string gameKey, CancellationToken cancellationToken = default)
        {
            try
            {
                return Ok(await _gameService.GetByKeyAsync(gameKey));
            }
            catch
            {
                return NotFound();
            }
        }
    }
}
