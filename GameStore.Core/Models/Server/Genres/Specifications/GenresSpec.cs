using System;
using System.Collections.Generic;
using System.Linq;
using Ardalis.Specification;
using GameStore.SharedKernel.Specifications;

namespace GameStore.Core.Models.Server.Genres.Specifications;

public class GenresSpec : SafeDeleteSpec<Genre>
{
    public GenresSpec ById(Guid id)
    {
        Query
           .Where(genre => genre.Id == id);

        return this;
    }

    public GenresSpec ByCategoryId(int categoryId)
    {
        Query
           .Where(genre => genre.CategoryId == categoryId);

        return this;
    }

    public GenresSpec ByParentId(Guid parentId)
    {
        Query
           .Where(genre => genre.ParentId == parentId);

        return this;
    }

    public GenresSpec ByIds(IEnumerable<Guid> genresIds)
    {
        Query
           .Where(genre => genresIds.Contains(genre.Id));

        return this;
    }

    public GenresSpec WithCategoryId()
    {
        Query
           .Where(genre => genre.CategoryId != null);

        return this;
    }

    public GenresSpec WithDetails()
    {
        Query
           .Include(genre => genre.Parent)
           .Include(genre => genre.SubGenres);

        return this;
    }
}