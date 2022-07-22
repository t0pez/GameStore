using System;
using System.Linq;
using AutoMapper;
using GameStore.Core.Models.Dto.Filters;
using GameStore.Core.Models.Games.Specifications.Filters;
using GameStore.Web.Models.Game;

namespace GameStore.Web.Profiles;

public class FilterWebProfile : Profile
{
    public FilterWebProfile()
    {
        CreateMap<GamesFilterRequestModel, AllProductsFilter>()
            .ForMember(searchFilter => searchFilter.GenresIds,
                       expression =>
                           expression.MapFrom(
                               request => request.SelectedGenres.Select(Guid.Parse)))
            .ForMember(searchFilter => searchFilter.PlatformsIds,
                       expression =>
                           expression.MapFrom(
                               request => request.SelectedPlatforms.Select(Guid.Parse)))
            .ForMember(searchFilter => searchFilter.PublishersNames,
                       expression =>
                           expression.MapFrom(
                               request => request.SelectedPublishers));
        
        CreateMap<GamesFilterRequestModel, GameSearchFilter>()
            .ForMember(searchFilter => searchFilter.GenresIds,
                       expression =>
                           expression.MapFrom(
                               request => request.SelectedGenres.Select(Guid.Parse)))
            .ForMember(searchFilter => searchFilter.PlatformsIds,
                       expression =>
                           expression.MapFrom(
                               request => request.SelectedPlatforms.Select(Guid.Parse)))
            .ForMember(searchFilter => searchFilter.PublishersNames,
                       expression =>
                           expression.MapFrom(
                               request => request.SelectedPublishers));
    }
}