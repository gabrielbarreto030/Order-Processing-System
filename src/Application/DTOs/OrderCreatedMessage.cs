namespace PortfolioFila.Application.DTOs;

public record OrderCreatedMessage(
    Guid OrderId,
    string CustomerName,
    string CustomerEmail,
    decimal Amount,
    DateTime CreatedAt,
    IReadOnlyCollection<OrderCreatedMessageItem> Items
);

public record OrderCreatedMessageItem(
    string ProductName,
    int Quantity,
    decimal UnitPrice,
    decimal TotalPrice
);
