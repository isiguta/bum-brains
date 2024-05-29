using BumBrains.Models.Configuration.Banking.Plaid.Client;
using BumBrains.Services.Banking.Client;
using Going.Plaid;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BumBrains.Extensions.ServiceCollection;

public static class BankingClientServiceCollectionExtensions
{
    public static void AddBankingClient(this IServiceCollection services, IConfiguration configuration)
    {
        
        configuration.GetSection(PlaidOptions.SectionKey).Get<PlaidCredentials>();
        services.Configure<PlaidCredentials>(configuration.GetSection(key: PlaidOptions.SectionKey));
        services.AddSingleton<PlaidClient>();
        services.AddSingleton<IBankingProvider, Plaid>();
    }
}
