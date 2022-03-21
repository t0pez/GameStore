using Ardalis.Specification;

namespace GameStore.Core.Models.Comments.Specifications;

internal class CommentsByGameKeySpec : Specification<Comment>
{
    public CommentsByGameKeySpec(string gameKey)
    {
        Query
            .Where(c => c.Game.Key == gameKey)
            .Include(c => c.Replies)
            .Include(c => c.Game);
    }
}