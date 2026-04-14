using PortfolioFila.Application.DTOs;

namespace PortfolioFila.Application.UseCases.CreateOrder;

public record CreateOrderCommand(
    string CustomerName,
    string CustomerEmail,
    IList<OrderItemRequest> Items
);
