using GameStore.Core.Models.Games;
using GameStore.SharedKernel;
using GameStore.SharedKernel.Interfaces;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace GameStore.Core.Models.Comments;

public class Comment : BaseEntity, ISafeDelete
{
    public Comment(string name, string body, Game game, Comment parent = null)
    {
        Name = name;
        Body = body;
        Game = game;
        Parent = parent;
        DateOfCreation = DateTime.Now;
    }

    public Comment()
    {
    }

    public string Name { get; set; }
    public string Body { get; set; }
    public DateTime DateOfCreation { get; set; }

    public Guid GameId { get; set; }
    [JsonIgnore] public Game Game { get; set; }
    public Guid? ParentId { get; set; }
    [JsonIgnore] public Comment Parent { get; set; }

    public ICollection<Comment> Replies { get; set; } = new List<Comment>();

    public bool IsDeleted { get; set; }
}