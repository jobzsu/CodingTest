using Microsoft.Extensions.DependencyInjection;
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
    options.SwaggerDoc("1.0", new()
    {
        Title = "Rainfall Api",
        Version = "1.0",
        Contact = new()
        {
            Name = "Sorted",
            Url = new Uri("https://www.sorted.com")
        },
        Description = "An API which provides rainfall reading data"
    });

    options.SwaggerGeneratorOptions.DescribeAllParametersInCamelCase = true;

    options.AddServer(new()
    {
        Url = "http://localhost:3000",
        Description = "Rainfall Api"
    });

    // Set the comments path for the Swagger JSON and UI.
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);
});

// Inject factory
builder.Services.AddScoped<IRainfaillReadingFactory, RainfaillReadingFactory>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(s =>
    {
        s.SwaggerEndpoint("/swagger/1.0/swagger.yaml", "Rainfall Api");
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
