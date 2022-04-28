using System.ComponentModel.DataAnnotations;

namespace GameStore.Web.Models.PlatformType;

public class PlatformTypeCreateRequestModel
{
    [Required] public string Name { get; set; }
}