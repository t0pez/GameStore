using System.Threading.Tasks;
using AutoFixture.Xunit2;
using FluentAssertions;
using GameStore.Core.Models.Server.Orders;
using GameStore.Core.Models.Server.Orders.Specifications;
using GameStore.Core.Models.ServiceModels.Orders;
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
    public async Task CreateAsync_CorrectValues_ReturnsOrder(
        OrderCreateModel createModel,
        [Frozen] Mock<IUnitOfWork> unitOfWorkMock,
        OrderService sut)
    {
        unitOfWorkMock
           .Setup(repository =>
                      repository.GetEfRepository<Order>().AnyAsync(It.IsAny<OrdersSpec>()))
           .ReturnsAsync(false);

        var actualResult = await sut.CreateAsync(createModel);

        actualResult.CustomerId.Should().Be(createModel.CustomerId);

        unitOfWorkMock.Verify(repository => repository.GetEfRepository<Order>().AddAsync(It.IsAny<Order>()),
                              Times.Once);

        unitOfWorkMock.Verify(unitOfWork => unitOfWork.SaveChangesAsync(), Times.Once);
    }
}