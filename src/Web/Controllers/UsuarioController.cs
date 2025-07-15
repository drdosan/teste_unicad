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
    public class UsuarioController : BaseUniCadController
    {
        #region Constantes
        private readonly UsuarioBusiness UsuarioBLL = new UsuarioBusiness();
        private const string NomeFiltro = "Filtro_Usuario";
        private const string NomePaginador = "Paginador_Usuario";
        private const string TotalRegistros = "totalRegistros_Usuario";
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
            ModelUsuario Model = new ModelUsuario();
            Model.Usuario = new Usuario();
            Model.Resultado = new ResultadoOperacao();
            Model.Usuario.Status = true;
            return PartialView("_Edicao", Model);
        }
        #endregion

        #region Editar
        [HttpGet]
        [AjaxOnlyAttribute]
        public ActionResult Editar(string Id)
        {
            ModelUsuario Model = new ModelUsuario();

            if (!string.IsNullOrEmpty(Id))
            {
                Model.Operacao = OperacoesCRUD.Editando;
                Model.ChavePrimaria = Id;
                Model.Resultado = new ResultadoOperacao();

                Model.Usuario = new Usuario();
                Model.Usuario.ID = int.Parse(Model.ChavePrimaria);
                Model.Usuario = this.UsuarioBLL.Selecionar(Model.Usuario.ID);
                Model.Usuario.Clientes = new UsuarioClienteBusiness().ListarClientesPorUsuario(Model.Usuario.ID);
                Model.Usuario.Clientes.ForEach(w => w.RazaoSocial = w.IBM + " - " + w.CPF_CNPJ + " - " + w.RazaoSocial);
                Model.Usuario.Transportadoras = new UsuarioTransportadoraBusiness().ListarTransportadorasPorUsuario(Model.Usuario.ID);
                Model.Usuario.Transportadoras.ForEach(w => w.Nome = w.IBM + " - " + w.CNPJCPF + " - " + w.Nome);
            }

            return PartialView("_Edicao", Model);
        }

        #endregion

        #region CarregarDefault
        private ActionResult CarregarDefault()
        {
            ModelUsuario model = new ModelUsuario();
            model = UsuarioLayout(model);
            model.Operacao = OperacoesCRUD.List;
            model.Filtro = new UsuarioFiltro();
            model.Filtro.Status = true;
            model.Resultado = new ResultadoOperacao();
            model.PaginadorDados = new PaginadorModel();
            model.PaginadorDados.Status = EstadoPaginador.RenovandoConsulta;
            this.ListarPaginador(model);

            return View("Index", model);
        }

        #endregion

        #region UsuarioLayout
        private ModelUsuario UsuarioLayout(ModelUsuario model)
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
        private void ListarPaginador(ModelUsuario Model)
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

        private void ListarRenovandoConsulta(ModelUsuario Model)
        {
            base.ArmazenarDados<UsuarioFiltro>(Model.Filtro, NomeFiltro);

            this.AtualizarQtdeRegPaginador(Model);
            int totalReg = base.RetornaDados<int>(TotalRegistros);

            Model.PaginadorDados = ModelUtils.IniciarPaginador(Model.PaginadorDados, totalReg);
            base.ArmazenarDados<int>(totalReg, TotalRegistros);
            base.ArmazenarDados<PaginadorModel>(Model.PaginadorDados, NomePaginador);

            if (totalReg > 0)
            {
                Model.ListaUsuario = this.UsuarioBLL.ListarUsuario(Model.Filtro, Model.PaginadorDados);
            }
        }

        private void AtualizarQtdeRegPaginador(ModelUsuario Model)
        {
            Model.Filtro = base.RetornaDados<UsuarioFiltro>(NomeFiltro);
            base.ArmazenarDados<UsuarioFiltro>(Model.Filtro, NomeFiltro);

            int totalRegistros = this.UsuarioBLL.ListarUsuarioCount(Model.Filtro);
            base.ArmazenarDados<int>(totalRegistros, TotalRegistros);
        }

        private void Listar(ModelUsuario Model)
        {
            this.ListarPaginador(Model);
            Model.Resultado = base.ProcessarResultado(!Model.ListaUsuario.IsNullOrEmpty(), Model.Operacao);
        }
        #endregion

        #region Pesquisar
        [HttpGet]
        [AjaxOnlyAttribute]
        public ActionResult Pesquisar(ModelUsuario Model)
        {
            this.PrepararPaginadorOperacoes(Model);
            this.Listar(Model);
            return PartialView("_Pesquisa", Model);
        }

        #endregion

        #region Paginacao

        private void PrepararPaginadorOperacoes(ModelUsuario Model)
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

        private void ListarPaginando(ModelUsuario Model)
        {
            Model.Filtro = base.RetornaDados<UsuarioFiltro>(NomeFiltro);
            base.ArmazenarDados<UsuarioFiltro>(Model.Filtro, NomeFiltro);

            int totalReg = base.RetornaDados<int>(TotalRegistros);
            Model.PaginadorDados.QtdeTotalRegistros = totalReg;

            base.ArmazenarDados<int>(totalReg, TotalRegistros);
            this.PrepararPaginador(ref Model);

            if (totalReg > 0)
            {
                Model.ListaUsuario = this.UsuarioBLL.ListarUsuario(Model.Filtro, Model.PaginadorDados);
            }
        }

        private void PrepararPaginador(ref ModelUsuario Model)
        {
            PaginadorModel clone = Model.PaginadorDados.Clone() as PaginadorModel;
            Paginador.CalcularPaginas(clone, TipoPaginador.Modelo_Linq);
            Model.PaginadorDados = clone;
        }

        private void ZerarFiltro(ModelUsuario Model)
        {
            Model.Filtro = new UsuarioFiltro();
            base.ArmazenarDados<UsuarioFiltro>(Model.Filtro, NomeFiltro);
        }

        #endregion

        #region Salvar

        [AjaxOnlyAttribute]
        public ActionResult Salvar(ModelUsuario Model)
        {
            Model.TipoValidacao = TiposValidacao.ValidacaoTotal;

            if (base.ValidarModel(Model, this.ModelState))
            {
                Model.Usuario.Email = Model.Usuario.Email.Trim();
                if (Model.Usuario.Externo && Model.Usuario.Perfil == "Transportadora" || Model.Usuario.Perfil == "Transportadora Argentina")
                    Model.Usuario.Operacao = "CIF";
                if (Model.Usuario.Externo && Model.Usuario.Perfil == "Cliente EAB")
                {
                    Model.Usuario.Operacao = "FOB";
                    Model.Usuario.IDEmpresa = (int)EnumEmpresa.EAB;
                }
                if (Model.Usuario.Externo && Model.Usuario.Perfil == "Transportadora" || Model.Usuario.Perfil == "Transportadora Argentina")
                    Model.Usuario.Login = Model.Usuario.Email;

                if (Model.Operacao == OperacoesCRUD.Insert)
                {
                    var retorno = this.UsuarioBLL.AdicionarUsuario(Model.Usuario);

                    if (string.IsNullOrEmpty(retorno))
                    {
                        Model.Resultado = base.ProcessarResultado(true, OperacoesCRUD.Insert);

                        this.ZerarFiltro(Model);
                        this.AtualizarQtdeRegPaginador(Model);
                        Model.Operacao = OperacoesCRUD.Update;
                    }
                    else
                    {
                        Model.Resultado = base.ProcessarResultado(false, retorno);
                        Model.ContemErrosModel = "S";
                    }
                }

                if (Model.Operacao == OperacoesCRUD.Editando)
                {
                    Model.Usuario.ID = int.Parse(Model.ChavePrimaria);

                    var retorno = this.UsuarioBLL.AtualizarUsuario(Model.Usuario);
                    Model.Operacao = OperacoesCRUD.Update;
                    if (string.IsNullOrEmpty(retorno))
                    {
                        Model.Resultado = base.ProcessarResultado(true, OperacoesCRUD.Update);

                    }
                    else
                    {
                        Model.Resultado = base.ProcessarResultado(false, retorno);
                        Model.ContemErrosModel = "S";
                    }
                }
            }

            return PartialView("_Edicao", Model);
        }

        #endregion

        #region ExcluirRegistro

        [HttpGet]
        [AjaxOnlyAttribute]
        public bool ExcluirRegistro(int Id)
        {
            ModelUsuario Model = new ModelUsuario();
            try
            {
                Model.Resultado = base.ProcessarResultado(this.UsuarioBLL.Excluir(Id), OperacoesCRUD.Update);
            }
            catch
            {
                return false;
            }
            return true;
        }

        #endregion


        #region WebMethods

        public PartialViewResult AdicionarCliente(int idCliente)
        {
            var model = new UsuarioClienteView();
            var cliente = new ClienteBusiness().Selecionar(idCliente);
            model.IDCliente = cliente.ID;
            model.RazaoSocial = cliente.IBM + " - " + cliente.CNPJCPF + " - " + cliente.RazaoSocial;
            return PartialView("_ItemCliente", model);
        }

        public PartialViewResult AdicionarTransportadora(int idTransportadora)
        {
            var model = new UsuarioTransportadoraView();
            var Transportadora = new TransportadoraBusiness().Selecionar(idTransportadora);
            model.IDTransportadora = Transportadora.ID;
            model.Nome = Transportadora.IBM + " - " + Transportadora.CNPJCPF + " - " + Transportadora.RazaoSocial;            
            return PartialView("_ItemTransportadora", model);
        }

        [AjaxOnly]
        public string ResetarSenha(int id)
        {
            return new UsuarioBusiness().ResetarSenha(id);
        }
        #endregion

    }

}
