using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using GameStore.Core.Exceptions;
using GameStore.Core.Interfaces;
using GameStore.Core.Interfaces.Loggers;
using GameStore.Core.Models.Dto;
using GameStore.Core.Models.Dto.Filters;
using GameStore.Core.Models.Mongo.Orders;
using GameStore.Core.Models.Mongo.Orders.Filters;
using GameStore.Core.Models.Mongo.Orders.Specifications;
using GameStore.Core.Models.Server.Games;
using GameStore.Core.Models.Server.Orders;
using GameStore.Core.Models.Server.Orders.Filters;
using GameStore.Core.Models.Server.Orders.Specifications;
using GameStore.Core.Models.ServiceModels.Orders;
using GameStore.SharedKernel.Interfaces.DataAccess;
using MongoDB.Bson;

namespace GameStore.Core.Services;

public class OrderService : IOrderService
{
    private readonly IMapper _mapper;
    private readonly IMongoLogger _mongoLogger;
    private readonly ISearchService _searchService;
    private readonly IUnitOfWork _unitOfWork;

    public OrderService(ISearchService searchService, IMongoLogger mongoLogger, IUnitOfWork unitOfWork, IMapper mapper)
    {
        _searchService = searchService;
        _mongoLogger = mongoLogger;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    private IRepository<Order> OrdersEfRepository => _unitOfWork.GetEfRepository<Order>();

    private IRepository<OrderMongo> OrdersMongoRepository => _unitOfWork.GetMongoRepository<OrderMongo>();

    public async Task<ICollection<OrderDto>> GetByFilterAsync(AllOrdersFilter filter)
    {
        var serverOrdersFilter = _mapper.Map<OrdersFilter>(filter);
        var serverOrdersByFilterSpec = new OrdersByFilterSpec(serverOrdersFilter);
        var filteredServerOrders = await OrdersEfRepository.GetBySpecAsync(serverOrdersByFilterSpec);

        var mongoOrdersFilter = _mapper.Map<MongoOrdersFilter>(filter);
        var mongoOrdersByFilterSpec = new MongoOrdersByFilterWithDetailsSpec(mongoOrdersFilter);
        var filteredMongoOrders = await OrdersMongoRepository.GetBySpecAsync(mongoOrdersByFilterSpec);

        var mappedServerOrders = _mapper.Map<IEnumerable<OrderDto>>(filteredServerOrders);
        var mappedMongoOrders = _mapper.Map<IEnumerable<OrderDto>>(filteredMongoOrders);

        var result = mappedServerOrders.Concat(mappedMongoOrders).ToList();

        return result;
    }

    public async Task<Order> GetByIdAsync(Guid orderId)
    {
        var spec = new OrdersSpec().ById(orderId).WithDetails();

        var order = await OrdersEfRepository.GetSingleOrDefaultBySpecAsync(spec)
                    ?? throw new ItemNotFoundException(typeof(Order), orderId);

        await IncludeProductDtoInOrderAsync(order);

        return order;
    }

    public async Task<Order> GetBasketByCustomerIdAsync(Guid customerId)
    {
        var order = await GetActiveOrderByCustomerIdOrDefaultAsync(customerId) ??
                    await GetBasketOrderByCustomerIdOrDefaultAsync(customerId);

        if (order is not null)
        {
            return order;
        }

        var createModel = new OrderCreateModel
        {
            CustomerId = customerId
        };

        order = await CreateAsync(createModel);

        return order;
    }

    public async Task<ICollection<Order>> GetByCustomerIdAsync(Guid customerId)
    {
        var spec = new OrdersSpec().ByCustomerId(customerId);
        var result = await OrdersEfRepository.GetBySpecAsync(spec);

        return result;
    }

    public async Task<Order> CreateAsync(OrderCreateModel createModel)
    {
        var isCustomerHasActiveOrder = await IsCustomerHasActiveOrderAsync(createModel.CustomerId);

        if (isCustomerHasActiveOrder)
        {
            throw new InvalidOperationException("Customer already has an active order");
        }

        var order = _mapper.Map<Order>(createModel);

        order.Status = OrderStatus.Created;

        await OrdersEfRepository.AddAsync(order);
        await _unitOfWork.SaveChangesAsync();

        await _mongoLogger.LogCreateAsync(order);

        return order;
    }

    public async Task MakeOrder(Guid orderId)
    {
        var spec = new OrdersSpec().ById(orderId).WithDetails();

        var order = await OrdersEfRepository.GetSingleOrDefaultBySpecAsync(spec)
                    ?? throw new ItemNotFoundException(typeof(Order), orderId);

        foreach (var orderDetail in order.OrderDetails)
        {
            var game = await _searchService.GetProductDtoByGameKeyOrDefaultAsync(orderDetail.GameKey)
                       ?? throw new ItemNotFoundException(typeof(Game), orderDetail.GameKey,
                                                          nameof(orderDetail.GameKey));

            orderDetail.Price = game.Price;
        }

        await OrdersEfRepository.UpdateAsync(order);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task FillShippersAsync(ActiveOrderCreateModel createModel)
    {
        var spec = new OrdersSpec().ById(createModel.OrderId);

        var order = await OrdersEfRepository.GetSingleOrDefaultBySpecAsync(spec)
                    ?? throw new ItemNotFoundException(typeof(Order), createModel.OrderId, nameof(createModel.OrderId));

        UpdateValues(order, createModel);
        order.OrderDate = DateTime.UtcNow;
        order.Status = OrderStatus.InProcess;

        await OrdersEfRepository.UpdateAsync(order);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task AddToOrderAsync(Guid customerId, BasketItem item)
    {
        var isGameExist = await _searchService.IsGameKeyExistsAsync(item.GameKey);

        if (isGameExist == false)
        {
            throw new ItemNotFoundException(typeof(Game), item.GameKey, nameof(item.GameKey));
        }

        var isCustomerHasActiveOrder = await IsCustomerHasActiveOrderAsync(customerId);

        if (isCustomerHasActiveOrder)
        {
            throw new InvalidOperationException("User already has an active order");
        }

        var order = await GetBasketByCustomerIdAsync(customerId);
        var game = await _searchService.GetProductDtoByGameKeyOrDefaultAsync(item.GameKey);

        if (TryGetOrderDetailsForGameKey(order, item.GameKey, out var orderDetails))
        {
            var quantity = GetQuantity(item, orderDetails, game);

            orderDetails.Quantity = quantity;
        }
        else
        {
            var orderDetail = _mapper.Map<OrderDetails>(item);

            var quantity = GetQuantity(orderDetail, game);
            orderDetail.Quantity = quantity;
            orderDetail.OrderId = order.Id;
            order.OrderDetails.Add(orderDetail);
        }

        await OrdersEfRepository.UpdateAsync(order);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task UpdateAsync(OrderUpdateModel updateModel)
    {
        var spec = new OrdersSpec().ById(updateModel.Id).WithDetails();
        var order = await OrdersEfRepository.GetSingleOrDefaultBySpecAsync(spec);

        var oldOrderVersion = order.ToBsonDocument();

        UpdateValues(order, updateModel);
        await OrdersEfRepository.UpdateAsync(order);
        await _unitOfWork.SaveChangesAsync();

        await _mongoLogger.LogUpdateAsync(typeof(Order), oldOrderVersion, order.ToBsonDocument());
    }

    public async Task<bool> IsCustomerHasActiveOrderAsync(Guid customerId)
    {
        return await OrdersEfRepository.AnyAsync(new OrdersSpec().InProcess().ByCustomerId(customerId));
    }

    public async Task<bool> IsCustomerHasBasketAsync(Guid customerId)
    {
        return await OrdersEfRepository.AnyAsync(new OrdersSpec().InBasket().ByCustomerId(customerId));
    }

    private async Task<Order> GetBasketOrderByCustomerIdOrDefaultAsync(Guid customerId)
    {
        var spec = new OrdersSpec().InBasket().ByCustomerId(customerId).WithDetails();
        var order = await OrdersEfRepository.GetSingleOrDefaultBySpecAsync(spec);

        if (order is not null)
        {
            await IncludeProductDtoInOrderAsync(order);
        }

        return order;
    }

    private async Task<Order> GetActiveOrderByCustomerIdOrDefaultAsync(Guid customerId)
    {
        var spec = new OrdersSpec().InProcess().ByCustomerId(customerId).WithDetails();

        var order = await OrdersEfRepository.GetSingleOrDefaultBySpecAsync(spec);

        if (order is not null)
        {
            await IncludeProductDtoInOrderAsync(order);
        }

        return order;
    }

    private async Task IncludeProductDtoInOrderAsync(Order order)
    {
        foreach (var orderDetail in order.OrderDetails)
        {
            var game = await _searchService.GetProductDtoByGameKeyOrDefaultAsync(orderDetail.GameKey);
            orderDetail.Game = game;
        }
    }

    private bool TryGetOrderDetailsForGameKey(Order order, string gameKey, out OrderDetails orderDetails)
    {
        var detailsOfSameGameKey = order.OrderDetails.FirstOrDefault(details => details.GameKey == gameKey);

        orderDetails = detailsOfSameGameKey;

        return detailsOfSameGameKey is not null;
    }

    private int GetQuantity(BasketItem item, OrderDetails orderDetails, ProductDto game)
    {
        var quantity = orderDetails.Quantity + item.Quantity > game.UnitsInStock
            ? game.UnitsInStock
            : orderDetails.Quantity + item.Quantity;

        return quantity;
    }

    private int GetQuantity(OrderDetails orderDetails, ProductDto game)
    {
        var quantity = orderDetails.Quantity > game.UnitsInStock
            ? game.UnitsInStock
            : orderDetails.Quantity;

        return quantity;
    }

    private void UpdateValues(Order order, OrderUpdateModel updateModel)
    {
        order.CustomerId = updateModel.CustomerId;
        order.OrderDetails = updateModel.OrderDetails;
        order.OrderDate = updateModel.OrderDate;
        order.Status = updateModel.Status;

        order.OrderDetails = updateModel.OrderDetails;
    }

    private void UpdateValues(Order order, ActiveOrderCreateModel createModel)
    {
        order.Freight = createModel.Freight;
        order.ShipAddress = createModel.ShipAddress;
        order.ShipCity = createModel.ShipCity;
        order.ShipCountry = createModel.ShipCountry;
        order.ShipName = createModel.ShipName;
        order.ShipPostalCode = createModel.ShipPostalCode;
        order.ShipRegion = createModel.ShipRegion;
        order.ShipperId = createModel.ShipperId;
    }
}