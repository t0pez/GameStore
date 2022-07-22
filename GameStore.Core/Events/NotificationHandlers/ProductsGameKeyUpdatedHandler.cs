using System.Threading;
using System.Threading.Tasks;
using GameStore.Core.Events.Notifications;
using GameStore.Core.Models.Mongo.Products;
using GameStore.Core.Models.Mongo.Products.Specifications;
using GameStore.SharedKernel.Interfaces.DataAccess;
using MediatR;

namespace GameStore.Core.Events.NotificationHandlers;

public class ProductsGameKeyUpdatedHandler : INotificationHandler<GameKeyUpdatedNotification>
{
    private readonly IUnitOfWork _unitOfWork;

    public ProductsGameKeyUpdatedHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    private IRepository<Product> ProductsRepository => _unitOfWork.GetMongoRepository<Product>();

    public async Task Handle(GameKeyUpdatedNotification notification, CancellationToken cancellationToken)
    {
        var products = await ProductsRepository.GetBySpecAsync(new ProductsByGameKeySpec(notification.OldGameKey));

        foreach (var product in products)
        {
            product.GameKey = notification.NewGameKey;

            await ProductsRepository.UpdateAsync(product);
        }
    }
}