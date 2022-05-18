using GameStore.Core.Models.Games;
using GameStore.SharedKernel.Interfaces;
using System;
using System.Collections.Generic;

namespace GameStore.Core.Models.Comments;

public class Comment : ISafeDelete
{
    public Comment()
    {
    }

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