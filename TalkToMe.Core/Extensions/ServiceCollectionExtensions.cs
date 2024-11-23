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
        services.AddScoped<IAiService, AiService>();
        services.AddScoped<IAiModelService, LamaAiModelService>();
        
        return services;
    }
}