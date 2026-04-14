using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using PortfolioFila.Application.DTOs;
using PortfolioFila.Application.UseCases.CreateOrder;
using PortfolioFila.Application.UseCases.GetOrderById;
using PortfolioFila.Application.UseCases.GetOrders;

namespace PortfolioFila.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class OrdersController : ControllerBase
{
    private readonly CreateOrderHandler _createOrderHandler;
    private readonly GetOrdersHandler _getOrdersHandler;
    private readonly GetOrderByIdHandler _getOrderByIdHandler;
    private readonly IValidator<CreateOrderRequest> _createOrderValidator;
    private readonly ILogger<OrdersController> _logger;

    public OrdersController(
        CreateOrderHandler createOrderHandler,
        GetOrdersHandler getOrdersHandler,
        GetOrderByIdHandler getOrderByIdHandler,
        IValidator<CreateOrderRequest> createOrderValidator,
        ILogger<OrdersController> logger)
    {
        _createOrderHandler = createOrderHandler;
        _getOrdersHandler = getOrdersHandler;
        _getOrderByIdHandler = getOrderByIdHandler;
        _createOrderValidator = createOrderValidator;
        _logger = logger;
    }

    /// <summary>Creates a new order.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(OrderResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateOrder(
        [FromBody] CreateOrderRequest request,
        CancellationToken cancellationToken)
    {
        var validation = await _createOrderValidator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            _logger.LogWarning("Create order validation failed: {Errors}", validation.ToString());
            return ValidationProblem(new ValidationProblemDetails(
                validation.Errors.GroupBy(e => e.PropertyName)
                    .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray())));
        }

        var command = new CreateOrderCommand(request.CustomerName, request.CustomerEmail, request.Items);
        var response = await _createOrderHandler.HandleAsync(command, cancellationToken);

        return CreatedAtAction(nameof(GetOrderById), new { id = response.Id }, response);
    }

    /// <summary>Returns all orders.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<OrderResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetOrders(CancellationToken cancellationToken)
    {
        var orders = await _getOrdersHandler.HandleAsync(new GetOrdersQuery(), cancellationToken);
        return Ok(orders);
    }

    /// <summary>Returns an order by ID.</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(OrderResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetOrderById(Guid id, CancellationToken cancellationToken)
    {
        var order = await _getOrderByIdHandler.HandleAsync(new GetOrderByIdQuery(id), cancellationToken);

        if (order is null)
            return NotFound(new { message = $"Order {id} not found." });

        return Ok(order);
    }
}
