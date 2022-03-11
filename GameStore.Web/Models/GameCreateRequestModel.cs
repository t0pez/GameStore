using Microsoft.AspNetCore.Mvc;

namespace GameStore.Web.Models;

public class GameCreateRequestModel
{
    [FromBody] public string Name { get; set; }
    [FromBody] public string Description { get; set; }
    [FromBody] public byte[] File { get; set; }
}