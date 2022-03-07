using Ardalis.Specification;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameStore.Core.Models.Games.Specifications
{
    internal class GamesWithDetailsSpec : Specification<Game>
    {
        public GamesWithDetailsSpec()
        {
            Query
                .Where(g => g.IsDeleted == false)
                .Include(g => g.Comments)
                .Include(g => g.Genres)
                .Include(g => g.PlatformTypes);
        }
    }
}
