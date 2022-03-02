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
        .WithResult<NewResponce>
    {
        private readonly IGameService _gameService;

        public New(IGameService gameService)
        {
            _gameService = gameService;
        }
        
        [HttpPost("games/new")]
        [SwaggerOperation(
            OperationId = "Games.New",
            Tags = new[] { "Games" })]
        public override async Task<NewResponce> HandleAsync(NewRequest request, CancellationToken cancellationToken = default)
        {
            var game = await _gameService.CreateAsync(request.Key, request.Name, request.Description, request.File);

            return new NewResponce
            {
                Key = game.Key,
                Name = game.Name,
                Description = game.Description,
                File = game.File
            };
        }
    }
}
