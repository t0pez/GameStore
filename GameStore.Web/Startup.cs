using GameStore.Core.Events.GameKeyUpdated;
using GameStore.Core.Profiles;
using GameStore.Infrastructure;
using GameStore.Infrastructure.Data.Configurations;
using GameStore.Infrastructure.Data.Context;
using GameStore.Web.Filters;
using GameStore.Web.Infrastructure;
using GameStore.Web.Middlewares;
using GameStore.Web.Profiles;
using MediatR;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Quartz;

namespace GameStore.Web;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddMvc();

        services.AddMediatR(typeof(Startup), typeof(GameKeyUpdatedEvent))
                .AddHttpContextAccessor();

        services.AddControllers()
                .AddRazorRuntimeCompilation()
                .AddNewtonsoftJson(options =>
                                   {
                                       options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                                   });

        services.AddQuartz(configurator =>
                           {
                               configurator.UseMicrosoftDependencyInjectionJobFactory();
                               configurator.AddOrderTimeOutHostedService();
                           });

        services.AddQuartzHostedService(options => options.WaitForJobsToComplete = true);

        services.AddScoped<WorkTimeTrackingFilter>();

        services.AddDbContext<ApplicationContext>(options =>
                                                  {
                                                      var connectionString =
                                                          Configuration.GetConnectionString("DefaultConnection");

                                                      options.UseSqlServer(connectionString);
                                                  });

        services.ConfigureNorthwindDatabase();

        services.ConfigureDomainServices(Configuration);

        services.ConfigureWebServices();

        services.ConfigureMiddlewares();

        services.AddAutoMapper(
            configuration =>
            {
                configuration.AddCoreProfiles();
                configuration.AddWebProfiles();
            });

        services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                           {
                               options.LoginPath = "/auth/login";
                               options.AccessDeniedPath = "/unauthorized";
                           });
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseMiddleware<ExceptionMiddleware>();
        app.UseMiddleware<UserCookieIdMiddleware>();
        app.UseMiddleware<UserRoleMiddleware>();

        app.UseHttpsRedirection();
        app.UseRouting();
        app.UseStaticFiles();
        app.UseCookiePolicy();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
                         {
                             endpoints.MapControllers();
                             endpoints.MapFallbackToController("GetAll", "Games");
                         });
    }
}