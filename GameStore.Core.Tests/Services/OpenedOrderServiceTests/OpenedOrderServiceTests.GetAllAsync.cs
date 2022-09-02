using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using FluentAssertions;
using GameStore.Core.Models.Server.Orders;
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
    public async Task GetAllAsync_NoParams_ReturnsCorrectValue(
        List<OpenedOrder> openedOrders,
        [Frozen] Mock<IUnitOfWork> unitOfWorkMock,
        OpenedOrderService sut)
    {
        unitOfWorkMock.Setup(repository => repository.GetEfRepository<OpenedOrder>().GetBySpecAsync(null))
                      .ReturnsAsync(openedOrders);

        var actualResult = await sut.GetAllAsync();

        actualResult.Should().BeEquivalentTo(openedOrders);
    }
}