using System;
using System.ComponentModel.DataAnnotations;

namespace GameStore.Web.Models.Comment;

public class ReplyCreateRequestModel
{
    public Guid GameId { get; set; }
    public Guid ParentId { get; set; }
    [Required] public string AuthorName { get; set; }
    [Required] public string Message { get; set; }
}