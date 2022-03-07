namespace GameStore.Core.Models.Records
{
    public class CreateGameModel
    {
        public string Key { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public byte[] File { get; set; }
    }
}
