using Raizen.Framework.Log.Bases;
using Raizen.Framework.Models;
using Raizen.Framework.Utils.Transacao;
using Raizen.UniCad.BLL.Interfaces;
using Raizen.UniCad.BLL.Util;
using Raizen.UniCad.DAL;
using Raizen.UniCad.Extensions;
using Raizen.UniCad.Model;
using Raizen.UniCad.Model.Filtro;
using Raizen.UniCad.Model.View;
using Raizen.UniCad.Web.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Transactions;
using IsolationLevel = System.Transactions.IsolationLevel;

namespace Raizen.UniCad.BLL
{
    public class PlacaBusiness : UniCadBusinessBase<Placa>, IPlacaBusiness
    {
        private readonly EnumPais _pais;

        public PlacaBusiness()
        {
            _pais = EnumPais.Brasil;
        }

        public PlacaBusiness(EnumPais pais)
        {
            _pais = pais;
        }

        private List<PlacaServicoView> GetQueryPlacaServico(PlacaServicoFiltro filtro)
        {
            var paramLinhaNegocio = new SqlParameter("@LinhaNegocio", SqlDbType.Int);
            var paramOperacao = new SqlParameter("@Operacao", SqlDbType.VarChar);
            var paramPlaca = new SqlParameter("@Placa", SqlDbType.VarChar);
            var paramIdPlaca = new SqlParameter("IDPlaca", SqlDbType.Int);

            paramLinhaNegocio.Value = filtro.LinhaNegocio ?? (object)DBNull.Value;
            paramOperacao.Value = filtro.Operacao ?? (object)DBNull.Value;
            paramPlaca.Value = filtro.PlacaVeiculo ?? (object)DBNull.Value;
            paramIdPlaca.Value = filtro.IDPlaca ?? (object)DBNull.Value;

            List<PlacaServicoView> dadosRelatorio = ExecutarProcedureComRetorno<PlacaServicoView>(
                "[dbo].[Proc_Listar_Placas_Servico] @LinhaNegocio,@Operacao,@Placa,@IDPlaca",
                new object[] { paramLinhaNegocio, paramOperacao, paramPlaca, paramIdPlaca });
            return dadosRelatorio;
        }

        public List<PlacaServicoView> ListarPlacaServico(PlacaServicoFiltro filtro)
        {
            using (UniCadDalRepositorio<Motorista> repositorio = new UniCadDalRepositorio<Motorista>())
            {
                var resultado = GetQueryPlacaServico(filtro);
                if (resultado != null && resultado.Any())
                    resultado.ForEach(x =>
                    {
                        x.ListaCompartimentos = ListarCompartimentosPorIdPlaca(x.ID, repositorio);
                        x.ListaDocumentos = ListarDocumentosPorIdPlaca(x.ID, repositorio);
                        x.ListaPermissoes = ListarPermissoesPorIdPlaca(x.ID, x.Operacao, repositorio);
                    });
                return resultado;
            }
        }

        private List<PlacaPermissaoServicoView> ListarPermissoesPorIdPlaca(int Id, string Operacao, UniCadDalRepositorio<Motorista> repositorio)
        {
            IQueryable<PlacaPermissaoServicoView> permissoes;
            if (Operacao == "FOB")
                permissoes = (from app in repositorio.ListComplex<Placa>().AsNoTracking().OrderBy(i => i.ID)
                              join pc in repositorio.ListComplex<PlacaCliente>().AsNoTracking() on app.ID equals pc.IDPlaca
                              join cli in repositorio.ListComplex<Cliente>().AsNoTracking() on pc.IDCliente equals cli.ID
                              where (app.ID == Id)
                              select new PlacaPermissaoServicoView
                              {
                                  IBM = cli.IBM,
                                  CpfCnpj = cli.CNPJCPF,
                                  NomeRazaoSocial = cli.RazaoSocial
                              });
            else
            {
                permissoes = (from app in repositorio.ListComplex<Placa>().AsNoTracking().OrderBy(i => i.ID)
                              join transp in repositorio.ListComplex<Transportadora>().AsNoTracking() on app.IDTransportadora equals transp.ID
                              where (app.ID == Id)
                              select new PlacaPermissaoServicoView
                              {
                                  IBM = transp.IBM,
                                  CpfCnpj = transp.CNPJCPF,
                                  NomeRazaoSocial = transp.RazaoSocial
                              });
            }
            return permissoes.ToList();
        }


        public List<int> ListarClientesPorPlaca(int idPlaca)
        {
            List<int> listClientes = null;

            using (UniCadDalRepositorio<PlacaCliente> repositorio = new UniCadDalRepositorio<PlacaCliente>())
            {
                var listPlacaCliente = repositorio.ListComplex<PlacaCliente>().Where(p => p.IDPlaca == idPlaca);

                if (listPlacaCliente != null && listPlacaCliente.Count() > 0)
                {
                    listClientes = new List<int>();

                    foreach (var placaCliente in listPlacaCliente)
                    {
                        listClientes.Add(placaCliente.IDCliente);
                    }
                }
            }

            return listClientes;
        }

        private List<PlacaDocumentoServicoView> ListarDocumentosPorIdPlaca(int Id, UniCadDalRepositorio<Motorista> repositorio)
        {
            var documentos = (from app in repositorio.ListComplex<Placa>().AsNoTracking().OrderBy(i => i.ID)
                              join docs in repositorio.ListComplex<PlacaDocumento>().AsNoTracking() on app.ID equals docs.IDPlaca
                              join tipoDoc in repositorio.ListComplex<TipoDocumento>().AsNoTracking() on docs.IDTipoDocumento equals tipoDoc.ID
                              where (app.ID == Id)
                              select new PlacaDocumentoServicoView
                              {
                                  Sigla = tipoDoc.Sigla,
                                  Descricao = tipoDoc.Descricao,
                                  DataVencimento = docs.DataVencimento
                              });
            return documentos.ToList();
        }
        private List<PlacaCompartimentoServicoView> ListarCompartimentosPorIdPlaca(int Id, UniCadDalRepositorio<Motorista> repositorio)
        {
            var compartimento = (from app in repositorio.ListComplex<Placa>().AsNoTracking().OrderBy(i => i.ID)
                                 join compart in repositorio.ListComplex<PlacaSeta>().AsNoTracking() on app.ID equals compart.IDPlaca
                                 where (app.ID == Id)
                                 orderby compart.ID
                                 select new PlacaCompartimentoServicoView
                                 {
                                     CapacidadeComp1 = compart.VolumeCompartimento1 ?? 0,
                                     CapacidadeComp2 = compart.VolumeCompartimento2 ?? 0,
                                     CapacidadeComp3 = compart.VolumeCompartimento3 ?? 0,
                                     CapacidadeComp4 = compart.VolumeCompartimento4 ?? 0,
                                     CapacidadeComp5 = compart.VolumeCompartimento5 ?? 0,
                                     CapacidadeComp6 = compart.VolumeCompartimento6 ?? 0,
                                     CapacidadeComp7 = compart.VolumeCompartimento7 ?? 0,
                                     CapacidadeComp8 = compart.VolumeCompartimento8 ?? 0,
                                     CapacidadeComp9 = compart.VolumeCompartimento9 ?? 0,
                                     CapacidadeComp10 = compart.VolumeCompartimento10 ?? 0,
                                     IsCompartimentoPrincipal1 = compart.CompartimentoPrincipal1 ?? false,
                                     IsCompartimentoPrincipal2 = compart.CompartimentoPrincipal2 ?? false,
                                     IsCompartimentoPrincipal3 = compart.CompartimentoPrincipal3 ?? false,
                                     IsCompartimentoPrincipal4 = compart.CompartimentoPrincipal4 ?? false,
                                     IsCompartimentoPrincipal5 = compart.CompartimentoPrincipal5 ?? false,
                                     IsCompartimentoPrincipal6 = compart.CompartimentoPrincipal6 ?? false,
                                     IsCompartimentoPrincipal7 = compart.CompartimentoPrincipal7 ?? false,
                                     IsCompartimentoPrincipal8 = compart.CompartimentoPrincipal8 ?? false,
                                     IsCompartimentoPrincipal9 = compart.CompartimentoPrincipal9 ?? false,
                                     IsCompartimentoPrincipal10 = compart.CompartimentoPrincipal10 ?? false,

                                 }).ToList()
                                 .Select((p, r) => new PlacaCompartimentoServicoView()
                                 {
                                     CapacidadeComp1 = p.CapacidadeComp1,
                                     CapacidadeComp2 = p.CapacidadeComp2,
                                     CapacidadeComp3 = p.CapacidadeComp3,
                                     CapacidadeComp4 = p.CapacidadeComp4,
                                     CapacidadeComp5 = p.CapacidadeComp5,
                                     CapacidadeComp6 = p.CapacidadeComp6,
                                     CapacidadeComp7 = p.CapacidadeComp7,
                                     CapacidadeComp8 = p.CapacidadeComp8,
                                     CapacidadeComp9 = p.CapacidadeComp9,
                                     CapacidadeComp10 = p.CapacidadeComp10,
                                     IsCompartimentoPrincipal1 = p.IsCompartimentoPrincipal1,
                                     IsCompartimentoPrincipal2 = p.IsCompartimentoPrincipal2,
                                     IsCompartimentoPrincipal3 = p.IsCompartimentoPrincipal3,
                                     IsCompartimentoPrincipal4 = p.IsCompartimentoPrincipal4,
                                     IsCompartimentoPrincipal5 = p.IsCompartimentoPrincipal5,
                                     IsCompartimentoPrincipal6 = p.IsCompartimentoPrincipal6,
                                     IsCompartimentoPrincipal7 = p.IsCompartimentoPrincipal7,
                                     IsCompartimentoPrincipal8 = p.IsCompartimentoPrincipal8,
                                     IsCompartimentoPrincipal9 = p.IsCompartimentoPrincipal9,
                                     IsCompartimentoPrincipal10 = p.IsCompartimentoPrincipal10,
                                     NumSeta = r + 1
                                 }).ToList();

            return compartimento;

        }

        public List<Placa> ListarPlacaSemComposicao(PlacaFiltro filtro, PaginadorModel paginador)
        {
            using (UniCadDalRepositorio<Placa> repositorio = new UniCadDalRepositorio<Placa>())
            {
                IQueryable<Placa> query = GetQueryPlacaSemComposicao(filtro, repositorio)
                                                        .Take(paginador.QtdeItensPagina * paginador.PaginaAtual)
                                                        .OrderBy(i => i.ID)
                                                        .Skip(unchecked((int)paginador.InicioPaginacao));
                return query.ToList();
            }
        }

        public int ListarPlacaSemComposicaoCount(PlacaFiltro filtro)
        {
            using (UniCadDalRepositorio<Placa> repositorio = new UniCadDalRepositorio<Placa>())
            {
                IQueryable<Placa> query = GetQueryPlacaSemComposicao(filtro, repositorio);

                return query.Count();
            }
        }

        private IQueryable<Placa> GetQueryPlacaSemComposicao(PlacaFiltro filtro, UniCadDalRepositorio<Placa> repositorio)
        {
            IQueryable<string> query1 = (from app in repositorio.ListComplex<Placa>().AsNoTracking().OrderBy(i => i.ID)
                                         join comp1 in repositorio.ListComplex<Composicao>().AsNoTracking() on app.ID equals comp1.IDPlaca1
                                         into j1
                                         from j2 in j1.DefaultIfEmpty()
                                         join comp2 in repositorio.ListComplex<Composicao>().AsNoTracking() on app.ID equals comp2.IDPlaca2
                                          into j3
                                         from j4 in j3.DefaultIfEmpty()
                                         join comp3 in repositorio.ListComplex<Composicao>().AsNoTracking() on app.ID equals comp3.IDPlaca3
                                          into j5
                                         from j6 in j5.DefaultIfEmpty()
                                         join comp4 in repositorio.ListComplex<Composicao>().AsNoTracking() on app.ID equals comp4.IDPlaca4
                                          into j7
                                         from j8 in j7.DefaultIfEmpty()
                                         where (j1.Any(p => p.IDStatus != 3) || j3.Any(p => p.IDStatus != 3) || j5.Any(p => p.IDStatus != 3) || j7.Any(p => p.IDStatus != 3))
                                         && (app.PlacaVeiculo == filtro.PlacaVeiculo || string.IsNullOrEmpty(filtro.PlacaVeiculo))
                                         select app.PlacaVeiculo);

            IQueryable<Placa> query = (from app in repositorio.ListComplex<Placa>().AsNoTracking().OrderBy(i => i.ID)
                                       where (app.PlacaVeiculo == filtro.PlacaVeiculo || string.IsNullOrEmpty(filtro.PlacaVeiculo))
                                       && (DbFunctions.TruncateTime(app.DataAtualizacao) == filtro.DataAtualizacao || !filtro.DataAtualizacao.HasValue)
                                       select app);

            return query.Where(b => !query1.Any(p => p == b.PlacaVeiculo));
        }


        public List<Placa> ListarPlacaPorOperacaoLinhaNegocio(PlacaFiltro placa)
        {
            using (UniCadDalRepositorio<Placa> repositorio = new UniCadDalRepositorio<Placa>())
            {
                IQueryable<Placa> query = GetQueryPlacaPorOperacaoELinhaDeNegocio(placa, repositorio);
                return query.Distinct().ToList();
            }
        }

        private IQueryable<Placa> GetQueryPlacaPorOperacaoELinhaDeNegocio(PlacaFiltro filtro, UniCadDalRepositorio<Placa> repositorio)
        {
            var query = (from app in repositorio.ListComplex<Placa>().AsNoTracking().OrderBy(i => i.ID)
                         join comp1 in repositorio.ListComplex<Composicao>().AsNoTracking() on app.ID equals comp1.IDPlaca1
                         into j1
                         from j5 in j1.DefaultIfEmpty()
                         join comp2 in repositorio.ListComplex<Composicao>().AsNoTracking() on app.ID equals comp2.IDPlaca2
                         into j2
                         from j6 in j2.DefaultIfEmpty()
                         join comp3 in repositorio.ListComplex<Composicao>().AsNoTracking() on app.ID equals comp3.IDPlaca3
                         into j3
                         from j7 in j3.DefaultIfEmpty()
                         join comp4 in repositorio.ListComplex<Composicao>().AsNoTracking() on app.ID equals comp4.IDPlaca4
                         into j4
                         from j8 in j4.DefaultIfEmpty()
                         where (j5.IDEmpresa == filtro.IDEmpresa || j6.IDEmpresa == filtro.IDEmpresa || j7.IDEmpresa == filtro.IDEmpresa || j8.IDEmpresa == filtro.IDEmpresa)
                         && (app.Operacao == filtro.Operacao)
                         && (app.PlacaVeiculo == filtro.PlacaVeiculo)
                         && (app.ID != filtro.Id)
                         select app);
            return query;
        }

        private IQueryable<Placa> GetQuerySelecionarPlacaComposicaoAprovada(Placa filtro, UniCadDalRepositorio<Placa> repositorio)
        {

            IQueryable<Placa> query = (from app in repositorio.ListComplex<Placa>().AsNoTracking().OrderBy(i => i.ID)
                                       join comp1 in repositorio.ListComplex<Composicao>().AsNoTracking() on app.ID equals comp1.IDPlaca1
                                       into j1
                                       from j5 in j1.DefaultIfEmpty()
                                       join comp2 in repositorio.ListComplex<Composicao>().AsNoTracking() on app.ID equals comp2.IDPlaca2
                                       into j2
                                       from j6 in j2.DefaultIfEmpty()
                                       join comp3 in repositorio.ListComplex<Composicao>().AsNoTracking() on app.ID equals comp3.IDPlaca3
                                       into j3
                                       from j7 in j3.DefaultIfEmpty()
                                       join comp4 in repositorio.ListComplex<Composicao>().AsNoTracking() on app.ID equals comp4.IDPlaca4
                                       into j4
                                       from j8 in j4.DefaultIfEmpty()
                                       orderby app.ID descending
                                       where (app.PlacaVeiculo == filtro.PlacaVeiculo)
                                       && (app.Operacao == filtro.Operacao)
                                       && ((j5.IDStatus == (int)EnumStatusComposicao.Aprovado || j5.IDStatus == (int)EnumStatusComposicao.Bloqueado)
                                        || (j6.IDStatus == (int)EnumStatusComposicao.Aprovado || j6.IDStatus == (int)EnumStatusComposicao.Bloqueado)
                                        || (j7.IDStatus == (int)EnumStatusComposicao.Aprovado || j7.IDStatus == (int)EnumStatusComposicao.Bloqueado)
                                        || (j8.IDStatus == (int)EnumStatusComposicao.Aprovado || j8.IDStatus == (int)EnumStatusComposicao.Bloqueado))
                                       select app);
            return query;
        }

        public bool ListarPorIdComposicao(string placa, int idComp)
        {
            using (UniCadDalRepositorio<Placa> PlacaRepositorio = new UniCadDalRepositorio<Placa>())
            {
                var query = from placas in PlacaRepositorio.ListComplex<Placa>().AsNoTracking()
                            join comp in PlacaRepositorio.ListComplex<Composicao>().AsNoTracking() on placas.PlacaVeiculo equals placa into c1
                            from c2 in c1.DefaultIfEmpty()
                            where (placas.PlacaVeiculo == placa)
                            && (c2.IDPlaca1 == placas.ID
                                || c2.IDPlaca2 == placas.ID
                                || c2.IDPlaca3 == placas.ID
                                || c2.IDPlaca4 == placas.ID)
                            && (c2.ID != idComp)
                            orderby placas.ID descending
                            select placas;
                return query.Any();
            }
        }

        public bool ExcluirPlaca(int id)
        {
            ComposicaoBusiness bll = new ComposicaoBusiness();
            var comp = bll.Selecionar(p => p.IDPlaca1 == id);
            if (comp != null)
                bll.ExcluirComposicao(comp.ID, true);
            comp = bll.Selecionar(p => p.IDPlaca2 == id);
            if (comp != null)
                bll.ExcluirComposicao(comp.ID, true);
            comp = bll.Selecionar(p => p.IDPlaca3 == id);
            if (comp != null)
                bll.ExcluirComposicao(comp.ID, true);
            comp = bll.Selecionar(p => p.IDPlaca4 == id);
            if (comp != null)
                bll.ExcluirComposicao(comp.ID, true);

            PlacaDocumentoBusiness placaDocBll = new PlacaDocumentoBusiness();
            placaDocBll.ExcluirListaDoc(id);
            PlacaSetaBusiness placaSetaBll = new PlacaSetaBusiness();
            placaSetaBll.ExcluirLista(p => p.IDPlaca == id);
            PlacaClienteBusiness placaClienteBll = new PlacaClienteBusiness();
            placaClienteBll.ExcluirLista(p => p.IDPlaca == id);

            var placa = Selecionar(id).PlacaVeiculo;
            try
            {
                var lista = this.Listar(p => p.PlacaVeiculo == placa);
                foreach (var item in lista)
                {
                    placaDocBll.ExcluirListaDoc(item.ID);
                    placaSetaBll.ExcluirLista(p => p.IDPlaca == item.ID);
                    placaClienteBll.ExcluirLista(p => p.IDPlaca == item.ID);
                    Excluir(item.ID);
                }
            }
            catch (Exception ex)
            {
                new RaizenException($"{Traducao.GetTextoPorLingua("Excluindo placa", "Eliminando patente", _pais)} - ", ex).LogarErro();
            }

            //Excluir(id);

            return true;
        }

        public List<Placa> ListarPlaca(PlacaFiltro filtro, PaginadorModel paginador)
        {
            using (UniCadDalRepositorio<Placa> repositorio = new UniCadDalRepositorio<Placa>())
            {
                IQueryable<Placa> query = GetQueryPlaca(filtro, repositorio)
                                                        .Take(paginador.QtdeItensPagina * paginador.PaginaAtual)
                                                        .OrderBy(i => i.ID)
                                                        .Skip(unchecked((int)paginador.InicioPaginacao));
                return query.ToList();
            }
        }

        public bool VerificarPlacaJaUsada(string placa, String operacao, int idEmpresa, int? idComposicao, EnumTipoComposicao tipo, int idCliente, int NumeroPlaca)
        {
            using (UniCadDalRepositorio<Placa> repositorio = new UniCadDalRepositorio<Placa>())
            {
                return GetPlacaJaUsada(placa, operacao, idEmpresa, idComposicao, tipo, idCliente, repositorio, NumeroPlaca).Any();
            }
        }

        private IQueryable<Placa> GetPlacaJaUsada(string placa, String operacao, int idEmpresa, int? idComposicao, EnumTipoComposicao tipo, int idCliente, IUniCadDalRepositorio<Placa> repositorio, int NumeroPlaca)
        {
            if (NumeroPlaca == 1)
            {
                IQueryable<Placa> query = (from p in repositorio.ListComplex<Placa>().AsNoTracking().OrderBy(i => i.ID)
                                           join composicao in repositorio.ListComplex<Composicao>().AsNoTracking() on p.ID equals composicao.IDPlaca1.Value
                                           where (composicao.Operacao == operacao)
                                           && (composicao.IDStatus == (int)EnumStatusComposicao.Aprovado)
                                           && (!idComposicao.HasValue || composicao.ID != idComposicao)
                                           && (p.PlacaVeiculo == placa)
                                           && (composicao.IDEmpresa == idEmpresa || composicao.IDEmpresa == 3 || idEmpresa == 3)
                                           select p);
                return query;
            }
            else
            {
                IQueryable<Placa> query = (from p in repositorio.ListComplex<Placa>().AsNoTracking().OrderBy(i => i.ID)
                                           join composicao in repositorio.ListComplex<Composicao>().AsNoTracking() on p.ID equals composicao.IDPlaca2.Value
                                           where (composicao.Operacao == operacao)
                                           && (composicao.IDStatus == (int)EnumStatusComposicao.Aprovado)
                                           && (!idComposicao.HasValue || composicao.ID != idComposicao)
                                           && (p.PlacaVeiculo == placa)
                                           && (composicao.IDEmpresa == idEmpresa || composicao.IDEmpresa == 3 || idEmpresa == 3)
                                           select p);
                return query;
            }
        }

        public int ListarPlacaCount(PlacaFiltro filtro)
        {

            using (UniCadDalRepositorio<Placa> repositorio = new UniCadDalRepositorio<Placa>())
            {
                IQueryable<Placa> query = GetQueryPlaca(filtro, repositorio);
                return query.Count();
            }

        }

        private List<PlacaView> GetQueryPlacaCompleta(ComposicaoFiltro filtro)
        {

            SqlParameter paramIdEmpresa = new SqlParameter("@IdEmpresa", SqlDbType.Int);
            SqlParameter paramIdPlacaOficial = new SqlParameter("@IdPlacaOficial", SqlDbType.Int);
            SqlParameter paramPlaca = new SqlParameter("@Placa", SqlDbType.VarChar);
            SqlParameter paramOperacao = new SqlParameter("@Operacao", SqlDbType.VarChar);
            SqlParameter paramDataInicio = new SqlParameter("@DataInicio", SqlDbType.DateTime);
            SqlParameter paramDataFim = new SqlParameter("@DataFim", SqlDbType.DateTime);
            SqlParameter paramIdTransportadora = new SqlParameter("@IDTransportadora", SqlDbType.Int);
            SqlParameter paramIdcLiente = new SqlParameter("@IDCLiente", SqlDbType.Int);
            SqlParameter paramIdStatus = new SqlParameter("@IDStatus", SqlDbType.Int);
            SqlParameter paramIdUsuarioCliente = new SqlParameter("@IDUsuarioCliente", SqlDbType.Int);
            SqlParameter paramIdUsuarioTransportadora = new SqlParameter("@IDUsuarioTransportadora", SqlDbType.Int);
            SqlParameter paramChamado = new SqlParameter("@Chamado", SqlDbType.VarChar);
            SqlParameter paramIdTipoComposicao = new SqlParameter("@IDTipoComposicao", SqlDbType.Int);

            paramIdEmpresa.Value = filtro.IDEmpresa ?? (object)DBNull.Value;
            paramIdPlacaOficial.Value = filtro.IdPlacaOficial ?? (object)DBNull.Value;
            paramPlaca.Value = string.IsNullOrEmpty(filtro.Placa) ? (object)DBNull.Value : filtro.Placa;
            paramOperacao.Value = string.IsNullOrEmpty(filtro.Operacao) ? (object)DBNull.Value : filtro.Operacao;
            paramDataInicio.Value = filtro.DataInicio ?? (object)DBNull.Value;
            paramDataFim.Value = filtro.DataFim ?? (object)DBNull.Value;
            paramIdTransportadora.Value = filtro.IDTransportadora ?? (object)DBNull.Value;
            paramIdcLiente.Value = filtro.IDCliente ?? (object)DBNull.Value;
            paramIdStatus.Value = filtro.IDStatus ?? (object)DBNull.Value;
            paramIdUsuarioCliente.Value = filtro.IDUsuarioCliente ?? (object)DBNull.Value;
            paramIdUsuarioTransportadora.Value = filtro.IDUsuarioTransportadora ?? (object)DBNull.Value;
            paramChamado.Value = string.IsNullOrEmpty(filtro.Chamado) ? (object)DBNull.Value : filtro.Chamado;
            paramIdTipoComposicao.Value = filtro.IDTipoComposicao ?? (object)DBNull.Value;

            List<PlacaView> dadosRelatorio = ExecutarProcedureComRetorno<PlacaView>(
                "[dbo].[Proc_Pesquisa_Placa] @IdEmpresa,@IdPlacaOficial,@Placa,@Operacao,@DataInicio,@DataFim,@IDTransportadora,@IDCLiente,@IDStatus,@IDUsuarioCliente,@IDUsuarioTransportadora,@Chamado,@IDTipoComposicao",
                new object[] { paramIdEmpresa, paramIdPlacaOficial, paramPlaca, paramOperacao, paramDataInicio, paramDataFim, paramIdTransportadora, paramIdcLiente, paramIdStatus, paramIdUsuarioCliente, paramIdUsuarioTransportadora, paramChamado, paramIdTipoComposicao });

            //teste com dapper
            //List<PlacaView> dadosRelatorio = ExecutarProcedureComRetornoD<PlacaView>(
            //    "[dbo].[Proc_Pesquisa_Placa]",
            //    new SqlParameter[] { paramIdEmpresa, paramIdPlacaOficial, paramPlaca, paramOperacao, paramDataInicio, paramDataFim, paramIdTransportadora, paramIdcLiente, paramIdStatus, paramIdUsuarioCliente, paramIdUsuarioTransportadora, paramChamado, paramIdTipoComposicao });
            return dadosRelatorio;
        }

        private DataTable GetQueryPlacaCompletaDataTable(ComposicaoFiltro filtro)
        {
            SqlParameter paramIdEmpresa = new SqlParameter("@IdEmpresa", SqlDbType.Int);
            SqlParameter paramIdPlacaOficial = new SqlParameter("@IdPlacaOficial", SqlDbType.Int);
            SqlParameter paramPlaca = new SqlParameter("@Placa", SqlDbType.VarChar);
            SqlParameter paramOperacao = new SqlParameter("@Operacao", SqlDbType.VarChar);
            SqlParameter paramDataInicio = new SqlParameter("@DataInicio", SqlDbType.DateTime);
            SqlParameter paramDataFim = new SqlParameter("@DataFim", SqlDbType.DateTime);
            SqlParameter paramIdTransportadora = new SqlParameter("@IDTransportadora", SqlDbType.Int);
            SqlParameter paramIdcLiente = new SqlParameter("@IDCLiente", SqlDbType.Int);
            SqlParameter paramIdStatus = new SqlParameter("@IDStatus", SqlDbType.Int);
            SqlParameter paramIdUsuarioCliente = new SqlParameter("@IDUsuarioCliente", SqlDbType.Int);
            SqlParameter paramIdUsuarioTransportadora = new SqlParameter("@IDUsuarioTransportadora", SqlDbType.Int);
            SqlParameter paramChamado = new SqlParameter("@Chamado", SqlDbType.VarChar);
            SqlParameter paramIdTipoComposicao = new SqlParameter("@IDTipoComposicao", SqlDbType.Int);
            SqlParameter paramIdPais = new SqlParameter("@IdPais", SqlDbType.Int);

            paramIdEmpresa.Value = filtro.IDEmpresa ?? (object)DBNull.Value;
            paramIdPlacaOficial.Value = filtro.IdPlacaOficial ?? (object)DBNull.Value;
            paramPlaca.Value = string.IsNullOrEmpty(filtro.Placa) ? (object)DBNull.Value : filtro.Placa;
            paramOperacao.Value = string.IsNullOrEmpty(filtro.Operacao) ? (object)DBNull.Value : filtro.Operacao;
            paramDataInicio.Value = filtro.DataInicio ?? (object)DBNull.Value;
            paramDataFim.Value = filtro.DataFim ?? (object)DBNull.Value;
            paramIdTransportadora.Value = filtro.IDTransportadora ?? (object)DBNull.Value;
            paramIdcLiente.Value = filtro.IDCliente ?? (object)DBNull.Value;
            paramIdStatus.Value = filtro.IDStatus ?? (object)DBNull.Value;
            paramIdUsuarioCliente.Value = filtro.IDUsuarioCliente ?? (object)DBNull.Value;
            paramIdUsuarioTransportadora.Value = filtro.IDUsuarioTransportadora ?? (object)DBNull.Value;
            paramChamado.Value = string.IsNullOrEmpty(filtro.Chamado) ? (object)DBNull.Value : filtro.Chamado;
            paramIdTipoComposicao.Value = filtro.IDTipoComposicao ?? (object)DBNull.Value;
            paramIdPais.Value = filtro.IdPais ?? (object)DBNull.Value;

            DataTable dadosPlaca = ExecutarProcedureComRetornoDataTable(
               "Proc_Pesquisa_Placa",
               new SqlParameter[] { paramIdEmpresa, paramIdPlacaOficial, paramPlaca, paramOperacao, paramDataInicio, paramDataFim, paramIdTransportadora, paramIdcLiente, paramIdStatus, paramIdUsuarioCliente, paramIdUsuarioTransportadora, paramChamado, paramIdTipoComposicao, paramIdPais });

            return dadosPlaca;
        }

        private IQueryable<Placa> GetQueryPlaca(PlacaFiltro filtro, IUniCadDalRepositorio<Placa> repositorio)
        {
            IQueryable<Placa> query = (from app in repositorio.ListComplex<Placa>().AsNoTracking().OrderBy(i => i.ID)
                                       where (app.PlacaVeiculo.Contains(string.IsNullOrEmpty(filtro.PlacaVeiculo) ? app.PlacaVeiculo : filtro.PlacaVeiculo))
                                       select app);
            return query;
        }

        public PlacaView SelecionarPlacaCompleta(ComposicaoFiltro filtro)
        {
            using (UniCadDalRepositorio<Placa> repositorio = new UniCadDalRepositorio<Placa>())
            {
                var resultado = GetQueryPlacaCompleta(filtro);
                return resultado.FirstOrDefault();
            }
        }

        public List<PlacaView> ListarPlacaRelatorio(ComposicaoFiltro filtro)
        {
            using (UniCadDalRepositorio<Placa> repositorio = new UniCadDalRepositorio<Placa>())
            {
                var resultado = GetQueryPlacaCompleta(filtro);
                return resultado.Where(p => p.IDStatus != 3).ToList();
            }
        }

        public DataTable ListarPlacaDataTable(ComposicaoFiltro filtro)
        {
            using (UniCadDalRepositorio<Placa> repositorio = new UniCadDalRepositorio<Placa>())
            {
                var resultado = GetQueryPlacaCompletaDataTable(filtro);
                return resultado;
            }
        }

        public bool AdicionarPlaca(Placa Placa)
        {
            using (TransactionScope transactionScope = Transactions.CreateTransactionScope(1, IsolationLevel.ReadCommitted))
            {
                using (UniCadDalRepositorio<Placa> PlacaRepositorio = new UniCadDalRepositorio<Placa>())
                {
                    Placa.DataAtualizacao = DateTime.Now;
                    if (!string.IsNullOrEmpty(Placa.Chassi))
                        Placa.Chassi = Placa.Chassi.ToUpper(CultureInfo.InvariantCulture);

                    Placa.PlacaVeiculo = Placa.PlacaVeiculo.ToUpper(CultureInfo.InvariantCulture);
                    PlacaRepositorio.Add(Placa);

                    var clienteBusiness = new PlacaClienteBusiness();

                    if (Placa.idUsuario > 0)
                    {
                        var placaAnteriorId = new PlacaBusiness().SelecionarPlacaComposicaoAprovada(Placa);
                        if (placaAnteriorId != null)
                        {
                            var placaAnteriorClientes = new PlacaClienteBusiness().ListarClientesPlaca(placaAnteriorId.ID);
                            if (placaAnteriorClientes != null && placaAnteriorClientes.Any())
                            {
                                var placaAnteriorClientesUsuario = new PlacaClienteBusiness().ListarClientesPorPlaca(placaAnteriorId.ID, Placa.idUsuario);
                                if (placaAnteriorClientesUsuario != null && placaAnteriorClientes.Any())
                                {
                                    List<PlacaClienteView> placas = new List<PlacaClienteView>();
                                    placas.AddRange(placaAnteriorClientes.Where(p2 =>
                                        placaAnteriorClientesUsuario.All(p1 => p1.IDCliente != p2.IDCliente)));
                                    Placa.Clientes.AddRange(placas.Where(p2 =>
                                        Placa.Clientes.All(p1 => p1.IDCliente != p2.IDCliente)));
                                }
                                else if (Placa.Clientes != null && Placa.Clientes.Any())
                                    Placa.Clientes.AddRange(placaAnteriorClientes);
                            }
                        }
                    }

                    if (Placa != null && Placa.Clientes != null)
                        foreach (var cliente in Placa.Clientes)
                        {
                            cliente.IDPlaca = Placa.ID;
                            clienteBusiness.Adicionar(new PlacaCliente { IDCliente = cliente.IDCliente, IDPlaca = Placa.ID, DataAprovacao = cliente.DataAprovacao });
                        }

                    var PlacaDocumentoBusiness = new PlacaDocumentoBusiness();

                    if (Placa?.Documentos != null)
                        foreach (var PlacaDocumento in Placa.Documentos)
                        {

                            PlacaDocumento.IDPlaca = Placa.ID;
                            PlacaDocumento.Vencido = PlacaDocumento.DataVencimento < DateTime.Now.Date ? true : false;

                            var placaDocumento = new PlacaDocumento
                            {
                                IDTipoDocumento = PlacaDocumento.IDTipoDocumento,
                                IDPlaca = Placa.ID,
                                Anexo = PlacaDocumento.Anexo,
                                Bloqueado = false,
                                DataVencimento = PlacaDocumento.DataVencimento,
                                Vencido = PlacaDocumento.Vencido
                            };

                            PlacaDocumentoBusiness.Adicionar(placaDocumento);
                            PlacaDocumento.ID = placaDocumento.ID;
                        }

                    var PlacaSetaBusiness = new PlacaSetaBusiness();

                    if (Placa.Setas != null && Placa.IDTipoVeiculo != (int)EnumTipoVeiculo.Cavalo && Placa.IDTipoVeiculo != (int)EnumTipoVeiculo.Dolly)
                        foreach (var item in Placa.Setas)
                        {
                            item.IDPlaca = Placa.ID;

                            if (item.ID == 0)
                            {
                                AdicionarPlacaSeta(Placa, PlacaSetaBusiness, item);
                            }
                        }

                    GravaTabelasEspecificas(Placa);

                    transactionScope.Complete();
                    return true;
                }
            }
        }

        public override Placa Selecionar(Expression<Func<Placa, bool>> where)
        {
            var placa = base.Selecionar(where);

            if (placa != null)
            {
                if (_pais == EnumPais.Brasil)
                    using (var placaDerivada = new UniCadDalRepositorio<PlacaBrasil>())
                        placa.PlacaBrasil = placaDerivada.Get(placa.ID);

                if (_pais == EnumPais.Argentina)
                    using (var placaDerivada = new UniCadDalRepositorio<PlacaArgentina>())
                        placa.PlacaArgentina = placaDerivada.Get(placa.ID);
            }

            return placa;
        }

        private void GravaTabelasEspecificas(Placa Placa)
        {
            switch (_pais)
            {
                case EnumPais.Brasil:
                    AdicionarPlacaBrasil(Placa.PlacaBrasil, Placa.ID);
                    break;
                case EnumPais.Argentina:
                    AdicionarPlacaArgentina(Placa.PlacaArgentina, Placa.ID);
                    break;
            }
        }

        private void AdicionarPlacaArgentina(PlacaArgentina placaArgentina, int idPlacaInserida)
        {
            using (UniCadDalRepositorio<PlacaArgentina> placaArgentinaRepositorio = new UniCadDalRepositorio<PlacaArgentina>())
            {
                placaArgentina.IDPlaca = idPlacaInserida;
                placaArgentina.CUIT = placaArgentina.CUIT.RemoveCharacter();
                placaArgentinaRepositorio.Add(placaArgentina);
            }
        }

        private void AdicionarPlacaBrasil(PlacaBrasil placaBrasil, int idPlacaInserida)
        {
            using (UniCadDalRepositorio<PlacaBrasil> placaBrasilRepositorio = new UniCadDalRepositorio<PlacaBrasil>())
            {
                placaBrasil.IDPlaca = idPlacaInserida;
                placaBrasil.CPFCNPJ = placaBrasil.CPFCNPJ.RemoveCharacter();
                placaBrasilRepositorio.Add(placaBrasil);
            }
        }

        public Placa SelecionarPlacaComposicaoAprovada(Placa placa)
        {
            using (UniCadDalRepositorio<Placa> repositorio = new UniCadDalRepositorio<Placa>())
            {
                IQueryable<Placa> query = GetQuerySelecionarPlacaComposicaoAprovada(placa, repositorio);
                return query.FirstOrDefault();
            }
        }

        public bool ValidarAcesso(string login, Usuario usuarioCsOnline, Placa placa)
        {
            var valido = false;

            if (placa == null || placa.ID == 0)
                return true;

            Usuario user = new Usuario();

            if (usuarioCsOnline != null)
                user = usuarioCsOnline;
            else
                user = new UsuarioBusiness().Selecionar(p => p.Login == login);

            if (user != null)
            {
                if (!user.Externo)
                    return true;

                var clientesPlaca = new PlacaClienteBusiness().Listar(p => p.IDPlaca == placa.ID);
                var clientesUsuario = new UsuarioClienteBusiness().Listar(p => p.IDUsuario == user.ID);
                var transportadoras = new UsuarioTransportadoraBusiness().Listar(p => p.IDUsuario == user.ID);

                valido = (clientesUsuario.Any(p => clientesPlaca.Any(b => b.IDCliente == p.IDCliente)));
                if (!valido)
                {
                    //CSCUNI-665
                    if (placa.LinhaNegocio == (int)EnumEmpresa.Ambos)
                    {
                        valido = (transportadoras.Any(p => p.IDTransportadora == placa.IDTransportadora) && transportadoras.Any(p => p.IDTransportadora == placa.IDTransportadora2));
                    }
                    else
                    {
                        valido = (transportadoras.Any(p => p.IDTransportadora == placa.IDTransportadora));
                    }
                }
            }

            return valido;
        }

        public int StatusPlaca(Placa placa)
        {
            var comp = new ComposicaoBusiness().Selecionar(p =>
               (p.IDPlaca1 == placa.ID ||
               p.IDPlaca2 == placa.ID ||
               p.IDPlaca3 == placa.ID ||
               p.IDPlaca4 == placa.ID));
            if (comp != null)
                return comp.IDStatus;
            return 0;
        }

        public bool PlacaAprovada(Placa placa)
        {
            return new ComposicaoBusiness().Existe(p =>
                (p.IDPlaca1 == placa.ID ||
                p.IDPlaca2 == placa.ID ||
                p.IDPlaca3 == placa.ID ||
                p.IDPlaca4 == placa.ID) && (p.IDStatus == (int)EnumStatusComposicao.Aprovado));
        }

        public Placa ListarPlacaEmAprovacao(string placaVeiculo, string operacao, int idEmpresa)
        {
            using (UniCadDalRepositorio<Placa> PlacaRepositorio = new UniCadDalRepositorio<Placa>())
            {
                var query = from placas in PlacaRepositorio.ListComplex<Placa>().AsNoTracking()
                            join comp in PlacaRepositorio.ListComplex<Composicao>().AsNoTracking() on placas.PlacaVeiculo equals placaVeiculo into c
                            from c2 in c.DefaultIfEmpty()
                            where (placas.ID != c2.IDPlaca1)
                                && (placas.ID != c2.IDPlaca2)
                                && (placas.ID != c2.IDPlaca3)
                                && (placas.ID != c2.IDPlaca4)
                                && (placas.PlacaVeiculo == placaVeiculo)
                                && (placas.Operacao == (string.IsNullOrEmpty(operacao) ? placas.Operacao : operacao))
                                && (placas.IDPais == _pais)
                            orderby placas.ID descending
                            select placas;
                return query.FirstOrDefault();
            }
        }

        public bool ListarPorStatus(int? idPlaca, string composicao, string placaVeiculo, EnumStatusComposicao status, string operacao)
        {
            return ListarPorStatus(idPlaca?.ToString(), composicao, placaVeiculo, status, operacao);
        }

        public bool ListarPorStatus(string idPlaca, string composicao, string placaVeiculo, EnumStatusComposicao status, string operacao)
        {
            int composicaoId = 0;

            string placaStr;
            Placa placa = new Placa();
            if (!string.IsNullOrEmpty(idPlaca) && idPlaca != "0")
            {
                placa = this.Selecionar(int.Parse(idPlaca));

                placaStr = placa.PlacaVeiculo;
            }
            else
                placaStr = placaVeiculo;

            if (!string.IsNullOrEmpty(composicao))
            {
                composicaoId = int.Parse(composicao);
            }
            else if (composicao == null)
            {
                if (placa != null && placa.ID > 0)
                {
                    var compo = new ComposicaoBusiness().Selecionar(p => (p.IDPlaca1 == placa.ID || p.IDPlaca2 == placa.ID || p.IDPlaca3 == placa.ID || p.IDPlaca4 == placa.ID) && p.IDStatus == (int)status);
                    if (compo != null)
                    {
                        composicaoId = compo.ID;
                    }
                }
            }

            SqlParameter paramStatus = new SqlParameter("@STATUS", SqlDbType.Int);
            SqlParameter paramIdComposicao = new SqlParameter("@IDComposicao", SqlDbType.Int);
            SqlParameter paramPlaca = new SqlParameter("@PLACA", SqlDbType.VarChar);
            SqlParameter paramOperacao = new SqlParameter("@OPERACAO", SqlDbType.VarChar);
            SqlParameter paramIdPais = new SqlParameter("@IDPAIS", SqlDbType.Int);

            paramStatus.Value = (int)status;
            paramIdComposicao.Value = composicaoId;
            paramPlaca.Value = String.IsNullOrEmpty(placaStr) ? (object)DBNull.Value : placaStr;
            paramOperacao.Value = String.IsNullOrEmpty(operacao) ? (object)DBNull.Value : operacao;
            paramIdPais.Value = (int)_pais;

            int cont = ExecutarProcedureComRetornoInteiro("[dbo].[Proc_Existe_Placa] @STATUS,@IDComposicao,@PLACA,@OPERACAO,@IDPAIS",
               new object[] { paramStatus, paramIdComposicao, paramPlaca, paramOperacao, paramIdPais });

            return cont > 0;
        }

        public void CalcularColunas(Placa placa)
        {
            int i = 1;
            if (placa != null && placa.Setas != null)
                foreach (var item in placa.Setas)
                {
                    item.Colunas = 0;
                    item.Sequencial = i;
                    item.Multiseta = placa.MultiSeta;
                    if (placa.Setas.Any(p => p.VolumeCompartimento1.HasValue)) item.Colunas = 1;
                    if (placa.Setas.Any(p => p.VolumeCompartimento2.HasValue)) item.Colunas = 2;
                    if (placa.Setas.Any(p => p.VolumeCompartimento3.HasValue)) item.Colunas = 3;
                    if (placa.Setas.Any(p => p.VolumeCompartimento4.HasValue)) item.Colunas = 4;
                    if (placa.Setas.Any(p => p.VolumeCompartimento5.HasValue)) item.Colunas = 5;
                    if (placa.Setas.Any(p => p.VolumeCompartimento6.HasValue)) item.Colunas = 6;
                    if (placa.Setas.Any(p => p.VolumeCompartimento7.HasValue)) item.Colunas = 7;
                    if (placa.Setas.Any(p => p.VolumeCompartimento8.HasValue)) item.Colunas = 8;
                    if (placa.Setas.Any(p => p.VolumeCompartimento9.HasValue)) item.Colunas = 9;
                    if (placa.Setas.Any(p => p.VolumeCompartimento10.HasValue)) item.Colunas = 10;

                    i++;
                }
        }

        public override Placa Selecionar(int id)
        {
            var placa = base.Selecionar(id);

            if (placa != null)
            {
                if (_pais == EnumPais.Brasil)
                    using (UniCadDalRepositorio<PlacaBrasil> placaDerivada = new UniCadDalRepositorio<PlacaBrasil>())
                        placa.PlacaBrasil = placaDerivada.Get(id);

                if (_pais == EnumPais.Argentina)
                    using (UniCadDalRepositorio<PlacaArgentina> placaDerivada = new UniCadDalRepositorio<PlacaArgentina>())
                        placa.PlacaArgentina = placaDerivada.Get(id);
            }

            return placa;
        }

        public PlacaAlteradaView ListarAlteracoes(int idPlacaSolicitacao, int idPlacaOficial)
        {
            PlacaAlteradaView placaAlteradaView = new PlacaAlteradaView();
            var placaSolicitacao = Selecionar(idPlacaSolicitacao);
            var placaOficial = Selecionar(idPlacaOficial);
            if (placaOficial != null)
            {
                placaAlteradaView.IsVolumeAlterado = placaSolicitacao.Volume != placaOficial.Volume;
                placaAlteradaView.IsAnoFabricacaoAlterado = placaSolicitacao.AnoFabricacao != placaOficial.AnoFabricacao;
                placaAlteradaView.IsAnoModeloAlterado = placaSolicitacao.AnoModelo != placaOficial.AnoModelo;
                placaAlteradaView.IsBombaDescargaAlterado = placaSolicitacao.BombaDescarga != placaOficial.BombaDescarga;
                placaAlteradaView.IsCameraMonitoramentoAlterado = placaSolicitacao.CameraMonitoramento != placaOficial.CameraMonitoramento;
                placaAlteradaView.IsChassiAlterado = placaSolicitacao.Chassi != placaOficial.Chassi;
                placaAlteradaView.IsPrincipalAlterado = ((placaSolicitacao.Principal.HasValue && placaOficial.Principal.HasValue && placaSolicitacao.Principal.Value != placaOficial.Principal.Value)
                   || (placaSolicitacao.Principal.HasValue && !placaOficial.Principal.HasValue)
                   || (!placaSolicitacao.Principal.HasValue && placaOficial.Principal.HasValue));
                placaAlteradaView.IsCorAlterado = placaSolicitacao.Cor != placaOficial.Cor;
                placaAlteradaView.IsDataNascimentoAlterado = placaSolicitacao.DataNascimento != placaOficial.DataNascimento;
                placaAlteradaView.IsEixosDistanciadosAlterado = placaSolicitacao.EixosDistanciados != placaOficial.EixosDistanciados;
                placaAlteradaView.IsEixosPneusDuplosAlterado = placaSolicitacao.EixosPneusDuplos != placaOficial.EixosPneusDuplos;
                placaAlteradaView.IsIDCategoriaVeiculoAlterado = placaSolicitacao.IDCategoriaVeiculo != placaOficial.IDCategoriaVeiculo;
                placaAlteradaView.IsIDTipoCarregamentoAlterado = ((placaSolicitacao.IDTipoCarregamento.HasValue && placaOficial.IDTipoCarregamento.HasValue && placaSolicitacao.IDTipoCarregamento != placaOficial.IDTipoCarregamento)
                    || (placaSolicitacao.IDTipoCarregamento.HasValue && !placaOficial.IDTipoCarregamento.HasValue)
                    || (!placaSolicitacao.IDTipoCarregamento.HasValue && placaOficial.IDTipoCarregamento.HasValue));
                placaAlteradaView.IsIDTipoProdutoAlterado = ((placaSolicitacao.IDTipoProduto.HasValue && placaOficial.IDTipoProduto.HasValue && placaSolicitacao.IDTipoProduto != placaOficial.IDTipoProduto)
                    || (placaSolicitacao.IDTipoProduto.HasValue && !placaOficial.IDTipoProduto.HasValue)
                    || (!placaSolicitacao.IDTipoProduto.HasValue && placaOficial.IDTipoProduto.HasValue));
                placaAlteradaView.IsIDTipoVeiculoAlterado = placaSolicitacao.IDTipoVeiculo != placaOficial.IDTipoVeiculo;
                placaAlteradaView.IsIDTransportadoraAlterado = (placaSolicitacao.IDTransportadora.HasValue && placaOficial.IDTransportadora.HasValue && placaSolicitacao.IDTransportadora != placaOficial.IDTransportadora)
                    || (placaSolicitacao.IDTransportadora.HasValue && !placaOficial.IDTransportadora.HasValue)
                    || (!placaSolicitacao.IDTransportadora.HasValue && placaOficial.IDTransportadora.HasValue);
                placaAlteradaView.IsIDTransportadora2Alterado = (placaSolicitacao.IDTransportadora2.HasValue && placaOficial.IDTransportadora2.HasValue && placaSolicitacao.IDTransportadora2 != placaOficial.IDTransportadora2)
                                                               || (placaSolicitacao.IDTransportadora2.HasValue && !placaOficial.IDTransportadora2.HasValue)
                                                               || (!placaSolicitacao.IDTransportadora2.HasValue && placaOficial.IDTransportadora2.HasValue);
                placaAlteradaView.IsMarcaAlterado = placaSolicitacao.Marca != placaOficial.Marca;
                placaAlteradaView.IsModeloAlterado = placaSolicitacao.Modelo != placaOficial.Modelo;

                placaAlteradaView.IsChassiAlterado = placaSolicitacao.Chassi != placaOficial.Chassi;
                placaAlteradaView.IsMultiSetaAlterado = placaSolicitacao.MultiSeta != placaOficial.MultiSeta;
                placaAlteradaView.IsTipoRastreadorAlterado = placaSolicitacao.TipoRastreador != placaOficial.TipoRastreador;
                placaAlteradaView.IsNumeroAntenaAlterado = placaSolicitacao.NumeroAntena != placaOficial.NumeroAntena;
                placaAlteradaView.IsNumeroEixosAlterado = placaSolicitacao.NumeroEixos != placaOficial.NumeroEixos;
                placaAlteradaView.IsNumeroEixosDistanciadosAlterado = ((placaSolicitacao.NumeroEixosDistanciados.HasValue && placaOficial.NumeroEixosDistanciados.HasValue && placaSolicitacao.NumeroEixosDistanciados != placaOficial.NumeroEixosDistanciados)
                    || (placaSolicitacao.NumeroEixosDistanciados.HasValue && !placaOficial.NumeroEixosDistanciados.HasValue)
                    || (!placaSolicitacao.NumeroEixosDistanciados.HasValue && placaOficial.NumeroEixosDistanciados.HasValue));
                placaAlteradaView.IsNumeroEixosPneusDuplosAlterado = ((placaSolicitacao.NumeroEixosPneusDuplos.HasValue && placaOficial.NumeroEixosPneusDuplos.HasValue && placaSolicitacao.NumeroEixosPneusDuplos != placaOficial.NumeroEixosPneusDuplos)
                    || (placaSolicitacao.NumeroEixosPneusDuplos.HasValue && !placaOficial.NumeroEixosPneusDuplos.HasValue)
                    || (!placaSolicitacao.NumeroEixosPneusDuplos.HasValue && placaOficial.NumeroEixosPneusDuplos.HasValue));
                placaAlteradaView.IsObservacaoAlterado = placaSolicitacao.Observacao != placaOficial.Observacao;
                placaAlteradaView.IsOperacaoAlterado = placaSolicitacao.Operacao != placaOficial.Operacao;
                placaAlteradaView.IsPlacaVeiculoAlterado = placaSolicitacao.PlacaVeiculo != placaOficial.PlacaVeiculo;
                placaAlteradaView.IsPossuiAbsAlterado = placaSolicitacao.PossuiAbs != placaOficial.PossuiAbs;
                placaAlteradaView.IsRazaoSocialAlterado = placaSolicitacao.RazaoSocial != placaOficial.RazaoSocial;
                placaAlteradaView.IsTaraAlterado = Convert.ToDecimal(placaSolicitacao.Tara) != Convert.ToDecimal(placaOficial.Tara);
                placaAlteradaView.IsVersaoAlterado = placaSolicitacao.Versao != placaOficial.Versao;

                if (_pais == EnumPais.Brasil)
                {
                    placaAlteradaView.IsRenavamAlterado = placaSolicitacao.PlacaBrasil.Renavam != placaOficial.PlacaBrasil.Renavam;
                    placaAlteradaView.IsUfAlterado = placaSolicitacao.PlacaBrasil.IDEstado != placaOficial.PlacaBrasil.IDEstado;
                    placaAlteradaView.IsCPFCNPJAlterado = placaSolicitacao.PlacaBrasil.CPFCNPJ != placaOficial.PlacaBrasil.CPFCNPJ;
                    placaAlteradaView.IsIDEstadoAlterado = placaSolicitacao.PlacaBrasil.IDEstado != placaOficial.PlacaBrasil.IDEstado;
                    placaAlteradaView.IsCidadeAlterado = placaSolicitacao.PlacaBrasil.Cidade != placaOficial.PlacaBrasil.Cidade;
                }

                if (_pais == EnumPais.Argentina)
                {
                    placaAlteradaView.IsCuitAlterado = placaSolicitacao.PlacaArgentina.CUIT != placaOficial.PlacaArgentina.CUIT;
                    placaAlteradaView.IsVencimentoAlterado = placaSolicitacao.PlacaArgentina.Vencimento != placaOficial.PlacaArgentina.Vencimento;
                    placaAlteradaView.IsMaterialAlterado = placaSolicitacao.PlacaArgentina.Material != placaOficial.PlacaArgentina.Material;
                    placaAlteradaView.IsPotenciaAlterado = placaSolicitacao.PlacaArgentina.Potencia != placaOficial.PlacaArgentina.Potencia;
                    placaAlteradaView.IsNrMotorAlterado = placaSolicitacao.PlacaArgentina.NrMotor != placaOficial.PlacaArgentina.NrMotor;
                    placaAlteradaView.IsSatelitalMarcaAlterado = placaSolicitacao.PlacaArgentina.SatelitalMarca != placaOficial.PlacaArgentina.SatelitalMarca;
                    placaAlteradaView.IsSatelitalModeloAlterado = placaSolicitacao.PlacaArgentina.SatelitalModelo != placaOficial.PlacaArgentina.SatelitalModelo;
                    placaAlteradaView.IsSatelitalNrInternoAlterado = placaSolicitacao.PlacaArgentina.SatelitalNrInterno != placaOficial.PlacaArgentina.SatelitalNrInterno;
                    placaAlteradaView.IsSatelitalEmpresaAlterado = placaSolicitacao.PlacaArgentina.SatelitalEmpresa != placaOficial.PlacaArgentina.SatelitalEmpresa;
                }

                if (placaSolicitacao != null && placaOficial != null && placaSolicitacao.SetaPrincipal != null && placaOficial.SetaPrincipal != null)
                {
                    placaAlteradaView.IsVolumeCompartimento1Alterado = ((placaSolicitacao.SetaPrincipal.VolumeCompartimento1.HasValue && placaOficial.SetaPrincipal.VolumeCompartimento1.HasValue && placaSolicitacao.SetaPrincipal.VolumeCompartimento1 != placaOficial.SetaPrincipal.VolumeCompartimento1)
                    || (placaSolicitacao.SetaPrincipal.VolumeCompartimento1.HasValue && !placaOficial.SetaPrincipal.VolumeCompartimento1.HasValue)
                    || (!placaSolicitacao.SetaPrincipal.VolumeCompartimento1.HasValue && placaOficial.SetaPrincipal.VolumeCompartimento1.HasValue));
                    placaAlteradaView.IsVolumeCompartimento2Alterado = ((placaSolicitacao.SetaPrincipal.VolumeCompartimento2.HasValue && placaOficial.SetaPrincipal.VolumeCompartimento2.HasValue && placaSolicitacao.SetaPrincipal.VolumeCompartimento2 != placaOficial.SetaPrincipal.VolumeCompartimento2)
                     || (placaSolicitacao.SetaPrincipal.VolumeCompartimento2.HasValue && !placaOficial.SetaPrincipal.VolumeCompartimento2.HasValue)
                     || (!placaSolicitacao.SetaPrincipal.VolumeCompartimento2.HasValue && placaOficial.SetaPrincipal.VolumeCompartimento2.HasValue));
                    placaAlteradaView.IsVolumeCompartimento3Alterado = ((placaSolicitacao.SetaPrincipal.VolumeCompartimento3.HasValue && placaOficial.SetaPrincipal.VolumeCompartimento3.HasValue && placaSolicitacao.SetaPrincipal.VolumeCompartimento3 != placaOficial.SetaPrincipal.VolumeCompartimento3)
                    || (placaSolicitacao.SetaPrincipal.VolumeCompartimento3.HasValue && !placaOficial.SetaPrincipal.VolumeCompartimento3.HasValue)
                    || (!placaSolicitacao.SetaPrincipal.VolumeCompartimento3.HasValue && placaOficial.SetaPrincipal.VolumeCompartimento3.HasValue));
                    placaAlteradaView.IsVolumeCompartimento4Alterado = ((placaSolicitacao.SetaPrincipal.VolumeCompartimento4.HasValue && placaOficial.SetaPrincipal.VolumeCompartimento4.HasValue && placaSolicitacao.SetaPrincipal.VolumeCompartimento4 != placaOficial.SetaPrincipal.VolumeCompartimento4)
                    || (placaSolicitacao.SetaPrincipal.VolumeCompartimento4.HasValue && !placaOficial.SetaPrincipal.VolumeCompartimento4.HasValue)
                    || (!placaSolicitacao.SetaPrincipal.VolumeCompartimento4.HasValue && placaOficial.SetaPrincipal.VolumeCompartimento4.HasValue));
                    placaAlteradaView.IsVolumeCompartimento5Alterado = ((placaSolicitacao.SetaPrincipal.VolumeCompartimento5.HasValue && placaOficial.SetaPrincipal.VolumeCompartimento5.HasValue && placaSolicitacao.SetaPrincipal.VolumeCompartimento5 != placaOficial.SetaPrincipal.VolumeCompartimento5)
                    || (placaSolicitacao.SetaPrincipal.VolumeCompartimento5.HasValue && !placaOficial.SetaPrincipal.VolumeCompartimento5.HasValue)
                    || (!placaSolicitacao.SetaPrincipal.VolumeCompartimento5.HasValue && placaOficial.SetaPrincipal.VolumeCompartimento5.HasValue));
                    placaAlteradaView.IsVolumeCompartimento6Alterado = ((placaSolicitacao.SetaPrincipal.VolumeCompartimento6.HasValue && placaOficial.SetaPrincipal.VolumeCompartimento6.HasValue && placaSolicitacao.SetaPrincipal.VolumeCompartimento6 != placaOficial.SetaPrincipal.VolumeCompartimento6)
                    || (placaSolicitacao.SetaPrincipal.VolumeCompartimento6.HasValue && !placaOficial.SetaPrincipal.VolumeCompartimento6.HasValue)
                    || (!placaSolicitacao.SetaPrincipal.VolumeCompartimento6.HasValue && placaOficial.SetaPrincipal.VolumeCompartimento6.HasValue));
                    placaAlteradaView.IsVolumeCompartimento7Alterado = ((placaSolicitacao.SetaPrincipal.VolumeCompartimento7.HasValue && placaOficial.SetaPrincipal.VolumeCompartimento7.HasValue && placaSolicitacao.SetaPrincipal.VolumeCompartimento7 != placaOficial.SetaPrincipal.VolumeCompartimento7)
                    || (placaSolicitacao.SetaPrincipal.VolumeCompartimento7.HasValue && !placaOficial.SetaPrincipal.VolumeCompartimento7.HasValue)
                    || (!placaSolicitacao.SetaPrincipal.VolumeCompartimento7.HasValue && placaOficial.SetaPrincipal.VolumeCompartimento7.HasValue));
                    placaAlteradaView.IsVolumeCompartimento8Alterado = ((placaSolicitacao.SetaPrincipal.VolumeCompartimento8.HasValue && placaOficial.SetaPrincipal.VolumeCompartimento8.HasValue && placaSolicitacao.SetaPrincipal.VolumeCompartimento8 != placaOficial.SetaPrincipal.VolumeCompartimento8)
                    || (placaSolicitacao.SetaPrincipal.VolumeCompartimento8.HasValue && !placaOficial.SetaPrincipal.VolumeCompartimento8.HasValue)
                    || (!placaSolicitacao.SetaPrincipal.VolumeCompartimento8.HasValue && placaOficial.SetaPrincipal.VolumeCompartimento8.HasValue));
                    placaAlteradaView.IsVolumeCompartimento9Alterado = ((placaSolicitacao.SetaPrincipal.VolumeCompartimento9.HasValue && placaOficial.SetaPrincipal.VolumeCompartimento9.HasValue && placaSolicitacao.SetaPrincipal.VolumeCompartimento9 != placaOficial.SetaPrincipal.VolumeCompartimento9)
                    || (placaSolicitacao.SetaPrincipal.VolumeCompartimento9.HasValue && !placaOficial.SetaPrincipal.VolumeCompartimento9.HasValue)
                    || (!placaSolicitacao.SetaPrincipal.VolumeCompartimento9.HasValue && placaOficial.SetaPrincipal.VolumeCompartimento9.HasValue));
                    placaAlteradaView.IsVolumeCompartimento10Alterado = ((placaSolicitacao.SetaPrincipal.VolumeCompartimento10.HasValue && placaOficial.SetaPrincipal.VolumeCompartimento10.HasValue && placaSolicitacao.SetaPrincipal.VolumeCompartimento10 != placaOficial.SetaPrincipal.VolumeCompartimento10)
                    || (placaSolicitacao.SetaPrincipal.VolumeCompartimento10.HasValue && !placaOficial.SetaPrincipal.VolumeCompartimento10.HasValue)
                    || (!placaSolicitacao.SetaPrincipal.VolumeCompartimento10.HasValue && placaOficial.SetaPrincipal.VolumeCompartimento10.HasValue));
                }
                if (placaSolicitacao.Setas != null)
                {
                    if (placaSolicitacao.Setas.Count > 0 && placaOficial.Setas.Count > 0)
                        placaAlteradaView.IsVolumeSeta1Alterado = placaSolicitacao.Setas[0].VolumeTotalCompartimento != placaOficial.Setas[0].VolumeTotalCompartimento;
                    else if ((placaSolicitacao.Setas.Count == 0 && placaOficial.Setas.Count > 0) || (placaSolicitacao.Setas.Count > 0 && placaOficial.Setas.Count == 0))
                        placaAlteradaView.IsVolumeSeta1Alterado = true;

                    if (placaSolicitacao.Setas.Count > 1 && placaOficial.Setas.Count > 1)
                        placaAlteradaView.IsVolumeSeta2Alterado = placaSolicitacao.Setas[1].VolumeTotalCompartimento != placaOficial.Setas[1].VolumeTotalCompartimento;
                    else if ((placaSolicitacao.Setas.Count == 1 && placaOficial.Setas.Count > 1) || (placaSolicitacao.Setas.Count > 1 && placaOficial.Setas.Count == 1))
                        placaAlteradaView.IsVolumeSeta2Alterado = true;

                    if (placaSolicitacao.Setas.Count > 2 && placaOficial.Setas.Count > 2)
                        placaAlteradaView.IsVolumeSeta3Alterado = placaSolicitacao.Setas[2].VolumeTotalCompartimento != placaOficial.Setas[2].VolumeTotalCompartimento;
                    else if ((placaSolicitacao.Setas.Count == 2 && placaOficial.Setas.Count > 2) || (placaSolicitacao.Setas.Count > 2 && placaOficial.Setas.Count == 2))
                        placaAlteradaView.IsVolumeSeta1Alterado = true;

                    if (placaSolicitacao.Setas.Count > 3 && placaOficial.Setas.Count > 3)
                        placaAlteradaView.IsVolumeSeta3Alterado = placaSolicitacao.Setas[3].VolumeTotalCompartimento != placaOficial.Setas[2].VolumeTotalCompartimento;
                    else if ((placaSolicitacao.Setas.Count == 3 && placaOficial.Setas.Count > 3) || (placaSolicitacao.Setas.Count > 3 && placaOficial.Setas.Count == 3))
                        placaAlteradaView.IsVolumeSeta1Alterado = true;

                    if (placaSolicitacao.Setas.Count > 4 && placaOficial.Setas.Count > 4)
                        placaAlteradaView.IsVolumeSeta4Alterado = placaSolicitacao.Setas[4].VolumeTotalCompartimento != placaOficial.Setas[4].VolumeTotalCompartimento;
                    else if ((placaSolicitacao.Setas.Count == 4 && placaOficial.Setas.Count > 4) || (placaSolicitacao.Setas.Count > 4 && placaOficial.Setas.Count == 4))
                        placaAlteradaView.IsVolumeSeta1Alterado = true;
                }
            }
            return placaAlteradaView;
        }

        public List<Placa> ListarPorComposicaoCapacidade(Composicao comp)
        {
            double tara = CalcularTara(comp);

            List<Placa> placas = CarregarPlacaCapacidade(comp, tara, comp.PBTC);
            return placas;
        }

        private static double CalcularTara(Composicao comp)
        {
            Double tara = 0;
            tara += new PlacaBusiness().Selecionar(comp.IDPlaca1.Value).Tara;
            if (comp.IDPlaca2.HasValue)
                tara += new PlacaBusiness().Selecionar(comp.IDPlaca2.Value).Tara;
            if (comp.IDPlaca3.HasValue)
                tara += new PlacaBusiness().Selecionar(comp.IDPlaca3.Value).Tara;
            if (comp.IDPlaca4.HasValue)
                tara += new PlacaBusiness().Selecionar(comp.IDPlaca4.Value).Tara;
            return tara;
        }

        public PlacaView ObterPlacaAprovada(string placaVeiculo)
        {
            using (UniCadDalRepositorio<Placa> PlacaRepositorio = new UniCadDalRepositorio<Placa>())
            {
                var existe = from placas in PlacaRepositorio.ListComplex<Placa>().AsNoTracking()
                             join comp in PlacaRepositorio.ListComplex<Composicao>() on (int)EnumStatusComposicao.Aprovado equals comp.IDStatus
                             where ((placas.ID == comp.IDPlaca1) || (placas.ID == comp.IDPlaca2) || (placas.ID == comp.IDPlaca3) || (placas.ID == comp.IDPlaca4))
                             && placas.PlacaVeiculo == placaVeiculo
                             select new PlacaView { ID = placas.ID };

                return existe.FirstOrDefault();
            }
        }

        public PlacaClientesAlteradosView isPlacaClientesAlterados(int iDPlacaSolicitacao, int idPlacaOficial, bool isValidarDocs = false, bool isValidarCompartimentos = false)
        {
			bool outrosDadosAlterados = false;
            bool clientesAlterados = false;

			var placaOficialAprovada = SelecionarPlacaCompleta(new ComposicaoFiltro { IdPlacaOficial = idPlacaOficial });
			if (placaOficialAprovada != null)
			{
				var placaSolicitacao = Selecionar(iDPlacaSolicitacao);
				placaSolicitacao.Setas = new PlacaSetaBusiness().Listar(w => w.IDPlaca == placaSolicitacao.ID);
				var placaOficial = Selecionar(idPlacaOficial);
				placaOficial.Setas = new PlacaSetaBusiness().Listar(w => w.IDPlaca == placaOficial.ID);
				if (placaOficial != null)
				{
					if (placaSolicitacao.AnoFabricacao != placaOficial.AnoFabricacao
					|| placaSolicitacao.AnoModelo != placaOficial.AnoModelo
					|| placaSolicitacao.BombaDescarga != placaOficial.BombaDescarga
					|| placaSolicitacao.CameraMonitoramento != placaOficial.CameraMonitoramento
					|| placaSolicitacao.Chassi != placaOficial.Chassi
					|| ((placaSolicitacao.Principal.HasValue && placaOficial.Principal.HasValue && placaSolicitacao.Principal.Value != placaOficial.Principal.Value)
					|| (placaSolicitacao.Principal.HasValue && !placaOficial.Principal.HasValue)
					|| (!placaSolicitacao.Principal.HasValue && placaOficial.Principal.HasValue))
					|| placaSolicitacao.Cor != placaOficial.Cor
					|| placaSolicitacao.DataNascimento != placaOficial.DataNascimento
					|| placaSolicitacao.EixosDistanciados != placaOficial.EixosDistanciados
					|| placaSolicitacao.EixosPneusDuplos != placaOficial.EixosPneusDuplos
					|| placaSolicitacao.IDCategoriaVeiculo != placaOficial.IDCategoriaVeiculo
					|| ((placaSolicitacao.IDTipoCarregamento.HasValue && placaOficial.IDTipoCarregamento.HasValue && placaSolicitacao.IDTipoCarregamento != placaOficial.IDTipoCarregamento)
					|| (placaSolicitacao.IDTipoCarregamento.HasValue && !placaOficial.IDTipoCarregamento.HasValue)
					|| (!placaSolicitacao.IDTipoCarregamento.HasValue && placaOficial.IDTipoCarregamento.HasValue))
					|| ((placaSolicitacao.IDTipoProduto.HasValue && placaOficial.IDTipoProduto.HasValue && placaSolicitacao.IDTipoProduto != placaOficial.IDTipoProduto)
					|| (placaSolicitacao.IDTipoProduto.HasValue && !placaOficial.IDTipoProduto.HasValue)
					|| (!placaSolicitacao.IDTipoProduto.HasValue && placaOficial.IDTipoProduto.HasValue))
					|| placaSolicitacao.IDTipoVeiculo != placaOficial.IDTipoVeiculo
					|| ((placaSolicitacao.IDTransportadora.HasValue && placaOficial.IDTransportadora.HasValue && placaSolicitacao.IDTransportadora != placaOficial.IDTransportadora)
					|| (placaSolicitacao.IDTransportadora.HasValue && !placaOficial.IDTransportadora.HasValue)
					|| (!placaSolicitacao.IDTransportadora.HasValue && placaOficial.IDTransportadora.HasValue))
					|| placaSolicitacao.Marca != placaOficial.Marca
					|| placaSolicitacao.Modelo != placaOficial.Modelo
					|| placaSolicitacao.MultiSeta != placaOficial.MultiSeta
					|| placaSolicitacao.NumeroAntena != placaOficial.NumeroAntena
					|| placaSolicitacao.NumeroEixos != placaOficial.NumeroEixos
					|| ((placaSolicitacao.NumeroEixosDistanciados.HasValue && placaOficial.NumeroEixosDistanciados.HasValue && placaSolicitacao.NumeroEixosDistanciados != placaOficial.NumeroEixosDistanciados)
					|| (placaSolicitacao.NumeroEixosDistanciados.HasValue && !placaOficial.NumeroEixosDistanciados.HasValue)
					|| (!placaSolicitacao.NumeroEixosDistanciados.HasValue && placaOficial.NumeroEixosDistanciados.HasValue))
					|| ((placaSolicitacao.NumeroEixosPneusDuplos.HasValue && placaOficial.NumeroEixosPneusDuplos.HasValue && placaSolicitacao.NumeroEixosPneusDuplos != placaOficial.NumeroEixosPneusDuplos)
					|| (placaSolicitacao.NumeroEixosPneusDuplos.HasValue && !placaOficial.NumeroEixosPneusDuplos.HasValue)
					|| (!placaSolicitacao.NumeroEixosPneusDuplos.HasValue && placaOficial.NumeroEixosPneusDuplos.HasValue))
					|| placaSolicitacao.Observacao != placaOficial.Observacao
					|| placaSolicitacao.Operacao != placaOficial.Operacao
					|| placaSolicitacao.PlacaVeiculo != placaOficial.PlacaVeiculo
					|| placaSolicitacao.PossuiAbs != placaOficial.PossuiAbs
					|| placaSolicitacao.RazaoSocial != placaOficial.RazaoSocial
					|| ((placaSolicitacao.Setas != null && placaOficial.Setas != null && placaSolicitacao.Setas.Count > placaOficial.Setas.Count)
					|| (placaSolicitacao.Setas == null && placaOficial.Setas != null)
					|| (placaSolicitacao.Setas != null && placaOficial.Setas == null))
					|| Convert.ToDecimal(placaSolicitacao.Tara) != Convert.ToDecimal(placaOficial.Tara)
					|| placaSolicitacao.Versao != placaOficial.Versao)
						outrosDadosAlterados = true;

					//Uma vez verificado os campos comuns da tabela Placa (acima), 
					//verifica agora os campos das tabelas específicas de cada país.
					if (!outrosDadosAlterados && VerificaAlteracoesPaises(placaSolicitacao, placaOficial))
						outrosDadosAlterados = true;

					//se retorno for true nem precisa entrar 
					if (!outrosDadosAlterados && isValidarDocs)
					{
						placaSolicitacao.Documentos =
							new PlacaDocumentoBusiness().ListarPlacaDocumentoPorPlaca(placaSolicitacao.ID);
						placaOficial.Documentos =
							new PlacaDocumentoBusiness().ListarPlacaDocumentoPorPlaca(placaOficial.ID);
						if (placaSolicitacao.Documentos != null && placaSolicitacao.Documentos.Any() &&
							placaOficial.Documentos != null && placaOficial.Documentos.Any())
						{
							var docsAlterados =
								new PlacaDocumentoBusiness().ListarAlteracoes(placaSolicitacao.Documentos,
									placaOficial.Documentos);
							outrosDadosAlterados = docsAlterados.Any(w => w.isAnexoAlterado || w.isDataVencimentoAlterada) || !(placaSolicitacao.Anexo == placaOficial.Anexo);
						}
						else if (placaSolicitacao.Documentos == null && placaOficial.Documentos != null)
							outrosDadosAlterados = true;
					}

					if (placaSolicitacao.Operacao == "FOB")
					{
						placaSolicitacao.Clientes =
							new PlacaClienteBusiness().ListarClientesPlaca(placaSolicitacao.ID);
						placaOficial.Clientes =
							new PlacaClienteBusiness().ListarClientesPlaca(placaOficial.ID);
						var distintos = placaSolicitacao.Clientes.Where(a => !placaOficial.Clientes.Any(b => a.IDCliente == b.IDCliente));
						if (distintos != null && distintos.Any())
							clientesAlterados = true;
					}

					//se retorno for true nem precisa entrar 
					if (!outrosDadosAlterados && isValidarCompartimentos)
					{
						placaSolicitacao.Setas =
							new PlacaSetaBusiness().Listar(p => p.IDPlaca == placaSolicitacao.ID);
						placaOficial.Setas =
							new PlacaSetaBusiness().Listar(p => p.IDPlaca == placaOficial.ID);


						if (placaSolicitacao.Setas != null && placaSolicitacao.Setas.Any() &&
							placaOficial.Setas != null && placaOficial.Setas.Any())
						{
							var setasAlteradas =
								new PlacaSetaBusiness().ListarAlteracoes(placaSolicitacao.Setas,
									placaOficial.Setas);
							outrosDadosAlterados = setasAlteradas.Any(w => w.isPrincipalAlterado || w.isVolumeAlterado);
						}
					}
				}
			}
			return new PlacaClientesAlteradosView
            {
				IsClientesAlterados = clientesAlterados,
				IsOutrosDadosAlterados = outrosDadosAlterados,
			};
		}

		public bool isPlacaAlterada(int iDPlacaSolicitacao, int idPlacaOficial, bool isValidarDocs = false, bool isValidarCompartimentos = false)
        {
            bool retorno = false;
            var placaOficialAprovada = SelecionarPlacaCompleta(new ComposicaoFiltro { IdPlacaOficial = idPlacaOficial });
            if (placaOficialAprovada != null)
            {
                var placaSolicitacao = Selecionar(iDPlacaSolicitacao);
                placaSolicitacao.Setas = new PlacaSetaBusiness().Listar(w => w.IDPlaca == placaSolicitacao.ID);
                var placaOficial = Selecionar(idPlacaOficial);
                placaOficial.Setas = new PlacaSetaBusiness().Listar(w => w.IDPlaca == placaOficial.ID);
                if (placaOficial != null)
                {
                    if (placaSolicitacao.AnoFabricacao != placaOficial.AnoFabricacao
                    || placaSolicitacao.AnoModelo != placaOficial.AnoModelo
                    || placaSolicitacao.BombaDescarga != placaOficial.BombaDescarga
                    || placaSolicitacao.CameraMonitoramento != placaOficial.CameraMonitoramento
                    || placaSolicitacao.Chassi != placaOficial.Chassi
                    || ((placaSolicitacao.Principal.HasValue && placaOficial.Principal.HasValue && placaSolicitacao.Principal.Value != placaOficial.Principal.Value)
                    || (placaSolicitacao.Principal.HasValue && !placaOficial.Principal.HasValue)
                    || (!placaSolicitacao.Principal.HasValue && placaOficial.Principal.HasValue))
                    || placaSolicitacao.Cor != placaOficial.Cor
                    || placaSolicitacao.DataNascimento != placaOficial.DataNascimento
                    || placaSolicitacao.EixosDistanciados != placaOficial.EixosDistanciados
                    || placaSolicitacao.EixosPneusDuplos != placaOficial.EixosPneusDuplos
                    || placaSolicitacao.IDCategoriaVeiculo != placaOficial.IDCategoriaVeiculo
                    || ((placaSolicitacao.IDTipoCarregamento.HasValue && placaOficial.IDTipoCarregamento.HasValue && placaSolicitacao.IDTipoCarregamento != placaOficial.IDTipoCarregamento)
                    || (placaSolicitacao.IDTipoCarregamento.HasValue && !placaOficial.IDTipoCarregamento.HasValue)
                    || (!placaSolicitacao.IDTipoCarregamento.HasValue && placaOficial.IDTipoCarregamento.HasValue))
                    || ((placaSolicitacao.IDTipoProduto.HasValue && placaOficial.IDTipoProduto.HasValue && placaSolicitacao.IDTipoProduto != placaOficial.IDTipoProduto)
                    || (placaSolicitacao.IDTipoProduto.HasValue && !placaOficial.IDTipoProduto.HasValue)
                    || (!placaSolicitacao.IDTipoProduto.HasValue && placaOficial.IDTipoProduto.HasValue))
                    || placaSolicitacao.IDTipoVeiculo != placaOficial.IDTipoVeiculo
                    || ((placaSolicitacao.IDTransportadora.HasValue && placaOficial.IDTransportadora.HasValue && placaSolicitacao.IDTransportadora != placaOficial.IDTransportadora)
                    || (placaSolicitacao.IDTransportadora.HasValue && !placaOficial.IDTransportadora.HasValue)
                    || (!placaSolicitacao.IDTransportadora.HasValue && placaOficial.IDTransportadora.HasValue))
                    || placaSolicitacao.Marca != placaOficial.Marca
                    || placaSolicitacao.Modelo != placaOficial.Modelo
                    || placaSolicitacao.MultiSeta != placaOficial.MultiSeta
                    || placaSolicitacao.NumeroAntena != placaOficial.NumeroAntena
                    || placaSolicitacao.NumeroEixos != placaOficial.NumeroEixos
                    || ((placaSolicitacao.NumeroEixosDistanciados.HasValue && placaOficial.NumeroEixosDistanciados.HasValue && placaSolicitacao.NumeroEixosDistanciados != placaOficial.NumeroEixosDistanciados)
                    || (placaSolicitacao.NumeroEixosDistanciados.HasValue && !placaOficial.NumeroEixosDistanciados.HasValue)
                    || (!placaSolicitacao.NumeroEixosDistanciados.HasValue && placaOficial.NumeroEixosDistanciados.HasValue))
                    || ((placaSolicitacao.NumeroEixosPneusDuplos.HasValue && placaOficial.NumeroEixosPneusDuplos.HasValue && placaSolicitacao.NumeroEixosPneusDuplos != placaOficial.NumeroEixosPneusDuplos)
                    || (placaSolicitacao.NumeroEixosPneusDuplos.HasValue && !placaOficial.NumeroEixosPneusDuplos.HasValue)
                    || (!placaSolicitacao.NumeroEixosPneusDuplos.HasValue && placaOficial.NumeroEixosPneusDuplos.HasValue))
                    || placaSolicitacao.Observacao != placaOficial.Observacao
                    || placaSolicitacao.Operacao != placaOficial.Operacao
                    || placaSolicitacao.PlacaVeiculo != placaOficial.PlacaVeiculo
                    || placaSolicitacao.PossuiAbs != placaOficial.PossuiAbs
                    || placaSolicitacao.RazaoSocial != placaOficial.RazaoSocial
                    || ((placaSolicitacao.Setas != null && placaOficial.Setas != null && placaSolicitacao.Setas.Count > placaOficial.Setas.Count)
                    || (placaSolicitacao.Setas == null && placaOficial.Setas != null)
                    || (placaSolicitacao.Setas != null && placaOficial.Setas == null))
                    || Convert.ToDecimal(placaSolicitacao.Tara) != Convert.ToDecimal(placaOficial.Tara)
                    || placaSolicitacao.Versao != placaOficial.Versao)
                        retorno = true;

                    //Uma vez verificado os campos comuns da tabela Placa (acima), 
                    //verifica agora os campos das tabelas específicas de cada país.
                    if (!retorno && VerificaAlteracoesPaises(placaSolicitacao, placaOficial))
                        retorno = true;

                    //se retorno for true nem precisa entrar 
                    if (!retorno && isValidarDocs)
                    {
                        placaSolicitacao.Documentos =
                            new PlacaDocumentoBusiness().ListarPlacaDocumentoPorPlaca(placaSolicitacao.ID);
                        placaOficial.Documentos =
                            new PlacaDocumentoBusiness().ListarPlacaDocumentoPorPlaca(placaOficial.ID);
                        if (placaSolicitacao.Documentos != null && placaSolicitacao.Documentos.Any() &&
                            placaOficial.Documentos != null && placaOficial.Documentos.Any())
                        {
                            var docsAlterados =
                                new PlacaDocumentoBusiness().ListarAlteracoes(placaSolicitacao.Documentos,
                                    placaOficial.Documentos);
                            retorno = docsAlterados.Any(w => w.isAnexoAlterado || w.isDataVencimentoAlterada) || !(placaSolicitacao.Anexo == placaOficial.Anexo);
                        }
                        else if (placaSolicitacao.Documentos == null && placaOficial.Documentos != null)
                            retorno = true;

                        if (!retorno && placaSolicitacao.Operacao == "FOB")
                        {
                            placaSolicitacao.Clientes =
                                new PlacaClienteBusiness().ListarClientesPlaca(placaSolicitacao.ID);
                            placaOficial.Clientes =
                                new PlacaClienteBusiness().ListarClientesPlaca(placaOficial.ID);
                            var distintos = placaSolicitacao.Clientes.Where(a => !placaOficial.Clientes.Any(b => a.IDCliente == b.IDCliente));
                            if (distintos != null && distintos.Any())
                                retorno = true;
                        }
                    }

                    //se retorno for true nem precisa entrar 
                    if (!retorno && isValidarCompartimentos)
                    {
                        placaSolicitacao.Setas =
                            new PlacaSetaBusiness().Listar(p => p.IDPlaca == placaSolicitacao.ID);
                        placaOficial.Setas =
                            new PlacaSetaBusiness().Listar(p => p.IDPlaca == placaOficial.ID);


                        if (placaSolicitacao.Setas != null && placaSolicitacao.Setas.Any() &&
                            placaOficial.Setas != null && placaOficial.Setas.Any())
                        {
                            var setasAlteradas =
                                new PlacaSetaBusiness().ListarAlteracoes(placaSolicitacao.Setas,
                                    placaOficial.Setas);
                            retorno = setasAlteradas.Any(w => w.isPrincipalAlterado || w.isVolumeAlterado);
                        }
                    }
                }
            }
            return retorno;
        }

        public bool VerificaAlteracoesPaises(Placa placaSolicitacao, Placa placaOficial)
        {
            switch (_pais)
            {
                case EnumPais.Brasil:
                    return
                    placaSolicitacao.PlacaBrasil.CPFCNPJ != placaOficial.PlacaBrasil.CPFCNPJ ||
                    placaSolicitacao.PlacaBrasil.IDEstado != placaOficial.PlacaBrasil.IDEstado ||
                    placaSolicitacao.PlacaBrasil.Renavam != placaOficial.PlacaBrasil.Renavam;

                case EnumPais.Argentina:
                    return
                    placaSolicitacao.PlacaArgentina.CUIT != placaOficial.PlacaArgentina.CUIT ||
                    placaSolicitacao.PlacaArgentina.Material != placaOficial.PlacaArgentina.Material ||
                    placaSolicitacao.PlacaArgentina.NrMotor != placaOficial.PlacaArgentina.NrMotor ||
                    !Equals(placaSolicitacao.PlacaArgentina.PBTC, placaOficial.PlacaArgentina.PBTC) ||
                    placaSolicitacao.PlacaArgentina.Potencia != placaOficial.PlacaArgentina.Potencia ||
                    placaSolicitacao.PlacaArgentina.NrMotor != placaOficial.PlacaArgentina.NrMotor ||
                    placaSolicitacao.PlacaArgentina.SatelitalEmpresa != placaOficial.PlacaArgentina.SatelitalEmpresa ||
                    placaSolicitacao.PlacaArgentina.SatelitalMarca != placaOficial.PlacaArgentina.SatelitalMarca ||
                    placaSolicitacao.PlacaArgentina.SatelitalModelo != placaOficial.PlacaArgentina.SatelitalModelo ||
                    placaSolicitacao.PlacaArgentina.SatelitalNrInterno != placaOficial.PlacaArgentina.SatelitalNrInterno ||
                    placaSolicitacao.PlacaArgentina.Vencimento != placaOficial.PlacaArgentina.Vencimento;
            }

            return false;
        }

        public bool isPlacaPendente(int idplaca)
        {
            return new PlacaDocumentoBusiness().Listar(w => w.IDPlaca == idplaca && (w.Bloqueado || w.Vencido)).Count > 0;
        }

        public bool AtualizarPlacaPermissao(Placa PlacaOld, out Placa placaNew)
        {
            using (TransactionScope transactionScope = Transactions.CreateTransactionScope(1, IsolationLevel.ReadCommitted))
            {
                var PlacaClienteBusiness = new PlacaClienteBusiness();
                var PlacaDocumentoBusiness = new PlacaDocumentoBusiness();
                var PlacaSetaBusiness = new PlacaSetaBusiness();

                var composicaoAprovado = PlacaAprovada(PlacaOld);
                placaNew = Selecionar(PlacaOld.ID);
                placaNew.IDTransportadora = PlacaOld.IDTransportadora;
                placaNew.IDTransportadora2 = PlacaOld.IDTransportadora2;

                if (composicaoAprovado)
                {
                    //não está adicionando novos clientes
                    var Clientes = PlacaClienteBusiness.ListarClientesPorPlaca(PlacaOld.ID);
                    if (Clientes != null && Clientes.Any())
                        PlacaOld.Clientes.AddRange(Clientes);
                    PlacaOld.Documentos = PlacaDocumentoBusiness.ListarPlacaDocumentoPorPlaca(PlacaOld.ID);
                    PlacaOld.Setas = PlacaSetaBusiness.Listar(p => p.IDPlaca == PlacaOld.ID);

                    placaNew.ID = 0;
                    Adicionar(placaNew);
                }
                else
                {
                    Atualizar(placaNew);

                    var PlacaClientes = PlacaClienteBusiness.Listar(p => p.IDPlaca == PlacaOld.ID);

                    foreach (var item in PlacaClientes)
                    {
                        if (PlacaOld.Clientes == null || !PlacaOld.Clientes.Any(p => p.ID == item.ID))
                        {

                            PlacaClienteBusiness.Excluir(item.ID);
                        }
                    }
                }

                if (PlacaOld.Clientes != null)
                    foreach (var item in PlacaOld.Clientes)
                    {
                        item.IDPlaca = placaNew.ID;
                        PlacaClienteBusiness.Adicionar(new PlacaCliente { IDCliente = item.IDCliente, IDPlaca = item.IDPlaca });
                    }

                if (PlacaOld.Documentos != null)
                    foreach (var item in PlacaOld.Documentos)
                    {
                        item.IDPlaca = placaNew.ID;
                        PlacaDocumentoBusiness.Adicionar(new PlacaDocumento
                        {
                            IDTipoDocumento = item.IDTipoDocumento,
                            DataVencimento = item.DataVencimento,
                            Anexo = item.Anexo,
                            Vencido = item.Vencido,
                            IDPlaca = item.IDPlaca
                        });
                    }

                if (PlacaOld.Setas != null)
                    foreach (var item in PlacaOld.Setas)
                    {
                        item.IDPlaca = placaNew.ID;
                        PlacaSetaBusiness.Adicionar(new PlacaSeta
                        {

                            VolumeCompartimento1 = item.VolumeCompartimento1,
                            VolumeCompartimento2 = item.VolumeCompartimento2,
                            VolumeCompartimento3 = item.VolumeCompartimento3,
                            VolumeCompartimento4 = item.VolumeCompartimento4,
                            VolumeCompartimento5 = item.VolumeCompartimento5,
                            VolumeCompartimento6 = item.VolumeCompartimento6,
                            VolumeCompartimento7 = item.VolumeCompartimento7,
                            VolumeCompartimento8 = item.VolumeCompartimento8,
                            VolumeCompartimento9 = item.VolumeCompartimento9,
                            VolumeCompartimento10 = item.VolumeCompartimento10,

                            LacreCompartimento1 = item.LacreCompartimento1,
                            LacreCompartimento2 = item.LacreCompartimento2,
                            LacreCompartimento3 = item.LacreCompartimento3,
                            LacreCompartimento4 = item.LacreCompartimento4,
                            LacreCompartimento5 = item.LacreCompartimento5,
                            LacreCompartimento6 = item.LacreCompartimento6,
                            LacreCompartimento7 = item.LacreCompartimento7,
                            LacreCompartimento8 = item.LacreCompartimento8,
                            LacreCompartimento9 = item.LacreCompartimento9,
                            LacreCompartimento10 = item.LacreCompartimento10,

                            CompartimentoPrincipal1 = item.CompartimentoPrincipal1,
                            CompartimentoPrincipal2 = item.CompartimentoPrincipal2,
                            CompartimentoPrincipal3 = item.CompartimentoPrincipal3,
                            CompartimentoPrincipal4 = item.CompartimentoPrincipal4,
                            CompartimentoPrincipal5 = item.CompartimentoPrincipal5,
                            CompartimentoPrincipal6 = item.CompartimentoPrincipal6,
                            CompartimentoPrincipal7 = item.CompartimentoPrincipal7,
                            CompartimentoPrincipal8 = item.CompartimentoPrincipal8,
                            CompartimentoPrincipal9 = item.CompartimentoPrincipal9,
                            CompartimentoPrincipal10 = item.CompartimentoPrincipal10,
                            IDPlaca = item.IDPlaca
                        });
                    }
                transactionScope.Complete();
                return true;
            }
        }

        public bool AtualizarPlaca(Placa Placa)
        {
            return AtualizarPlaca(Placa, EnumPais.Brasil);
        }

        public bool AtualizarPlaca(Placa Placa, EnumPais pais)
        {
            using (TransactionScope transactionScope = Transactions.CreateTransactionScope(1, IsolationLevel.ReadCommitted))
            {
                Placa.DataAtualizacao = DateTime.Now;

                Placa.PlacaVeiculo = Placa.PlacaVeiculo.ToUpper(CultureInfo.InvariantCulture);
                Atualizar(Placa);
                AtualizarTabelasEspecificas(Placa);
                var PlacaClienteBusiness = new PlacaClienteBusiness();
                var UsuarioBusiness = new UsuarioBusiness();
                List<PlacaCliente> PlacaClientes = new List<PlacaCliente>();
                Usuario usuario = null;
                List<int> listIdsTodosClientes = new List<int>(); //Todos os clientes ligados a uma placa

                if (Placa.idUsuario > 0)
                    usuario = UsuarioBusiness.Selecionar(Placa.idUsuario);

                //se for cliente EAB ou FOB irá carregar apenas os clientes daquele usuário e manipular somente esses
                if (Placa.idUsuario > 0 && (usuario != null && usuario.Externo))
                {
                    var placaCls = PlacaClienteBusiness.ListarClientesPorPlaca(Placa.ID, Placa.idUsuario);
                    if (placaCls != null)
                    {
                        if (PlacaClientes == null)
                            PlacaClientes = new List<PlacaCliente>();
                        foreach (var item in placaCls)
                        {
                            PlacaClientes.Add(new PlacaCliente { ID = item.ID, IDCliente = item.IDCliente, IDPlaca = item.IDPlaca });
                        }
                    }
                }
                else
                {
                    PlacaClientes = PlacaClienteBusiness.Listar(p => p.IDPlaca == Placa.ID);
                }
                foreach (var item in PlacaClientes)
                {
                    if (Placa.Clientes == null || !Placa.Clientes.Any(p => p.ID == item.ID))
                    {

                        PlacaClienteBusiness.Excluir(item.ID);
                    }
                }

                //Seleciona todos os clientes vinculados a uma determinada placa
                List<PlacaClienteView> TodosClientes = PlacaClienteBusiness.ListarClientesPorPlaca(Placa.ID);
                if (TodosClientes != null && TodosClientes.Count > 0)
                    listIdsTodosClientes = TodosClientes.Select(c => c.IDCliente).ToList();

                if (Placa.Clientes != null)
                    foreach (var item in Placa.Clientes)
                    {
                        item.IDPlaca = Placa.ID;

                        if (item.ID == 0)
                        {
                            if (!listIdsTodosClientes.Contains(item.IDCliente))
                                PlacaClienteBusiness.Adicionar(new PlacaCliente { IDCliente = item.IDCliente, IDPlaca = Placa.ID, DataAprovacao = item.DataAprovacao });
                        }
                        else
                        {
                            PlacaClienteBusiness.Atualizar(new PlacaCliente { IDCliente = item.IDCliente, IDPlaca = Placa.ID, ID = item.ID, DataAprovacao = item.DataAprovacao });
                        }
                    }

                var PlacaPlacaDocumentoBusiness = new PlacaDocumentoBusiness();
                var PlacaPlacaDocumentos = PlacaPlacaDocumentoBusiness.Listar(p => p.IDPlaca == Placa.ID);

                foreach (var item in PlacaPlacaDocumentos)
                {
                    if (Placa.Documentos == null || !Placa.Documentos.Any(p => p.ID == item.ID))
                    {

                        PlacaPlacaDocumentoBusiness.ExcluirDoc(item.ID);
                    }
                }

                if (Placa.Documentos != null)
                    foreach (var item in Placa.Documentos)
                    {
                        item.IDPlaca = Placa.ID;

                        if (item.ID == 0)
                        {
                            PlacaPlacaDocumentoBusiness.Adicionar(new PlacaDocumento { IDTipoDocumento = item.IDTipoDocumento, IDPlaca = Placa.ID, Anexo = item.Anexo, DataVencimento = item.DataVencimento });
                        }
                        else
                        {
                            PlacaPlacaDocumentoBusiness.Atualizar(new PlacaDocumento { IDTipoDocumento = item.IDTipoDocumento, IDPlaca = Placa.ID, ID = item.ID, Anexo = item.Anexo, DataVencimento = item.DataVencimento });
                        }
                    }


                var PlacaSetaBusiness = new PlacaSetaBusiness();
                var PlacaSetas = PlacaSetaBusiness.Listar(p => p.IDPlaca == Placa.ID);

                foreach (var item in PlacaSetas)
                {
                    if (Placa.Setas == null || !Placa.Setas.Any(p => p.ID == item.ID))
                    {

                        PlacaSetaBusiness.Excluir(item.ID);
                    }
                }

                if (Placa.Setas != null && Placa.IDTipoVeiculo != (int)EnumTipoVeiculo.Cavalo && Placa.IDTipoVeiculo != (int)EnumTipoVeiculo.Dolly)
                    foreach (var item in Placa.Setas)
                    {
                        item.IDPlaca = Placa.ID;

                        if (item.ID == 0)
                        {
                            AdicionarPlacaSeta(Placa, PlacaSetaBusiness, item);
                        }
                        else
                        {
                            PlacaSetaBusiness.Atualizar(new PlacaSeta
                            {
                                ID = item.ID,
                                IDPlaca = Placa.ID,
                                Volume = item.Volume,

                                VolumeCompartimento1 = item.VolumeCompartimento1,
                                VolumeCompartimento2 = item.VolumeCompartimento2,
                                VolumeCompartimento3 = item.VolumeCompartimento3,
                                VolumeCompartimento4 = item.VolumeCompartimento4,
                                VolumeCompartimento5 = item.VolumeCompartimento5,
                                VolumeCompartimento6 = item.VolumeCompartimento6,
                                VolumeCompartimento7 = item.VolumeCompartimento7,
                                VolumeCompartimento8 = item.VolumeCompartimento8,
                                VolumeCompartimento9 = item.VolumeCompartimento9,
                                VolumeCompartimento10 = item.VolumeCompartimento10,

                                LacreCompartimento1 = item.LacreCompartimento1,
                                LacreCompartimento2 = item.LacreCompartimento2,
                                LacreCompartimento3 = item.LacreCompartimento3,
                                LacreCompartimento4 = item.LacreCompartimento4,
                                LacreCompartimento5 = item.LacreCompartimento5,
                                LacreCompartimento6 = item.LacreCompartimento6,
                                LacreCompartimento7 = item.LacreCompartimento7,
                                LacreCompartimento8 = item.LacreCompartimento8,
                                LacreCompartimento9 = item.LacreCompartimento9,
                                LacreCompartimento10 = item.LacreCompartimento10,

                                CompartimentoPrincipal1 = item.CompartimentoPrincipal1,
                                CompartimentoPrincipal2 = item.CompartimentoPrincipal2,
                                CompartimentoPrincipal3 = item.CompartimentoPrincipal3,
                                CompartimentoPrincipal4 = item.CompartimentoPrincipal4,
                                CompartimentoPrincipal5 = item.CompartimentoPrincipal5,
                                CompartimentoPrincipal6 = item.CompartimentoPrincipal6,
                                CompartimentoPrincipal7 = item.CompartimentoPrincipal7,
                                CompartimentoPrincipal8 = item.CompartimentoPrincipal8,
                                CompartimentoPrincipal9 = item.CompartimentoPrincipal9,
                                CompartimentoPrincipal10 = item.CompartimentoPrincipal10
                            });
                        }
                    }

                transactionScope.Complete();
                return true;
            }
        }

        private void AtualizarTabelasEspecificas(Placa placa)
        {
            switch (_pais)
            {
                case EnumPais.Brasil:
                    AtualizarPlacaBrasil(placa.PlacaBrasil, placa.ID);
                    break;
                case EnumPais.Argentina:
                    AtualizarPlacaArgentina(placa.PlacaArgentina, placa.ID);
                    break;
            }
        }

        private void AtualizarPlacaArgentina(PlacaArgentina placaArgentina, int idPlacaAlterada)
        {
            using (UniCadDalRepositorio<PlacaArgentina> placaArgentinaRepositorio = new UniCadDalRepositorio<PlacaArgentina>())
            {
                placaArgentina.IDPlaca = idPlacaAlterada;
                placaArgentina.CUIT = placaArgentina.CUIT.RemoveCharacter();

                placaArgentinaRepositorio.Update(placaArgentina);
            }
        }

        private void AtualizarPlacaBrasil(PlacaBrasil placaBrasil, int idPlacaAlterada)
        {
            using (UniCadDalRepositorio<PlacaBrasil> placaBrasilRepositorio = new UniCadDalRepositorio<PlacaBrasil>())
            {
                placaBrasil.IDPlaca = idPlacaAlterada;
                placaBrasil.CPFCNPJ = placaBrasil.CPFCNPJ.RemoveCharacter();

                placaBrasilRepositorio.Update(placaBrasil);
            }
        }

        private static void AdicionarPlacaSeta(Placa Placa, PlacaSetaBusiness PlacaSetaBusiness, PlacaSeta item)
        {
            PlacaSetaBusiness.Adicionar(new PlacaSeta
            {
                IDPlaca = Placa.ID,
                Volume = item.Volume,
                VolumeCompartimento1 = item.VolumeCompartimento1,
                VolumeCompartimento2 = item.VolumeCompartimento2,
                VolumeCompartimento3 = item.VolumeCompartimento3,
                VolumeCompartimento4 = item.VolumeCompartimento4,
                VolumeCompartimento5 = item.VolumeCompartimento5,
                VolumeCompartimento6 = item.VolumeCompartimento6,
                VolumeCompartimento7 = item.VolumeCompartimento7,
                VolumeCompartimento8 = item.VolumeCompartimento8,
                VolumeCompartimento9 = item.VolumeCompartimento9,
                VolumeCompartimento10 = item.VolumeCompartimento10,

                CompartimentoPrincipal1 = item.CompartimentoPrincipal1,
                CompartimentoPrincipal2 = item.CompartimentoPrincipal2,
                CompartimentoPrincipal3 = item.CompartimentoPrincipal3,
                CompartimentoPrincipal4 = item.CompartimentoPrincipal4,
                CompartimentoPrincipal5 = item.CompartimentoPrincipal5,
                CompartimentoPrincipal6 = item.CompartimentoPrincipal6,
                CompartimentoPrincipal7 = item.CompartimentoPrincipal7,
                CompartimentoPrincipal8 = item.CompartimentoPrincipal8,
                CompartimentoPrincipal9 = item.CompartimentoPrincipal9,
                CompartimentoPrincipal10 = item.CompartimentoPrincipal10,

                LacreCompartimento1 = item.LacreCompartimento1,
                LacreCompartimento2 = item.LacreCompartimento2,
                LacreCompartimento3 = item.LacreCompartimento3,
                LacreCompartimento4 = item.LacreCompartimento4,
                LacreCompartimento5 = item.LacreCompartimento5,
                LacreCompartimento6 = item.LacreCompartimento6,
                LacreCompartimento7 = item.LacreCompartimento7,
                LacreCompartimento8 = item.LacreCompartimento8,
                LacreCompartimento9 = item.LacreCompartimento9,
                LacreCompartimento10 = item.LacreCompartimento10
            });
        }

        public List<Placa> ListarPorComposicao(Composicao comp)
        {
            List<Placa> placas = new List<Placa>();

            var placaBll = new PlacaBusiness(_pais);

            Double tara = 0;
            tara += placaBll.Selecionar(comp.IDPlaca1.Value).Tara;
            if (comp.IDPlaca2.HasValue)
                tara += placaBll.Selecionar(comp.IDPlaca2.Value).Tara;
            if (comp.IDPlaca3.HasValue)
                tara += placaBll.Selecionar(comp.IDPlaca3.Value).Tara;
            if (comp.IDPlaca4.HasValue)
                tara += placaBll.Selecionar(comp.IDPlaca4.Value).Tara;

            CarregarPlaca(placas, comp.IDPlaca1, tara, comp.PBTC);
            CarregarPlaca(placas, comp.IDPlaca2, tara, comp.PBTC);
            CarregarPlaca(placas, comp.IDPlaca3, tara, comp.PBTC);
            CarregarPlaca(placas, comp.IDPlaca4, tara, comp.PBTC);

            return placas;
        }

        private List<Placa> CarregarPlacaCapacidade(Composicao comp, double taraTotalComposicao, double? pbtc)
        {
            List<Placa> placas = new List<Placa>();
            List<Placa> retorno = new List<Placa>();

            if (comp.IDPlaca1.HasValue)
            {
                placas.Add(new PlacaBusiness().Selecionar(comp.IDPlaca1.Value));
            }
            if (comp.IDPlaca2.HasValue)
            {
                placas.Add(new PlacaBusiness().Selecionar(comp.IDPlaca2.Value));
            }
            if (comp.IDPlaca3.HasValue)
            {
                placas.Add(new PlacaBusiness().Selecionar(comp.IDPlaca3.Value));
            }
            if (comp.IDPlaca4.HasValue)
            {
                placas.Add(new PlacaBusiness().Selecionar(comp.IDPlaca4.Value));
            }
            foreach (var item in placas)
            {
                item.Setas = new PlacaSetaBusiness().Listar(p => p.IDPlaca == item.ID);
            }

            Placa placa = new Placa();

            if (placas != null && placas.Any())
            {
                var totalSetas = placas.Where(p => p.Setas != null && p.Setas.Any()).Min(p => p.Setas.Count);
                PlacaSeta[] setas = new PlacaSeta[totalSetas];

                foreach (var item in placas.Where(p => p.Setas != null))
                {
                    for (int i = 0; i < totalSetas; i++)
                    {
                        if (item.Setas.Count > i)
                        {
                            if (setas[i] == null)
                            {
                                setas[i] = new PlacaSeta
                                {
                                    VolumeCompartimento1 = item.Setas[i].VolumeCompartimento1.HasValue ? item.Setas[i].Compartimento1IsInativo.HasValue && item.Setas[i].Compartimento1IsInativo.Value ? 0 : item.Setas[i].VolumeCompartimento1.Value : 0,
                                    VolumeCompartimento2 = item.Setas[i].VolumeCompartimento2.HasValue ? item.Setas[i].Compartimento2IsInativo.HasValue && item.Setas[i].Compartimento2IsInativo.Value ? 0 : item.Setas[i].VolumeCompartimento2.Value : 0,
                                    VolumeCompartimento3 = item.Setas[i].VolumeCompartimento3.HasValue ? item.Setas[i].Compartimento3IsInativo.HasValue && item.Setas[i].Compartimento3IsInativo.Value ? 0 : item.Setas[i].VolumeCompartimento3.Value : 0,
                                    VolumeCompartimento4 = item.Setas[i].VolumeCompartimento4.HasValue ? item.Setas[i].Compartimento4IsInativo.HasValue && item.Setas[i].Compartimento4IsInativo.Value ? 0 : item.Setas[i].VolumeCompartimento4.Value : 0,
                                    VolumeCompartimento5 = item.Setas[i].VolumeCompartimento5.HasValue ? item.Setas[i].Compartimento5IsInativo.HasValue && item.Setas[i].Compartimento5IsInativo.Value ? 0 : item.Setas[i].VolumeCompartimento5.Value : 0,
                                    VolumeCompartimento6 = item.Setas[i].VolumeCompartimento6.HasValue ? item.Setas[i].Compartimento6IsInativo.HasValue && item.Setas[i].Compartimento6IsInativo.Value ? 0 : item.Setas[i].VolumeCompartimento6.Value : 0,
                                    VolumeCompartimento7 = item.Setas[i].VolumeCompartimento7.HasValue ? item.Setas[i].Compartimento7IsInativo.HasValue && item.Setas[i].Compartimento7IsInativo.Value ? 0 : item.Setas[i].VolumeCompartimento7.Value : 0,
                                    VolumeCompartimento8 = item.Setas[i].VolumeCompartimento8.HasValue ? item.Setas[i].Compartimento8IsInativo.HasValue && item.Setas[i].Compartimento8IsInativo.Value ? 0 : item.Setas[i].VolumeCompartimento8.Value : 0,
                                    VolumeCompartimento9 = item.Setas[i].VolumeCompartimento9.HasValue ? item.Setas[i].Compartimento9IsInativo.HasValue && item.Setas[i].Compartimento9IsInativo.Value ? 0 : item.Setas[i].VolumeCompartimento9.Value : 0,
                                    VolumeCompartimento10 = item.Setas[i].VolumeCompartimento10.HasValue ? item.Setas[i].Compartimento10IsInativo.HasValue && item.Setas[i].Compartimento10IsInativo.Value ? 0 : item.Setas[i].VolumeCompartimento10.Value : 0,

                                };
                            }
                            else
                            {
                                setas[i].VolumeCompartimento1 = (setas[i].VolumeCompartimento1.HasValue ?
                                                                       setas[i].Compartimento1IsInativo.HasValue
                                                                    && setas[i].Compartimento1IsInativo.Value ? 0 : setas[i].VolumeCompartimento1.Value : 0) + item.Setas[i].VolumeCompartimento1;
                                setas[i].VolumeCompartimento2 = (setas[i].VolumeCompartimento2.HasValue ?
                                                                    setas[i].Compartimento2IsInativo.HasValue
                                                                    && setas[i].Compartimento2IsInativo.Value ? 0 : setas[i].VolumeCompartimento2.Value : 0) + item.Setas[i].VolumeCompartimento2;
                                setas[i].VolumeCompartimento3 = (setas[i].VolumeCompartimento3.HasValue ?
                                                                    setas[i].Compartimento3IsInativo.HasValue
                                                                    && setas[i].Compartimento3IsInativo.Value ? 0 : setas[i].VolumeCompartimento3.Value : 0) + item.Setas[i].VolumeCompartimento3;
                                setas[i].VolumeCompartimento4 = (setas[i].VolumeCompartimento4.HasValue ?
                                                                    setas[i].Compartimento4IsInativo.HasValue
                                                                    && setas[i].Compartimento4IsInativo.Value ? 0 : setas[i].VolumeCompartimento4.Value : 0) + item.Setas[i].VolumeCompartimento4;
                                setas[i].VolumeCompartimento5 = (setas[i].VolumeCompartimento5.HasValue ?
                                                                    setas[i].Compartimento5IsInativo.HasValue
                                                                    && setas[i].Compartimento5IsInativo.Value ? 0 : setas[i].VolumeCompartimento5.Value : 0) + item.Setas[i].VolumeCompartimento5;
                                setas[i].VolumeCompartimento6 = (setas[i].VolumeCompartimento6.HasValue ?
                                                                    setas[i].Compartimento6IsInativo.HasValue
                                                                    && setas[i].Compartimento6IsInativo.Value ? 0 : setas[i].VolumeCompartimento6.Value : 0) + item.Setas[i].VolumeCompartimento6;
                                setas[i].VolumeCompartimento7 = (setas[i].VolumeCompartimento7.HasValue ?
                                                                    setas[i].Compartimento7IsInativo.HasValue
                                                                    && setas[i].Compartimento7IsInativo.Value ? 0 : setas[i].VolumeCompartimento7.Value : 0) + item.Setas[i].VolumeCompartimento7;
                                setas[i].VolumeCompartimento8 = (setas[i].VolumeCompartimento8.HasValue ?
                                                                    setas[i].Compartimento8IsInativo.HasValue
                                                                    && setas[i].Compartimento8IsInativo.Value ? 0 : setas[i].VolumeCompartimento8.Value : 0) + item.Setas[i].VolumeCompartimento8;
                                setas[i].VolumeCompartimento9 = (setas[i].VolumeCompartimento9.HasValue ?
                                                                    setas[i].Compartimento9IsInativo.HasValue
                                                                    && setas[i].Compartimento9IsInativo.Value ? 0 : setas[i].VolumeCompartimento9.Value : 0) + item.Setas[i].VolumeCompartimento9;
                                setas[i].VolumeCompartimento10 = (setas[i].VolumeCompartimento10.HasValue ?
                                                                    setas[i].Compartimento10IsInativo.HasValue
                                                                    && setas[i].Compartimento10IsInativo.Value ? 0 : setas[i].VolumeCompartimento10.Value : 0) + item.Setas[i].VolumeCompartimento10;
                            }
                        }
                    }
                }

                foreach (var item in placas)
                {
                    placa.PlacaVeiculo = $"{placa.PlacaVeiculo} {item.PlacaVeiculo} /";
                    foreach (var seta in setas)
                    {
                        if (seta.Produtos == null)
                            seta.Produtos = new List<Produto>();
                        var lista = new ProdutoBusiness().Listar(p => p.IDTipoProduto == item.IDTipoProduto);
                        foreach (var prod in lista)
                        {
                            if (!seta.Produtos.Any(p => p.ID == prod.ID))
                                seta.Produtos.Add(prod);
                        }
                    }
                }

                placa.PlacaVeiculo = placa.PlacaVeiculo.Substring(0, placa.PlacaVeiculo.Length - 1);
                foreach (var item in setas)
                {
                    item.Produtos = item.Produtos.Distinct().ToList();

                    foreach (var prod in item.Produtos)
                    {
                        CalcularCapacidade(taraTotalComposicao, pbtc, item, prod);
                    }
                }

                placa.Setas = setas.ToList();

                retorno.Add(placa);
            }
            return retorno;
        }

        private static void CalcularCapacidade(double taraTotalComposicao, double? pbtc, PlacaSeta seta, Produto prod)
        {
            var capacidadeCarreta = pbtc;
            double setaCargaTotal = VolumeTotalSeta(seta);

            var pesoTotal = (prod.Densidade * (setaCargaTotal / 1000)) + (taraTotalComposicao);
            prod.Liberado = capacidadeCarreta >= pesoTotal;
            if (capacidadeCarreta > pesoTotal)
                prod.Situacao = (int)EnumSituacaoPlacaLimite.Permitido;
            else if (capacidadeCarreta < pesoTotal)
                prod.Situacao = (int)EnumSituacaoPlacaLimite.NaoPermitido;
            else
                prod.Situacao = (int)EnumSituacaoPlacaLimite.NoLimite;
        }

        private static double VolumeTotalSeta(PlacaSeta seta)
        {
            return (double)(
                        (seta.VolumeCompartimento1.HasValue ? seta.VolumeCompartimento1.Value : 0) +
                        (seta.VolumeCompartimento2.HasValue ? seta.VolumeCompartimento2.Value : 0) +
                        (seta.VolumeCompartimento3.HasValue ? seta.VolumeCompartimento3.Value : 0) +
                        (seta.VolumeCompartimento4.HasValue ? seta.VolumeCompartimento4.Value : 0) +
                        (seta.VolumeCompartimento5.HasValue ? seta.VolumeCompartimento5.Value : 0) +
                        (seta.VolumeCompartimento6.HasValue ? seta.VolumeCompartimento6.Value : 0) +
                        (seta.VolumeCompartimento7.HasValue ? seta.VolumeCompartimento7.Value : 0) +
                        (seta.VolumeCompartimento8.HasValue ? seta.VolumeCompartimento8.Value : 0) +
                        (seta.VolumeCompartimento9.HasValue ? seta.VolumeCompartimento9.Value : 0) +
                        (seta.VolumeCompartimento10.HasValue ? seta.VolumeCompartimento10.Value : 0));
        }

        private void CarregarPlaca(List<Placa> placas, int? idPlaca, double taraTotalComposicao, double? pbtc)
        {
            if (idPlaca.HasValue)
            {
                var placa = Selecionar(x => x.ID == idPlaca.Value);
                placa.Documentos = new PlacaDocumentoBusiness().ListarPlacaDocumentoPorPlaca(placa.ID);
                placa.Documentos.Add(new PlacaDocumentoView { Descricao = Traducao.GetTextoPorLingua("ANEXO GERAL", "ANEXO GENERAL", _pais), Anexo = placa.Anexo });
                placa.Setas = new PlacaSetaBusiness().Listar(p => p.IDPlaca == placa.ID);
                if (placa.IDTipoProduto.HasValue && placa.Setas != null)
                {
                    foreach (var seta in placa.Setas)
                    {
                        seta.Produtos = new ProdutoBusiness().Listar(p => p.IDTipoProduto == placa.IDTipoProduto);
                        if (seta.Produtos != null)
                            foreach (var prod in seta.Produtos)
                            {
                                var capacidadeCarreta = pbtc;
                                double setaCargaTotal = (double)(
                                (seta.VolumeCompartimento1.HasValue ? seta.VolumeCompartimento1.Value : 0) +
                                (seta.VolumeCompartimento2.HasValue ? seta.VolumeCompartimento2.Value : 0) +
                                (seta.VolumeCompartimento3.HasValue ? seta.VolumeCompartimento3.Value : 0) +
                                (seta.VolumeCompartimento4.HasValue ? seta.VolumeCompartimento4.Value : 0) +
                                (seta.VolumeCompartimento5.HasValue ? seta.VolumeCompartimento5.Value : 0) +
                                (seta.VolumeCompartimento6.HasValue ? seta.VolumeCompartimento6.Value : 0) +
                                (seta.VolumeCompartimento7.HasValue ? seta.VolumeCompartimento7.Value : 0) +
                                (seta.VolumeCompartimento8.HasValue ? seta.VolumeCompartimento8.Value : 0) +
                                (seta.VolumeCompartimento9.HasValue ? seta.VolumeCompartimento9.Value : 0) +
                                (seta.VolumeCompartimento10.HasValue ? seta.VolumeCompartimento10.Value : 0));

                                var pesoTotal = (prod.Densidade * (setaCargaTotal / 1000)) + (taraTotalComposicao);
                                prod.Liberado = capacidadeCarreta > pesoTotal;
                                if (capacidadeCarreta > pesoTotal)
                                    prod.Situacao = (int)EnumSituacaoPlacaLimite.Permitido;
                                else if (capacidadeCarreta < pesoTotal)
                                    prod.Situacao = (int)EnumSituacaoPlacaLimite.NaoPermitido;
                                else
                                    prod.Situacao = (int)EnumSituacaoPlacaLimite.NoLimite;

                            }
                    }
                }

                new PlacaBusiness().CalcularColunas(placa);
                placas.Add(placa);
            }
        }

        public virtual Placa ObtemPlaca1(Composicao composicao)
        {
            return ObtemPlacaPorId(composicao.IDPlaca1);
        }

        public virtual Placa ObtemPlaca2(Composicao composicao)
        {
            return ObtemPlacaPorId(composicao.IDPlaca2);
        }

        private Placa ObtemPlacaPorId(int? placaID)
        {
            if (placaID != null)
                return this.Selecionar((int)placaID);

            return null;
        }

        private MensagemValidacaoView ValidarVencimento(string placaDoVeiculo, string xDocumento, PlacaDocumentoAAServicoView documento, PlacaValidarFiltro placaFiltro)
        {
            int alerta = 10;
            if (placaFiltro?.WarningAdviceTime != null)
                alerta = placaFiltro.WarningAdviceTime.Value;

            MensagemValidacaoView msgRetorno = new MensagemValidacaoView();

            //if (documento.DataVencimento == null)
            //    return msgRetorno;

            if ((documento.DataVencimento == null || documento.DataVencimento < DateTime.Today) && documento.Obrigatorio)
            {
                msgRetorno.Mensagem = $"Veiculo {placaDoVeiculo} com {xDocumento} vencido: {documento.DataVencimento?.ToString("dd/MM/yyyy")}.";
                msgRetorno.RestringirOperacao = true;
            }
            //else if (placaFiltro?.WarningAdviceTime != null && documento.DataVencimento.Value.AddDays(alerta) < DateTime.Today)
            //{
            //    msgRetorno.Mensagem = $"Veiculo {placaDoVeiculo} ficará com {xDocumento} vencido em {documento.DataVencimento?.ToString("dd/MM/yyyy")}. Continue o carregamento hoje, mas providencie a atualização até esta data.\r\n";
            //    msgRetorno.RestringirOperacao = false;
            //}
            return msgRetorno;
        }

        public List<MensagemValidacaoView> ValidarPlacasDocumentos(ComposicaoAAServicoView dadosComposicao, PlacaValidarFiltro placaFiltro)
        {

            List<MensagemValidacaoView> lstMsgReturn = new List<MensagemValidacaoView>();

            var msgRet = ValidarListaDocumentos(dadosComposicao?.PlacaCarreta1, placaFiltro, "da carreta");
            if (msgRet != null && msgRet.Count > 0)
                lstMsgReturn.AddRange(msgRet);

            if (placaFiltro.IsValidarComposicao)
            {
                msgRet = ValidarListaDocumentos(dadosComposicao?.PlacaCavaloTruck, placaFiltro, "do cavalo");
                if (msgRet != null && msgRet.Count > 0)
                    lstMsgReturn.AddRange(msgRet);

                msgRet = ValidarListaDocumentos(dadosComposicao?.PlacaCarreta2, placaFiltro, "da 2ª carreta");
                if (msgRet != null && msgRet.Count > 0)
                    lstMsgReturn.AddRange(msgRet);

                msgRet = ValidarListaDocumentos(dadosComposicao?.PlacaDollyCarreta2, placaFiltro, "da dolly");
                if (msgRet != null && msgRet.Count > 0)
                    lstMsgReturn.AddRange(msgRet);
            }

            if (lstMsgReturn != null && lstMsgReturn.Count < 1)
            {
                lstMsgReturn.Add(new MensagemValidacaoView
                {
                    Mensagem = "Veículo validado com sucesso, sem restrição de documento",
                    RestringirOperacao = false
                });
            }

            return lstMsgReturn;
        }

        private List<MensagemValidacaoView> ValidarListaDocumentos(PlacaAAServicoView placaServiceView, PlacaValidarFiltro placaFiltro, string descricaoVeiculo)
        {
            List<MensagemValidacaoView> lstMsgReturn = new List<MensagemValidacaoView>();
            if (placaServiceView != null)
            {
                var msgRet = ValidarListaDocumentos(placaServiceView.ListaDocumentos, placaServiceView.Placa, descricaoVeiculo, placaFiltro);
                if (msgRet != null && msgRet.Count > 0)
                    lstMsgReturn.AddRange(msgRet);
            }
            return lstMsgReturn;
        }

        private List<MensagemValidacaoView> ValidarListaDocumentos(List<PlacaDocumentoAAServicoView> documentos, string placa, string descricaoVeiculo, PlacaValidarFiltro placaFiltro)
        {
            List<MensagemValidacaoView> lstMensagemResult = new List<MensagemValidacaoView>();
            foreach (PlacaDocumentoAAServicoView documento in documentos)
            {
                var msg = MensagemVencimento(documento, placa, descricaoVeiculo, placaFiltro);
                if (msg != null && !string.IsNullOrWhiteSpace(msg.Mensagem))
                    lstMensagemResult.Add(msg);
            }

            return lstMensagemResult;
        }

        private MensagemValidacaoView MensagemVencimento(PlacaDocumentoAAServicoView documento, string placa, string veiculo, PlacaValidarFiltro placaFiltro)
        {
            MensagemValidacaoView msgRetorno = new MensagemValidacaoView();
            return ValidarVencimento(placa, $"{documento.Sigla} {veiculo}", documento, placaFiltro);
        }

        public List<PlacaAAServicoView> ListarPlacaAAServico(PlacaServicoFiltro filtro)
        {
            using (UniCadDalRepositorio<Motorista> repositorio = new UniCadDalRepositorio<Motorista>())
            {
                var resultado = GetQueryPlacaServico(filtro);
                List<PlacaAAServicoView> lstView = new List<PlacaAAServicoView>();

                resultado.ForEach(r =>
                {
                    lstView.Add(new PlacaAAServicoView
                    {
                        ID = r.ID,
                        LinhaNegocio = r.LinhaNegocio,
                        Marca = r.Marca,
                        Observacao = r.Observacao,
                        Operacao = r.Operacao,
                        Placa = r.Placa,
                    });
                });


                if (lstView != null && lstView.Any())
                    lstView.ForEach(x =>
                    {
                        //x.ListaCompartimentos = ListarCompartimentosPorIdPlaca(x.ID, repositorio);
                        x.ListaDocumentos = ListarDocumentosPorIdPlacaAA(x.ID, repositorio);
                        //x.ListaPermissoes = ListarPermissoesPorIdPlaca(x.ID, x.Operacao, repositorio);
                    });
                return lstView;
            }
        }

        private List<PlacaDocumentoAAServicoView> ListarDocumentosPorIdPlacaAA(int Id, UniCadDalRepositorio<Motorista> repositorio)
        {
            var documentos = (from app in repositorio.ListComplex<Placa>().AsNoTracking().OrderBy(i => i.ID)
                              join docs in repositorio.ListComplex<PlacaDocumento>().AsNoTracking() on app.ID equals docs.IDPlaca
                              join tipoDoc in repositorio.ListComplex<TipoDocumento>().AsNoTracking() on docs.IDTipoDocumento equals tipoDoc.ID
                              where (app.ID == Id)
                              select new PlacaDocumentoAAServicoView
                              {
                                  Sigla = tipoDoc.Sigla,
                                  Descricao = tipoDoc.Descricao,
                                  DataVencimento = docs.DataVencimento,
                                  Obrigatorio = tipoDoc.Obrigatorio
                              });
            return documentos.ToList();
        }
    }
}

