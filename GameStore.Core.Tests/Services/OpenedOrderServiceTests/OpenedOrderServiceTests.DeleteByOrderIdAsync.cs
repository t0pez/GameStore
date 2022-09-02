using AutoFixture.Xunit2;
using GameStore.Core.Models.Server.Orders;
using GameStore.Core.Models.Server.Orders.Specifications;
using GameStore.Core.Services.TimeOutServices;
using GameStore.SharedKernel.Interfaces.DataAccess;
using GameStore.Tests.Infrastructure.Attributes;
using Moq;
using Xunit;

namespace GameStore.Core.Tests.Services.OpenedOrderServiceTests;

public partial class OpenedOrderServiceTests
{
    [Theory]
    [AutoMoqData]
    public async void DeleteByOrderIdAsync_ExistingOpenedOrder_DeletesOpenedOrder(
        OpenedOrder openedOrder,
        [Frozen] Mock<IUnitOfWork> unitOfWorkMock,
        OpenedOrderService openedOrderService)
    {
        unitOfWorkMock.Setup(repository => repository.GetEfRepository<OpenedOrder>()
                                                     .GetSingleOrDefaultBySpecAsync(It.IsAny<OpenedOrdersSpec>()))
                      .ReturnsAsync(openedOrder);

        await openedOrderService.DeleteByOrderIdAsync(openedOrder.OrderId);

        unitOfWorkMock.Verify(repository => repository.GetEfRepository<OpenedOrder>().DeleteAsync(openedOrder));
        unitOfWorkMock.Verify(unitOfWork => unitOfWork.SaveChangesAsync());
    }
}