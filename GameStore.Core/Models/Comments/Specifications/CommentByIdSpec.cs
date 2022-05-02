using System;
using Ardalis.Specification;
using GameStore.SharedKernel.Specifications;

namespace GameStore.Core.Models.Comments.Specifications;

public sealed class CommentByIdSpec : SafeDeleteSpec<Comment>
{
    public CommentByIdSpec(Guid id)
    {
        Id = id;

        Query
            .Where(comment => comment.Id == id);
    }

    public Guid Id { get; }
}