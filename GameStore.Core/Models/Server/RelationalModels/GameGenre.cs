using System;
using GameStore.Core.Models.Server.Games;
using GameStore.Core.Models.Server.Genres;

namespace GameStore.Core.Models.Server.RelationalModels;

public class GameGenre
{
    public Guid GameId { get; set; }

    public Guid GenreId { get; set; }

    public Game Game { get; set; }

    public Genre Genre { get; set; }
}