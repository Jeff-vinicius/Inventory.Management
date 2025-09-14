using Inventory.Management.Domain.Interfaces;
using Inventory.Management.Infra.Data.Context;
using Inventory.Management.Infra.Data.Repository;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Inventory.Management.Infra.Data
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfraDataSqliteInMemory(this IServiceCollection services)
        {
            SQLitePCL.Batteries_V2.Init();

            var connection = new SqliteConnection("Data Source=:memory:");
            connection.Open();

            services.AddSingleton(connection);

            services.AddDbContext<InventoryDbContext>(options =>
                options.UseSqlite(connection),
                contextLifetime: ServiceLifetime.Scoped,
                optionsLifetime: ServiceLifetime.Singleton);

            services.AddScoped<IUnitOfWork, UnitOfWork.UnitOfWork>();
            services.AddScoped<IInventoryRepository, InventoryRepository>();

            services.AddHostedService<DbInitializerHostedService>();

            services.AddHealthChecks()
                .AddSqlite(connection.ConnectionString, name: "sqlite-inmemory");

            return services;
        }
    }

    public class DbInitializerHostedService : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;

        public DbInitializerHostedService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<InventoryDbContext>();
            await context.Database.EnsureCreatedAsync(cancellationToken);
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
