using System;
using System.Collections.Generic;
using System.Reflection;
using GameStore.Core.Models.Server.Games.Specifications;
using Xunit.Sdk;

namespace GameStore.Infrastructure.Tests.TestData.Repositories;

public class GetSingleBySpecWithResultData : DataAttribute
{
    public override IEnumerable<object[]> GetData(MethodInfo testMethod)
    {
        yield return new object[]
        {
            new GamesSpec().ById(Guid.Parse("6fd6d158-7ffd-472a-b971-08da067d7601")),
            Guid.Parse("6fd6d158-7ffd-472a-b971-08da067d7601")
        };

        yield return new object[]
        {
            new GamesSpec().ById(Guid.Parse("6fd6d158-7ffd-472a-b972-08da067d7601")),
            Guid.Parse("6fd6d158-7ffd-472a-b972-08da067d7601")
        };

        yield return new object[]
        {
            new GamesSpec().ById(Guid.Parse("6fd6d158-7ffd-472a-b974-08da067d7601")),
            Guid.Parse("6fd6d158-7ffd-472a-b974-08da067d7601")
        };

        yield return new object[]
        {
            new GamesSpec().WithDetails().ById(Guid.Parse("6fd6d158-7ffd-472a-b971-08da067d7601")),
            Guid.Parse("6fd6d158-7ffd-472a-b971-08da067d7601")
        };

        yield return new object[]
        {
            new GamesSpec().WithDetails().ById(Guid.Parse("6fd6d158-7ffd-472a-b972-08da067d7601")),
            Guid.Parse("6fd6d158-7ffd-472a-b972-08da067d7601")
        };

        yield return new object[]
        {
            new GamesSpec().WithDetails().ById(Guid.Parse("6fd6d158-7ffd-472a-b974-08da067d7601")),
            Guid.Parse("6fd6d158-7ffd-472a-b974-08da067d7601")
        };

        yield return new object[]
        {
            new GamesSpec().ByKey("first-game"),
            Guid.Parse("6fd6d158-7ffd-472a-b971-08da067d7601")
        };

        yield return new object[]
        {
            new GamesSpec().ByKey("second-game"),
            Guid.Parse("6fd6d158-7ffd-472a-b972-08da067d7601")
        };

        yield return new object[]
        {
            new GamesSpec().ByKey("fourth-game"),
            Guid.Parse("6fd6d158-7ffd-472a-b974-08da067d7601")
        };

        yield return new object[]
        {
            new GamesSpec().WithDetails().ByKey("first-game"),
            Guid.Parse("6fd6d158-7ffd-472a-b971-08da067d7601")
        };

        yield return new object[]
        {
            new GamesSpec().WithDetails().ByKey("second-game"),
            Guid.Parse("6fd6d158-7ffd-472a-b972-08da067d7601")
        };

        yield return new object[]
        {
            new GamesSpec().WithDetails().ByKey("fourth-game"),
            Guid.Parse("6fd6d158-7ffd-472a-b974-08da067d7601")
        };
    }
}