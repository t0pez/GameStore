using System;
using System.Collections.Generic;
using GameStore.Core.Models.Comments;

namespace GameStore.Web.ViewModels.Comments;

public class CommentViewModel
{
    public Guid Id { get; set; }
    public string GameKey { get; set; }
    public string AuthorName { get; set; }
    public string Body { get; set; }
    public bool MessageIsDeleted { get; set; }
    public DateTime DateOfCreation { get; set; }
    public CommentState? State { get; set; }
    public CommentInReplyViewModel Parent { get; set; }
    public ICollection<CommentViewModel> Replies { get; set; } = new List<CommentViewModel>();
}