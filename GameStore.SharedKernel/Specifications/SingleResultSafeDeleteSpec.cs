using Ardalis.Specification;
using GameStore.SharedKernel.Interfaces;

namespace GameStore.SharedKernel.Specifications;

public class SingleResultSafeDeleteSpec<TModel>
    : SingleResultDomainSpec<TModel> where TModel : ISafeDelete
{
    public SingleResultSafeDeleteSpec()
    {
        Query
            .Where(model => model.IsDeleted == IsEnabledDeleted ||
                            model.IsDeleted == !IsEnabledNotDeleted);
    }

    public bool IsEnabledDeleted { get; set; }
    public bool IsEnabledNotDeleted { get; set; } = true;
}