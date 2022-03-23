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
                Id = Guid.Parse("6fd60058-7ffd-472a-b97c-08d1067d8613"),
                Name = "Some name",
                Key = "some-name",
                Description = "Some description",
                File = new byte[] { 0, 0, 0, 0, 1 }
            }
        };
        yield return new object[]
        {
            new Game
            {
                Id = Guid.Parse("6fd60058-7f44-472a-b97c-08d1067d8613"),
                Name = "Some new name",
                Key = "some-new-name",
                Description = "Some new description",
                File = new byte[] { 0, 0, 0, 0, 1 }
            }
        };
    }
}