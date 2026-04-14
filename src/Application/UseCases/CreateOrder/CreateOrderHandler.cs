using Microsoft.Extensions.Logging;
using PortfolioFila.Application.DTOs;
using PortfolioFila.Application.Interfaces;
using PortfolioFila.Application.Messaging;
using PortfolioFila.Domain.Entities;
using PortfolioFila.Domain.Interfaces;

namespace PortfolioFila.Application.UseCases.CreateOrder;

public class CreateOrderHandler
{
    private readonly IOrderRepository _orderRepository;
    private readonly IMessagePublisher _publisher;
    private readonly ILogger<CreateOrderHandler> _logger;

    public CreateOrderHandler(
        IOrderRepository orderRepository,
        IMessagePublisher publisher,
        ILogger<CreateOrderHandler> logger)
    {
        _orderRepository = orderRepository;
        _publisher = publisher;
        _logger = logger;
    }

    public async Task<OrderResponse> HandleAsync(CreateOrderCommand command, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Creating order for customer {CustomerEmail}", command.CustomerEmail);

        var items = command.Items.Select(i => new OrderItem(i.ProductName, i.Quantity, i.UnitPrice));
        var order = new Order(command.CustomerName, command.CustomerEmail, items);

        await _orderRepository.AddAsync(order, cancellationToken);

        _logger.LogInformation("Order {OrderId} saved. Publishing to queue {Queue}", order.Id, QueueNames.OrderCreated);

        var message = new OrderCreatedMessage(
            order.Id,
            order.CustomerName,
            order.CustomerEmail,
            order.Amount,
            order.CreatedAt,
            order.Items.Select(i => new OrderCreatedMessageItem(i.ProductName, i.Quantity, i.UnitPrice, i.TotalPrice))
                       .ToList()
                       .AsReadOnly()
        );

        await _publisher.PublishAsync(message, QueueNames.OrderCreated, cancellationToken);

        _logger.LogInformation("Order {OrderId} published successfully", order.Id);

        return order.ToResponse();
    }
}
