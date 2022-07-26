using System;

namespace GameStore.Core.Models.Mongo.Attributes;

public class ManyNavigationPropertyAttribute : Attribute
{
    public ManyNavigationPropertyAttribute(string navigationIdName)
    {
        NavigationIdName = navigationIdName;
    }

    public string NavigationIdName { get; }
}