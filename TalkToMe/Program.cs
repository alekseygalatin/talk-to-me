using TalkToMe.Core.Configuration;

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
builder.Services.AddSwaggerGen();
builder.Services.AddBedrockServices(new BedrockSettings
{
    Region = "us-east-1",
    DefaultModelId = "us.meta.llama3-1-8b-instruct-v1:0"
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();