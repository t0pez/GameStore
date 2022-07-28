using GameStore.SharedKernel.Specifications.Filters;
using SpecificationExtensions.Core.Specifications;

namespace GameStore.SharedKernel.Specifications.Extensions;

public static class BaseSpecExtensions
{
    public static BaseSpec<T> EnablePaging<T>(this BaseSpec<T> spec, PaginationFilter filter)
    {
        return spec.EnablePaging(filter.Skip, filter.Take);
    }
}