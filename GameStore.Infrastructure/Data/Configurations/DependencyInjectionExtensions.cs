using GameStore.Core.Models.Mongo.Categories;
using GameStore.Core.Models.Mongo.Orders;
using GameStore.Core.Models.Mongo.Products;
using GameStore.Core.Models.Mongo.Shippers;
using GameStore.Core.Models.Mongo.Suppliers;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson.Serialization;

namespace GameStore.Infrastructure.Data.Configurations;

public static class DependencyInjectionExtensions
{
    public static void ConfigureNorthwindDatabase(this IServiceCollection services)
    {
        BsonClassMap.RegisterClassMap<OrderDetailsMongo>(cm =>
                                                         {
                                                             cm.AutoMap();
                                                             cm.SetIgnoreExtraElements(true);
                                                         });
        BsonClassMap.RegisterClassMap<OrderMongo>(cm =>
                                                  {
                                                      cm.AutoMap();
                                                      cm.SetIgnoreExtraElements(true);
                                                  });
        BsonClassMap.RegisterClassMap<Product>(cm =>
                                               {
                                                   cm.AutoMap();
                                                   cm.SetIgnoreExtraElements(true);
                                               });

        BsonClassMap.RegisterClassMap<Category>(cm =>
                                                {
                                                    cm.AutoMap();
                                                    cm.SetIgnoreExtraElements(true);
                                                });

        BsonClassMap.RegisterClassMap<Shipper>(cm =>
                                               {
                                                   cm.AutoMap();
                                                   cm.SetIgnoreExtraElements(true);
                                               });

        BsonClassMap.RegisterClassMap<Supplier>(cm =>
                                                {
                                                    cm.AutoMap();
                                                    cm.SetIgnoreExtraElements(true);
                                                });
    }
}