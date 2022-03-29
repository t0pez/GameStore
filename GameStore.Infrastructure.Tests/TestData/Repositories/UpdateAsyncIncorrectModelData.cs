using System;
using System.Collections.Generic;
using System.Reflection;
using GameStore.Core.Models.Games;
using Xunit.Sdk;

namespace GameStore.Infrastructure.Tests.TestData.Repositories;

public class UpdateAsyncIncorrectModelData : DataAttribute
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
        yield return new object[]
        {
            new Game
            {
                Id = Guid.NewGuid(),
                Key = "key-3"
            }
        };
        yield return new object[]
        {
            new Game
            {
                Id = Guid.NewGuid(),
                Key = "key-4"
            }
        };
    }
}