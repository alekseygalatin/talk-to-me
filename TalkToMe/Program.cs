using Amazon.DynamoDBv2;
using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.OpenApi.Models;
using TalkToMe.Configuration;
using TalkToMe.Core.Configuration;
using TalkToMe.Core.Interfaces;
using TalkToMe.Core.Services;
using TalkToMe.Domain.Entities;
using TalkToMe.Infrastructure.Helpers;
using TalkToMe.Infrastructure.IRepository;
using TalkToMe.Infrastructure.Repository;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddCors(x => x.AddDefaultPolicy(y =>
    y.AllowAnyMethod()
     .AllowAnyOrigin()
     .AllowAnyHeader()));

builder.Services.AddAWSLambdaHosting(LambdaEventSource.RestApi);

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services
    .AddEndpointsApiExplorer()
    .AddSwaggerGen(options =>
    {
        const string identifier = "ApiKey";
        options.AddSecurityDefinition(identifier, new()
            {
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Description = "Put **_ONLY_** your token on textbox below!",
            });
        options.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new ()
                    {
                        Id = identifier,
                        Type = ReferenceType.SecurityScheme
                    }
                },
                Array.Empty<string>()
            }
        });
    });

builder.Services.AddMemoryCache();
builder.Services.AddHttpClient();

builder.Services
    .Configure<AuthenticationSchemeOptions>(_ => { })
    .AddAuthentication(o => { o.DefaultScheme = "CognitoToken"; })
    .AddScheme<AuthenticationSchemeOptions, CognitoTokenAuthHandler>("CognitoToken", _ => { });

builder.Services.AddBedrockServices(new BedrockSettings
{
    Region = "us-east-1",
    DefaultModelId = "us.meta.llama3-1-8b-instruct-v1:0"
});

var mapperConfig = new MapperConfiguration(mc =>
{
    mc.AddProfile(new MappingProfile());
});

var mapper = mapperConfig.CreateMapper();
builder.Services.AddSingleton(mapper);
builder.Services.AddAWSService<IAmazonDynamoDB>();
builder.Services.AddSingleton(typeof(IBaseRepository<>), typeof(BaseRepository<>));
builder.Services.AddSingleton<ILanguageRepository, LanguageRepository>();
builder.Services.AddSingleton<IWordRepository, WordRepository>();
builder.Services.AddSingleton<IChatHistoryRepository, ChatHistoryRepository>();
builder.Services.AddSingleton<IUserPreferenceService, UserPreferenceService>();
builder.Services.AddSingleton<IQueryCounterRepository, QueryCounterRepository>();
builder.Services.AddSingleton<ILanguageService, LanguageService>();
builder.Services.AddSingleton<IQueryCounterService, QueryCounterService>();
builder.Services.AddSingleton<IWordService, WordService>();
builder.Services.AddSingleton<IHistoryService, HistoryService>();
builder.Services.AddSingleton<IBedrockAgentService, BedrockAgentService>();
builder.Services.AddSingleton<DynamoDbTableManager>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

/*app.Use(async (context, next) =>
{
    if (context.Request.Method == HttpMethods.Options)
    {
        context.Response.Headers.Add("Access-Control-Allow-Origin", "http://localhost:5173");
        context.Response.Headers.Add("Access-Control-Allow-Methods", "GET, POST, PUT, DELETE");
        context.Response.Headers.Add("Access-Control-Allow-Headers", "Content-Type, Authorization");
        context.Response.StatusCode = StatusCodes.Status204NoContent;
        return;
    }

    await next();
});*/

app.UseCors();

app.UseHttpsRedirection();

app.UseAuthorization();
app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var tableManager = scope.ServiceProvider.GetRequiredService<DynamoDbTableManager>();
    // Seed langauges if not exist
    await tableManager.SeedLanguageDataAsync();
}

app.Run();