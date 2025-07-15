using System;
using System.Web.Mvc;

using Raizen.Framework.UserSystem.Client;
using Raizen.Framework.Web.Common;

using Raizen.UniCad.Web.Models;

namespace Raizen.UserSystem.Web.Controllers
{
    /// <summary>
    /// Controlador de erro da aplicação
    /// </summary>
    public class ErrorController : Controller
    {
        #region Actions

        /// <summary>
        /// Ocorre quando um erro inesperado é capturado pelo filter de erro
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            // Mensagem técnica será a exceção ocorrida, caso exista. Senão, será a mensagem
            // repassada por quem invocou a rota de erro
            var msgTecnica = 
                ApplicationSession.UltimaExcecaoInesperada == null ?
                    ApplicationSession.UltimoErroInesperado :
                    "Mensagem da exceção: " + ApplicationSession.UltimaExcecaoInesperada.Message;

            return this.RetornarViewErro("Ocorreu um erro inesperado",
                                         "Estamos com problemas técnicos. Consulte o log para mais detalhes",
                                         msgTecnica,
                                         AcaoUsuarioErro.ContacteHelpDesk);
        }

        /// <summary>
        /// Ocorre quando um algum recurso não é encontrado dentro do sistema
        /// </summary>
        /// <returns></returns>
        public ActionResult NotFound()
        {
            return this.RetornarViewErro("Recurso não encontrado",
                                         "Você digitou um endereço que não existe em nosso sistema",
                                         string.IsNullOrWhiteSpace(ApplicationSession.UltimoErroInesperado) ?
                                            string.Empty :
                                            "URL solicitada: " + ApplicationSession.UltimoErroInesperado,
                                         AcaoUsuarioErro.TenteNovamente);
        }

        /// <summary>
        /// Ocorre quando o filtro que valida autenticidade e permissão encontra alguma falha
        /// </summary>
        /// <returns></returns>
        public ActionResult SemPermissao()
        {
            return this.RetornarViewErro("Acesso Negado",
                                         "Você não possui permissão para acessar esse recurso",
                                         string.IsNullOrWhiteSpace(ApplicationSession.UltimoErroInesperado) ?
                                             string.Empty :
                                             "Recurso: " + ApplicationSession.UltimoErroInesperado,
                                         AcaoUsuarioErro.SoliciteAcesso);
        }


        public ActionResult ErrorJavaScript()
        {
            return ErrorJavaScript(null, null);
        }

        /// <summary>
        /// Ocorre quando a recursod e central de erro java script localizado na Master View (Shared/_Layout.cshtml)
        /// Encontra uma falha de Script, seja ela CSS, JavaScript, Bundles ...
        /// </summary>
        /// <param name="Mensagem"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        /// 

        public ActionResult ErrorJavaScript(string Mensagem, string url)
        {
            return this.RetornarViewErro("Erro de recurso Script (Css, JavaScript e Imagens)",
                                         "Ocorreu um erro interno no sistema",
                                         Mensagem + " -- " + url,
                                         AcaoUsuarioErro.ContacteHelpDesk);
        }

        #endregion

        #region Métodos Privados

        /// <summary>
        /// Prepara a view de erro com os dados fornecidos e retorna o resultado
        /// </summary>
        /// <param name="titulo"></param>
        /// <param name="mensagem"></param>
        /// <param name="detalheTecnico"></param>
        /// <param name="tipoAcao"></param>
        /// <returns></returns>
        private ActionResult RetornarViewErro(string titulo, string mensagem, string detalheTecnico, AcaoUsuarioErro tipoAcao)
        {
            var model = new ModelErro();

            model.Titulo = titulo;
            model.MensagemAmigavel = mensagem;
            model.DetalheTecnico = detalheTecnico;
            this.PreencherAcao(tipoAcao, model);
            this.TratarBotoesTela(model);

            ApplicationSession.UltimoErroInesperado = string.Empty;

            return View("Index", model);
        }

        /// <summary>
        /// Preenche o campo detalhe da ação do usuário
        /// </summary>
        /// <param name="Acao"></param>
        /// <param name="Model"></param>
        private void PreencherAcao(AcaoUsuarioErro Acao, ModelErro Model)
        {
            switch (Acao)
            {
                case AcaoUsuarioErro.SoliciteAcesso:
                    Model.DescricaoAcaoUsuario = "Entre em contato com o Administrador do sistema e solicite acesso";
                    break;
                case AcaoUsuarioErro.TenteNovamente:
                    Model.DescricaoAcaoUsuario = "Verifique se as informações fornecidas estão corretas e tente novamente";
                    break;
                default:
                    Model.DescricaoAcaoUsuario = "Tente novamente mais tarde. Caso o erro persista, entre em contato com o Help Desk";
                    break;
            }
        }

        /// <summary>
        /// Trata a exibição dos botões de tela baseado no usuário logado
        /// </summary>
        /// <param name="Model"></param>
        private void TratarBotoesTela(ModelErro Model)
        {
            // O menu esquerdo e o botão de logout são exibidos apenas se houver usuário logado
            bool exibir = UserSession.UserIsAuthenticated();
            Model.ConfiguracaoLayout.UtilizaMenuEsquerdo = exibir;
            Model.ConfiguracaoLayout.UtilizaPerfilUsuario = exibir;
        }

        #endregion
    }
}