using Ardalis.Specification;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameStore.Core.Models.Games.Specifications
{
    internal class GamesWithDetails : Specification<Game>
    {
        public GamesWithDetails()
        {
            Query
                .Where(g => g.IsDeleted == false)
                .Include(g => g.Comments)
                .Include(g => g.Genres)
                .Include(g => g.PlatformTypes);
        }
    }
}
