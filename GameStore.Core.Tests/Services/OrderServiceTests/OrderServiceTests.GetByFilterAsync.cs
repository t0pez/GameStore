using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using FluentAssertions;
using GameStore.Core.Models.Dto.Filters;
using GameStore.Core.Models.Mongo.Orders;
using GameStore.Core.Models.Mongo.Orders.Specifications;
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
    public async Task GetByFilterAsync_ReturnsCorrectResult(
        AllOrdersFilter filter,
        List<Order> orders,
        List<OrderMongo> ordersMongo,
        [Frozen] Mock<IUnitOfWork> unitOfWorkMock,
        OrderService sut)
    {
        unitOfWorkMock.Setup(repository =>
                                 repository.GetEfRepository<Order>().GetBySpecAsync(It.IsAny<OrdersByFilterSpec>()))
                      .ReturnsAsync(orders);

        unitOfWorkMock
           .Setup(repository => repository.GetMongoRepository<OrderMongo>()
                                          .GetBySpecAsync(It.IsAny<MongoOrdersByFilterWithDetailsSpec>()))
           .ReturnsAsync(ordersMongo);

        var actualResult = await sut.GetByFilterAsync(filter);

        var expectedCount = orders.Count + ordersMongo.Count;
        actualResult.Should().HaveCount(expectedCount);
    }
}