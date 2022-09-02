using AutoMapper;
using GameStore.Core.Models.Server.Users;
using GameStore.Core.Models.ServiceModels.Users;

namespace GameStore.Core.Profiles;

public class UserCoreProfile : Profile
{
    public UserCoreProfile()
    {
        CreateMap<UserCreateModel, User>()
            .ForMember(user => user.Id,
                       expression => expression.MapFrom(model => model.CookieUserId));
    }
}