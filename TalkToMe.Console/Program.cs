using Microsoft.Extensions.DependencyInjection;

class Program
{
    static async Task Main(string[] args)
    {
        // Simple error handler without string interpolation
        AppDomain.CurrentDomain.UnhandledException += (sender, eventArgs) =>
        {
            Console.WriteLine("Unhandled exception occurred:");
            Console.WriteLine(eventArgs.ExceptionObject.ToString());
        };

        try
        {
            await RunAsync();
        }
        catch (Exception ex)
        {
            // Avoid string interpolation here as well
            Console.WriteLine("Application error occurred:");
            Console.WriteLine(ex.Message);
            Console.WriteLine(ex.StackTrace);
        }
    }

    private static async Task RunAsync()
    {
        var settings = new BedrockSettings
        {
            Region = "us-east-1",
            DefaultModelId = "us.meta.llama3-1-8b-instruct-v1:0"
        };

        var services = new ServiceCollection()
            .AddBedrockServices(settings);

        using var serviceProvider = services.BuildServiceProvider();
        using var scope = serviceProvider.CreateScope();
        
        var bedrockService = scope.ServiceProvider.GetRequiredService<IBedrockService>();
        
        var request = new BedrockRequestBuilder()
            .WithModel("us.meta.llama3-1-8b-instruct-v1:0")
            .WithPrompt("Tell me a joke")
            .Build();

        var response = await bedrockService.InvokeModelAsync(request);
        Console.WriteLine(response.Response);
    }
}