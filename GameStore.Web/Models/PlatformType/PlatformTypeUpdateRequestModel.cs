using System;
using System.ComponentModel.DataAnnotations;

namespace GameStore.Web.Models.PlatformType;

public class PlatformTypeUpdateRequestModel
{
    public Guid Id { get; set; }
    [Required] public string Name { get; set; }
}