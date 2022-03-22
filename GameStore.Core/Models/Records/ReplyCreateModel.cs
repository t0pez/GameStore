using System;

namespace GameStore.Core.Models.Records;

public class ReplyCreateModel
{
    public Guid ParentId { get; set; }
    public Guid GameId { get; set; }
    public string AuthorName { get; set; }
    public string Message { get; set; }
}