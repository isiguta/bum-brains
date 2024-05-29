using BumBrains.Models.Configuration.Banking.Plaid.Client;
using Going.Plaid;
using Microsoft.Extensions.Options;

namespace BumBrains.Services.Banking.Client;

public sealed class Plaid : IBankingProvider
{
    public Plaid(PlaidClient client, IOptions<PlaidCredentials> configuration)
    {

    }
}
