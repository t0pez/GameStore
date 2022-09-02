using GameStore.Web.Interfaces;
using GameStore.Web.Middlewares;
using GameStore.Web.Services;
using Microsoft.Extensions.DependencyInjection;

namespace GameStore.Web.Infrastructure;

public static class DependencyResolverModule
{
    public static void ConfigureWebServices(this IServiceCollection services)
    {
        services.AddScoped<IUserCookieService, UserCookieService>()
                .AddScoped<IPublisherAuthHelper, PublisherAuthHelper>()
                .AddScoped<IUserAuthHelper, UserAuthHelper>();
    }

    public static void ConfigureMiddlewares(this IServiceCollection services)
    {
        services.AddTransient<ExceptionMiddleware>()
                .AddTransient<UserCookieIdMiddleware>()
                .AddTransient<UserRoleMiddleware>();
    }
}