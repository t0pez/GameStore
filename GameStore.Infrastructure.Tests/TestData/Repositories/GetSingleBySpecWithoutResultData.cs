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
            new GameByIdSpec(Guid.Parse("6fd6d158-00fd-472a-b971-08da067d7601"))
        };
        yield return new object[]
        {
            new GameByIdSpec(Guid.Parse("6fd6d158-7f00-472a-b972-08da067d7601"))
        };
        yield return new object[]
        {
            new GameByIdSpec(Guid.Parse("6fd6d158-7ffd-472a-b974-00da067d7601"))
        };
        
        yield return new object[]
        {
            new GameByIdWithDetailsSpec(Guid.Parse("00d6d158-7ffd-472a-b971-08da067d7601"))
        };
        yield return new object[]
        {
            new GameByIdWithDetailsSpec(Guid.Parse("6fd6d100-7ffd-472a-b972-08da067d7601"))
        };
        yield return new object[]
        {
            new GameByIdWithDetailsSpec(Guid.Parse("6fd60058-7ffd-472a-b974-08da067d7601"))
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