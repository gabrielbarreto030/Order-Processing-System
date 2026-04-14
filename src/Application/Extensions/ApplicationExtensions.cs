using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using PortfolioFila.Application.UseCases.CreateOrder;
using PortfolioFila.Application.UseCases.GetOrderById;
using PortfolioFila.Application.UseCases.GetOrders;

namespace PortfolioFila.Application.Extensions;

public static class ApplicationExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<CreateOrderHandler>();

        services.AddScoped<CreateOrderHandler>();
        services.AddScoped<GetOrderByIdHandler>();
        services.AddScoped<GetOrdersHandler>();

        return services;
    }
}
