using Dapper;
using Discount.Core.Entities;
using Discount.Core.Repositories;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace Discount.Infrastructure.Repositories
{
    public class DiscountRepository(IConfiguration _configuration) : IDiscountRepository
    {
        public async Task<Coupon> GetDiscount(string productName)
        {
            await using var connection = new NpgsqlConnection(_configuration.GetValue<string>("DatabaseSettings:ConnectionString"));
            // Implementation for fetching discount
            var coupoun = await connection.QueryFirstOrDefaultAsync<Coupon>("SELECT * FROM Coupon WHERE ProductName = @ProductName", 
                new { ProductName = productName });
            if(coupoun == null)
            {
                return new Coupon { ProductName = "No Discount", Amount = 0, Description = "No Discount Available" };
            }
            return coupoun;
        }

        public async Task<bool> CreateDiscount(Coupon coupon)
        {
            await using var connection = new NpgsqlConnection(_configuration.GetValue<string>("DatabaseSettings:ConnectionString"));
            var affected = await connection.ExecuteAsync("INSERT INTO Coupon (ProductName, Amount, Description) VALUES (@ProductName, @Amount, @Description)", 
                new { ProductName = coupon.ProductName, Amount = coupon.Amount, Description = coupon.Description });
           if(affected == 0)
           {
                return false;
           }
            return true;
        }

        public async Task<bool> UpdateDiscount(Coupon coupon)
        {
            await using var connection = new NpgsqlConnection(_configuration.GetValue<string>("DatabaseSettings:ConnectionString"));
            var affected = await connection.ExecuteAsync("UPDATE Coupon SET  ProductName = @ProductName,Amount = @Amount, Description = @Description WHERE Id = @Id", 
                new { ProductName = coupon.ProductName, Amount = coupon.Amount, Description = coupon.Description, Id = coupon.Id });
           if(affected == 0)
            {
                return false;
            }
            return true;
        }

        public async Task<bool> DeleteDiscount(string productName)
        {
            await using var connection = new NpgsqlConnection(_configuration.GetValue<string>("DatabaseSettings:ConnectionString"));
            var affected = await connection.ExecuteAsync("DELETE FROM Coupon WHERE ProductName = @ProductName", new { ProductName = productName });
            if(affected==0)
            {
                return false;
            }
            return true;
        }

    }
}
