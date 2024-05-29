namespace BumBrains.Models.Requests.Link;

/// <summary>
/// Represents a model fo a request to <see cref="CreateLinkToken"/> function.
/// </summary>
public class CreateLinkTokenRequest
{
    /// <summary>
    /// Gets or sets a fix value for testing purposes.
    /// </summary>
    public bool Fix { get; set; }
}
