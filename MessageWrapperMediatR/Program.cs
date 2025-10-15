using System.Net;
using Microsoft.AspNetCore.OpenApi;
using System.Text.Json.Serialization;
using MessageWrapperMediatR.Application.Extensions;
using MessageWrapperMediatR.Infrastructure.MessageBus;

//var otherBuilder = Host.CreateDefaultBuilder(args).ConfigureWebHostDefaults(web =>
//{
//    web.UseStartup<Startup>()
//    .UseDefaultServiceProvider(o => o.ValidateScopes = false);
//});
var builder = WebApplication.CreateBuilder(args);

// Use Swagger and OpenApi.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.UseInlineDefinitionsForEnums();
});
builder.Services.AddControllers();
builder.Services.AddMessagingWithMediatR();
builder.Services.AddMessageSystem();
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

var sampleTodos = new Todo[] {
    new(1, "Walk the dog"),
    new(2, "Do the dishes", DateOnly.FromDateTime(DateTime.Now)),
    new(3, "Do the laundry", DateOnly.FromDateTime(DateTime.Now.AddDays(1))),
    new(4, "Clean the bathroom"),
    new(5, "Clean the car", DateOnly.FromDateTime(DateTime.Now.AddDays(2)))
};

var todosApi = app.MapGroup("/todos");
todosApi.MapGet("/", () => sampleTodos)
    .WithOpenApi();
todosApi.MapGet("/{id}", (int id) =>
    sampleTodos.FirstOrDefault(a => a.Id == id) is { } todo
        ? Results.Ok(todo)
        : Results.NotFound())
    .WithOpenApi();

app.Run();

public record Todo(int Id, string? Title, DateOnly? DueBy = null, bool IsComplete = false);

[JsonSerializable(typeof(Todo[]))]
internal partial class AppJsonSerializerContext : JsonSerializerContext
{

}
