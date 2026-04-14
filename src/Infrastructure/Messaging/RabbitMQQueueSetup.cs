using Microsoft.Extensions.Logging;
using PortfolioFila.Application.Messaging;
using RabbitMQ.Client;

namespace PortfolioFila.Infrastructure.Messaging;

public class RabbitMQQueueSetup
{
    private readonly RabbitMQConnectionFactory _connectionFactory;
    private readonly ILogger<RabbitMQQueueSetup> _logger;

    public RabbitMQQueueSetup(RabbitMQConnectionFactory connectionFactory, ILogger<RabbitMQQueueSetup> logger)
    {
        _connectionFactory = connectionFactory;
        _logger = logger;
    }

    public void EnsureQueuesExist()
    {
        using var channel = _connectionFactory.GetConnection().CreateModel();

        // DLQ exchange and queue (declared first so main queue can reference it)
        channel.ExchangeDeclare(
            exchange: QueueNames.OrderCreatedDlqExchange,
            type: ExchangeType.Direct,
            durable: true,
            autoDelete: false);

        channel.QueueDeclare(
            queue: QueueNames.OrderCreatedDlq,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null);

        channel.QueueBind(
            queue: QueueNames.OrderCreatedDlq,
            exchange: QueueNames.OrderCreatedDlqExchange,
            routingKey: QueueNames.OrderCreatedDlq);

        // Main exchange and queue with DLQ configured
        channel.ExchangeDeclare(
            exchange: QueueNames.OrderCreatedExchange,
            type: ExchangeType.Direct,
            durable: true,
            autoDelete: false);

        var queueArgs = new Dictionary<string, object>
        {
            { "x-dead-letter-exchange", QueueNames.OrderCreatedDlqExchange },
            { "x-dead-letter-routing-key", QueueNames.OrderCreatedDlq }
        };

        channel.QueueDeclare(
            queue: QueueNames.OrderCreated,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: queueArgs);

        channel.QueueBind(
            queue: QueueNames.OrderCreated,
            exchange: QueueNames.OrderCreatedExchange,
            routingKey: QueueNames.OrderCreated);

        _logger.LogInformation(
            "RabbitMQ queues declared: {MainQueue} → DLQ: {DlqQueue}",
            QueueNames.OrderCreated,
            QueueNames.OrderCreatedDlq);
    }
}
