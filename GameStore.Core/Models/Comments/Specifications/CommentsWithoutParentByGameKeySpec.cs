using Ardalis.Specification;
using GameStore.SharedKernel.Specifications;

namespace GameStore.Core.Models.Comments.Specifications;

public sealed class CommentsWithoutParentByGameKeySpec : MultipleResultSafeDeleteSpec<Comment>
{
    public CommentsWithoutParentByGameKeySpec(string gameKey)
    {
        GameKey = gameKey;

        Query
            .Where(comment => comment.Game.Key == gameKey &&
                              comment.ParentId == null)
            .Include(comment => comment.Replies)
            .Include(comment => comment.Game);
    }

    public string GameKey { get; }
}