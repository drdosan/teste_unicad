using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Raizen.UniCad.Web.Models;
using Raizen.Framework.Web.MVC.Bases;
using Raizen.UniCad.Web.Util;
using Raizen.Framework.UserSystem.Client;
using Raizen.SSO.BLL;
using Raizen.Framework.Web.MVC.Models;

namespace Raizen.UniCad.Web.Controllers
{
    public class HomeController : BaseUniCadController
    {
        private string IbmUsuario => Session["ibmUsuario"]?.ToString();
        public HomeController() : base(BaseControllerOptions.NaoValidarAcesso)
        {
        }

        [ChildActionOnly]
        public ActionResult Menu()
        {
            return PartialView("_Menu", MenuHelper.ObterListaMenu());
        }

        //
        // GET: /Home/
        public ActionResult Index()
        {

            ModelHome model = new ModelHome();
            model.ConfiguracaoLayout.UtilizaComponenteBusca = false;
            model.ConfiguracaoLayout.UtilizaEmailUsuario = false;
            model.ConfiguracaoLayout.UtilizaListaAplicacoes = false;
            model.ConfiguracaoLayout.UtilizaMenuEsquerdo = true;
            model.ConfiguracaoLayout.UtilizaNotificacaoUsuario = false;
            model.ConfiguracaoLayout.UtilizaPerfilUsuario = true;
            model.ConfiguracaoLayout.UtilizaStatusTime = false;
            model.ConfiguracaoLayout.UtilizaTarefaUsuario = false;

            var usuario = UsuarioLogado;
            if (usuario == null)
            {
                throw new Exception("Usuário não cadastrado no UNICAD, entre em contato com o responsável");
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult Index(int id)
        {

            return View();
        }

        public ActionResult SimularErro()
        {
            throw new Exception("Simulação de erro ocorrido");
        }
    }
}
