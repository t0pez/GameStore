using System;

namespace GameStore.Web.Endpoints.Games
{
    public class AddToGenreRequest
    {
        public Guid GameId { get; set; }
        public Guid GenreId { get; set; }
    }
}
