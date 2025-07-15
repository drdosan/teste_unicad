using Raizen.Framework.Log.Bases;
using Raizen.Framework.Utils.Extensions;
using Raizen.Framework.Utils.Helpers;
using Raizen.Framework.Web.Common;
using Raizen.Framework.Web.MVC.Utils;
using Raizen.UserSystem.Web.Controllers;
using System;
using System.Globalization;
using System.Net;
using System.Threading;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace Raizen.UniCad.Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        private const string KEY_THEME = "THEME";
        private const string DEFAULT_THEME = "neutro";

        protected void Application_Start()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            CultureInfo info = new CultureInfo(System.Threading.Thread.CurrentThread.CurrentCulture.ToString());
            info.DateTimeFormat.ShortDatePattern = "dd.MM.yyyy";
            System.Threading.Thread.CurrentThread.CurrentCulture = info;

            AreaRegistration.RegisterAllAreas();

            GlobalConfiguration.Configure(WebApiConfig.Register);

            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            

            // Configura a Cultura para Português
            Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("pt-BR");
            Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture;

            // Carrega o thema definido no web.config
            var theme = ConfigHelper.GetValue(ConfigSection.AppSettings, KEY_THEME, DEFAULT_THEME);
            ContextHelper.SetValue(ContextType.Application, KEY_THEME, theme);

            ModelBinders.Binders.Add(typeof(decimal), new DecimalModelBinder());
            ModelBinders.Binders.Add(typeof(decimal?), new DecimalModelBinder());
            ModelBinders.Binders.Add(typeof(DateTime), new DateTimeModelBinder());
            ModelBinders.Binders.Add(typeof(DateTime?), new DateTimeModelBinder());
            ModelBinders.Binders.Add(typeof(double), new DoubleModelBinder());
            ModelBinders.Binders.Add(typeof(double?), new DoubleModelBinder());

			System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
		}

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            HttpContext.Current.Response.Cache.SetExpires(DateTime.UtcNow.AddDays(-1));
            HttpContext.Current.Response.Cache.SetValidUntilExpires(false);
            HttpContext.Current.Response.Cache.SetRevalidation(HttpCacheRevalidation.AllCaches);
            HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            HttpContext.Current.Response.Cache.SetNoStore();
            // Code scanning: Missing X-Frame-Options HTTP header
            HttpContext.Current.Response.Headers.Add("X-Frame-Options", "DENY");
        }

        /// <summary>
        /// Application_Error
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Application_Error(object sender, EventArgs e)
        {
            TratarErroAplicacao(Server.GetLastError());
        }

        private void TratarErroAplicacao(Exception exception)
        {
            TiposRota tipoRota;

            // Especificamente para erros Http Code 404 (página não encontrada), redirecionamos
            // para a view de Erro, e não efetuamos log
            var http404 = exception as HttpException;
            if (http404 != null && http404.GetHttpCode() == 404)
            {
                Response.TrySkipIisCustomErrors = true;

                // Repassa Url inválida para ser exibida na tela de erro
                ApplicationSession.UltimoErroInesperado = HttpContext.Current.Request.Url.ToString();

                // Redireciona para erro de Not Found (404)
                tipoRota = TiposRota.UrlNaoEncontrada;
            }
            else
            {
                if (exception != null)
                {
                    // Se temos exceção, o conteúdo dela será exibido na tela de erro
                    ApplicationSession.UltimoErroInesperado = string.Empty;
                    ApplicationSession.UltimaExcecaoInesperada = exception;

                    new RaizenException("Erro tratado pelo Global.asax", exception).LogarErro();
                }
                else
                {
                    // Não há exceção (improvável). Exibe erro genérico
                    ApplicationSession.UltimoErroInesperado = "Erro inesperado. Não há mais detalhes a serem exibidos";
                    ApplicationSession.UltimaExcecaoInesperada = null;
                }

                // Redireciona para erro genérico
                tipoRota = TiposRota.Erro;
            }

            // Realiza limpeza do buffer Http e do erro ocorrido
            Response.Clear();
            Server.ClearError();

            // Executa redirecionamento
            var result = Rotas.GetActionResult(HttpContext.Current.Request.GetRequestBase(), tipoRota);
            result.ExecuteResult(Rotas.CriarController<ErrorController>().ControllerContext);
        }
    }
}
