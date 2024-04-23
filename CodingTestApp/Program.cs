using CodingTestApp.CustomMiddleware;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Serialization;
using RainfaillReadingService;
using RainfaillReadingService.Abstractions;
using System.Reflection;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers()
    .AddJsonOptions(opt => opt.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase);
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo()
    {
        Title = "Rainfall Api",
        Version = "1.0",
        Contact = new()
        {
            Name = "Sorted",
            Url = new Uri("https://www.sorted.com")
        },
        Description = "An API which provides rainfall reading data",
    });

    options.AddServer(new()
    {
        Url = "http://localhost:3000",
        Description = "Rainfall Api"
    });

    options.EnableAnnotations();
    options.SupportNonNullableReferenceTypes();

    // Set the comments path for the Swagger JSON and UI.
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);
});

// Inject factory
builder.Services.AddScoped<IRainfallReadingFactory, RainfallReadingFactory>();

// Inject global error handler middleware
builder.Services.AddTransient<GlobalErrorHandlerMiddleware>();

builder.Services.AddCors();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(s =>
    {
        s.SwaggerEndpoint("/swagger/v1/swagger.yaml", "Rainfall Api");
    });
}

app.UseCors(options => 
    options.WithOrigins([
        "https://localhost:7157",
        "http://localhost:3000",
        "https://localhost:44384"
        ])
    .AllowAnyHeader()
    .AllowAnyMethod());

app.UseMiddleware<GlobalErrorHandlerMiddleware>();

app.UseAuthorization();

app.MapControllers();

app.Run();
