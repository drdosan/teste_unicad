using Raizen.UniCad.Web.Models.Filtros;
using System.Web.Routing;

namespace Raizen.UniCad.Web.Util
{
    public static class WebHelper
    {
        public static bool VerificarMenuAberto()
        {
            return System.Web.HttpContext.Current.Request.Cookies["mini_sidebar"]?.Value == "1";
        }
    }
}