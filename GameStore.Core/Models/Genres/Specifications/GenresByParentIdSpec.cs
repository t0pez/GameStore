using System;
using Ardalis.Specification;
using GameStore.SharedKernel.Specifications;

namespace GameStore.Core.Models.Genres.Specifications;

public class GenresByParentIdSpec : SafeDeleteSpec<Genre>
{
    public GenresByParentIdSpec(Guid parentId)
    {
        Query
            .Where(genre => genre.ParentId == parentId);

        ParentId = parentId;
    }

    public Guid ParentId { get; set; }
}