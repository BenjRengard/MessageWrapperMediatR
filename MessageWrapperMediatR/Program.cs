using System.Net;
using Microsoft.AspNetCore.OpenApi;
using System.Text.Json.Serialization;
using MessageWrapperMediatR.Application.Extensions;
using MessageWrapperMediatR.Infrastructure.MessageBus;
using MessageWrapperMediatR.Infrastructure.RabbitMq;
using MessageWrapperMediatR.Infrastructure.IbmMqSeries;
using MessageWrapperMediatR.Infrastructure.Kafka;
using MessageWrapperMediatR.Infrastructure.SqlServer;

var builder = WebApplication.CreateBuilder(args);
IConfigurationRoot config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .AddEnvironmentVariables()
    .Build();

// Use Swagger and OpenApi.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.UseInlineDefinitionsForEnums();
});
builder.Services.AddControllers();

// References du MessageWrapper.
builder.Services.AddMessagingWithMediatR();
builder.Services.AddPublisherServices(config);
builder.Services.AddRabbitMqConfiguration(config);
builder.Services.AddIbmMqSeriesConfiguration(config);
builder.Services.AddKafkaConfiguration(config);
builder.Services.AddSqlServerMessageWrapper(config);
builder.Services.AddMappers();


builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default);
});

var app = builder.Build();

// User Swagger and Swashbuckle.
app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();

app.Run();

public record Todo(int Id, string? Title, DateOnly? DueBy = null, bool IsComplete = false);

[JsonSerializable(typeof(Todo[]))]
internal partial class AppJsonSerializerContext : JsonSerializerContext
{

}
