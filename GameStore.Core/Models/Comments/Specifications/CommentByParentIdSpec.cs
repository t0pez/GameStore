using System;
using Ardalis.Specification;

namespace GameStore.Core.Models.Comments.Specifications;

internal sealed class CommentByParentIdSpec : Specification<Comment>
{
    public CommentByParentIdSpec(Guid parentId)
    {
        Query
            .Where(comment => comment.ParentId == parentId
                              && comment.IsDeleted == false);
    }
}