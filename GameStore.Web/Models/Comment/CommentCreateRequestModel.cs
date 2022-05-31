using System;
using System.ComponentModel.DataAnnotations;
using GameStore.Core.Models.Comments;
using Microsoft.AspNetCore.Mvc;

namespace GameStore.Web.Models.Comment;

public class CommentCreateRequestModel
{
    [FromRoute(Name = "gameKey")] public string GameKey { get; set; }
    [Required] public string AuthorName { get; set; }
    [Required] public string Message { get; set; }
    public Guid? ParentId { get; set; }
    public CommentState State { get; set; }
}