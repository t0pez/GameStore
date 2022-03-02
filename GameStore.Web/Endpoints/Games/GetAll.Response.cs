using GameStore.Core.Models.Games;
using System.Collections.Generic;

namespace GameStore.Web.Endpoints.Games
{
    public class GetAllResponce
    {
        public ICollection<Game> Games { get; set; }
    }
}
