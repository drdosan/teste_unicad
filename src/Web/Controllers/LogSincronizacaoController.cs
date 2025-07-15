using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Raizen.UniCad.Web.Models;
using Raizen.Framework.Web.MVC.Bases;
using Raizen.Framework.Web.MVC.Filters;
using Raizen.UniCad.Model;
using Raizen.UniCad.Model.Filtro;
using Raizen.Framework.Models;
using Raizen.Framework.Web.MVC.Models;
using Raizen.UniCad.BLL;
using Raizen.Framework.Web.MVC.Utils;
using Raizen.Framework.Utils.Extensions;
using Raizen.Framework.UserSystem.Client;
using Raizen.UniCad.Model.View;
using Raizen.UniCad.BLL.Util;

namespace Raizen.UniCad.Web.Controllers
{
    public class LogSincronizacaoController : BaseUniCadController
    {
        #region Constantes
        private readonly LogSincronizacaoBusiness LogSincronizacaoBLL = new LogSincronizacaoBusiness();        
        private readonly LogSincronizacaoBusiness SincronizacaoBLL = new LogSincronizacaoBusiness();        
        private readonly JobBusiness jobBLL = new JobBusiness();
        private const string NomeFiltro = "Filtro_LogSincronizacao";
        private const string NomePaginador = "Paginador_LogSincronizacao";
        private const string TotalRegistros = "totalRegistros_LogSincronizacao";
        #endregion

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
            ModelSincronizacaoMotoritas model = new ModelSincronizacaoMotoritas();
            model = LogSincronizacaoLayout(model);
            model.Operacao = OperacoesCRUD.List;
            model.Filtro = new SincronizacaoMotoristasFiltro();
            model.Resultado = new ResultadoOperacao();
            model.PaginadorDados = new PaginadorModel();
            model.PaginadorDados.Status = EstadoPaginador.RenovandoConsulta;
            this.ListarRenovandoConsulta(model);

            return View("Index", model);
        }

        #endregion

        #region LogSincronizacaoLayout
        private ModelSincronizacaoMotoritas LogSincronizacaoLayout(ModelSincronizacaoMotoritas model)
        {
            model.ConfiguracaoLayout.UtilizaComponenteBusca = true;
            model.ConfiguracaoLayout.UtilizaEmailUsuario = true;
            model.ConfiguracaoLayout.UtilizaListaAplicacoes = false;
            model.ConfiguracaoLayout.UtilizaMenuEsquerdo = true;
            model.ConfiguracaoLayout.UtilizaNotificacaoUsuario = true;
            model.ConfiguracaoLayout.UtilizaPerfilUsuario = true;
            model.ConfiguracaoLayout.UtilizaStatusTime = false;
            model.ConfiguracaoLayout.UtilizaTarefaUsuario = false;
            return model;
        }
        #endregion

        #region Listar

        private void ListarPaginador(ModelSincronizacaoMotoritas Model)
        {
            if (Model.PaginadorDados.Status == EstadoPaginador.RenovandoConsulta && Model.Operacao != OperacoesCRUD.Editando)
            {
                this.ListarRenovandoConsulta(Model);
            }
            else
            {
                this.ListarPaginando(Model);
            }
        }       

        private void ListarRenovandoConsulta(ModelSincronizacaoMotoritas Model)
        {
            base.ArmazenarDados<SincronizacaoMotoristasFiltro>(Model.Filtro, NomeFiltro);

            this.AtualizarQtdeRegPaginador(Model);
            int totalReg = base.RetornaDados<int>(TotalRegistros);

            Model.PaginadorDados = ModelUtils.IniciarPaginador(Model.PaginadorDados, totalReg);
            base.ArmazenarDados<int>(totalReg, TotalRegistros);
            base.ArmazenarDados<PaginadorModel>(Model.PaginadorDados, NomePaginador);

            if (totalReg > 0)
            {
                Model.ListaSincronizacaoMotoritas = this.LogSincronizacaoBLL.Listar(Model.Filtro, Model.PaginadorDados);
            }
        }       

        private void AtualizarQtdeRegPaginador(ModelSincronizacaoMotoritas Model)
        {
            Model.Filtro = base.RetornaDados<SincronizacaoMotoristasFiltro>(NomeFiltro);
            base.ArmazenarDados<SincronizacaoMotoristasFiltro>(Model.Filtro, NomeFiltro);

            int totalRegistros = this.LogSincronizacaoBLL.ListarCount(Model.Filtro);
            base.ArmazenarDados<int>(totalRegistros, TotalRegistros);
        }

        private void Listar(ModelSincronizacaoMotoritas Model)
        {
            this.ListarPaginador(Model);
            Model.Resultado = base.ProcessarResultado(!Model.ListaSincronizacaoMotoritas.IsNullOrEmpty(), Model.Operacao);
        }
        #endregion

        #region Pesquisar
        [HttpGet]
        [AjaxOnlyAttribute]
        public ActionResult Pesquisar(ModelSincronizacaoMotoritas Model)
        {
            this.PrepararPaginadorOperacoes(Model);
            this.Listar(Model);
            return PartialView("_Pesquisa", Model);
        }

        #endregion

        #region Paginacao

        private void PrepararPaginadorOperacoes(ModelSincronizacaoMotoritas Model)
        {
            if (Model.PaginadorDados == null)
            {
                Model.PaginadorDados = base.RetornaDados<PaginadorModel>(NomePaginador);
                base.ArmazenarDados<PaginadorModel>(Model.PaginadorDados, NomePaginador);
            }
            else
            {
                base.ArmazenarDados<PaginadorModel>(Model.PaginadorDados, NomePaginador);
            }

            //Sifnifica que não tem registro ainda
            if (Model.PaginadorDados == null)
            {
                Model.PaginadorDados = new PaginadorModel();
            }

            Model.PaginadorDados.ConjuntoPaginas = ModelUtils.ListarConjuntoPaginas();
        }

        private void ListarPaginando(ModelSincronizacaoMotoritas Model)
        {
            Model.Filtro = base.RetornaDados<SincronizacaoMotoristasFiltro>(NomeFiltro);
            base.ArmazenarDados<SincronizacaoMotoristasFiltro>(Model.Filtro, NomeFiltro);

            int totalReg = base.RetornaDados<int>(TotalRegistros);
            Model.PaginadorDados.QtdeTotalRegistros = totalReg;

            base.ArmazenarDados<int>(totalReg, TotalRegistros);
            this.PrepararPaginador(ref Model);

            if (totalReg > 0)
            {
                Model.ListaSincronizacaoMotoritas = this.LogSincronizacaoBLL.Listar(Model.Filtro, Model.PaginadorDados);
            }
        }

        private void PrepararPaginador(ref ModelSincronizacaoMotoritas Model)
        {
            PaginadorModel clone = Model.PaginadorDados.Clone() as PaginadorModel;
            Paginador.CalcularPaginas(clone, TipoPaginador.Modelo_Linq);
            Model.PaginadorDados = clone;
        }

        #endregion
    }
}
