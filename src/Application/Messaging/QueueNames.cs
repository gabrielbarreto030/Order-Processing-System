namespace PortfolioFila.Application.Messaging;

public static class QueueNames
{
    public const string OrderCreated = "order.created";
    public const string OrderCreatedDlq = "order.created.dlq";
    public const string OrderCreatedExchange = "order.created.exchange";
    public const string OrderCreatedDlqExchange = "order.created.dlq.exchange";
}
