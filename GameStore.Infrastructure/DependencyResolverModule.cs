using GameStore.Core.Interfaces;
using GameStore.Core.Services;
using GameStore.Infrastructure.Data.Repositories;
using GameStore.SharedKernel.Interfaces.DataAccess;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameStore.Infrastructure
{
    public static class DependencyResolverModule
    {
        public static void ConfigureDomainServices(this IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>()
                    .AddScoped<IGameService, GameService>()
                    .AddScoped<ICommentService, CommentService>();
        }
    }
}
