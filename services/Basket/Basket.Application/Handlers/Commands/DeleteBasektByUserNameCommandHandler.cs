using Basket.Application.Commands;
using Basket.Core.Repositories;
using MediatR;

namespace Basket.Application.Handlers.Commands
{
    public class DeleteBasektByUserNameCommandHandler(IBasketRepository _basketRepository) : IRequestHandler<DeleteBasketByUserNameCommand, Unit>
    {
        public async Task<Unit> Handle(DeleteBasketByUserNameCommand request, CancellationToken cancellationToken)
        {
            await _basketRepository.DeleteBasket(request.UserName);
            return Unit.Value;
        }
    }
}
