using System;
using System.Collections.Generic;
using System.Linq;
using Raizen.Framework.Models;
using Raizen.UniCad.DAL;
using Raizen.UniCad.Model;
using Raizen.UniCad.Model.Filtro;
using Raizen.UniCad.Model.View;
using System.IO;
using ClosedXML.Excel;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Raizen.UniCad.BLL.Util;
using System.Data.SqlClient;
using System.Data;
using Raizen.UniCad.Extensions;

namespace Raizen.UniCad.BLL
{
    public class AgendamentoTreinamentoBusiness : UniCadBusinessBase<AgendamentoTreinamento>
    {
        private readonly MotoristaBusiness _motoristaBll = new MotoristaBusiness();
        private readonly HistoricoTreinamentoTeoricoMotoristaBusiness _histTreinamentoMotBll = new HistoricoTreinamentoTeoricoMotoristaBusiness();
        private readonly MotoristaTreinamentoTerminalBusiness _motTreinamentoTerminalBll = new MotoristaTreinamentoTerminalBusiness();
        private readonly AgendamentoTerminalHorarioBusiness _agendamentoHorarioBll = new AgendamentoTerminalHorarioBusiness();
        public List<AgendamentoTreinamentoView> ListarAgendamentoTreinamento(AgendamentoTreinamentoFiltro filtro, PaginadorModel paginador)
        {

            int ultimaPagina = paginador.QtdeItensPagina * paginador.PaginaAtual;
            var dados = GetListaAgendamentoTreinamento(filtro, false, paginador.InicioPaginacao, ultimaPagina);
            return dados;
        }

        public AgendamentoTreinamentoRetornoView BuscarMotorista(AgendamentoTreinamentoFiltro filtro, bool isEditar = false)
        {
            filtro.CPF = filtro.CPF.RemoveCharacter();
            DateTime dataHoje = DateTime.Now.Date;
            MotoristaFiltro motoFiltro = new MotoristaFiltro
            {
                CPF = filtro.CPF,
                IDEmpresa = filtro.IDEmpresa,
                IDUsuarioCliente = filtro.IDUsuarioCliente,
                IDUsuarioTransportadora = filtro.IDUsuarioTransportadora,
                Operacao = filtro.Operacao
            };

            var motorista = _motoristaBll.ListarMotorista(motoFiltro);

            //verificar se veio algum motorista
            if (motorista != null && motorista.Any())
            {
                //verificar se o motorista está apto
                var motoristaInapto = motorista.Any(w => w.IDStatus == (int)EnumStatusMotorista.EmAprovacao || w.IDStatus == (int)EnumStatusMotorista.Reprovado);


                if (!motoristaInapto)
                {
                    var motoristaApto = motorista.Where(w => w.IDStatus == (int)EnumStatusMotorista.Aprovado || w.IDStatus == (int)EnumStatusMotorista.Bloqueado).FirstOrDefault();

                    if (filtro.IDTipoTreinamento.HasValue)
                    {
                        List<HistoricoTreinamentoTeoricoMotorista> treinamentoTeorico = null;
                        MotoristaTreinamentoTerminal treinamentoPratico = null;
                        string treinamento = null;

                        if (filtro.IDTipoTreinamento.Value == (int)EnumTipoAgenda.TreinamentoTeorico)
                        {
                            //verificar se já tem treinamento teórico naquela data ou superior
                            if (filtro.Data.HasValue)
                            {
                                bool existeTeorico = isEditar ?
                                    false :
                                    Listar(w => w.IDMotorista == motoristaApto.ID && w.Data >= filtro.Data).Any();

                                if (existeTeorico)
                                    return new AgendamentoTreinamentoRetornoView { IdMotorista = 0, NomeMotorista = "", Situacao = "jaExisteAgendamentoTeorico" };
                            }

                            //verificar se já tem treinamento teórico cadastrado 
                            treinamentoTeorico = _histTreinamentoMotBll.Listar(p => p.IDMotorista == motoristaApto.ID && p.Data >= dataHoje && p.TreinamentoAprovado.Value);
                            if (treinamentoTeorico != null && treinamentoTeorico.Any())
                                treinamento = treinamentoTeorico
                                                .OrderByDescending(w => w.Data)
                                                .Select(s => s.Data)
                                                .FirstOrDefault()
                                                .ToString("dd/MM/yyyy");
                        }
                        else
                        {
                            //verificar se já tem treinamento prático naquela data
                            if (filtro.Data.HasValue)
                            {
                                bool existeTeorico = _histTreinamentoMotBll.Listar(w => w.IDMotorista == motoristaApto.ID && w.Data >= filtro.Data).Any();
                                if (!existeTeorico)
                                    return new AgendamentoTreinamentoRetornoView { IdMotorista = 0, NomeMotorista = "", Situacao = "naoExisteTeorico" };
                                var existePratico = isEditar ?
                                    false :
                                    Listar(w => w.IDMotorista == motoristaApto.ID && w.Data == filtro.Data).Any();
                                if (existePratico)
                                    return new AgendamentoTreinamentoRetornoView { IdMotorista = 0, NomeMotorista = "", Situacao = "jaExisteAgendamentoPratico" };
                            }

                            if (filtro.IDTerminal.HasValue)
                            {
                                treinamentoPratico = _motTreinamentoTerminalBll.Selecionar(p =>
                                p.IDMotorista == motoristaApto.ID
                                    && p.DataValidade >= dataHoje
                                    && p.IDTerminal == filtro.IDTerminal.Value);
                                if (treinamentoPratico != null)
                                {
                                    treinamento = treinamentoPratico.DataValidade.Value.ToString("dd/MM/yyyy");
                                }
                            }
                        }
                        if (treinamentoPratico != null || (treinamentoTeorico != null && treinamentoTeorico.Any()))
                        {
                            return new AgendamentoTreinamentoRetornoView
                            {
                                IdMotorista = motoristaApto.ID,
                                NomeMotorista = motoristaApto.Nome,
                                Situacao = "agendado",
                                DataValidadeAgendamento = treinamento
                            };
                        }

                        //se não tiver treinamento válido o motorista está apto
                        return new AgendamentoTreinamentoRetornoView { IdMotorista = motoristaApto.ID, NomeMotorista = motoristaApto.Nome, Situacao = "apto" };
                    }
                    else
                        return new AgendamentoTreinamentoRetornoView { IdMotorista = motoristaApto.ID, NomeMotorista = motoristaApto.Nome, Situacao = "apto" };
                }

                //caso motoristaApto seja null significa que ele está inapto 
                return new AgendamentoTreinamentoRetornoView { IdMotorista = 0, Situacao = "inapto" };

            }
            else
            {
                return new AgendamentoTreinamentoRetornoView { IdMotorista = 0, Situacao = "novo" };
            }
        }

        public int ListarAgendamentoTreinamentoCount(AgendamentoTreinamentoFiltro filtro)
        {

            var dados = GetListaAgendamentoTreinamento(filtro, true);
            return dados[0].Linhas;

        }

        public AgendamentoTreinamentoView SelecionarAgendamentoTreinamento(int id)
        {
            var filtro = new AgendamentoTreinamentoFiltro() { ID = id };
            using (UniCadDalRepositorio<AgendamentoTreinamento> repositorio = new UniCadDalRepositorio<AgendamentoTreinamento>())
            {
                var query = GetListaAgendamentoTreinamento(filtro, false);
                return query.First();
            }

        }


        private List<AgendamentoTreinamentoView> GetListaAgendamentoTreinamento(AgendamentoTreinamentoFiltro filtro, bool isCount, long? paginalInicial = null, long? paginalFinal = null)
        {
            SqlParameter paramIsCount = new SqlParameter("@IsCount", SqlDbType.Bit);
            SqlParameter paramPrimeiraPagina = new SqlParameter("@PrimeiraPagina", SqlDbType.Int);
            SqlParameter paramUltimaPagina = new SqlParameter("@UltimaPagina", SqlDbType.Int);
            SqlParameter paramId = new SqlParameter("@ID", SqlDbType.Int);
            SqlParameter paramIdEmpresa = new SqlParameter("@IDEmpresa", SqlDbType.Int);
            SqlParameter paramOperacao = new SqlParameter("@Operacao", SqlDbType.VarChar);
            SqlParameter paramNome = new SqlParameter("@Nome", SqlDbType.VarChar);
            SqlParameter paramInicio = new SqlParameter("@DataInicio", SqlDbType.DateTime);
            SqlParameter paramFim = new SqlParameter("@DataFim", SqlDbType.DateTime);
            SqlParameter paramCPF = new SqlParameter("@CPF", SqlDbType.VarChar);
            SqlParameter paramIdTerminal = new SqlParameter("@IDTerminal", SqlDbType.Int);
            SqlParameter paramIdUsuarioTransportadora = new SqlParameter("@IDUsuarioTransportadora", SqlDbType.Int);
            SqlParameter paramIdUsuarioCliente = new SqlParameter("@IDUsuarioCliente", SqlDbType.Int);
            SqlParameter paramIdTipoTreinamento = new SqlParameter("@IDTipoTreinamento", SqlDbType.Int);


            paramIsCount.Value = isCount;
            paramPrimeiraPagina.Value = paginalInicial ?? (object)DBNull.Value;
            paramUltimaPagina.Value = paginalFinal ?? (object)DBNull.Value;
            paramIdEmpresa.Value = filtro.IDEmpresa ?? (object)DBNull.Value;
            paramId.Value = filtro.ID ?? (object)DBNull.Value;
            paramOperacao.Value = String.IsNullOrEmpty(filtro.Operacao) ? (object)DBNull.Value : filtro.Operacao;
            paramNome.Value = filtro.Motorista ?? (object)DBNull.Value;
            paramInicio.Value = filtro.DataInicio ?? (object)DBNull.Value;
            paramFim.Value = filtro.DataFim ?? (object)DBNull.Value;
            paramCPF.Value = String.IsNullOrEmpty(filtro.CPF) ? (object)DBNull.Value : filtro.CPF;
            paramIdTerminal.Value = filtro.IDTerminal ?? (object)DBNull.Value;
            paramIdUsuarioTransportadora.Value = filtro.IDUsuarioTransportadora ?? (object)DBNull.Value;
            paramIdUsuarioCliente.Value = filtro.IDUsuarioCliente ?? (object)DBNull.Value;
            paramIdTipoTreinamento.Value = filtro.IDTipoTreinamento ?? (object)DBNull.Value;

            List<AgendamentoTreinamentoView> dadosRelatorio = ExecutarProc(
                "[dbo].[Proc_Pesquisa_Agendamento_Treinamento] @IsCount,@PrimeiraPagina,@UltimaPagina,@ID,@IDEmpresa,@Operacao,@Nome,@DataInicio,@DataFim,@CPF,@IDTerminal,@IDTipoTreinamento,@IDUsuarioTransportadora,@IDUsuarioCliente",
                new Object[] { paramIsCount, paramPrimeiraPagina, paramUltimaPagina, paramId, paramIdEmpresa, paramOperacao, paramNome, paramInicio, paramFim, paramCPF, paramIdTerminal, paramIdTipoTreinamento, paramIdUsuarioTransportadora, paramIdUsuarioCliente });

            return dadosRelatorio;
        }

        public AgendamentoTreinamentoRetornoView VerificarCpfCongenereJaCadastrado(AgendamentoTreinamentoFiltro filtro, bool isEditar)
        {
            if (filtro.IDTipoTreinamento.Value == (int)EnumTipoAgenda.TreinamentoTeorico)
            {
                filtro.CPF = filtro.CPF.Replace(".", "").Replace("-","");
                //verificar se já tem treinamento teórico naquela data ou superior
                bool existeTeorico = isEditar
                    ? false
                    : Listar(w => w.CPFCongenere == filtro.CPF && w.Data >= filtro.Data).Any();

                if (existeTeorico)
                    return new AgendamentoTreinamentoRetornoView
                    { IdMotorista = 0, NomeMotorista = "", Situacao = "jaExisteAgendamentoTeorico" };
            }
            else
            {
                //verificar se já tem treinamento prático naquela data
                var existePratico = isEditar ?
                    false :
                    Listar(w => w.CPFCongenere == filtro.CPF && w.Data == filtro.Data).Any();
                if (existePratico)
                    return new AgendamentoTreinamentoRetornoView { IdMotorista = 0, NomeMotorista = "", Situacao = "jaExisteAgendamentoPratico" };
            }

            return new AgendamentoTreinamentoRetornoView { IdMotorista = 0, Situacao = "novo" };

        }

        public virtual List<AgendamentoTreinamentoView> ExecutarProc(string procedure, object[] parametros)
        {
            using (UniCadDalRepositorio<AgendamentoTreinamentoView> repositorio = new UniCadDalRepositorio<AgendamentoTreinamentoView>())
            {
                return repositorio.ExecutarProcedureComRetorno(procedure, parametros);
            }
        }

        public List<AgendamentoTreinamento> ListarPorAgendamentoTerminal(int id)
        {

            using (UniCadDalRepositorio<AgendamentoTreinamento> repositorio = new UniCadDalRepositorio<AgendamentoTreinamento>())
            {
                IQueryable<AgendamentoTreinamento> query = GetQueryAgendamentoTreinamentoPorAgendamentoTerminal(id, repositorio);
                return query.ToList();
            }

        }

        private IQueryable<AgendamentoTreinamento> GetQueryAgendamentoTreinamentoPorAgendamentoTerminal(int id, UniCadDalRepositorio<AgendamentoTreinamento> repositorio)
        {
            IQueryable<AgendamentoTreinamento> query = (from app in repositorio.ListComplex<AgendamentoTreinamento>().AsNoTracking().OrderByDescending(i => i.ID)
                                                        join horario in repositorio.ListComplex<AgendamentoTerminalHorario>().AsNoTracking() on app.IDAgendamentoTerminalHorario equals horario.ID
                                                        join ag in repositorio.ListComplex<AgendamentoTerminal>().AsNoTracking() on horario.IDAgendamentoTerminal equals ag.ID
                                                        where (ag.ID == id)
                                                        select app);
            return query;
        }

        public AgendamentoTreinamentoView SelecionarAgendamentoTerminalPorId(int id)
        {

            using (UniCadDalRepositorio<AgendamentoTreinamento> repositorio = new UniCadDalRepositorio<AgendamentoTreinamento>())
            {
                IQueryable<AgendamentoTreinamentoView> query = GetQueryAgendamentoTreinamentoPorId(id, repositorio);
                return query.FirstOrDefault();
            }

        }

        private IQueryable<AgendamentoTreinamentoView> GetQueryAgendamentoTreinamentoPorId(int id, UniCadDalRepositorio<AgendamentoTreinamento> repositorio)
        {
            IQueryable<AgendamentoTreinamentoView> query = (from app in repositorio.ListComplex<AgendamentoTreinamento>().AsNoTracking().OrderByDescending(i => i.ID)
                                                            join horario in repositorio.ListComplex<AgendamentoTerminalHorario>().AsNoTracking() on app.IDAgendamentoTerminalHorario equals horario.ID
                                                            join moto in repositorio.ListComplex<Motorista>().AsNoTracking() on app.IDMotorista equals moto.ID into m
                                                            from m1 in m.DefaultIfEmpty()
                                                            join ag in repositorio.ListComplex<AgendamentoTerminal>().AsNoTracking() on horario.IDAgendamentoTerminal equals ag.ID
                                                            join tipoAgenda in repositorio.ListComplex<TipoAgenda>().AsNoTracking() on ag.IDTipoAgenda equals tipoAgenda.ID
                                                            where (app.ID == id)
                                                            select new AgendamentoTreinamentoView
                                                            {
                                                                IDMotorista = app.IDMotorista,
                                                                IDTipoTreinamento = tipoAgenda.IDTipo,
                                                                IdTipo = ag.IDTipoAgenda,
                                                                IDAgendamentoTerminalHorario = app.IDAgendamentoTerminalHorario,
                                                                IDEmpresa = horario.IDEmpresa.Value,
                                                                Operacao = horario.Operacao,
                                                                IDEmpresaMotorista = m1 != null ? m1.IDEmpresa : horario.IDEmpresa.Value,
                                                                OperacaoMotorista = m1 != null ? m1.Operacao : horario.Operacao,
                                                                IDTerminal = ag.IDTerminal,
                                                                Data = ag.Data,
                                                                CPF = m1 != null ? m1.MotoristaBrasil.CPF : app.CPFCongenere,
                                                                HoraInicio = horario.HoraInicio,
                                                                HoraFim = horario.HoraFim,
                                                                Nome = m1 != null ? m1.Nome : app.NomeMotorista,
                                                                IDEmpresaCongenere = app.IDEmpresaCongenere
                                                            });
            return query;
        }

        public Stream Exportar(AgendamentoTreinamentoFiltro filtro)
        {
            var lista = new AgendamentoTreinamentoBusiness().ListarAgendamentoTreinamentoRelatorio(filtro);
            Stream fs = new MemoryStream();
            var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("AgendamentoTreinamento");
            MontarColunasAgendamentoTreinamento(worksheet);

            int linha = 2;
            {
                foreach (var item in lista)
                {
                    MontarLinhasAgendamentoTreinamento(worksheet, linha, item);
                    linha++;
                }
            }

            using (var range = worksheet.Range($"A{2}:I{linha - 1}"))
            {
                DesenharBorda(range);
            }

            workbook.SaveAs(fs, false);
            fs.Position = 0;

            return fs;

        }

        private List<AgendamentoTreinamentoView> ListarAgendamentoTreinamentoRelatorio(AgendamentoTreinamentoFiltro filtro)
        {

            using (UniCadDalRepositorio<AgendamentoTreinamento> repositorio = new UniCadDalRepositorio<AgendamentoTreinamento>())
            {
                return GetListaAgendamentoTreinamento(filtro, false);
            }

        }

        private void MontarColunasAgendamentoTreinamento(IXLWorksheet worksheet)
        {

            worksheet.Cell(1, 1).Value = "Data";
            worksheet.Cell(1, 2).Value = "Horário";
            worksheet.Cell(1, 3).Value = "Linha de Negócio";
            worksheet.Cell(1, 4).Value = "Operação";
            worksheet.Cell(1, 5).Value = "Tipo de Treinamento";
            worksheet.Cell(1, 6).Value = "Nome";
            worksheet.Cell(1, 7).Value = "CPF";
            worksheet.Cell(1, 8).Value = "Terminal";
            worksheet.Cell(1, 9).Value = "Situação";

            using (IXLRange range = worksheet.Range("A1:I1"))
            {
                range.Style.Font.Bold = true;
                range.Style.Font.SetFontColor(XLColor.White);
                range.Style.Fill.PatternType = XLFillPatternValues.Solid;
                range.Style.Fill.SetBackgroundColor(XLColor.FromArgb(150, 26, 141)); //Roxo Raízen.
                range.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                range.SetAutoFilter();

                DesenharBorda(range);
            }
        }

        private void MontarLinhasAgendamentoTreinamento(IXLWorksheet worksheet, int linha, AgendamentoTreinamentoView agendamento)
        {
            worksheet.Cell(linha, 1).Value = agendamento.Data;
            worksheet.Cell(linha, 2).DataType = XLCellValues.Text;
            worksheet.Cell(linha, 2).Value = "'" + agendamento.Horario;
            worksheet.Cell(linha, 3).Value = agendamento.Empresa;
            worksheet.Cell(linha, 4).Value = agendamento.Operacao;
            worksheet.Cell(linha, 5).Value = agendamento.TipoTreinamento;
            worksheet.Cell(linha, 6).Value = agendamento.Motorista;
            worksheet.Cell(linha, 7).Value = agendamento.CPF;
            worksheet.Cell(linha, 8).Value = agendamento.Terminal;
            worksheet.Cell(linha, 9).Value = agendamento.idSituacao.HasValue ? EnumExtensions.GetDescription((EnumSituacaoAgendamento)agendamento.idSituacao) : "";
        }

        public string Validar(AgendamentoTreinamento model, AgendamentoTreinamentoFiltro filtro, bool isEditar = false)
        {
            //validar se pode inscrever nesse horário, se ainda possui a vaga.            
            var lista = _agendamentoHorarioBll.ListarAgendamentoTerminalHorarioPorTerminalPorTipoAgenda(model.IDEmpresa, model.Operacao, model.IDTerminal, model.IDTipo, model.Data);
            var retorno = lista.Any(p => p.ID == model.IDAgendamentoTerminalHorario && p.NumVagas > 0) ? "" :
                "Já não existem mais vagas disponíveis para o horário! Favor selecionar outro horário.";
            if (!string.IsNullOrEmpty(retorno))
                return retorno;

            var retornoMotorista = this.BuscarMotorista(filtro, isEditar);
            if (retornoMotorista.Situacao != "apto" && retornoMotorista.Situacao != "agendado")
                retorno = "Esse motorista não está apto para o agendamento! Favor verificar o cadastro.";
            return retorno;
        }

        private static void DesenharBorda(IXLRange celulas)
        {
            celulas.Style.Border.TopBorder = XLBorderStyleValues.Thin;
            celulas.Style.Border.LeftBorder = XLBorderStyleValues.Thin;
            celulas.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            celulas.Style.Border.RightBorder = XLBorderStyleValues.Thin;
        }

        public MemoryStream GerarPdf(int id)
        {
            #region parametros e configurações
            MemoryStream memoryStream = new MemoryStream { Position = 0 };
            AgendamentoTreinamentoView agendamento = SelecionarAgendamentoTreinamento(id);

            //Cria o documento e informa o caminho/nome que será usado para salvar o arquivo.
            //A4 size - 210mm x 297mm, or 8.26 inches x 11.69 inches            
            Document oDocument = new Document(PageSize.A4, 0, 0, 1, 0);

            var terminal = agendamento.Terminal;
            if (!string.IsNullOrEmpty(agendamento.EnderecoTerminal))
                terminal += " - " + agendamento.EnderecoTerminal;
            if (!string.IsNullOrEmpty(agendamento.CidadeTerminal))
                terminal += " - " + agendamento.CidadeTerminal;
            if (!string.IsNullOrEmpty(agendamento.EstadoTerminal))
                terminal += "/" + agendamento.EstadoTerminal;
            //USAR COMO PAISAGEM
            //oDocument.SetPageSize(PageSize.A4.Rotate());

            //Cria uma instância do PdfWriter e escreve no Response.OutputStream.
            PdfWriter pdfWriter = PdfWriter.GetInstance(oDocument, memoryStream);
            pdfWriter.PageEvent = new PdfFooter();

            #endregion parametros e configurações


            //Abre o documento para iniciar a construção.
            oDocument.Open();
            PdfContentByte cb = pdfWriter.DirectContent;

            #region [ Objetos genéricos e constantes ]
            //Cria uma tabela generica que servirá para transportar os detalhes de todas as areas.
            PdfPTable oPdfPTable;

            //Cria uma celular generica que servirá para transportar as tabelas com detalhes de todas as áreas.
            PdfPCell oPdfPCellMaster;


            //Cria uma cor generica Cinza CSC                
            BaseColor oRoxoBackgroundHeader = new BaseColor(146, 41, 128);
            BaseColor oCinzaSubFonte = new BaseColor(120, 120, 120);
            var oBrancoBackground = new BaseColor(255, 255, 255);
            //BaseColor oBrancoFonteHeader = new BaseColor(255, 255, 240);

            //Fontes que serão utilizadas
            Font oFontForHeader = new Font(Font.FontFamily.HELVETICA, 15, Font.NORMAL, BaseColor.BLACK);
            Font oFontForField = new Font(Font.FontFamily.HELVETICA, 11, Font.NORMAL, BaseColor.BLACK);
            Font oFontForFieldMenor = new Font(Font.FontFamily.HELVETICA, 10, Font.NORMAL, BaseColor.BLACK);
            Font oFontValue = new Font(Font.FontFamily.HELVETICA, 11, Font.BOLD, BaseColor.BLACK);
            Font oFontValueMenor = new Font(Font.FontFamily.HELVETICA, 10, Font.BOLD, BaseColor.BLACK);
            #endregion

            //Cria a tabela que servirá de container para todas as áreas
            PdfPTable oPdfPTableContainer = new PdfPTable(1);
            oPdfPTableContainer.TotalWidth = 580f;
            oPdfPTableContainer.LockedWidth = true;

            #region [ CORPO DOCUMENTO ]
            // Define a quantidade e Largura das Colunas - 40
            oPdfPTable = new PdfPTable(new float[] { 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10 });
            oPdfPTable.TotalWidth = 750f;
            oPdfPTable.SpacingBefore = 0f;
            oPdfPTable.SpacingAfter = 0f;
            oPdfPTable.SplitLate = false;
            oPdfPTable.SplitRows = false;
            oPdfPTable.DefaultCell.Border = Rectangle.BODY;

            PdfPCell opdfCellEsquerdo = null;
            var oPdfEsq = new PdfPTable(new float[] { 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10 });

            opdfCellEsquerdo = new PdfPCell(new Phrase("Agendamento de Treinamento", oFontForHeader)) { Colspan = 40, PaddingLeft = 10 };
            opdfCellEsquerdo.HorizontalAlignment = Element.ALIGN_LEFT;
            opdfCellEsquerdo.VerticalAlignment = Element.ALIGN_LEFT;
            opdfCellEsquerdo.Border = Rectangle.NO_BORDER;
            opdfCellEsquerdo.PaddingBottom = 3f;
            oPdfEsq.AddCell(opdfCellEsquerdo);

            opdfCellEsquerdo = new PdfPCell(new Phrase(" ", oFontForFieldMenor)) { Colspan = 40, PaddingLeft = 10 };
            opdfCellEsquerdo.HorizontalAlignment = Element.ALIGN_LEFT;
            opdfCellEsquerdo.VerticalAlignment = Element.ALIGN_LEFT;
            opdfCellEsquerdo.Border = Rectangle.NO_BORDER;
            opdfCellEsquerdo.PaddingBottom = 3f;
            oPdfEsq.AddCell(opdfCellEsquerdo);

            opdfCellEsquerdo = new PdfPCell(new Phrase("Data/Hora:", oFontForField)) { Colspan = 8, PaddingLeft = 10 };
            opdfCellEsquerdo.HorizontalAlignment = Element.ALIGN_LEFT;
            opdfCellEsquerdo.VerticalAlignment = Element.ALIGN_LEFT;
            opdfCellEsquerdo.Border = Rectangle.NO_BORDER;
            opdfCellEsquerdo.PaddingBottom = 3f;
            oPdfEsq.AddCell(opdfCellEsquerdo);

            opdfCellEsquerdo = new PdfPCell(new Phrase(agendamento.Data.ToString("dd/MM/yyyy-hh:mm") + "h", oFontValue)) { Colspan = 32 };
            opdfCellEsquerdo.HorizontalAlignment = Element.ALIGN_LEFT;
            opdfCellEsquerdo.VerticalAlignment = Element.ALIGN_LEFT;
            opdfCellEsquerdo.Border = Rectangle.NO_BORDER;
            opdfCellEsquerdo.PaddingBottom = 3f;
            oPdfEsq.AddCell(opdfCellEsquerdo);

            opdfCellEsquerdo = new PdfPCell(new Phrase("Terminal:", oFontForField)) { Colspan = 8, PaddingLeft = 10 };
            opdfCellEsquerdo.HorizontalAlignment = Element.ALIGN_LEFT;
            opdfCellEsquerdo.VerticalAlignment = Element.ALIGN_LEFT;
            opdfCellEsquerdo.Border = Rectangle.NO_BORDER;
            opdfCellEsquerdo.PaddingBottom = 3f;
            oPdfEsq.AddCell(opdfCellEsquerdo);

            opdfCellEsquerdo = new PdfPCell(new Phrase(terminal, oFontValue)) { Colspan = 32 };
            opdfCellEsquerdo.HorizontalAlignment = Element.ALIGN_LEFT;
            opdfCellEsquerdo.VerticalAlignment = Element.ALIGN_LEFT;
            opdfCellEsquerdo.Border = Rectangle.NO_BORDER;
            opdfCellEsquerdo.PaddingBottom = 3f;
            oPdfEsq.AddCell(opdfCellEsquerdo);

            opdfCellEsquerdo = new PdfPCell(new Phrase("Motorista:", oFontForFieldMenor)) { Colspan = 8, PaddingLeft = 10 };
            opdfCellEsquerdo.HorizontalAlignment = Element.ALIGN_LEFT;
            opdfCellEsquerdo.VerticalAlignment = Element.ALIGN_LEFT;
            opdfCellEsquerdo.Border = Rectangle.NO_BORDER;
            opdfCellEsquerdo.PaddingBottom = 3f;
            oPdfEsq.AddCell(opdfCellEsquerdo);

            opdfCellEsquerdo = new PdfPCell(new Phrase(agendamento.CPF + " - " + agendamento.Motorista, oFontValueMenor)) { Colspan = 15 };
            opdfCellEsquerdo.HorizontalAlignment = Element.ALIGN_LEFT;
            opdfCellEsquerdo.VerticalAlignment = Element.ALIGN_LEFT;
            opdfCellEsquerdo.Border = Rectangle.NO_BORDER;
            opdfCellEsquerdo.PaddingBottom = 3f;
            oPdfEsq.AddCell(opdfCellEsquerdo);

            opdfCellEsquerdo = new PdfPCell(new Phrase("Treinamento:", oFontForFieldMenor)) { Colspan = 8, PaddingLeft = 10 };
            opdfCellEsquerdo.HorizontalAlignment = Element.ALIGN_LEFT;
            opdfCellEsquerdo.VerticalAlignment = Element.ALIGN_LEFT;
            opdfCellEsquerdo.Border = Rectangle.NO_BORDER;
            opdfCellEsquerdo.PaddingBottom = 3f;
            oPdfEsq.AddCell(opdfCellEsquerdo);

            opdfCellEsquerdo = new PdfPCell(new Phrase(agendamento.TipoTreinamento, oFontValueMenor)) { Colspan = 15 };
            opdfCellEsquerdo.HorizontalAlignment = Element.ALIGN_LEFT;
            opdfCellEsquerdo.VerticalAlignment = Element.ALIGN_LEFT;
            opdfCellEsquerdo.Border = Rectangle.NO_BORDER;
            opdfCellEsquerdo.PaddingBottom = 3f;
            oPdfEsq.AddCell(opdfCellEsquerdo);

            opdfCellEsquerdo = new PdfPCell(new Phrase("Linha de Negócio:", oFontForFieldMenor)) { Colspan = 8, PaddingLeft = 10 };
            opdfCellEsquerdo.HorizontalAlignment = Element.ALIGN_LEFT;
            opdfCellEsquerdo.VerticalAlignment = Element.ALIGN_LEFT;
            opdfCellEsquerdo.Border = Rectangle.NO_BORDER;
            opdfCellEsquerdo.PaddingBottom = 3f;
            oPdfEsq.AddCell(opdfCellEsquerdo);

            opdfCellEsquerdo = new PdfPCell(new Phrase(agendamento.Empresa, oFontValueMenor)) { Colspan = 15 };
            opdfCellEsquerdo.HorizontalAlignment = Element.ALIGN_LEFT;
            opdfCellEsquerdo.VerticalAlignment = Element.ALIGN_LEFT;
            opdfCellEsquerdo.Border = Rectangle.NO_BORDER;
            opdfCellEsquerdo.PaddingBottom = 3f;
            oPdfEsq.AddCell(opdfCellEsquerdo);

            opdfCellEsquerdo = new PdfPCell(new Phrase("Operação:", oFontForFieldMenor)) { Colspan = 8, PaddingLeft = 10 };
            opdfCellEsquerdo.HorizontalAlignment = Element.ALIGN_LEFT;
            opdfCellEsquerdo.VerticalAlignment = Element.ALIGN_LEFT;
            opdfCellEsquerdo.Border = Rectangle.NO_BORDER;
            opdfCellEsquerdo.PaddingBottom = 3f;
            oPdfEsq.AddCell(opdfCellEsquerdo);

            opdfCellEsquerdo = new PdfPCell(new Phrase(agendamento.Operacao, oFontValueMenor)) { Colspan = 15 };
            opdfCellEsquerdo.HorizontalAlignment = Element.ALIGN_LEFT;
            opdfCellEsquerdo.VerticalAlignment = Element.ALIGN_LEFT;
            opdfCellEsquerdo.Border = Rectangle.NO_BORDER;
            opdfCellEsquerdo.PaddingBottom = 3f;
            oPdfEsq.AddCell(opdfCellEsquerdo);

            opdfCellEsquerdo = new PdfPCell(new Phrase("Data Agendamento:", oFontForFieldMenor)) { Colspan = 8, PaddingLeft = 10 };
            opdfCellEsquerdo.HorizontalAlignment = Element.ALIGN_LEFT;
            opdfCellEsquerdo.VerticalAlignment = Element.ALIGN_LEFT;
            opdfCellEsquerdo.Border = Rectangle.NO_BORDER;
            opdfCellEsquerdo.PaddingBottom = 3f;
            oPdfEsq.AddCell(opdfCellEsquerdo);

            opdfCellEsquerdo = new PdfPCell(new Phrase(agendamento.Data.ToShortDateString(), oFontValueMenor)) { Colspan = 15 };
            opdfCellEsquerdo.HorizontalAlignment = Element.ALIGN_LEFT;
            opdfCellEsquerdo.VerticalAlignment = Element.ALIGN_LEFT;
            opdfCellEsquerdo.Border = Rectangle.NO_BORDER;
            opdfCellEsquerdo.PaddingBottom = 3f;
            oPdfEsq.AddCell(opdfCellEsquerdo);

            opdfCellEsquerdo = new PdfPCell(new Phrase("Agendado por:", oFontForFieldMenor)) { Colspan = 8, PaddingLeft = 10 };
            opdfCellEsquerdo.HorizontalAlignment = Element.ALIGN_LEFT;
            opdfCellEsquerdo.VerticalAlignment = Element.ALIGN_LEFT;
            opdfCellEsquerdo.Border = Rectangle.NO_BORDER;
            opdfCellEsquerdo.PaddingBottom = 3f;
            oPdfEsq.AddCell(opdfCellEsquerdo);

            opdfCellEsquerdo = new PdfPCell(new Phrase(agendamento.Usuario + " ", oFontValueMenor)) { Colspan = 15 };
            opdfCellEsquerdo.HorizontalAlignment = Element.ALIGN_LEFT;
            opdfCellEsquerdo.VerticalAlignment = Element.ALIGN_LEFT;
            opdfCellEsquerdo.Border = Rectangle.NO_BORDER;
            opdfCellEsquerdo.PaddingBottom = 3f;
            oPdfEsq.AddCell(opdfCellEsquerdo);

            opdfCellEsquerdo = new PdfPCell(new Phrase(" ", oFontForFieldMenor)) { Colspan = 40, PaddingLeft = 10 };
            opdfCellEsquerdo.HorizontalAlignment = Element.ALIGN_LEFT;
            opdfCellEsquerdo.VerticalAlignment = Element.ALIGN_LEFT;
            opdfCellEsquerdo.Border = Rectangle.NO_BORDER;
            opdfCellEsquerdo.PaddingBottom = 3f;
            oPdfEsq.AddCell(opdfCellEsquerdo);

            opdfCellEsquerdo = new PdfPCell(new Phrase("Caro Motorista,", oFontForFieldMenor)) { Colspan = 40, PaddingLeft = 10 };
            opdfCellEsquerdo.HorizontalAlignment = Element.ALIGN_LEFT;
            opdfCellEsquerdo.VerticalAlignment = Element.ALIGN_LEFT;
            opdfCellEsquerdo.Border = Rectangle.NO_BORDER;
            opdfCellEsquerdo.PaddingBottom = 3f;
            oPdfEsq.AddCell(opdfCellEsquerdo);

            opdfCellEsquerdo = new PdfPCell(new Phrase("Atente-se ao horário de agendamento! Confira o endereço do Terminal onde será realizado o Treinamento e chegue com antecedência.", oFontForFieldMenor)) { Colspan = 40, PaddingLeft = 10 };
            opdfCellEsquerdo.HorizontalAlignment = Element.ALIGN_LEFT;
            opdfCellEsquerdo.VerticalAlignment = Element.ALIGN_LEFT;
            opdfCellEsquerdo.Border = Rectangle.NO_BORDER;
            opdfCellEsquerdo.PaddingBottom = 3f;
            oPdfEsq.AddCell(opdfCellEsquerdo);

            opdfCellEsquerdo = new PdfPCell(new Phrase(" ", oFontForFieldMenor)) { Colspan = 40, PaddingLeft = 10 };
            opdfCellEsquerdo.HorizontalAlignment = Element.ALIGN_LEFT;
            opdfCellEsquerdo.VerticalAlignment = Element.ALIGN_LEFT;
            opdfCellEsquerdo.Border = Rectangle.NO_BORDER;
            opdfCellEsquerdo.PaddingBottom = 3f;
            oPdfEsq.AddCell(opdfCellEsquerdo);

            opdfCellEsquerdo = new PdfPCell(new Phrase("Documento emitido pela Raízen através do sistema de cadastro - UNICAD", oFontForFieldMenor)) { Colspan = 40, PaddingLeft = 10 };
            opdfCellEsquerdo.HorizontalAlignment = Element.ALIGN_LEFT;
            opdfCellEsquerdo.VerticalAlignment = Element.ALIGN_LEFT;
            opdfCellEsquerdo.Border = Rectangle.NO_BORDER;
            opdfCellEsquerdo.PaddingBottom = 3f;
            oPdfEsq.AddCell(opdfCellEsquerdo);

            oPdfPCellMaster = new PdfPCell(oPdfEsq) { Colspan = 40 };
            oPdfPCellMaster.HorizontalAlignment = Element.ALIGN_CENTER;
            oPdfPCellMaster.VerticalAlignment = Element.ALIGN_CENTER;
            oPdfPCellMaster.Border = Rectangle.NO_BORDER;
            oPdfPCellMaster.BackgroundColor = oBrancoBackground;
            oPdfPCellMaster.PaddingBottom = 3f;
            oPdfPTable.AddCell(oPdfPCellMaster);

            //Cria a celula master que irá abrigar a tabela formulário.
            oPdfPCellMaster = new PdfPCell(oPdfPTable);
            oPdfPCellMaster.Padding = 0;
            oPdfPCellMaster.Border = Rectangle.NO_BORDER;


            //Adiciona a celula master ao container principal.
            oPdfPTableContainer.AddCell(oPdfPCellMaster);

            //Adiciona a tabela ao documento
            oDocument.Add(oPdfPTableContainer);

            //Add border to page
            PdfContentByte content = pdfWriter.DirectContent;

            PdfGState gs1 = new PdfGState();
            gs1.FillOpacity = 0f;
            content.SetGState(gs1);


            //Rectangle rectangle = new Rectangle(oDocument.PageSize.Width - 10, 840, 10, 640);
            //rectangle.Left += oDocument.LeftMargin;
            //rectangle.Right -= oDocument.RightMargin;
            //rectangle.Top -= oDocument.TopMargin;
            //rectangle.Bottom += oDocument.BottomMargin;
            //rectangle.Border = Rectangle.RECTANGLE;
            ////content.SetColorFill(oRoxoBackgroundHeader);
            //content.SetColorStroke(BaseColor.BLACK);
            //content.Rectangle(rectangle.Left, rectangle.Bottom, rectangle.Width, rectangle.Height);
            //content.FillStroke();

            #endregion
            oDocument.Close();
            return memoryStream;

        }
    }
}

