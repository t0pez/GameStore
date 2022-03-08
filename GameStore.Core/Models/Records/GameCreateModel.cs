namespace GameStore.Core.Models.Records
{
    public class GameCreateModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public byte[] File { get; set; }
    }
}
