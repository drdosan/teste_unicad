using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Raizen.UniCad.Model;
using Raizen.Framework.Log;
using Raizen.UniCad.Model.Filtro;
using Raizen.Framework.Models;
using Raizen.UniCad.DAL;
using Raizen.Framework.Log.Bases;
using Raizen.UniCad.Model.View;
using System.IO;
using Raizen.UniCad.BLL.Util;
using System.Data.OleDb;
using System.Data;
using Raizen.UniCad.Extensions;
using System.Globalization;
using System.Threading;
using System.Text.RegularExpressions;
using Raizen.Framework.Utils.Transacao;
using System.Transactions;

namespace Raizen.UniCad.BLL
{
    public class ImportacaoBusiness : UniCadBusinessBase<Importacao>
    {
        public static Dictionary<int, decimal> Percentual { get; set; }

        public List<ImportacaoView> ListarImportacao(ImportacaoFiltro filtro, PaginadorModel paginador)
        {

            try
            {
                using (UniCadDalRepositorio<Importacao> repositorio = new UniCadDalRepositorio<Importacao>())
                {
                    IQueryable<ImportacaoView> query = this.GetQueryImportacao(filtro, repositorio)
                                                            .Take(paginador.QtdeItensPagina * paginador.PaginaAtual)
                                                            .OrderBy(i => i.Nome)
                                                            .Skip(unchecked((int)paginador.InicioPaginacao));
                    return query.ToList();
                }
            }
            catch (RaizenException RaizenEx)
            {
                RaizenEx.LogarErro();
            }
            catch (Exception ex)
            {
                new RaizenException("", ex).LogarErro();
            }

            return new List<ImportacaoView>();
        }

        public int ListarImportacaoCount(ImportacaoFiltro filtro)
        {
            try
            {
                using (UniCadDalRepositorio<Importacao> repositorio = new UniCadDalRepositorio<Importacao>())
                {
                    IQueryable<ImportacaoView> query = this.GetQueryImportacao(filtro, repositorio);
                    return query.Count();
                }
            }
            catch (RaizenException RaizenEx)
            {
                RaizenEx.LogarErro();
            }
            catch (Exception ex)
            {
                new RaizenException("", ex).LogarErro();
            }

            return 0;
        }

        private IQueryable<ImportacaoView> GetQueryImportacao(ImportacaoFiltro filtro, IUniCadDalRepositorio<Importacao> repositorio)
        {
            IQueryable<ImportacaoView> query = (from app in repositorio.ListComplex<Importacao>().AsNoTracking().OrderBy(i => i.Nome)
                                                select new ImportacaoView() { Anexo = app.Anexo, Status = app.Status, Nome = app.Nome, ID = app.ID, Tipo = app.Tipo, Data = app.Data });
            return query;
        }

        public static void Processo(object obj)
        {
            new ImportacaoBusiness().ProcessarAync(obj);
        }

        public void ProcessarAync(object obj)
        {
            var item = (Importacao)obj;
            try
            {
                List<ErroImportacao> erros = new List<ErroImportacao>();

                if (item.Tipo == (int)EnumTipoImportacao.Placa)
                {
                    ProcessarPlaca(item.ID, item.Anexo, erros);
                }
                if (item.Tipo == (int)EnumTipoImportacao.Composicao)
                {
                    ProcessarComposicao(item.ID, item.Anexo, erros);
                }
                if (erros.Any())
                {
                    item.Status = (int)EnumStatusImportacao.ProcessadoErro;
                    GravarErros(erros);
                }
                else
                    item.Status = (int)EnumStatusImportacao.Processado;
                this.Atualizar(item);
            }
            catch (Exception ex)
            {
                new RaizenException("Importação", ex).LogarErro();
            }

            Percentual[item.ID] = 100;
        }

        private void GravarErros(List<ErroImportacao> erros)
        {
            var erroBll = new ErroImportacaoBusiness();
            int i = 0;
            foreach (var item in erros)
            {
                erroBll.Adicionar(item);
                i++;
            }
        }

        public void ZerarContador(int id)
        {
            if (Percentual != null && Percentual.Any(p => p.Key == id))
                Percentual[id] = 0;
        }

        public void Processar(int id)
        {
            using (UniCadDalRepositorio<Importacao> repositorio = new UniCadDalRepositorio<Importacao>())
            {
                var importacao = this.Selecionar(p => p.ID == id);
                try
                {
                    Percentual = new Dictionary<int, decimal>();
                    Percentual.Add(id, 0);
                    importacao.Status = (int)EnumStatusImportacao.Processando;
                    this.Atualizar(importacao);

                    ParameterizedThreadStart param = new ParameterizedThreadStart(Processo);
                    Thread trh = new Thread(param);
                    trh.Start(importacao);
                }
                catch (Exception ex)
                {
                    importacao.Status = (int)EnumStatusImportacao.ProcessadoErro;
                    this.Atualizar(importacao);
                    new RaizenException("Importação", ex).LogarErro();
                }

            }
        }

        private List<ErroImportacao> ProcessarComposicao(int id, string anexo, List<ErroImportacao> erros)
        {
            List<Composicao> composicoes = ObterComposicoes(id, anexo, erros);
            Percentual[id] = 5;
            ComposicaoBusiness compBll = new ComposicaoBusiness();
            int i = 1;
            foreach (var item in composicoes)
            {
                if (!erros.Any(p => p.Linha == item.Linha))
                {
                    using (TransactionScope transactionScope = Transactions.CreateTransactionScope(TransactionScopeOption.Required, System.Transactions.IsolationLevel.ReadCommitted, 10))
                    {
                        Percentual[id] += (((decimal)100 / composicoes.Count));

                        Composicao compExistente;

                        compExistente = compBll.Selecionar(p => p.IDPlaca1 == item.IDPlaca1 && p.IDStatus == (int)EnumStatusComposicao.Aprovado);
                        item.DataAtualizacao = DateTime.Now;
                        if (compExistente != null)
                        {
                            item.ID = compExistente.ID;
                            compBll.AtualizarComposicao(compExistente, false, enviaEmail : false, idStatus: (int)EnumStatusComposicao.Aprovado);

                            var tipoIntegracao = EnumTipoIntegracaoSAP.Inclusao;
                            var retorno = compBll.IntegrarSAP(compExistente, tipoIntegracao);
                            if (compExistente.jaExiste)
                            {
                                tipoIntegracao = EnumTipoIntegracaoSAP.Alteracao;
                                retorno = compBll.IntegrarSAP(compExistente, tipoIntegracao);
                            }
                            if (!string.IsNullOrEmpty(retorno))
                            {
                                AdicionarErro(EnumErro.IntegracaoSap, id, retorno, item.Linha, erros);
                            }
                            else
                            {
                                transactionScope.Complete();
                            }
                        }
                        else
                        {
                            compBll.AdicionarComposicao(item, false);

                            var tipoIntegracao = EnumTipoIntegracaoSAP.Inclusao;
                            var retorno = compBll.IntegrarSAP(item, tipoIntegracao);
                            if (item != null && item.jaExiste)
                            {
                                tipoIntegracao = EnumTipoIntegracaoSAP.Alteracao;
                                retorno = compBll.IntegrarSAP(item, tipoIntegracao);
                            }
                            if (!string.IsNullOrEmpty(retorno))
                            {
                                AdicionarErro(EnumErro.IntegracaoSap, id, retorno, item.Linha, erros);
                            }
                            else
                            {
                                transactionScope.Complete();
                            }
                        }
                    }
                }
                i++;
            }
            return erros;
        }

        private List<Composicao> ObterComposicoes(int id, string anexo, List<ErroImportacao> errosReturn)
        {
            var dt = CarregarDadosExcel(anexo);
            List<Composicao> composicoes = new List<Composicao>();
            var placaBll = new PlacaBusiness();
            int i = 2;

            if (dt != null)
                foreach (DataRow row in dt.Rows)
                {
                    try
                    {


                        List<ErroImportacao> erros = new List<ErroImportacao>();
                        Composicao composicao = new Composicao();
                        composicao.Linha = i;
                        validarColunasObrigatoriasComposicao(row, id, i, erros);

                        if (!erros.Any())
                        {
                            var tipoComposicao = row[4].ToString().ToLower(CultureInfo.InvariantCulture);
                            var tipoComposicaoEixo = row[5].ToString().ToLower(CultureInfo.InvariantCulture);
                            var categoriaVeiculo = row[7].ToString();
                            var empresa = row[9].ToString();
                            if (tipoComposicao == "truck")
                            {
                                composicao.IDPlaca1 = ValidarPlacaAprovada(row[1].ToString(), id, EnumTipoVeiculo.Truck, "Carreta1", i, erros);
                            }
                            else
                            {
                                composicao.IDPlaca1 = ValidarPlacaAprovada(row[0].ToString(), id, EnumTipoVeiculo.Cavalo, "Cavalo", i, erros);
                                composicao.IDPlaca2 = ValidarPlacaAprovada(row[1].ToString(), id, EnumTipoVeiculo.Carreta, "Carreta1", i, erros);

                                if (tipoComposicao == "bitrem")
                                    composicao.IDPlaca3 = ValidarPlacaAprovada(row[3].ToString(), id, EnumTipoVeiculo.Carreta, "Carreta2", i, erros);
                                else if (tipoComposicao == "bitrem+dolly")
                                {
                                    composicao.IDPlaca3 = ValidarPlacaAprovada(row[2].ToString(), id, EnumTipoVeiculo.Dolly, "Dolly", i, erros);
                                    composicao.IDPlaca4 = ValidarPlacaAprovada(row[3].ToString(), id, EnumTipoVeiculo.Carreta, "Carreta2", i, erros);
                                }
                            }

                            composicao.IDTipoComposicao = new TipoComposicaoBusiness().Selecionar(p => p.Nome == tipoComposicao).ID;
                            composicao.IDTipoComposicaoEixo = new ComposicaoEixoBusiness().Selecionar(p => p.Codigo == tipoComposicaoEixo).ID;
                            composicao.Operacao = row[6].ToString();
                            composicao.IDCategoriaVeiculo = new CategoriaVeiculoBusiness().Selecionar(p => p.Nome == categoriaVeiculo).ID;
                            composicao.IDEmpresa = new EmpresaBusiness().Selecionar(p => p.Nome == empresa).ID;
                            if (!string.IsNullOrEmpty(row[8].ToString()))
                                composicao.PBTC = Convert.ToDouble(row[8].ToString());

                            if (!erros.Any())
                            {
                                composicoes.Add(composicao);
                            }
                        }
                        errosReturn.AddRange(erros);
                        i++;
                    }
                    catch (Exception ex)
                    {
                        new RaizenException("", ex).LogarErro();
                    }
                }
            return composicoes;
        }

        public bool ExcluirImportacao(int id)
        {
            using (TransactionScope transactionScope = Transactions.CreateTransactionScope(1, System.Transactions.IsolationLevel.ReadCommitted))
            {
                ErroImportacaoBusiness erroimportacaoBll = new ErroImportacaoBusiness();
                erroimportacaoBll.ExcluirLista(p => p.IDImportacao == id);
                this.Excluir(id);
                transactionScope.Complete();
                return true;
            }
        }

        private int? ValidarPlacaAprovada(string placaStr, int id, EnumTipoVeiculo tipoVeiculo, string placaN, int index, List<ErroImportacao> erros)
        {
            var placaBll = new PlacaBusiness();
            if (!string.IsNullOrEmpty(placaStr))
            {
                var placaView = placaBll.ObterPlacaAprovada(placaStr);
                if (placaView != null && placaView.ID > 0)
                {
                    var placa = placaBll.Selecionar(placaView.ID);
                    if (placa.IDTipoVeiculo != (int)tipoVeiculo)
                    {
                        AdicionarErro(EnumErro.PlacaTipoInvalido, id, placaN, index, erros);
                    }
                    if (placaBll.PlacaAprovada(placa))
                    {
                        return placa.ID;
                    }
                    else
                    {
                        AdicionarErro(EnumErro.PlacaNaoAprovada, id, placaN, index, erros);
                    }
                }
                else
                {
                    var placas = placaBll.Listar(p => p.PlacaVeiculo == placaStr);
                    if (placas != null && placas.Any())
                    {
                        return placas.OrderByDescending(p => p.ID).ToList().First().ID;
                    }
                    else
                    {
                        AdicionarErro(EnumErro.PlacaNaoExiste, id, placaN, index, erros);
                    }
                }
            }
            return null;
        }

        private void validarColunasObrigatoriasComposicao(DataRow row, int id, int index, List<ErroImportacao> erros)
        {
            if (row[4] == null || string.IsNullOrEmpty(row[4].ToString()))
            {
                AdicionarErro(EnumErro.CampoObrigatorio, id, "Tipo de Composicao", index, erros);
            }
            else
            {
                if (row[4].ToString().ToLower(CultureInfo.InvariantCulture) == "truck")
                {
                    if (row[1] == null || string.IsNullOrEmpty(row[1].ToString()))
                    {
                        AdicionarErro(EnumErro.CampoObrigatorio, id, "Carreta1", index, erros);
                    }
                }
                else
                {
                    if (row[0] == null || string.IsNullOrEmpty(row[0].ToString()))
                    {
                        AdicionarErro(EnumErro.CampoObrigatorio, id, "Cavalo", index, erros);
                    }
                    if (row[1] == null || string.IsNullOrEmpty(row[1].ToString()))
                    {
                        AdicionarErro(EnumErro.CampoObrigatorio, id, "Carreta1", index, erros);
                    }

                    if (row[4].ToString().ToLower(CultureInfo.InvariantCulture) == "bitrem" || row[4].ToString().ToLower(CultureInfo.InvariantCulture) == "bitrem+dolly")
                    {
                        if (row[3] == null || string.IsNullOrEmpty(row[3].ToString()))
                        {
                            AdicionarErro(EnumErro.CampoObrigatorio, id, "Carreta2", index, erros);
                        }
                    }

                    if (row[4].ToString().ToLower(CultureInfo.InvariantCulture) == "bitrem+dolly")
                    {
                        if (row[2] == null || string.IsNullOrEmpty(row[2].ToString()))
                        {
                            AdicionarErro(EnumErro.CampoObrigatorio, id, "Dolly", index, erros);
                        }
                    }
                }
            }

            if (row[5] == null || string.IsNullOrEmpty(row[5].ToString()))
            {
                AdicionarErro(EnumErro.CampoObrigatorio, id, "Cod. Composição", index, erros);
            }

            if (row[6] == null || string.IsNullOrEmpty(row[6].ToString()))
            {
                AdicionarErro(EnumErro.CampoObrigatorio, id, "Operação", index, erros);
            }
            if (row[7] == null || string.IsNullOrEmpty(row[7].ToString()))
            {
                AdicionarErro(EnumErro.CampoObrigatorio, id, "Categoria", index, erros);
            }
            if (row[9] == null || string.IsNullOrEmpty(row[9].ToString()))
            {
                AdicionarErro(EnumErro.CampoObrigatorio, id, "Linha de Negócio", index, erros);
            }
        }

        public decimal Status(int id)
        {
            if (Percentual != null && Percentual.Any(p => p.Key == id))
                return Percentual[id];
            return 0;
        }

        private void ProcessarPlaca(int id, string anexo, List<ErroImportacao> erros)
        {
            Percentual[id] = 5;
            List<Placa> placas = ObterPlacas(id, anexo, erros);

            PlacaBusiness placaBll = new PlacaBusiness();

            int i = 1;
            foreach (var item in placas)
            {
                Percentual[id] += (((decimal)50 / placas.Count));
                if (!erros.Any(p => p.Linha == item.Linha))
                {
                    PlacaView placaExistente = placaBll.ObterPlacaAprovada(item.PlacaVeiculo);
                    item.DataAtualizacao = DateTime.Now;
                    if (placaExistente != null && placaExistente.ID > 0)
                    {
                        item.ID = placaExistente.ID;
                        placaBll.AtualizarPlaca(item);
                    }

                    else
                    {
                        placaBll.AdicionarPlaca(item);
                    }
                }
                i++;
            }
        }

        private List<Placa> ObterPlacas(int id, string anexo, List<ErroImportacao> errosReturn)
        {
            var dt = CarregarDadosExcel(anexo);
            List<Placa> placas = new List<Placa>();
            var placaBll = new PlacaBusiness();
            int i = 2;

            if (dt != null)
            {
                var total = dt.Rows.Count;
                foreach (DataRow row in dt.Rows)
                {

                    try
                    {
                        Percentual[id] += (((decimal)50 / total));
                        List<ErroImportacao> erros = new List<ErroImportacao>();
                        Placa placa = new Placa();
                        placa.Linha = i;
                        ValidarColunasObrigatoriasPlaca(row, id, i, erros);

                        if (!erros.Any())
                        {
                            PreencherPlaca(row, id, placa, i, erros);

                            if (!erros.Any())
                            {
                                if (!erros.Any())
                                {
                                    placas.Add(placa);
                                }
                            }
                        }
                        errosReturn.AddRange(erros);
                    }
                    catch (Exception ex)
                    {
                        new RaizenException("", ex).LogarErro();
                    }
                    i++;

                }
            }
            return placas;
        }

        private void PreencherPlaca(DataRow row, int id, Placa placa, int index, List<ErroImportacao> erros)
        {
            if (!Vazio(row, 26))
                placa.AnoFabricacao = Convert.ToInt32(row[26].ToString());

            if (!Vazio(row, 27))
                placa.AnoModelo = Convert.ToInt32(row[27].ToString());

            if (!Vazio(row, 33))
                placa.BombaDescarga = row[33].ToString().ToLower(CultureInfo.InvariantCulture) == "sim";

            if (!Vazio(row, 32))
                placa.CameraMonitoramento = row[32].ToString().ToLower(CultureInfo.InvariantCulture) == "sim";

            if (!Vazio(row, 25))
                placa.Chassi = row[25].ToString();

            CarregarClientes(row, id, placa, index, erros);

            if (!Vazio(row, 28))
                placa.Cor = row[28].ToString();

            if (!Vazio(row, 35))
            {
                var ibm = row[35].ToString();
                var transp = new TransportadoraBusiness().Selecionar(p => p.IBM.Contains(ibm));
                if (transp != null)
                {
                    placa.CPFCNPJ = transp.CNPJCPF;
                    placa.RazaoSocial = transp.RazaoSocial;
                }
            }
            if (!Vazio(row, 38))
                placa.DataNascimento = Convert.ToDateTime(row[38].ToString());

            CarregarDocumentos(row, id, placa, index, erros);

            if (!Vazio(row, 7) && Val(row, 2) != "cavalo")
            {
                placa.EixosDistanciados = Convert.ToInt32(row[7].ToString()) > 0;
                placa.NumeroEixosDistanciados = Convert.ToInt32(row[7].ToString());
            }

            if (!Vazio(row, 6) && Val(row, 2) != "cavalo")
            {
                placa.EixosPneusDuplos = Convert.ToInt32(row[6].ToString()) > 0;
                placa.NumeroEixosPneusDuplos = Convert.ToInt32(row[6].ToString());
            }

            var categoriaVeiculo = Val(row, 41);
            placa.IDCategoriaVeiculo = new CategoriaVeiculoBusiness().Selecionar(p => p.Nome == categoriaVeiculo).ID;

            var estado = Val(row, 39);
            placa.IDEstado = new EstadoBusiness().Selecionar(p => p.Nome == estado).ID;

            var tipoCarregamento = Val(row, 9);
            placa.IDTipoCarregamento = new TipoCarregamentoBusiness().Selecionar(p => p.Nome.Contains(tipoCarregamento)).ID;

            var tipoProduto = Val(row, 8);
            placa.IDTipoProduto = new TipoProdutoBusiness().Selecionar(p => p.Nome == tipoProduto).ID;

            var tipoVeiculo = Val(row, 2);
            placa.IDTipoVeiculo = new TipoVeiculoBusiness().Selecionar(p => p.Nome == tipoVeiculo).ID;

            if (row[35] != null && !string.IsNullOrEmpty(row[35].ToString()) && row[1].ToString() != "FOB")
            {
                var transportadora = Val(row, 35);
                var transp = new TransportadoraBusiness().Selecionar(p => p.IBM.Contains(transportadora));
                if (transp == null)
                    AdicionarErro(EnumErro.TransportadoraNaoExiste, id, "Transportadora", index, erros);
                else
                    placa.IDTransportadora = transp.ID;
            }

            if (!Vazio(row, 23))
                placa.Marca = Val(row, 23);

            if (!Vazio(row, 24))
                placa.Modelo = Val(row, 24);

            if (!Vazio(row, 10))
                placa.MultiSeta = Val(row, 10) == "sim";

            if (Vazio(row, 28))
                placa.NumeroAntena = Val(row, 30);

            if (!Vazio(row, 5))
                placa.NumeroEixos = Convert.ToInt32(Val(row, 5));

            placa.Operacao = Val(row, 1);
            placa.PlacaVeiculo = Val(row, 0).ToUpper(CultureInfo.InvariantCulture);

            if (!Vazio(row, 34))
                placa.PossuiAbs = Val(row, 34) == "sim";

            if (!Vazio(row, 2))
                placa.Renavam = Val(row, 22);

            placa.Setas = new List<PlacaSeta>();
            CarregarDadosSetas(row, id, 11, 1, placa, index, erros);
            CarregarDadosSetas(row, id, 12, 2, placa, index, erros);
            CarregarDadosSetas(row, id, 13, 3, placa, index, erros);
            CarregarDadosSetas(row, id, 14, 4, placa, index, erros);
            CarregarDadosSetas(row, id, 15, 5, placa, index, erros);
            CarregarDadosSetas(row, id, 16, 6, placa, index, erros);
            CarregarDadosSetas(row, id, 17, 7, placa, index, erros);
            CarregarDadosSetas(row, id, 18, 8, placa, index, erros);
            CarregarDadosSetas(row, id, 19, 9, placa, index, erros);
            CarregarDadosSetas(row, id, 20, 10, placa, index, erros);

            if (!Vazio(row, 21))
            {
                var principal = Convert.ToInt32(Val(row, 21));
                int i = 1;
                if (placa.Setas != null && placa.Setas.Any())
                    foreach (var item in placa.Setas)
                    {
                        if (principal == i)
                        {
                            if (item.VolumeCompartimento1.HasValue && item.VolumeCompartimento1.Value > 0)
                                item.CompartimentoPrincipal1 = true;

                            if (item.VolumeCompartimento2.HasValue && item.VolumeCompartimento2.Value > 0)
                                item.CompartimentoPrincipal2 = true;

                            if (item.VolumeCompartimento3.HasValue && item.VolumeCompartimento3.Value > 0)
                                item.CompartimentoPrincipal3 = true;

                            if (item.VolumeCompartimento4.HasValue && item.VolumeCompartimento4.Value > 0)
                                item.CompartimentoPrincipal4 = true;

                            if (item.VolumeCompartimento5.HasValue && item.VolumeCompartimento5.Value > 0)
                                item.CompartimentoPrincipal5 = true;

                            if (item.VolumeCompartimento6.HasValue && item.VolumeCompartimento6.Value > 0)
                                item.CompartimentoPrincipal6 = true;

                            if (item.VolumeCompartimento7.HasValue && item.VolumeCompartimento7.Value > 0)
                                item.CompartimentoPrincipal7 = true;

                            if (item.VolumeCompartimento8.HasValue && item.VolumeCompartimento8.Value > 0)
                                item.CompartimentoPrincipal8 = true;

                            if (item.VolumeCompartimento9.HasValue && item.VolumeCompartimento9.Value > 0)
                                item.CompartimentoPrincipal9 = true;

                            if (item.VolumeCompartimento10.HasValue && item.VolumeCompartimento10.Value > 0)
                                item.CompartimentoPrincipal10 = true;
                        }
                        i++;
                    }
            }

            if (!Vazio(row, 3))
                placa.Tara = Convert.ToDouble(Val(row, 3))/1000;

            if (!Vazio(row, 29))
                placa.TipoRastreador = Val(row, 29);

            if (!Vazio(row, 31))
                placa.Versao = Val(row, 31);
        }

        private void CarregarClientes(DataRow row, int id, Placa placa, int index, List<ErroImportacao> erros)
        {
            if (!Vazio(row, 42))
            {
                var clientes = Val(row, 42);
                if (clientes.Contains('/'))
                    foreach (var item in clientes.Split('/'))
                    {
                        var cliente = new ClienteBusiness().Selecionar(p => p.IBM.Contains(item.Trim()));
                        if (cliente != null)
                            placa.Clientes.Add(new PlacaClienteView() { IDPlaca = placa.ID, IDCliente = cliente.ID });
                        else
                            AdicionarErro(EnumErro.CampoInvalido, id, "IBM Cliente Permitido -" + item, index, erros);
                    }
                else
                {
                    var cliente = new ClienteBusiness().Selecionar(p => p.IBM.Contains(clientes.Trim()));
                    if (cliente != null)
                        placa.Clientes.Add(new PlacaClienteView() { IDPlaca = placa.ID, IDCliente = cliente.ID });
                    else
                        AdicionarErro(EnumErro.CampoObrigatorio, id, "Clientes Permitidos -" + clientes, index, erros);

                }
            }
            else if (Val(row, 1) == "fob")
                AdicionarErro(EnumErro.CampoObrigatorio, id, "Clientes Permitidos", index, erros);
        }

        private void CarregarDocumentos(DataRow row, int id, Placa placa, int index, List<ErroImportacao> erros)
        {
            var docBll = new TipoDocumentoBusiness();
            placa.Documentos = new List<PlacaDocumentoView>();
            CarregarDoc(row, id, index, "CRLV", 44, 45, placa, erros, docBll);
            CarregarDoc(row, id, index, "CIV", 46, 47, placa, erros, docBll);
            CarregarDoc(row, id, index, "CIPP", 48, 49, placa, erros, docBll);
            CarregarDoc(row, id, index, "CTF", 50, 51, placa, erros, docBll);
            CarregarDoc(row, id, index, "RNTC", 52, 53, placa, erros, docBll);
            CarregarDoc(row, id, index, "CVV", 56, 57, placa, erros, docBll);
            CarregarDoc(row, id, index, "LO", 59, 58, placa, erros, docBll);
        }

        private void CarregarDoc(DataRow row, int id, int index, string docStr, int col1, int col2, Placa placa, List<ErroImportacao> erros, TipoDocumentoBusiness docBll)
        {
            if (!Vazio(row, col1) && !Vazio(row, col2))
            {
                DateTime data;
                if (DateTime.TryParse(Val(row, col2), out data))
                {
                    var doc = docBll.Selecionar(p => p.Sigla == docStr);
                    if (doc != null)
                    {
                        var anexo = Config.GetConfig(EnumConfig.PastaArquivoCarga) + Val(row, col1);
                        placa.Documentos.Add(new PlacaDocumentoView() { IDPlaca = placa.ID, IDTipoDocumento = doc.ID, DataVencimento = data, Anexo = anexo });
                    }
                }
                else
                    AdicionarErro(EnumErro.CampoInvalido, id, "Data Validade " + docStr, index, erros);
            }
        }

        private void CarregarDadosSetas(DataRow row, int id, int colValor, int comp, Placa placa, int index, List<ErroImportacao> erros)
        {

            if (!Vazio(row, colValor))
            {
                if (Val(row, 10) == "sim")
                {
                    var setas = Val(row, colValor).Split('/');
                    int i = 0;
                    foreach (var item in setas)
                    {
                        PlacaSeta seta;

                        if (placa.Setas.Count > i)
                            seta = placa.Setas[i];
                        else
                            seta = new PlacaSeta();

                        if (comp == 1)
                            seta.VolumeCompartimento1 = Convert.ToDecimal(item.Trim()) * 1000;
                        else if (comp == 2)
                            seta.VolumeCompartimento2 = Convert.ToDecimal(item.Trim()) * 1000;
                        else if (comp == 3)
                            seta.VolumeCompartimento3 = Convert.ToDecimal(item.Trim()) * 1000;
                        else if (comp == 4)
                            seta.VolumeCompartimento4 = Convert.ToDecimal(item.Trim()) * 1000;
                        else if (comp == 5)
                            seta.VolumeCompartimento5 = Convert.ToDecimal(item.Trim()) * 1000;
                        else if (comp == 6)
                            seta.VolumeCompartimento6 = Convert.ToDecimal(item.Trim()) * 1000;
                        else if (comp == 7)
                            seta.VolumeCompartimento7 = Convert.ToDecimal(item.Trim()) * 1000;
                        else if (comp == 8)
                            seta.VolumeCompartimento8 = Convert.ToDecimal(item.Trim()) * 1000;
                        else if (comp == 9)
                            seta.VolumeCompartimento9 = Convert.ToDecimal(item.Trim()) * 1000;
                        else if (comp == 10)
                            seta.VolumeCompartimento10 = Convert.ToDecimal(item.Trim()) * 1000;

                        if (placa.Setas.Count > i)
                            placa.Setas[i] = seta;
                        else
                            placa.Setas.Add(seta);

                        i++;
                    }

                }
                else
                {

                    PlacaSeta seta = new PlacaSeta();
                    if (placa.Setas.Any())
                        seta = placa.Setas.First();

                    if (comp == 1)
                        seta.VolumeCompartimento1 = Convert.ToDecimal(Val(row, colValor).Trim());
                    else if (comp == 2)
                        seta.VolumeCompartimento2 = Convert.ToDecimal(Val(row, colValor).Trim());
                    else if (comp == 3)
                        seta.VolumeCompartimento3 = Convert.ToDecimal(Val(row, colValor).Trim());
                    else if (comp == 4)
                        seta.VolumeCompartimento4 = Convert.ToDecimal(Val(row, colValor).Trim());
                    else if (comp == 5)
                        seta.VolumeCompartimento5 = Convert.ToDecimal(Val(row, colValor).Trim());
                    else if (comp == 6)
                        seta.VolumeCompartimento6 = Convert.ToDecimal(Val(row, colValor).Trim());
                    else if (comp == 7)
                        seta.VolumeCompartimento7 = Convert.ToDecimal(Val(row, colValor).Trim());
                    else if (comp == 8)
                        seta.VolumeCompartimento8 = Convert.ToDecimal(Val(row, colValor).Trim());
                    else if (comp == 9)
                        seta.VolumeCompartimento9 = Convert.ToDecimal(Val(row, colValor).Trim());
                    else if (comp == 10)
                        seta.VolumeCompartimento10 = Convert.ToDecimal(Val(row, colValor).Trim());

                    if (!placa.Setas.Any())
                        placa.Setas.Add(seta);

                }
            }
        }

        private void ValidarColunasObrigatoriasPlaca(DataRow row, int id, int index, List<ErroImportacao> erros)
        {
            CamposObrigatoriosGeral(row, id, index, erros);

            if (!Vazio(row, 2) && Val(row, 2) == "carreta" || Val(row, 2) == "truck")
                CamposObrigatoriosComp(row, id, index, erros);

            if (!Vazio(row, 1) && Val(row, 1) != "fob")
                CamposObrigatoriosCif(row, id, index, erros);
            if (!Vazio(row, 1) && Val(row, 1) != "cif")
                CamposObrigatoriosFob(row, id, index, erros);

            CamposObrigatoriosDocs(row, id, index, erros);

        }

        private void CamposObrigatoriosComp(DataRow row, int id, int index, List<ErroImportacao> erros)
        {
            for (int i = 11; i < 21; i++)
            {
                if (!Vazio(row, 10) && Val(row, 10) == "sim" && !Vazio(row, i) && !Val(row, i).Contains("/"))
                    AdicionarErro(EnumErro.CampoInvalido, id, "Comp. " + (i - 10).ToString() + " - Não possui Seta", index, erros);
            }
        }

        private void CamposObrigatoriosDocs(DataRow row, int id, int index, List<ErroImportacao> erros)
        {
            var docBll = new TipoDocumentoBusiness();
            var tipoStr = Val(row, 2);
            var categoriaVeiculo = Val(row, 41);
            var iDCategoriaVeiculo = new CategoriaVeiculoBusiness().Selecionar(p => p.Nome == categoriaVeiculo);

            var tipoProduto = Val(row, 8);
            var iDTipoProduto = new TipoProdutoBusiness().Selecionar(p => p.Nome == tipoProduto);

            var tipoVeiculo = Val(row, 2);
            var iDTipoVeiculo = new TipoVeiculoBusiness().Selecionar(p => p.Nome == tipoVeiculo);

            if (iDCategoriaVeiculo != null && iDTipoProduto != null && iDTipoVeiculo != null)
            {
                var docs = new PlacaDocumentoBusiness().ListarPlacaDocumento(iDTipoVeiculo.ID, iDCategoriaVeiculo.ID, row[1].ToString(), iDTipoProduto.ID);
                if (docs.Any())
                {
                    if (row[1] != null && !string.IsNullOrEmpty(row[1].ToString()))
                        foreach (var item in docs.Where(p => p.Operacao == row[1].ToString()))
                        {
                            ValidarDoc(row, id, index, "CRLV", 43, 44, erros, item);
                            ValidarDoc(row, id, index, "CIV", 45, 46, erros, item);
                            ValidarDoc(row, id, index, "CIPP", 47, 48, erros, item);
                            ValidarDoc(row, id, index, "CTF", 49, 50, erros, item);
                            ValidarDoc(row, id, index, "RNTC", 51, 52, erros, item);
                            ValidarDoc(row, id, index, "CVV", 55, 56, erros, item);
                            ValidarDoc(row, id, index, "LO", 57, 58, erros, item);
                        }
                }
            }
        }

        private void ValidarDoc(DataRow row, int id, int index, string doc, int coluna1, int coluna2, List<ErroImportacao> erros, PlacaDocumentoView item)
        {
            if (item.Sigla == doc && (row[coluna1] == null || string.IsNullOrEmpty(row[coluna1].ToString())))
            {
                AdicionarErro(EnumErro.CampoObrigatorio, id, "Nome " + doc + " (Anexo)", index, erros);
            }
            if (item.Sigla == doc && (row[coluna2] == null || string.IsNullOrEmpty(row[coluna2].ToString())))
            {
                AdicionarErro(EnumErro.CampoObrigatorio, id, "Validade " + doc + "", index, erros);
            }
        }


        private void CamposObrigatoriosFob(DataRow row, int id, int index, List<ErroImportacao> erros)
        {
            if (row[42] == null || string.IsNullOrEmpty(row[42].ToString()))
            {
                AdicionarErro(EnumErro.CampoObrigatorio, id, "Clientes", index, erros);
            }
        }

        private void CamposObrigatoriosCif(DataRow row, int id, int index, List<ErroImportacao> erros)
        {
            if (row[35] == null || string.IsNullOrEmpty(row[35].ToString()))
            {
                AdicionarErro(EnumErro.CampoObrigatorio, id, "Transportadora", index, erros);
            }
            else
            {
                var transportadora = row[35].ToString();
                if (new TransportadoraBusiness().Selecionar(p => p.IBM.Contains(transportadora)) == null)
                    AdicionarErro(EnumErro.TransportadoraNaoExiste, id, "Transportadora", index, erros);
            }
        }

        private void CamposObrigatoriosGeral(DataRow row, int id, int index, List<ErroImportacao> erros)
        {
            row[0] = row[0].ToString().Replace(" ", string.Empty).Replace("-", string.Empty);
            Regex regex = new Regex(@"^[a-zA-Z]{3}\d{4}$");
            if (Vazio(row, 0))
                AdicionarErro(EnumErro.CampoObrigatorio, id, "Placa", index, erros);
            else if (!regex.Match(row[0].ToString()).Success)
                AdicionarErro(EnumErro.CampoInvalido, id, "Placa", index, erros);

            if (Vazio(row, 1))
                AdicionarErro(EnumErro.CampoObrigatorio, id, "Operação", index, erros);

            if (Vazio(row, 35))
            {
                AdicionarErro(EnumErro.CampoObrigatorio, id, "Transportadora Comb", index, erros);
            }
            else
            {
                var ibm = row[35].ToString();
                var transp = new TransportadoraBusiness().Selecionar(p => p.IBM.Contains(ibm));
                if (transp == null || string.IsNullOrEmpty(transp.CNPJCPF))
                {
                    AdicionarErro(EnumErro.CampoInvalido, id, "Transportadora Comb (N Existe ou CNPJ)", index, erros);
                }
            }

            if (Vazio(row, 41))
                AdicionarErro(EnumErro.CampoObrigatorio, id, "Categoria do Veículo", index, erros);

            else if (Val(row, 1) != "cif" && Val(row, 1) != "fob")
                AdicionarErro(EnumErro.CampoInvalido, id, "Operação", index, erros);

            if (Vazio(row, 2))
                AdicionarErro(EnumErro.CampoObrigatorio, id, "Tipo", index, erros);
            else if (row[2].ToString().ToLower(CultureInfo.InvariantCulture) != "cavalo" && row[2].ToString().ToLower(CultureInfo.InvariantCulture) != "carreta" && row[2].ToString().ToLower(CultureInfo.InvariantCulture) != "truck" && row[2].ToString().ToLower(CultureInfo.InvariantCulture) != "dolly")
                AdicionarErro(EnumErro.CampoInvalido, id, "Tipo", index, erros);

            if (Vazio(row, 3))
                AdicionarErro(EnumErro.CampoObrigatorio, id, "Tara", index, erros);
            else
            {
                var tar = row[3].ToString();
                decimal tara = 0;
                decimal.TryParse(tar, out tara);

                if (tara == 0)
                    AdicionarErro(EnumErro.CampoInvalido, id, "Tara", index, erros);
            }

            //if (Vazio(row, 4))
            //    AdicionarErro(EnumErro.CampoObrigatorio, id, "PBT", index, erros);
            //else
            //{
            //    var pbt = row[4].ToString();
            //    decimal pbtc = 0;
            //    decimal.TryParse(pbt, out pbtc);

            //    if (pbtc == 0)
            //        AdicionarErro(EnumErro.CampoInvalido, id, "PBT", index, erros);
            //}
            if (Vazio(row, 5))
                AdicionarErro(EnumErro.CampoObrigatorio, id, "Nº Eixos", index, erros);
            else if (!row[5].ToString().IsNumeric())
                AdicionarErro(EnumErro.CampoInvalido, id, "Nº Eixos", index, erros);

            if (Vazio(row, 6) && Val(row, 2) != "cavalo")
                AdicionarErro(EnumErro.CampoObrigatorio, id, "Pneus Duplos", index, erros);
            if (Vazio(row, 7) && Val(row, 2) != "cavalo")
                AdicionarErro(EnumErro.CampoObrigatorio, id, "Eixos distanciados", index, erros);

            if (Vazio(row, 8) && !Vazio(row, 2))
                if (Val(row, 2) == "carreta" || Val(row, 2) == "truck")
                    AdicionarErro(EnumErro.CampoObrigatorio, id, "Tipo de Produto", index, erros);

            if (Vazio(row, 9) && !Vazio(row, 2))
                if (Val(row, 2) == "carreta" || Val(row, 2) == "truck")
                    AdicionarErro(EnumErro.CampoObrigatorio, id, "Modalidade de Carregamento", index, erros);

            if (Vazio(row, 10) && !Vazio(row, 2))
            {
                if (Val(row, 2) == "carreta" || Val(row, 2) == "truck")
                    AdicionarErro(EnumErro.CampoObrigatorio, id, "MultiSeta", index, erros);
            }
            else if (Val(row, 10) != "sim" && Val(row, 10) != "não")
                AdicionarErro(EnumErro.CampoInvalido, id, "MultiSeta", index, erros);

            if (Vazio(row, 39) && !Vazio(row, 1) && Val(row, 1) != "cif")
                AdicionarErro(EnumErro.CampoObrigatorio, id, "UF", index, erros);

            if (Vazio(row, 40))
                AdicionarErro(EnumErro.CampoObrigatorio, id, "Arrendado", index, erros);
            else if (Val(row, 40) != "sim" && Val(row, 40) != "não")
                AdicionarErro(EnumErro.CampoInvalido, id, "Arrendado", index, erros);

            if (Vazio(row, 41))
                AdicionarErro(EnumErro.CampoObrigatorio, id, "Categoria do Veículo", index, erros);
            else if (Val(row, 41) != "particular" && Val(row, 41) != "aluguel")
                AdicionarErro(EnumErro.CampoInvalido, id, "Categoria do Veículo", index, erros);

        }

        private bool Vazio(DataRow row, int i)
        {
            return (row[i] == null || string.IsNullOrEmpty(row[i].ToString()));
        }

        private string Val(DataRow row, int i)
        {
            return row[i].ToString().ToLower(CultureInfo.InvariantCulture);
        }

        private void AdicionarErro(EnumErro erro, int id, string campo, int index, List<ErroImportacao> erros)
        {
            var newerro = new ErroImportacao()
            {
                Descricao = string.Format("{0} - {1}", EnumExtensions.GetDescription(erro), campo),
                Linha = index,
                IDImportacao = id
            };
            erros.Add(newerro);
        }

        private DataTable CarregarDadosExcel(string anexo)
        {
            string HDR = "YES";

            var fileFullPath = Config.GetConfig(EnumConfig.CaminhoAnexos) + "\\" + anexo;

            string ConStr = string.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=\"Excel 12.0;HDR={1};IMEX=1\"", fileFullPath, HDR);
            using (OleDbConnection oleDbConnection = new OleDbConnection(ConStr))
            {
                oleDbConnection.Open();
                DataTable dtSheet = oleDbConnection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);

                //itera por aba
                foreach (DataRow drSheet in dtSheet.Rows)
                {
                    //nome da aba
                    string sheetname = drSheet["TABLE_NAME"].ToString();
                    if (sheetname == "Placas$" || sheetname == "Composição$")
                    {
                        //aba com nome inválido, teste próxima.
                        if (!sheetname.Contains("$") || sheetname.Contains("_Filter"))
                            continue;

                        ////se não está na lista de planilhas obrigatórias, ignorar.
                        //if (!this.TabelasObrigatorias.Contains(sheetname.Replace("$", "")))
                        //    continue;

                        //carrega datatable com dados da aba
                        OleDbCommand oleDbCommand = new OleDbCommand("select * from [" + sheetname + "]", oleDbConnection);
                        OleDbDataAdapter oleDbAdapter = new OleDbDataAdapter(oleDbCommand);
                        DataTable dt = new DataTable();
                        oleDbAdapter.Fill(dt);

                        RemoverLinhasNulasDataTable(dt);

                        return dt;
                    }
                }
            }
            return null;
        }

        private void RemoverLinhasNulasDataTable(DataTable dt)
        {
            //percorre todas as linhas
            for (int i = dt.Rows.Count - 1; i >= 0; i--)
            {
                if (dt.Rows[i][0] != DBNull.Value && dt.Rows[i][1] != DBNull.Value)
                {
                    dt.Rows[i].Delete();
                    break;
                }
            }

            dt.AcceptChanges();
        }

        private string RemoverCaracteresEspeciais(string str)
        {
            StringBuilder sb = new StringBuilder();
            byte[] bytes = System.Text.Encoding.GetEncoding("iso-8859-8").GetBytes(str);
            string strSemAcentos = System.Text.Encoding.UTF8.GetString(bytes);
            strSemAcentos = strSemAcentos.Replace("$", "S"); //este caracter é mantido na conversão para byte, tem que alterar manualmente
            foreach (char c in strSemAcentos)
            {
                if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == '_' || c == ' ')
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }


    }
}

