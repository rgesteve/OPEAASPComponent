using System.Net.Http;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Text;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

var client = new HttpClient();
client.Timeout = TimeSpan.FromMilliseconds(60 * 1000 * 10); // 10 minutes

builder.Services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = "vscodelinux:6379";
        });

// Add Semantic Kernel
IKernelBuilder kernelBuilder = builder.Services.AddKernel();
// should be able to add the completion endpoint directly per
//      https://github.com/microsoft/semantic-kernel/blob/a2f495023dc73a7ca48cc7405f95498846e04cb4/dotnet/samples/Demos/ContentSafety/Program.cs#L27
//builder.Services.AddOpenAIChatCompletion(modelId: "phi3", apiKey: null, endpoint: new Uri("http://localhost:11434"), httpClient: client); 
#pragma warning disable SKEXP0010
kernelBuilder.AddOpenAIChatCompletion(modelId: "phi3", apiKey: null, endpoint: new Uri("http://localhost:11434"), httpClient: client);

var app = builder.Build();

app.MapGet("/v1/dataprep", async (IDistributedCache cache) =>
{
    var person = await GetDocumentFromCache(cache);
    return person != null ? Results.Ok(person) : Results.NotFound();
});

app.MapPost("/v1/dataprep", async (Document document, IDistributedCache cache) => {
    var json = JsonSerializer.Serialize(document);
    //var jsonString = json.ToString();
    //await cache.SetStringAsync("person", json);
    //await cache.SetStringAsync("person2", "testing");
    await cache.SetStringAsync("document", json);
    return Results.Created("/document", document);
});

app.Run("http://localhost:5000");

/*private */static async Task<Document> GetDocumentFromCache(IDistributedCache cache)
{
    var cachedDocument = await cache.GetStringAsync("document");
    return cachedDocument != null ? JsonSerializer.Deserialize<Document>(cachedDocument) : null;
}

#if false
/*private */static async Task StorePersonInCache(IConnectionMultiplexer redis, string json)
{
    var db = redis.GetDatabase();
    await db.StringSetAsync("person", json);
}
#endif

public class Document
{
    public string? Name { get; set; }
    public int Id { get; set; }
}

