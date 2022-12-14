namespace GameStore.SharedKernel.Specifications.Filters;

public class PaginationFilter
{
    public const int DefaultCurrentPage = 1;
    public const int DefaultPageSize = 10;

    public int CurrentPage { get; set; }

    public int PageSize { get; set; }

    public int Take => PageSize > 0 ? PageSize : int.MaxValue;

    public int Skip => PageSize > 0 && CurrentPage > 0 ? PageSize * (CurrentPage - 1) : 0;
}