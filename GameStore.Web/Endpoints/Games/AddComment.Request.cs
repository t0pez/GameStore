namespace GameStore.Web.Endpoints.Games
{
    public class AddCommentRequest
    {
        public string GameKey { get; set; }
        public string AuthorName { get; set; }
        public string Message { get; set; }
    }
}
