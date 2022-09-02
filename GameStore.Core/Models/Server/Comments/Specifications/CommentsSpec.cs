using System;
using Ardalis.Specification;
using GameStore.SharedKernel.Specifications;

namespace GameStore.Core.Models.Server.Comments.Specifications;

public class CommentsSpec : SafeDeleteSpec<Comment>
{
    public CommentsSpec()
    {
        LoadAll();
    }

    public CommentsSpec ById(Guid id)
    {
        Query
           .Where(comment => comment.Id == id);

        return this;
    }

    public CommentsSpec ByGameKey(string gameKey)
    {
        Query
           .Where(comment => comment.Game.Key == gameKey);

        return this;
    }

    public CommentsSpec WithoutParent()
    {
        Query
           .Where(comment => comment.ParentId == null);

        return this;
    }

    public CommentsSpec WithDetails()
    {
        Query
           .Include(comment => comment.Replies)
           .ThenInclude(comment => comment.Replies)
           .ThenInclude(comment => comment.Replies);

        return this;
    }
}