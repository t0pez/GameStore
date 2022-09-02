using FluentAssertions;
using GameStore.Tests.Infrastructure.Attributes;
using GameStore.Web.Controllers;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace GameStore.Web.Tests.Controllers.GameControllerTests;

public partial class GameControllerTests
{
    [Theory]
    [InlineAutoMoqData("Some name", "{ key = some-name }")]
    [InlineAutoMoqData("First game Part 2", "{ key = first-game-part-2 }")]
    public async void GenerateKeyAsync_GameKeyAsParameter_GeneratesCorrectKeys(
        string name,
        string expectedJsonValue,
        GamesController sut)
    {
        var actualResult = await sut.GenerateKeyAsync(name);

        actualResult.Should().BeOfType<JsonResult>().Which.Value.ToString().Should().Be(expectedJsonValue);
    }
}