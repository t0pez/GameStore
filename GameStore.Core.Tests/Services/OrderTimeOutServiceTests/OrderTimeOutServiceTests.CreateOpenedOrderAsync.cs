using System.Threading.Tasks;
using AutoFixture.Xunit2;
using GameStore.Core.Interfaces;
using GameStore.Core.Models.Dto;
using GameStore.Core.Models.Server.Orders;
using GameStore.Core.Models.ServiceModels.Games;
using GameStore.Core.Services.TimeOutServices;
using GameStore.Tests.Infrastructure.Attributes;
using Moq;
using Xunit;

namespace GameStore.Core.Tests.Services.OrderTimeOutServiceTests;

public partial class OrderTimeOutServiceTests
{
    [Theory]
    [AutoMoqData]
    public async Task CreateOpenedOrderAsync_OrderAsAParameter_ReducesGameUnitsInStock(
        Order order,
        ProductDto game,
        [Frozen] Mock<ISearchService> searchServiceMock,
        [Frozen] Mock<IGameService> gameServiceMock,
        OrderTimeOutService sut)
    {
        searchServiceMock.Setup(service => service.GetProductDtoByGameKeyOrDefaultAsync(game.Key))
                         .ReturnsAsync(game);

        await sut.CreateOpenedOrderAsync(order);

        gameServiceMock.Verify(service => service.UpdateAsync(It.IsAny<GameUpdateModel>()));
    }
}