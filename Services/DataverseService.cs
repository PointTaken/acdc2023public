using PirateApi.Functions.Models;
using System.Net.Http.Headers;
using Microsoft.Identity.Client;
using Newtonsoft.Json;

namespace PirateApi.Functions.Services;

public interface IDataverseService
{
    public Task<string> Query(string entity, string query);
}

public class DataverseService: IDataverseService
{
    private readonly string TenantId = "";
    private readonly string ClientId = "";
    private readonly string CrmUri = "";
    private readonly HttpClient _client;

    public DataverseService()
    {
        var clientSecret = Environment.GetEnvironmentVariable("ClientSecret");
        var authBuilder = ConfidentialClientApplicationBuilder.Create(ClientId)
            .WithTenantId(TenantId)
            .WithClientSecret(clientSecret)
            .Build();

        string[] scopes = {
            CrmUri + "/.default"
        };

        var token = authBuilder.AcquireTokenForClient(scopes).ExecuteAsync().Result;

        _client = new HttpClient
        {
            BaseAddress = new Uri(CrmUri + "/api/data/v9.2/"),
            Timeout = new TimeSpan(0, 2, 0)  // Standard two minute timeout.
        };

        HttpRequestHeaders headers = _client.DefaultRequestHeaders;
        headers.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);
        headers.Add("OData-MaxVersion", "4.0");
        headers.Add("OData-Version", "4.0");
        headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }



    public async Task<string> Query(string entity, string query)
    {
        var requestUri = entity;

        if(string.IsNullOrWhiteSpace(query) is false)
            requestUri += $"?{query}";
        
        var result = await _client.GetAsync(requestUri);
        var json = await result.Content.ReadAsStringAsync();

        var objectResult = JsonConvert.DeserializeObject(json);
        var prettyJson = JsonConvert.SerializeObject(objectResult, Formatting.Indented);

        return prettyJson;
    }
}
