using FluentAssertions;
using PortfolioFila.Domain.Entities;
using PortfolioFila.Domain.Enums;

namespace PortfolioFila.Domain.Tests;

public class OrderTests
{
    private static List<OrderItem> CreateItems(int count = 1)
        => Enumerable.Range(1, count)
            .Select(i => new OrderItem($"Product {i}", i, 10.00m * i))
            .ToList();

    [Fact]
    public void NewOrder_ShouldHavePendingStatus()
    {
        var order = new Order("John Doe", "john@example.com", CreateItems());
        order.Status.Should().Be(OrderStatus.Pending);
    }

    [Fact]
    public void NewOrder_ShouldHaveGeneratedId()
    {
        var order = new Order("John Doe", "john@example.com", CreateItems());
        order.Id.Should().NotBe(Guid.Empty);
    }

    [Fact]
    public void NewOrder_ShouldCalculateAmountFromItems()
    {
        var items = new List<OrderItem>
        {
            new("Widget A", 2, 15.00m),
            new("Widget B", 1, 30.00m)
        };

        var order = new Order("Jane Doe", "jane@example.com", items);

        order.Amount.Should().Be(60.00m);
    }

    [Fact]
    public void MarkAsProcessing_ShouldUpdateStatusToProcessing()
    {
        var order = new Order("John Doe", "john@example.com", CreateItems());
        order.MarkAsProcessing();
        order.Status.Should().Be(OrderStatus.Processing);
    }

    [Fact]
    public void MarkAsCompleted_ShouldUpdateStatusToCompleted()
    {
        var order = new Order("John Doe", "john@example.com", CreateItems());
        order.MarkAsCompleted();
        order.Status.Should().Be(OrderStatus.Completed);
    }

    [Fact]
    public void MarkAsFailed_ShouldUpdateStatusToFailed()
    {
        var order = new Order("John Doe", "john@example.com", CreateItems());
        order.MarkAsFailed();
        order.Status.Should().Be(OrderStatus.Failed);
    }

    [Fact]
    public void SetStatus_ShouldUpdateUpdatedAt()
    {
        var order = new Order("John Doe", "john@example.com", CreateItems());
        order.UpdatedAt.Should().BeNull();

        order.MarkAsProcessing();

        order.UpdatedAt.Should().NotBeNull();
        order.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void NewOrder_ShouldContainExpectedItems()
    {
        var items = CreateItems(3);
        var order = new Order("John Doe", "john@example.com", items);

        order.Items.Should().HaveCount(3);
    }

    [Fact]
    public void OrderItem_TotalPrice_ShouldBeQuantityTimesUnitPrice()
    {
        var item = new OrderItem("Test Product", 4, 25.00m);
        item.TotalPrice.Should().Be(100.00m);
    }
}
