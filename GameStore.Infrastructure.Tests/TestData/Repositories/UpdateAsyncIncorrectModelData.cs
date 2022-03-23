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
                Id = Guid.Parse("6fd6d100-7ffd-472a-b971-08da067d7601"),
                Name = "New First game",
                Key = "first-game",
                Description = "Must be error with update",
                File = new byte[] { 1, 1, 1, 1 },
                IsDeleted = false
            }
        };
        yield return new object[]
        {
            new Game
            {
                Id = Guid.Parse("6fd60058-7ffd-472a-b972-08da067d7601"),
                Name = "Second game with errors",
                Key = "second-game",
                Description = "Sorry, but this models doesnt exists",
                File = new byte[] { 1, 1, 1, 1 }, 
                IsDeleted = false
            }
        };
        yield return new object[]
        {
            new Game
            {
                Id = Guid.Parse("6f000158-71fd-472a-b973-08da060d7600"),
                Name = "[NotExisting] Third game",
                Key = "third-game-",
                Description = "Updated Third description",
                File = new byte[] { 1, 1, 1, 1 }, 
                IsDeleted = false
            }
        };
        yield return new object[]
        {
            new Game
            {
                Id = Guid.Parse("00d6d158-7ffd-472a-b974-08da067d7601"),
                Name = "Error game",
                Key = "fourth-game",
                Description = "Updated Fourth description, not really",
                File = new byte[] { 1, 1, 1, 1 }, 
                IsDeleted = true
            }
        };
    }
}