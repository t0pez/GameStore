using AutoFixture.Xunit2;

namespace GameStore.Tests.Infrastructure.Attributes;

public class InlineAutoMoqDataAttribute : InlineAutoDataAttribute
{
    public InlineAutoMoqDataAttribute(params object[] inlineValues) : base(new AutoMoqDataAttribute(), inlineValues)
    {
    }
}