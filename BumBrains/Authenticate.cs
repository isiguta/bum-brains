using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace BumBrains;

public class Authenticate
{
    private readonly HttpClient _httpClient;
    private readonly ILogger _logger;

    public Authenticate(HttpClient httpClient, ILoggerFactory loggerFactory)
    {
        _httpClient = httpClient;
        _logger = loggerFactory.CreateLogger<Authenticate>();
    }

    [Function("Authenticate")]
    public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestData req)
    {
        _logger.LogInformation("C# HTTP trigger function processed a request.");

        await _httpClient.PostAsync("", req.Body);
        var response = req.CreateResponse(HttpStatusCode.OK);
        response.Headers.Add("Content-Type", "text/plain; charset=utf-8");

        response.WriteString("Welcome to Azure Functions!");

        return response;
    }
}
