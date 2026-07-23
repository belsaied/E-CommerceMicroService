using Microsoft.Extensions.Logging;
using Ordering.Core.Entities;

namespace Ordering.Infrastructure.Data
{
    public class OrderContextSeed
    {
        public static async Task SeedAsync(OrderContext orderContext , ILogger<OrderContextSeed> logger)
        {
            if (!orderContext.Orders.Any())
            {
                orderContext.Orders.AddRange(GetOrders());
                await orderContext.SaveChangesAsync();
                logger.LogInformation($"Ordering database :{typeof(OrderContext).Name} seeded with data.");
            }
        }
        private static IEnumerable<Order> GetOrders()
        {
            return new List<Order>
            {
                new()
                {
                    UserName = "belal",
                    FirstName = "Belal",
                    LastName = "Saied",
                    EmailAddress = "bellllyelnagggar225@gmail.com",
                    AddressLine = "Cairo",
                    Country = "Egypt",
                    State = "EG",
                    TotalPrice = 750,
                    ZipCode = "71111",
                    CardName = "Visa",
                    CardNumber = "1234567890123456",
                    Expiration = "12/26",
                    Cvv = "123",
                    PaymentMethod = 1,
                    LastModifiedBy = "belal",
                    LastModifiedDate = new DateTime(),
                    CreatedBy = "belal",
                }
            };
        }
    }
}
