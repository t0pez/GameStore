using AutoMapper;
using GameStore.Core.Models.Server.Users;
using GameStore.Core.Models.ServiceModels.Users;
using GameStore.Web.Models.User;
using GameStore.Web.ViewModels.User;

namespace GameStore.Web.Profiles;

public class UsersWebProfile : Profile
{
    public UsersWebProfile()
    {
        CreateMap<UserCreateRequestModel, UserCreateModel>();
        CreateMap<UserUpdateRequestModel, UserUpdateModel>();

        CreateMap<User, UserUpdateRequestModel>()
           .ForMember(model => model.OldEmail,
                      expression => expression.MapFrom(user => user.Email))
           .ForMember(model => model.OldUserName,
                      expression => expression.MapFrom(user => user.UserName));

        CreateMap<User, UserViewModel>();
        CreateMap<User, UserListViewModel>();

        CreateMap<LoginRequestModel, LoginModel>();
    }
}