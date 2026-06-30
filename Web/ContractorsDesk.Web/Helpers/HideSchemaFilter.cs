using ContractorsDesk.Core.ApiPayloadModels;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ContractorsDesk.WebPortal.Helpers
{
    public class HideSchemaFilter : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            schema.Properties.Clear();
            //if (context.Type == typeof(InvoiceItemModel))
            //{
            //    schema.Properties.Clear();
            //}
        }
    }

}
