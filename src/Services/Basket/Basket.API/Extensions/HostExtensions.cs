using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace Basket.API.Extensions
{
    public static class HostExtensions
    {
        public static IHost MigrateDatabase<TContext>(this IHost host, int? retry = 0)
        {
            int retryForAvailability = retry.Value;


            using(var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var configuration = services.GetRequiredService<IConfiguration>();
                var logger = services.GetRequiredService<ILogger<TContext>>();

                try
                {
                    logger.LogInformation("Migrating postgres database for basket");
                    
                    // Connection
                    using var connection = new NpgsqlConnection(configuration.GetValue<string>("DatabaseSettings:ConnectionString"));
                    connection.Open();

                    
                    // Command
                    using var command = new NpgsqlCommand()
                    {
                        Connection = connection
                    };

                    
                    // Operations -> Drop table if exists with name Coupon
                    command.CommandText = "DROP TABLE IF EXISTS Basket";
                    command.ExecuteNonQuery();

                    
                    // Create Table
                    command.CommandText = @"CREATE TABLE Basket(Id SERIAL PRIMARY KEY, UserName VARCHAR(24), CourseName TEXT, CourseDescription TEXT, Price INT, Quantity INT)";
                    command.ExecuteNonQuery();


                    // Seeding Data
                    command.CommandText = "INSERT INTO Basket(UserName, CourseName, CourseDescription, Price, Quantity) VALUES ('swn', 'DotNet', 'Sample description', 3500, 2);";
                    command.ExecuteNonQuery();
                    command.CommandText = "INSERT INTO Basket(UserName, CourseName, CourseDescription, Price, Quantity) VALUES ('swn', 'SpringBoot', 'Sample description', 2000, 1);";
                    command.ExecuteNonQuery();

                    logger.LogInformation("Database migrated successfully");
                }
                catch (NpgsqlException ex)
                {
                    logger.LogError(ex, "Error! migrating postgres Basket database");

                    if(retryForAvailability < 50)
                    {
                        retryForAvailability++;
                        System.Threading.Thread.Sleep(2000);
                        MigrateDatabase<TContext>(host, retryForAvailability);
                    }
                }
            }
            return host;
        }
    }
}
