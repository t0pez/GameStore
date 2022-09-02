using System.Threading.Tasks;
using AutoFixture.Xunit2;
using FluentAssertions;
using GameStore.Core.Models.Server.Orders;
using GameStore.Core.Models.Server.Orders.Specifications;
using GameStore.Core.Services;
using GameStore.SharedKernel.Interfaces.DataAccess;
using GameStore.Tests.Infrastructure.Attributes;
using Moq;
using Xunit;

namespace GameStore.Core.Tests.Services.OrderServiceTests;

public partial class OrderServiceTests
{
    [Theory]
    [AutoMoqData]
    public async Task GetByIdAsync_ExistingOrderId_ReturnsCorrectResult(
        Order order,
        [Frozen] Mock<IUnitOfWork> unitOfWorkMock,
        OrderService sut)
    {
        unitOfWorkMock.Setup(repository =>
                                 repository.GetEfRepository<Order>().GetSingleOrDefaultBySpecAsync(
                                     It.IsAny<OrdersSpec>()))
                      .ReturnsAsync(order);

        var actualResult = await sut.GetByIdAsync(order.Id);

        actualResult.Id.Should().Be(order.Id);
    }
}