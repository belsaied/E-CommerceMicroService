using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace Discount.Infrastructure.Extensions
{
    public static class DbExtension 
    {
        public static IHost MigrateDatabase<TContext>(this IHost host)
        {
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var configuration = services.GetRequiredService<IConfiguration>();
                var logger = services.GetRequiredService<ILogger<TContext>>();
                try
                {
                    logger.LogInformation("Discount Database Migration started.");
                    ApplyMigrations(configuration);
                    logger.LogInformation("Discount Database Migration completed.");
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "An error occurred while migrating the discount database");
                    throw;
                }
            }
            return host;
        }

        private static void ApplyMigrations(IConfiguration configuration)
        {
            var retry = 5;
            while (retry > 0)
            {
                try
                {
                    using var connection = new NpgsqlConnection(configuration.GetValue<string>("DatabaseSettings:ConnectionString"));
                    connection.Open();
                    using var command = new NpgsqlCommand
                    {
                        Connection = connection
                    };
                    command.CommandText = "DROP TABLE IF EXISTS Coupon";
                    command.ExecuteNonQuery();
                    command.CommandText = @"CREATE TABLE IF NOT EXISTS Coupon(Id SERIAL PRIMARY KEY, ProductName VARCHAR(50) NOT NULL, Description TEXT, Amount INT)";
                    command.ExecuteNonQuery();
                    command.CommandText = "INSERT INTO Coupon(ProductName, Description, Amount) VALUES('Egypt Adidas Quick Force Indoor Badminton Shoes', 'Adidas Discount', 600) ON CONFLICT (ProductName) DO NOTHING";
                    command.ExecuteNonQuery();
                    command.CommandText = "INSERT INTO Coupon(ProductName, Description, Amount) VALUES('PowerFit 19 FH Rubber Spike Cricket Shoes', 'PowerFit Discount', 800) ON CONFLICT (ProductName) DO NOTHING";
                    command.ExecuteNonQuery();
                    break; // Exit the loop if successful
                }
                catch (NpgsqlException ex)
                {
                    retry--;
                    if (retry == 0)
                    {
                        throw; // Rethrow the exception if no retries left
                    }
                    Thread.Sleep(2000); // Wait for 2 seconds before retrying
                } 
            }
        }
    }
}
