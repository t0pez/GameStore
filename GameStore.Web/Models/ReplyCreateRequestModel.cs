using System;
using System.ComponentModel.DataAnnotations;

namespace GameStore.Web.Models;

public class ReplyCreateRequestModel
{
    [Required]
    public Guid GameId { get; set; }
    [Required]
    public Guid ParentId { get; set; }
    [Required]
    public string AuthorName { get; set; }
    [Required]
    public string Message { get; set; }
}