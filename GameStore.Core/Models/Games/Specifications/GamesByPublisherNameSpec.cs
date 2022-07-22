using Ardalis.Specification;
using GameStore.SharedKernel.Specifications;

namespace GameStore.Core.Models.Games.Specifications;

public class GamesByPublisherNameSpec : SafeDeleteSpec<Game>
{
    public GamesByPublisherNameSpec(string publisherName)
    {
        PublisherName = publisherName;

        Query
            .Where(game => game.PublisherName == publisherName);
    }
    
    public string PublisherName { get; set; }
}