using FluentValidation;
using PriceNegotiationApp.Database.DbContext;
using Microsoft.EntityFrameworkCore;
using PriceNegotiationApp.Services;
using PriceNegotiationApp.Services.Interfaces;
using PriceNegotiationApp.Database.Repositories;
using PriceNegotiationApp.Database.Repositories.Interfaces;
using PriceNegotiationApp.Models.Dtos;
using PriceNegotiationApp.Validators;

namespace PriceNegotiationApp;

public class Startup
{
    private readonly IConfiguration _configuration;

    public Startup(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        ConfigureControllers(services);
        ConfigureSwagger(services);
        ConfigureScopedServices(services);
        ConfigureDatabaseContext(services);
    }

    private static void ConfigureScopedServices(IServiceCollection services)
    {
        services.AddControllersWithViews();
        services.AddRazorPages();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<INegotiationService, NegotiationService>();
        services.AddScoped<INegotiationRepository, NegotiationRepository>();
        services.AddScoped<IPropositionRepository, PropositionRepository>();
        services.AddScoped<IValidator<UserRegisterDto>, UserRegisterModelValidator>();
        services.AddScoped<IValidator<Product>, ProductModelValidator>();
        services.AddSingleton<IPasswordHasher, PasswordHascher>();
    }

    private static void ConfigureControllers(IServiceCollection services)
    {
        services.AddControllers().AddNewtonsoftJson(options =>
            options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);
        services.AddEndpointsApiExplorer();
    }

    private void ConfigureDatabaseContext(IServiceCollection services)
    {
        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseNpgsql(_configuration.GetConnectionString("AppDbContext"));
        });
    }

    private static void ConfigureSwagger(IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            options.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
        });
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, AppDbContext dbContext)
    {
        if (!env.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        }

        if (env.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseRouting();

        app.Use(async (context, next) =>
                {
                    if (!context.Request.Cookies.TryGetValue("ClientId", out var anonIdStr))
                    {
                        var newId = Guid.NewGuid().ToString();
                        context.Response.Cookies.Append("ClientId", newId, new CookieOptions
                        {
                            HttpOnly = true,
                            SameSite = SameSiteMode.Strict,
                            Expires = DateTimeOffset.UtcNow.AddDays(30)
                        });
                        context.Items["ClientId"] = newId;
                    }
                    else
                    {
                        context.Items["ClientId"] = anonIdStr;
                    }

                    await next.Invoke();
                });

        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();
        });
    }
}
