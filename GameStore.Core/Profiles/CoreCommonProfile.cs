using AutoMapper;
using GameStore.Core.Models.Games;
using GameStore.Core.Models.Records;

namespace GameStore.Core.Profiles;

public class CoreCommonProfile : Profile
{
    public CoreCommonProfile()
    {
        CreateMap<GameCreateModel, Game>();
    }
}