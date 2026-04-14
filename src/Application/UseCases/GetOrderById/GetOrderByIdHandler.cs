using Microsoft.Extensions.Logging;
using PortfolioFila.Application.DTOs;
using PortfolioFila.Domain.Interfaces;

namespace PortfolioFila.Application.UseCases.GetOrderById;

public class GetOrderByIdHandler
{
    private readonly IOrderRepository _orderRepository;
    private readonly ILogger<GetOrderByIdHandler> _logger;

    public GetOrderByIdHandler(IOrderRepository orderRepository, ILogger<GetOrderByIdHandler> logger)
    {
        _orderRepository = orderRepository;
        _logger = logger;
    }

    public async Task<OrderResponse?> HandleAsync(GetOrderByIdQuery query, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Fetching order {OrderId}", query.Id);

        var order = await _orderRepository.GetByIdAsync(query.Id, cancellationToken);

        if (order is null)
        {
            _logger.LogWarning("Order {OrderId} not found", query.Id);
            return null;
        }

        return order.ToResponse();
    }
}
