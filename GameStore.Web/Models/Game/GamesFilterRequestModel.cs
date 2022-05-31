using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using GameStore.Core.Models.Games.Specifications.Filters;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GameStore.Web.Models.Game;

public class GamesFilterRequestModel
{
    [MinLength(3)]
    public string Name { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public SelectList Genres { get; set; }
    public List<string> SelectedGenres { get; set; } = new();
    public SelectList Platforms { get; set; }
    public List<string> SelectedPlatforms { get; set; } = new();
    public SelectList Publishers { get; set; }
    public List<string> SelectedPublishers { get; set; } = new();
    
    public GameSearchFilterOrderByState OrderBy { get; set; } = GameSearchFilterOrderByState.Default;

    public int CurrentPage { get; set; }
    public int PageSize { get; set; }

    public bool IsEmpty => string.IsNullOrWhiteSpace(Name) &&
                           MinPrice is null &&
                           MaxPrice is null &&
                           SelectedGenres.Any() == false &&
                           SelectedPlatforms.Any() == false &&
                           SelectedPublishers.Any() == false &&
                           OrderBy == GameSearchFilterOrderByState.Default;
}