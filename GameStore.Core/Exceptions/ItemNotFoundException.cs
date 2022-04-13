using System;

namespace GameStore.Core.Exceptions;

[Serializable]
public class ItemNotFoundException : Exception
{
    public ItemNotFoundException()
    {
    }

    public ItemNotFoundException(string entity, string predicateProperty,
                                 string predicateValue) // TODO: create overload with object to call ToString in ctor
    {
        Entity = entity;
        PredicateProperty = predicateProperty;
        PredicateValue = predicateValue;
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