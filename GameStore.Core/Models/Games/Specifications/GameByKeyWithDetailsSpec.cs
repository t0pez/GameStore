﻿using Ardalis.Specification;

namespace GameStore.Core.Models.Games.Specifications;

public sealed class GameByKeyWithDetailsSpec : Specification<Game>, ISingleResultSpecification
{
    public GameByKeyWithDetailsSpec(string gameKey)
    {
        Key = gameKey;
        
        Query
            .Where(g => g.Key == gameKey
                        && g.IsDeleted == false)
            .Include(g => g.Comments)
            .Include(g => g.Genres)
            .ThenInclude(gameGenre => gameGenre.Genre)
            .Include(g => g.Platforms)
            .ThenInclude(gamePlatform => gamePlatform.Platform);
    }
    
    public string Key { get; }
}