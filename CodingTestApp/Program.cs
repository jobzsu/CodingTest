using CodingTestApp.CustomMiddleware;
using Common.Filters;
using Microsoft.OpenApi.Models;
using RainfallReadingService;
using RainfallReadingService.Abstractions;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
    });
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

// Configure Swagger generation options
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

    options.SchemaFilter<CamelCasingPropertiesFilter>();
    options.SchemaGeneratorOptions.SchemaFilters.Add(new CamelCasingPropertiesFilter());
    options.DescribeAllParametersInCamelCase();

    // Set the comments path for the Swagger JSON and UI.
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);
});

// Inject rainfall service
builder.Services.AddScoped<IRainfallReadingService, RainfallReadingService.RainfallReadingService>();

// Inject http client and configure default http client options
builder.Services.AddHttpClient("RainfallClient", options =>
{
    options.BaseAddress = new Uri(builder.Configuration["RainfallApi:BaseAddress"]!);
    options.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
});

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
        "http://localhost:3000"
        ])
    .AllowAnyHeader()
    .AllowAnyMethod());

app.UseMiddleware<GlobalErrorHandlerMiddleware>();

app.UseAuthorization();

app.MapControllers();

app.Run();
