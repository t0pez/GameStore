namespace GameStore.Web.Models;

public class GameCreateRequestModel
{
    public string Name { get; set; }
    public string Description { get; set; }
    public byte[] File { get; set; }
}