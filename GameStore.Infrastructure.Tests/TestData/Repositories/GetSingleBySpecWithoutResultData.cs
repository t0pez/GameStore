using System;
using System.Collections.Generic;
using System.Reflection;
using GameStore.Core.Models.Games.Specifications;
using Xunit.Sdk;

namespace GameStore.Infrastructure.Tests.TestData.Repositories;

public class GetSingleBySpecWithoutResultData : DataAttribute
{
    public override IEnumerable<object[]> GetData(MethodInfo testMethod)
    {
        yield return new object[]
        {
            new GameByIdSpec(Guid.NewGuid())
        };
        yield return new object[]
        {
            new GameByIdSpec(Guid.NewGuid())
        };
        yield return new object[]
        {
            new GameByIdSpec(Guid.NewGuid())
        };

        yield return new object[]
        {
            new GameByIdWithDetailsSpec(Guid.NewGuid())
        };
        yield return new object[]
        {
            new GameByIdWithDetailsSpec(Guid.NewGuid())
        };
        yield return new object[]
        {
            new GameByIdWithDetailsSpec(Guid.NewGuid())
        };

        yield return new object[]
        {
            new GameByKeySpec("wrong-game-1")
        };
        yield return new object[]
        {
            new GameByKeySpec("second-00-game")
        };
        yield return new object[]
        {
            new GameByKeySpec("fourth-game00")
        };

        yield return new object[]
        {
            new GameByKeyWithDetailsSpec("fIrst-game")
        };
        yield return new object[]
        {
            new GameByKeyWithDetailsSpec("second-gAme")
        };
        yield return new object[]
        {
            new GameByKeyWithDetailsSpec("fourth-gamE")
        };
    }
}