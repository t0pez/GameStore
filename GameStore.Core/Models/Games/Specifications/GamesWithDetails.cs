using Ardalis.Specification;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameStore.Core.Models.Games.Specifications
{
    internal class GamesWithDetails : Specification<Game>, ISingleResultSpecification
    {
        public GamesWithDetails()
        {
            Query
                .Include(g => g.Comments)
                .Include(g => g.Genres)
                .Include(g => g.PlatformTypes);
        }
    }
}
