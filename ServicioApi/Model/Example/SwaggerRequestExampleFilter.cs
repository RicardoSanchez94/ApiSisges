using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Filters;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.Swagger;
using ServicioApi.Model.Clases;

namespace ServicioApi.Model.Example
{
    public class SwaggerRequestExampleFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var parameters = operation.Parameters;

            foreach (var parameter in parameters)
            {
                var parameterAttributes = context.ApiDescription.ParameterDescriptions
                    .FirstOrDefault(p => p.Name == parameter.Name)?.CustomAttributes();

                var exampleAttribute = parameterAttributes?.FirstOrDefault(a => a.GetType().GetProperty("AttributeType")?.GetValue(a) == typeof(SwaggerRequestExampleAttribute));

                if (exampleAttribute != null)
                {
                    var exampleType = exampleAttribute.GetType().GetConstructor(Type.EmptyTypes)?.DeclaringType;
                    var exampleProvider = (IExamplesProvider<object>)Activator.CreateInstance(exampleType!);
                    var example = exampleProvider.GetExamples();

                    if (example != null)
                    {
                        var json = JsonConvert.SerializeObject(example);
                        var schema = context.SchemaGenerator.GenerateSchema(example.GetType(), context.SchemaRepository);
                        parameter.Example = OpenApiAnyFactory.CreateFromJson(json);

                    }
                }
            }
        }

    }


}
