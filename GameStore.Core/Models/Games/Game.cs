using System;
using GameStore.SharedKernel.Interfaces;
using System.Collections.Generic;
using GameStore.Core.Models.Comments;
using GameStore.Core.Models.Publishers;
using GameStore.Core.Models.RelationalModels;

namespace GameStore.Core.Models.Games;

public class Game : ISafeDelete
{
    public Guid Id { get; set; }
    public string Key { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public bool Discontinued { get; set; }
    public int UnitsInStock { get; set; }
    public byte[] File { get; set; }
    public int Views { get; set; }

    public DateTime PublishedAt { get; set; }
    public DateTime AddedToStoreAt { get; set; }

    public Guid PublisherId { get; set; }
    public Publisher Publisher { get; set; }
    
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    public ICollection<GameGenre> Genres { get; set; } = new List<GameGenre>();
    public ICollection<GamePlatformType> Platforms { get; set; } = new List<GamePlatformType>();

    public bool IsDeleted { get; set; }
}