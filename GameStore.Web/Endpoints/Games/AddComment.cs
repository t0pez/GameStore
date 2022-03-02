using Ardalis.ApiEndpoints;
using GameStore.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;

namespace GameStore.Web.Endpoints.Games
{
    public class AddComment : EndpointBaseAsync.WithRequest<AddCommentRequest>.WithActionResult
    {
        private readonly ICommentService _commentService;

        public AddComment(ICommentService commentService)
        {
            _commentService = commentService;
        }

        [HttpPost("games/{key}/newcomment")]
        public override async Task<ActionResult> HandleAsync(AddCommentRequest request, CancellationToken cancellationToken = default)
        {
            await _commentService.CommentGameAsync(request.GameKey, request.AuthorName, request.Message);

            return Ok();
        }
    }
}
