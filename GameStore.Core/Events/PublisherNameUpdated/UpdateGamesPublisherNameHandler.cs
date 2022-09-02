using System.Threading;
using System.Threading.Tasks;
using GameStore.Core.Models.Server.Games;
using GameStore.Core.Models.Server.Games.Specifications;
using GameStore.SharedKernel.Interfaces.DataAccess;
using MediatR;

namespace GameStore.Core.Events.PublisherNameUpdated;

public class UpdateGamesPublisherNameHandler : INotificationHandler<PublisherNameUpdatedEvent>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateGamesPublisherNameHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    private IRepository<Game> GameRepository => _unitOfWork.GetEfRepository<Game>();

    public async Task Handle(PublisherNameUpdatedEvent notification, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(notification.OldName))
        {
            return;
        }

        var spec = new GamesSpec().ByPublisherName(notification.OldName).LoadAll();

        var games = await GameRepository.GetBySpecAsync(spec);

        foreach (var game in games)
        {
            game.PublisherName = notification.NewName;

            await GameRepository.UpdateAsync(game);
        }

        await _unitOfWork.SaveChangesAsync();
    }
}