using BumBrains.Models.Configuration.Banking.Client;
using BumBrains.Services.Banking.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BumBrains.Extensions.ServiceCollection;

public static class BankingClientServiceCollectionExtensions
{
    public static void AddBankingClient(this IServiceCollection services, IConfiguration configuration)
    {
        var plaidConfiguration = configuration.GetSection(nameof(PlaidConfiguration)).Get<PlaidConfiguration>();
        services.Configure<PlaidConfiguration>(configuration.GetSection(key: nameof(PlaidConfiguration)));
        services.AddSingleton<Plaid>();
    }
}
