using Common.Filters;
using Swashbuckle.AspNetCore.Annotations;

namespace Common.Models.WebAPI;

[SwaggerSchemaFilter(typeof(CamelCasingPropertiesFilter))]
public class ErrorResponse
{
    public string Message { get; set; }

    public List<ErrorDetail> Details { get; set; }
}
