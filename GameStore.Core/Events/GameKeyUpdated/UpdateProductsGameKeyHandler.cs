using System.Threading;
using System.Threading.Tasks;
using GameStore.Core.Models.Mongo.Products;
using GameStore.Core.Models.Mongo.Products.Specifications;
using GameStore.SharedKernel.Interfaces.DataAccess;
using MediatR;

namespace GameStore.Core.Events.GameKeyUpdated;

public class UpdateProductsGameKeyHandler : INotificationHandler<GameKeyUpdatedEvent>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateProductsGameKeyHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    private IRepository<Product> ProductsRepository => _unitOfWork.GetMongoRepository<Product>();

    public async Task Handle(GameKeyUpdatedEvent notification, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(notification.OldGameKey))
        {
            return;
        }

        var spec = new ProductsSpec().ByGameKey(notification.OldGameKey);
        var products = await ProductsRepository.GetBySpecAsync(spec);

        foreach (var product in products)
        {
            product.GameKey = notification.NewGameKey;

            await ProductsRepository.UpdateAsync(product);
        }
    }
}