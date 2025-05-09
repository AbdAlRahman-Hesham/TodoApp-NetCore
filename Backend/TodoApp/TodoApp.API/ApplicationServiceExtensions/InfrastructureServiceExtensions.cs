using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TodoApp.API.Middleware;
using TodoApp.Application.Interfaces.Persistence;
using TodoApp.Application.Interfaces.Services;
using TodoApp.Domain.IdentityEntities;
using TodoApp.Infrastructure.EventHandlers;
using TodoApp.Infrastructure.Identity;
using TodoApp.Infrastructure.Persistence.Data;
using TodoApp.Infrastructure.Persistence.Repositories;
using TodoApp.Infrastructure.Services;

namespace TodoApp.Api.ServiceExtensions;

public static class InfrastructureServiceExtensions
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(config.GetConnectionString("DefaultConnection")));

        services.AddIdentity<ApplicationUser, IdentityRole>()
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
        services.AddCors();
        services.AddTransient<ExceptionMiddleware>();

        services.AddScoped<IDomainEventService, DomainEventService>();
        services.AddScoped<IEmailService, EmailService>();

        services.AddMediatR(cfg => {
            cfg.RegisterServicesFromAssembly(typeof(TodoCompletedEmailNotificationHandler).Assembly);
        });

        return services;
    }
}