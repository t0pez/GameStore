using Ardalis.ApiEndpoints;
using GameStore.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
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
        public override async Task<ActionResult> HandleAsync(Guid id, CancellationToken cancellationToken = default)
        {
            await _gameService.DeleteAsync(id);

            return Ok();
        }
    }
}
