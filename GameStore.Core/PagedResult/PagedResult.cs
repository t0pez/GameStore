using System;
using System.Collections;
using System.Collections.Generic;
using GameStore.SharedKernel.Specifications.Filters;

namespace GameStore.Core.PagedResult;

public class PagedResult<T>
{
    public PagedResult(IEnumerable<T> result, int totalGamesCount, PaginationFilter paginationFilter)
    {
        Result = result;
        TotalPagesCount = Convert.ToInt32(Math.Ceiling((double)totalGamesCount / (double)paginationFilter.PageSize));
        CurrentPage = paginationFilter.CurrentPage;
        PageSize = paginationFilter.PageSize;
    }
    
    public IEnumerable<T> Result { get; set; }
    public int TotalPagesCount { get; set; }
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }

    public bool HasNextPage => TotalPagesCount > CurrentPage;
    public bool HasPreviousPage => CurrentPage > 1;
}