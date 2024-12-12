using Amazon.DynamoDBv2;
using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.OpenApi.Models;
using TalkToMe.Configuration;
using TalkToMe.Core.Configuration;
using TalkToMe.Core.Interfaces;
using TalkToMe.Core.Services;
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
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(setup =>
{
    // Include 'SecurityScheme' to use JWT Authentication
    var jwtSecurityScheme = new OpenApiSecurityScheme
    {
        BearerFormat = "JWT",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        Description = "Put **_ONLY_** your JWT Bearer token on textbox below!",
    };
    setup.AddSecurityDefinition("Bearer", jwtSecurityScheme);
    setup.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            ArraySegment<string>.Empty
        }
    });
});

builder.Services.Configure<AuthenticationSchemeOptions>(o =>
{
    
}).AddAuthentication(o => { o.DefaultScheme = "CognitoToken"; })
.AddScheme<AuthenticationSchemeOptions, CognitoTokenAuthHandler>("CognitoToken", _ => {});

builder.Services.AddBedrockServices(new BedrockSettings
{
    Region = "us-east-1",
    DefaultModelId = "us.meta.llama3-1-8b-instruct-v1:0"
});

var mapperConfig = new MapperConfiguration(mc =>
{
    mc.AddProfile(new MappingProfile());
});

IMapper mapper = mapperConfig.CreateMapper();
builder.Services.AddSingleton(mapper);
builder.Services.AddAWSService<IAmazonDynamoDB>();
builder.Services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
builder.Services.AddScoped<ILanguageRepository, LanguageRepository>();
builder.Services.AddScoped<IUserPreferenceService, UserPreferenceService>();
builder.Services.AddScoped<ILanguageService, LanguageService>();
builder.Services.AddSingleton<DynamoDbTableManager>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

app.UseCors();

app.UseHttpsRedirection();

app.UseAuthorization();
app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var tableManager = scope.ServiceProvider.GetRequiredService<DynamoDbTableManager>();
    // Ensure table exists and seed data
    await tableManager.CreateTablesIfNotExist();
}

app.Run();