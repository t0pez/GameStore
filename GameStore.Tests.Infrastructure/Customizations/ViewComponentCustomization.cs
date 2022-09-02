using AutoFixture;
using Microsoft.AspNetCore.Mvc.ViewComponents;

namespace GameStore.Tests.Infrastructure.Customizations;

internal class ViewComponentCustomization : ICustomization
{
    public void Customize(IFixture fixture)
    {
        fixture.Inject(new ViewComponentContext());
    }
}