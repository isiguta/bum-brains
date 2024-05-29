using BumBrains.Models.Configuration.Banking.Plaid.Client;
using BumBrains.Models.Configuration.Banking.Plaid.Link;
using BumBrains.Models.Requests.Link;
using Going.Plaid;
using Going.Plaid.Entity;
using Going.Plaid.Item;
using Going.Plaid.Link;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net;
using System.Text.Json;

namespace BumBrains.Functions.Link;

/// <summary>
/// Class that contains functions related to Link part of authentication with Plaid.
/// </summary>
public sealed class LinkFunctions
{
    private readonly PlaidClient plaid;
    private readonly PlaidCredentials credentials;
    private readonly ILogger logger;

    /// <summary>
    /// Creates an instance of the <see cref="LinkFunctions"/> class.
    /// </summary>
    /// <param name="plaid">Consolidated banking API client.</param>
    /// <param name="loggerFactory">Logger provider.</param>
    public LinkFunctions(PlaidClient plaid, IOptions<PlaidCredentials> credentials, ILoggerFactory loggerFactory)
    {
        ArgumentNullException.ThrowIfNull(plaid);
        ArgumentNullException.ThrowIfNull(credentials.Value);
        ArgumentNullException.ThrowIfNull(loggerFactory);

        this.plaid = plaid;
        this.credentials = credentials.Value;
        this.logger = loggerFactory.CreateLogger<LinkFunctions>();
    }

    /// <summary>
    /// Function that creates a Link token for a further exchange for a public token.
    /// </summary>
    /// <param name="req">Incoming request data.</param>
    /// <returns>Function-specific response data.</returns>
    [Function(nameof(CreateLinkToken))]
    public async Task<HttpResponseData> CreateLinkToken(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestData req)
    {
        this.logger.LogInformation("Creating a Link token...");
        
        try
        {

            byte[] buffer = new byte[req.Body.Length];
            await req.Body.ReadAsync(buffer);
            using Stream stream = new MemoryStream(buffer);
            var functionRequest = await JsonSerializer.DeserializeAsync<CreateLinkTokenRequest>(stream);

            if (functionRequest == null)
            {
                var badRequestError = req.CreateResponse();
                badRequestError.StatusCode = HttpStatusCode.BadRequest;
                await badRequestError.WriteStringAsync($"Bad request to {nameof(CreateLinkToken)} function");
                return badRequestError;
            }

            LinkTokenCreateRequest plaidRequest = new()
            {
                AccessToken = functionRequest.Fix == true ? this.credentials.AccessToken : null,
                User = new LinkTokenCreateRequestUser { ClientUserId = Guid.NewGuid().ToString(), },
                ClientName = "Quickstart for .NET",
                Products = functionRequest.Fix != true ? this.credentials!.Products!.Split(',').Select(p => Enum.Parse<Products>(p, true)).ToArray() : Array.Empty<Products>(),
                Language = Language.English,
                CountryCodes = this.credentials!.CountryCodes!.Split(',').Select(p => Enum.Parse<CountryCode>(p, true)).ToArray(),
            };

            var plaidResponse = await this.plaid.LinkTokenCreateAsync(plaidRequest);

            if (plaidResponse.Error is not null)
            {
                var plaidError = await this.CreateErrorResponseAsync(req, HttpStatusCode.BadRequest, plaidResponse.Error.ErrorMessage);
                return plaidError;
            }

            var functionResponse = req.CreateResponse(HttpStatusCode.OK);
            functionResponse.Headers.Add("Content-Type", "application/json; charset=utf-8");
            using MemoryStream functionResponseStream = new();
            await JsonSerializer.SerializeAsync(functionResponseStream, plaidResponse.LinkToken);
            functionResponse.WriteBytes(functionResponseStream.GetBuffer());

            return functionResponse;
        }
        catch (Exception e)
        {
            var error = await this.CreateErrorResponseAsync(req, HttpStatusCode.InternalServerError, e.Message);
            return error;
        }
        
    }

    /// <summary>
    /// Function that exchanges a link token for an access token, used in subsequent API requests.
    /// </summary>
    /// <returns>Function-specific response data.</returns>
    [Function(nameof(ExchangePublicToken))]
    public async Task<HttpResponseData> ExchangePublicToken(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestData req)
    {
        this.logger.LogInformation("Exchanging public token for access token...");

        try
        {
            byte[] buffer = new byte[req.Body.Length];
            await req.Body.ReadAsync(buffer);
            using Stream stream = new MemoryStream(buffer);
            var functionRequest = await JsonSerializer.DeserializeAsync<LinkResult>(stream);

            if (functionRequest == null)
            {
                var badRequestError = await this.CreateErrorResponseAsync(req, HttpStatusCode.BadRequest, $"Bad request to {nameof(ExchangePublicToken)} function");
                return badRequestError;
            }

            ItemPublicTokenExchangeRequest request = new()
            {
                PublicToken = functionRequest.public_token
            };

            var plaidResponse = await this.plaid.ItemPublicTokenExchangeAsync(request);

            if (plaidResponse.Error is not null)
            {
                var plaidError = await this.CreateErrorResponseAsync(req, HttpStatusCode.BadRequest, plaidResponse.Error.ErrorMessage);
                return plaidError;
            }

            this.credentials.AccessToken = plaidResponse.AccessToken;
            this.credentials.ItemId = plaidResponse.ItemId;

            this.logger.LogInformation("Successfully retrieved access token and itemId!");
            
            var functionResponse = req.CreateResponse();
            using MemoryStream functionResponseStream = new();
            await JsonSerializer.SerializeAsync(functionResponseStream, this.credentials);
            functionResponse.WriteBytes(functionResponseStream.GetBuffer());
            return functionResponse;
        }
        catch (Exception e)
        {
            var error = await this.CreateErrorResponseAsync(req, HttpStatusCode.InternalServerError, e.Message);
            return error;
        }
    }

    /// <summary>
    /// Logs a Link-related error.
    /// </summary>
    /// <param name="req">Request with error data.</param>
    /// <returns>Function-specific response data.</returns>
    [Function(nameof(LinkFail))]
    public async Task<HttpResponseData> LinkFail(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequestData req,
        ILogger logger)
    {
        byte[] buffer = new byte[req.Body.Length];
        await req.Body.ReadAsync(buffer);
        using Stream stream = new MemoryStream(buffer);
        var deserializedLinkResult = await JsonSerializer.DeserializeAsync<LinkResult>(stream);
        logger.LogError($"Link error occured!\n ");
        var response = req.CreateResponse();
        //TODO: Fill the response.
        return response;
    }

    private async Task<HttpResponseData> CreateErrorResponseAsync(HttpRequestData request, HttpStatusCode statusCode, string message)
    {
        var error = request.CreateResponse();
        error.StatusCode = statusCode;
        await error.WriteStringAsync(message);
        return error;
    }
}
