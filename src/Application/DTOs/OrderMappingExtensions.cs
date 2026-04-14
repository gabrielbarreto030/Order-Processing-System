using PortfolioFila.Domain.Entities;

namespace PortfolioFila.Application.DTOs;

public static class OrderMappingExtensions
{
    public static OrderResponse ToResponse(this Order order)
    {
        var items = order.Items
            .Select(i => new OrderItemResponse(i.Id, i.ProductName, i.Quantity, i.UnitPrice, i.TotalPrice))
            .ToList()
            .AsReadOnly();

        return new OrderResponse(
            order.Id,
            order.CustomerName,
            order.CustomerEmail,
            order.Amount,
            order.Status,
            order.Status.ToString(),
            order.CreatedAt,
            order.UpdatedAt,
            items
        );
    }
}
