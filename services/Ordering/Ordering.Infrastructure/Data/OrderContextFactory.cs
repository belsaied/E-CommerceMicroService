using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Ordering.Infrastructure.Data
{
    public class OrderContextFactory : IDesignTimeDbContextFactory<OrderContext>
    {
        public OrderContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<OrderContext>();
            optionsBuilder.UseSqlServer("Server=localhost;Database=OrderDb2;User Id=sa;Password=P@ssw0rd123;TrustServerCertificate=True;");
            return new OrderContext(optionsBuilder.Options);
        }
    }
}