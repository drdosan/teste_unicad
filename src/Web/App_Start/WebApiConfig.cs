//using Newtonsoft.Json.Serialization;
using Swashbuckle.Application;
using System.Web.Http;
using System.Web.Http.Cors;

namespace Raizen.UniCad.Web
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            string hostCors = System.Configuration.ConfigurationManager.AppSettings["cors"];

            var enableCorsAttribute = new EnableCorsAttribute(hostCors, "*", "*");
            config.EnableCors(enableCorsAttribute);

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );


            //Remover formatter de XML.
            config.Formatters.Remove(config.Formatters.XmlFormatter);

            //Configurar formatter de JSON, indentando e removendo caixa alta do começo dos nomes de objetos (padrão js).
            var jsonFormatter = config.Formatters.JsonFormatter;
            jsonFormatter.Indent = true;

            jsonFormatter.UseDataContractJsonSerializer = false;
            //jsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
        }
    }
}
