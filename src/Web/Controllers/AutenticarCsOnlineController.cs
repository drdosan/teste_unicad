using Raizen.Framework.UserSystem.Client;
using Raizen.UniCad.BLL;
using Raizen.UniCad.Model;
using Raizen.UniCad.Web.Models;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Raizen.UniCad.Web.Controllers
{
    [AllowAnonymous]
    public class AutenticarCsOnlineController : Controller
    {
        // GET: Autenticar
        [AllowAnonymous]
        public ActionResult Index(string lan, string tipo, string dv, string token)
        {
            EnumPais pais = GetPais(lan);
            string usuario = null;
            string tokenSso = null;
            string mensagemErro = new ClienteAcsBusiness().Autenticar(dv, token, out usuario, out tokenSso, pais);

            Session["ibmUsuario"] = usuario;

            var user = new UsuarioBusiness().Selecionar(p => p.Login == usuario);

            Session["UsuarioUnicad"] = user;

            AutenticarCsOnlineModel model = new AutenticarCsOnlineModel();
            model = UsuarioLayout(model);
            model.IdPais = (int)pais;

            if (user != null && string.IsNullOrEmpty(user.Email))
            {
                Session["TOKEN_SSO_TEMP"] = tokenSso;
                model.Usuario = user.ID;
                model.Redirect = tipo;

                if (pais == EnumPais.Argentina)
                    return View("IndexAr", model);

                return View(model);

            }
            else if (!string.IsNullOrEmpty(mensagemErro))
            {
                model.Mensagem = mensagemErro;

                if (pais == EnumPais.Argentina)
                    return View("IndexAr", model);

                return View(model);
            }
            else
            {
                Session["TOKEN_SSO"] = tokenSso;
                if (Equals(model.IdPais, (int)EnumPais.Brasil))
                {
                    if (tipo == "Motorista")
                    {
                        return RedirectToAction("Index", "Motorista");
                    }
                    else
                    {
                        return RedirectToAction("Index", "Composicao");
                    }
                }
                else
                {
                    if (tipo == "Motorista")
                    {
                        return RedirectToAction("Index", "MotoristaArgentina");
                    }
                    else
                    {
                        return RedirectToAction("Index", "ComposicaoArgentina");
                    }

                }
            }
        }

        public ActionResult IndexModel(int userID, string tipo, EnumPais pais)
        {
            AutenticarCsOnlineModel model = new AutenticarCsOnlineModel();
            model = UsuarioLayout(model);
            model.IdPais = (int)pais;
            model.Usuario = userID;
            model.Redirect = tipo;

            if (pais == EnumPais.Argentina)
                return View("IndexAr", model);

            return View("Index", model);
        }

        public ActionResult IndexMensagemErro(string lan, string mensagem)
        {
            EnumPais pais = GetPais(lan);
            AutenticarCsOnlineModel model = new AutenticarCsOnlineModel();
            model = UsuarioLayout(model);
            model.IdPais = (int)pais;

            model.Mensagem = mensagem;

            if (pais == EnumPais.Argentina)
                return View("IndexAr", model);

            return View("Index", model);
        }

        private EnumPais GetPais(string lan)
        {
            switch (lan)
            {
                case "pt-BR":
                    return EnumPais.Brasil;

                case "es-AR":
                    return EnumPais.Argentina;
            }

            return 0;
        }

        public ActionResult EmailNecessario(AutenticarCsOnlineModel model)
        {
            return View("Index", model);
        }

        public ActionResult AtualizarEmail(AutenticarCsOnlineModel model)
        {
            if (!string.IsNullOrEmpty(model.Email))
            {
                UsuarioBusiness userBll = new UsuarioBusiness();
                var usuario = userBll.Selecionar(p => p.ID == model.Usuario);
                usuario.Email = model.Email;

                if (model.IdPais == (int)EnumPais.Argentina)
                    usuario.Perfil = EnumPerfil.CLIENTE_ACS_ARGENTINA;

                userBll.Atualizar(usuario);
                Session["TOKEN_SSO"] = Session["TOKEN_SSO_TEMP"];
                Session["UsuarioUnicad"] = usuario;

                if (model.Redirect == "Motorista")
                {
                    if (model.IdPais == (int)EnumPais.Brasil)
                        return RedirectToAction("Index", "Motorista");

                    return RedirectToAction("Index", "MotoristaArgentina");
                }
                else if (model.Redirect == "Composicao")
                {
                    if (model.IdPais == (int)EnumPais.Brasil)
                        return RedirectToAction("Index", "Composicao");

                    return RedirectToAction("Index", "ComposicaoArgentina");
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                model.Mensagem = ClienteAcsBusiness.GetMensagemTraduzida((EnumPais)model.IdPais,
                    "Bem vindo ao UNICAD! Para o primeiro acesso, informe um e-mail válido. Ele será utilizado para envio de notificações do seu interesse.",
                    "¡Bienvenido a UNICAD! Para el primer inicio de sesión, ingrese una dirección de correo electrónico válida. Se utilizará para enviar notificaciones de interés para usted.");

                return View("Index", model);
            }
        }

        #region Private methods

        private AutenticarCsOnlineModel UsuarioLayout(AutenticarCsOnlineModel model)
        {
            model.ConfiguracaoLayout.UtilizaComponenteBusca = false;
            model.ConfiguracaoLayout.UtilizaEmailUsuario = false;
            model.ConfiguracaoLayout.UtilizaListaAplicacoes = false;
            model.ConfiguracaoLayout.UtilizaMenuEsquerdo = false;
            model.ConfiguracaoLayout.UtilizaNotificacaoUsuario = false;
            model.ConfiguracaoLayout.UtilizaPerfilUsuario = false;
            model.ConfiguracaoLayout.UtilizaStatusTime = false;
            model.ConfiguracaoLayout.UtilizaTarefaUsuario = false;
            return model;
        }

        #endregion
    }
}