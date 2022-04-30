namespace GameStore.Core.Models.ServiceModels.Comments;

public class CommentCreateModel
{
    public string GameKey { get; set; }
    public string AuthorName { get; set; }
    public string Message { get; set; }
}