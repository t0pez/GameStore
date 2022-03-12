using GameStore.Core.Models.Comments;
using GameStore.SharedKernel;
using GameStore.SharedKernel.Interfaces;
using System.Collections.Generic;

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

    public string Key { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public byte[] File { get; set; }

    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    public ICollection<Genre> Genres { get; set; } = new List<Genre>();
    public ICollection<PlatformType> PlatformTypes { get; set; } = new List<PlatformType>();

    public bool IsDeleted { get; set; }
}