using System.Web.Mvc;
using Raizen.UniCad.Web.Models;
using Raizen.UniCad.BLL;
using Raizen.UniCad.Model.View;
using System;
using Raizen.Framework.UserSystem.Client;
using System.IO;
using Raizen.UniCad.Utils;
using Raizen.Framework.Models;
using Raizen.UniCad.Model.Filtro;

namespace Raizen.UniCad.Web.Controllers
{
    public class ImpressaoCrachaArgentinaController : ImpressaoCrachaBaseController
    {


        #region Constantes
        private readonly ImpressaoCrachaBusiness _impressaoCrachaBll = new ImpressaoCrachaBusiness();
        private readonly MotoristaBusiness _motoristaBll = new MotoristaBusiness();
        #endregion

        // GET: ImpressaoCracha
        #region Index
        [HttpGet]
        public ActionResult Index()
        {
            return this.CarregarDefault();
        }

        #endregion

        #region CarregarDefault
        private ActionResult CarregarDefault()
        {
            ModelImpressaoCracha model = new ModelImpressaoCracha();
            model = ImpressaoCrachaLayout(model);
            return View("Index", model);

        }

        #endregion

        #region Pesquisar
        [HttpGet]
        public ActionResult Pesquisar(ModelImpressaoCracha model)
        {
            model.MotoristaView = _impressaoCrachaBll.BuscarMotoristaArgentina(model.Filtro);
            if (model.MotoristaView != null)
            {
                model.Motorista = new Model.Motorista();
                model.ImpressaoCrachaRetornoView = _impressaoCrachaBll.ValidarMotoristaArgentinoImpressaoCracha(model.MotoristaView);

            }
            else
            {
                model = ImpressaoCrachaLayout(model);
            }

            return PartialView("_Pesquisa", model);

        }
        #endregion


        /// <summary>
        /// Faz dowload e visualizacao do crachá 
        /// </summary>
        /// <param name="idMotorista"></param>
        /// <param name="nomeArquivo">Nome do arquivo de foto no upload</param>
        /// <param name="download"></param>
        /// <returns></returns>
        
        #region ImpressaoCrachaLayout
        private ModelImpressaoCracha ImpressaoCrachaLayout(ModelImpressaoCracha model)
        {
            model.ConfiguracaoLayout.UtilizaComponenteBusca = true;
            model.ConfiguracaoLayout.UtilizaEmailUsuario = true;
            model.ConfiguracaoLayout.UtilizaListaAplicacoes = false;
            model.ConfiguracaoLayout.UtilizaMenuEsquerdo = true;
            model.ConfiguracaoLayout.UtilizaNotificacaoUsuario = true;
            model.ConfiguracaoLayout.UtilizaPerfilUsuario = true;
            model.ConfiguracaoLayout.UtilizaStatusTime = false;
            model.ConfiguracaoLayout.UtilizaTarefaUsuario = false;
            model.Motorista = new Model.Motorista();
            model.Filtro = new ImpressaoCrachaFiltro();
            model.ImpressaoCrachaRetornoView = new ImpressaoCrachaRetornoView();

            return model;
        }
        #endregion

    }
}