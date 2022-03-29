using System;
using System.Collections.Generic;
using System.Reflection;
using GameStore.Core.Models.Games;
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
                Key = "key-1" // Alternative key must not be null
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