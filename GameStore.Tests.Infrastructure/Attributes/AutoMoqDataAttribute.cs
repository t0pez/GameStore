using System.Linq;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Community.AutoMapper;
using AutoFixture.Xunit2;
using GameStore.Core.Profiles;
using GameStore.Tests.Infrastructure.Customizations;
using GameStore.Web.Profiles;

namespace GameStore.Tests.Infrastructure.Attributes;

public sealed class AutoMoqDataAttribute : AutoDataAttribute
{
    public AutoMoqDataAttribute() : base(GetDefaultFixture)
    {
    }

    private static IFixture GetDefaultFixture()
    {
        var fixture = new Fixture().Customize(new CompositeCustomization(new AutoMoqCustomization(),
                                                                         new ControllerCustomization(),
                                                                         new ViewComponentCustomization(),
                                                                         new AutoMapperCustomization(configuration =>
                                                                         {
                                                                             configuration.AddCoreProfiles();
                                                                             configuration.AddWebProfiles();
                                                                         })));

        fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList().ForEach(b => fixture.Behaviors.Remove(b));

        fixture.Behaviors.Add(new OmitOnRecursionBehavior());

        return fixture;
    }
}