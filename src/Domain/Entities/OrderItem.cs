namespace PortfolioFila.Domain.Entities;

public class OrderItem
{
    public Guid Id { get; private set; }
    public Guid OrderId { get; private set; }
    public string ProductName { get; private set; } = string.Empty;
    public int Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }
    public decimal TotalPrice => Quantity * UnitPrice;

    private OrderItem() { }

    public OrderItem(string productName, int quantity, decimal unitPrice)
    {
        Id = Guid.NewGuid();
        ProductName = productName;
        Quantity = quantity;
        UnitPrice = unitPrice;
    }
}
