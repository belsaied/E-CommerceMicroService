using AutoMapper;
using Basket.Application.Commands;
using Basket.Application.Responses;
using Basket.Core.Repositories;
using MediatR;

namespace Basket.Application.Handlers.Commands
{
    internal class CreateShoppingCartCommandHandler(IBasketRepository _basketRepository,IMapper _mapper) : IRequestHandler<CreateShoppingCartCommand, ShoppingCartResponse>
    {
        public async Task<ShoppingCartResponse> Handle(CreateShoppingCartCommand request, CancellationToken cancellationToken)
        {
            var shoppingCart = await _basketRepository.UpdateBasket(new Core.Entities.ShoppingCart()
            {
                UserName = request.UserName,
                Items = request.Items
            });
            return _mapper.Map<ShoppingCartResponse>(shoppingCart);
        }
    }
}
