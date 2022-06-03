using Ardalis.Specification;
using GameStore.SharedKernel.Specifications.Filters;

namespace GameStore.SharedKernel.Specifications;

public class PagedSpec<TModel> : Specification<TModel>
{
    public PagedSpec<TModel> EnablePaging(PaginationFilter searchFilter)
    {
        return EnablePaging(searchFilter.Skip, searchFilter.Take);
    }

    public PagedSpec<TModel> EnablePaging(int skip, int take)
    {
        Query
            .Skip(skip)
            .Take(take);

        return this;
    }
}