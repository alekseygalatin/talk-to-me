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
        
        services.AddSingleton<IBedrockClientFactory, BedrockClientFactory>();
        services.AddScoped<IConversationManager, ConversationManager>();
        services.AddScoped<IAiService, BedrockService>();
        services.AddScoped<IAiModelService, LamaAiModelService>();
        
        return services;
    }

    public static IServiceCollection AddOpenAiServices(
        this IServiceCollection services,
        OpenAiSettings settings)
    {
        services.AddScoped<IAiService, OpenAiService>(opts => 
        {
            return new OpenAiService(settings);
        });

        return services;
    }
}