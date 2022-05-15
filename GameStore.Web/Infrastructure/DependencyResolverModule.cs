using GameStore.Web.Interfaces;
using GameStore.Web.Services;
using Microsoft.Extensions.DependencyInjection;

namespace GameStore.Web.Infrastructure;

public static class DependencyResolverModule
{
    public static void ConfigureWebServices(this IServiceCollection services)
    {
        services.AddScoped<IBasketCookieService, BasketCookieService>();
    }
}