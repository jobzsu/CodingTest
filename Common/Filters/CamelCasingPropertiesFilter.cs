using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Common.Filters;

public class CamelCasingPropertiesFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema model, SchemaFilterContext context)
    {
        model.Properties = model.Properties.ToDictionary(
            d => d.Key.Substring(0, 1).ToLower() + d.Key.Substring(1),
            d => d.Value);
    }
}
