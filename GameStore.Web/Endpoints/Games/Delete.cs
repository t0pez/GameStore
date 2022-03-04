using Ardalis.ApiEndpoints;
using GameStore.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace GameStore.Web.Endpoints.Games
{
    public class Delete : EndpointBaseAsync.WithRequest<Guid>.WithActionResult
    {
        private readonly IGameService _gameService;

        public Delete(IGameService gameService)
        {
            _gameService = gameService;
        }

        [HttpPost("games/remove")]
        [SwaggerOperation(
            Summary = "Deletes game",
            OperationId = "Games.Delete",
            Tags = new[] { "Games" })]
        public override async Task<ActionResult> HandleAsync([FromBody] Guid id, CancellationToken token = default)
        {
            try
            {
                await _gameService.DeleteAsync(id, token);

                return Ok();
            }
            catch (InvalidOperationException)
            {
                return BadRequest();
            }
        }
    }
}
