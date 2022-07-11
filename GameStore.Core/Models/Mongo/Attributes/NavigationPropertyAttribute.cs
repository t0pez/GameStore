using System;

namespace GameStore.Core.Models.Mongo.Attributes;

public class NavigationPropertyAttribute : Attribute
{
    public NavigationPropertyAttribute(string navigationIdName)
    {
        NavigationIdName = navigationIdName;
    }

    public string NavigationIdName { get; }
}