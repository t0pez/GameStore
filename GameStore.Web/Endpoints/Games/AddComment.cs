using Ardalis.ApiEndpoints;
using GameStore.Core.Interfaces;
using GameStore.Web.Attributes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NLog;
using Swashbuckle.AspNetCore.Annotations;
using System;
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

        [HttpPost("games/{gameKey}/newcomment")]
        [SwaggerOperation(
            Summary = "Adds comment to game",
            OperationId = "Games.AddComment",
            Tags = new[] { "Games" })]
        public override async Task<ActionResult> HandleAsync([FromMultiSource] AddCommentRequest request, CancellationToken token = default)
        {
            try
            {
                await _commentService.CommentGameAsync(request.GameKey, request.AuthorName, request.Message, token);

                return Ok();
            }
            catch (ArgumentException)
            {
                return BadRequest();
            }
        }
    }
}
