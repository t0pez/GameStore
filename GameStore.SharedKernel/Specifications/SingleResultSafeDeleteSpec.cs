using Ardalis.Specification;
using GameStore.SharedKernel.Interfaces;

namespace GameStore.SharedKernel.Specifications;

public class SingleResultSafeDeleteSpec<TModel>
    : SingleResultDomainSpec<TModel> where TModel : ISafeDelete
{
    protected SingleResultSafeDeleteSpec()
    {
        Query
            .Where(model => model.IsDeleted == IsIncludedDeleted ||
                            model.IsDeleted == IsExcludedNotDeleted);
    }

    public bool IsIncludedDeleted { get; set; }

    public bool IsExcludedNotDeleted { get; set; }
}