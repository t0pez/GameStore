using GameStore.SharedKernel.Interfaces;
using GameStore.SharedKernel.Specifications;

namespace GameStore.Core.Helpers.Specifications;

public static class DomainSpecExtensions
{
    public static SingleResultSafeDeleteSpec<T> IncludeDeleted<T>(this SingleResultSafeDeleteSpec<T> spec,
                                                                  bool isEnabledDeleted = true) where T : ISafeDelete
    {
        spec.IsIncludedDeleted = isEnabledDeleted;
        return spec;
    }

    public static MultipleResultSafeDeleteSpec<T> IncludeDeleted<T>(this MultipleResultSafeDeleteSpec<T> spec,
                                                                    bool isEnabledDeleted = true) where T : ISafeDelete
    {
        spec.IsIncludedDeleted = isEnabledDeleted;
        return spec;
    }

    public static SingleResultSafeDeleteSpec<T> ExcludeNotDeleted<T>(this SingleResultSafeDeleteSpec<T> spec,
                                                                     bool isExcludeNotDeleted = true)
        where T : ISafeDelete
    {
        spec.IsExcludedNotDeleted = isExcludeNotDeleted;
        return spec;
    }

    public static MultipleResultSafeDeleteSpec<T> ExcludeNotDeleted<T>(this MultipleResultSafeDeleteSpec<T> spec,
                                                                       bool isExcludeNotDeleted = true)
        where T : ISafeDelete
    {
        spec.IsExcludedNotDeleted = isExcludeNotDeleted;
        return spec;
    }
}