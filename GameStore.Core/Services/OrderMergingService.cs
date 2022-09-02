using System;
using System.Linq;
using System.Threading.Tasks;
using GameStore.Core.Interfaces;
using GameStore.Core.Models.Server.Orders;
using GameStore.Core.Models.Server.Orders.Specifications;
using GameStore.SharedKernel.Interfaces.DataAccess;

namespace GameStore.Core.Services;

public class OrderMergingService : IOrderMergingService
{
    private readonly IUnitOfWork _unitOfWork;

    public OrderMergingService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    private IRepository<Order> OrdersRepository => _unitOfWork.GetEfRepository<Order>();

    public async Task MergeOrdersAsync(Guid sourceUserId, Guid targetUserId)
    {
        Task<Order> GetBasketByUserIdAsync(Guid userId)
        {
            var spec = new OrdersSpec().InBasket().ByCustomerId(userId).WithDetails();

            return OrdersRepository.GetSingleOrDefaultBySpecAsync(spec);
        }

        var sourceOrder = await GetBasketByUserIdAsync(sourceUserId);
        var targetOrder = await GetBasketByUserIdAsync(targetUserId);

        if (sourceOrder?.OrderDetails.Any() == false)
        {
            return;
        }

        if (targetOrder is null)
        {
            sourceOrder.CustomerId = targetUserId;

            await OrdersRepository.UpdateAsync(sourceOrder);
            await _unitOfWork.SaveChangesAsync();

            return;
        }

        foreach (var orderDetail in sourceOrder.OrderDetails)
        {
            if (IsOrderContainsSameOrderDetails(targetOrder, orderDetail))
            {
                var targetOrderDetails =
                    targetOrder.OrderDetails.First(details => details.GameKey == orderDetail.GameKey);

                targetOrderDetails.Quantity += orderDetail.Quantity;
            }
            else
            {
                targetOrder.OrderDetails.Add(orderDetail);
            }
        }

        sourceOrder.IsDeleted = true;

        await OrdersRepository.UpdateAsync(sourceOrder);
        await OrdersRepository.UpdateAsync(targetOrder);
        await _unitOfWork.SaveChangesAsync();
    }

    private static bool IsOrderContainsSameOrderDetails(Order targetOrder, OrderDetails orderDetail)
    {
        return targetOrder.OrderDetails.Any(details => details.GameKey == orderDetail.GameKey);
    }
}