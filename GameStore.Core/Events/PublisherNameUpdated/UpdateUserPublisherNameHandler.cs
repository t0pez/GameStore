using System.Threading;
using System.Threading.Tasks;
using GameStore.Core.Models.Server.Users;
using GameStore.Core.Models.Server.Users.Specifications;
using GameStore.SharedKernel.Interfaces.DataAccess;
using MediatR;

namespace GameStore.Core.Events.PublisherNameUpdated;

public class UpdateUsersPublisherNameHandler : INotificationHandler<PublisherNameUpdatedEvent>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateUsersPublisherNameHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    private IRepository<User> Repository => _unitOfWork.GetEfRepository<User>();

    public async Task Handle(PublisherNameUpdatedEvent notification, CancellationToken cancellationToken)
    {
        var spec = new UsersSpec().ByPublisherName(notification.OldName);

        var users = await Repository.GetBySpecAsync(spec);

        foreach (var user in users)
        {
            user.PublisherName = notification.NewName;
            await Repository.UpdateAsync(user);
        }

        await _unitOfWork.SaveChangesAsync();
    }
}