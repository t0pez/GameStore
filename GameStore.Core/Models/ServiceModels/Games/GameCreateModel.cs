namespace GameStore.Core.Models.ServiceModels.Games;

public class GameCreateModel
{
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public byte[] File { get; set; }
}