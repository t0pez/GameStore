using GameStore.SharedKernel.Interfaces;
using SpecificationExtensions.Core.Attributes;

namespace GameStore.SharedKernel.Specifications;

[SafeDeleteSpec(typeof(ISafeDelete), nameof(ISafeDelete.IsDeleted))]
public partial class SafeDeleteSpec<TModel> where TModel : ISafeDelete
{
}