namespace GameStore.SharedKernel.Specifications.Filters;

public class BaseSearchFilter
{
    public bool IsPagingEnabled { get; set; }
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
}