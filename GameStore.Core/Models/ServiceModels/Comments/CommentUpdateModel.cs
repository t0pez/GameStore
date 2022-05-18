using System;

namespace GameStore.Core.Models.ServiceModels.Comments;

public class CommentUpdateModel
{
    public Guid Id { get; set; }
    public string AuthorName { get; set; }
    public string Body { get; set; }
}