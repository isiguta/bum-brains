using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace BumBrains;

public class Account
{
    private readonly HttpClient httpClient;
    private readonly ILogger logger;
    private const string endpoint = "/accounts/get";

    public Account(HttpClient httpClient, ILoggerFactory loggerFactory)
    {
        this.httpClient = httpClient;
        this.logger = loggerFactory.CreateLogger<Account>();

        ArgumentNullException.ThrowIfNull(nameof(httpClient));
        ArgumentNullException.ThrowIfNull(nameof(loggerFactory));
    }

    [Function(nameof(Account))]
    public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequestData req)
    {
        try
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
        catch (Exception)
        {

            throw;
        }
    }
}
