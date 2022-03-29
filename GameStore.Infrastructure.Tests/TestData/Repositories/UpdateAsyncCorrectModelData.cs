using System;
using System.Collections.Generic;
using System.Reflection;
using GameStore.Core.Models.Games;
using Xunit.Sdk;

namespace GameStore.Infrastructure.Tests.TestData.Repositories;

public class UpdateAsyncCorrectModelData : DataAttribute
{
    public override IEnumerable<object[]> GetData(MethodInfo testMethod)
    {
        yield return new object[]
        {
            new Game
            {
                Id = Guid.Parse("6fd6d158-7ffd-472a-b971-08da067d7601"),
                Name = "New First game",
                Key = "first-game",
                Description = "New First description",
                File = new byte[] { 1, 0, 0, 1 },
                IsDeleted = false
            }
        };
        yield return new object[]
        {
            new Game
            {
                Id = Guid.Parse("6fd6d158-7ffd-472a-b972-08da067d7601"),
                Name = "Second game with changes",
                Key = "second-game",
                Description = "Second description with a new text here",
                File = new byte[] { 1, 0, 0, 2 }, 
                IsDeleted = false
            }
        };
        yield return new object[]
        {
            new Game
            {
                Id = Guid.Parse("6fd6d158-7ffd-472a-b973-08da067d7601"),
                Name = "[Updated] Third game",
                Key = "Third-game",
                Description = "Updated Third description",
                File = new byte[] { 1, 0, 0, 3 }, 
                IsDeleted = false
            }
        };
        yield return new object[]
        {
            new Game
            {
                Id = Guid.Parse("6fd6d158-7ffd-472a-b974-08da067d7601"),
                Name = "New Fourth game",
                Key = "fourth-game",
                Description = "Updated Fourth description",
                File = new byte[] { 1, 0, 0, 4 }, 
                IsDeleted = false
            }
        };
    }
}