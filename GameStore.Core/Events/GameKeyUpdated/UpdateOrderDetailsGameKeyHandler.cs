using System.Threading;
using System.Threading.Tasks;
using GameStore.Core.Models.Server.Orders;
using GameStore.Core.Models.Server.Orders.Specifications;
using GameStore.SharedKernel.Interfaces.DataAccess;
using MediatR;

namespace GameStore.Core.Events.GameKeyUpdated;

public class UpdateOrderDetailsGameKeyHandler : INotificationHandler<GameKeyUpdatedEvent>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateOrderDetailsGameKeyHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    private IRepository<OrderDetails> OrderDetailsRepository => _unitOfWork.GetEfRepository<OrderDetails>();

    public async Task Handle(GameKeyUpdatedEvent notification, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(notification.OldGameKey))
        {
            return;
        }

        var spec = new OrderDetailsSpec().ByGameKey(notification.OldGameKey);
        var orderDetails = await OrderDetailsRepository.GetBySpecAsync(spec);

        foreach (var orderDetail in orderDetails)
        {
            orderDetail.GameKey = notification.NewGameKey;

            await OrderDetailsRepository.UpdateAsync(orderDetail);
        }

        await _unitOfWork.SaveChangesAsync();
    }
}