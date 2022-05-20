using System;
using GameStore.SharedKernel.Specifications.Filters;

namespace GameStore.SharedKernel.Specifications;

public static class PaginationHelper
{
    public static int CalculateTake(int pageSize)
    {
        if (pageSize <= 0)
        {
            throw new ArgumentException("Page size must be more, than zero");
        }
        
        return pageSize;
    }
    
    public static int CalculateSkip(int pageSize, int page)
    {
        if (page <= 0)
        {
            throw new ArgumentException("Page number must be greater or equal to 0");
        }

        return CalculateTake(pageSize) * (page - 1);
    }

    public static int CalculateTake(BaseSearchFilter searchFilter)
    {
        return CalculateTake(searchFilter.PageSize);
    }
    
    public static int CalculateSkip(BaseSearchFilter searchFilter)
    {
        return CalculateSkip(searchFilter.PageSize, searchFilter.CurrentPage);
    }
}