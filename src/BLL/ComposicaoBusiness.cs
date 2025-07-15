using ClosedXML.Excel;
using Raizen.Framework.Log.Bases;
using Raizen.Framework.UserSystem.Client;
using Raizen.Framework.Utils.Transacao;
using Raizen.UniCad.BLL.Extensions;
using Raizen.UniCad.BLL.Interfaces;
using Raizen.UniCad.BLL.Util;
using Raizen.UniCad.DAL;
using Raizen.UniCad.Extensions;
using Raizen.UniCad.Model;
using Raizen.UniCad.Model.Filtro;
using Raizen.UniCad.Model.View;
using Raizen.UniCad.SAL;
using Raizen.UniCad.Utils;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Transactions;

namespace Raizen.UniCad.BLL
{
    public class ComposicaoBusiness : UniCadBusinessBase<Composicao>, IComposicaoBusiness
    {
        readonly PlacaArgentinaBusiness PlacaArgentinaBll = new PlacaArgentinaBusiness();
        readonly PlacaBrasilBusiness PlacaBrasilBll = new PlacaBrasilBusiness();
        private readonly PlacaClienteBusiness _placaClienteBLL = new PlacaClienteBusiness();
        private readonly PlacaBusiness _placaBLL;

        private readonly EnumPais _pais;

        #region Constructors

        public ComposicaoBusiness()
        {
            this._pais = EnumPais.Brasil;

            _placaBLL = new PlacaBusiness(_pais);
        }

        public ComposicaoBusiness(EnumPais pais)
        {
            this._pais = pais;

            _placaBLL = new PlacaBusiness(_pais);
        }

        #endregion

        public List<ComposicaoServicoView> ListarComposicaoServico(ComposicaoServicoFiltro filtro)
        {

            using (var repositorio = new UniCadDalRepositorio<Composicao>())
            {
                var query = GetQueryComposicaoServico(filtro, repositorio);
                var retorno = query.ToList();
                if (retorno.Any())
                {
                    PlacaDocumentoBusiness placaDocBll = new PlacaDocumentoBusiness();
                    foreach (var item in retorno)
                    {
                        if (item.IDPlaca1.HasValue)
                        {
                            item.PlacaCavaloTruck = _placaBLL.ListarPlacaServico(new PlacaServicoFiltro
                            {
                                IDPlaca = item.IDPlaca1.Value,
                                LinhaNegocio = item.LinhaNegocio,
                                Operacao = item.Operacao
                            }).FirstOrDefault();
                            item.TaraComposicao = item.PlacaCavaloTruck.Tara;
                        }
                        if (item.IDPlaca2.HasValue)
                        {
                            item.PlacaCarreta1 = _placaBLL.ListarPlacaServico(new PlacaServicoFiltro
                            {
                                IDPlaca = item.IDPlaca2.Value,
                                LinhaNegocio = item.LinhaNegocio,
                                Operacao = item.Operacao
                            }).FirstOrDefault();
                            item.TaraComposicao += item.PlacaCarreta1.Tara;

                            if (string.IsNullOrEmpty(item.PlacaVeiculoCarreta1))
                                item.PlacaVeiculoCarreta1 = item.PlacaCarreta1.Placa;
                        }
                        if (item.IDPlaca3.HasValue)
                        {
                            item.PlacaDollyCarreta2 = _placaBLL.ListarPlacaServico(new PlacaServicoFiltro
                            {
                                IDPlaca = item.IDPlaca3.Value,
                                LinhaNegocio = item.LinhaNegocio,
                                Operacao = item.Operacao
                            }).FirstOrDefault();
                            item.TaraComposicao += item.PlacaDollyCarreta2.Tara;

                            if (string.IsNullOrEmpty(item.PlacaVeiculoDollyCarreta2))
                                item.PlacaVeiculoDollyCarreta2 = item.PlacaDollyCarreta2.Placa;
                        }
                        if (item.IDPlaca4.HasValue)
                        {
                            item.PlacaCarreta2 = _placaBLL.ListarPlacaServico(new PlacaServicoFiltro
                            {
                                IDPlaca = item.IDPlaca4.Value,
                                LinhaNegocio = item.LinhaNegocio,
                                Operacao = item.Operacao
                            }).FirstOrDefault();
                            item.TaraComposicao += item.PlacaCarreta2.Tara;

                            if (string.IsNullOrEmpty(item.PlacaVeiculoCarreta2))
                                item.PlacaVeiculoCarreta2 = item.PlacaCarreta2.Placa;
                        }

                    }
                }
                return retorno;
            }

        }

        private IQueryable<Composicao> GetQueryVeiculoComposicaoAprovadaBloqueada(int id, string placa, int idTipoVeiculo, int idEmpresa, string operacao, UniCadDalRepositorio<Composicao> repositorio)
        {
            IQueryable<Composicao> query = (from app in repositorio.ListComplex<Composicao>().AsNoTracking().OrderBy(i => i.ID)
                                            join placa1 in repositorio.ListComplex<Placa>().AsNoTracking() on app.IDPlaca1 equals placa1.ID
                                            into j1
                                            from j5 in j1.DefaultIfEmpty()
                                            join placa2 in repositorio.ListComplex<Placa>().AsNoTracking() on app.IDPlaca2 equals placa2.ID
                                            into j2
                                            from j6 in j2.DefaultIfEmpty()
                                            where (app.IDStatus != 3)
                                            && (app.ID != id)
                                            && ((j5.PlacaVeiculo.Contains(placa)
                                                    && app.Operacao == operacao
                                                    && ((app.IDEmpresa == (int)EnumEmpresa.EAB
                                                    && idEmpresa == (int)EnumEmpresa.EAB)
                                                    || app.IDTipoComposicao == (int)EnumTipoComposicao.Truck))
                                                || (j6.PlacaVeiculo.Contains(placa)
                                                    && app.Operacao == operacao
                                                    && app.IDEmpresa == (int)EnumEmpresa.Combustiveis
                                                    && idEmpresa == (int)EnumEmpresa.Combustiveis)
                                                 || ((j5.PlacaVeiculo.Contains(placa)
                                                        || j6.PlacaVeiculo.Contains(placa))
                                                    && app.Operacao == operacao
                                                    && idEmpresa == (int)EnumEmpresa.Ambos))
                                            select app);
            return query;
        }

        private List<ComposicaoServicoView> GetQueryComposicaoServico(ComposicaoServicoFiltro filtro, UniCadDalRepositorio<Composicao> repositorio)
        {
            var paramDataAtualizacao = new SqlParameter("@DataAtualizacao", System.Data.SqlDbType.DateTime);
            var paramLinhaNegocio = new SqlParameter("@LinhaNegocio", System.Data.SqlDbType.Int);
            var paramOperacao = new SqlParameter("@Operacao", System.Data.SqlDbType.VarChar);
            var paramPlaca = new SqlParameter("@Placa", System.Data.SqlDbType.VarChar);

            paramDataAtualizacao.Value = filtro.DataAtualizacao;
            paramLinhaNegocio.Value = filtro.LinhaNegocio ?? (object)DBNull.Value;
            paramOperacao.Value = filtro.Operacao ?? (object)DBNull.Value;
            paramPlaca.Value = filtro.PlacaVeiculo ?? (object)DBNull.Value;

            List<ComposicaoServicoView> dadosRelatorio = ExecutarProcedureComRetorno<ComposicaoServicoView>(
                "[dbo].[Proc_Listar_Composicoes_Servico] @LinhaNegocio,@Operacao,@Placa,@DataAtualizacao",
                new object[] { paramLinhaNegocio, paramOperacao, paramPlaca, paramDataAtualizacao });
            return dadosRelatorio.ToList();
        }

        public List<Composicao> SelecionarPlacaAprovadaBloqueada(int id, string placaVeiculo, int idTipoVeiculo, int idEmpresa, string operacao)
        {
            using (var repositorio = new UniCadDalRepositorio<Composicao>())
            {
                var query = GetQueryVeiculoComposicaoAprovadaBloqueada(id, placaVeiculo, idTipoVeiculo, idEmpresa, operacao, repositorio);
                return query.ToList();
            }
        }

        public bool IntegrarComposicaoPendenteSap()
        {
            var composicoesPendentes = Listar(p => p.IDStatus == (int)EnumStatusComposicao.AguardandoAtualizacaoSAP && p.IdPais == (int)_pais);
            foreach (var item in composicoesPendentes)
            {
                var tentativaMaxima = Config.GetConfig(EnumConfig.QtdTentativasIntegracaoSAP, (int)EnumPais.Padrao);
                int numeroMax;
                var numero = Int32.TryParse(tentativaMaxima, out numeroMax);
                if (!numero)
                    numeroMax = 10;

                if (item.TentativaIntegracao.HasValue && item.TentativaIntegracao >= numeroMax)
                {
                    item.IDStatus = (int)EnumStatusComposicao.EmAprovacao;
                    var placabll = new PlacaBusiness();
                    if (item.IDPlaca1.HasValue)
                    {
                        var placa1 = placabll.Selecionar(item.IDPlaca1.Value);
                        placa1.Observacao = "N�o foi poss�vel atualizar o cadastro no SAP, pois ainda existe LECI aberta. Favor verificar se n�o h� falhas operacionais.";
                        placabll.Atualizar(placa1);
                    }
                    if (item.IDPlaca2.HasValue)
                    {
                        var placa2 = placabll.Selecionar(item.IDPlaca2.Value);
                        placa2.Observacao = "N�o foi poss�vel atualizar o cadastro no SAP, pois ainda existe LECI aberta. Favor verificar se n�o h� falhas operacionais.";
                        placabll.Atualizar(placa2);
                    }
                    if (item.IDPlaca3.HasValue)
                    {
                        var placa3 = placabll.Selecionar(item.IDPlaca3.Value);
                        placa3.Observacao = "N�o foi poss�vel atualizar o cadastro no SAP, pois ainda existe LECI aberta. Favor verificar se n�o h� falhas operacionais.";
                        placabll.Atualizar(placa3);
                    }
                    if (item.IDPlaca4.HasValue)
                    {
                        var placa4 = placabll.Selecionar(item.IDPlaca4.Value);
                        placa4.Observacao = "N�o foi poss�vel atualizar o cadastro no SAP, pois ainda existe LECI aberta. Favor verificar se n�o h� falhas operacionais.";
                        placabll.Atualizar(placa4);
                    }
                    Atualizar(item);
                }
                else
                {
                    item.IDStatus = (int)EnumStatusComposicao.Aprovado;
                    var retorno = AtualizarComposicao(item, false, false, true, item.IDStatus);

                    if (!string.IsNullOrEmpty(item.Mensagem) && item.Mensagem.Contains("Existe LECI aberta"))// adicionar o if olhando para o retorno para ver se tem essa msg assim que alterado no SAP
                    {
                        item.TentativaIntegracao = item.TentativaIntegracao != null ? item.TentativaIntegracao.Value + 1 : 1;
                        item.IDStatus = (int)EnumStatusComposicao.AguardandoAtualizacaoSAP;
                        Atualizar(item);
                    }
                    else if (!string.IsNullOrEmpty(item.Mensagem))
                    {
                        new RaizenException("Aguardando Integra��o SAP - " + retorno).LogarErro();
                        return false;
                    }
                }
            }
            return true;
        }

        public AgendamentoChecklistView ObterPlacas(string placa, int idEmpresa, string operacao, int? idUsuarioCliente, int? idUsuarioTransportadora, out string dados)
        {
            dados = string.Empty;
            var placabll = new PlacaBusiness();
            //var placaSel = placabll.ListarPlacaRelatorio(new ComposicaoFiltro {  IDUsuarioCliente = idUsuarioCliente,IDUsuarioTransportadora = idUsuarioTransportadora, Placa = placa }).Select(p => p.ID);
            var placaSel = placabll.ListarPlacaRelatorio(new ComposicaoFiltro { Placa = placa }).Select(p => p.ID);
            if (placaSel.Any())
            {
                var compaprov = this.Selecionar(p => (placaSel.Any(b => b == p.IDPlaca1) || placaSel.Any(b => b == p.IDPlaca2) || placaSel.Any(b => b == p.IDPlaca3) || placaSel.Any(b => b == p.IDPlaca4)) && (p.IDStatus == (int)EnumStatusComposicao.EmAprovacao || p.IDStatus == (int)EnumStatusComposicao.Reprovado || p.IDStatus == (int)EnumStatusComposicao.AguardandoAtualizacaoSAP));
                var placaUser = placabll.ListarPlacaRelatorio(new ComposicaoFiltro { IDUsuarioCliente = idUsuarioCliente, IDUsuarioTransportadora = idUsuarioTransportadora, Placa = placa }).Select(p => p.ID);

                //a placa existe,  por�m o usu�rio n�o tem acesso a ela ou est� em aprova��o // reprovado / aguardando atualiza��o SAP
                if (compaprov != null || !placaUser.Any())
                {
                    dados = "Esse ve�culo n�o est� apto para o agendamento! Favor verificar o cadastro.";
                }
                else
                {
                    var comp = this.Selecionar(p => (p.IDStatus == (int)EnumStatusComposicao.Aprovado || p.IDStatus == (int)EnumStatusComposicao.Bloqueado) && (p.Operacao == operacao || p.Operacao == "Ambos") && (p.IDEmpresa == idEmpresa || p.IDEmpresa == (int)EnumEmpresa.Ambos) && ((placaSel.Any(b => b == p.IDPlaca2) || ((p.IDEmpresa == (int)EnumEmpresa.EAB || p.IDTipoComposicao == (int)EnumTipoComposicao.Truck) && placaSel.Any(b => b == p.IDPlaca1)))));
                    var model = new AgendamentoChecklistView();
                    if (comp != null)
                    {

                        if (comp.IDStatus == (int)EnumStatusComposicao.Aprovado || comp.IDStatus == (int)EnumStatusComposicao.Bloqueado)
                        {
                            model.Placas = placabll.Selecionar(p => p.ID == comp.IDPlaca1).PlacaVeiculo;
                            if (comp.IDPlaca2.HasValue)
                                model.Placas += " / " + placabll.Selecionar(p => p.ID == comp.IDPlaca2).PlacaVeiculo;
                            if (comp.IDPlaca3.HasValue)
                                model.Placas += " / " + placabll.Selecionar(p => p.ID == comp.IDPlaca3).PlacaVeiculo;
                            if (comp.IDPlaca4.HasValue)
                                model.Placas += " / " + placabll.Selecionar(p => p.ID == comp.IDPlaca4).PlacaVeiculo;

                            model.IDComposicao = comp.ID;

                            var data = new AgendamentoChecklistBusiness().SelecionarAgendamentoChecklistPorComposicao(comp.ID);

                            if (data.HasValue)
                            {
                                dados = "J� existe agendamento ativo para essa composi��o!";
                            }
                            else
                            {
                                var checklistTot = new ChecklistComposicaoBusiness().Listar(p => p.IDComposicao == comp.ID).OrderByDescending(o => o.ID).FirstOrDefault();
                                var checklist = checklistTot != null && checklistTot.Data >= DateTime.Now.Date ? checklistTot : null;
                                if (checklist != null)
                                {
                                    dados = string.Format("Esse ve�culo possui checklist v�lido at� {0}. Caso queira realizar outro checklist, favor seguir com o agendamento.", checklist.Data.ToShortDateString());
                                    model.Mensagem = dados;
                                }

                                return model;
                            }
                        }
                        else
                        {
                            dados = "Esse ve�culo n�o est� apto para o agendamento! Favor verificar o cadastro.";
                        }

                    }
                    else
                    {
                        dados = "Esse ve�culo n�o est� apto para o agendamento! Favor verificar o cadastro.";
                    }
                }
            }
            else
            {
                dados = "Esse ve�culo n�o tem cadastro no sistema! Caso deseje cadastrar, clique em '+'";
            }
            return null;
        }

        private decimal ObterVolumeComposicao(Composicao composicao)
        {

            var volume = SelecionarVolumeCompartimento(composicao.p1)
                + SelecionarVolumeCompartimento(composicao.p2)
                + SelecionarVolumeCompartimento(composicao.p3)
                + SelecionarVolumeCompartimento(composicao.p4);
            return volume;


        }

        public int SelecionarUfCRLV(Composicao composicao)
        {
            if (composicao.IDEmpresa == (int)EnumEmpresa.Combustiveis && composicao.IDPlaca2.HasValue)
            {
                return (int)(new PlacaBusiness().Selecionar(composicao.IDPlaca2.Value).PlacaBrasil.IDEstado);
            }

            if (composicao.IDEmpresa == (int)EnumEmpresa.EAB && composicao.IDPlaca1.HasValue)
                return (int)(new PlacaBusiness().Selecionar(composicao.IDPlaca1.Value).PlacaBrasil.IDEstado);
            return 0;
        }

        public bool AdicionarComposicao(Composicao composicao, bool abrirChamado = true, bool comAlteracoesExcetoClientes = false)
        {
            var idPais = 1;
            using (TransactionScope transactionScope = Transactions.CreateTransactionScope(1, IsolationLevel.ReadCommitted))
            {
                using (UniCadDalRepositorio<Composicao> composicaoRepositorio = new UniCadDalRepositorio<Composicao>())
                {
                    CarregarDados(composicao, false);
                    if (composicao.IDEmpresa == (int)EnumEmpresa.Ambos)
                        composicao.Operacao = "CIF";

                    composicao.IdPais = (int)_pais;
                    composicao.DataAtualizacao = DateTime.Now;
                    composicao.CPFCNPJ = composicao.CPFCNPJ.RemoveCharacter();
                    composicao.CPFCNPJArrendamento = composicao.CPFCNPJArrendamento.RemoveCharacter();
                    composicao.PBTC = composicao.PBTC == null ? 0 : composicao.PBTC;

                    composicaoRepositorio.Add(composicao);

                    if (!composicao.IDComposicao.HasValue)
                    {
                        //verificar se a composi��o est� usando alguma placa chave 

                        List<Composicao> composicaoAnterior;
                        if (composicao.IDEmpresa == (int)EnumEmpresa.EAB || composicao.IDTipoComposicao == (int)EnumTipoComposicao.Truck)
                        {
                            composicaoAnterior = SelecionarPlacaAprovadaBloqueada(composicao.ID, composicao.Placa1, composicao.IDTipoComposicao, composicao.IDEmpresa, composicao.Operacao);
                        }
                        else if (composicao.IDEmpresa == (int)EnumEmpresa.Combustiveis)
                            composicaoAnterior = SelecionarPlacaAprovadaBloqueada(composicao.ID, composicao.Placa2, composicao.IDTipoComposicao, composicao.IDEmpresa, composicao.Operacao);
                        else //ambas
                        {
                            composicaoAnterior = SelecionarPlacaAprovadaBloqueada(composicao.ID, composicao.Placa1, composicao.IDTipoComposicao, composicao.IDEmpresa, composicao.Operacao);
                            if (composicaoAnterior == null)
                                composicaoAnterior = SelecionarPlacaAprovadaBloqueada(composicao.ID, composicao.Placa2, composicao.IDTipoComposicao, composicao.IDEmpresa, composicao.Operacao);
                        }

                        if (composicaoAnterior != null && composicaoAnterior.Any(w => w.IDStatus == (int)EnumStatusComposicao.Aprovado
                          || w.IDStatus == (int)EnumStatusComposicao.Bloqueado
                          || w.IDStatus == (int)EnumStatusComposicao.Reprovado))
                        {
                            Composicao compAnt;
                            if (composicaoAnterior.Any(w => w.IDStatus == (int)EnumStatusComposicao.Reprovado))
                            {
                                compAnt = composicaoAnterior.Where(w => w.IDStatus == (int)EnumStatusComposicao.Reprovado).FirstOrDefault();
                                compAnt.IDStatus = 3;
                                this.Atualizar(compAnt);
                                composicao.IDComposicao = compAnt.ID;
                            }
                            else
                                composicao.IDComposicao = composicaoAnterior.Where(w => w.IDStatus == (int)EnumStatusComposicao.Aprovado || w.IDStatus == (int)EnumStatusComposicao.Bloqueado).FirstOrDefault().ID;
                            composicao.isUtilizaPlacaChave = true;
                        }
                    }

                    if (composicao.IDComposicao.HasValue)
                    {
                        var historicos = new HistorioBloqueioComposicaoBusiness().Listar(p => p.IDComposicao == composicao.IDComposicao);
                        HistorioBloqueioComposicaoBusiness bll = new HistorioBloqueioComposicaoBusiness();
                        foreach (var item in historicos)
                        {
                            item.ID = 0;
                            item.IDComposicao = composicao.ID;
                            bll.Adicionar(item);
                        }

                        var historicosChecklist = new ChecklistComposicaoBusiness().Listar(p => p.IDComposicao == composicao.IDComposicao);
                        ChecklistComposicaoBusiness bllChk = new ChecklistComposicaoBusiness();
                        foreach (var item in historicosChecklist)
                        {
                            item.ID = 0;
                            item.IDComposicao = composicao.ID;
                            bllChk.Adicionar(item);
                        }
                    }

                    composicao.VolumeComposicao = ObterVolumeComposicao(composicao);

                    idPais = (int)SelecionarPaisPlaca1(composicao);
                    var easy = Config.GetConfig(EnumConfig.EasyQuery, idPais);

                    if (abrirChamado && ValidarAbrirChamadoSalesForce(composicao, idPais))
                    {
                        AbrirChamadoSalesForce(composicao, idPais);
                    }

                    else if (abrirChamado && ValidarAbrirChamadoEasyQuery(composicao, idPais))
                    {
                        AbrirChamadoEasyQuery(composicao, idPais);
                    }

                    Atualizar(composicao);

                    transactionScope.Complete();
                }
            }

			if (!comAlteracoesExcetoClientes && PodeAprovarComposicaoAutomaticamente(composicao) && idPais==1)
            {
                var composicaoAprovar = Selecionar(composicao.ID);
                var resultadoAprovacaoAutomatica = AtualizarComposicao(composicaoAprovar, comRessalvas: false, idStatus: (int)EnumStatusComposicao.Aprovado, aprovacaoAutomatica: true);
				composicao.Mensagem = "Aprovação automática realizada com sucesso!";

				return resultadoAprovacaoAutomatica;
			}

            return true;
        }

        private static EnumPais SelecionarPaisPlaca1(Composicao composicao)
        {
            if (composicao.IDPlaca1.HasValue)
                return new PlacaBusiness().Selecionar((int)composicao.IDPlaca1).IDPais;
            else
                return new PlacaBusiness().Selecionar((int)composicao.IDPlaca2).IDPais;
        }

        public string ExcluirComposicao(int id, bool somenteComposicao)
        {
            using (TransactionScope transactionScope = Transactions.CreateTransactionScope(1, IsolationLevel.ReadCommitted))
            {
                Composicao comp = Selecionar(id);
                PlacaBusiness placaBll = new PlacaBusiness();

                ChecklistComposicaoBusiness bll = new ChecklistComposicaoBusiness();
                HistorioBloqueioComposicaoBusiness hbll = new HistorioBloqueioComposicaoBusiness();
                bll.ExcluirLista(p => p.IDComposicao == id);
                hbll.ExcluirLista(p => p.IDComposicao == id);

                string msg = GetMessageExclusao();
                string placas = string.Empty;

                if (!somenteComposicao)
                {
                    if (comp.IDStatus == (int)EnumStatusComposicao.Aprovado || comp.IDStatus == (int)EnumStatusComposicao.Bloqueado)
                    {
                        ExcluirHistorico(comp.IDComposicao);

                        var listaComp = this.Listar(p => p.IDComposicao == comp.ID);
                        foreach (var item in listaComp)
                        {
                            bll.ExcluirLista(p => p.IDComposicao == item.ID);
                            hbll.ExcluirLista(p => p.IDComposicao == item.ID);
                            this.Excluir(item.ID);
                        }
                        CarregarDados(comp, false);
                    }

                    Excluir(id);

                    if (comp.IDPlaca1.HasValue)
                    {
                        var placa1 = placaBll.Selecionar(comp.IDPlaca1.Value);
                        if (!placaBll.ListarPorIdComposicao(placa1.PlacaVeiculo, comp.ID))
                        {
                            placaBll.ExcluirPlaca(comp.IDPlaca1.Value);
                        }
                        else
                        {
                            placas += placa1.PlacaVeiculo + ", ";
                        }
                    }
                    if (comp.IDPlaca2.HasValue)
                    {
                        var placa2 = placaBll.Selecionar(comp.IDPlaca2.Value);
                        if (!placaBll.ListarPorIdComposicao(placa2.PlacaVeiculo, comp.ID))
                        {
                            placaBll.ExcluirPlaca(comp.IDPlaca2.Value);
                        }
                        else
                        {
                            placas += placa2.PlacaVeiculo + ", ";
                        }
                    }
                    if (comp.IDPlaca3.HasValue)
                    {
                        var placa3 = placaBll.Selecionar(comp.IDPlaca3.Value);
                        if (!placaBll.ListarPorIdComposicao(placa3.PlacaVeiculo, comp.ID))
                        {
                            placaBll.ExcluirPlaca(comp.IDPlaca3.Value);
                        }
                        else
                        {
                            placas += placa3.PlacaVeiculo + ", ";
                        }
                    }
                    if (comp.IDPlaca4.HasValue)
                    {
                        var placa4 = placaBll.Selecionar(comp.IDPlaca4.Value);
                        if (!placaBll.ListarPorIdComposicao(placa4.PlacaVeiculo, comp.ID))
                        {
                            placaBll.ExcluirPlaca(comp.IDPlaca4.Value);
                        }
                        else
                        {
                            placas += placa4.PlacaVeiculo + ", ";
                        }
                    }

                    if (!string.IsNullOrEmpty(placas))
                    {
                        placas = placas.Substring(0, placas.Length - 2);
                        msg += GetPlacaMessage(placas);
                    }
                    if (comp.IDStatus == (int)EnumStatusComposicao.Aprovado || comp.IDStatus == (int)EnumStatusComposicao.Bloqueado)
                    {
                        string msgSap = IntegrarSAP(comp, EnumTipoIntegracaoSAP.Excluir);
                        if (!string.IsNullOrEmpty(msgSap.Trim()))
                        {
                            msg = msgSap;
                            return msg;
                        }
                    }
                }
                else
                {
                    Excluir(id);
                }

                transactionScope.Complete();
                return msg;
            }
        }

        private string GetPlacaMessage(string placas)
        {
            switch (_pais)
            {
                case EnumPais.Brasil:
                    return $"! As placas a seguir n�o foram exclu�das pois est�o em outras Composi��es: {placas}";

                case EnumPais.Argentina:
                    return $"! Las patentes siguientes no fueron eliminadas pues est�n relacionadas con otras composiciones {placas}";

                default:
                    return $"! As placas a seguir n�o foram exclu�das pois est�o em outras Composi��es: {placas}";
            }

        }

        private string GetMessageExclusao()
        {
            switch (_pais)
            {
                case EnumPais.Brasil:
                    return "Composi��o exclu�da com sucesso";

                case EnumPais.Argentina:
                    return "Composici�n eliminada con �xito";

                default:
                    return "Composi��o exclu�da com sucesso";
            }
        }

        private void ExcluirHistorico(int? IDComposicao)
        {
            var hist = this.Listar(p => p.ID == IDComposicao);
            foreach (var item in hist)
            {
                if (item.IDComposicao.HasValue)
                    ExcluirHistorico(item.IDComposicao);
                ExcluirComposicao(item.ID, false);
            }
        }

        private void CarregarDados(Composicao composicao, bool comRessalvas)
        {
            var transportadoraBll = new TransportadoraBusiness();
            var placaSetaBll = new PlacaSetaBusiness();
            var placaBll = new PlacaBusiness(_pais);
            var placaDocBll = new PlacaDocumentoBusiness();
            Placa placa1 = null;
            Placa placa2 = null;
            Placa placa3 = null;
            Placa placa4 = null;
            composicao.TaraComposicao = 0;
            composicao.EixosComposicao = 0;
            Composicao composicaoOficial = composicao.IDComposicao.HasValue ? Selecionar(composicao.IDComposicao.Value) : null;

            if (composicao.IDPlaca1 > 0)
            {
                placa1 = CarregarPlaca(placaSetaBll, placaBll, placaDocBll, composicao.IDPlaca1.Value, composicaoOficial?.IDPlaca1 != null ? composicaoOficial.IDPlaca1 : null, comRessalvas);
                composicao.p1 = placa1;

                composicao.isPlaca1Pendente = placa1.Documentos?.Any(w => w.Pendente) ?? false;
                composicao.TaraComposicao += placa1.Tara;
                composicao.EixosComposicao += placa1.NumeroEixos;
                composicao.CategoriaVeiculo = new CategoriaVeiculoBusiness().Selecionar(composicao.IDCategoriaVeiculo).Nome;
            }
            if (composicao.IDPlaca2 > 0)
            {
                placa2 = CarregarPlaca(placaSetaBll, placaBll, placaDocBll, composicao.IDPlaca2.Value, composicaoOficial != null && composicaoOficial.IDPlaca2 != null ? composicaoOficial.IDPlaca2 : null, comRessalvas);
                composicao.p2 = placa2;
                composicao.isPlaca2Pendente = placa2.Documentos?.Any(w => w.Pendente) ?? false;
                composicao.TaraComposicao += placa2.Tara;
                composicao.EixosComposicao += placa2.NumeroEixos;
            }
            if (composicao.IDPlaca3 > 0)
            {
                placa3 = CarregarPlaca(placaSetaBll, placaBll, placaDocBll, composicao.IDPlaca3.Value, composicaoOficial != null && composicaoOficial.IDPlaca3 != null ? composicaoOficial.IDPlaca3 : null, comRessalvas);
                composicao.p3 = placa3;
                composicao.isPlaca3Pendente = placa3.Documentos?.Any(w => w.Pendente) ?? false;
                composicao.TaraComposicao += placa3.Tara;
                composicao.EixosComposicao += placa3.NumeroEixos;
            }
            if (composicao.IDPlaca4 > 0)
            {
                placa4 = CarregarPlaca(placaSetaBll, placaBll, placaDocBll, composicao.IDPlaca4.Value, composicaoOficial != null && composicaoOficial.IDPlaca4 != null ? composicaoOficial.IDPlaca4 : null, comRessalvas);
                composicao.p4 = placa4;
                composicao.isPlaca4Pendente = placa4.Documentos?.Any(w => w.Pendente) ?? false;
                composicao.TaraComposicao += placa4.Tara;
                composicao.EixosComposicao += placa4.NumeroEixos;
            }

            composicao.IsDocumentosPendentes = (composicao.isPlaca1Pendente || composicao.isPlaca2Pendente || composicao.isPlaca3Pendente || composicao.isPlaca4Pendente);
            composicao.QtdCompartimentos = CarregarQuantidadeCompartimentos(composicao);
            var particular = true;

            if (placa1 != null && placa1.IDCategoriaVeiculo == (int)EnumCategoriaVeiculo.Aluguel)
                particular = false;

            if (placa2 != null && placa2.IDCategoriaVeiculo == (int)EnumCategoriaVeiculo.Aluguel)
                particular = false;

            if (placa3 != null && placa3.IDCategoriaVeiculo == (int)EnumCategoriaVeiculo.Aluguel)
                particular = false;
            if (placa4 != null && placa4.IDCategoriaVeiculo == (int)EnumCategoriaVeiculo.Aluguel)
                particular = false;

            string cnpjTransportadora;
            int? idTransportadora = null;

            if (composicao.IDEmpresa == (int)EnumEmpresa.Combustiveis || composicao.IDEmpresa == (int)EnumEmpresa.Ambos)
            {
                cnpjTransportadora = RecuperaCnpjCuit(placa2, placa1);
                idTransportadora = placa2?.IDTransportadora ?? placa1?.IDTransportadora;

                if (!idTransportadora.HasValue)
                {
                    var transp = transportadoraBll.Selecionar(w => w.CNPJCPF == cnpjTransportadora);

                    if (transp != null)
                    {
                        idTransportadora = transp?.ID;
                    }
                }
            }
            else
            {
                cnpjTransportadora = _pais == EnumPais.Brasil ? placa1.PlacaBrasil.CPFCNPJ : placa1.PlacaArgentina.CUIT;
                idTransportadora = placa1.IDTransportadora;
            }
            //CSCUNI-665
            var idTransportadora2 = composicao.IDEmpresa == (int)EnumEmpresa.Ambos ? placa1?.IDTransportadora2 : placa1?.IDTransportadora;

            composicao.IDCategoriaVeiculo = particular ? (int)EnumCategoriaVeiculo.Particular : (int)EnumCategoriaVeiculo.Aluguel;

            Transportadora transportadora, transportadora2;
            if (composicao.IDEmpresa == (int)EnumEmpresa.Combustiveis)
            {
                transportadora = composicao.Operacao == "CIF" ? idTransportadora.HasValue ? transportadoraBll.Selecionar(idTransportadora.Value) : null : cnpjTransportadora != null ? transportadoraBll.Selecionar(w => w.CNPJCPF == cnpjTransportadora && w.Operacao == "FOB" && (_pais != EnumPais.Brasil || w.IBM.StartsWith("T"))) : null;
                transportadora2 = composicao.Operacao == "CIF" ? idTransportadora2.HasValue ? transportadoraBll.Selecionar(idTransportadora2.Value) : null : cnpjTransportadora != null ? transportadoraBll.Selecionar(w => w.CNPJCPF == cnpjTransportadora && w.Operacao == "FOB" && (_pais != EnumPais.Brasil || w.IBM.StartsWith("T"))) : null;
            }
            else
            {
                transportadora = composicao.Operacao == "CIF" ? idTransportadora.HasValue ? transportadoraBll.Selecionar(idTransportadora.Value) : null : cnpjTransportadora != null ? transportadoraBll.Selecionar(w => w.CNPJCPF == cnpjTransportadora && w.Operacao == "FOB") : null;
                transportadora2 = composicao.Operacao == "CIF" ? idTransportadora2.HasValue ? transportadoraBll.Selecionar(idTransportadora2.Value) : null : cnpjTransportadora != null ? transportadoraBll.Selecionar(w => w.CNPJCPF == cnpjTransportadora && w.Operacao == "FOB") : null;
            }

            string cnpj = string.Empty;
            if (composicao.Operacao == "FOB")
            {
                if (!string.IsNullOrEmpty(composicao.CPFCNPJArrendamento))
                    cnpj = composicao.CPFCNPJArrendamento.RemoveCharacter();

                else if (!string.IsNullOrEmpty(composicao.CPFCNPJ))
                    cnpj = composicao.CPFCNPJ.RemoveCharacter();

                if (cnpj != string.Empty)
                    SelecionarTransportadora(composicao, transportadoraBll, cnpj);
                else
                    SelecionarTransportadora(composicao, transportadora, transportadora2);
            }
            else
                SelecionarTransportadora(composicao, transportadora, transportadora2);

            composicao.DataAtualizacao = DateTime.Now;
            composicao.CPFCNPJ = composicao.CPFCNPJ.RemoveCharacter();
            composicao.CPFCNPJArrendamento = composicao.CPFCNPJArrendamento.RemoveCharacter();

            if (string.IsNullOrEmpty(composicao.CPFCNPJ) && cnpjTransportadora != null)
            {
                var transp = transportadoraBll.Selecionar(w => w.CNPJCPF == cnpjTransportadora);

                if (transp != null)
                {
                    composicao.CPFCNPJ = transp.CNPJCPF;
                    composicao.RazaoSocial = transp.RazaoSocial;
                }
            }


            if (composicao.p1 != null)
            {
                if (composicao.p1.Setas != null && composicao.p1.Setas.Any())
                    composicao.Metros = SomarCompartimentos(composicao.p1.Setas.First());
            }

            if (composicao.p2 != null)
            {
                if (composicao.p2 != null && composicao.p2.Setas != null && composicao.p2.Setas.Any())
                    composicao.Metros += SomarCompartimentos(composicao.p2.Setas.First());
            }

            if (composicao.p3 != null)
            {
                if (composicao.p3 != null && composicao.p3.Setas != null && composicao.p3.Setas.Any())
                    composicao.Metros += SomarCompartimentos(composicao.p3.Setas.First());
            }

            if (composicao.p4 != null)
            {
                if (composicao.p4 != null && composicao.p4.Setas != null && composicao.p4.Setas.Any())
                    composicao.Metros += SomarCompartimentos(composicao.p4.Setas.First());
            }

        }

        private static void SelecionarTransportadora(Composicao composicao, Transportadora transportadora, Transportadora transportadora2)
        {
            composicao.IBMTransportadora = transportadora != null ? transportadora.IBM : string.Empty;
            //CSCUNI-665
            if (composicao.IDEmpresa == (int)EnumEmpresa.Ambos)
                composicao.IBMTransportadora2 = transportadora2 != null ? transportadora2.IBM : string.Empty;
        }

        private void SelecionarTransportadora(Composicao composicao, TransportadoraBusiness transportadoraBll, string cnpj)
        {
            Transportadora transp;

            if (composicao.IDEmpresa == (int)EnumEmpresa.Combustiveis)
                transp = transportadoraBll.Selecionar(w => w.CNPJCPF == cnpj && w.IDEmpresa == composicao.IDEmpresa && ((w.Operacao == "FOB" && (_pais != EnumPais.Brasil || w.IBM.StartsWith("T"))) || w.IDEmpresa == (int)EnumEmpresa.EAB));
            else
                transp = transportadoraBll.Selecionar(w => w.CNPJCPF == cnpj && w.IDEmpresa == composicao.IDEmpresa && (w.Operacao == "FOB" || w.IDEmpresa == (int)EnumEmpresa.EAB));

            if (transp != null)
            {
                composicao.IBMTransportadora = transp.IBM;

                //CSCUNI-665
                if (composicao.IDEmpresa == (int)EnumEmpresa.Ambos)
                    composicao.IBMTransportadora2 = transp.IBM;
            }
        }

        public string RecuperaCnpjCuit(Placa placa2, Placa placa1)
        {
            if (_pais == EnumPais.Brasil)
                return placa2 != null ? placa2?.PlacaBrasil.CPFCNPJ : placa1?.PlacaBrasil.CPFCNPJ;

            return placa2 != null ? placa2?.PlacaArgentina.CUIT : placa1?.PlacaArgentina.CUIT;
        }

        private int CarregarQuantidadeCompartimentos(Composicao composicao)
        {
            int qtd = 0;
            if (composicao.p1?.Compartimentos != null)
                qtd = composicao.p1.Compartimentos.Count;
            if (composicao.p2?.Compartimentos != null)
                qtd += composicao.p2.Compartimentos.Count;
            if (composicao.p3?.Compartimentos != null)
                qtd += composicao.p3.Compartimentos.Count;
            if (composicao.p4?.Compartimentos != null)
                qtd += composicao.p4.Compartimentos.Count;
            return qtd;
        }

        private Placa CarregarPlaca(PlacaSetaBusiness placaSetaBll, PlacaBusiness placaBll, PlacaDocumentoBusiness placaDocBll, int idPlaca, int? idPlacaOficial, bool comRessalvas)
        {
            Placa placa = placaBll.Selecionar(idPlaca);
            placa.Setas = placaSetaBll.Listar(w => w.IDPlaca == placa.ID);
            placa.Setas = SomarVolumesCompartimentos(placa.Setas);
            placa.Volume = SelecionarVolumeCompartimento(placa);
            placa.Documentos = placaDocBll.ListarPlacaDocumentoPorPlaca(placa.ID);
            placa.Clientes = new PlacaClienteBusiness().ListarClientesPorPlaca(idPlaca);
            if (placa.Documentos != null && placa.Documentos.Any() && comRessalvas)
            {
                var dias = Config.GetConfig(EnumConfig.SomarDiasRessalvaDocumentos, (int)EnumPais.Padrao);
                if (!string.IsNullOrEmpty(dias))
                    placa.Documentos.ForEach(w => w.DataVencimento = DateTime.Now.AddDays(Convert.ToInt32(dias)).Date);
                else
                    placa.Documentos.ForEach(w => w.DataVencimento = DateTime.Now.AddDays(10).Date);
            }

            if (_pais.Equals(EnumPais.Brasil))
                placa.Uf = new EstadoBusiness().Selecionar((int)placa.PlacaBrasil.IDEstado).Nome;

            placa.Compartimentos = CarregarCompartimentos(placa);
            if (idPlacaOficial.HasValue)
            {
                placa.PlacaAlteracoes = placaBll.ListarAlteracoes(placa.ID, idPlacaOficial.Value);
                placa.PlacaOficial = placaBll.Selecionar(idPlacaOficial.Value);
                placa.PlacaOficial.Setas = placaSetaBll.Listar(w => w.IDPlaca == placa.PlacaOficial.ID);
                placa.PlacaOficial.Setas = SomarVolumesCompartimentos(placa.PlacaOficial.Setas);
                placa.PlacaOficial.Volume = SelecionarVolumeCompartimento(placa.PlacaOficial);
                placa.PlacaOficial.Documentos = placaDocBll.ListarPlacaDocumentoPorPlaca(placa.PlacaOficial.ID);
                placa.PlacaOficial.Compartimentos = CarregarCompartimentos(placa.PlacaOficial);
                //placa.Compartimentos = CarregarAlteracoesCompartimento(placa, placa.PlacaOficial);
                if (placa.PlacaOficial.Documentos != null && placa.Documentos != null)
                    placa.Documentos = placaDocBll.ListarAlteracoes(placa.Documentos, placa.PlacaOficial.Documentos);
            }

            RecuperaPlacasEspecificas(ref placa);

            return placa;
        }

        private void RecuperaPlacasEspecificas(ref Placa placa)
        {
            switch (_pais)
            {
                case EnumPais.Brasil:
                    placa.PlacaBrasil = RecuperaPlacaBrasil(placa.ID);
                    break;
                case EnumPais.Argentina:
                    placa.PlacaArgentina = RecuperaPlacaArgentina(placa.ID);
                    break;
            }
        }

        private PlacaArgentina RecuperaPlacaArgentina(int id)
        {
            return PlacaArgentinaBll.Selecionar(id);
        }

        private PlacaBrasil RecuperaPlacaBrasil(int id)
        {
            return PlacaBrasilBll.Selecionar(id);
        }

        public Stream Exportar(ComposicaoFiltro filtro)
        {
            filtro.IdPais = (int)_pais;

            var lista = new ComposicaoPesquisaBusiness(_pais).ListarComposicaoExportar(filtro);
            Stream fs = new MemoryStream();
            var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add(Traducao.GetTextoPorLingua("Composicao", "Composici�n", _pais));

            int numeroColunas = 0;
            int linha = 2;

            switch (_pais)
            {
                case EnumPais.Brasil:
                    numeroColunas = MontarColunasComposicao(worksheet, filtro.UsuarioExterno, _pais);
                    foreach (var item in lista)
                    {
                        MontarLinhasComposicao(worksheet, linha, item, filtro.UsuarioExterno, _pais);
                        linha++;
                    }
                    break;
                case EnumPais.Argentina:
                    numeroColunas = MontarColunasComposicaoArgentina(worksheet, _pais);
                    foreach (var item in lista)
                    {
                        MontarLinhasComposicaoArgentina(worksheet, linha, item, _pais);
                        linha++;
                    }
                    break;
            }

            using (IXLRange range = worksheet.Range(2, 1, linha - 1, numeroColunas))
            {
                Excel.DesenharBorda(range);
            }

            var placaBll = new PlacaBusiness();
            var listaPlacas = placaBll.ListarPlacaDataTable(filtro);
            var tipoDocs = new TipoDocumentoBusiness().Listar(w => w.tipoCadastro == (int)EnumTipoCadastroDocumento.Veiculo && w.IDPais == (int)_pais);

            var worksheet2 = workbook.Worksheets.Add(Traducao.GetTextoPorLingua("Placa", "Patente", _pais));

            linha = 2;
            switch (_pais)
            {
                case EnumPais.Brasil:
                    MontarColunasPlacas(worksheet2, tipoDocs);
                    foreach (System.Data.DataRow item in listaPlacas.Rows)
                    {
                        MontarLinhasPlacas(worksheet2, linha, item, tipoDocs);
                        linha++;
                    }
                    break;
                case EnumPais.Argentina:
                    MontarColunasPlacasArgentina(worksheet2, tipoDocs);
                    foreach (System.Data.DataRow item in listaPlacas.Rows)
                    {
                        MontarLinhasPlacasArgentina(worksheet2, linha, item, tipoDocs);
                        linha++;
                    }
                    break;
            }

            using (IXLRange range = worksheet2.Range(1, 1, listaPlacas.Rows.Count + 1, 59))
                Excel.DesenharBorda(range);

            workbook.SaveAs(fs, false);
            fs.Position = 0;

            return fs;
        }

        private void MontarLinhasComposicaoArgentina(IXLWorksheet worksheet, int linha, ComposicaoView comp, EnumPais pais)
        {
            worksheet.Cell(linha, 1).Value = comp.Operacao;
            worksheet.Cell(linha, 2).Value = comp.TipoComposicao;
            worksheet.Cell(linha, 3).Value = GetCategoriaVeiculo(comp, pais);
            worksheet.Cell(linha, 4).SetValue<string>(Convert.ToString(comp.Placa1));
            worksheet.Cell(linha, 5).SetValue<string>(Convert.ToString(comp.Placa2));
            worksheet.Cell(linha, 6).SetValue<string>(Convert.ToString(comp.Placa3));
            worksheet.Cell(linha, 7).SetValue<string>(Convert.ToString(comp.Placa4));
            worksheet.Cell(linha, 8).Value = comp.CPFCNPJ;
            worksheet.Cell(linha, 8).SetDataType(XLCellValues.Text);
            worksheet.Cell(linha, 9).Value = comp.RazaoSocial;
            worksheet.Cell(linha, 10).Value = comp.DataAtualizacao.ToShortDateString();
            worksheet.Cell(linha, 11).Value = GetStatusDescription(comp, pais);
            worksheet.Cell(linha, 12).Value = comp.PBTC;
            worksheet.Cell(linha, 13).Value = comp.NumCompartimentos;
            worksheet.Cell(linha, 14).Value = GetBlockedByLanguage(comp.Bloqueado, pais);
            worksheet.Cell(linha, 15).Value = comp.CapacidadeMaxima;

            worksheet.Row(linha).AdjustToContents();
        }

        private int MontarColunasComposicaoArgentina(IXLWorksheet worksheet, EnumPais pais)
        {
            IList<string> nomeColunas = new List<string>();

            nomeColunas.Add(Traducao.GetTextoPorLingua("Opera��o", "Operaci�n", pais));
            nomeColunas.Add(Traducao.GetTextoPorLingua("Tipo de Composi��o", "Tipo de Composici�n", pais));
            nomeColunas.Add(Traducao.GetTextoPorLingua("Categoria do Ve�culo", "Categor�a de Veh�culo", pais));
            nomeColunas.Add(Traducao.GetTextoPorLingua("Placa 1", "Patente 1", pais));
            nomeColunas.Add(Traducao.GetTextoPorLingua("Placa 2", "Patente 2", pais));
            nomeColunas.Add(Traducao.GetTextoPorLingua("Placa 3", "Patente 3", pais));
            nomeColunas.Add(Traducao.GetTextoPorLingua("Placa 4", "Patente 4", pais));
            nomeColunas.Add(Traducao.GetTextoPorLingua("CPF | CNPJ", "CUIT Transportista", pais));
            nomeColunas.Add(Traducao.GetTextoPorLingua("Raz�o Social", "Nombre Transportista", pais));
            nomeColunas.Add(Traducao.GetTextoPorLingua("Data Atualiza��o", "Fecha de Actualizaci�n", pais));
            nomeColunas.Add(Traducao.GetTextoPorLingua("Status", "Estado", pais));
            nomeColunas.Add(Traducao.GetTextoPorLingua("PBTC", "PBTC", pais));
            nomeColunas.Add(Traducao.GetTextoPorLingua("N� de Compartimentos", "N� de Compartimientos", pais));
            nomeColunas.Add(Traducao.GetTextoPorLingua("Bloqueado", "Bloqueado", pais));
            nomeColunas.Add(Traducao.GetTextoPorLingua("Capacidade Max.", "Capacidad M�x.", pais));

            return Excel.PreencheColunas(worksheet, nomeColunas);
        }

        private void MontarColunasPlacas(IXLWorksheet worksheet, List<TipoDocumento> tipoDocs)
        {
            worksheet.Cell(1, 1).Value = Traducao.GetTextoPorLingua("Placa", "Patente", _pais);
            worksheet.Cell(1, 2).Value = Traducao.GetTextoPorLingua("Opera��o", "Operaci�n", _pais);
            worksheet.Cell(1, 3).Value = Traducao.GetTextoPorLingua("TipoVeiculo", "Tipo de veh�culo", _pais);
            worksheet.Cell(1, 4).Value = Traducao.GetTextoPorLingua("Transportadora", "Transportadora", _pais);
            worksheet.Cell(1, 5).Value = "Renavam";
            worksheet.Cell(1, 6).Value = "Marca";
            worksheet.Cell(1, 7).Value = "Modelo";
            worksheet.Cell(1, 8).Value = "Chassi";
            worksheet.Cell(1, 9).Value = "Ano Fabrica��o";
            worksheet.Cell(1, 10).Value = "Ano Modelo";
            worksheet.Cell(1, 11).Value = "Cor";
            worksheet.Cell(1, 12).Value = "Tipo Rastreador";
            worksheet.Cell(1, 13).Value = "N�mero Antena";
            worksheet.Cell(1, 14).Value = "Vers�o";
            worksheet.Cell(1, 15).Value = "C�mera Monitoramento";
            worksheet.Cell(1, 16).Value = "Bomba Descarga";
            worksheet.Cell(1, 17).Value = Traducao.GetTextoPorLingua("N�mero Eixos", "N�mero de Ejes", _pais);
            worksheet.Cell(1, 18).Value = Traducao.GetTextoPorLingua("Eixos Pneus Duplos", "Ejes Neum�ticos Dobles", _pais);
            worksheet.Cell(1, 19).Value = Traducao.GetTextoPorLingua("Numero Eixos Pneus Duplos", "N�mero de Ejes Neum�ticos Dobles", _pais);
            worksheet.Cell(1, 20).Value = Traducao.GetTextoPorLingua("Eixos Distanciados", "Ejes Distanciados", _pais);
            worksheet.Cell(1, 21).Value = Traducao.GetTextoPorLingua("Numero Eixos Distanciados", "", _pais);
            worksheet.Cell(1, 22).Value = Traducao.GetTextoPorLingua("TipoProduto", "Tipo de Producto", _pais);
            worksheet.Cell(1, 23).Value = Traducao.GetTextoPorLingua("MultiSeta", "Multi Flecha", _pais);
            worksheet.Cell(1, 24).Value = Traducao.GetTextoPorLingua("TipoCarregamento", "Tipo de carga", _pais);
            worksheet.Cell(1, 25).Value = Traducao.GetTextoPorLingua("CPFCNPJ", "CUIT", _pais);
            worksheet.Cell(1, 26).Value = Traducao.GetTextoPorLingua("DataNascimento", "Fecha de nacimiento", _pais);
            worksheet.Cell(1, 27).Value = Traducao.GetTextoPorLingua("Raz�o Social", "Raz�n social", _pais);
            worksheet.Cell(1, 28).Value = Traducao.GetTextoPorLingua("Categoria Veiculo", "Categor�a de veh�culo", _pais);
            worksheet.Cell(1, 29).Value = Traducao.GetTextoPorLingua("Data Atualiza��o", "Fecha de actualizaci�n", _pais);
            worksheet.Cell(1, 30).Value = Traducao.GetTextoPorLingua("Observa��o", "Observaci�n", _pais);
            worksheet.Cell(1, 31).Value = Traducao.GetTextoPorLingua("Status", "Estatus", _pais);
            worksheet.Cell(1, 32).Value = "Possui Abs";
            worksheet.Cell(1, 33).Value = "Estado";
            worksheet.Cell(1, 34).Value = Traducao.GetTextoPorLingua("Compartimento 1", "Compartimiento 1", _pais);
            worksheet.Cell(1, 35).Value = Traducao.GetTextoPorLingua("Compartimento 2", "Compartimiento 2", _pais);
            worksheet.Cell(1, 36).Value = Traducao.GetTextoPorLingua("Compartimento 3", "Compartimiento 3", _pais);
            worksheet.Cell(1, 37).Value = Traducao.GetTextoPorLingua("Compartimento 4", "Compartimiento 4", _pais);
            worksheet.Cell(1, 38).Value = Traducao.GetTextoPorLingua("Compartimento 5", "Compartimiento 5", _pais);
            worksheet.Cell(1, 39).Value = Traducao.GetTextoPorLingua("Compartimento 6", "Compartimiento 6", _pais);
            worksheet.Cell(1, 40).Value = Traducao.GetTextoPorLingua("Compartimento 7", "Compartimiento 7", _pais);
            worksheet.Cell(1, 41).Value = Traducao.GetTextoPorLingua("Compartimento 8", "Compartimiento 8", _pais);
            worksheet.Cell(1, 42).Value = Traducao.GetTextoPorLingua("Compartimento 9", "Compartimiento 9", _pais);
            worksheet.Cell(1, 43).Value = Traducao.GetTextoPorLingua("Compartimento 10", "Compartimiento 10", _pais);
            worksheet.Cell(1, 44).Value = "IBM";
            worksheet.Cell(1, 45).Value = Traducao.GetTextoPorLingua("Raz�o Social Cliente", "Raz�n Social del Cliente", _pais);

            for (int i = 0; i < tipoDocs.Count; i++)
            {
                worksheet.Cell(1, i + 46).Value = tipoDocs[i].Sigla;
            }

            using (IXLRange range = worksheet.Range(1, 1, 1, tipoDocs.Count + 45))
            {
                range.Style.Font.Bold = true;
                range.Style.Font.SetFontColor(XLColor.White);
                range.Style.Fill.PatternType = XLFillPatternValues.Solid;
                range.Style.Fill.SetBackgroundColor(XLColor.FromArgb(150, 26, 141)); //Roxo Ra�zen.
                range.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                range.SetAutoFilter();

                Excel.DesenharBorda(range);
            }
        }

        private void MontarColunasPlacasArgentina(IXLWorksheet worksheet, List<TipoDocumento> tipoDocs)
        {
            worksheet.Cell(1, 1).Value = "Patente";
            worksheet.Cell(1, 2).Value = "Operaci�n";
            worksheet.Cell(1, 3).Value = "Tipo de veh�culo";
            worksheet.Cell(1, 4).Value = "Transportista";
            worksheet.Cell(1, 5).Value = "Marca";
            worksheet.Cell(1, 6).Value = "Modelo";
            worksheet.Cell(1, 7).Value = "Chassi";
            worksheet.Cell(1, 8).Value = "Poder";
            worksheet.Cell(1, 9).Value = "N�mero Motor";
            worksheet.Cell(1, 10).Value = "Sistema Satelital - Marca";
            worksheet.Cell(1, 11).Value = "Sistema Satelital - Modelo";
            worksheet.Cell(1, 12).Value = "Sistema Satelital - N� Interno";
            worksheet.Cell(1, 13).Value = "Sistema Satelital - Empresa";
            worksheet.Cell(1, 14).Value = "N�mero de Ejes";
            worksheet.Cell(1, 15).Value = "Ejes Neum�ticos Dobles";
            worksheet.Cell(1, 16).Value = "N�mero de Ejes Neum�ticos Dobles";
            worksheet.Cell(1, 17).Value = "Tipo de Producto";
            worksheet.Cell(1, 18).Value = "Multi Flecha";
            worksheet.Cell(1, 19).Value = "Tipo de carga";
            worksheet.Cell(1, 20).Value = "CUIT";
            worksheet.Cell(1, 21).Value = "Fecha de nacimiento";
            worksheet.Cell(1, 22).Value = "Nombre Transportista";
            worksheet.Cell(1, 23).Value = "Categor�a de veh�culo";
            worksheet.Cell(1, 24).Value = "Fecha de actualizaci�n";
            worksheet.Cell(1, 25).Value = "Observaci�n";
            worksheet.Cell(1, 26).Value = "Estado";
            worksheet.Cell(1, 27).Value = "Compartimiento 1";
            worksheet.Cell(1, 28).Value = "Compartimiento 2";
            worksheet.Cell(1, 29).Value = "Compartimiento 3";
            worksheet.Cell(1, 30).Value = "Compartimiento 4";
            worksheet.Cell(1, 31).Value = "Compartimiento 5";
            worksheet.Cell(1, 32).Value = "Compartimiento 6";
            worksheet.Cell(1, 33).Value = "Compartimiento 7";
            worksheet.Cell(1, 34).Value = "Compartimiento 8";
            worksheet.Cell(1, 35).Value = "Compartimiento 9";
            worksheet.Cell(1, 36).Value = "Compartimiento 10";
            worksheet.Cell(1, 37).Value = "IBM";
            worksheet.Cell(1, 38).Value = "Nombre Cliente";

            for (int i = 0; i < tipoDocs.Count; i++)
            {
                worksheet.Cell(1, i + 39).Value = tipoDocs[i].Sigla;
            }

            using (IXLRange range = worksheet.Range(1, 1, 1, tipoDocs.Count + 38))
            {
                range.Style.Font.Bold = true;
                range.Style.Font.SetFontColor(XLColor.White);
                range.Style.Fill.PatternType = XLFillPatternValues.Solid;
                range.Style.Fill.SetBackgroundColor(XLColor.FromArgb(150, 26, 141)); //Roxo Ra�zen.
                range.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                range.SetAutoFilter();

                Excel.DesenharBorda(range);
            }
        }

        private void MontarLinhasPlacas(IXLWorksheet worksheet, int linha, System.Data.DataRow item, List<TipoDocumento> tipoDocs)
        {
            worksheet.Cell(linha, 1).SetValue<string>(Convert.ToString(item["PlacaVeiculo"]));
            worksheet.Cell(linha, 2).Value = item["Operacao"];
            worksheet.Cell(linha, 3).Value = item["TipoVeiculo"];
            worksheet.Cell(linha, 4).Value = item["Transportadora"];
            worksheet.Cell(linha, 5).Value = item["Renavam"];
            worksheet.Cell(linha, 5).SetDataType(XLCellValues.Text);
            worksheet.Cell(linha, 6).Value = item["Marca"];
            worksheet.Cell(linha, 7).Value = item["Modelo"];
            worksheet.Cell(linha, 8).Value = item["Chassi"];
            worksheet.Cell(linha, 9).Value = item["AnoFabricacao"];
            worksheet.Cell(linha, 10).Value = item["AnoModelo"];
            worksheet.Cell(linha, 11).Value = item["Cor"];
            worksheet.Cell(linha, 12).Value = item["TipoRastreador"];
            worksheet.Cell(linha, 13).Value = item["NumeroAntena"];
            worksheet.Cell(linha, 13).SetDataType(XLCellValues.Text);
            worksheet.Cell(linha, 14).Value = item["Versao"];
            worksheet.Cell(linha, 14).SetDataType(XLCellValues.Text);
            worksheet.Cell(linha, 15).Value = item["CameraMonitoramento"] != null && item["CameraMonitoramento"] != DBNull.Value ? Convert.ToBoolean(item["CameraMonitoramento"]) ? "SIM" : "N�O" : "";
            worksheet.Cell(linha, 16).Value = item["BombaDescarga"] != null && item["BombaDescarga"] != DBNull.Value ? Convert.ToBoolean(item["BombaDescarga"]) ? "SIM" : "N�O" : "";
            worksheet.Cell(linha, 17).Value = item["NumeroEixos"];
            worksheet.Cell(linha, 18).Value = item["EixosPneusDuplos"] != null && item["EixosPneusDuplos"] != DBNull.Value ? Convert.ToBoolean(item["EixosPneusDuplos"]) ? "SIM" : "N�O" : "";
            worksheet.Cell(linha, 19).Value = item["NumeroEixosPneusDuplos"];
            worksheet.Cell(linha, 20).Value = "N�O";
            worksheet.Cell(linha, 21).Value = "";
            worksheet.Cell(linha, 22).Value = item["TipoProduto"];
            worksheet.Cell(linha, 23).Value = item["MultiSeta"] != null && item["MultiSeta"] != DBNull.Value ? Convert.ToBoolean(item["MultiSeta"]) ? "SIM" : "N�O" : "";
            worksheet.Cell(linha, 24).Value = item["TipoCarregamento"];
            worksheet.Cell(linha, 25).Value = item["CPFCNPJ"];
            worksheet.Cell(linha, 25).SetDataType(XLCellValues.Text);
            worksheet.Cell(linha, 26).Value = item["DataNascimento"];
            worksheet.Cell(linha, 27).Value = item["RazaoSocial"];
            worksheet.Cell(linha, 28).Value = item["CategoriaVeiculo"];
            worksheet.Cell(linha, 29).Value = item["DataAtualizacao"];
            worksheet.Cell(linha, 30).Value = item["Observacao"];
            worksheet.Cell(linha, 31).Value = EnumExtensions.GetDescription((EnumStatusComposicao)item["IDStatus"]);
            worksheet.Cell(linha, 32).Value = item["PossuiAbs"] != null && item["PossuiAbs"] != DBNull.Value ? Convert.ToBoolean(item["PossuiAbs"]) ? "SIM" : "N�O" : "";
            worksheet.Cell(linha, 33).Value = item["Estado"];
            worksheet.Cell(linha, 34).Value = item["VolumeCompartimento1"];
            worksheet.Cell(linha, 35).Value = item["VolumeCompartimento2"];
            worksheet.Cell(linha, 36).Value = item["VolumeCompartimento3"];
            worksheet.Cell(linha, 37).Value = item["VolumeCompartimento4"];
            worksheet.Cell(linha, 38).Value = item["VolumeCompartimento5"];
            worksheet.Cell(linha, 39).Value = item["VolumeCompartimento6"];
            worksheet.Cell(linha, 40).Value = item["VolumeCompartimento7"];
            worksheet.Cell(linha, 41).Value = item["VolumeCompartimento8"];
            worksheet.Cell(linha, 42).Value = item["VolumeCompartimento9"];
            worksheet.Cell(linha, 43).Value = item["VolumeCompartimento10"];
            worksheet.Cell(linha, 44).Value = item["IBM"];
            worksheet.Cell(linha, 44).SetDataType(XLCellValues.Text);
            worksheet.Cell(linha, 45).Value = item["RazaoSocialCliente"];

            for (int i = 0; i < tipoDocs.Count; i++)
            {
                worksheet.Cell(linha, 46 + i).Value = item[tipoDocs[i].Sigla == "C.A." ? "CA" : tipoDocs[i].Sigla];
                worksheet.Cell(linha, 46 + i).Value = item[tipoDocs[i].Sigla == "C.A." ? "CA" : tipoDocs[i].Sigla];
            }

            worksheet.Row(linha).AdjustToContents();
        }

        private void MontarLinhasPlacasArgentina(IXLWorksheet worksheet, int linha, System.Data.DataRow item, List<TipoDocumento> tipoDocs)
        {
            var simIdioma = Traducao.GetTextoPorLingua("SIM", "SI", _pais);
            var naoIdioma = Traducao.GetTextoPorLingua("N�O", "NO", _pais);

            worksheet.Cell(linha, 1).SetValue<string>(Convert.ToString(item["PlacaVeiculo"]));
            worksheet.Cell(linha, 2).Value = item["Operacao"];
            worksheet.Cell(linha, 3).Value = item["TipoVeiculo"];
            worksheet.Cell(linha, 4).Value = item["Transportadora"];
            worksheet.Cell(linha, 5).Value = item["Marca"];
            worksheet.Cell(linha, 6).Value = item["Modelo"];
            worksheet.Cell(linha, 7).Value = item["Chassi"];
            worksheet.Cell(linha, 8).Value = item["Potencia"];
            worksheet.Cell(linha, 9).Value = item["NrMotor"];
            worksheet.Cell(linha, 10).Value = item["SatelitalMarca"];
            worksheet.Cell(linha, 11).Value = item["SatelitalModelo"];
            worksheet.Cell(linha, 12).Value = item["SatelitalNrInterno"];
            worksheet.Cell(linha, 13).Value = item["SatelitalEmpresa"];
            worksheet.Cell(linha, 14).Value = item["NumeroEixos"];
            worksheet.Cell(linha, 15).Value = item["EixosPneusDuplos"] != null && item["EixosPneusDuplos"] != DBNull.Value ? Convert.ToBoolean(item["EixosPneusDuplos"]) ? simIdioma : naoIdioma : "";
            worksheet.Cell(linha, 16).Value = item["NumeroEixosPneusDuplos"];
            worksheet.Cell(linha, 17).Value = item["TipoProduto"];
            worksheet.Cell(linha, 18).Value = item["MultiSeta"] != null && item["MultiSeta"] != DBNull.Value ? Convert.ToBoolean(item["MultiSeta"]) ? simIdioma : naoIdioma : "";
            worksheet.Cell(linha, 19).Value = item["IDTipoCarregamento"] != null && item["IDTipoCarregamento"] != DBNull.Value ? EnumExtensions.GetDescription((EnumTipoCarregamentoArgentina)item["IDTipoCarregamento"]) : "";
            worksheet.Cell(linha, 20).Value = _pais == EnumPais.Brasil ? item["CPFCNPJ"] : item["CUIT"];
            worksheet.Cell(linha, 20).SetDataType(XLCellValues.Text);
            worksheet.Cell(linha, 21).Value = item["DataNascimento"];
            worksheet.Cell(linha, 22).Value = item["RazaoSocial"];
            worksheet.Cell(linha, 23).Value = item["CategoriaVeiculoEs"];
            worksheet.Cell(linha, 24).Value = item["DataAtualizacao"];
            worksheet.Cell(linha, 25).Value = item["Observacao"];
            worksheet.Cell(linha, 26).Value = EnumExtensions.GetDescription((EnumStatusComposicaoArg)item["IDStatus"]);
            worksheet.Cell(linha, 27).Value = item["VolumeCompartimento1"];
            worksheet.Cell(linha, 28).Value = item["VolumeCompartimento2"];
            worksheet.Cell(linha, 29).Value = item["VolumeCompartimento3"];
            worksheet.Cell(linha, 30).Value = item["VolumeCompartimento4"];
            worksheet.Cell(linha, 31).Value = item["VolumeCompartimento5"];
            worksheet.Cell(linha, 32).Value = item["VolumeCompartimento6"];
            worksheet.Cell(linha, 33).Value = item["VolumeCompartimento7"];
            worksheet.Cell(linha, 34).Value = item["VolumeCompartimento8"];
            worksheet.Cell(linha, 35).Value = item["VolumeCompartimento9"];
            worksheet.Cell(linha, 36).Value = item["VolumeCompartimento10"];
            worksheet.Cell(linha, 37).Value = item["IBM"];
            worksheet.Cell(linha, 37).SetDataType(XLCellValues.Text);
            worksheet.Cell(linha, 38).Value = item["RazaoSocialCliente"];

            for (int i = 0; i < tipoDocs.Count; i++)
            {
                worksheet.Cell(linha, 39 + i).Value = item[tipoDocs[i].Sigla == "C.A." ? "CA" : tipoDocs[i].Sigla];
                worksheet.Cell(linha, 39 + i).Value = item[tipoDocs[i].Sigla == "C.A." ? "CA" : tipoDocs[i].Sigla];
            }

            worksheet.Row(linha).AdjustToContents();
        }

        private void MontarLinhasComposicao(IXLWorksheet worksheet, int linha, ComposicaoView comp, bool usuarioExterno, EnumPais pais = EnumPais.Brasil)
        {
            worksheet.Cell(linha, 1).Value = comp.EmpresaNome;
            worksheet.Cell(linha, 2).Value = comp.Operacao;
            worksheet.Cell(linha, 3).Value = comp.TipoComposicao;
            worksheet.Cell(linha, 4).Value = comp.TipoComposicaoEixo;
            worksheet.Cell(linha, 5).Value = GetCategoriaVeiculo(comp, pais);
            worksheet.Cell(linha, 6).SetValue<string>(Convert.ToString(comp.Placa1));
            worksheet.Cell(linha, 7).SetValue<string>(Convert.ToString(comp.Placa2));
            worksheet.Cell(linha, 8).SetValue<string>(Convert.ToString(comp.Placa3));
            worksheet.Cell(linha, 9).SetValue<string>(Convert.ToString(comp.Placa4));
            worksheet.Cell(linha, 10).Value = comp.CPFCNPJ;
            worksheet.Cell(linha, 10).SetDataType(XLCellValues.Text);
            if (comp.DataNascimento.HasValue)
                worksheet.Cell(linha, 11).Value = comp.DataNascimento.Value.ToShortDateString();
            worksheet.Cell(linha, 12).Value = comp.RazaoSocial;
            worksheet.Cell(linha, 13).Value = comp.DataAtualizacao.ToShortDateString();
            worksheet.Cell(linha, 14).Value = comp.Observacao;
            worksheet.Cell(linha, 14).SetDataType(XLCellValues.Text);
            worksheet.Cell(linha, 15).Value = comp.CodigoEasyQuery;
            worksheet.Cell(linha, 16).Value = GetStatusDescription(comp, pais);
            worksheet.Cell(linha, 17).Value = comp.CPFCNPJArrendamento;
            worksheet.Cell(linha, 17).SetDataType(XLCellValues.Text);
            worksheet.Cell(linha, 18).Value = comp.RazaoSocialArrendamento;
            worksheet.Cell(linha, 19).Value = comp.LoginUsuario;
            worksheet.Cell(linha, 20).Value = comp.PBTC;
            worksheet.Cell(linha, 21).Value = comp.NumCompartimentos;
            worksheet.Cell(linha, 22).Value = GetBlockedByLanguage(comp.Bloqueado, pais);
            worksheet.Cell(linha, 23).Value = comp.CheckListData;
            worksheet.Cell(linha, 24).Value = GetCheckListApprovedMessage(comp.CheckListAprovado, pais);
            worksheet.Cell(linha, 25).Value = comp.CapacidadeMinima;
            worksheet.Cell(linha, 26).Value = comp.CapacidadeMaxima;

            int numeroColunas = 26;
            if (!usuarioExterno)
            {
                numeroColunas++;
                worksheet.Cell(linha, numeroColunas).Value = comp.UsuarioAlterouStatus;
            }

            worksheet.Row(linha).AdjustToContents();
        }

        private string GetCheckListApprovedMessage(bool? checkListAprovado, EnumPais pais = EnumPais.Brasil)
        {
            switch (pais)
            {
                case EnumPais.Brasil:
                    return checkListAprovado.HasValue ? checkListAprovado.Value ? "SIM" : "N�O" : "";

                case EnumPais.Argentina:
                    return checkListAprovado.HasValue ? checkListAprovado.Value ? "SI" : "NO" : "";

                default:
                    return checkListAprovado.HasValue ? checkListAprovado.Value ? "SIM" : "N�O" : "";
            }
        }

        private string GetBlockedByLanguage(bool blocked, EnumPais pais = EnumPais.Brasil)
        {
            switch (pais)
            {
                case EnumPais.Brasil:
                    return blocked ? "SIM" : "N�O";

                case EnumPais.Argentina:
                    return blocked ? "SI" : "NO";

                default:
                    return blocked ? "SIM" : "N�O";
            }
        }

        private string GetStatusDescription(ComposicaoView comp, EnumPais pais = EnumPais.Brasil)
        {
            switch (pais)
            {
                case EnumPais.Brasil:
                    return ((EnumStatusComposicao)comp.IDStatus).GetDescription();

                case EnumPais.Argentina:
                    return ((EnumStatusComposicaoArg)comp.IDStatus).GetDescription();

                default:
                    return ((EnumStatusComposicao)comp.IDStatus).GetDescription();
            }

        }

        private string GetCategoriaVeiculo(ComposicaoView comp, EnumPais pais = EnumPais.Brasil)
        {
            switch (pais)
            {
                case EnumPais.Brasil:
                    return comp.CategoriaVeiculo;

                case EnumPais.Argentina:
                    return comp.CategoriaVeiculoEs;

                default:
                    return comp.CategoriaVeiculo;
            }
        }

        private static int MontarColunasComposicao(IXLWorksheet worksheet, bool usuarioExterno, EnumPais pais)
        {
            IList<string> nomeColunas = new List<string>();

            nomeColunas.Add(Traducao.GetTextoPorLingua("Linha de Neg�cio", "L�nea de Negocio", pais));
            nomeColunas.Add(Traducao.GetTextoPorLingua("Opera��o", "Operaci�n", pais));
            nomeColunas.Add(Traducao.GetTextoPorLingua("Tipo de Composi��o", "Tipo de Composici�n", pais));
            nomeColunas.Add(Traducao.GetTextoPorLingua("Tipo de Composi��o Eixo", "Composici�n Tipo Eje", pais));
            nomeColunas.Add(Traducao.GetTextoPorLingua("Categoria do Ve�culo", "Categor�a de Veh�culo", pais));
            nomeColunas.Add(Traducao.GetTextoPorLingua("Placa 1", "Patente 1", pais));
            nomeColunas.Add(Traducao.GetTextoPorLingua("Placa 2", "Patente 2", pais));
            nomeColunas.Add(Traducao.GetTextoPorLingua("Placa 3", "Patente 3", pais));
            nomeColunas.Add(Traducao.GetTextoPorLingua("Placa 4", "Patente 4", pais));
            nomeColunas.Add(Traducao.GetTextoPorLingua("CPF | CNPJ", "CUIT", pais));
            nomeColunas.Add(Traducao.GetTextoPorLingua("Data Nascimento", "Fecha de Nacimiento", pais));
            nomeColunas.Add(Traducao.GetTextoPorLingua("Raz�o Social", "Raz�n Social", pais));
            nomeColunas.Add(Traducao.GetTextoPorLingua("Data Atualiza��o", "Fecha de Actualizaci�n", pais));
            nomeColunas.Add(Traducao.GetTextoPorLingua("Observa��o", "Observaci�n", pais));
            nomeColunas.Add(Traducao.GetTextoPorLingua("Numero do Chamado", "N�mero de solicitud", pais));
            nomeColunas.Add(Traducao.GetTextoPorLingua("Status", "Estatus", pais));
            nomeColunas.Add(Traducao.GetTextoPorLingua("CPF | CNPJ Arrendamento", "CUIT de Arrendamiento", pais));
            nomeColunas.Add(Traducao.GetTextoPorLingua("Raz�o Social Arrendamento", "Nombre de la empresa Alquiler", pais));
            nomeColunas.Add(Traducao.GetTextoPorLingua("Usu�rio", "Usuario", pais));
            nomeColunas.Add(Traducao.GetTextoPorLingua("PBTC", "PBTC", pais));
            nomeColunas.Add(Traducao.GetTextoPorLingua("N� de Compartimentos", "N� de Compartimientos", pais));
            nomeColunas.Add(Traducao.GetTextoPorLingua("Bloqueado", "Bloqueado", pais));
            nomeColunas.Add(Traducao.GetTextoPorLingua("Data Check List", "Lista de verificaci�n de datos", pais));
            nomeColunas.Add(Traducao.GetTextoPorLingua("Check List Aprovado", "Lista de verificaci�n Aprobada", pais));
            nomeColunas.Add(Traducao.GetTextoPorLingua("Capacidade Min.", "Capacidad M�n.", pais));
            nomeColunas.Add(Traducao.GetTextoPorLingua("Capacidade Max.", "Capacidad M�x.", pais));

            if (!usuarioExterno)
            {
                nomeColunas.Add(Traducao.GetTextoPorLingua("�ltima Altera��o do Status(Usu�rio)", "�ltimo cambio de Estado (Usuario)", pais));
            }

            return Excel.PreencheColunas(worksheet, nomeColunas);
        }

        private List<CompartimentoView> CarregarCompartimentos(Placa placa)
        {
            if (placa == null || placa.Setas == null)
                return null;

            List<CompartimentoView> compartimentos = new List<CompartimentoView>();
            for (int i = 1; i <= 10; i++)
            {
                int s = 1;
                var comp = new CompartimentoView { setas = new List<SetaView>() };
                foreach (var seta in placa.Setas.OrderBy(o => o.ID))
                {
                    SetaView set;
                    switch (i)
                    {

                        case 1:
                            if (seta.VolumeCompartimento1.HasValue && ((seta.Compartimento1IsInativo.HasValue && !seta.Compartimento1IsInativo.Value) || !seta.Compartimento1IsInativo.HasValue))
                            {
                                comp.seq = i;
                                set = new SetaView
                                {
                                    seq = s,
                                    Volume = seta.VolumeCompartimento1.Value
                                };
                                if (seta.LacreCompartimento1.HasValue)
                                    set.Lacres = seta.LacreCompartimento1.Value;
                                set.Principal = seta.CompartimentoPrincipal1 ?? false;
                                comp.setas.Add(set);
                            }
                            break;
                        case 2:
                            if (seta.VolumeCompartimento2.HasValue && ((seta.Compartimento2IsInativo.HasValue && !seta.Compartimento2IsInativo.Value) || !seta.Compartimento2IsInativo.HasValue))
                            {
                                comp.seq = i;
                                set = new SetaView
                                {
                                    seq = s,
                                    Volume = seta.VolumeCompartimento2.Value
                                };
                                if (seta.LacreCompartimento2.HasValue)
                                    set.Lacres = seta.LacreCompartimento2.Value;
                                set.Principal = seta.CompartimentoPrincipal2 ?? false;
                                comp.setas.Add(set);
                            }
                            break;
                        case 3:
                            if (seta.VolumeCompartimento3.HasValue && ((seta.Compartimento3IsInativo.HasValue && !seta.Compartimento3IsInativo.Value) || !seta.Compartimento3IsInativo.HasValue))
                            {
                                comp.seq = i;
                                set = new SetaView
                                {
                                    seq = s,
                                    Volume = seta.VolumeCompartimento3.Value
                                };
                                if (seta.LacreCompartimento3.HasValue)
                                    set.Lacres = seta.LacreCompartimento3.Value;
                                set.Principal = seta.CompartimentoPrincipal3 ?? false;
                                comp.setas.Add(set);
                            }
                            break;
                        case 4:
                            if (seta.VolumeCompartimento4.HasValue && ((seta.Compartimento4IsInativo.HasValue && !seta.Compartimento4IsInativo.Value) || !seta.Compartimento4IsInativo.HasValue))
                            {
                                comp.seq = i;
                                set = new SetaView
                                {
                                    seq = s,
                                    Volume = seta.VolumeCompartimento4.Value
                                };
                                if (seta.LacreCompartimento4.HasValue)
                                    set.Lacres = seta.LacreCompartimento4.Value;
                                set.Principal = seta.CompartimentoPrincipal4 ?? false;
                                comp.setas.Add(set);
                            }
                            break;
                        case 5:
                            if (seta.VolumeCompartimento5.HasValue && ((seta.Compartimento5IsInativo.HasValue && !seta.Compartimento5IsInativo.Value) || !seta.Compartimento5IsInativo.HasValue))
                            {
                                comp.seq = i;
                                set = new SetaView
                                {
                                    seq = s,
                                    Volume = seta.VolumeCompartimento5.Value
                                };
                                if (seta.LacreCompartimento5.HasValue)
                                    set.Lacres = seta.LacreCompartimento5.Value;
                                set.Principal = seta.CompartimentoPrincipal5 ?? false;
                                comp.setas.Add(set);
                            }
                            break;
                        case 6:
                            if (seta.VolumeCompartimento6.HasValue && ((seta.Compartimento6IsInativo.HasValue && !seta.Compartimento6IsInativo.Value) || !seta.Compartimento6IsInativo.HasValue))
                            {
                                comp.seq = i;
                                set = new SetaView
                                {
                                    seq = s,
                                    Volume = seta.VolumeCompartimento6.Value
                                };
                                if (seta.LacreCompartimento6.HasValue)
                                    set.Lacres = seta.LacreCompartimento6.Value;
                                set.Principal = seta.CompartimentoPrincipal6 ?? false;
                                comp.setas.Add(set);
                            }
                            break;
                        case 7:
                            if (seta.VolumeCompartimento7.HasValue && ((seta.Compartimento7IsInativo.HasValue && !seta.Compartimento7IsInativo.Value) || !seta.Compartimento7IsInativo.HasValue))
                            {
                                comp.seq = i;
                                set = new SetaView
                                {
                                    seq = s,
                                    Volume = seta.VolumeCompartimento7.Value
                                };
                                if (seta.LacreCompartimento7.HasValue)
                                    set.Lacres = seta.LacreCompartimento7.Value;
                                set.Principal = seta.CompartimentoPrincipal7 ?? false;
                                comp.setas.Add(set);
                            }
                            break;
                        case 8:
                            if (seta.VolumeCompartimento8.HasValue && ((seta.Compartimento8IsInativo.HasValue && !seta.Compartimento8IsInativo.Value) || !seta.Compartimento8IsInativo.HasValue))
                            {
                                comp.seq = i;
                                set = new SetaView
                                {
                                    seq = s,
                                    Volume = seta.VolumeCompartimento8.Value
                                };
                                if (seta.LacreCompartimento8.HasValue)
                                    set.Lacres = seta.LacreCompartimento8.Value;
                                set.Principal = seta.CompartimentoPrincipal8 ?? false;
                                comp.setas.Add(set);
                            }
                            break;
                        case 9:
                            if (seta.VolumeCompartimento9.HasValue && ((seta.Compartimento9IsInativo.HasValue && !seta.Compartimento9IsInativo.Value) || !seta.Compartimento9IsInativo.HasValue))
                            {
                                comp.seq = i;
                                set = new SetaView
                                {
                                    seq = s,
                                    Volume = seta.VolumeCompartimento9.Value
                                };
                                if (seta.LacreCompartimento9.HasValue)
                                    set.Lacres = seta.LacreCompartimento9.Value;
                                set.Principal = seta.CompartimentoPrincipal9 ?? false;
                                comp.setas.Add(set);
                            }
                            break;
                        case 10:
                            if (seta.VolumeCompartimento10.HasValue && ((seta.Compartimento10IsInativo.HasValue && !seta.Compartimento10IsInativo.Value) || !seta.Compartimento10IsInativo.HasValue))
                            {
                                comp.seq = i;
                                set = new SetaView
                                {
                                    seq = s,
                                    Volume = seta.VolumeCompartimento10.Value
                                };
                                if (seta.LacreCompartimento10.HasValue)
                                    set.Lacres = seta.LacreCompartimento10.Value;
                                set.Principal = seta.CompartimentoPrincipal10 ?? false;
                                comp.setas.Add(set);
                            }
                            break;
                    }
                    s++;
                }
                if (comp.setas.Any())
                    compartimentos.Add(comp);

            }

            return compartimentos;
        }

        private List<PlacaSeta> SomarVolumesCompartimentos(List<PlacaSeta> setas)
        {
            if (setas != null)
            {
                foreach (var seta in setas)
                {
                    //verificar se o volume tem valor e se n�o est� inativo
                    seta.VolumeTotalCompartimento =
                        (seta.VolumeCompartimento1.HasValue ?
                            seta.Compartimento1IsInativo.HasValue && seta.Compartimento1IsInativo.Value ?
                            0 : seta.VolumeCompartimento1.Value : 0)
                        + (seta.VolumeCompartimento2.HasValue ?
                            seta.Compartimento2IsInativo.HasValue && seta.Compartimento2IsInativo.Value ?
                            0 : seta.VolumeCompartimento2.Value : 0)
                        + (seta.VolumeCompartimento3.HasValue ?
                            seta.Compartimento3IsInativo.HasValue && seta.Compartimento3IsInativo.Value ?
                            0 : seta.VolumeCompartimento3.Value : 0)
                        + (seta.VolumeCompartimento4.HasValue ?
                            seta.Compartimento4IsInativo.HasValue && seta.Compartimento4IsInativo.Value ?
                            0 : seta.VolumeCompartimento4.Value : 0)
                        + (seta.VolumeCompartimento5.HasValue ?
                            seta.Compartimento5IsInativo.HasValue && seta.Compartimento5IsInativo.Value ?
                            0 : seta.VolumeCompartimento5.Value : 0)
                        + (seta.VolumeCompartimento6.HasValue ?
                            seta.Compartimento6IsInativo.HasValue && seta.Compartimento6IsInativo.Value ?
                            0 : seta.VolumeCompartimento6.Value : 0)
                        + (seta.VolumeCompartimento7.HasValue ?
                            seta.Compartimento7IsInativo.HasValue && seta.Compartimento7IsInativo.Value ?
                            0 : seta.VolumeCompartimento7.Value : 0)
                        + (seta.VolumeCompartimento8.HasValue ?
                            seta.Compartimento8IsInativo.HasValue && seta.Compartimento8IsInativo.Value ?
                            0 : seta.VolumeCompartimento8.Value : 0)
                        + (seta.VolumeCompartimento9.HasValue ?
                            seta.Compartimento9IsInativo.HasValue && seta.Compartimento9IsInativo.Value ?
                            0 : seta.VolumeCompartimento9.Value : 0)
                        + (seta.VolumeCompartimento10.HasValue ?
                            seta.Compartimento10IsInativo.HasValue && seta.Compartimento10IsInativo.Value ?
                            0 : seta.VolumeCompartimento10.Value : 0);
                }
                return setas;
            }

            return null;
        }

        private decimal SelecionarVolumeCompartimento(Placa placa)
        {
            if (placa != null)
            {
                decimal volume = 0;
                if (placa.MultiSeta)
                {
                    foreach (var item in placa.Setas)
                    {
                        volume += SomarCompartimentos(item);
                    }
                }
                else
                {
                    volume = placa.Setas
                                    .Sum(s =>
                                          (s.VolumeCompartimento1 ?? 0)
                                        + (s.VolumeCompartimento2 ?? 0)
                                        + (s.VolumeCompartimento3 ?? 0)
                                        + (s.VolumeCompartimento4 ?? 0)
                                        + (s.VolumeCompartimento5 ?? 0)
                                        + (s.VolumeCompartimento6 ?? 0)
                                        + (s.VolumeCompartimento7 ?? 0)
                                        + (s.VolumeCompartimento8 ?? 0)
                                        + (s.VolumeCompartimento9 ?? 0)
                                        + (s.VolumeCompartimento10 ?? 0));
                }
                return volume;
            }

            return 0;

        }

        private static decimal SomarCompartimentos(PlacaSeta item)
        {
            decimal volume = 0;
            volume += item.CompartimentoPrincipal1.HasValue && item.CompartimentoPrincipal1.Value ?
                item.Compartimento1IsInativo.HasValue && item.Compartimento1IsInativo.Value ? 0 : item.VolumeCompartimento1 ?? 0 : 0;
            volume += item.CompartimentoPrincipal2.HasValue && item.CompartimentoPrincipal2.Value ?
                item.Compartimento2IsInativo.HasValue && item.Compartimento2IsInativo.Value ? 0 : item.VolumeCompartimento2 ?? 0 : 0;
            volume += item.CompartimentoPrincipal3.HasValue && item.CompartimentoPrincipal3.Value ?
                item.Compartimento3IsInativo.HasValue && item.Compartimento3IsInativo.Value ? 0 : item.VolumeCompartimento3 ?? 0 : 0;
            volume += item.CompartimentoPrincipal4.HasValue && item.CompartimentoPrincipal4.Value ?
                item.Compartimento4IsInativo.HasValue && item.Compartimento4IsInativo.Value ? 0 : item.VolumeCompartimento4 ?? 0 : 0;
            volume += item.CompartimentoPrincipal5.HasValue && item.CompartimentoPrincipal5.Value ?
                item.Compartimento5IsInativo.HasValue && item.Compartimento5IsInativo.Value ? 0 : item.VolumeCompartimento5 ?? 0 : 0;
            volume += item.CompartimentoPrincipal6.HasValue && item.CompartimentoPrincipal6.Value ?
                item.Compartimento6IsInativo.HasValue && item.Compartimento6IsInativo.Value ? 0 : item.VolumeCompartimento6 ?? 0 : 0;
            volume += item.CompartimentoPrincipal7.HasValue && item.CompartimentoPrincipal7.Value ?
                item.Compartimento7IsInativo.HasValue && item.Compartimento7IsInativo.Value ? 0 : item.VolumeCompartimento7 ?? 0 : 0;
            volume += item.CompartimentoPrincipal8.HasValue && item.CompartimentoPrincipal8.Value ?
                item.Compartimento8IsInativo.HasValue && item.Compartimento8IsInativo.Value ? 0 : item.VolumeCompartimento8 ?? 0 : 0;
            volume += item.CompartimentoPrincipal9.HasValue && item.CompartimentoPrincipal9.Value ?
                item.Compartimento9IsInativo.HasValue && item.Compartimento9IsInativo.Value ? 0 : item.VolumeCompartimento9 ?? 0 : 0;
            volume += item.CompartimentoPrincipal10.HasValue && item.CompartimentoPrincipal10.Value ?
                item.Compartimento10IsInativo.HasValue && item.Compartimento10IsInativo.Value ? 0 : item.VolumeCompartimento10 ?? 0 : 0;

            return volume;
        }

        private string IntegrarSAP(Composicao composicao, EnumTipoIntegracaoSAP tipoIntegracao)
        {
            switch (composicao.IDEmpresa)
            {
                case (int)EnumEmpresa.EAB:
                    composicao.chaveSAPEAB = composicao.p1.PlacaVeiculo;
                    break;
                case (int)EnumEmpresa.Combustiveis:
                    composicao.chaveSAPCOMB = composicao.p2 != null ? composicao.p2.PlacaVeiculo : composicao.p1.PlacaVeiculo;
                    break;
                case (int)EnumEmpresa.Ambos:
                    composicao.chaveSAPCOMB = composicao.p2 != null ? composicao.p2.PlacaVeiculo : composicao.p1.PlacaVeiculo;
                    composicao.chaveSAPEAB = composicao.p1.PlacaVeiculo;
                    break;
            }

            string retorno = "";

            if (composicao.IDEmpresa == (int)EnumEmpresa.Combustiveis)
            {
                switch (_pais)
                {
                    case EnumPais.Brasil:
                        var integraSap = new WsIntegraSAP();
                        retorno = integraSap.IntegrarComposicao(composicao, tipoIntegracao);
                        break;

                    case EnumPais.Argentina:
                        var integraSapAr_Veiculo = new WsIntegraSAPAR_Veiculo();
                        retorno = integraSapAr_Veiculo.IntegrarComposicao(composicao, tipoIntegracao);
                        break;
                }
            }

            else if (composicao.IDEmpresa == (int)EnumEmpresa.EAB && _pais == EnumPais.Brasil)
            {
                var integraSap = new WsIntegraSAPEAB();
                retorno = integraSap.IntegrarComposicao(composicao, tipoIntegracao);
            }
            else if (_pais == EnumPais.Brasil) //ambas
            {
                var integraSapeab = new WsIntegraSAPEAB();
                retorno = " " + integraSapeab.IntegrarComposicao(composicao, tipoIntegracao);
                //verifica se o retorno est� em branco (ok) e no caso de for checklist, se foi selecionada a op��o de integrar EAB 
                if (string.IsNullOrEmpty(retorno.Trim())
                    && ((composicao.checkList != null && composicao.checkList.isReplicarEab) || composicao.checkList == null))
                {
                    var integraSap = new WsIntegraSAP();
                    retorno += integraSap.IntegrarComposicao(composicao, tipoIntegracao);
                }
            }

            return retorno;
        }

        private static void AbrirChamadoEasyQuery(Composicao composicao, int idPais)
        {
            StringBuilder descricao = new StringBuilder();
            descricao.AppendLine("Placa do Cavalo: " + composicao.p1.PlacaVeiculo);
            descricao.AppendLine("Placa da Carreta 1: " + composicao.p2?.PlacaVeiculo);
            descricao.AppendLine("Placa da Carreta 2: " + composicao.p3?.PlacaVeiculo);
            descricao.AppendLine("Placa da Carreta 3: " + composicao.p4?.PlacaVeiculo);
            descricao.AppendLine("N�mero de eixos (composi��o): " + composicao.EixosComposicao);
            descricao.AppendLine("Quantidade de eixos com pneu duplo (composi��o): " + composicao.EixosPneusDuplos);
            descricao.AppendLine("Volume do Conjunto (composi��o): " + decimal.Truncate(composicao.VolumeComposicao));
            descricao.AppendLine("Compartimenta��o da carreta 1: " + decimal.Truncate(composicao.p1.Volume));
            descricao.AppendLine("Compartimenta��o da carreta 2 (Composi��es tipo Carreta): " + (composicao.p2 != null ? decimal.Truncate(composicao.p2.Volume).ToString(CultureInfo.InvariantCulture) : ""));
            descricao.AppendLine("Compartimenta��o da carreta 3 (Composi��es tipo Bitrem): " + (composicao.p3 != null ? decimal.Truncate(composicao.p3.Volume).ToString(CultureInfo.InvariantCulture) : ""));
            descricao.AppendLine("Compartimenta��o da carreta 4 (Composi��es tipo Bitrem + Dolly): " + (composicao.p4 != null ? decimal.Truncate(composicao.p4.Volume).ToString(CultureInfo.InvariantCulture) : ""));
            descricao.AppendLine("IBM � C�digo de IBMs replicados: ");
            descricao.AppendLine("Observa��o: " + composicao.Observacao);

            var easy = new EasyQueryView
            {
                Description = descricao.ToString(),
                ContactEmail = composicao.EmailSolicitante,
                Subject = Config.GetConfig(EnumConfig.TituloChamadoEasyQueryComposicao, idPais),
                idSubcategoria = Convert.ToInt32(Config.GetConfig(EnumConfig.SubCategoryId, idPais)),
                ResolutionGroupID = Convert.ToInt32(Config.GetConfig(EnumConfig.ResolutionGroupId, idPais))
            };
            var codigoEasyQuery = new EasyQueryBusiness().CriarNovoTicket(easy);
            composicao.CodigoEasyQuery = codigoEasyQuery;
            composicao.CodigoSalesForce = null;
        }
		public static bool ValidarEncerrarChamadoSalesForce(Composicao composicao)
		{
			var salesforce = Config.GetConfigInt(EnumConfig.SalesForce, (int)composicao.IdPais);

			if (salesforce != 0 && !string.IsNullOrEmpty(composicao.CodigoSalesForce))
			{
				return true;
			}

			return false;
		}

		private void EncerrarChamadoSalesForce(Composicao composicao, bool comRessalvas)
		{
			var urlSalesforce = ConfigurationManager.AppSettings["LinkIntegracaoSalesForce"];
			var clientIdSalesforce = ConfigurationManager.AppSettings["clientIdSalesForce"];
			var clientSecretSalesforce = ConfigurationManager.AppSettings["clientSecretSalesForce"];

			new WsSalesForce(urlSalesforce, clientIdSalesforce, clientSecretSalesforce)
				.EncerrarTicket(MontarDadosEncerrarTicketSalesForce(composicao, comRessalvas, (int)composicao.IdPais));
		}
		private SalesForceEncerrarTicketView MontarDadosEncerrarTicketSalesForce(Composicao composicao, bool comRessalvas, int idPais)
		{
			string approval = String.Empty;
			string justificativa = String.IsNullOrEmpty(composicao.Justificativa) ? String.Empty : composicao.Justificativa.Trim().Trim(Environment.NewLine.ToCharArray());

			switch (composicao.IDStatus)
			{
				case (int)EnumStatusComposicao.Aprovado:
					if (String.IsNullOrEmpty(justificativa))
					{
						justificativa = "Ol�! Seu cadastro foi aprovado com sucesso. Att. Cadastro";
					}

					if (comRessalvas)
						approval = "Aprovado com ressalvas";
					else
						approval = "Aprovado";
					break;
				case (int)EnumStatusMotorista.Reprovado:
					approval = "Reprovado";
					break;
			}

			var informacoesUsuario = UserSession.GetCurrentInfoUserSystem().InformacoesUsuario;

			var sf = new SalesForceEncerrarTicketView
			{
				Ticket = composicao.CodigoSalesForce,
				UserCS = informacoesUsuario.Login.RemoverZerosAEsquerda(),
				UserName = informacoesUsuario.Nome,
				Approval = approval,
				Status = "Encerrado",
				CustomerResponse = justificativa,
			};

			return sf;
		}

		public static void AbrirChamadoSalesForce(Composicao composicao, int idPais, bool aprovadoAutomaticamente = false)
        {
            var usuarios = new UsuarioBusiness().Listar(x => x.Login == composicao.LoginUsuario);

            var sf = new SalesForceCriarNovoTicketView
            {
                Subject = MontarAssuntoTicketSalesForce(composicao, (EnumPais)idPais, aprovadoAutomaticamente),
                Description = MontarDescricaoTicketSalesForce(composicao),
                Profile = usuarios.FirstOrDefault()?.Perfil,
                User = composicao.LoginUsuario?.RemoverZerosAEsquerda()
            };

            var tipoEmpresa = composicao.IDEmpresa == (int)EnumEmpresa.Combustiveis ? "Comb" : "EAB";

            var config = $"IDSFComp{tipoEmpresa}{composicao.Operacao}";

            EnumConfig key;
            if (Enum.TryParse(config, out key))
            {
                sf.CategoriaAtendimentoId = Config.GetConfig(key, idPais);
            }

            var urlSalesforce = ConfigurationManager.AppSettings.Get("LinkIntegracaoSalesForce");

            var clientIdSalesforce = ConfigurationManager.AppSettings.Get("clientIdSalesForce");
            var clientSecretSalesforce = ConfigurationManager.AppSettings.Get("clientSecretSalesForce");

            try
            {
                var ticket = new WsSalesForce(urlSalesforce, clientIdSalesforce, clientSecretSalesforce).CriarNovoTicket(sf);
                composicao.CodigoSalesForce = ticket;
                composicao.CodigoEasyQuery = null;

                if (string.IsNullOrEmpty(ticket))
                {
                    throw new Exception("Token vazio.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("\nMessage ---\n{0}", ex.Message);
                Console.WriteLine(
                    "\nHelpLink ---\n{0}", ex.HelpLink);
                Console.WriteLine("\nSource ---\n{0}", ex.Source);
                Console.WriteLine(
                    "\nStackTrace ---\n{0}", ex.StackTrace);
                Console.WriteLine(
                    "\nTargetSite ---\n{0}", ex.TargetSite);

                LogUtil.GravarLog("AbrirChamadoSalesForce", string.Format("{0} - Executado com erro!", "AbrirChamadoSalesForce"), ex.Message, composicao.LoginUsuarioCorrente);
            }


        }

        private static string MontarAssuntoTicketSalesForce(Composicao composicao, EnumPais pais, bool aprovadoAutomaticamente)
        {
            var negocio = EnumExtensions.GetDescription((EnumEmpresa)composicao.IDEmpresa);
            var nomePais = pais == EnumPais.Argentina ? "ARG" : "BR";
            var tipo = aprovadoAutomaticamente ? "Réplica" : "Cadastro";
			return $"{tipo} {negocio} Composição {composicao.Operacao} - {nomePais}";
        }

        private static string MontarDescricaoTicketSalesForce(Composicao composicao)
        {
            return new StringBuilder()
                .AppendLine($"Placa 1: {composicao.Placa1}")
                .AppendLine($"Placa 2: {composicao.Placa2}")
                .AppendLine($"Placa 3: {composicao.Placa3}")
                .AppendLine($"Placa 4: {composicao.Placa4}")
                .AppendLine($"Link UNICAD: https://apps.raizen.com/unicad/Composicao/Aprovar/{composicao.ID}")
                .ToString();
        }

        public static bool ValidarAbrirChamadoSalesForce(Composicao composicao, int idPais, bool aprovacaoAutomatica = false)
        {
            var salesforce = Config.GetConfigInt(EnumConfig.SalesForce, idPais);

            if (composicao.IDStatus != (int)EnumStatusComposicao.EmAprovacao && !aprovacaoAutomatica)
            {
                return false;
            }

            if (salesforce == 0 || !ValidarUsuarioExterno(composicao))
            {
                return false;
            }

            var tipoEmpresa = composicao.IDEmpresa == (int)EnumEmpresa.EAB ? "Eab" : "Comb";
            var config = $"SfComp{tipoEmpresa}{composicao.Operacao}";

            EnumConfig key;
            if (!Enum.TryParse(config, true, out key))
            {
                return true;
            }

            return Config.GetConfigInt(key, idPais) != 0;
        }

        private static bool ValidarUsuarioExterno(Composicao composicao)
        {
            if (string.IsNullOrEmpty(composicao.LoginUsuarioCorrente))
            {
                return true;
            }

            var usuario = new UsuarioBusiness().Selecionar(w => w.Login == composicao.LoginUsuarioCorrente);

            return usuario.Externo;
        }

        private static bool ValidarAbrirChamadoEasyQuery(Composicao composicao, int idPais)
        {
            var easy = Config.GetConfigInt(EnumConfig.EasyQuery, idPais);

            if (easy == 0 || !ValidarUsuarioExterno(composicao))
            {
                return false;
            }

            switch (composicao.IDEmpresa)
            {
                case (int)EnumEmpresa.EAB:
                    switch (composicao.Operacao)
                    {
                        case "CIF":

                            return Config.GetConfigInt(EnumConfig.EqCompEabCif, idPais) != 0;
                        case "FOB":
                            return Config.GetConfigInt(EnumConfig.EqCompEabFob, idPais) != 0;
                    }

                    break;
                case (int)EnumEmpresa.Combustiveis:
                    switch (composicao.Operacao)
                    {
                        case "CIF":
                            return Config.GetConfigInt(EnumConfig.EqCompCombCif, idPais) != 0;
                        case "FOB":
                            return Config.GetConfigInt(EnumConfig.EqCompCombFob, idPais) != 0;
                    }
                    break;
                case (int)EnumEmpresa.Ambos:
                    switch (composicao.Operacao)
                    {
                        case "CIF":
                            return Config.GetConfigInt(EnumConfig.EqCompAmbasCif, idPais) != 0;
                        case "FOB":
                            return Config.GetConfigInt(EnumConfig.EqCompAmbasFob, idPais) != 0;
                    }
                    break;
            }
            return true;
        }

        public bool AtualizarComposicao(Composicao Composicao, bool comRessalvas, bool bloqueio = false, bool enviaEmail = true, int idStatus = 0, bool aprovacaoAutomatica = false)
        {
            using (TransactionScope transactionScope = Transactions.CreateTransactionScope(TransactionScopeOption.Required, IsolationLevel.ReadCommitted, 1))
            {
                //Atualizar o status
                CarregarDados(Composicao, comRessalvas);

                //verificar se a composicao antiga ainda existe, pois ela pode ter sido excluida
                if (Composicao.IDComposicao.HasValue)
                {
                    var compAnterior = Selecionar(Composicao.IDComposicao.Value);
                    if (compAnterior == null)
                        Composicao.IDComposicao = null;
                }

                Composicao.IdPais = (int)_pais;
                Composicao.PBTC = Composicao.PBTC == null ? 0 : Composicao.PBTC;

                Atualizar(Composicao);
                bool placaUsada = false;
                var usuario = new UsuarioBusiness().Selecionar(x => x.Login == Composicao.LoginUsuario);
                int idCliente = 0;
                if (usuario != null)
                {
                    if (Composicao.Operacao == "FOB")
                    {

                        var c = new UsuarioClienteBusiness().Selecionar(x => x.IDUsuario == usuario.ID);
                        if (c != null)
                            idCliente = c.IDCliente;
                    }
                    else
                    {
                        var c = new UsuarioTransportadoraBusiness().Selecionar(x => x.IDUsuario == usuario.ID);
                        if (c != null)
                            idCliente = c.IDTransportadora;
                    }
                }

                //Verificar se est� usando a placa chave (truck ou carreta 1), caso sim, dever� passar os clientes dessa placa para toda composi��o CSCUNI-412
                if (Composicao.IDEmpresa == (int)EnumEmpresa.Combustiveis)
                {
                    placaUsada = new PlacaBusiness().VerificarPlacaJaUsada(Composicao.Placa2 != null ? Composicao.Placa2 : Composicao.Placa1, Composicao.Operacao, Composicao.IDEmpresa, Composicao.IDComposicao, (EnumTipoComposicao)Composicao.IDTipoComposicao, idCliente, 2);
                }

                //passar os clientes de uma placa para outra no caso de FOB
                if (Composicao.IDComposicao.HasValue && Composicao.isUtilizaPlacaChave.HasValue && Composicao.isUtilizaPlacaChave.Value && Composicao.Operacao == "FOB" && !bloqueio)
                {
                    Composicao compAtenrior = this.Selecionar(Composicao.IDComposicao.Value);
                    Placa placaChave;
                    if (Composicao.IDEmpresa == (int)EnumEmpresa.EAB)
                    {
                        placaChave = new PlacaBusiness().Selecionar(compAtenrior.IDPlaca1.Value);
                        placaChave.Clientes = new PlacaClienteBusiness().ListarClientesPorPlaca(placaChave.ID);
                        var placa = new PlacaBusiness().Selecionar(Composicao.IDPlaca1.Value);
                        placa.Clientes = new PlacaClienteBusiness().ListarClientesPorPlaca(placa.ID);
                        placa.Clientes.ForEach(x => new PlacaClienteBusiness().Excluir(x.ID));
                        placa.Clientes.AddRange(placaChave.Clientes.Where(p2 =>
                            placa.Clientes.All(p1 => p1.IDCliente != p2.IDCliente)));
                        placa.Clientes.ForEach(x => new PlacaClienteBusiness().Adicionar(new PlacaCliente { IDCliente = x.IDCliente, IDPlaca = placa.ID }));
                    }
                    else if (Composicao.IDEmpresa == (int)EnumEmpresa.Combustiveis)
                    {
                        placaChave = new PlacaBusiness().Selecionar(compAtenrior.IDPlaca2 ?? compAtenrior.IDPlaca1.Value);
                        placaChave.Clientes = new PlacaClienteBusiness().ListarClientesPorPlaca(placaChave.ID);
                        var placa = new PlacaBusiness().Selecionar(compAtenrior.IDPlaca2 ?? compAtenrior.IDPlaca1.Value);
                        placa.Clientes = new PlacaClienteBusiness().ListarClientesPorPlaca(placa.ID);
                        placa.Clientes.ForEach(x => new PlacaClienteBusiness().Excluir(x.ID));
                        placa.Clientes.AddRange(placaChave.Clientes.Where(p2 =>
                            placa.Clientes.All(p1 => p1.IDCliente != p2.IDCliente)));
                        placa.Clientes.ForEach(x => new PlacaClienteBusiness().Adicionar(new PlacaCliente { ID = x.ID, IDCliente = x.IDCliente, IDPlaca = x.IDPlaca }));
                    }
                    else
                    {
                        placaChave = new PlacaBusiness().Selecionar(compAtenrior.IDPlaca1.Value);
                        placaChave.Clientes = new PlacaClienteBusiness().ListarClientesPorPlaca(placaChave.ID);
                        var placa = new PlacaBusiness().Selecionar(compAtenrior.IDPlaca1.Value);
                        placa.Clientes = new PlacaClienteBusiness().ListarClientesPorPlaca(placa.ID);
                        placa.Clientes.ForEach(x => new PlacaClienteBusiness().Excluir(x.ID));
                        placa.Clientes.AddRange(placaChave.Clientes.Where(p2 =>
                            placa.Clientes.All(p1 => p1.IDCliente != p2.IDCliente)));
                        placa.Clientes.ForEach(x => new PlacaClienteBusiness().Adicionar(new PlacaCliente { ID = x.ID, IDCliente = x.IDCliente, IDPlaca = x.IDPlaca }));
                        if (compAtenrior.IDPlaca2.HasValue)
                        {
                            var placaChave2 = new PlacaBusiness().Selecionar(compAtenrior.IDPlaca2 ?? 0);
                            placaChave2.Clientes = new PlacaClienteBusiness().ListarClientesPorPlaca(placaChave2.ID);
                            var placa2 = new PlacaBusiness().Selecionar(compAtenrior.IDPlaca2.Value);
                            placa2.Clientes = new PlacaClienteBusiness().ListarClientesPorPlaca(placa.ID);
                            placa2.Clientes.ForEach(x => new PlacaClienteBusiness().Excluir(x.ID));
                            placa2.Clientes.AddRange(placaChave2.Clientes.Where(p2 =>
                                placa2.Clientes.All(p1 => p1.IDCliente != p2.IDCliente)));
                            placa2.Clientes.ForEach(x => new PlacaClienteBusiness().Adicionar(new PlacaCliente { ID = x.ID, IDCliente = x.IDCliente, IDPlaca = x.IDPlaca }));
                        }
                    }
                }
                PlacaDocumentoBusiness placaDocBLL = new PlacaDocumentoBusiness();
                TipoDocumentoBusiness tipoDocBLL = new TipoDocumentoBusiness();

                foreach (var item in Composicao.p1.Documentos)
                {
                    var documento = placaDocBLL.Selecionar(item.ID);
                    if (idStatus == (int)EnumStatusMotorista.Aprovado)
                    {
                        documento.Processado = false;
                        placaDocBLL.Atualizar(documento);
                    }
                    var tipoDoc = tipoDocBLL.Selecionar(documento.IDTipoDocumento);
                    documento.DataVencimento = item.DataVencimento;
                    item.MesesValidade = tipoDoc.MesesValidade.HasValue ? tipoDoc.MesesValidade.Value : 0;
                    if (comRessalvas)
                        placaDocBLL.Atualizar(documento);
                }

                if (Composicao.p2 != null)
                {
                    foreach (var item in Composicao.p2.Documentos)
                    {
                        var documento = placaDocBLL.Selecionar(item.ID);
                        if (idStatus == (int)EnumStatusMotorista.Aprovado)
                        {
                            documento.Processado = false;
                            placaDocBLL.Atualizar(documento);
                        }
                        var tipoDoc = tipoDocBLL.Selecionar(documento.IDTipoDocumento);
                        documento.DataVencimento = item.DataVencimento;
                        item.MesesValidade = tipoDoc.MesesValidade.HasValue ? tipoDoc.MesesValidade.Value : 0;
                        if (comRessalvas)
                            placaDocBLL.Atualizar(documento);
                    }
                }

                if (Composicao.p3 != null)
                {
                    foreach (var item in Composicao.p3.Documentos)
                    {
                        var documento = placaDocBLL.Selecionar(item.ID);
                        if (idStatus == (int)EnumStatusMotorista.Aprovado)
                        {
                            documento.Processado = false;
                            placaDocBLL.Atualizar(documento);
                        }
                        var tipoDoc = tipoDocBLL.Selecionar(documento.IDTipoDocumento);
                        documento.DataVencimento = item.DataVencimento;
                        item.MesesValidade = tipoDoc.MesesValidade.HasValue ? tipoDoc.MesesValidade.Value : 0;
                        if (comRessalvas)
                            placaDocBLL.Atualizar(documento);
                    }
                }

                if (Composicao.p4 != null)
                {
                    foreach (var item in Composicao.p4.Documentos)
                    {
                        var documento = placaDocBLL.Selecionar(item.ID);
                        if (idStatus == (int)EnumStatusMotorista.Aprovado)
                        {
                            documento.Processado = false;
                            placaDocBLL.Atualizar(documento);
                        }
                        var tipoDoc = tipoDocBLL.Selecionar(documento.IDTipoDocumento);
                        documento.DataVencimento = item.DataVencimento;
                        item.MesesValidade = tipoDoc.MesesValidade.HasValue ? tipoDoc.MesesValidade.Value : 0;
                        if (comRessalvas)
                            placaDocBLL.Atualizar(documento);
                    }
                }

                if (Composicao.Operacao == "FOB" && idStatus == (int)EnumStatusComposicao.Aprovado)
                {
                    if (Composicao.p1.Clientes != null)
                        Composicao.p1.Clientes.Where(w => !w.DataAprovacao.HasValue).ForEach(x => _placaClienteBLL.Atualizar(new PlacaCliente
                        {
                            ID = x.ID,
                            DataAprovacao = DateTime.Now,
                            IDCliente = x.IDCliente,
                            IDPlaca = x.IDPlaca
                        }));
                    if (Composicao.p2?.Clientes != null)
                        Composicao.p2.Clientes.Where(w => !w.DataAprovacao.HasValue).ForEach(x => _placaClienteBLL.Atualizar(new PlacaCliente
                        {
                            ID = x.ID,
                            DataAprovacao = DateTime.Now,
                            IDCliente = x.IDCliente,
                            IDPlaca = x.IDPlaca
                        }));
                    if (Composicao.p3?.Clientes != null)
                        Composicao.p3.Clientes.Where(w => !w.DataAprovacao.HasValue).ForEach(x => _placaClienteBLL.Atualizar(new PlacaCliente
                        {
                            ID = x.ID,
                            DataAprovacao = DateTime.Now,
                            IDCliente = x.IDCliente,
                            IDPlaca = x.IDPlaca
                        }));
                    if (Composicao.p4?.Clientes != null)
                        Composicao.p4.Clientes.Where(w => !w.DataAprovacao.HasValue).ForEach(x => _placaClienteBLL.Atualizar(new PlacaCliente
                        {
                            ID = x.ID,
                            DataAprovacao = DateTime.Now,
                            IDCliente = x.IDCliente,
                            IDPlaca = x.IDPlaca
                        }));
                }

                Atualizar(Composicao);

                if (placaUsada)
                {
                    if (Composicao.Operacao == "FOB")
                    {
                        //Equalizar todas as placas com os mesmos clientes (mesmas permiss�es)
                        List<PlacaClienteView> clientes = Composicao.p2 != null ? Composicao.p2.Clientes : Composicao.p1.Clientes;
                        RemoverAdicionarClientes(Composicao.p1, clientes);
                        RemoverAdicionarClientes(Composicao.p2, clientes);
                        RemoverAdicionarClientes(Composicao.p3, clientes);
                        RemoverAdicionarClientes(Composicao.p4, clientes);
                    }
                }

                transactionScope.Complete();
            }

            var statusAntigo = Selecionar(Composicao.ID).IDStatus;

            //integrar sap//atualizar composicao antiga/ mudar status da atual e enviar e-mail
            using (TransactionScope transactionScope = Transactions.CreateTransactionScope(TransactionScopeOption.Required, IsolationLevel.ReadCommitted, 4))
            {
                Composicao.IDStatus = idStatus;

                if (Composicao.IDStatus != (int)EnumStatusComposicao.Reprovado && Composicao.IDStatus != (int)EnumStatusComposicao.EmAprovacao)
                {
                    //tentar integrar com SAP
                    EnumTipoIntegracaoSAP tipoIntegracao;
                    if (Composicao.tipoIntegracao == null)
                        tipoIntegracao = EnumTipoIntegracaoSAP.Inclusao;
                    else
                        tipoIntegracao = Composicao.tipoIntegracao.Value;

                    Composicao.VolumeComposicao = ObterVolumeComposicao(Composicao);
                    var ForcaLeci = false;
                    if (UserSession.GetCurrentInfoUserSystem() != null)
                    {
                        ForcaLeci = UserSession.GetCurrentInfoUserSystem().InformacoesPermissao.FirstOrDefault(p => p.MVCController == "Composicao" && p.NomeAcao == "IgnorarLeci") != null;
                        if (statusAntigo == (int)EnumStatusComposicao.AguardandoAtualizacaoSAP && ForcaLeci)
                            Composicao.IgnorarLeciAdm = ForcaLeci && Composicao.IgnorarLeci.HasValue && Composicao.IgnorarLeci.Value;
                    }
                    if (PodeIntegrarSAP(Composicao))
                    {
                        var retorno = IntegrarSAP(Composicao, tipoIntegracao);
                        if (Composicao.jaExiste)
                        {
                            tipoIntegracao = EnumTipoIntegracaoSAP.Alteracao;
                            retorno = IntegrarSAP(Composicao, tipoIntegracao);
                        }

                        if (!string.IsNullOrEmpty(retorno.Trim()))
                            if (retorno.Contains("Existe LECI aberta"))// adicionar o if olhando para o retorno para ver se tem essa msg assim que alterado no SAP
                            {
                                if (statusAntigo == (int)EnumStatusComposicao.AguardandoAtualizacaoSAP && ForcaLeci)
                                    Composicao.Mensagem = retorno + Traducao.GetTextoPorLingua(" Deseja continuar?", " Queres continuar?", _pais);
                                else if (Composicao.IDStatus == (int)EnumStatusComposicao.Bloqueado || tipoIntegracao == EnumTipoIntegracaoSAP.AprovarCheckList)
                                    Composicao.Mensagem = retorno + Traducao.GetTextoPorLingua(" Essa a��o n�o pode ser executada.", " Esta acci�n no puede ser realizada", _pais);
                                else
                                    Composicao.Mensagem = retorno + Traducao.GetTextoPorLingua(" Deseja continuar? Se sim, a composi��o entrar� numa fila para atualiza��o no SAP ap�s o encerramento da LECI", "  Queres continuar? Si es as�, la composici�n se pondr� en cola para la actualizaci�n de SAP despu�s de que LECI se cierre", _pais);

                                return false;
                            }
                            else if (!string.IsNullOrEmpty(retorno))
                            {
                                Composicao.Mensagem = retorno;
                                return false;
                            }
                    }
                    else if (Composicao.IDStatus == (int)EnumStatusComposicao.Aprovado && Composicao.IDComposicao.HasValue)
                    {
                        var composicaoAntiga = Selecionar(Composicao.IDComposicao.Value);
                        composicaoAntiga.IDStatus = (int)EnumStatusComposicao.Aprovado;
                        composicaoAntiga.CodigoEasyQuery = Composicao.CodigoEasyQuery;
                        composicaoAntiga.CodigoSalesForce = Composicao.CodigoSalesForce;
                        Atualizar(composicaoAntiga);
                    }

					var ativaIntegracaoCsonline = Config.GetConfigInt(EnumConfig.AtivaIntegracaoCsonline, (int)Composicao.IdPais);
					if (Composicao.Operacao == "FOB" && idStatus == (int)EnumStatusComposicao.Aprovado && ativaIntegracaoCsonline != 0)
					{
						if (Composicao.IDEmpresa == (int)EnumEmpresa.Combustiveis)
                        {
                            //Efetuar a chamada do SAP com a placa correspondente.
                            //Obs.: esta fun��o s� existe no sap de FUELS.
                            var integraSap = new WsIntegraSAP();

                            var placaSap = SelecionaPlacaPrincipalFOB(Composicao);

                            //Enviar os dados de placa e cliente para o SAP.
                            var retorno = integraSap.VincularVeiculoClienteSap(placaSap);

                            if (!string.IsNullOrEmpty(retorno))
                            {
                                Composicao.Mensagem = retorno;
                                return false;
                            }
                        }
                    }

                    if (Composicao.IDStatus == (int)EnumStatusComposicao.Aprovado && Composicao.IDComposicao.HasValue)
                    {
                        var composicaoAntiga = Selecionar(Composicao.IDComposicao.Value);
                        if (composicaoAntiga != null)
                        {
                            composicaoAntiga.IDStatus = 3;
                            composicaoAntiga.CodigoEasyQuery = Composicao.CodigoEasyQuery;
                            composicaoAntiga.CodigoSalesForce = Composicao.CodigoSalesForce;
                            this.Atualizar(composicaoAntiga);
                        }
                    }

                    if (Composicao.IDStatus == (int)EnumStatusComposicao.Aprovado)
                    {
                        ReplicarAlteracoes(Composicao.p1, Composicao);
                        ReplicarAlteracoes(Composicao.p2, Composicao);
                        ReplicarAlteracoes(Composicao.p3, Composicao);
                        ReplicarAlteracoes(Composicao.p4, Composicao);
                    }
                }

                var idPais = (int)SelecionarPaisPlaca1(Composicao);

                var abrirEq = ValidarAbrirChamadoEasyQuery(Composicao, idPais);
                var abrirSf = ValidarAbrirChamadoSalesForce(Composicao, idPais, aprovacaoAutomatica);

                var alterandoChamado = (abrirEq && !string.IsNullOrEmpty(Composicao.CodigoEasyQuery))
                    || (abrirSf && !string.IsNullOrEmpty(Composicao.CodigoSalesForce));

                var eraReprovado = statusAntigo == (int)EnumStatusComposicao.Reprovado && idStatus == (int)EnumStatusComposicao.EmAprovacao;
                var eraAprovado = statusAntigo == (int)EnumStatusComposicao.Aprovado && idStatus == (int)EnumStatusComposicao.EmAprovacao;

                if (eraReprovado || (eraAprovado && alterandoChamado) || aprovacaoAutomatica)
                {
                    Composicao.VolumeComposicao = ObterVolumeComposicao(Composicao);
                    //composicao.EmailSolicitante = "testeUnitario@raizen.com";

                    if (abrirSf)
                    {
                        AbrirChamadoSalesForce(Composicao, idPais, aprovacaoAutomatica);
                    }
                    else if (abrirEq)
                    {
                        AbrirChamadoEasyQuery(Composicao, idPais);
                    }

                }

                this.Atualizar(Composicao);

                if (!bloqueio && (Composicao.IDStatus == (int)EnumStatusComposicao.Aprovado || Composicao.IDStatus == (int)EnumStatusComposicao.Reprovado) && enviaEmail)
                {
                    EnviarEmailAlertaSituacao(Composicao, (int)idPais);

                    //Angelira s� manda se for CIF e se for aprova��o
                    if (Composicao.Operacao == "CIF" && Composicao.IDEmpresa == (int)EnumEmpresa.Combustiveis && Composicao.IDStatus == (int)EnumStatusComposicao.Aprovado)
                        EnviarEmailAlertaSituacao(Composicao, (int)idPais, true);
                }

				if ((Composicao.IDStatus == (int)EnumStatusComposicao.Aprovado || Composicao.IDStatus == (int)EnumStatusComposicao.Reprovado) && ValidarEncerrarChamadoSalesForce(Composicao))
				{
					EncerrarChamadoSalesForce(Composicao, comRessalvas);
				}


                if (Composicao.IDStatus == (int)EnumStatusComposicao.Aprovado && ValidarEnviarDadosCSOnline(Composicao))
                {
                    CriarOuAtualizarComposicaoNoCsOnline(Composicao);
                }

                transactionScope.Complete();
                return true;
            }
        }

        private void CriarOuAtualizarComposicaoNoCsOnline(Composicao composicao)
        {
            var wsCsonline = new WsCsonline(
                ConfigurationManager.AppSettings["CsOnlineEndpoint"],
                ConfigurationManager.AppSettings["CsOnlineOrigin"],
                ConfigurationManager.AppSettings["CsOnlinePin"],
                ConfigurationManager.AppSettings["CsOnlineApplicationKey"]
            );

			int taraEmKg = (int)(1000 * ((composicao.p1?.Tara ?? 0) + (composicao.p2?.Tara ?? 0) + (composicao.p3?.Tara ?? 0) + (composicao.p4?.Tara ?? 0)));

			int numeroEixos = (composicao.p1?.NumeroEixos ?? 0) + (composicao.p2?.NumeroEixos ?? 0) + (composicao.p3?.NumeroEixos ?? 0) + (composicao.p4?.NumeroEixos ?? 0);

            int pbtcEmKg = (int)(1000 * composicao.PBTC);

			if (composicao.p2 != null)
            {
                wsCsonline.CriarOuAtualizarVeiculo(CriaRequisicaoCsonlineVeiculo(composicao.p2, taraEmKg, numeroEixos, pbtcEmKg));
            }
			else if (composicao.p1 != null)
			{
				wsCsonline.CriarOuAtualizarVeiculo(CriaRequisicaoCsonlineVeiculo(composicao.p1, taraEmKg, numeroEixos, pbtcEmKg));
			}
        }

        private CsonlineVehicleUpdateRequestView CriaRequisicaoCsonlineVeiculo(Placa placa, int taraEmKg, int numeroEixos, int pbtcEmKg)
        {
            var licencePlate = placa.PlacaVeiculo.Replace("-", "");

			if (licencePlate.Length > 3)
			{
				licencePlate = $"{licencePlate.Substring(0, 3)}-{licencePlate.Substring(3)}";
			}

			var customers = placa.Clientes.Select(c => c.Ibm).ToList();

            return new CsonlineVehicleUpdateRequestView
            {
                Vehicle = new CsonlineVehicleRequestView
                {
                    Id = placa.PlacaVeiculo,
                    LicencePlate = licencePlate,
                    Axles = numeroEixos,
                    Tare = taraEmKg,
					AxleSpacing = placa.EixosDistanciados,
                    WarnCustomers = false,
                    IsDeleted = false,
                    MaxVolume = CalcularVolumeMaximo(placa),
                    MaxWeight = pbtcEmKg,
                },
                Customers = customers,
            };
        }

        private decimal CalcularVolumeMaximo(Placa placa)
        {
            decimal volumeMaximo = 0;

            if (placa == null)
            {
                return 0;
            }

            foreach (var compartimento in placa.Compartimentos)
            {
                foreach (var seta in compartimento.setas)
                {
                    if (seta.Principal)
                    {
                        volumeMaximo += seta.Volume ?? 0;
                    }
                }
            }

            return volumeMaximo;
        }

        private bool ValidarEnviarDadosCSOnline(Composicao composicao)
        {
            var ativaIntegracaoCsonline = Config.GetConfigInt(EnumConfig.AtivaIntegracaoCsonline, (int)composicao.IdPais);

            return ativaIntegracaoCsonline != 0 && composicao.IdPais == (int)EnumPais.Brasil && composicao.Operacao == "FOB";
        }

        private bool PodeIntegrarSAP(Composicao composicao)
        {
            if (_pais == EnumPais.Argentina)
                return true;

            if (composicao.IDEmpresa == (int)EnumEmpresa.Combustiveis || composicao.IDEmpresa == (int)EnumEmpresa.Ambos)
                return true;

            if ((composicao.IDEmpresa == (int)EnumEmpresa.EAB) && (!composicao.IgnorarLeci.HasValue || !composicao.IgnorarLeci.Value) || (composicao.IgnorarLeciAdm.HasValue && composicao.IgnorarLeciAdm.Value))
                return true;

            return false;
        }

        public Placa SelecionaPlacaPrincipalFOB(Composicao composicao)
        {
            if ((composicao.IDTipoComposicao == (int)EnumTipoComposicao.Truck || composicao.IDTipoComposicao == (int)EnumTipoComposicao.Truck_ARG) && composicao.IDPlaca1.HasValue)
                return (composicao.p1 ?? _placaBLL.Selecionar((int)composicao.IDPlaca1));
            else if (composicao.IDEmpresa == 1 && composicao.IDPlaca1.HasValue)
                return (composicao.p1 ?? _placaBLL.Selecionar((int)composicao.IDPlaca1));
            else if (composicao.IDEmpresa == 2 && composicao.IDPlaca2.HasValue)
                return (composicao.p2 ?? _placaBLL.Selecionar((int)composicao.IDPlaca2));
            else if (composicao.IDEmpresa == 3 && composicao.IDPlaca1.HasValue)
                return (composicao.p1 ?? _placaBLL.Selecionar((int)composicao.IDPlaca1));
            else
                return new Placa();
        }

        private void ReplicarAlteracoes(Placa placa, Composicao comp)
        {
            if (placa != null)
            {
                var placas = new PlacaBusiness().ListarPlacaPorOperacaoLinhaNegocio(new PlacaFiltro { PlacaVeiculo = placa.PlacaVeiculo, Id = placa.ID, IDEmpresa = comp.IDEmpresa, Operacao = placa.Operacao });
                if (placas != null && placas.Any())
                {
                    foreach (var item in placas)
                    {
                        AlterarPlaca(placa, item, item.ID);
                    }
                }
            }
        }

        private static void AlterarPlaca(Placa placaNova, Placa placaAlterar, int id)
        {
            var placaSetaBll = new PlacaSetaBusiness();
            var docBll = new PlacaDocumentoBusiness();
            var placaClienteBll = new PlacaClienteBusiness();

            //limpar os documentos, compartimentos, setas e clientes
            var docsPlacaAntiga = docBll.Listar(x => x.IDPlaca == id);
            docsPlacaAntiga.ForEach(x => docBll.ExcluirDoc(x.ID));

            var setas = placaSetaBll.Listar(w => w.IDPlaca == placaAlterar.ID);
            setas.ForEach(x => placaSetaBll.Excluir(x.ID));

            var clientes = placaClienteBll.Listar(w => w.IDPlaca == placaAlterar.ID);
            clientes.ForEach(x => placaClienteBll.Excluir(x.ID));

            //Adicionar os documentos da placa nova na placa antiga
            if (placaNova.Documentos != null && placaNova.Documentos.Any())
            {
                var documentos = docBll.Listar(x => x.IDPlaca == placaNova.ID);
                documentos.ForEach(x => { x.ID = 0; x.IDPlaca = id; docBll.Adicionar(x); });
            }

            //se for fob ir� adicionar os clientes da placa nova na placa antiga
            if (placaAlterar.Operacao == "FOB")
            {
                if (placaNova.Clientes != null && placaNova.Clientes.Any())
                {

                    var listClientes = new PlacaClienteBusiness().Listar(x => x.IDPlaca == placaNova.ID);

                    listClientes.ForEach(x =>
                    {
                        x.ID = 0; x.IDPlaca = id; new PlacaClienteBusiness()
                                                           .Adicionar(new PlacaCliente
                                                           {
                                                               IDCliente = x.IDCliente,
                                                               IDPlaca = x.IDPlaca
                                                           });
                    });
                }
            }


            if (placaNova.Setas != null && placaNova.Setas.Any())
            {
                var listSetas = placaSetaBll.Listar(w => w.IDPlaca == placaNova.ID);
                listSetas.ForEach(x => { x.ID = 0; x.IDPlaca = id; placaSetaBll.Adicionar(x); });

            }

            //passar os dados da placa nova para as placas antigas (ficar�o todas iguais)
            placaAlterar = (Placa)placaNova.Clone();
            placaAlterar.ID = id;

            new PlacaBusiness().Atualizar(placaAlterar);
        }

        private void RemoverAdicionarClientes(Placa placa, List<PlacaClienteView> clientes)
        {
            if (placa != null)
            {
                foreach (var item in placa.Clientes)
                {
                    var pc = new PlacaClienteBusiness().Selecionar(item.ID);
                    if (pc != null)
                        _placaClienteBLL.Excluir(pc);
                }
                //placa.Clientes.ForEach(x => new PlacaClienteBusiness().Excluir(x.ID));
                foreach (var item in clientes)
                {
                    item.IDPlaca = placa.ID;
                    new PlacaClienteBusiness().Adicionar(new PlacaCliente { IDCliente = item.IDCliente, IDPlaca = item.IDPlaca });
                }
                placa.Clientes = clientes;
            }
        }

        private void EnviarEmailAlertaSituacao(Composicao composicao, int idPais, bool isEmailAngelira = false)
        {
            var placas = new PlacaBusiness((EnumPais)idPais).ListarPorComposicao(composicao);
            Usuario usuario;
            string assunto;
            var estadoComposicao = composicao.IDStatus == (int)EnumStatusComposicao.Aprovado ? true : false;
            var corpoEmail = new StringBuilder();


            //EMAIL ANGELIRA
            if (isEmailAngelira)
            {
                if (idPais == (int)EnumPais.Brasil)
                    PreencheEmailAngeliraBrasil(idPais, placas, out usuario, out assunto, estadoComposicao, corpoEmail);
                else
                    PreencheEmailAngeliraArgentina(idPais, placas, out usuario, out assunto, estadoComposicao, corpoEmail);

            }
            else
            {
                var ibm = string.Empty;
                var razaoSocial = string.Empty;
                var email = string.Empty;
                usuario = new UsuarioBusiness().Selecionar(w => w.Login == composicao.LoginUsuario && w.Status);
                if (usuario == null) return;
                email = usuario.Email;
                if (composicao.Operacao == "FOB")
                {
                    var clienteUsuario = new UsuarioClienteBusiness().Selecionar(w => w.IDUsuario == usuario.ID);
                    if (clienteUsuario != null)
                    {
                        var cliente = new ClienteBusiness().Selecionar(w => w.ID == clienteUsuario.IDCliente);
                        ibm = cliente.IBM;
                        razaoSocial = cliente.RazaoSocial;
                    }
                }
                else if (composicao.Operacao == "CIF")
                {
                    var idPlaca = composicao.IDEmpresa == (int)EnumEmpresa.EAB ? composicao.IDPlaca1 : composicao.IDPlaca2 ?? composicao.IDPlaca1;
                    if (idPlaca != null)
                    {
                        var placa = new PlacaBusiness().Selecionar(idPlaca.Value);
                        var transportadora = new TransportadoraBusiness().Selecionar(placa.IDTransportadora.Value);
                        if (transportadora != null)
                        {
                            ibm = transportadora.IBM;
                            razaoSocial = transportadora.RazaoSocial;
                        }
                    }
                }

                string placasEmail = string.Join(", ", placas.Select(w => w.PlacaVeiculo));

                //REPROVADA
                if (!estadoComposicao)
                {
                    assunto = Config.GetConfig(EnumConfig.TituloVeiculoReprovado, idPais);
                    corpoEmail.AppendFormat(Config.GetConfig(EnumConfig.CorpoVeiculoReprovado, idPais), placasEmail);
                }
                //APROVADA
                else
                {
                    StringBuilder permissaoCarregamento = new StringBuilder();
                    assunto = Config.GetConfig(EnumConfig.TituloVeiculoAprovado, idPais);

                    placas = new PlacaBusiness().ListarPorComposicaoCapacidade(composicao);
                    foreach (Placa placa in placas)
                    {
                        if (placa.Setas != null)
                        {
                            int nSeta = 1;
                            foreach (var seta in placa.Setas.OrderBy(o => o.ID))
                            {
                                MontarMensagemPermissaoCarregamento(permissaoCarregamento, nSeta, seta);

                                nSeta++;
                            }
                        }
                    }

                    corpoEmail.AppendFormat(Config.GetConfig(EnumConfig.CorpoVeiculoAprovado, idPais), placasEmail, permissaoCarregamento);
                }

                Email.Enviar(email, assunto, corpoEmail.ToString());
            }
        }

        private void MontarMensagemPermissaoCarregamento(
            StringBuilder permissaoCarregamento,
            int numeroSeta,
            PlacaSeta seta,
            //O parametro "usarAcento" foi criado como alternativa para resolver o problema de acentua��o nos testes unit�rios do Jenkins
            bool usarAcentos = true)
        {

            var color = string.Empty;
            var textoStatus = string.Empty;

            if (seta.Produtos != null)
            {
                permissaoCarregamento.AppendFormat("<b>" + Traducao.GetTextoPorLingua("Seta", "Flecha", _pais) + " {0}</b><br/>", numeroSeta);
                foreach (var prod in seta.Produtos)
                {
                    switch ((EnumSituacaoPlacaLimite)prod.Situacao)
                    {
                        case EnumSituacaoPlacaLimite.NaoPermitido:
                            textoStatus = Traducao.GetTextoPorLingua($"{(usarAcentos ? "N�o" : "Nao")} Permitido", "No se permite", _pais);
                            color = "#CC0000";
                            break;
                        case EnumSituacaoPlacaLimite.NoLimite:
                            textoStatus = Traducao.GetTextoPorLingua("No Limite", $"En el {(usarAcentos ? "l�mite" : "limite")}", _pais);
                            color = "#F8551F";
                            break;
                        case EnumSituacaoPlacaLimite.Permitido:
                            textoStatus = Traducao.GetTextoPorLingua("Permitido", "Permitido", _pais);
                            color = "#31698A";
                            break;
                    }
                    permissaoCarregamento.AppendFormat("<b>{0} </b><font color=\"{1}\">{2}</font> " + Traducao.GetTextoPorLingua("o carregamento do produto para seta informada", "cargar el producto para la flecha informada", _pais) + "{3}.<br/>", prod.Nome, color, textoStatus, prod.Situacao == (int)EnumSituacaoPlacaLimite.NaoPermitido ? " " + Traducao.GetTextoPorLingua("em virtude de excesso de peso", "debido al exceso de peso", _pais) : "");
                }
                permissaoCarregamento.Append("<br/>");
            }
        }

        private static void PreencheEmailAngeliraBrasil(int idPais, List<Placa> placas, out Usuario usuario, out string assunto, bool estadoComposicao, StringBuilder corpoEmail)
        {
            EnumPais pais = (EnumPais)idPais;

            usuario = new Usuario { Nome = "Angelira", Email = Config.GetConfig(EnumConfig.EmailAngelira, idPais) };
            assunto = estadoComposicao ?
                Traducao.GetTextoPorLingua("Ra�zen - Aprova��o de Cadastro de Ve�culo", "Ra�zen - Aprobaci�n de registro de veh�culo", pais) :
                Traducao.GetTextoPorLingua("Ra�zen - Reprova��o de Cadastro de Ve�culo", "Ra�zen - Reprobaci�n de registro de veh�culo", pais);

            corpoEmail.Append($"{Traducao.GetTextoPorLingua("Prezado", "Estimado", pais)}(s) <b>Angelira</b>,<br/>");
            corpoEmail.Append("Informamos que o cadastro da composi��o abaixo foi conclu�do.Seguem as informa��es:<br/><br/>");
            corpoEmail.Append("Placas da composi��o: ");
            placas.ForEach(w => corpoEmail.Append(w.PlacaVeiculo + " "));



            foreach (var placa in placas)
            {
                var tipoVeiculo = new TipoVeiculoBusiness().Selecionar(placa.IDTipoVeiculo).Nome;
                var transportadora = placa.IDTransportadora.HasValue ? new TransportadoraBusiness().Selecionar(placa.IDTransportadora.Value).RazaoSocial : string.Empty;
                var tipoCarregamento = placa.IDTipoCarregamento.HasValue ? new TipoCarregamentoBusiness().Selecionar(placa.IDTipoCarregamento.Value).Nome : string.Empty;
                var categoriaVeiculo = new CategoriaVeiculoBusiness().Selecionar(placa.IDCategoriaVeiculo).Nome;
                var estado = new EstadoBusiness().Selecionar((int)placa.PlacaBrasil.IDEstado).Nome;

                corpoEmail.AppendFormat("<br/><br/> <b>Placa: {0}</b> <br/>", placa.PlacaVeiculo);
                corpoEmail.AppendFormat("<b>Opera��o: </b>{0} <br/>", placa.Operacao);
                corpoEmail.AppendFormat("<b>Tipo Ve�culo: </b>{0} <br/>", tipoVeiculo);

                //AMBAS //CSCUNI-665
                if (placa.IDTransportadora2.HasValue)
                {
                    var transportadora2 = new TransportadoraBusiness().Selecionar(placa.IDTransportadora2.Value).RazaoSocial;

                    corpoEmail.AppendFormat("<b>Transportadora Combust�vel: </b>{0} <br/>", transportadora);
                    corpoEmail.AppendFormat("<b>Transportadora EAB: </b>{0} <br/>", transportadora2);
                }
                else
                    corpoEmail.AppendFormat("<b>Transportadora: </b>{0} <br/>", transportadora);

                corpoEmail.AppendFormat("<b>Renavam: </b>{0} <br/>", placa.PlacaBrasil.Renavam);
                corpoEmail.AppendFormat("<b>Marca: </b>{0} <br/>", placa.Marca);
                corpoEmail.AppendFormat("<b>Modelo: </b>{0} <br/>", placa.Modelo);
                corpoEmail.AppendFormat("<b>Chassi: </b>{0} <br/>", placa.Chassi);
                corpoEmail.AppendFormat("<b>Ano Fabrica��o: </b>{0} <br/>", placa.AnoFabricacao);
                corpoEmail.AppendFormat("<b>Ano Modelo: </b>{0} <br/>", placa.AnoFabricacao);
                corpoEmail.AppendFormat("<b>Cor: </b>{0} <br/>", placa.Cor);
                corpoEmail.AppendFormat("<b>Tara: </b>{0} <br/>", placa.Tara);
                corpoEmail.AppendFormat("<b>Tipo Rastreador: </b>{0} <br/>", placa.TipoRastreador);
                corpoEmail.AppendFormat("<b>N�mero antena: </b>{0} <br/>", placa.NumeroAntena);
                corpoEmail.AppendFormat("<b>Vers�o :</b>{0} <br/>", placa.Versao);
                corpoEmail.AppendFormat("<b>C�mera monitoramento: </b>{0} <br/>", placa.CameraMonitoramento ? "Sim" : "N�o");
                corpoEmail.AppendFormat("<b>Bomba descarga: </b>{0} <br/>", placa.BombaDescarga ? "Sim" : "N�o");
                corpoEmail.AppendFormat("<b>N�mero Eixos: </b>{0} <br/>", placa.NumeroEixos);
                corpoEmail.AppendFormat("<b>N�mero eixos pneus duplos: </b>{0} <br/>", placa.NumeroEixosPneusDuplos);
                corpoEmail.AppendFormat("<b>Multi seta: </b>{0} <br/>", placa.MultiSeta ? "Sim" : "N�o");
                corpoEmail.AppendFormat("<b>Tipo Carregamento: </b>{0} <br/>", tipoCarregamento);
                corpoEmail.AppendFormat("<b>CPF/CNPJ: </b>{0} <br/>", placa.PlacaBrasil.CPFCNPJ);
                if (placa.PlacaBrasil.CPFCNPJ != null && placa.PlacaBrasil.CPFCNPJ.Length <= 11)
                    corpoEmail.AppendFormat("<b>Data Nascimento: </b>{0} <br/>", placa.DataNascimento?.ToString("dd/MM/yyyy") ?? string.Empty);
                else if (placa.PlacaBrasil.CPFCNPJ != null && placa.PlacaBrasil.CPFCNPJ.Length > 11)
                    corpoEmail.AppendFormat("<b>Raz�o Social: </b>{0} <br/>", placa.RazaoSocial);
                corpoEmail.AppendFormat("<b>Categoria ve�culo: </b>{0} <br/>", categoriaVeiculo);
                corpoEmail.AppendFormat("<b>Observa��o: </b>{0} <br/>", placa.Observacao);
                corpoEmail.AppendFormat("<b>Estado: </b>{0} <br/>", estado);

            }
            corpoEmail.Append("<br/>Eventuais d�vidas ou solicita��es, nos contatar atrav�s da Equipe de Cadastros Ra�zen.");
            Email.Enviar(usuario.Email, assunto, corpoEmail.ToString());
        }

        private static void PreencheEmailAngeliraArgentina(int idPais, List<Placa> placas, out Usuario usuario, out string assunto, bool estadoComposicao, StringBuilder corpoEmail)
        {
            usuario = new Usuario { Nome = "Rutasky", Email = Config.GetConfig(EnumConfig.EmailRutasky, idPais) };
            assunto = estadoComposicao ? "Ra�zen - Aprobaci�n de registro de veh�culo" : "Ra�zen - Reprobaci�n de registro de veh�culo";

            corpoEmail.Append("Estimado(s) <b>Rutasky</b>,<br/>"); // ver se aqui � esse nome mesmo ou Angelira
            corpoEmail.Append("Tenga en cuenta que el registro de composici�n a continuaci�n se ha completado. Siguen las informaciones:<br/><br/>");
            corpoEmail.Append("Patentes da composici�n: ");
            placas.ForEach(w => corpoEmail.Append(w.PlacaVeiculo + " "));

            foreach (var placa in placas)
            {
                var tipoVeiculo = new TipoVeiculoBusiness().Selecionar(placa.IDTipoVeiculo).Nome;
                var transportadora = placa.IDTransportadora.HasValue ? new TransportadoraBusiness().Selecionar(placa.IDTransportadora.Value).RazaoSocial : string.Empty;
                var tipoCarregamento = placa.IDTipoCarregamento.HasValue ? new TipoCarregamentoBusiness().Selecionar(placa.IDTipoCarregamento.Value).Nome : string.Empty;
                var categoriaVeiculo = new CategoriaVeiculoBusiness().Selecionar(placa.IDCategoriaVeiculo).Nome;

                corpoEmail.AppendFormat("<br/><br/> <b>Patente: {0}</b> <br/>", placa.PlacaVeiculo);
                corpoEmail.AppendFormat("<b>Operaci�n: </b>{0} <br/>", placa.Operacao);
                corpoEmail.AppendFormat("<b>Tipo de Veh�culo: </b>{0} <br/>", tipoVeiculo);

                //AMBAS //CSCUNI-665
                if (placa.IDTransportadora2.HasValue)
                {
                    var transportadora2 = new TransportadoraBusiness().Selecionar(placa.IDTransportadora2.Value).RazaoSocial;

                    corpoEmail.AppendFormat("<b>Transportadora Combustible: </b>{0} <br/>", transportadora);
                    corpoEmail.AppendFormat("<b>Transportadora EAB: </b>{0} <br/>", transportadora2);
                }
                else
                    corpoEmail.AppendFormat("<b>Transportadora: </b>{0} <br/>", transportadora);


                corpoEmail.AppendFormat("<b>PBTC: </b>{0} <br/>", placa.PlacaArgentina.PBTC);

                corpoEmail.AppendFormat("<b>Marca: </b>{0} <br/>", placa.Marca);
                corpoEmail.AppendFormat("<b>Modelo: </b>{0} <br/>", placa.Modelo);
                if (placa.IDTipoVeiculo == (int)EnumTipoVeiculo.Tractor)
                {
                    corpoEmail.AppendFormat("<b>Potencia: </b>{0} <br/>", placa.PlacaArgentina.Potencia);
                    corpoEmail.AppendFormat("<b>N�mero de Motor: </b>{0} <br/>", placa.PlacaArgentina.NrMotor);
                    corpoEmail.AppendFormat("<b>Sistema Satelital - Marca: </b>{0} <br/>", placa.PlacaArgentina.SatelitalMarca);
                    corpoEmail.AppendFormat("<b>Sistema Satelital - Modelo: </b>{0} <br/>", placa.PlacaArgentina.SatelitalModelo);
                    corpoEmail.AppendFormat("<b>Sistema Satelital - N� Interno: </b>{0} <br/>", placa.PlacaArgentina.SatelitalNrInterno);
                    corpoEmail.AppendFormat("<b>Sistema Satelital � Empresa: </b>{0} <br/>", placa.PlacaArgentina.SatelitalEmpresa);
                }
                if (placa.IDTipoVeiculo == (int)EnumTipoVeiculo.Semiremolque)
                {
                    corpoEmail.AppendFormat("<b>Sistema Satelital � Empresa: </b>{0} <br/>", placa.PlacaArgentina.Material);
                }
                corpoEmail.AppendFormat("<b>Chassi: </b>{0} <br/>", placa.Chassi);
                corpoEmail.AppendFormat("<b>Ano Fabrica��o: </b>{0} <br/>", placa.AnoFabricacao);
                corpoEmail.AppendFormat("<b>Ano Modelo: </b>{0} <br/>", placa.AnoFabricacao);
                corpoEmail.AppendFormat("<b>Cor: </b>{0} <br/>", placa.Cor);
                corpoEmail.AppendFormat("<b>Tara: </b>{0} <br/>", placa.Tara);
                corpoEmail.AppendFormat("<b>Tipo Rastreador: </b>{0} <br/>", placa.TipoRastreador);
                corpoEmail.AppendFormat("<b>N�mero antena: </b>{0} <br/>", placa.NumeroAntena);
                corpoEmail.AppendFormat("<b>Vers�o :</b>{0} <br/>", placa.Versao);
                corpoEmail.AppendFormat("<b>C�mera monitoramento: </b>{0} <br/>", placa.CameraMonitoramento ? "Sim" : "N�o");
                corpoEmail.AppendFormat("<b>Bomba descarga: </b>{0} <br/>", placa.BombaDescarga ? "Sim" : "N�o");
                corpoEmail.AppendFormat("<b>N�mero de Ejes: </b>{0} <br/>", placa.NumeroEixos);
                corpoEmail.AppendFormat("<b>Multi Flecha: </b>{0} <br/>", placa.MultiSeta ? "Sim" : "N�o");
                corpoEmail.AppendFormat("<b>Tipo de carga: </b>{0} <br/>", tipoCarregamento);

                corpoEmail.AppendFormat("<b>CUIT: </b>{0} <br/>", placa.PlacaArgentina.CUIT);
                if (placa.PlacaArgentina.CUIT != null && placa.PlacaArgentina.CUIT.Length <= 11)
                    corpoEmail.AppendFormat("<b>Fecha de nacimiento: </b>{0} <br/>", placa.DataNascimento?.ToString("dd/MM/yyyy") ?? string.Empty);
                else if (placa.PlacaArgentina.CUIT != null && placa.PlacaArgentina.CUIT.Length > 11)
                    corpoEmail.AppendFormat("<b>Raz�n social: </b>{0} <br/>", placa.RazaoSocial);
                corpoEmail.AppendFormat("<b>Categor�a de veh�culo: </b>{0} <br/>", categoriaVeiculo);
                corpoEmail.AppendFormat("<b>Observaci�n: </b>{0} <br/>", placa.Observacao);


            }
            corpoEmail.Append("<br/>Cualquier pregunta o solicitud, cont�ctenos a trav�s del Equipo de Registro de Ra�zen.");
            Email.Enviar(usuario.Email, assunto, corpoEmail.ToString());
        }

        public List<ComposicaoAAServicoView> ListarComposicaoAAServico(ComposicaoServicoFiltro filtro)
        {
            List<ComposicaoAAServicoView> placasAA = new List<ComposicaoAAServicoView>();
            using (var repositorio = new UniCadDalRepositorio<Composicao>())
            {
                var query = GetQueryComposicaoServico(filtro, repositorio);
                var retorno = query.ToList();
                if (retorno.Any())
                {
                    PlacaDocumentoBusiness placaDocBll = new PlacaDocumentoBusiness();
                    foreach (var item in retorno)
                    {
                        var placaAA = new ComposicaoAAServicoView();
                        if (item.IDPlaca1.HasValue)
                        {
                            placaAA.PlacaCavaloTruck = _placaBLL.ListarPlacaAAServico(new PlacaServicoFiltro
                            {
                                IDPlaca = item.IDPlaca1.Value,
                                LinhaNegocio = item.LinhaNegocio,
                                Operacao = item.Operacao
                            }).FirstOrDefault();
                            placaAA.TaraComposicao = placaAA.PlacaCavaloTruck.Tara;
                        }
                        if (item.IDPlaca2.HasValue)
                        {
                            placaAA.PlacaCarreta1 = _placaBLL.ListarPlacaAAServico(new PlacaServicoFiltro
                            {
                                IDPlaca = item.IDPlaca2.Value,
                                LinhaNegocio = item.LinhaNegocio,
                                Operacao = item.Operacao
                            }).FirstOrDefault();
                            placaAA.TaraComposicao += placaAA.PlacaCarreta1.Tara;
                        }
                        if (item.IDPlaca3.HasValue)
                        {
                            placaAA.PlacaDollyCarreta2 = _placaBLL.ListarPlacaAAServico(new PlacaServicoFiltro
                            {
                                IDPlaca = item.IDPlaca3.Value,
                                LinhaNegocio = item.LinhaNegocio,
                                Operacao = item.Operacao
                            }).FirstOrDefault();
                            placaAA.TaraComposicao += placaAA.PlacaDollyCarreta2.Tara;
                        }
                        if (item.IDPlaca4.HasValue)
                        {
                            placaAA.PlacaCarreta2 = _placaBLL.ListarPlacaAAServico(new PlacaServicoFiltro
                            {
                                IDPlaca = item.IDPlaca4.Value,
                                LinhaNegocio = item.LinhaNegocio,
                                Operacao = item.Operacao
                            }).FirstOrDefault();
                            placaAA.TaraComposicao += placaAA.PlacaCarreta2.Tara;
                        }
                        placasAA.Add(placaAA);
                    }
                }
                return placasAA;
            }

        }

        private bool PossuiDocumentoVencido(Composicao composicao)
        {
            return PossuiDocumentoVencido(composicao.p1) || PossuiDocumentoVencido(composicao.p2) ||
                PossuiDocumentoVencido(composicao.p3) || PossuiDocumentoVencido(composicao.p4);

		}

        private bool PossuiDocumentoVencido(Placa placa)
        {
            if (placa == null) return false;
            if (placa.Documentos == null) return false;

            foreach (var documento in placa.Documentos)
            {
                if (documento == null) continue;
                if (documento.DocumentoPossuiVencimento && documento.DataVencimento < DateTime.Today)
                    return true;
            }

            return false;
        }

        public bool PodeAprovarComposicaoAutomaticamente(Composicao composicao)
        {
            using (TransactionScope transactionScope = Transactions.CreateTransactionScope(TransactionScopeOption.Required, IsolationLevel.ReadCommitted, 1))
            {
                CarregarDados(composicao, comRessalvas: false);

                // Esta condição é para desligar a demanda DMND0013528 (aprovação automática de nova composição)
                // Retirar quando for para subir a demanda.
                if (composicao.IDComposicao == null)
                {
                    return false;
                }

                // Nao pode aprovar automaticamente se qualquer das placas tiver documentos pendentes:
                if (composicao.IsDocumentosPendentes || PossuiDocumentoVencido(composicao))
                {
                    return false;
                }

                // Nao pode aprovar automaticamente se qualquer das placas nunca tiver tido composicao aprovada:
                if (PossuiPlacaNuncaAprovadaEmOutraComposicao(composicao))
                {
                    return false;
                }

                return true;
            }
        }

        private bool PossuiPlacaNuncaAprovadaEmOutraComposicao(Composicao composicao)
        {
            var placa1aprovadaOuNaoUsada = String.IsNullOrEmpty(composicao.Placa1) || _placaBLL.ObterPlacaAprovada(composicao.Placa1) != null;
            var placa2aprovadaOuNaoUsada = String.IsNullOrEmpty(composicao.Placa2) || _placaBLL.ObterPlacaAprovada(composicao.Placa2) != null;
			var placa3aprovadaOuNaoUsada = String.IsNullOrEmpty(composicao.Placa3) || _placaBLL.ObterPlacaAprovada(composicao.Placa3) != null;
			var placa4aprovadaOuNaoUsada = String.IsNullOrEmpty(composicao.Placa4) || _placaBLL.ObterPlacaAprovada(composicao.Placa4) != null;

			if (placa1aprovadaOuNaoUsada && placa2aprovadaOuNaoUsada && placa3aprovadaOuNaoUsada && placa4aprovadaOuNaoUsada)
            {
                return false;
            }

            return true;
        }
    }
}