using Microsoft.Extensions.Configuration;

public class AppConfig
{
    public IConfiguration Configuration { get; }

    public AppConfig()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false);

        Configuration = builder
            .AddUserSecrets<AppConfig>(optional: true)
            .AddEnvironmentVariables()
            .Build();
    }

    public string OpenAIApiKey =>
        Configuration["OpenAI:ApiKey"]
        ?? throw new Exception("Missing OpenAI API key");

    public string OpenAIModel =>
        Configuration["OpenAI:Model"]
        ?? throw new Exception("Missing OpenAI model");

    public string OpenAIEndpoint =>
        Configuration["OpenAI:Endpoint"]
        ?? throw new Exception("Missing OpenAI endpoint");
}