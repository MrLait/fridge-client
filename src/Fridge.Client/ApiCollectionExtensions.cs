using Fridge.Client.Api;
using Fridge.Client.Api.Clients;

namespace Fridge.Client;

public static class ApiCollectionExtensions
{
    public static IServiceCollection AddFridgeApi(this IServiceCollection services, string baseUrl)
    {
        services.AddHttpContextAccessor();
        services.AddTransient<AuthCookieHandler>();

        services.AddHttpClient("FridgeApi", http =>
        {
            http.BaseAddress = new Uri(baseUrl);
        }).AddHttpMessageHandler<AuthCookieHandler>();

        services.AddHttpClient<AuthApi>("FridgeApi");
        services.AddHttpClient<FridgeModelsApi>("FridgeApi");
        services.AddHttpClient<FridgeProductApi>("FridgeApi");
        services.AddHttpClient<FridgesApi>("FridgeApi");
        services.AddHttpClient<MaintenanceApi>("FridgeApi");
        services.AddHttpClient<ProductsApi>("FridgeApi");
        services.AddHttpClient<ProductImagesApi>("FridgeApi");

        return services;
    }
}