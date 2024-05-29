using BumBrains.Functions.Link;

namespace Microsoft.Extensions.DependencyInjection;

public static class LinkServiceCollectionExtensions
{
    public static IServiceCollection AddLink(this IServiceCollection services)
    {
        services.AddHttpClient<LinkFunctions>(client =>
        {
            client.BaseAddress = new Uri("https://developer.bankofamerica.com");
        });

        return services;
    }
}
