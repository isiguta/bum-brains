using System.Net;
using BumBrains.Functions.Link;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace BumBrains;

public class CurrentBalance
{
    private readonly HttpClient httpClient;
    private readonly ILogger logger;
    private const string endpoint = "/cashpro/reporting/v1/balance-inquiries/current-day";

    public CurrentBalance(HttpClient httpClient, ILoggerFactory loggerFactory)
    {
        this.httpClient = httpClient;
        this.logger = loggerFactory.CreateLogger<LinkFunctions>();
    }

    [Function(nameof(CurrentBalance))]
    public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequestData req)
    {
        this.logger.LogInformation("Authenticating with Bankf of America...");

        byte[] buffer = new byte[req.Body.Length];
        await req.Body.ReadAsync(buffer);
        ByteArrayContent content = new(buffer);
        var endpointResponse = await this.httpClient.PostAsync(endpoint, content);
        var response = req.CreateResponse(HttpStatusCode.OK);
        response.Headers.Add("Content-Type", "application/json; charset=utf-8");

        //response.WriteString("Welcome to Azure Functions!");

        return response;
    }
}
