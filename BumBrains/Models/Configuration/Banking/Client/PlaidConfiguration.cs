namespace BumBrains.Models.Configuration.Banking.Client;

/// <summary>
/// Represents configuration of Plaid consolidated banking APIs.
/// </summary>
public class PlaidConfiguration
{
    /// <summary>
    /// Gets or sets the client id. Can be found on developer's dashboard.
    /// </summary>
    public string ClientId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the client secret. Can be found on developer's dashboard.
    /// </summary>
    public string ClientSecret { get; set; } = string.Empty;
}
