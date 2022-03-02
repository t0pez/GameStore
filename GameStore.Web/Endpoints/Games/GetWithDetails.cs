using Ardalis.ApiEndpoints;
using GameStore.Core.Interfaces;
using GameStore.Core.Models.Games;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace GameStore.Web.Endpoints.Games
{
    public class GetWithDetails : EndpointBaseAsync.WithRequest<string>.WithResult<Game>
    {

        private readonly IGameService _gameService;

        public GetWithDetails(IGameService gameService)
        {
            _gameService = gameService;
        }

        [HttpGet("games/{key}")]
        public override async Task<Game> HandleAsync(string key, CancellationToken cancellationToken = default)
        {
            return await _gameService.GetByKeyAsync(key);
        }
    }
}
