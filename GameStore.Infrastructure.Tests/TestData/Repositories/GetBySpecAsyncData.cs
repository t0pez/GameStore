using System.Collections.Generic;
using System.Reflection;
using GameStore.Core.Models.Games.Specifications;
using Xunit.Sdk;

namespace GameStore.Infrastructure.Tests.TestData.Repositories;

public class GetBySpecAsyncData : DataAttribute
{
    public override IEnumerable<object[]> GetData(MethodInfo testMethod)
    {
        yield return new object[] { new GamesWithDetailsSpec(), 6 };
    }
}