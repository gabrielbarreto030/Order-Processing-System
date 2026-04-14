namespace PortfolioFila.Application.DTOs;

public record OrderItemResponse(
    Guid Id,
    string ProductName,
    int Quantity,
    decimal UnitPrice,
    decimal TotalPrice
);
