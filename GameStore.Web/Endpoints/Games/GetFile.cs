using Ardalis.ApiEndpoints;
using GameStore.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Threading;
using System.Threading.Tasks;

namespace GameStore.Web.Endpoints.Games
{
    public class GetFile : EndpointBaseAsync.WithRequest<string>.WithActionResult<byte[]>
    {
        private readonly IGameService _gameService;

        public GetFile(IGameService gameService)
        {
            _gameService = gameService;
        }

        [HttpPost("games/{gameKey}/download")]
        [SwaggerOperation(
            Summary = "Gets game file",
            OperationId = "Games.GetFile",
            Tags = new[] { "Games" })]
        public override async Task<ActionResult<byte[]>> HandleAsync([FromRoute(Name = "gameKey")] string gameKey, CancellationToken cancellationToken = default)
        {
            try
            {
                return Ok(await _gameService.GetFileAsync(gameKey));
            }
            catch
            {
                return NotFound();
            }
        }
    }
}
