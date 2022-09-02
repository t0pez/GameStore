using System;
using System.Collections.Generic;
using System.Reflection;
using GameStore.Core.Models.Server.Games;
using Xunit.Sdk;

namespace GameStore.Infrastructure.Tests.TestData.Repositories;

public class DeleteAsyncCorrectModelData : DataAttribute
{
    public override IEnumerable<object[]> GetData(MethodInfo testMethod)
    {
        yield return new object[]
        {
            new Game
            {
                Id = Guid.Parse("6fd6d158-7ffd-472a-b971-08da067d7601"),
                Name = "First game",
                Key = "first-game",
                Description = "First description",
                File = new byte[] { 0, 0, 0, 1 },
                IsDeleted = false
            }
        };

        yield return new object[]
        {
            new Game
            {
                Id = Guid.Parse("6fd6d158-7ffd-472a-b972-08da067d7601"),
                Name = "Second game",
                Key = "second-game",
                Description = "Second description",
                File = new byte[] { 0, 0, 0, 2 },
                IsDeleted = false
            }
        };

        yield return new object[]
        {
            new Game
            {
                Id = Guid.Parse("6fd6d158-7ffd-472a-b973-08da067d7601"),
                Name = "Third game",
                Key = "Third-game",
                Description = "Third description",
                File = new byte[] { 0, 0, 0, 3 },
                IsDeleted = false
            }
        };
    }
}