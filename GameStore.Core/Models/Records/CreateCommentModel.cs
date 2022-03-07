namespace GameStore.Core.Models.Records
{
    public class CreateCommentModel
    {
        public string GameKey { get; set; }
        public string AuthorName { get; set; }
        public string Message { get; set; }
    }
}
