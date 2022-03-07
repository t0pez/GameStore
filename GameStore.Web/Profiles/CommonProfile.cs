using AutoMapper;
using GameStore.Core.Models.Records;
using GameStore.Web.Models;

namespace GameStore.Web.Profiles
{
    public class CommonProfile : Profile
    {
        public CommonProfile()
        {
            CreateMap<CreateGameRequestModel, CreateGameModel>().ReverseMap();
            CreateMap<CreateCommentRequestModel, CreateCommentModel>().ReverseMap();
        }
    }
}
