using System;
using System.Collections.Generic;
using GameStore.Core.Models.ServiceModels.Enums;
using GameStore.Web.ViewModels.Genres;
using GameStore.Web.ViewModels.PlatformTypes;
using GameStore.Web.ViewModels.Publisher;

namespace GameStore.Web.ViewModels.Games;

public class GameViewModel
{
    public string Id { get; set; }

    public string Key { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public decimal Price { get; set; }

    public decimal Discount { get; set; }

    public int UnitsInStock { get; set; }

    public DateTime PublishedAt { get; set; }

    public DateTime AddedToStoreAt { get; set; }

    public int Views { get; set; }

    public PublisherInGameViewModel Publisher { get; set; }

    public List<GenreListViewModel> Genres { get; set; } = new();

    public List<PlatformTypeListViewModel> Platforms { get; set; } = new();

    public Database Database { get; set; }
}