using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Discount.Grpc.Extensions
{
    public static class HostExtensions
    {
        public static IHost MigrateDatabase<TContext>(this IHost host, int? retry = 0)
        {
            int retryForAvailability = retry.Value;

            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var configuration = services.GetRequiredService<IConfiguration>();
                var logger = services.GetRequiredService<ILogger<TContext>>();

                try
                {
                    logger.LogInformation("Migrating database postgres coupon");

                    using var connection = new NpgsqlConnection(configuration.GetValue<string>("ConnectionStrings:DatabaseSettings"));
                    connection.Open();

                    using var command = new NpgsqlCommand
                    {
                        Connection = connection
                    };

                    // Drop table if exists with same name
                    command.CommandText = "DROP TABLE IF EXISTS Coupon";
                    command.ExecuteNonQuery();

                    // Create table with following columns
                    command.CommandText = @"CREATE TABLE Coupon(Id SERIAL PRIMARY KEY,
                                                                CourseName VARCHAR(24) NOT NULL,
                                                                CourseDescription TEXT,
                                                                Price INT)";
                    command.ExecuteNonQuery();

                    // Inserting sample data into table
                    command.CommandText = "INSERT INTO Coupon(CourseName, CourseDescription, Price) VALUES ('DotNet', 'This is demo coupon description', 250);";
                    command.ExecuteNonQuery();
                    command.CommandText = "INSERT INTO Coupon(CourseName, CourseDescription, Price) VALUES ('SpringBoot', 'This is demo coupon description', 120);";
                    command.ExecuteNonQuery();

                    logger.LogInformation("Postgres coupon database migrated successfully!");
                }

                catch (NpgsqlException ex)
                {
                    logger.LogInformation(ex, "Error! Migrating database.");

                    if(retryForAvailability < 50)
                    {
                        retryForAvailability++;
                        System.Threading.Thread.Sleep(2000);
                        MigrateDatabase<TContext>(host, retryForAvailability);
                    }
                }
            }
            return host;  // what is host ??
        }
    }
}
