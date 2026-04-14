using Microsoft.Extensions.Logging;
using PortfolioFila.Application.DTOs;
using PortfolioFila.Domain.Interfaces;

namespace PortfolioFila.Application.UseCases.GetOrders;

public class GetOrdersHandler
{
    private readonly IOrderRepository _orderRepository;
    private readonly ILogger<GetOrdersHandler> _logger;

    public GetOrdersHandler(IOrderRepository orderRepository, ILogger<GetOrdersHandler> logger)
    {
        _orderRepository = orderRepository;
        _logger = logger;
    }

    public async Task<IEnumerable<OrderResponse>> HandleAsync(GetOrdersQuery query, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Fetching all orders");

        var orders = await _orderRepository.GetAllAsync(cancellationToken);
        return orders.Select(o => o.ToResponse());
    }
}
