using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PortfolioFila.Application.Interfaces;
using PortfolioFila.Domain.Interfaces;
using PortfolioFila.Infrastructure.Messaging;
using PortfolioFila.Infrastructure.Persistence;
using PortfolioFila.Infrastructure.Persistence.Repositories;

namespace PortfolioFila.Infrastructure.Extensions;

public static class InfrastructureExtensions
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Database
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName)));

        services.AddScoped<IOrderRepository, OrderRepository>();

        // RabbitMQ
        services.Configure<RabbitMQSettings>(opts =>
            configuration.GetSection(RabbitMQSettings.SectionName).Bind(opts));
        services.AddSingleton<RabbitMQConnectionFactory>();
        services.AddSingleton<RabbitMQQueueSetup>();
        services.AddScoped<IMessagePublisher, RabbitMQPublisher>();

        return services;
    }
}
