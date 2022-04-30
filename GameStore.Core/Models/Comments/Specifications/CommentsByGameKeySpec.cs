using Ardalis.Specification;

namespace GameStore.Core.Models.Comments.Specifications;

public sealed class CommentsByGameKeySpec : Specification<Comment>
{
    public CommentsByGameKeySpec(string gameKey)
    {
        GameKey = gameKey;

        Query
            .Where(comment => comment.Game.Key == gameKey &&
                              comment.ParentId == null &&
                              comment.IsDeleted == false)
            .Include(comment => comment.Replies)
            .Include(comment => comment.Game);
    }

    public string GameKey { get; set; }
}