using Ardalis.ApiEndpoints;
using GameStore.Core.Interfaces;
using GameStore.Core.Models.Comments;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace GameStore.Web.Endpoints.Games
{
    public class GetComments : EndpointBaseAsync.WithRequest<string>.WithResult<ICollection<Comment>>
    {
        private readonly ICommentService _commentService;

        public GetComments(ICommentService commentService)
        {
            _commentService = commentService;
        }

        [HttpGet("games/{key}/comments")]
        public override async Task<ICollection<Comment>> HandleAsync(string gameKey, CancellationToken cancellationToken = default)
        {
            return await _commentService.GetCommentsByGameKeyAsync(gameKey);
        }
    }
}
