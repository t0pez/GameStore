using System;
using Ardalis.Specification;

namespace GameStore.Core.Models.Comments.Specifications;

internal sealed class CommentByIdSpec : Specification<Comment>
{
    public CommentByIdSpec(Guid id)
    {
        Query
            .Where(comment => comment.Id == id
                              && comment.IsDeleted == false);
    }
}