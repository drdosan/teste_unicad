//using Microsoft.;
//using Swashbuckle.Swagger;
//using Swashbuckle.AspNetCore.SwaggerGen;
//using System.Collections.Generic;
//using System.Linq;

//namespace SGI.Backend.Api.Infraestrutura.Swagger.Filters
//{
//    public class FormFileOperationFilter : IOperationFilter
//    {
//        public void Apply(Operation operation, OperationFilterContext context)
//        {
//            if (operation.Parameters == null)
//                return;

//            var paramFormFile = context.ApiDescription.ActionDescriptor.Parameters
//                                    .Where(x => x.ParameterType.IsAssignableFrom(typeof(IFormFile)))
//                                    .SingleOrDefault();

//            if (paramFormFile == null)
//            {
//                return;
//            }

//            var propsParamFormFile = paramFormFile.ParameterType.GetProperties().Select(x => x.Name);
//            if (!propsParamFormFile.Any())
//            {
//                return;
//            }

//            var parametrosParaRemoverOperacao = new List<IParameter>();
//            foreach (var param in operation.Parameters)
//            {
//                if (propsParamFormFile.Contains(param.Name))
//                {
//                    parametrosParaRemoverOperacao.Add(param);
//                }
//            }

//            parametrosParaRemoverOperacao.ForEach(x=> operation.Parameters.Remove(x));

//            var fileParam = new NonBodyParameter
//            {
//                Type = "file",
//                Name = paramFormFile.Name,
//                In = "formData"
//            };
//            operation.Parameters.Add(fileParam);

//            foreach (IParameter param in operation.Parameters)
//            {
//                param.In = "formData";
//            }

//            //operation.Consumes = new List<string>() { "multipart/form-data" };
//        }
//    }
//}
