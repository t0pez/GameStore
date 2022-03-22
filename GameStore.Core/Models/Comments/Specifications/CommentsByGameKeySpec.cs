using Ardalis.Specification;

namespace GameStore.Core.Models.Comments.Specifications;

internal sealed class CommentsByGameKeySpec : Specification<Comment>
{
    public CommentsByGameKeySpec(string gameKey)
    {
        Query
            .Where(comment => comment.Game.Key == gameKey
                              && comment.IsDeleted == false)
            .Include(comment => comment.Replies)
            .Include(comment => comment.Game);
    }
}