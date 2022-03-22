using System;
using GameStore.Core.Models.Games;

namespace GameStore.Core.Models.RelationalModels;

public class GameGenre
{
    public Guid GameId { get; set; }
    public Guid GenreId { get; set; }

    public Game Game { get; set; }
    public Genre Genre { get; set; }
}