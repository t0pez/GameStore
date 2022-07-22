﻿using System.Threading;
using System.Threading.Tasks;
using GameStore.Core.Events.Notifications;
using GameStore.Core.Models.Games;
using GameStore.Core.Models.Games.Specifications;
using GameStore.SharedKernel.Interfaces.DataAccess;
using GameStore.SharedKernel.Specifications.Extensions;
using MediatR;

namespace GameStore.Core.Events.NotificationHandlers;

public class GamesPublisherNameUpdatedHandler : INotificationHandler<PublisherNameUpdatedNotification>
{
    private readonly IUnitOfWork _unitOfWork;

    public GamesPublisherNameUpdatedHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    private IRepository<Game> GameRepository => _unitOfWork.GetEfRepository<Game>();

    public async Task Handle(PublisherNameUpdatedNotification notification, CancellationToken cancellationToken)
    {
        var games = await GameRepository.GetBySpecAsync(new GamesByPublisherNameSpec(notification.OldName)
                                                            .IncludeDeleted());

        foreach (var game in games)
        {
            game.PublisherName = notification.NewName;

            await GameRepository.UpdateAsync(game);
        }

        await _unitOfWork.SaveChangesAsync();
    }
}