﻿using System.ComponentModel.DataAnnotations;

namespace GameStore.Web.Models.Publisher;

public class PublisherCreateRequestModel
{
    [Required] public string Name { get; set; }
    [Required] public string Description { get; set; }
    [Required] public string HomePage { get; set; }
}