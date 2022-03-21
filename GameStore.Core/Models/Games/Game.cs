using System;
using GameStore.SharedKernel;
using GameStore.SharedKernel.Interfaces;
using System.Collections.Generic;
using GameStore.Core.Models.Comments;
using GameStore.Core.Models.RelationalModels;

namespace GameStore.Core.Models.Games;

public class Game : BaseEntity, ISafeDelete
{
    public Game(string key, string name, string description, byte[] file)
    {
        Key = key;
        Name = name;
        Description = description;
        File = file;
        IsDeleted = false;
    }

    public Game()
    {
        
    }

    public Guid Id { get; set; }
    public string Key { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public byte[] File { get; set; }

    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    public ICollection<GameGenre> Genres { get; set; } = new List<GameGenre>();
    public ICollection<GamePlatformType> Platforms { get; set; } = new List<GamePlatformType>();

    public bool IsDeleted { get; set; }
}