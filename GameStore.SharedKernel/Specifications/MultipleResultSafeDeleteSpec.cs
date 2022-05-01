using Ardalis.Specification;
using GameStore.SharedKernel.Interfaces;

namespace GameStore.SharedKernel.Specifications;

public class MultipleResultSafeDeleteSpec<TModel>
    : MultipleResultDomainSpec<TModel> where TModel : ISafeDelete
{
    public MultipleResultSafeDeleteSpec()
    {
        Query
            .Where(model => model.IsDeleted == IsEnabledDeleted ||
                            model.IsDeleted == !IsEnabledNotDeleted);
    }

    public bool IsEnabledDeleted { get; set; }
    public bool IsEnabledNotDeleted { get; set; } = true;
}
