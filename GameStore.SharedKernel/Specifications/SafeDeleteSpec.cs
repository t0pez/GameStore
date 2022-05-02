using Ardalis.Specification;
using GameStore.SharedKernel.Interfaces;

namespace GameStore.SharedKernel.Specifications;

public class SafeDeleteSpec<TModel>
    : DomainSpec<TModel> where TModel : ISafeDelete
{
    protected SafeDeleteSpec()
    {
        Query
            .Where(model => model.IsDeleted == IsIncludedDeleted ||
                            model.IsDeleted == IsExcludedNotDeleted);
    }

    public bool IsIncludedDeleted { get; set; }

    public bool IsExcludedNotDeleted { get; set; }
}
