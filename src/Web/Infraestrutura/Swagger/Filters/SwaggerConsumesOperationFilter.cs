using Raizen.UniCad.Web.Infraestrutura.Swagger.Attributes;
using Swashbuckle.Swagger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Description;

namespace Raizen.UniCad.Web.Infraestrutura.Swagger.Filters
{
    public class SwaggerConsumesOperationFilter : IOperationFilter
    {
        public void Apply(Operation operation, SchemaRegistry schemaRegistry, ApiDescription apiDescription)
        {
           var attribute = apiDescription.GetControllerAndActionAttributes<SwaggerConsumesAttribute>().SingleOrDefault(x => x.GetType() == typeof(SwaggerConsumesAttribute));
            if (attribute == null)
            {
                return;
            }
            else
            {
                operation.consumes.Clear();
                operation.consumes = (attribute as SwaggerConsumesAttribute).ContentTypes.ToList();
            }
        }
    }
}