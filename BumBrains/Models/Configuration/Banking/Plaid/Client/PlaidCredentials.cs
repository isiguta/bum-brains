namespace BumBrains.Models.Configuration.Banking.Plaid.Client;

/// <summary>
/// Represents configuration of Plaid consolidated banking APIs.
/// </summary>
public class PlaidCredentials
{
    /// <summary>
    /// Gets or sets the client id. Can be found on developer's dashboard.
    /// </summary>
    public string ClientId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the client secret. Can be found on developer's dashboard.
    /// </summary>
    public string ClientSecret { get; set; } = string.Empty;

    /// <summary>
    /// 
    /// </summary>
    public string LinkToken { get; set; } = string.Empty;

    /// <summary>
    /// 
    /// </summary>
    public string AccessToken { get; set; } = string.Empty;

    /// <summary>
    /// 
    /// </summary>
    public string ItemId { get; set; } = string.Empty;
}
