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

namespace Raizen.UniCad.Web.Controllers
{
    public class TipoDocumentoController : BaseUniCadController
    {
        // Comentário de teste
        #region Constantes
        private readonly TipoDocumentoBusiness TipoDocumentoBLL = new TipoDocumentoBusiness();
        private const string NomeFiltro = "Filtro_TipoDocumento";
        private const string NomePaginador = "Paginador_TipoDocumento";
        private const string TotalRegistros = "totalRegistros_TipoDocumento";
        private const int ID_PAIS_ARGENTINA = 2;
        #endregion

        #region Index
        [HttpGet]
        public ActionResult Index()
        {
            return this.CarregarDefault();
        }

        #endregion

        #region Novo
        [HttpGet]
        [AjaxOnlyAttribute]
        public ActionResult Novo()
        {
            ModelTipoDocumento Model = new ModelTipoDocumento();
            Model.TipoDocumento = new TipoDocumento();
            Model.Resultado = new ResultadoOperacao();
            Model.TipoDocumento.Status = true;
            Model.TipoDocumento.TipoAcaoVencimento = (int)EnumTipoAcaoVencimento.SemAcao;
            Model.TipoDocumento.DocumentoPossuiVencimento = true;
            Model.TipoDocumento.TiposComposicao = new TipoComposicaoBusiness().ListarPorPais(ID_PAIS_ARGENTINA);
            return PartialView("_Edicao", Model);
        }
        #endregion

        #region Editar
        [HttpGet]
        [AjaxOnly]
        public ActionResult Editar(string Id)
        {
            ModelTipoDocumento Model = new ModelTipoDocumento();

            if (!string.IsNullOrEmpty(Id))
            {

                Model.Operacao = OperacoesCRUD.Editando;
                Model.ChavePrimaria = Id;
                Model.Resultado = new ResultadoOperacao();

                Model.TipoDocumento = new TipoDocumento();
                Model.TipoDocumento.ID = int.Parse(Model.ChavePrimaria);
                Model.TipoDocumento = this.TipoDocumentoBLL.Selecionar(Model.TipoDocumento.ID);

                var _pais = Model.TipoDocumento?.IDPais == 2 ? EnumPais.Argentina : EnumPais.Brasil;

                Model.TipoDocumento.TiposProduto = new TipoDocumentoTipoProdutoBusiness().ListarTipoProdutoPorTipoDocumento(Model.TipoDocumento.ID);
                Model.TipoDocumento.TiposVeiculo = new TipoDocumentoTipoVeiculoBusiness().ListarTipoVeiculoPorTipoDocumento(Model.TipoDocumento.ID);
                Model.TipoDocumento.ComposicaoPlaca = new TipoDocumentoTipoComposicaoBusiness(_pais).ListarTipoComposicaoPorTipoDocumento(Model.TipoDocumento.ID).ToList();
                Model.TipoDocumento.ComposicaoMotorista = new TipoDocumentoTipoComposicaoBusiness().ListarComposicaoPorTipoDocumentoSemPlaca(Model.TipoDocumento.ID).ToList();
            }

            Model.TipoDocumento.TiposComposicao = new TipoComposicaoBusiness().ListarPorPais(ID_PAIS_ARGENTINA);

            return PartialView("_Edicao", Model);
        }

        #endregion

        #region CarregarDefault
        private ActionResult CarregarDefault()
        {
            ModelTipoDocumento model = new ModelTipoDocumento();
            model = TipoDocumentoLayout(model);
            model.Operacao = OperacoesCRUD.List;
            model.Filtro = new TipoDocumentoFiltro();
            model.Filtro.Status = true;
            model.Resultado = new ResultadoOperacao();
            model.PaginadorDados = new PaginadorModel();
            model.PaginadorDados.Status = EstadoPaginador.RenovandoConsulta;
            model.TipoDocumento = new TipoDocumento()
            {
                TiposComposicao = new TipoComposicaoBusiness().ListarPorPais(ID_PAIS_ARGENTINA),
            };
            this.ListarPaginador(model);

            return View("Index", model);
        }

        #endregion

        [AjaxOnly]
        [HttpGet]
        public ActionResult GetComposicoes(int docId, int paisId)
        {

            var _pais = paisId == 2 ? EnumPais.Argentina : EnumPais.Brasil;

            IList<TipoDocumentoTipoComposicaoPlacaView> composicoes = new TipoDocumentoTipoComposicaoBusiness(_pais).ListarTipoComposicaoPorTipoDocumento(docId).ToList();
            return Json(composicoes, JsonRequestBehavior.AllowGet);
        }

        #region TipoDocumentoLayout
        private ModelTipoDocumento TipoDocumentoLayout(ModelTipoDocumento model)
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
        private void ListarPaginador(ModelTipoDocumento Model)
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

        private void ListarRenovandoConsulta(ModelTipoDocumento Model)
        {
            base.ArmazenarDados<TipoDocumentoFiltro>(Model.Filtro, NomeFiltro);

            this.AtualizarQtdeRegPaginador(Model);
            int totalReg = base.RetornaDados<int>(TotalRegistros);

            Model.PaginadorDados = ModelUtils.IniciarPaginador(Model.PaginadorDados, totalReg);
            base.ArmazenarDados<int>(totalReg, TotalRegistros);
            base.ArmazenarDados<PaginadorModel>(Model.PaginadorDados, NomePaginador);

            if (totalReg > 0)
            {
                Model.ListaTipoDocumento = this.TipoDocumentoBLL.ListarTipoDocumento(Model.Filtro, Model.PaginadorDados);
            }
        }

        private void AtualizarQtdeRegPaginador(ModelTipoDocumento Model)
        {
            Model.Filtro = base.RetornaDados<TipoDocumentoFiltro>(NomeFiltro);
            base.ArmazenarDados<TipoDocumentoFiltro>(Model.Filtro, NomeFiltro);

            int totalRegistros = this.TipoDocumentoBLL.ListarTipoDocumentoCount(Model.Filtro);
            base.ArmazenarDados<int>(totalRegistros, TotalRegistros);
        }

        private void Listar(ModelTipoDocumento Model)
        {
            this.ListarPaginador(Model);
            Model.Resultado = base.ProcessarResultado(!Model.ListaTipoDocumento.IsNullOrEmpty(), Model.Operacao);
        }
        #endregion

        #region Pesquisar
        [HttpGet]
        [AjaxOnlyAttribute]
        public ActionResult Pesquisar(ModelTipoDocumento Model)
        {
            this.PrepararPaginadorOperacoes(Model);
            this.Listar(Model);
            return PartialView("_Pesquisa", Model);
        }

        #endregion

        #region Paginacao

        private void PrepararPaginadorOperacoes(ModelTipoDocumento Model)
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

        private void ListarPaginando(ModelTipoDocumento Model)
        {
            Model.Filtro = base.RetornaDados<TipoDocumentoFiltro>(NomeFiltro);
            base.ArmazenarDados<TipoDocumentoFiltro>(Model.Filtro, NomeFiltro);

            int totalReg = base.RetornaDados<int>(TotalRegistros);
            Model.PaginadorDados.QtdeTotalRegistros = totalReg;

            base.ArmazenarDados<int>(totalReg, TotalRegistros);
            this.PrepararPaginador(ref Model);

            if (totalReg > 0)
            {
                Model.ListaTipoDocumento = this.TipoDocumentoBLL.ListarTipoDocumento(Model.Filtro, Model.PaginadorDados);
            }
        }

        private void PrepararPaginador(ref ModelTipoDocumento Model)
        {
            PaginadorModel clone = Model.PaginadorDados.Clone() as PaginadorModel;
            Paginador.CalcularPaginas(clone, TipoPaginador.Modelo_Linq);
            Model.PaginadorDados = clone;
        }

        private void ZerarFiltro(ModelTipoDocumento Model)
        {
            Model.Filtro = new TipoDocumentoFiltro();
            base.ArmazenarDados<TipoDocumentoFiltro>(Model.Filtro, NomeFiltro);
        }

        #endregion

        #region Salvar

        [AjaxOnly]
        public ActionResult Salvar(ModelTipoDocumento Model)
        {
            Model.TipoValidacao = TiposValidacao.ValidacaoTotal;

            if (Model.TipoDocumento.IDPais == (int)EnumPais.Brasil && Model.TipoDocumento.tipoCadastro == (int)EnumTipoCadastroDocumento.Motorista)
            {
                Model.TipoDocumento.TiposProduto = null;
                Model.TipoDocumento.TiposVeiculo = null;
                Model.TipoDocumento.ComposicaoMotorista = null;
            }

            if (Model.TipoDocumento.IDPais == (int)EnumPais.Brasil && Model.TipoDocumento.tipoCadastro == (int)EnumTipoCadastroDocumento.Veiculo)
            {
                Model.TipoDocumento.ComposicaoMotorista = null;
                Model.TipoDocumento.TiposComposicao = null;
            }

            if (Model.TipoDocumento.IDPais == (int)EnumPais.Argentina && Model.TipoDocumento.tipoCadastro == (int)EnumTipoCadastroDocumento.Motorista)
            {
                Model.TipoDocumento.TiposComposicao = null;
            }

            if (Model.TipoDocumento.IDPais == (int)EnumPais.Argentina && Model.TipoDocumento.tipoCadastro == (int)EnumTipoCadastroDocumento.Veiculo)
            {
                Model.TipoDocumento.ComposicaoMotorista = null;
                Model.TipoDocumento.TiposVeiculo = null;
            }

            if (base.ValidarModel(Model, this.ModelState))
            {
                Model.TipoDocumento.IDCategoriaVeiculo = Model.TipoDocumento.tipoCadastro == 2 ? (int)EnumCategoriaVeiculo.Motorista : Model.TipoDocumento.IDCategoriaVeiculo;

                if (Model.TipoDocumento.TipoAcaoVencimento == (int)EnumTipoAcaoVencimento.SemAcao)
                {
                    Model.TipoDocumento.BloqueioImediato = null;
                }

                if (Model.TipoDocumento.BloqueioImediato != (int)EnumTipoBloqueioImediato.Nao)
                    Model.TipoDocumento.QtdDiasBloqueio = null;

                if (Model.Operacao == OperacoesCRUD.Insert)
                {
                    Model.Resultado = base.ProcessarResultado(this.TipoDocumentoBLL.AdicionarTipoDocumento(Model.TipoDocumento), OperacoesCRUD.Insert);
                    this.ZerarFiltro(Model);
                    this.AtualizarQtdeRegPaginador(Model);
                    Model.Operacao = OperacoesCRUD.Update;
                }

                if (Model.Operacao == OperacoesCRUD.Editando)
                {
                    Model.TipoDocumento.ID = int.Parse(Model.ChavePrimaria);

                    Model.Resultado = base.ProcessarResultado(this.TipoDocumentoBLL.AtualizarTipoDocumento(Model.TipoDocumento), OperacoesCRUD.Update);
                    Model.Operacao = OperacoesCRUD.Update;
                }
            }

            Model.TipoDocumento.TiposComposicao = new TipoComposicaoBusiness().ListarPorPais(ID_PAIS_ARGENTINA);
            return PartialView("_Edicao", Model);
        }

        public JsonResult CarregarCategorias(string idTipoCategoria)
        {
            if (!string.IsNullOrEmpty(idTipoCategoria))
            {
                int id = Convert.ToInt32(idTipoCategoria);
                var retorno = new CategoriaVeiculoBusiness().Listar(w => w.Tipo == id);
                return Json(new SelectList(retorno, "ID", "Nome"), JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(string.Empty, JsonRequestBehavior.AllowGet);
            }

        }

        #endregion

        #region ExcluirRegistro

        [HttpGet]
        [AjaxOnlyAttribute]
        public bool ExcluirRegistro(int Id)
        {
            ModelTipoDocumento Model = new ModelTipoDocumento();
            try
            {
                Model.Resultado = base.ProcessarResultado(this.TipoDocumentoBLL.Excluir(Id), OperacoesCRUD.Update);
            }
            catch
            {
                return false;
            }
            return true;
        }

        #endregion


        #region WebMethods

        public PartialViewResult AdicionarTipoProduto(int idTipoProduto, int idPais)
        {
            var model = new TipoDocumentoTipoProdutoView();
            var tipo = new TipoProdutoBusiness().Selecionar(idTipoProduto);
            model.IDTipoProduto = tipo.ID;
            model.Nome = tipo.Nome;

            if (ID_PAIS_ARGENTINA == idPais)
                return PartialView("~/Views/MotoristaArgentina/_Produtos.cshtml", model);
            return PartialView("_ItemTipoProduto", model);
        }

        public PartialViewResult AdicionarTipoVeiculo(int idTipoVeiculo)
        {
            var model = new TipoDocumentoTipoVeiculoView();
            var tipo = new TipoVeiculoBusiness().Selecionar(idTipoVeiculo);
            model.IDTipoVeiculo = tipo.ID;
            model.Nome = tipo.Nome;
            return PartialView("_ItemTipoVeiculo", model);
        }

        public PartialViewResult AdicionarComposicaomotorista(int idTipoComposicao, int idPais)
        {
            var model = new TipoDocumentoTipoComposicaoPlacaView();
            var tipo = new TipoComposicaoBusiness().Selecionar(idTipoComposicao);
            model.IdComposicao = tipo.ID;
            model.NomeComposicao = tipo.Nome;

            if (ID_PAIS_ARGENTINA == idPais)
                return PartialView("~/Views/MotoristaArgentina/_Composicoes.cshtml", model);
            return PartialView("_ItemTipoComposicao", model);
        }

        #endregion

    }

}
