using Ardalis.ApiEndpoints;
using GameStore.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Threading;
using System.Threading.Tasks;

namespace GameStore.Web.Endpoints.Games
{
    public class New : EndpointBaseAsync
        .WithRequest<NewRequest>
        .WithActionResult<NewResponce>
    {
        private readonly IGameService _gameService;

        public New(IGameService gameService)
        {
            _gameService = gameService;
        }
        
        [HttpPost("games/new")]
        [SwaggerOperation(
            Summary = "Creates new game",
            OperationId = "Games.Create",
            Tags = new[] { "Games" })]
        public override async Task<ActionResult<NewResponce>> HandleAsync([FromBody] NewRequest request, CancellationToken cancellationToken = default)
        {
            try
            {
                var game = await _gameService.CreateAsync(request.Key, request.Name, request.Description, request.File);

                return Ok(new NewResponce
                {
                    Key = game.Key,
                    Name = game.Name,
                    Description = game.Description,
                    File = game.File
                });
            }
            catch
            {
                return NotFound();
            }
        }
    }
}
