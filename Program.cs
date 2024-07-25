using System.Net.Http;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Text;

var builder = WebApplication.CreateBuilder(args);

var client = new HttpClient();
client.Timeout = TimeSpan.FromMilliseconds(60 * 1000 * 10); // 10 minutes

// Add Semantic Kernel
IKernelBuilder kernelBuilder = builder.Services.AddKernel();
// should be able to add the completion endpoint directly per
//      https://github.com/microsoft/semantic-kernel/blob/a2f495023dc73a7ca48cc7405f95498846e04cb4/dotnet/samples/Demos/ContentSafety/Program.cs#L27
//builder.Services.AddOpenAIChatCompletion(modelId: "phi3", apiKey: null, endpoint: new Uri("http://localhost:11434"), httpClient: client); 
#pragma warning disable SKEXP0010
kernelBuilder.AddOpenAIChatCompletion(modelId: "phi3", apiKey: null, endpoint: new Uri("http://localhost:11434"), httpClient: client);

var app = builder.Build();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast")
.WithOpenApi();

app.Run("http://localhost:5000");

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
