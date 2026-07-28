using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Polly;

namespace Ordering.API.Extensions
{
    public static class DbExtension
    {
        public static IHost MigrateDatabase<TContext>(this IHost host , Action<TContext,IServiceProvider> seeder) where TContext : DbContext
        {
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var logger = services.GetRequiredService<ILogger<TContext>>();
                var context = services.GetService<TContext>();

                try
                {
                    logger.LogInformation($"Started db migration: {typeof(TContext).Name}");

                    var retry = Policy.Handle<SqlException>()
                        .WaitAndRetry(
                            retryCount: 5,
                            sleepDurationProvider: retryAttempts =>
                                TimeSpan.FromSeconds(Math.Pow(2, retryAttempts)),
                            onRetry: (exception, span, count) =>
                            {
                                logger.LogInformation(
                                    $"Retrying because of {exception} {span}");
                            });

                    retry.Execute(() => CallSeeder(seeder, context, services));
                    logger.LogInformation($"finished db Migration {typeof(TContext).Name}");
                }
                catch (Exception ex)
                {
                    logger.LogInformation($"An error occured while Migrating DB {typeof(TContext).Name}");
                }
            }
            return host;
        }

        private static void CallSeeder<TContext>(Action<TContext,IServiceProvider> seeder , TContext? context , IServiceProvider serviceProvider)
            where TContext : DbContext
        {
            context.Database.Migrate();
            seeder(context,serviceProvider);
        }
            
    }
}
