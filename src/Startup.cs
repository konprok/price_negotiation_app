using System.Text;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using PriceNegotiationApp.Database.DbContext;
using PriceNegotiationApp.Database.Repositories;
using PriceNegotiationApp.Database.Repositories.Interfaces;
using PriceNegotiationApp.Models.Dtos;
using PriceNegotiationApp.Services;
using PriceNegotiationApp.Services.Interfaces;
using PriceNegotiationApp.Swagger;
using PriceNegotiationApp.Validators;
using Swashbuckle.AspNetCore.SwaggerGen;

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
        ConfigureAuthentication(services, _configuration);
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
        services.AddScoped<IIdentityService, IdentityService>();
        services.AddScoped<INegotiationRepository, NegotiationRepository>();
        services.AddScoped<IPropositionRepository, PropositionRepository>();
        services.AddScoped<IValidator<UserRegisterDto>, UserRegisterModelValidator>();
        services.AddScoped<IValidator<Product>, ProductModelValidator>();
        services.AddSingleton<IPasswordHasher, PasswordHascher>();
    }

    private static void ConfigureAuthentication(IServiceCollection services, IConfiguration configuration)
    {
        var jwtSettings = configuration.GetSection("JwtSettings");

        var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]);

        services.AddAuthentication(x =>
        {
            x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(x =>
        {
            x.TokenValidationParameters = new TokenValidationParameters
            {
                ValidIssuer = jwtSettings["Issuer"],
                ValidAudience = jwtSettings["Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true
            };
        });

        services.AddAuthorization();
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
        services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, AppDbContext dbContext)
    {
        if (env.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        
        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseEndpoints(endpoint =>
        {
            endpoint.MapControllers();
        });
    }
}
