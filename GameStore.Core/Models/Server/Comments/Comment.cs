using System;
using System.Collections.Generic;
using GameStore.Core.Models.Server.Games;
using GameStore.SharedKernel.Interfaces;

namespace GameStore.Core.Models.Server.Comments;

public class Comment : ISafeDelete
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public string Body { get; set; }

    public CommentState State { get; set; }

    public DateTime DateOfCreation { get; set; }

    public Guid GameId { get; set; }

    public Game Game { get; set; }

    public Guid? ParentId { get; set; }

    public Comment Parent { get; set; }

    public ICollection<Comment> Replies { get; set; } = new List<Comment>();

    public bool IsDeleted { get; set; }
}