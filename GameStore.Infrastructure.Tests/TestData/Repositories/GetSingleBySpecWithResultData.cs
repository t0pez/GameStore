using System;
using System.Collections.Generic;
using System.Reflection;
using GameStore.Core.Models.Games.Specifications;
using Xunit.Sdk;

namespace GameStore.Infrastructure.Tests.TestData.Repositories;

public class GetSingleBySpecWithResultData : DataAttribute
{
    public override IEnumerable<object[]> GetData(MethodInfo testMethod)
    {
        yield return new object[]
        {
            new GameByIdSpec(Guid.Parse("6fd6d158-7ffd-472a-b971-08da067d7601")),
            Guid.Parse("6fd6d158-7ffd-472a-b971-08da067d7601")
        };
        yield return new object[]
        {
            new GameByIdSpec(Guid.Parse("6fd6d158-7ffd-472a-b972-08da067d7601")),
            Guid.Parse("6fd6d158-7ffd-472a-b972-08da067d7601")
        };
        yield return new object[]
        {
            new GameByIdSpec(Guid.Parse("6fd6d158-7ffd-472a-b974-08da067d7601")),
            Guid.Parse("6fd6d158-7ffd-472a-b974-08da067d7601")
        };

        yield return new object[]
        {
            new GameByIdWithDetailsSpec(Guid.Parse("6fd6d158-7ffd-472a-b971-08da067d7601")),
            Guid.Parse("6fd6d158-7ffd-472a-b971-08da067d7601")
        };
        yield return new object[]
        {
            new GameByIdWithDetailsSpec(Guid.Parse("6fd6d158-7ffd-472a-b972-08da067d7601")),
            Guid.Parse("6fd6d158-7ffd-472a-b972-08da067d7601")
        };
        yield return new object[]
        {
            new GameByIdWithDetailsSpec(Guid.Parse("6fd6d158-7ffd-472a-b974-08da067d7601")),
            Guid.Parse("6fd6d158-7ffd-472a-b974-08da067d7601")
        };

        yield return new object[]
        {
            new GameByKeySpec("first-game"),
            Guid.Parse("6fd6d158-7ffd-472a-b971-08da067d7601")
        };
        yield return new object[]
        {
            new GameByKeySpec("second-game"),
            Guid.Parse("6fd6d158-7ffd-472a-b972-08da067d7601")
        };
        yield return new object[]
        {
            new GameByKeySpec("fourth-game"),
            Guid.Parse("6fd6d158-7ffd-472a-b974-08da067d7601")
        };

        yield return new object[]
        {
            new GameByKeyWithDetailsSpec("first-game"),
            Guid.Parse("6fd6d158-7ffd-472a-b971-08da067d7601")
        };
        yield return new object[]
        {
            new GameByKeyWithDetailsSpec("second-game"),
            Guid.Parse("6fd6d158-7ffd-472a-b972-08da067d7601")
        };
        yield return new object[]
        {
            new GameByKeyWithDetailsSpec("fourth-game"),
            Guid.Parse("6fd6d158-7ffd-472a-b974-08da067d7601")
        };
    }
}