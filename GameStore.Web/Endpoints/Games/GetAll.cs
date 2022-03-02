using Ardalis.ApiEndpoints;
using GameStore.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;

namespace GameStore.Web.Endpoints.Games
{
    public class GetAll : EndpointBaseAsync.WithoutRequest.WithResult<GetAllResponce>
    {
        private readonly IGameService _gameService;

        public GetAll(IGameService gameService)
        {
            _gameService = gameService;
        }

        [HttpGet("games")]
        public override async Task<GetAllResponce> HandleAsync(CancellationToken cancellationToken = default)
        {
            var games = await _gameService.GetAllAsync();

            var result = new GetAllResponce
            {
                Games = games
            };

            return result;
        }
    }
}
