using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace GameStore.Web.Extensions;

public static class DisplayExtensions
{
    public static string GetDisplayName(this Enum value)
    {
        var field = value.GetType().GetField(value.ToString());

        var displayAttribute = field.GetCustomAttribute<DisplayAttribute>();

        return displayAttribute is not null ? displayAttribute.Name : string.Empty;
    }
}