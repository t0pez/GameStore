using AutoMapper;

namespace GameStore.Web.Profiles;

public static class MapperConfigurationExtensions
{
    public static void AddWebProfiles(this IMapperConfigurationExpression expression)
    {
        expression.AddProfiles(new Profile[]
        {
            new CommentWebProfile(),
            new FilterWebProfile(),
            new GameWebProfile(),
            new GenreWebProfile(),
            new OrderWebProfile(),
            new PlatformTypeWebProfile(),
            new PublisherWebProfile()
        });
    }
}