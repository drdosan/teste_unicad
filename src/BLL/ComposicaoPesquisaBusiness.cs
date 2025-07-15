using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Raizen.Framework.Log.Bases;
using Raizen.Framework.Models;
using Raizen.UniCad.Model;
using Raizen.UniCad.Model.Filtro;
using Raizen.UniCad.Model.View;

namespace Raizen.UniCad.BLL
{
    public class ComposicaoPesquisaBusiness : UniCadBusinessBase<ComposicaoView>
    {
        private readonly EnumPais _pais;

        #region Constructors

        public ComposicaoPesquisaBusiness()
        {
            _pais = EnumPais.Brasil;
        }

        public ComposicaoPesquisaBusiness(EnumPais pais)
        {
            _pais = pais;
        }

        #endregion

        public List<ComposicaoView> ListarComposicaoRelatorio(ComposicaoFiltro filtro)
        {
            var dados = GetComposicao(filtro, false);
            return dados;
        }

        public List<ComposicaoView> ListarComposicaoExportar(ComposicaoFiltro filtro)
        {
            var dados = GetComposicaoExportar(filtro, false);
            return dados;
        }

        public int ListarComposicaoCount(ComposicaoFiltro filtro)
        {
            var dados = GetComposicao(filtro, true);
            return dados[0].Linhas;
        }

        public List<ComposicaoView> ListarComposicao(ComposicaoFiltro filtro, PaginadorModel paginador)
        {
            int ultimaPagina = paginador.QtdeItensPagina * paginador.PaginaAtual;

            if (paginador.QtdeTotalRegistros <= paginador.QtdeItensPagina)
            {
                return GetComposicao(filtro, false, paginador.InicioPaginacao, null);
            }
            else
            {
                return GetComposicao(filtro, false, paginador.InicioPaginacao, ultimaPagina);
            }
        }

        private List<ComposicaoView> GetComposicao(ComposicaoFiltro filtro, bool isCount, long? paginalInicial = null, long? paginalFinal = null)
        {
            SqlParameter paramIsCount = new SqlParameter("@IsCount", SqlDbType.Bit);
            SqlParameter paramPrimeiraPagina = new SqlParameter("@PrimeiraPagina", SqlDbType.Int);
            SqlParameter paramUltimaPagina = new SqlParameter("@UltimaPagina", SqlDbType.Int);
            SqlParameter paramIdEmpresa = new SqlParameter("@IDEmpresa", SqlDbType.Int);
            SqlParameter paramIdStatus = new SqlParameter("@IDStatus", SqlDbType.Int);
            SqlParameter paramOperacao = new SqlParameter("@Operacao", SqlDbType.VarChar);
            SqlParameter paramIdTipoComposicao = new SqlParameter("@IDTipoComposicao", SqlDbType.Int);
            SqlParameter paramChamado = new SqlParameter("@Chamado", SqlDbType.VarChar);
            SqlParameter paramInicio = new SqlParameter("@DataInicio", SqlDbType.DateTime);
            SqlParameter paramFim = new SqlParameter("@DataFim", SqlDbType.DateTime);
            SqlParameter paramPlaca = new SqlParameter("@Placa", SqlDbType.VarChar);
            SqlParameter paramIdTransportadora = new SqlParameter("@IDTransportadora", SqlDbType.Int);
            SqlParameter paramIdUsuarioTransportadora = new SqlParameter("@IDUsuarioTransportadora", SqlDbType.Int);
            SqlParameter paramIdCliente = new SqlParameter("@IDCliente", SqlDbType.Int);
            SqlParameter paramIdUsuarioCliente = new SqlParameter("@IDUsuarioCliente", SqlDbType.Int);
            SqlParameter paramIdPais = new SqlParameter("@IdPais", SqlDbType.Int);

            paramIsCount.Value = isCount;
            paramPrimeiraPagina.Value = paginalInicial ?? (object)DBNull.Value;
            paramUltimaPagina.Value = paginalFinal ?? (object)DBNull.Value;
            paramIdEmpresa.Value = filtro.IDEmpresa ?? (object)DBNull.Value;
            paramIdStatus.Value = filtro.IDStatus ?? (object)DBNull.Value;
            paramOperacao.Value = String.IsNullOrEmpty(filtro.Operacao) ? (object)DBNull.Value : filtro.Operacao;
            paramIdTipoComposicao.Value = filtro.IDTipoComposicao ?? (object)DBNull.Value;
            paramChamado.Value = String.IsNullOrEmpty(filtro.Chamado) ? (object)DBNull.Value : filtro.Chamado;
            paramInicio.Value = filtro.DataInicio ?? (object)DBNull.Value;
            paramFim.Value = filtro.DataFim ?? (object)DBNull.Value;
            paramPlaca.Value = String.IsNullOrEmpty(filtro.Placa) ? (object)DBNull.Value : filtro.Placa;
            paramIdTransportadora.Value = filtro.IDTransportadora ?? (object)DBNull.Value;
            paramIdUsuarioTransportadora.Value = filtro.IDUsuarioTransportadora ?? (object)DBNull.Value;
            paramIdCliente.Value = filtro.IDCliente ?? (object)DBNull.Value;
            paramIdUsuarioCliente.Value = filtro.IDUsuarioCliente ?? (object)DBNull.Value;
            paramIdPais.Value = (int)_pais;

            List<ComposicaoView> dadosRelatorio = ExecutarProcedureComRetorno(
                "[dbo].[Proc_Pesquisa_Composicao] @IsCount,@PrimeiraPagina,@UltimaPagina,@IDEmpresa,@IDStatus,@Operacao,@IDTipoComposicao,@Chamado,@DataInicio,@DataFim,@Placa,@IDTransportadora,@IDUsuarioTransportadora,@IDCliente,@IDUsuarioCliente,@IdPais",
                new Object[] { paramIsCount, paramPrimeiraPagina, paramUltimaPagina, paramIdEmpresa, paramIdStatus, paramOperacao, paramIdTipoComposicao, paramChamado, paramInicio, paramFim, paramPlaca, paramIdTransportadora, paramIdUsuarioTransportadora, paramIdCliente, paramIdUsuarioCliente, paramIdPais });
            //List<ComposicaoView> dadosRelatorio = ExecutarProcedureComRetornoD(
            //    "[dbo].[Proc_Pesquisa_Composicao]",
            //    new SqlParameter[] { paramIsCount, paramPrimeiraPagina, paramUltimaPagina, paramIdEmpresa, paramIdStatus, paramOperacao, paramIdTipoComposicao, paramChamado, paramInicio, paramFim, paramPlaca, paramIdTransportadora, paramIdUsuarioTransportadora, paramIdCliente, paramIdUsuarioCliente });
            return dadosRelatorio;
        }



        private List<ComposicaoView> GetComposicaoExportar(ComposicaoFiltro filtro, bool isCount, long? paginalInicial = null, long? paginalFinal = null)
        {
            SqlParameter paramIsCount = new SqlParameter("@IsCount", SqlDbType.Bit);
            SqlParameter paramPrimeiraPagina = new SqlParameter("@PrimeiraPagina", SqlDbType.Int);
            SqlParameter paramUltimaPagina = new SqlParameter("@UltimaPagina", SqlDbType.Int);
            SqlParameter paramIdEmpresa = new SqlParameter("@IDEmpresa", SqlDbType.Int);
            SqlParameter paramIdStatus = new SqlParameter("@IDStatus", SqlDbType.Int);
            SqlParameter paramOperacao = new SqlParameter("@Operacao", SqlDbType.VarChar);
            SqlParameter paramIdTipoComposicao = new SqlParameter("@IDTipoComposicao", SqlDbType.Int);
            SqlParameter paramChamado = new SqlParameter("@Chamado", SqlDbType.VarChar);
            SqlParameter paramInicio = new SqlParameter("@DataInicio", SqlDbType.DateTime);
            SqlParameter paramFim = new SqlParameter("@DataFim", SqlDbType.DateTime);
            SqlParameter paramPlaca = new SqlParameter("@Placa", SqlDbType.VarChar);
            SqlParameter paramIdTransportadora = new SqlParameter("@IDTransportadora", SqlDbType.Int);
            SqlParameter paramIdUsuarioTransportadora = new SqlParameter("@IDUsuarioTransportadora", SqlDbType.Int);
            SqlParameter paramIdCliente = new SqlParameter("@IDCliente", SqlDbType.Int);
            SqlParameter paramIdUsuarioCliente = new SqlParameter("@IDUsuarioCliente", SqlDbType.Int);
            SqlParameter paramIdPais = new SqlParameter("@IdPais", SqlDbType.Int);

            paramIsCount.Value = isCount;
            paramPrimeiraPagina.Value = paginalInicial ?? (object)DBNull.Value;
            paramUltimaPagina.Value = paginalFinal ?? (object)DBNull.Value;
            paramIdEmpresa.Value = filtro.IDEmpresa ?? (object)DBNull.Value;
            paramIdStatus.Value = filtro.IDStatus ?? (object)DBNull.Value;
            paramOperacao.Value = String.IsNullOrEmpty(filtro.Operacao) ? (object)DBNull.Value : filtro.Operacao;
            paramIdTipoComposicao.Value = filtro.IDTipoComposicao ?? (object)DBNull.Value;
            paramChamado.Value = String.IsNullOrEmpty(filtro.Chamado) ? (object)DBNull.Value : filtro.Chamado;
            paramInicio.Value = filtro.DataInicio ?? (object)DBNull.Value;
            paramFim.Value = filtro.DataFim ?? (object)DBNull.Value;
            paramPlaca.Value = String.IsNullOrEmpty(filtro.Placa) ? (object)DBNull.Value : filtro.Placa;
            paramIdTransportadora.Value = filtro.IDTransportadora ?? (object)DBNull.Value;
            paramIdUsuarioTransportadora.Value = filtro.IDUsuarioTransportadora ?? (object)DBNull.Value;
            paramIdCliente.Value = filtro.IDCliente ?? (object)DBNull.Value;
            paramIdUsuarioCliente.Value = filtro.IDUsuarioCliente ?? (object)DBNull.Value;
            paramIdPais.Value = (int)_pais;

            List<ComposicaoView> dadosRelatorio = ExecutarProcedureComRetorno(
                "[dbo].[Proc_Pesquisa_Composicao_Excel] @IsCount,@PrimeiraPagina,@UltimaPagina,@IDEmpresa,@IDStatus,@Operacao,@IDTipoComposicao,@Chamado,@DataInicio,@DataFim,@Placa,@IDTransportadora,@IDUsuarioTransportadora,@IDCliente,@IDUsuarioCliente,@IdPais",
                new Object[] { paramIsCount, paramPrimeiraPagina, paramUltimaPagina, paramIdEmpresa, paramIdStatus, paramOperacao, paramIdTipoComposicao, paramChamado, paramInicio, paramFim, paramPlaca, paramIdTransportadora, paramIdUsuarioTransportadora, paramIdCliente, paramIdUsuarioCliente, paramIdPais });

            return dadosRelatorio;
        }
    }
}
