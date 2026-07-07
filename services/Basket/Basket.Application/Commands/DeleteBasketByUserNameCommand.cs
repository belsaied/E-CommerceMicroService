using MediatR;

namespace Basket.Application.Commands
{
    // Unit is a struct that equivelent to return void.
    public class DeleteBasketByUserNameCommand : IRequest<Unit>
    {
        public string UserName { get; set; }
        public DeleteBasketByUserNameCommand(string userName)
        {
            UserName = userName;
        }
    }
}
