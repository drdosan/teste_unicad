using Raizen.UniCad.Web.Models;
using Raizen.Framework.Models;
using Raizen.Framework.UserSystem.Client;
using Raizen.UserSystem.SAL.Model;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Globalization;

namespace Raizen.UniCad.Web.Util
{
    public static class MenuHelper
    {
        public static IEnumerable<ItemMenuViewModel> ObterListaMenu()
        {
            if (UserSession.GetCurrentInfoUserSystem() == null)
            {
                return Enumerable.Empty<ItemMenuViewModel>();
            }

            var menus = UserSession
                .GetCurrentInfoUserSystem()
                .InformacoesMenu
                .Where(p => !string.IsNullOrEmpty(p.MVCController) && p.VisivelMenu)
                .OrderBy(p => p.OrdemMenuModulo)
                .ThenBy(p => p.OrdemMenu)
                .ToList();

            return menus
                .Where(p => p.IdModuloPai.GetValueOrDefault(0) == 0)
                .GroupBy(p => new { p.IdModulo, p.NomeModulo })
                .Select(modulo => new ItemMenuViewModel
                {
                    Nome = modulo.Key.NomeModulo,
                    Icone = ObterIcone(menus, modulo.Key.IdModulo),
                    RotaAtual = menus.Any(p => p.IdModulo == modulo.Key.IdModulo && VerificarRotaAtual(p)),
                    Filhos = menus
                            .Where(p => p.IdModulo == modulo.Key.IdModulo)
                            .Select(p => new ItemMenuViewModel
                            {
                                Endereco = ObterEndereco(p),
                                Nome = p.NomePagina,
                                RotaAtual = VerificarRotaAtual(p),
                            })
                });
        }

        private static string ObterIcone(IEnumerable<Menu> menus, int idmodulo)
        {
            var pagina = menus.FirstOrDefault(p => p.IdModulo == idmodulo && !string.IsNullOrEmpty(p.IconeModulo));
            if (pagina != null)
            {
                return pagina.IconeModulo;
            }

            return "fa-cog";
        }

        private static string MontarUrl(Menu menu)
        {
            if (!string.IsNullOrEmpty(menu.URL) && menu.URL.ToLower(CultureInfo.InvariantCulture) == "index")
            {
                return string.Format("~/{0}", menu.MVCController);
            }

            return string.Format("~/{0}/{1}", menu.MVCController, menu.URL);
        }

        private static bool VerificarRotaAtual(Menu menu)
        {
            return HttpContext.Current.Request.Url.AbsolutePath.Equals(ObterEndereco(menu));
        }

        private static string ObterEndereco(Menu menu)
        {
            return string.IsNullOrEmpty(menu.URL) ? new RotaMVC(menu).UrlRota : MontarUrl(menu);
        }
    }
}
