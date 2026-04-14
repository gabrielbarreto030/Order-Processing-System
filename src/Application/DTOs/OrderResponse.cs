using PortfolioFila.Domain.Enums;

namespace PortfolioFila.Application.DTOs;

public record OrderResponse(
    Guid Id,
    string CustomerName,
    string CustomerEmail,
    decimal Amount,
    OrderStatus Status,
    string StatusDescription,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    IReadOnlyCollection<OrderItemResponse> Items
);
