using System;

namespace GameStore.Web.ViewModels.Comments;

public class CommentInReplyViewModel
{
    public Guid Id { get; set; }
    public string AuthorName { get; set; }
    public string Body { get; set; }
    public bool MessageIsDeleted { get; set; }
}