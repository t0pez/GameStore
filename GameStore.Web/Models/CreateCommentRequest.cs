using Microsoft.AspNetCore.Mvc;

namespace GameStore.Web.Models
{
    public class CreateCommentRequest
    {
        [FromRoute(Name = "gameKey")] public string GameKey { get; set; }
        [FromBody] public string AuthorName { get; set; }
        [FromBody] public string Message { get; set; }
    }
}
