using Ardalis.ApiEndpoints;
using GameStore.Core.Interfaces;
using GameStore.Core.Models.Comments;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace GameStore.Web.Endpoints.Games
{
    public class GetComments : EndpointBaseAsync.WithRequest<string>.WithActionResult<ICollection<Comment>>
    {
        private readonly ICommentService _commentService;

        public GetComments(ICommentService commentService)
        {
            _commentService = commentService;
        }

        [HttpGet("games/{gameKey}/comments")]
        [SwaggerOperation(
            Summary = "Gets all comment of game",
            OperationId = "Games.GetComments",
            Tags = new[] { "Games" })]
        public override async Task<ActionResult<ICollection<Comment>>> HandleAsync([FromRoute(Name = "gameKey")] string gameKey, CancellationToken cancellationToken = default)
        {
            try
            {
                return Ok(await _commentService.GetCommentsByGameKeyAsync(gameKey));
            }
            catch
            {
                return NotFound();
            }
        }
    }
}
