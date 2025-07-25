using Microsoft.Extensions.DependencyInjection;
using TalkToMe.Core.Configuration;
using TalkToMe.Core.Factories;
using TalkToMe.Core.Interfaces;
using TalkToMe.Core.Services;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBedrockServices(
        this IServiceCollection services,
        BedrockSettings settings)
    {
        services.AddSingleton(settings);
        services.AddSingleton<AwsAgentFactory>();
        services.AddSingleton<IBedrockClientFactory, BedrockClientFactory>();
        services.AddSingleton<IAIProviderFactory, AIProviderFactory>();
        services.AddSingleton<BedrockService>();

        return services;
    }

    public static IServiceCollection AddOpenAiServices(
        this IServiceCollection services,
        OpenAiSettings settings)
    {
        services.AddScoped<IAiModelService, ChatGPT4oMiniModelService>();
        services.AddTransient<OpenAiService>(opts => 
        {
            return new OpenAiService(settings);
        });

        return services;
    }
}