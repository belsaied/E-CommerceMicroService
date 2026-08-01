using AutoMapper;
using EventBus.Messages.Events;
using MassTransit;
using MediatR;
using Ordering.Application.Commands;

namespace Ordering.API.EventBusConsumer
{
    public class BasketOrderingConsumer : IConsumer<BasketCheckoutEvent>
    {
        private readonly ILogger<BasketOrderingConsumer> _logger;
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        public BasketOrderingConsumer(ILogger<BasketOrderingConsumer> logger, IMediator mediator, IMapper mapper)
        {
            _logger = logger;
            _mediator = mediator;
            _mapper = mapper;
        }
        public async Task Consume(ConsumeContext<BasketCheckoutEvent> context)
        {
            using var scope = _logger.BeginScope("Consume BasketCheckoutEvent {correlationId}" , context.Message.CorrelationId);
            var cmd = _mapper.Map<CheckoutOrderCommand>(context.Message);
            await _mediator.Send(cmd);
            _logger.LogInformation("BasketCheckoutEvent consumed successfully {correlationId}", context.Message.CorrelationId);
        }
    }
}
