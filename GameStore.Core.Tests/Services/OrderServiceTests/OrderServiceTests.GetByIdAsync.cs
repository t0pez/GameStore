using System;
using System.Collections.Generic;
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
    public async Task GetByCustomerIdAsync_ExistingCustomer_ReturnsCorrectResult(
        Guid customerId,
        List<Order> orders,
        [Frozen] Mock<IUnitOfWork> unitOfWorkMock,
        OrderService sut)
    {
        unitOfWorkMock.Setup(repository =>
                                 repository.GetEfRepository<Order>().GetBySpecAsync(
                                     It.IsAny<OrdersSpec>()))
                      .ReturnsAsync(orders);

        var actualResult = await sut.GetByCustomerIdAsync(customerId);

        actualResult.Should().BeEquivalentTo(orders);
    }
}