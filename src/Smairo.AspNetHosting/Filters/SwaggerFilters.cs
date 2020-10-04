using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
namespace Smairo.AspNetHosting.Filters
{
    /// <summary>
    /// Hides additionalProperties fields from swagger generation
    /// </summary>
    public class HideAdditionalPropertiesFilter : IDocumentFilter
    {
        /// <summary>
        /// Apply filter to schema
        /// </summary>
        /// <param name="openApiDoc"></param>
        /// <param name="context"></param>
        public void Apply(OpenApiDocument openApiDoc, DocumentFilterContext context)
        {
            foreach (var schema in context.SchemaRepository.Schemas)
            {
                schema.Value.AdditionalPropertiesAllowed = false;
            }
        }
    }
}