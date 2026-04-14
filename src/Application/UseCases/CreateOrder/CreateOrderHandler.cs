using Microsoft.Extensions.Logging;
using PortfolioFila.Application.DTOs;
using PortfolioFila.Domain.Entities;
using PortfolioFila.Domain.Interfaces;

namespace PortfolioFila.Application.UseCases.CreateOrder;

public class CreateOrderHandler
{
    private readonly IOrderRepository _orderRepository;
    private readonly ILogger<CreateOrderHandler> _logger;

    public CreateOrderHandler(IOrderRepository orderRepository, ILogger<CreateOrderHandler> logger)
    {
        _orderRepository = orderRepository;
        _logger = logger;
    }

    public async Task<OrderResponse> HandleAsync(CreateOrderCommand command, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Creating order for customer {CustomerEmail}", command.CustomerEmail);

        var items = command.Items.Select(i => new OrderItem(i.ProductName, i.Quantity, i.UnitPrice));
        var order = new Order(command.CustomerName, command.CustomerEmail, items);

        await _orderRepository.AddAsync(order, cancellationToken);

        _logger.LogInformation("Order {OrderId} created successfully with status {Status}", order.Id, order.Status);

        return order.ToResponse();
    }
}
