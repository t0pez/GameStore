using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using GameStore.Core.Exceptions;
using GameStore.Core.Models.Orders;
using GameStore.Core.Models.Orders.Specifications;
using GameStore.Core.Services;
using GameStore.SharedKernel.Interfaces.DataAccess;
using Moq;
using Xunit;

namespace GameStore.Core.Tests.Services;

public class OpenedOrderServiceTests
{
    private readonly OpenedOrderService _openedOrderService;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IRepository<OpenedOrder>> _openedOrderRepoMock;

    public OpenedOrderServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _openedOrderRepoMock = new Mock<IRepository<OpenedOrder>>();

        _unitOfWorkMock.Setup(unitOfWork => unitOfWork.GetRepository<OpenedOrder>())
                       .Returns(_openedOrderRepoMock.Object);
        _unitOfWorkMock.Setup(unitOfWork => unitOfWork.SaveChangesAsync())
                       .Verifiable();
        
        _openedOrderService = new OpenedOrderService(_unitOfWorkMock.Object);
    }

    [Fact]
    public async void GetAllAsync_NoParameters_ReturnsCorrectValue()
    {
        const int expectedCount = 4;
        var expectedResult = new List<OpenedOrder>(new OpenedOrder[expectedCount]);
        
        _openedOrderRepoMock.Setup(repository => repository.GetBySpecAsync(null))
                            .ReturnsAsync(expectedResult);

        var actualResult = await _openedOrderService.GetAllAsync();

        actualResult.Count().Should().Be(expectedCount);
    }

    [Fact]
    public async void CreateAsync_CreatesNewOpenedOrder()
    {
        var openedOrder = new OpenedOrder
        {
            OrderId = Guid.NewGuid(),
            TimeOutDate = DateTime.Now.AddMonths(-2)
        };

        _openedOrderRepoMock.Setup(repository => repository.AddAsync(openedOrder))
                            .Verifiable();

        await _openedOrderService.CreateAsync(openedOrder);
        
        _openedOrderRepoMock.Verify(repository => repository.AddAsync(openedOrder));
        _unitOfWorkMock.Verify(unitOfWork => unitOfWork.SaveChangesAsync());
    }
    
    [Fact]
    public async void DeleteByOrderIdAsync_ExistingOpenedOrder_DeletesOpenedOrder()
    {
        var openedOrderId = Guid.NewGuid();
        
        var openedOrder = new OpenedOrder
        {
            OrderId = openedOrderId,
            TimeOutDate = DateTime.Now.AddMonths(-2)
        };

        _openedOrderRepoMock.Setup(repository =>
                                       repository.GetSingleOrDefaultBySpecAsync(
                                           It.Is<OpenedOrderByOrderIdSpec>(spec => spec.OrderId == openedOrderId)))
                            .ReturnsAsync(openedOrder);
        _openedOrderRepoMock.Setup(repository => repository.DeleteAsync(openedOrder))
                            .Verifiable();

        await _openedOrderService.CreateAsync(openedOrder);
        
        _openedOrderRepoMock.Verify(repository => repository.AddAsync(openedOrder));
        _unitOfWorkMock.Verify(unitOfWork => unitOfWork.SaveChangesAsync());
    }
    
    [Fact]
    public async void DeleteByOrderIdAsync_NotExistingOpenedOrder_ThrowsException()
    {
        var notExistingOrderId = Guid.NewGuid();

        _openedOrderRepoMock.Setup(repository =>
                                       repository.GetSingleOrDefaultBySpecAsync(
                                           It.Is<OpenedOrderByOrderIdSpec>(spec => spec.OrderId == notExistingOrderId)))
                            .ThrowsAsync(new ItemNotFoundException());

        var deleteMethod = async () => await _openedOrderService.DeleteByOrderIdAsync(notExistingOrderId);

        await deleteMethod.Should().ThrowAsync<ItemNotFoundException>();
    }
}