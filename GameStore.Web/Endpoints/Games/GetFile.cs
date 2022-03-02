using Ardalis.ApiEndpoints;
using GameStore.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;

namespace GameStore.Web.Endpoints.Games
{
    public class GetFile : EndpointBaseAsync.WithRequest<string>.WithResult<byte[]>
    {
        private readonly IGameService _gameService;

        public GetFile(IGameService gameService)
        {
            _gameService = gameService;
        }

        [HttpPost("games/{key}/download")]
        public override async Task<byte[]> HandleAsync(string request, CancellationToken cancellationToken = default)
        {
            return await _gameService.GetFileAsync(request);
        }
    }
}
