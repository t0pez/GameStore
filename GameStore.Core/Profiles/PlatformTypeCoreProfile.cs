using AutoMapper;
using GameStore.Core.Models.Server.PlatformTypes;
using GameStore.Core.Models.ServiceModels.PlatformTypes;

namespace GameStore.Core.Profiles;

public class PlatformTypeCoreProfile : Profile
{
    public PlatformTypeCoreProfile()
    {
        CreateMap<PlatformTypeCreateModel, PlatformType>();
    }
}