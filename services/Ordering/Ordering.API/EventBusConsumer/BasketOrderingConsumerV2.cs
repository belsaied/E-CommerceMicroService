using AutoMapper;
using EventBus.Messages.Events;
using MassTransit;
using MediatR;
using Ordering.Application.Commands;

namespace Ordering.API.EventBusConsumer
{
    public class BasketOrderingConsumerV2 : IConsumer<BasketCheckoutEventV2>
    {
        private readonly ILogger<BasketOrderingConsumerV2> _logger;
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        public BasketOrderingConsumerV2(ILogger<BasketOrderingConsumerV2> logger, IMediator mediator, IMapper mapper)
        {
            _logger = logger;
            _mediator = mediator;
            _mapper = mapper;
        }
        public async Task Consume(ConsumeContext<BasketCheckoutEventV2> context)
        {
            using var scope = _logger.BeginScope("Consume BasketCheckoutEvent {correlationId} with V2", context.Message.CorrelationId);
            var cmd = _mapper.Map<CheckoutOrderCommandV2>(context.Message);
            await _mediator.Send(cmd);
            _logger.LogInformation("BasketCheckoutEvent consumed successfully {correlationId} with V2", context.Message.CorrelationId);
        }
    }
}
