using MediatR;
using Microsoft.AspNetCore.Mvc;
using Ordering.Application.Commands;
using Ordering.Application.Queries;
using Ordering.Application.Responses;

namespace Ordering.API.Controllers
{
    public class OrdersController : BaseApiController
    {
        private readonly IMediator _mediator;
        private readonly ILogger<OrdersController> _logger;
        public OrdersController(IMediator mediator , ILogger<OrdersController> logger)
        {
            _logger = logger;
            _mediator = mediator;
        }

        // GetOrdersByUserName.
        [HttpGet("{userName}", Name = "GetOrdersByUserName")]
        [ProducesResponseType(typeof(IEnumerable<OrderResponse>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<OrderResponse>>> GerOrderByUserName(string userName)
            => Ok(await _mediator.Send(new GetOrderListQuery(userName)));

        // CheckOutOrder.
        [HttpPost(Name = "CheckoutOrder")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<int>> CheckoutOrder([FromBody] CheckoutOrderCommand command)
            => Ok(await _mediator.Send(command));

        // UpdateOrder.
        [HttpPut(Name = "UpdateOrder")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult<int>> UpdateOrder([FromBody] UpdateOrderCommand command)
        {
            var result = await _mediator.Send(command);
            return NoContent();
        }

        // DeleteOrder.
        [HttpDelete(Name = "DeleteOrder")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult> DeleteOrder([FromBody] DeleteOrderCommand command)
        {
            var result = await _mediator.Send(command);
            return NoContent();
        }

    }
}
