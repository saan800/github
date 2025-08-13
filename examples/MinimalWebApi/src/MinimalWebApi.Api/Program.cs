using Microsoft.OpenApi.Models;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddEndpointsApiExplorer(); // Required for OpenAPI
// Microsoft’s built-in OpenAPI generator
builder.Services.AddOpenApi(o =>
{
    o.AddDocumentTransformer((document, context, cancellationToken) =>
    {
        document.Info = new OpenApiInfo
        {
            Title = "My Basic Minimal Web API",
            Version = "v1",
            Description = "Awesome api that does some things",
            Contact = new OpenApiContact
            {
                Name = "Acme Ltd",
                Email = "farley@example.com",
                Url = new Uri("https://example.com"),
            }
        };
        return Task.CompletedTask;
    });
    o.AddDocumentTransformer((document, context, cancellationToken) =>
    {
        foreach (var path in document.Paths.Values)
        {
            foreach (var op in path.Operations.Values)
            {
                if (op.Responses.TryGetValue("201", out var response))
                {
                    response.Headers["Location"] = new Microsoft.OpenApi.Models.OpenApiHeader
                    {
                        Description = "URL of the newly created resource",
                        Schema = new Microsoft.OpenApi.Models.OpenApiSchema
                        {
                            Type = "string",
                            Format = "uri"
                        }
                    };
                }
            }
        }
        return Task.CompletedTask;
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
// if (app.Environment.IsDevelopment())
// {
// Enable OpenAPI JSON
app.MapOpenApi(); //.CacheOutput(); // Add Scalar UI pointing to the OpenAPI JSON
app.MapScalarApiReference("docs", options =>
{
    options
        .WithTheme(ScalarTheme.DeepSpace)
        .WithDefaultHttpClient(ScalarTarget.Shell, ScalarClient.Curl)
        .AddDocument("v1");
});
// }

app.UseHttpsRedirection();

//app.UseAuthorization();

app.MapControllers();

app.Run();
