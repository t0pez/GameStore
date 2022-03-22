using System;
using System.Collections.Generic;

namespace GameStore.Web.ViewModels;

public class CommentViewModel
{
    public string GameKey { get; set; }
    public string AuthorName { get; set; }
    public string Body { get; set; }
    public DateTime DateOfCreation { get; set; }
    public ICollection<CommentViewModel> Replies { get; set; } = new List<CommentViewModel>();
}