using System.Collections.Generic;
using System.Reflection;
using Xunit.Sdk;
using GameStore.Core.Models.Games.Specifications;

namespace GameStore.Infrastructure.Tests.TestData.Repositories;

public class GetBySpecAsyncData : DataAttribute
{
    public override IEnumerable<object[]> GetData(MethodInfo testMethod)
    {
        yield return new object[] { new GamesWithDetailsSpec(), 5 };
        
    }
}