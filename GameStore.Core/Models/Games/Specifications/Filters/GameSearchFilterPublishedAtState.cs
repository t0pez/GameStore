using System.ComponentModel.DataAnnotations;

namespace GameStore.Core.Models.Games.Specifications.Filters;

public enum GameSearchFilterPublishedAtState
{
    [Display(Name = "Default")] Default = 0,
    [Display(Name = "Last week")] OneWeek = 1,
    [Display(Name = "Last month")] OneMonth = 2,
    [Display(Name = "Last year")] OneYear = 3,
    [Display(Name = "Two years")] TwoYears = 4,
    [Display(Name = "Three years")] ThreeYears = 5
}