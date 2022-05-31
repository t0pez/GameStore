using GameStore.SharedKernel.Interfaces;

namespace GameStore.SharedKernel.Specifications.Extensions;

public static class SafeDeleteSpecExtensions
{
    public static SafeDeleteSpec<T> IncludeDeleted<T>(this SafeDeleteSpec<T> spec,
                                                      bool isEnabledDeleted = true) 
        where T : ISafeDelete
    {
        spec.IsIncludedDeleted = isEnabledDeleted;
        return spec;
    }

    public static SafeDeleteSpec<T> ExcludeNotDeleted<T>(this SafeDeleteSpec<T> spec,
                                                         bool isExcludeNotDeleted = true)
        where T : ISafeDelete
    {
        spec.IsExcludedNotDeleted = isExcludeNotDeleted;
        return spec;
    }
}