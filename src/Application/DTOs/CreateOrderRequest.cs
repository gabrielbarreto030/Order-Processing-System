namespace PortfolioFila.Application.DTOs;

public record CreateOrderRequest(
    string CustomerName,
    string CustomerEmail,
    IList<OrderItemRequest> Items
);
