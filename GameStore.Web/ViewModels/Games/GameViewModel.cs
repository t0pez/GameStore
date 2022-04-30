using System;
using System.Collections.Generic;
using GameStore.Web.ViewModels.Genres;
using GameStore.Web.ViewModels.PlatformTypes;
using GameStore.Web.ViewModels.Publisher;

namespace GameStore.Web.ViewModels.Games;

public class GameViewModel
{
    public Guid Id { get; set; }
    
    public string Key { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public decimal Discount { get; set; }
    public int UnitsInStock { get; set; }

    public PublisherInGameViewModel Publisher { get; set; }
    public List<GenreViewModel> Genres { get; set; }
    public List<PlatformTypeViewModel> Platforms { get; set; }
}