using System;
using System.Collections.Generic;
using System.Linq;
using Raizen.Framework.Log.Bases;
using Raizen.Framework.Models;
using Raizen.UniCad.DAL;
using Raizen.UniCad.Model;
using Raizen.UniCad.Model.Filtro;
using Raizen.UniCad.Model.View;
using System.Data.Entity;
using System.IO;
using ClosedXML.Excel;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Raizen.UniCad.BLL.Util;
using System.Data.SqlClient;
using System.Data;

namespace Raizen.UniCad.BLL
{
    public class AgendamentoChecklistBusiness : UniCadBusinessBase<AgendamentoChecklist>
    {

        public int ListarAgendamentoChecklistCount(AgendamentoChecklistFiltro filtro)
        {
            var dados = GetAgendamentoChecklist(filtro, true);
            return dados[0].Linhas;
        }

        public List<AgendamentoChecklistView> ListarAgendamentoChecklist(AgendamentoChecklistFiltro filtro, PaginadorModel paginador)
        {
            int? ultimaPagina = null;
            if (paginador.QtdeItensPagina > 0)
            {
                ultimaPagina = paginador.QtdeItensPagina * paginador.PaginaAtual;
            }
            var dados = GetAgendamentoChecklist(filtro, false, paginador.InicioPaginacao, ultimaPagina);
            return dados;
        }

        private List<AgendamentoChecklistView> GetAgendamentoChecklist(AgendamentoChecklistFiltro filtro, bool isCount, long? paginalInicial = null, long? paginalFinal = null)
        {
            SqlParameter paramIsCount = new SqlParameter("@IsCount", SqlDbType.Bit);
            SqlParameter paramPrimeiraPagina = new SqlParameter("@PrimeiraPagina", SqlDbType.Int);
            SqlParameter paramUltimaPagina = new SqlParameter("@UltimaPagina", SqlDbType.Int);
            SqlParameter paramIdEmpresa = new SqlParameter("@IDEmpresa", SqlDbType.Int);
            SqlParameter paramIdStatus = new SqlParameter("@IDStatus", SqlDbType.Int);
            SqlParameter paramOperacao = new SqlParameter("@Operacao", SqlDbType.VarChar);
            SqlParameter paramIdTipoComposicao = new SqlParameter("@IDTipoComposicao", SqlDbType.Int);
            SqlParameter paramInicio = new SqlParameter("@DataInicio", SqlDbType.DateTime);
            SqlParameter paramFim = new SqlParameter("@DataFim", SqlDbType.DateTime);
            SqlParameter paramPlaca = new SqlParameter("@Placa", SqlDbType.VarChar);
            SqlParameter paramIdTransportadora = new SqlParameter("@IDTransportadora", SqlDbType.Int);
            SqlParameter paramIdUsuarioTransportadora = new SqlParameter("@IDUsuarioTransportadora", SqlDbType.Int);
            SqlParameter paramIdCliente = new SqlParameter("@IDCliente", SqlDbType.Int);
            SqlParameter paramIdUsuarioCliente = new SqlParameter("@IDUsuarioCliente", SqlDbType.Int);
            SqlParameter paramIdTerminal = new SqlParameter("@IDTerminal", SqlDbType.Int);

            paramIsCount.Value = isCount;
            paramPrimeiraPagina.Value = paginalInicial ?? (object)DBNull.Value;
            paramUltimaPagina.Value = paginalFinal ?? (object)DBNull.Value;
            paramIdEmpresa.Value = filtro.IDEmpresa ?? (object)DBNull.Value;
            paramIdStatus.Value = filtro.IDStatus ?? (object)DBNull.Value;
            paramOperacao.Value = String.IsNullOrEmpty(filtro.Operacao) ? (object)DBNull.Value : filtro.Operacao;
            paramIdTipoComposicao.Value = filtro.IDTipoComposicao ?? (object)DBNull.Value;
            paramInicio.Value = filtro.DataInicio ?? (object)DBNull.Value;
            paramFim.Value = filtro.DataFim ?? (object)DBNull.Value;
            paramPlaca.Value = String.IsNullOrEmpty(filtro.Placa) ? (object)DBNull.Value : filtro.Placa;
            paramIdTransportadora.Value = filtro.IDTransportadora ?? (object)DBNull.Value;
            paramIdUsuarioTransportadora.Value = filtro.IDUsuarioTransportadora ?? (object)DBNull.Value;
            paramIdCliente.Value = filtro.IDCliente ?? (object)DBNull.Value;
            paramIdUsuarioCliente.Value = filtro.IDUsuarioCliente ?? (object)DBNull.Value;
            paramIdTerminal.Value = filtro.IDTerminal ?? (object)DBNull.Value;

            List<AgendamentoChecklistView> dadosRelatorio = ExecutarProc(
                "[dbo].[Proc_Pesquisa_Agendamento_Checklist] @IsCount,@PrimeiraPagina,@UltimaPagina,@IDEmpresa,@IDStatus,@Operacao,@IDTipoComposicao,@DataInicio,@DataFim,@Placa,@IDTransportadora,@IDUsuarioTransportadora,@IDCliente,@IDUsuarioCliente,@IDTerminal",
                new Object[] { paramIsCount, paramPrimeiraPagina, paramUltimaPagina, paramIdEmpresa, paramIdStatus, paramOperacao, paramIdTipoComposicao, paramInicio, paramFim, paramPlaca, paramIdTransportadora, paramIdUsuarioTransportadora, paramIdCliente, paramIdUsuarioCliente, paramIdTerminal });

            return dadosRelatorio;
        }

        public virtual List<AgendamentoChecklistView> ExecutarProc(string procedure, object[] parametros)
        {
            using (UniCadDalRepositorio<AgendamentoChecklistView> repositorio = new UniCadDalRepositorio<AgendamentoChecklistView>())
            {
                return repositorio.ExecutarProcedureComRetorno(procedure, parametros);
            }
        }

        public AgendamentoChecklistView SelecionarAgendamentoChecklist(int id)
        {
            var filtro = new AgendamentoChecklistFiltro() { ID = id };
            using (UniCadDalRepositorio<AgendamentoChecklist> repositorio = new UniCadDalRepositorio<AgendamentoChecklist>())
            {
                IQueryable<AgendamentoChecklistView> query = GetQueryAgendamentoChecklist(filtro, repositorio);
                return query.First();
            }

        }

        private IQueryable<AgendamentoChecklistView> GetQueryAgendamentoChecklist(AgendamentoChecklistFiltro filtro, IUniCadDalRepositorio<AgendamentoChecklist> repositorio)
        {
            IQueryable<AgendamentoChecklistView> query = (from app in repositorio.ListComplex<AgendamentoChecklist>().AsNoTracking().OrderByDescending(i => i.ID)
                                                          join horario in repositorio.ListComplex<AgendamentoTerminalHorario>().AsNoTracking() on app.IDAgendamentoTerminalHorario equals horario.ID
                                                          join ag in repositorio.ListComplex<AgendamentoTerminal>().AsNoTracking() on horario.IDAgendamentoTerminal equals ag.ID
                                                          join emp in repositorio.ListComplex<Empresa>().AsNoTracking() on app.IDEmpresa equals emp.ID into e
                                                          from empresa in e.DefaultIfEmpty()
                                                          join comp in repositorio.ListComplex<Composicao>().AsNoTracking() on app.IDComposicao equals comp.ID into c
                                                          from composicao in c.DefaultIfEmpty()
                                                          join tipo in repositorio.ListComplex<TipoComposicao>().AsNoTracking() on composicao.IDTipoComposicao equals tipo.ID into t
                                                          from tp in t.DefaultIfEmpty()
                                                          join placa1 in repositorio.ListComplex<Placa>().AsNoTracking() on composicao.IDPlaca1 equals placa1.ID into g
                                                          from placa01 in g.DefaultIfEmpty()
                                                          join placa2 in repositorio.ListComplex<Placa>().AsNoTracking() on composicao.IDPlaca2 equals placa2.ID into h
                                                          from placa02 in h.DefaultIfEmpty()
                                                          join placa3 in repositorio.ListComplex<Placa>().AsNoTracking() on composicao.IDPlaca3 equals placa3.ID into i
                                                          from placa03 in i.DefaultIfEmpty()
                                                          join placa4 in repositorio.ListComplex<Placa>().AsNoTracking() on composicao.IDPlaca4 equals placa4.ID into j
                                                          from placa04 in j.DefaultIfEmpty()
                                                          join terminal in repositorio.ListComplex<Terminal>().AsNoTracking() on app.IDTerminal equals terminal.ID
                                                          join estado in repositorio.ListComplex<Estado>().AsNoTracking() on terminal.IDEstado equals estado.ID into k
                                                          from estado1 in k.DefaultIfEmpty()
                                                          where (app.Operacao == filtro.Operacao || string.IsNullOrEmpty(filtro.Operacao))
                                                          && (app.IDEmpresa == filtro.IDEmpresa || !filtro.IDEmpresa.HasValue)
                                                          && (app.IDTerminal == filtro.IDTerminal || !filtro.IDTerminal.HasValue)
                                                          && (composicao.IDTipoComposicao == filtro.IDTipoComposicao || !filtro.IDTipoComposicao.HasValue)
                                                          && (DbFunctions.TruncateTime(app.Data) >= filtro.DataInicio || !filtro.DataInicio.HasValue)
                                                          && (DbFunctions.TruncateTime(app.Data) >= filtro.DataFim || !filtro.DataFim.HasValue)
                                                          && (app.ID == filtro.ID || !filtro.ID.HasValue)
                                                          select new AgendamentoChecklistView
                                                          {
                                                              ID = app.ID,
                                                              Data = app.Data,
                                                              DataAgendamento = ag.Data,
                                                              SiglaTerminal = terminal.Sigla,
                                                              Terminal = terminal.Nome,
                                                              EnderecoTerminal = terminal.Endereco,
                                                              CidadeTerminal = terminal.Cidade,
                                                              EstadoTerminal = estado1.Nome,
                                                              Horario = horario.HoraInicio + ":" + horario.HoraFim,
                                                              Empresa = empresa.Nome,
                                                              Operacao = app.Operacao == "CON" ? "Congênere" :  app.Operacao,
                                                              TipoComposicao = tp.Nome,
                                                              Placa1 = !String.IsNullOrEmpty(app.PlacaCongenere) ? app.PlacaCongenere :  placa01.PlacaVeiculo,
                                                              Placa2 = placa02.PlacaVeiculo,
                                                              Placa3 = placa03.PlacaVeiculo,
                                                              Placa4 = placa04.PlacaVeiculo,
                                                              Usuario = app.Usuario
                                                          });
            return query;
        }

        private AgendamentoChecklistView GetQueryAgendamentoChecklistPorComposicao(AgendamentoChecklistFiltro filtro, IUniCadDalRepositorio<AgendamentoChecklist> repositorio)
        {
            IQueryable<AgendamentoChecklistView> query = (from app in repositorio.ListComplex<AgendamentoChecklist>().AsNoTracking().OrderByDescending(i => i.ID)
                                                          join horario in repositorio.ListComplex<AgendamentoTerminalHorario>().AsNoTracking() on app.IDAgendamentoTerminalHorario equals horario.ID
                                                          join ag in repositorio.ListComplex<AgendamentoTerminal>().AsNoTracking() on horario.IDAgendamentoTerminal equals ag.ID
                                                          join comp in repositorio.ListComplex<Composicao>().AsNoTracking() on app.IDComposicao equals comp.ID
                                                          where (app.IDComposicao == filtro.IDComposicao || !filtro.IDComposicao.HasValue)
                                                          && ag.Data >= filtro.DataInicio
                                                          select new AgendamentoChecklistView
                                                          {
                                                              ID = app.ID,
                                                              Data = app.Data,
                                                              DataAgendamento = ag.Data,
                                                              Horario = horario.HoraInicio + ":" + horario.HoraFim,
                                                              Operacao = app.Operacao,
                                                              Usuario = app.Usuario
                                                          });
            return query.FirstOrDefault();
        }

        public List<AgendamentoChecklist> ListarPorAgendamentoTerminal(int id)
        {

            using (UniCadDalRepositorio<AgendamentoChecklist> repositorio = new UniCadDalRepositorio<AgendamentoChecklist>())
            {
                IQueryable<AgendamentoChecklist> query = GetQueryAgendamentoChecklistPorAgendamentoTerminal(id, repositorio);
                return query.ToList();
            }
        }

        private IQueryable<AgendamentoChecklist> GetQueryAgendamentoChecklistPorAgendamentoTerminal(int id, UniCadDalRepositorio<AgendamentoChecklist> repositorio)
        {
            IQueryable<AgendamentoChecklist> query = (from app in repositorio.ListComplex<AgendamentoChecklist>().AsNoTracking().OrderByDescending(i => i.ID)
                                                      join horario in repositorio.ListComplex<AgendamentoTerminalHorario>().AsNoTracking() on app.IDAgendamentoTerminalHorario equals horario.ID
                                                      join ag in repositorio.ListComplex<AgendamentoTerminal>().AsNoTracking() on horario.IDAgendamentoTerminal equals ag.ID
                                                      where (ag.ID == id)
                                                      select app);
            return query;
        }

        public Stream Exportar(AgendamentoChecklistFiltro filtro)
        {
            var lista = new AgendamentoChecklistBusiness().ListarAgendamentoChecklist(filtro, new PaginadorModel() {  });
            Stream fs = new MemoryStream();
            var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("AgendamentoChecklist");
            MontarColunasAgendamentoChecklist(worksheet);

            int linha = 2;
            {
                foreach (var item in lista)
                {
                    MontarLinhasAgendamentoChecklist(worksheet, linha, item);
                    linha++;
                }
            }

            using (var range = worksheet.Range($"A{2}:T{linha - 1}"))
            {
                DesenharBorda(range);
            }

            workbook.SaveAs(fs, false);
            fs.Position = 0;

            return fs;

        }

        private List<AgendamentoChecklistView> ListarAgendamentoChecklistRelatorio(AgendamentoChecklistFiltro filtro)
        {

            using (UniCadDalRepositorio<AgendamentoChecklist> repositorio = new UniCadDalRepositorio<AgendamentoChecklist>())
            {
                IQueryable<AgendamentoChecklistView> query = GetQueryAgendamentoChecklist(filtro, repositorio);
                return query.ToList();
            }

        }

        private void MontarColunasAgendamentoChecklist(IXLWorksheet worksheet)
        {

            worksheet.Cell(1, 1).Value = "Data";
            worksheet.Cell(1, 2).Value = "Horário";
            worksheet.Cell(1, 3).Value = "Linha de Negócio";
            worksheet.Cell(1, 4).Value = "Operação";
            worksheet.Cell(1, 5).Value = "Tipo de Composição";
            worksheet.Cell(1, 6).Value = "Placa 1";
            worksheet.Cell(1, 7).Value = "Placa 2";
            worksheet.Cell(1, 8).Value = "Placa 3";
            worksheet.Cell(1, 9).Value = "Placa 4";
            worksheet.Cell(1, 10).Value = "Terminal";

            using (IXLRange range = worksheet.Range("A1:J1"))
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

        private void MontarLinhasAgendamentoChecklist(IXLWorksheet worksheet, int linha, AgendamentoChecklistView agendamento)
        {
            worksheet.Cell(linha, 1).Value = agendamento.Data;
            worksheet.Cell(linha, 2).Value = "'" + agendamento.Horario;
            worksheet.Cell(linha, 3).Value = agendamento.Empresa;
            worksheet.Cell(linha, 4).Value = agendamento.Operacao;
            worksheet.Cell(linha, 5).Value = agendamento.TipoComposicao;
            worksheet.Cell(linha, 6).Value = agendamento.Placa1;
            worksheet.Cell(linha, 7).Value = agendamento.Placa2;
            worksheet.Cell(linha, 8).Value = agendamento.Placa3;
            worksheet.Cell(linha, 9).Value = agendamento.Placa4;
            worksheet.Cell(linha, 10).Value = agendamento.Terminal;
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
            AgendamentoChecklistView agendamento = SelecionarAgendamentoChecklist(id);

            //Cria o documento e informa o caminho/nome que será usado para salvar o arquivo.
            //A4 size - 210mm x 297mm, or 8.26 inches x 11.69 inches            
            Document oDocument = new Document(PageSize.A4, 0, 0, 1, 0);

            var Placas = "";
            Placas += agendamento.Placa1;
            if (!string.IsNullOrEmpty(agendamento.Placa1))
                Placas += "/ " + agendamento.Placa2;
            if (!string.IsNullOrEmpty(agendamento.Placa3))
                Placas += "/ " + agendamento.Placa3;
            if (!string.IsNullOrEmpty(agendamento.Placa4))
                Placas += "/ " + agendamento.Placa4;

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

            opdfCellEsquerdo = new PdfPCell(new Phrase("Agendamento de Checklist", oFontForHeader)) { Colspan = 40, PaddingLeft = 10 };
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

            opdfCellEsquerdo = new PdfPCell(new Phrase("Composição:", oFontForFieldMenor)) { Colspan = 8, PaddingLeft = 10 };
            opdfCellEsquerdo.HorizontalAlignment = Element.ALIGN_LEFT;
            opdfCellEsquerdo.VerticalAlignment = Element.ALIGN_LEFT;
            opdfCellEsquerdo.Border = Rectangle.NO_BORDER;
            opdfCellEsquerdo.PaddingBottom = 3f;
            oPdfEsq.AddCell(opdfCellEsquerdo);

            opdfCellEsquerdo = new PdfPCell(new Phrase(Placas, oFontValueMenor)) { Colspan = 15 };
            opdfCellEsquerdo.HorizontalAlignment = Element.ALIGN_LEFT;
            opdfCellEsquerdo.VerticalAlignment = Element.ALIGN_LEFT;
            opdfCellEsquerdo.Border = Rectangle.NO_BORDER;
            opdfCellEsquerdo.PaddingBottom = 3f;
            oPdfEsq.AddCell(opdfCellEsquerdo);

            opdfCellEsquerdo = new PdfPCell(new Phrase("Tipo de Composição:", oFontForFieldMenor)) { Colspan = 8, PaddingLeft = 10 };
            opdfCellEsquerdo.HorizontalAlignment = Element.ALIGN_LEFT;
            opdfCellEsquerdo.VerticalAlignment = Element.ALIGN_LEFT;
            opdfCellEsquerdo.Border = Rectangle.NO_BORDER;
            opdfCellEsquerdo.PaddingBottom = 3f;
            oPdfEsq.AddCell(opdfCellEsquerdo);

            opdfCellEsquerdo = new PdfPCell(new Phrase(agendamento.TipoComposicao, oFontValueMenor)) { Colspan = 15 };
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

            opdfCellEsquerdo = new PdfPCell(new Phrase(agendamento.Usuario, oFontValueMenor)) { Colspan = 15 };
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

            opdfCellEsquerdo = new PdfPCell(new Phrase("Atente-se ao horário de agendamento! Confira o endereço do Terminal onde será realizado o Checklist e chegue com antecedência.", oFontForFieldMenor)) { Colspan = 40, PaddingLeft = 10 };
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

        public string Validar(AgendamentoChecklist model)
        {
            var msg = string.Empty;
            //validar se pode inscrever nesse horário, se ainda possui a vaga.
            var bll = new AgendamentoTerminalHorarioBusiness();

            var lista = bll.ListarAgendamentoTerminalHorarioPorTerminal(model.IDEmpresa, model.Operacao, model.IDTerminal, model.Data);
            if (lista.Any(p => p.ID == model.IDAgendamentoTerminalHorario && p.NumVagas > 0))
                msg = null;
            else
                msg = "Já não existem mais vagas disponíveis para o horário! Favor selecionar outro.";
            return msg;
        }

        public DateTime? SelecionarAgendamentoChecklistPorComposicao(int id)
        {
            var filtro = new AgendamentoChecklistFiltro() { IDComposicao = id, DataInicio = DateTime.Now.Date };
            using (UniCadDalRepositorio<AgendamentoChecklist> repositorio = new UniCadDalRepositorio<AgendamentoChecklist>())
            {
                var ag = GetQueryAgendamentoChecklistPorComposicao(filtro, repositorio);
                if (ag != null)
                    return ag.DataAgendamento;
                else
                    return null;
            }
        }
    }
}

