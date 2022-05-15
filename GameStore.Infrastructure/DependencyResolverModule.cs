using GameStore.Core.Helpers.OrderMapping;
using GameStore.Core.Helpers.PdfGenerators;
using GameStore.Core.Interfaces;
using GameStore.Core.Interfaces.PaymentMethods;
using GameStore.Core.Interfaces.RelationshipModelsServices;
using GameStore.Core.Services;
using GameStore.Core.Services.PaymentMethods;
using GameStore.Core.Services.RelationshipModelsServices;
using GameStore.Infrastructure.Data.Repositories;
using GameStore.SharedKernel.Interfaces.DataAccess;
using Microsoft.Extensions.DependencyInjection;

namespace GameStore.Infrastructure;

public static class DependencyResolverModule
{
    public static void ConfigureDomainServices(this IServiceCollection services)
    {
        services.AddScoped<IUnitOfWork, UnitOfWork>()
                .AddScoped<IGameService, GameService>()
                .AddScoped<ICommentService, CommentService>()
                .AddScoped<IPublisherService, PublisherService>()
                .AddScoped<IBasketService, BasketService>()
                .AddScoped<IGenreService, GenreService>()
                .AddScoped<IPlatformTypeService, PlatformTypeService>()
                .AddScoped<IOrderService, OrderService>()
                .AddScoped<IOrderMappingHelper, OrderMappingHelper>()
                .AddScoped<IPaymentService, PaymentService>()
                .AddScoped<IPaymentMethodFactory, PaymentMethodFactory>()
                .AddScoped<IInvoiceFileGenerator, InvoiceFileGenerator>()
                .AddScoped(typeof(IRelationshipModelService<>), typeof(RelationshipModelService<>));
    }
}