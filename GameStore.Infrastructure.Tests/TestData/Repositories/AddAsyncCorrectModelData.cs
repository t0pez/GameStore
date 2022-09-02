using System;
using System.Collections.Generic;
using System.Reflection;
using GameStore.Core.Models.Server.Games;
using Xunit.Sdk;

namespace GameStore.Infrastructure.Tests.TestData.Repositories;

public class AddAsyncCorrectModelData : DataAttribute
{
    public override IEnumerable<object[]> GetData(MethodInfo testMethod)
    {
        yield return new object[]
        {
            new Game
            {
                Id = Guid.NewGuid(),
                Key = "key-1"
            }
        };

        yield return new object[]
        {
            new Game
            {
                Id = Guid.NewGuid(),
                Key = "key-2"
            }
        };
    }
}