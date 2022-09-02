using System;
using System.Collections.Generic;
using System.Reflection;
using GameStore.Core.Models.Server.Games.Specifications;
using Xunit.Sdk;

namespace GameStore.Infrastructure.Tests.TestData.Repositories;

public class GetSingleBySpecWithoutResultData : DataAttribute
{
    public override IEnumerable<object[]> GetData(MethodInfo testMethod)
    {
        yield return new object[]
        {
            new GamesSpec().ById(Guid.NewGuid())
        };

        yield return new object[]
        {
            new GamesSpec().ById(Guid.NewGuid())
        };

        yield return new object[]
        {
            new GamesSpec().ById(Guid.NewGuid())
        };

        yield return new object[]
        {
            new GamesSpec().ById(Guid.NewGuid()).WithDetails()
        };

        yield return new object[]
        {
            new GamesSpec().ById(Guid.NewGuid()).WithDetails()
        };

        yield return new object[]
        {
            new GamesSpec().ById(Guid.NewGuid()).WithDetails()
        };

        yield return new object[]
        {
            new GamesSpec().ByKey("wrong-game-1")
        };

        yield return new object[]
        {
            new GamesSpec().ByKey("second-00-game")
        };

        yield return new object[]
        {
            new GamesSpec().ByKey("fourth-game00")
        };

        yield return new object[]
        {
            new GamesSpec().ByKey("fIrst-game").WithDetails()
        };

        yield return new object[]
        {
            new GamesSpec().ByKey("second-gAme").WithDetails()
        };

        yield return new object[]
        {
            new GamesSpec().ByKey("fourth-gamE").WithDetails()
        };
    }
}