using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace GameStore.Web.Models;

public class CommentCreateRequestModel
{
    [Required] [FromRoute(Name = "gameKey")] 
    public string GameKey { get; set; }
    [Required] [FromForm]
    public string AuthorName { get; set; }
    [Required] [FromForm]
    public string Message { get; set; }
}