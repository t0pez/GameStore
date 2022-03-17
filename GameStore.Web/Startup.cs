using GameStore.Infrastructure;
using GameStore.Infrastructure.Data.Context;
using GameStore.Web.Converters;
using GameStore.Web.Middlewares;
using GameStore.Web.Profiles;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Text.Json.Serialization;
using AutoMapper;
using GameStore.Core.Profiles;

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
        services.AddMvc().AddHybridModelBinder();
        
        services.AddControllers().AddJsonOptions(opt =>
        {
            opt.JsonSerializerOptions.Converters.Add(new ByteArrayJsonConverter());
            opt.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        });

        services.ConfigureDomainServices();

        services.AddDbContext<ApplicationContext>(opt =>
        {
            var connectionString = Configuration.GetConnectionString("DefaultConnection");
            opt.UseSqlServer(connectionString);
        });

        services.AddAutoMapper(cfg => cfg.AddProfiles(
            new Profile[] { new WebCommonProfile(), new CoreCommonProfile() }));
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ApplicationContext dbContext)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        dbContext.Database.EnsureCreated();
        dbContext.SeedData();
            
        app.UseMiddleware<ExceptionMiddleware>();

        app.UseHttpsRedirection();

        app.UseRouting();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}