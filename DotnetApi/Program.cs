using DotnetApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// 1. Register the built-in in-memory cache
builder.Services.AddMemoryCache();
var baseUrl = builder.Configuration["Api:BaseUrl"]
    ?? throw new InvalidOperationException("Configuration value 'Api:BaseUrl' is missing.");

// 2. Register the typed HttpClient
builder.Services.AddHttpClient<ICollegeService, CollegeService>(client =>
{
    client.BaseAddress = new Uri(baseUrl);
    client.Timeout = TimeSpan.FromSeconds(10);
});

var app = builder.Build();

app.MapControllers();
app.Run();
