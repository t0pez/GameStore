using AutoMapper;

namespace GameStore.Core.Profiles;

public static class MapperConfigurationExtensions
{
    public static void AddCoreProfiles(this IMapperConfigurationExpression expression)
    {
        expression.AddProfiles(new Profile[]
        {
            new CommentCoreProfile(),
            new FilterCoreProfile(),
            new GameCoreProfile(),
            new GenreCoreProfile(),
            new OrderCoreProfile(),
            new PlatformTypeCoreProfile(),
            new PublisherCoreProfile(),
        });
    }
}