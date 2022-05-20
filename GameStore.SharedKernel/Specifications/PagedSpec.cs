using System;
using Ardalis.Specification;
using GameStore.SharedKernel.Specifications.Filters;

namespace GameStore.SharedKernel.Specifications;

public class PagedSpec<TModel> : Specification<TModel>
{
    public PagedSpec<TModel> EnablePaging(BaseSearchFilter searchFilter)
    {
        if (searchFilter.IsPagingEnabled == false)
        {
            throw new ArgumentException("Paging isn't enabled");
        }

        var skip = PaginationHelper.CalculateSkip(searchFilter);
        var take = PaginationHelper.CalculateTake(searchFilter);

        EnablePaging(skip, take);

        return this;
    }

    public PagedSpec<TModel> EnablePaging(int skip, int take)
    {
        Query
            .Skip(skip)
            .Take(take);

        return this;
    }
}