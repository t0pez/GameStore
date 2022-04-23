using System;
using System.Runtime.CompilerServices;

namespace GameStore.Core.Exceptions;

[Serializable]
public class ItemNotFoundException : Exception
{
    public ItemNotFoundException()
    {
    }

    public ItemNotFoundException(Type entity, object predicateProperty,
                                 [CallerArgumentExpression("predicateProperty")]
                                 string propertyName = "")
    {
        Entity = entity.Name;
        PredicateProperty = propertyName;
        PredicateValue = predicateProperty.ToString();
        Message = CraftExceptionMessage();
    }

    public override string Message { get; }
    public string Entity { get; }
    public string PredicateProperty { get; }
    public string PredicateValue { get; }

    private string CraftExceptionMessage()
    {
        return $"Item of type {Entity} can not be found. {PredicateProperty} of value {PredicateValue} doesn't exists";
    }
}