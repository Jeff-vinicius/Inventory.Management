using Inventory.Management.API.Infrastructure;

namespace Inventory.Management.API
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddPresentation(this IServiceCollection services)
        {
            //TODO: trazer tudo relacionado a API para cá

            services.AddExceptionHandler<GlobalExceptionHandler>();

            return services;
        }
    }
}
