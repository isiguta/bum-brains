using BumBrains.Models.Configuration.Banking.Client;
using Microsoft.Extensions.Options;

namespace BumBrains.Services.Banking.Client;

public sealed class Plaid : BankingClient
{
    public Plaid(IOptions<PlaidConfiguration> configuration)
    {

    }
}
