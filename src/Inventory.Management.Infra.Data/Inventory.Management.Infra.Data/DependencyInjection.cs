using Inventory.Management.Domain.Interfaces;
using Inventory.Management.Infra.Data.Context;
using Inventory.Management.Infra.Data.Repository;
using Inventory.Management.Infra.Data.UnitOfWork;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Inventory.Management.Infra.Data
{
    /// <summary>
    /// Configuração centralizada de persistência para InventoryDbContext, UnitOfWork e Repository usando SQLite em memória.
    /// </summary>
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfraDataSqliteInMemory(this IServiceCollection services)
        {
            // Inicializa as dependências nativas do SQLite
            SQLitePCL.Batteries_V2.Init();

            // Cria e mantém a conexão SQLite em memória viva
            var connection = new SqliteConnection("Data Source=:memory:");
            connection.Open();

            // Registra a conexão como singleton (reutilizada em todos os DbContexts)
            services.AddSingleton(connection);

            // Configura o DbContext usando a mesma conexão
            services.AddDbContext<InventoryDbContext>(options =>
                options.UseSqlite(connection),
                contextLifetime: ServiceLifetime.Scoped,
                optionsLifetime: ServiceLifetime.Singleton);

            // Registra UnitOfWork e Repository
            services.AddScoped<IUnitOfWork, UnitOfWork.UnitOfWork>();
            services.AddScoped<IInventoryRepository, InventoryRepository>();

            // Garante que o banco seja criado ao inicializar a aplicação
            services.AddHostedService<DbInitializerHostedService>();

            return services;
        }
    }

    // HostedService para garantir que o schema seja criado no startup
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


        ///// <summary>
        ///// Adiciona o InventoryDbContext com SQLite em memória, mantendo a conexão aberta durante o ciclo de vida da aplicação.
        ///// </summary>
        //public static IServiceCollection AddInfraDataSqliteInMemory(this IServiceCollection services)
        //{
        //    // Cria e mantém a conexão SQLite em memória viva
        //    var connection = new SqliteConnection("Data Source=:memory:");
        //        connection.Open();

        //    services.AddSingleton(connection);

        //    services.AddDbContext<InventoryDbContext>(options =>
        //        options.UseSqlite(connection));

        //    // Registra UnitOfWork e Repository
        //    services.AddScoped<IUnitOfWork, UnitOfWork.UnitOfWork>();
        //    services.AddScoped<IInventoryRepository, InventoryRepository>();

        //    // Garante que o banco seja criado ao inicializar a aplicação
        //    services.AddSingleton<IStartupFilter, InventoryDbStartupFilter>();


        //    return services;
        //}

        ///// <summary>
        ///// StartupFilter para garantir que o banco em memória seja criado antes do uso.
        ///// </summary>
        //private class InventoryDbStartupFilter : IStartupFilter
        //{
        //    public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
        //    {
        //        return app =>
        //        {
        //            using var scope = app.ApplicationServices.CreateScope();
        //            var context = scope.ServiceProvider.GetRequiredService<InventoryDbContext>();
        //            context.Database.EnsureCreated();
        //            next(app);
        //        };
        //    }
        //}
    }
}
