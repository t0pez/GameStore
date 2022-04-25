using System;
using System.Collections.Generic;

namespace GameStore.Web.ViewModels.Genres;

public class GenreViewModel
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public ICollection<GenreListViewModel> SubGenres { get; set; }
}