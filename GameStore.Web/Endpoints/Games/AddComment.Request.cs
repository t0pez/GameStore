using Microsoft.AspNetCore.Mvc;

namespace GameStore.Web.Endpoints.Games
{

    public class AddCommentRequest
    {
        [FromRoute(Name = "gameKey")] public string GameKey { get; set; }
        [FromBody] public string AuthorName { get; set; }
        [FromBody] public string Message { get; set; }
    }
}
