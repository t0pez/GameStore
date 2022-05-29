using System.ComponentModel.DataAnnotations;

namespace GameStore.Core.Models.Games.Specifications.Filters;

public enum GameSearchFilterOrderByState
{
    [Display(Name = "Default")] Default = 0,
    [Display(Name = "Most popular")] MostPopular = 1,
    [Display(Name = "Most commented")] MostCommented = 2,
    [Display(Name = "Price from lowest")] PriceAscending = 3,
    [Display(Name = "Price from highest")] PriceDescending = 4,
    [Display(Name = "Newest")] New = 5
}