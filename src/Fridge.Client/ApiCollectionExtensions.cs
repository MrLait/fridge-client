using Fridge.Client.Api.Clients;

namespace Fridge.Client;

public static class ApiCollectionExtensions
{
    public static IServiceCollection AddFridgeApi(this IServiceCollection services, string baseUrl)
    {
        services.AddHttpClient<FridgesApi>(
            http => http.BaseAddress = new Uri(baseUrl!));
        services.AddHttpClient<MaintenanceApi>(
            http => http.BaseAddress = new Uri(baseUrl!));
        services.AddHttpClient<ProductsApi>(
            http => http.BaseAddress = new Uri(baseUrl!));

        return services;
    }
}