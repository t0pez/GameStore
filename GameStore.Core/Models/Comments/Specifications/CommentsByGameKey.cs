using Ardalis.Specification;

namespace GameStore.Core.Models.Comments.Specifications;

internal class CommentsByGameKey : Specification<Comment>
{
    public CommentsByGameKey(string gameKey)
    {
        Query
            .Where(c => c.Game.Key == gameKey)
            .Include(c => c.Replies);
    }
}