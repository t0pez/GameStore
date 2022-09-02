using System;
using GameStore.Core.Models.Server.Comments;

namespace GameStore.Core.Models.ServiceModels.Comments;

public class ReplyCreateModel
{
    public Guid ParentId { get; set; }

    public string GameKey { get; set; }

    public string AuthorName { get; set; }

    public string Message { get; set; }

    public CommentState State { get; set; }
}