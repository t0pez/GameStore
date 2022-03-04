using Ardalis.ApiEndpoints;
using GameStore.Core.Interfaces;
using GameStore.Core.Models.Games;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
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
        [SwaggerOperation(
            Summary = "Updates game",
            OperationId = "Games.Edit",
            Tags = new[] { "Games" })]
        public override async Task<ActionResult> HandleAsync([FromBody] Game game, CancellationToken cancellationToken = default)
        {
            try
            {
                await _gameService.UpdateAsync(game);
                return Ok();
            }
            catch
            {
                return BadRequest();
            }
        }
    }
}
