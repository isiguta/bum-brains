using BumBrains.Models.Configuration.Banking.Plaid.Link;
using BumBrains.Services.Banking.Client;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text.Json;

namespace BumBrains.Functions.Link;

/// <summary>
/// Class that contains functions related to Link part of authentication with Plaid.
/// </summary>
public sealed class LinkFunctions
{
    private readonly HttpClient _httpClient;
    private readonly ILogger _logger;
    private const string endpoint = "";

    /// <summary>
    /// Creates an instance of the <see cref="LinkFunctions"/> class.
    /// </summary>
    /// <param name="httpClient">http client, configured specifically for Link-related business-logic.</param>
    /// <param name="loggerFactory">Logger provider.</param>
    public LinkFunctions(HttpClient httpClient, IBankingProvider bankingClient, ILoggerFactory loggerFactory)
    {
        _httpClient = httpClient;
        _logger = loggerFactory.CreateLogger<LinkFunctions>();
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
        _logger.LogInformation("Creating a Link token...");

        byte[] buffer = new byte[req.Body.Length];
        await req.Body.ReadAsync(buffer);
        ByteArrayContent content = new(buffer);
        var endpointResponse = await _httpClient.PostAsync(endpoint, content);

        if (!endpointResponse.IsSuccessStatusCode)
        {
            // Handle error.
            var errorResponse = req.CreateResponse(endpointResponse.StatusCode);
            var errorResponseContent = await endpointResponse.Content.ReadAsStringAsync();
            await errorResponse.WriteStringAsync(errorResponseContent);
            return errorResponse;
        }

        var response = req.CreateResponse(HttpStatusCode.OK);
        response.Headers.Add("Content-Type", "application/json; charset=utf-8");

        response.WriteString("Welcome to Azure Functions!");

        return response;
    }

    /// <summary>
    /// Function that exchanges a link token for an access token, used in subsequent API requests.
    /// </summary>
    /// <returns>Function-specific response data.</returns>
    [Function(nameof(ExchangePublicToken))]
    public async Task<HttpResponseData> ExchangePublicToken(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestData req)
    {
        return req.CreateResponse();
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
}
