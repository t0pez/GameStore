using System;

namespace GameStore.Web.Models;

public class ReplyCreateRequestModel
{
    public Guid GameId { get; set; }
    public Guid ParentId { get; set; }
    public string AuthorName { get; set; }
    public string Message { get; set; }
}