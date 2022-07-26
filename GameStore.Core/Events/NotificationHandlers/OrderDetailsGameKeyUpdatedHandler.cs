using System.Threading;
using System.Threading.Tasks;
using GameStore.Core.Events.Notifications;
using GameStore.Core.Models.Orders;
using GameStore.Core.Models.Orders.Specifications;
using GameStore.SharedKernel.Interfaces.DataAccess;
using MediatR;

namespace GameStore.Core.Events.NotificationHandlers;

public class OrderDetailsGameKeyUpdatedHandler : INotificationHandler<GameKeyUpdatedNotification>
{
    private readonly IUnitOfWork _unitOfWork;

    public OrderDetailsGameKeyUpdatedHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    private IRepository<OrderDetails> OrderDetailsRepository => _unitOfWork.GetEfRepository<OrderDetails>();

    public async Task Handle(GameKeyUpdatedNotification notification, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(notification.OldGameKey))
        {
            return;
        }
        
        var orderDetails =
            await OrderDetailsRepository.GetBySpecAsync(new OrderDetailsByGameKeySpec(notification.OldGameKey));

        foreach (var orderDetail in orderDetails)
        {
            orderDetail.GameKey = notification.NewGameKey;

            await OrderDetailsRepository.UpdateAsync(orderDetail);
        }

        await _unitOfWork.SaveChangesAsync();
    }
}