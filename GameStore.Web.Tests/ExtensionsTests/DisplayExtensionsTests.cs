using System;
using System.Linq;
using FluentAssertions;
using GameStore.Core.Models.Games.Specifications.Filters;
using GameStore.Web.Extensions;
using Xunit;

namespace GameStore.Web.Tests.ExtensionsTests;

public class DisplayExtensionsTests
{
    [Fact]
    public void GetDisplayName_GameSearchFilterOrderByStateAsAParameter_ReturnsCorrectValues()
    {
        var expectedDisplayNames = new[]
            { "Default", "Most popular", "Most commented", "Price from lowest", "Price from highest", "Newest" };

        var enumProps = Enum.GetValues(typeof(GameSearchFilterOrderByState)).OfType<Enum>();

        var displayNames = enumProps.Select(enumElement => enumElement.GetDisplayName());

        displayNames.Should().Contain(expectedDisplayNames);
    }
}