using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace GameStore.Web.Models.Comment;

public class CommentUpdateRequestModel
{
    [FromRoute(Name = "gameKey")]
    public string GameKey { get; set; }

    public Guid Id { get; set; }

    [Required]
    public string AuthorName { get; set; }

    [Required]
    public string Body { get; set; }
}