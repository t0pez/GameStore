using Ardalis.ApiEndpoints;
using GameStore.Core.Interfaces;
using GameStore.Core.Models.Games;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace GameStore.Web.Endpoints.Games
{
    public class GetAll : EndpointBaseAsync.WithoutRequest.WithResult<ICollection<Game>>
    {
        private readonly IGameService _gameService;

        public GetAll(IGameService gameService)
        {
            _gameService = gameService;
        }

        [HttpGet("games")]
        [SwaggerOperation(
            Summary = "Gets all non-deleted games",
            OperationId = "Games.GetAll",
            Tags = new[] { "Games" })]
        public override async Task<ICollection<Game>> HandleAsync(CancellationToken cancellationToken = default)
        {
            var games = await _gameService.GetAllAsync();

            return games;
        }
    }
}
