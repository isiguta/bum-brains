using System.Net;
using BumBrains.Services.Banking.Client;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace BumBrains.Functions.Link;

/// <summary>
/// Class that contains functions related to Link part of authentication with Plaid.
/// </summary>
public sealed class LinkFunctions
{
    private readonly HttpClient _httpClient;
    private readonly ILogger _logger;
    private const string endpoint = "/cashpro/reporting/v1/balance-inquiries/current-day";

    /// <summary>
    /// Creates an instance of the <see cref="LinkFunctions"/> class.
    /// </summary>
    /// <param name="httpClient">http client, configured specifically for Link-related business-logic.</param>
    /// <param name="loggerFactory">Logger provider.</param>
    public LinkFunctions(HttpClient httpClient, BankingClient bankingClient, ILoggerFactory loggerFactory)
    {
        _httpClient = httpClient;
        _logger = loggerFactory.CreateLogger<LinkFunctions>();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="req"></param>
    /// <returns></returns>
    [Function(nameof(CreateLinkToken))]
    public async Task<HttpResponseData> CreateLinkToken(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestData req)
    {
        _logger.LogInformation("Authenticating with Bankf of America...");

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
    /// 
    /// </summary>
    /// <returns></returns>
    [Function(nameof(ExchangePublicToken))]
    public async Task<HttpResponseData> ExchangePublicToken(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestData req)
    {
        return req.CreateResponse();
    }
}
