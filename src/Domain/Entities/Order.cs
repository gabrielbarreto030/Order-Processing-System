using PortfolioFila.Domain.Enums;

namespace PortfolioFila.Domain.Entities;

public class Order
{
    public Guid Id { get; private set; }
    public string CustomerName { get; private set; } = string.Empty;
    public string CustomerEmail { get; private set; } = string.Empty;
    public decimal Amount { get; private set; }
    public OrderStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();

    private readonly List<OrderItem> _items = new();

    private Order() { }

    public Order(string customerName, string customerEmail, IEnumerable<OrderItem> items)
    {
        Id = Guid.NewGuid();
        CustomerName = customerName;
        CustomerEmail = customerEmail;
        Status = OrderStatus.Pending;
        CreatedAt = DateTime.UtcNow;
        _items.AddRange(items);
        Amount = _items.Sum(i => i.TotalPrice);
    }

    public void SetStatus(OrderStatus status)
    {
        Status = status;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkAsProcessing() => SetStatus(OrderStatus.Processing);
    public void MarkAsCompleted() => SetStatus(OrderStatus.Completed);
    public void MarkAsFailed() => SetStatus(OrderStatus.Failed);
}
