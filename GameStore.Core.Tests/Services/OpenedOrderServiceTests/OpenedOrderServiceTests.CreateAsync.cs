using AutoFixture.Xunit2;
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
    public async void CreateAsync_CreatesNewOpenedOrder(
        OpenedOrder openedOrder,
        [Frozen] Mock<IUnitOfWork> unitOfWorkMock,
        OpenedOrderService sut)
    {
        await sut.CreateAsync(openedOrder);

        unitOfWorkMock.Verify(repository => repository.GetEfRepository<OpenedOrder>().AddAsync(openedOrder));
        unitOfWorkMock.Verify(unitOfWork => unitOfWork.SaveChangesAsync());
    }
}