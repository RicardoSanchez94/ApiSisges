using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using ServicioApi.Model.Clases;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ServicioApi.Model.Example
{
    public class IdocFIPagosRPExample : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            if (context.Type == typeof(IdocFIPagos_RPExample))
            {
                schema.Properties["Name"].Example = new OpenApiString("Example Name");
            }
        }
    }
}
