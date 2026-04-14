using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using PortfolioFila.Application.DTOs;
using PortfolioFila.Application.Interfaces;
using PortfolioFila.Application.Messaging;
using PortfolioFila.Application.UseCases.CreateOrder;
using PortfolioFila.Domain.Entities;
using PortfolioFila.Domain.Enums;
using PortfolioFila.Domain.Interfaces;

namespace PortfolioFila.Application.Tests;

public class CreateOrderHandlerTests
{
    private readonly Mock<IOrderRepository> _repositoryMock;
    private readonly Mock<IMessagePublisher> _publisherMock;
    private readonly CreateOrderHandler _handler;

    public CreateOrderHandlerTests()
    {
        _repositoryMock = new Mock<IOrderRepository>();
        _publisherMock = new Mock<IMessagePublisher>();
        _handler = new CreateOrderHandler(
            _repositoryMock.Object,
            _publisherMock.Object,
            NullLogger<CreateOrderHandler>.Instance);
    }

    [Fact]
    public async Task HandleAsync_WithValidCommand_ShouldCreateOrderAndReturnResponse()
    {
        var command = new CreateOrderCommand(
            "John Doe",
            "john@example.com",
            new List<OrderItemRequest> { new("Product A", 2, 50.00m) });

        _repositoryMock
            .Setup(r => r.AddAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _publisherMock
            .Setup(p => p.PublishAsync(It.IsAny<OrderCreatedMessage>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var result = await _handler.HandleAsync(command);

        result.Should().NotBeNull();
        result.CustomerName.Should().Be("John Doe");
        result.CustomerEmail.Should().Be("john@example.com");
        result.Amount.Should().Be(100.00m);
        result.Status.Should().Be(OrderStatus.Pending);
        result.Id.Should().NotBe(Guid.Empty);
    }

    [Fact]
    public async Task HandleAsync_ShouldCallRepositoryAddAsync()
    {
        var command = new CreateOrderCommand(
            "Jane Doe",
            "jane@example.com",
            new List<OrderItemRequest> { new("Product B", 1, 25.00m) });

        _repositoryMock
            .Setup(r => r.AddAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _publisherMock
            .Setup(p => p.PublishAsync(It.IsAny<OrderCreatedMessage>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        await _handler.HandleAsync(command);

        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_ShouldPublishMessageToOrderCreatedQueue()
    {
        var command = new CreateOrderCommand(
            "Alice",
            "alice@example.com",
            new List<OrderItemRequest> { new("Item X", 1, 40.00m) });

        _repositoryMock
            .Setup(r => r.AddAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _publisherMock
            .Setup(p => p.PublishAsync(It.IsAny<OrderCreatedMessage>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        await _handler.HandleAsync(command);

        _publisherMock.Verify(
            p => p.PublishAsync(
                It.IsAny<OrderCreatedMessage>(),
                QueueNames.OrderCreated,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task HandleAsync_ShouldPublishMessageWithCorrectOrderData()
    {
        var command = new CreateOrderCommand(
            "Bob",
            "bob@example.com",
            new List<OrderItemRequest> { new("Gadget", 3, 20.00m) });

        _repositoryMock
            .Setup(r => r.AddAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        OrderCreatedMessage? capturedMessage = null;
        _publisherMock
            .Setup(p => p.PublishAsync(It.IsAny<OrderCreatedMessage>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Callback<OrderCreatedMessage, string, CancellationToken>((msg, _, _) => capturedMessage = msg)
            .Returns(Task.CompletedTask);

        await _handler.HandleAsync(command);

        capturedMessage.Should().NotBeNull();
        capturedMessage!.CustomerEmail.Should().Be("bob@example.com");
        capturedMessage.Amount.Should().Be(60.00m);
        capturedMessage.Items.Should().HaveCount(1);
        capturedMessage.Items.First().ProductName.Should().Be("Gadget");
    }

    [Fact]
    public async Task HandleAsync_WithMultipleItems_ShouldCalculateTotalAmount()
    {
        var command = new CreateOrderCommand(
            "Alice",
            "alice@example.com",
            new List<OrderItemRequest>
            {
                new("Item 1", 3, 10.00m),
                new("Item 2", 1, 20.00m)
            });

        _repositoryMock
            .Setup(r => r.AddAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _publisherMock
            .Setup(p => p.PublishAsync(It.IsAny<OrderCreatedMessage>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var result = await _handler.HandleAsync(command);

        result.Amount.Should().Be(50.00m);
        result.Items.Should().HaveCount(2);
    }

    [Fact]
    public async Task HandleAsync_WhenPublishFails_ShouldPropagateException()
    {
        var command = new CreateOrderCommand(
            "Fail User",
            "fail@example.com",
            new List<OrderItemRequest> { new("Item", 1, 10.00m) });

        _repositoryMock
            .Setup(r => r.AddAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _publisherMock
            .Setup(p => p.PublishAsync(It.IsAny<OrderCreatedMessage>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("RabbitMQ unavailable"));

        var act = async () => await _handler.HandleAsync(command);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("RabbitMQ unavailable");
    }
}
