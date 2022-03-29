using System;
using Ardalis.Specification;

namespace GameStore.Core.Models.Comments.Specifications;

public sealed class CommentByIdSpec : Specification<Comment>
{
    public CommentByIdSpec(Guid id)
    {
        Id = id;
        
        Query
            .Where(comment => comment.Id == id
                              && comment.IsDeleted == false);
    }

    public Guid Id { get; }
}