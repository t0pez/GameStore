﻿using Microsoft.AspNetCore.Mvc;

namespace GameStore.Web.Models
{
    public class CreateGameRequest
    {
        [FromBody] public string Key { get; set; }
        [FromBody] public string Name { get; set; }
        [FromBody] public string Description { get; set; }
        [FromBody] public byte[] File { get; set; }
    }
}
