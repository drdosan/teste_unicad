using System;
using System.Collections.Generic;
using System.Linq;
using Raizen.Framework.Log.Bases;
using Raizen.Framework.Models;
using Raizen.UniCad.DAL;
using Raizen.UniCad.Model;
using Raizen.UniCad.Model.Filtro;
using Raizen.UniCad.Model.View;
using System.Transactions;
using Raizen.Framework.Utils.Transacao;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using IsolationLevel = System.Transactions.IsolationLevel;
using System.IO;
using iTextSharp.text.pdf;
using iTextSharp.text;
using Raizen.UniCad.BLL.Util;
using Raizen.Framework.Utils.Extensions;
using System.Data.Entity;

namespace Raizen.UniCad.BLL
{
    public class AgendamentoTerminalBusiness : UniCadBusinessBase<AgendamentoTerminal>
    {
        private readonly AgendamentoTerminalHorarioBusiness _agendamentoTerminalHorarioBLL = new AgendamentoTerminalHorarioBusiness();
        private readonly AgendamentoChecklistBusiness _agendamentoCheckListBLL = new AgendamentoChecklistBusiness();
        private readonly AgendamentoTreinamentoBusiness _agendamentoTreinamentoBLL = new AgendamentoTreinamentoBusiness();
        private readonly TerminalBusiness _terminalBLL = new TerminalBusiness();
        private readonly EstadoBusiness _estadoBLL = new EstadoBusiness();
        private readonly TipoAgendaBusiness _tipoAgendaBLL = new TipoAgendaBusiness();
        public List<AgendamentoTerminalView> ListarAgendamentoTerminal(AgendamentoTerminalFiltro filtro, PaginadorModel paginador)
        {

            using (UniCadDalRepositorio<AgendamentoTerminal> repositorio = new UniCadDalRepositorio<AgendamentoTerminal>())
            {
                IQueryable<AgendamentoTerminalView> query = GetQueryAgendamentoTerminal(filtro, repositorio)
                                                        .Take(paginador.QtdeItensPagina * paginador.PaginaAtual)
                                                        .OrderBy(i => i.Data)
                                                        .Skip(unchecked((int)paginador.InicioPaginacao));
                return query.ToList();
            }

        }

        public int ListarTerminalHorario(AgendamentoTerminalFiltro filtroAgendamentoTerminal)
        {

            using (UniCadDalRepositorio<AgendamentoTerminal> repositorio = new UniCadDalRepositorio<AgendamentoTerminal>())
            {
                IQueryable<AgendamentoTerminalView> query = GetQueryListarAgendamentoTerminalHorario(filtroAgendamentoTerminal, repositorio);
                return query.Count();
            }

        }

        public bool verificarSeJaExisteHorario(AgendamentoTerminalFiltro filtroAgendamentoTerminal)
        {

            using (UniCadDalRepositorio<AgendamentoTerminal> repositorio = new UniCadDalRepositorio<AgendamentoTerminal>())
            {
                IQueryable<AgendamentoTerminalView> query = GetQueryVerificarSeJaExisteHorario(filtroAgendamentoTerminal, repositorio);
                return query.Count() > 0;
            }

        }

        public MemoryStream GerarPdf(DateTime data, int idTerminal, int idTipoAgenda, int vagas, int vagasDisponiveis)
        {
            #region parametros e configurações
            MemoryStream memoryStream = new MemoryStream { Position = 0 };
            //AgendamentoChecklistView agendamento = SelecionarAgendamentoChecklist(id);

            //Cria o documento e informa o caminho/nome que será usado para salvar o arquivo.
            //A4 size - 210mm x 297mm, or 8.26 inches x 11.69 inches            
            Document oDocument = new Document(PageSize.A4, 0, 0, 1, 0);


            //USAR COMO PAISAGEM
            //oDocument.SetPageSize(PageSize.A4.Rotate());

            //Cria uma instância do PdfWriter e escreve no Response.OutputStream.
            PdfWriter pdfWriter = PdfWriter.GetInstance(oDocument, memoryStream);
            pdfWriter.PageEvent = new PdfFooter();

            #endregion parametros e configurações

            #region objetos que serão usados
            var terminal = _terminalBLL.Selecionar(idTerminal);
            var estado = _estadoBLL.Selecionar(terminal.IDEstado ?? 0);
            var tipoAgenda = _tipoAgendaBLL.Selecionar(idTipoAgenda);
            var tipoDeAgenda = EnumExtensions.GetDescription((EnumTipoAgenda)tipoAgenda.IDTipo);
            var listaInscritos = ListarInscritos(idTerminal, idTipoAgenda, data, tipoAgenda.IDTipo);
            #endregion

            //Abre o documento para iniciar a construção.
            oDocument.Open();
            PdfContentByte cb = pdfWriter.DirectContent;

            #region [ Objetos genéricos e constantes ]
            //Cria uma tabela generica que servirá para transportar os detalhes de todas as areas.
            PdfPTable oPdfPTable;

            //Cria uma celular generica que servirá para transportar as tabelas com detalhes de todas as áreas.
            PdfPCell oPdfPCellMaster;


            //Cria uma cor generica ROXO Raízen                
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

            PdfPCell opfCell = null;
            var oPdf = new PdfPTable(new float[] { 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10 });

            opfCell = new PdfPCell(new Phrase("Controle de Agendamentos", oFontForHeader)) { Colspan = 40 };
            opfCell.HorizontalAlignment = Element.ALIGN_LEFT;
            opfCell.VerticalAlignment = Element.ALIGN_LEFT;
            opfCell.Border = Rectangle.NO_BORDER;
            opfCell.PaddingBottom = 3f;
            oPdf.AddCell(opfCell);

            opfCell = new PdfPCell(new Phrase(" ", oFontForFieldMenor)) { Colspan = 40, PaddingLeft = 10 };
            opfCell.HorizontalAlignment = Element.ALIGN_LEFT;
            opfCell.VerticalAlignment = Element.ALIGN_LEFT;
            opfCell.Border = Rectangle.NO_BORDER;
            opfCell.PaddingBottom = 3f;
            oPdf.AddCell(opfCell);

            opfCell = new PdfPCell(new Phrase("Data:", oFontForField)) { Colspan = 3, PaddingLeft = 8 };
            opfCell.HorizontalAlignment = Element.ALIGN_LEFT;
            opfCell.VerticalAlignment = Element.ALIGN_LEFT;
            opfCell.Border = Rectangle.NO_BORDER;
            opfCell.PaddingBottom = 3f;
            oPdf.AddCell(opfCell);

            opfCell = new PdfPCell(new Phrase(data.ToString("dd/MM/yyyy"), oFontValue)) { Colspan = 17 };
            opfCell.HorizontalAlignment = Element.ALIGN_LEFT;
            opfCell.VerticalAlignment = Element.ALIGN_LEFT;
            opfCell.Border = Rectangle.NO_BORDER;
            opfCell.PaddingBottom = 3f;
            oPdf.AddCell(opfCell);

            opfCell = new PdfPCell(new Phrase("Agenda:", oFontForField)) { Colspan = 4, PaddingLeft = 8 };
            opfCell.HorizontalAlignment = Element.ALIGN_LEFT;
            opfCell.VerticalAlignment = Element.ALIGN_LEFT;
            opfCell.Border = Rectangle.NO_BORDER;
            opfCell.PaddingBottom = 3f;
            oPdf.AddCell(opfCell);

            opfCell = new PdfPCell(new Phrase(tipoAgenda.Nome, oFontValue)) { Colspan = 16 };
            opfCell.HorizontalAlignment = Element.ALIGN_LEFT;
            opfCell.VerticalAlignment = Element.ALIGN_LEFT;
            opfCell.Border = Rectangle.NO_BORDER;
            opfCell.PaddingBottom = 3f;
            oPdf.AddCell(opfCell);

            opfCell = new PdfPCell(new Phrase("Terminal:", oFontForField)) { Colspan = 4, PaddingLeft = 8 };
            opfCell.HorizontalAlignment = Element.ALIGN_LEFT;
            opfCell.VerticalAlignment = Element.ALIGN_LEFT;
            opfCell.Border = Rectangle.NO_BORDER;
            opfCell.PaddingBottom = 3f;
            oPdf.AddCell(opfCell);

            opfCell = new PdfPCell(new Phrase($"{terminal.Nome} - {terminal.Endereco} - {terminal.Cidade}/{estado?.Nome}", oFontValue)) { Colspan = 36 };
            opfCell.HorizontalAlignment = Element.ALIGN_LEFT;
            opfCell.VerticalAlignment = Element.ALIGN_LEFT;
            opfCell.Border = Rectangle.NO_BORDER;
            opfCell.PaddingBottom = 3f;
            oPdf.AddCell(opfCell);

            opfCell = new PdfPCell(new Phrase("Nº Vagas:", oFontForField)) { Colspan = 5, PaddingLeft = 8 };
            opfCell.HorizontalAlignment = Element.ALIGN_LEFT;
            opfCell.VerticalAlignment = Element.ALIGN_LEFT;
            opfCell.Border = Rectangle.NO_BORDER;
            opfCell.PaddingBottom = 3f;
            oPdf.AddCell(opfCell);

            opfCell = new PdfPCell(new Phrase(vagas.ToString(), oFontValue)) { Colspan = 15 };
            opfCell.HorizontalAlignment = Element.ALIGN_LEFT;
            opfCell.VerticalAlignment = Element.ALIGN_LEFT;
            opfCell.Border = Rectangle.NO_BORDER;
            opfCell.PaddingBottom = 3f;
            oPdf.AddCell(opfCell);

            opfCell = new PdfPCell(new Phrase("Vagas Disponíveis:", oFontForField)) { Colspan = 8, PaddingLeft = 8 };
            opfCell.HorizontalAlignment = Element.ALIGN_LEFT;
            opfCell.VerticalAlignment = Element.ALIGN_LEFT;
            opfCell.Border = Rectangle.NO_BORDER;
            opfCell.PaddingBottom = 3f;
            oPdf.AddCell(opfCell);

            opfCell = new PdfPCell(new Phrase(vagasDisponiveis.ToString(), oFontValue)) { Colspan = 12 };
            opfCell.HorizontalAlignment = Element.ALIGN_LEFT;
            opfCell.VerticalAlignment = Element.ALIGN_LEFT;
            opfCell.Border = Rectangle.NO_BORDER;
            opfCell.PaddingBottom = 3f;
            oPdf.AddCell(opfCell);

            opfCell = new PdfPCell(new Phrase(" ", oFontValueMenor)) { Colspan = 40 };
            opfCell.HorizontalAlignment = Element.ALIGN_LEFT;
            opfCell.VerticalAlignment = Element.ALIGN_LEFT;
            opfCell.Border = Rectangle.NO_BORDER;
            opfCell.PaddingBottom = 3f;
            oPdf.AddCell(opfCell);

            opfCell = new PdfPCell(new Phrase(" ", oFontValueMenor)) { Colspan = 40 };
            opfCell.HorizontalAlignment = Element.ALIGN_LEFT;
            opfCell.VerticalAlignment = Element.ALIGN_LEFT;
            opfCell.Border = Rectangle.NO_BORDER;
            opfCell.PaddingBottom = 3f;
            oPdf.AddCell(opfCell);

            //tabela de horários
            opfCell = new PdfPCell(new Phrase("Lista de Inscritos", oFontValueMenor)) { Colspan = 40 };
            opfCell.HorizontalAlignment = Element.ALIGN_LEFT;
            opfCell.VerticalAlignment = Element.ALIGN_LEFT;
            opfCell.Border = Rectangle.NO_BORDER;
            opfCell.PaddingBottom = 3f;
            oPdf.AddCell(opfCell);

            opfCell = new PdfPCell(new Phrase(" ", oFontValueMenor)) { Colspan = 40 };
            opfCell.HorizontalAlignment = Element.ALIGN_LEFT;
            opfCell.VerticalAlignment = Element.ALIGN_LEFT;
            opfCell.Border = Rectangle.NO_BORDER;
            opfCell.PaddingBottom = 3f;
            oPdf.AddCell(opfCell);


            //checklist
            if (tipoAgenda.IDTipo == (int)EnumTipoAgenda.Checklist)
            {
                //colunas
                opfCell = new PdfPCell(new Phrase("Horário", oFontValueMenor)) { Colspan = 5, PaddingLeft = 5 };
                opfCell.HorizontalAlignment = Element.ALIGN_LEFT;
                opfCell.VerticalAlignment = Element.ALIGN_LEFT;
                opfCell.Border = Rectangle.BOX;
                opfCell.PaddingBottom = 3f;
                oPdf.AddCell(opfCell);

                opfCell = new PdfPCell(new Phrase("Linha de Negócios", oFontValueMenor)) { Colspan = 7, PaddingLeft = 5 };
                opfCell.HorizontalAlignment = Element.ALIGN_LEFT;
                opfCell.VerticalAlignment = Element.ALIGN_LEFT;
                opfCell.Border = Rectangle.BOX;
                opfCell.PaddingBottom = 3f;
                oPdf.AddCell(opfCell);

                opfCell = new PdfPCell(new Phrase("Operação", oFontValueMenor)) { Colspan = 4, PaddingLeft = 5 };
                opfCell.HorizontalAlignment = Element.ALIGN_LEFT;
                opfCell.VerticalAlignment = Element.ALIGN_LEFT;
                opfCell.Border = Rectangle.BOX;
                opfCell.PaddingBottom = 3f;
                oPdf.AddCell(opfCell);

                opfCell = new PdfPCell(new Phrase("Tipo da Composição", oFontValueMenor)) { Colspan = 8, PaddingLeft = 5 };
                opfCell.HorizontalAlignment = Element.ALIGN_LEFT;
                opfCell.VerticalAlignment = Element.ALIGN_LEFT;
                opfCell.Border = Rectangle.BOX;
                opfCell.PaddingBottom = 3f;
                oPdf.AddCell(opfCell);

                opfCell = new PdfPCell(new Phrase("Placa 1", oFontValueMenor)) { Colspan = 4, PaddingLeft = 5 };
                opfCell.HorizontalAlignment = Element.ALIGN_LEFT;
                opfCell.VerticalAlignment = Element.ALIGN_LEFT;
                opfCell.Border = Rectangle.BOX;
                opfCell.PaddingBottom = 3f;
                oPdf.AddCell(opfCell);

                opfCell = new PdfPCell(new Phrase("Placa 2", oFontValueMenor)) { Colspan = 4, PaddingLeft = 5 };
                opfCell.HorizontalAlignment = Element.ALIGN_LEFT;
                opfCell.VerticalAlignment = Element.ALIGN_LEFT;
                opfCell.Border = Rectangle.BOX;
                opfCell.PaddingBottom = 3f;
                oPdf.AddCell(opfCell);

                opfCell = new PdfPCell(new Phrase("Placa 3", oFontValueMenor)) { Colspan = 4, PaddingLeft = 5 };
                opfCell.HorizontalAlignment = Element.ALIGN_LEFT;
                opfCell.VerticalAlignment = Element.ALIGN_LEFT;
                opfCell.Border = Rectangle.BOX;
                opfCell.PaddingBottom = 3f;
                oPdf.AddCell(opfCell);

                opfCell = new PdfPCell(new Phrase("Placa 4", oFontValueMenor)) { Colspan = 4, PaddingLeft = 5 };
                opfCell.HorizontalAlignment = Element.ALIGN_LEFT;
                opfCell.VerticalAlignment = Element.ALIGN_LEFT;
                opfCell.Border = Rectangle.BOX;
                opfCell.PaddingBottom = 3f;
                oPdf.AddCell(opfCell);

                //linhas
                if (listaInscritos != null && listaInscritos.Any())
                {
                    foreach (var item in listaInscritos)
                    {
                        opfCell = new PdfPCell(new Phrase(item.Horario, oFontForFieldMenor)) { Colspan = 5, PaddingLeft = 5 };
                        opfCell.HorizontalAlignment = Element.ALIGN_LEFT;
                        opfCell.VerticalAlignment = Element.ALIGN_LEFT;
                        opfCell.Border = Rectangle.BOX;
                        opfCell.PaddingBottom = 3f;
                        oPdf.AddCell(opfCell);

                        opfCell = new PdfPCell(new Phrase(item.LinhaNegocios, oFontForFieldMenor)) { Colspan = 7, PaddingLeft = 5 };
                        opfCell.HorizontalAlignment = Element.ALIGN_LEFT;
                        opfCell.VerticalAlignment = Element.ALIGN_LEFT;
                        opfCell.Border = Rectangle.BOX;
                        opfCell.PaddingBottom = 3f;
                        oPdf.AddCell(opfCell);

                        opfCell = new PdfPCell(new Phrase(item.Operacao, oFontForFieldMenor)) { Colspan = 4, PaddingLeft = 5 };
                        opfCell.HorizontalAlignment = Element.ALIGN_LEFT;
                        opfCell.VerticalAlignment = Element.ALIGN_LEFT;
                        opfCell.Border = Rectangle.BOX;
                        opfCell.PaddingBottom = 3f;
                        oPdf.AddCell(opfCell);

                        opfCell = new PdfPCell(new Phrase(item.TipoComposicao, oFontForFieldMenor)) { Colspan = 8, PaddingLeft = 5 };
                        opfCell.HorizontalAlignment = Element.ALIGN_LEFT;
                        opfCell.VerticalAlignment = Element.ALIGN_LEFT;
                        opfCell.Border = Rectangle.BOX;
                        opfCell.PaddingBottom = 3f;
                        oPdf.AddCell(opfCell);

                        opfCell = new PdfPCell(new Phrase(item.Placa1, oFontForFieldMenor)) { Colspan = 4, PaddingLeft = 5 };
                        opfCell.HorizontalAlignment = Element.ALIGN_LEFT;
                        opfCell.VerticalAlignment = Element.ALIGN_LEFT;
                        opfCell.Border = Rectangle.BOX;
                        opfCell.PaddingBottom = 3f;
                        oPdf.AddCell(opfCell);

                        opfCell = new PdfPCell(new Phrase(item.Placa2, oFontForFieldMenor)) { Colspan = 4, PaddingLeft = 5 };
                        opfCell.HorizontalAlignment = Element.ALIGN_LEFT;
                        opfCell.VerticalAlignment = Element.ALIGN_LEFT;
                        opfCell.Border = Rectangle.BOX;
                        opfCell.PaddingBottom = 3f;
                        oPdf.AddCell(opfCell);

                        opfCell = new PdfPCell(new Phrase(item.Placa3, oFontForFieldMenor)) { Colspan = 4, PaddingLeft = 5 };
                        opfCell.HorizontalAlignment = Element.ALIGN_LEFT;
                        opfCell.VerticalAlignment = Element.ALIGN_LEFT;
                        opfCell.Border = Rectangle.BOX;
                        opfCell.PaddingBottom = 3f;
                        oPdf.AddCell(opfCell);

                        opfCell = new PdfPCell(new Phrase(item.Placa4, oFontForFieldMenor)) { Colspan = 4, PaddingLeft = 5 };
                        opfCell.HorizontalAlignment = Element.ALIGN_LEFT;
                        opfCell.VerticalAlignment = Element.ALIGN_LEFT;
                        opfCell.Border = Rectangle.BOX;
                        opfCell.PaddingBottom = 3f;
                        oPdf.AddCell(opfCell);
                    }
                }
            }
            else
            {
                //colunas
                opfCell = new PdfPCell(new Phrase("Horário", oFontValueMenor)) { Colspan = 5, PaddingLeft = 5 };
                opfCell.HorizontalAlignment = Element.ALIGN_LEFT;
                opfCell.VerticalAlignment = Element.ALIGN_LEFT;
                opfCell.Border = Rectangle.BOX;
                opfCell.PaddingBottom = 3f;
                oPdf.AddCell(opfCell);

                opfCell = new PdfPCell(new Phrase("Operação", oFontValueMenor)) { Colspan = 4, PaddingLeft = 5 };
                opfCell.HorizontalAlignment = Element.ALIGN_LEFT;
                opfCell.VerticalAlignment = Element.ALIGN_LEFT;
                opfCell.Border = Rectangle.BOX;
                opfCell.PaddingBottom = 3f;
                oPdf.AddCell(opfCell);

                opfCell = new PdfPCell(new Phrase("CPF Motorista", oFontValueMenor)) { Colspan = 6, PaddingLeft = 5 };
                opfCell.HorizontalAlignment = Element.ALIGN_LEFT;
                opfCell.VerticalAlignment = Element.ALIGN_LEFT;
                opfCell.Border = Rectangle.BOX;
                opfCell.PaddingBottom = 3f;
                oPdf.AddCell(opfCell);

                opfCell = new PdfPCell(new Phrase("Nome Motorista", oFontValueMenor)) { Colspan = 6, PaddingLeft = 5 };
                opfCell.HorizontalAlignment = Element.ALIGN_LEFT;
                opfCell.VerticalAlignment = Element.ALIGN_LEFT;
                opfCell.Border = Rectangle.BOX;
                opfCell.PaddingBottom = 3f;
                oPdf.AddCell(opfCell);

                opfCell = new PdfPCell(new Phrase("Situação", oFontValueMenor)) { Colspan = 6, PaddingLeft = 5 };
                opfCell.HorizontalAlignment = Element.ALIGN_LEFT;
                opfCell.VerticalAlignment = Element.ALIGN_LEFT;
                opfCell.Border = Rectangle.BOX;
                opfCell.PaddingBottom = 3f;
                oPdf.AddCell(opfCell);

                opfCell = new PdfPCell(new Phrase("Assinatura", oFontValueMenor)) { Colspan = 13, PaddingLeft = 5 };
                opfCell.HorizontalAlignment = Element.ALIGN_LEFT;
                opfCell.VerticalAlignment = Element.ALIGN_LEFT;
                opfCell.Border = Rectangle.BOX;
                opfCell.PaddingBottom = 3f;
                oPdf.AddCell(opfCell);

                //linhas
                if (listaInscritos != null && listaInscritos.Any())
                {
                    foreach (var item in listaInscritos)
                    {
                        opfCell = new PdfPCell(new Phrase(item.Horario, oFontForFieldMenor)) { Colspan = 5, PaddingLeft = 5 };
                        opfCell.HorizontalAlignment = Element.ALIGN_LEFT;
                        opfCell.VerticalAlignment = Element.ALIGN_LEFT;
                        opfCell.Border = Rectangle.BOX;
                        opfCell.PaddingBottom = 3f;
                        oPdf.AddCell(opfCell);

                        opfCell = new PdfPCell(new Phrase(item.Operacao, oFontForFieldMenor)) { Colspan = 4, PaddingLeft = 5 };
                        opfCell.HorizontalAlignment = Element.ALIGN_LEFT;
                        opfCell.VerticalAlignment = Element.ALIGN_LEFT;
                        opfCell.Border = Rectangle.BOX;
                        opfCell.PaddingBottom = 3f;
                        oPdf.AddCell(opfCell);

                        opfCell = new PdfPCell(new Phrase(Convert.ToUInt64(item.CPF).ToString(@"000\.000\.000\-00"), oFontForFieldMenor)) { Colspan = 6, PaddingLeft = 5 };
                        opfCell.HorizontalAlignment = Element.ALIGN_LEFT;
                        opfCell.VerticalAlignment = Element.ALIGN_LEFT;
                        opfCell.Border = Rectangle.BOX;
                        opfCell.PaddingBottom = 3f;
                        oPdf.AddCell(opfCell);

                        opfCell = new PdfPCell(new Phrase(item.Nome, oFontForFieldMenor)) { Colspan = 6, PaddingLeft = 5 };
                        opfCell.HorizontalAlignment = Element.ALIGN_LEFT;
                        opfCell.VerticalAlignment = Element.ALIGN_LEFT;
                        opfCell.Border = Rectangle.BOX;
                        opfCell.PaddingBottom = 3f;
                        oPdf.AddCell(opfCell);

                        opfCell = new PdfPCell(new Phrase(item.IdSituacao != null ? EnumExtensions.GetDescription((EnumSituacaoAgendamento)item.IdSituacao) : " ", oFontForFieldMenor)) { Colspan = 6, PaddingLeft = 5 };
                        opfCell.HorizontalAlignment = Element.ALIGN_LEFT;
                        opfCell.VerticalAlignment = Element.ALIGN_LEFT;
                        opfCell.Border = Rectangle.BOX;
                        opfCell.PaddingBottom = 3f;
                        oPdf.AddCell(opfCell);

                        opfCell = new PdfPCell(new Phrase(" ", oFontForFieldMenor)) { Colspan = 13, PaddingLeft = 5 };
                        opfCell.HorizontalAlignment = Element.ALIGN_LEFT;
                        opfCell.VerticalAlignment = Element.ALIGN_LEFT;
                        opfCell.Border = Rectangle.BOX;
                        opfCell.PaddingBottom = 3f;
                        oPdf.AddCell(opfCell);
                    }
                }
            }
            oPdfPCellMaster = new PdfPCell(oPdf) { Colspan = 40 };
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

            Rectangle linha = new Rectangle(oDocument.PageSize.Width - 10, 728, 10, 728);
            linha.Left += oDocument.LeftMargin;
            linha.Right -= oDocument.RightMargin;
            linha.Top -= oDocument.TopMargin;
            linha.Bottom += oDocument.BottomMargin;
            linha.Border = Rectangle.TOP_BORDER;
            //content.SetColorFill(oRoxoBackgroundHeader);
            content.SetColorStroke(BaseColor.BLACK);
            content.Rectangle(linha.Left, linha.Bottom, linha.Width, linha.Height);
            content.FillStroke();

            #endregion
            oDocument.Close();
            return memoryStream;

        }

        public void SalvarPresenca(AgendamentoTerminalView model)
        {
            if (model.listaControles != null)
            {
                model.listaControles.ForEach(x => _agendamentoTreinamentoBLL.Atualizar(new AgendamentoTreinamento
                {
                    ID = x.idAgendamentoTreinamento,
                    IDMotorista = x.IDEmpresaCongenere > 0 ? 0 : x.idMotorista,
                    IDAgendamentoTerminalHorario = x.idAgendamentoHorario,
                    Usuario = x.Usuario,
                    Data = x.Data,
                    IDSituacao = x.Situacao,
                    CPFCongenere = x.IDEmpresaCongenere > 0 ? x.Cpf : "",
                    NomeMotorista = x.IDEmpresaCongenere > 0 ? x.Nome : "",
                    IDEmpresaCongenere = x.IDEmpresaCongenere
                }));
            }
        }

        public List<ControlePresencaMotoristaView> ListarInscritosTreinamento(int idTerminal, int idTipoAgenda, DateTime data)
        {

            using (UniCadDalRepositorio<AgendamentoTerminal> repositorio = new UniCadDalRepositorio<AgendamentoTerminal>())
            {
                IQueryable<ControlePresencaMotoristaView> query = GetQueryInscritosTreinamento(idTerminal, idTipoAgenda, data, repositorio)
                                                                   .OrderBy(i => i.HoraInicio);
                return query.ToList();
            }

        }

        private List<AgendamentoTerminalHorarioView> ListarInscritos(int idTerminal, int idTipoAgenda, DateTime data, int tipoAgenda = (int)EnumTipoAgenda.Checklist)
        {

            using (UniCadDalRepositorio<AgendamentoTerminal> repositorio = new UniCadDalRepositorio<AgendamentoTerminal>())
            {

                IQueryable<AgendamentoTerminalHorarioView> query = GetQueryInscritos(idTerminal, idTipoAgenda, data, repositorio, tipoAgenda)
                                                                   .OrderBy(i => i.HoraInicio);
                return query.ToList();
            }

        }

        private IQueryable<AgendamentoTerminalHorarioView> GetQueryInscritos(int idTerminal, int idTipoAgenda, DateTime data, UniCadDalRepositorio<AgendamentoTerminal> repositorio, int tipoAgenda = (int)EnumTipoAgenda.Checklist)
        {
            IQueryable<AgendamentoTerminalHorarioView> query;
            if (tipoAgenda == (int)EnumTipoAgenda.Checklist)
                query = (from app in repositorio.ListComplex<AgendamentoTerminal>().AsNoTracking().OrderBy(i => i.Data)
                         join agendamentoHorario in repositorio.ListComplex<AgendamentoTerminalHorario>().AsNoTracking() on app.ID equals agendamentoHorario.IDAgendamentoTerminal
                         join agendamentoCheckList in repositorio.ListComplex<AgendamentoChecklist>().AsNoTracking() on agendamentoHorario.ID equals agendamentoCheckList.IDAgendamentoTerminalHorario
                         join comp in repositorio.ListComplex<Composicao>().AsNoTracking() on agendamentoCheckList.IDComposicao equals comp.ID into c
                         from composicao in c.DefaultIfEmpty()
                         join tipo in repositorio.ListComplex<TipoComposicao>().AsNoTracking() on composicao.IDTipoComposicao equals tipo.ID into t
                         from tp in t.DefaultIfEmpty()
                         join tipoComp in repositorio.ListComplex<TipoComposicao>().AsNoTracking() on agendamentoCheckList.IDTipoComposicaoCongenere equals tipoComp.ID into tc
                         from tcc in tc.DefaultIfEmpty()
                         join empresa in repositorio.ListComplex<Empresa>().AsNoTracking() on agendamentoHorario.IDEmpresa equals empresa.ID
                         join placa1 in repositorio.ListComplex<Placa>().AsNoTracking() on composicao.IDPlaca1 equals placa1.ID into g
                         from placa01 in g.DefaultIfEmpty()
                         join placa2 in repositorio.ListComplex<Placa>().AsNoTracking() on composicao.IDPlaca2 equals placa2.ID into h
                         from placa02 in h.DefaultIfEmpty()
                         join placa3 in repositorio.ListComplex<Placa>().AsNoTracking() on composicao.IDPlaca3 equals placa3.ID into i
                         from placa03 in i.DefaultIfEmpty()
                         join placa4 in repositorio.ListComplex<Placa>().AsNoTracking() on composicao.IDPlaca4 equals placa4.ID into j
                         from placa04 in j.DefaultIfEmpty()
                         where (app.IDTerminal == idTerminal)
                         && (app.IDTipoAgenda == idTipoAgenda)
                         && (app.Data == data)
                         && (app.Ativo == true)
                         select new AgendamentoTerminalHorarioView
                         {
                             HoraInicio = agendamentoHorario.HoraInicio,
                             HoraFim = agendamentoHorario.HoraFim,
                             Operacao = agendamentoHorario.Operacao == "CON" ? "Congênere" : agendamentoHorario.Operacao,
                             TipoComposicao = tcc != null ? tcc.Nome : tp.Nome,
                             Placa1 = !String.IsNullOrEmpty(agendamentoCheckList.PlacaCongenere) ? agendamentoCheckList.PlacaCongenere : placa01.PlacaVeiculo,
                             Placa2 = placa02.PlacaVeiculo,
                             Placa3 = placa03.PlacaVeiculo,
                             Placa4 = placa04.PlacaVeiculo,
                             LinhaNegocios = empresa.Nome
                         });
            else
                query = (from app in repositorio.ListComplex<AgendamentoTerminal>().AsNoTracking().OrderBy(i => i.Data)
                         join agendamentoHorario in repositorio.ListComplex<AgendamentoTerminalHorario>().AsNoTracking() on app.ID equals agendamentoHorario.IDAgendamentoTerminal
                         join agendamentoTreinamento in repositorio.ListComplex<AgendamentoTreinamento>().AsNoTracking() on agendamentoHorario.ID equals agendamentoTreinamento.IDAgendamentoTerminalHorario
                         join moto in repositorio.ListComplex<Motorista>().AsNoTracking() on agendamentoTreinamento.IDMotorista equals moto.ID into m
                         from m1 in m.DefaultIfEmpty()
                         join empresa in repositorio.ListComplex<Empresa>().AsNoTracking() on m1.IDEmpresa equals empresa.ID into e
                         from e1 in e.DefaultIfEmpty()
                         join empresa2 in repositorio.ListComplex<Empresa>().AsNoTracking() on agendamentoHorario.IDEmpresa equals empresa2.ID into em
                         from e2 in em.DefaultIfEmpty()
                         where (app.IDTerminal == idTerminal)
                         && (app.IDTipoAgenda == idTipoAgenda)
                         && (app.Data == data)
                         && (app.Ativo == true)
                         select new AgendamentoTerminalHorarioView
                         {
                             HoraInicio = agendamentoHorario.HoraInicio,
                             HoraFim = agendamentoHorario.HoraFim,
                             Operacao = m1 != null && !String.IsNullOrEmpty(m1.Operacao) ? m1.Operacao : agendamentoHorario.Operacao,
                             LinhaNegocios = e1 != null && !String.IsNullOrEmpty(e1.Nome) ?  e1.Nome : e2.Nome,
                             CPF = m1 != null && !String.IsNullOrEmpty(m1.MotoristaBrasil.CPF) ?  m1.MotoristaBrasil.CPF : agendamentoTreinamento.CPFCongenere,
                             Nome = m1 != null && !String.IsNullOrEmpty(m1.Nome) ?  m1.Nome : agendamentoTreinamento.NomeMotorista,
                             IdSituacao = agendamentoTreinamento.IDSituacao
                         });

            return query;
        }

        private IQueryable<ControlePresencaMotoristaView> GetQueryInscritosTreinamento(int idTerminal, int idTipoAgenda, DateTime data, UniCadDalRepositorio<AgendamentoTerminal> repositorio)
        {
            IQueryable<ControlePresencaMotoristaView> query = (from app in repositorio.ListComplex<AgendamentoTerminal>().AsNoTracking().OrderBy(i => i.Data)
                                                               join agendamentoHorario in repositorio.ListComplex<AgendamentoTerminalHorario>().AsNoTracking() on app.ID equals agendamentoHorario.IDAgendamentoTerminal
                                                               join agendamentoTreinamento in repositorio.ListComplex<AgendamentoTreinamento>().AsNoTracking() on agendamentoHorario.ID equals agendamentoTreinamento.IDAgendamentoTerminalHorario
                                                               join moto in repositorio.ListComplex<Motorista>().AsNoTracking() on agendamentoTreinamento.IDMotorista equals moto.ID into m
                                                               from m1 in m.DefaultIfEmpty()
                                                               join empresa in repositorio.ListComplex<Empresa>().AsNoTracking() on m1.IDEmpresa equals empresa.ID into e
                                                               from e1 in e.DefaultIfEmpty()
                                                               join empresa2 in repositorio.ListComplex<Empresa>().AsNoTracking() on agendamentoHorario.IDEmpresa equals empresa2.ID into ee
                                                               from e2 in ee.DefaultIfEmpty()
                                                               where (app.IDTerminal == idTerminal)
                                                               && (app.IDTipoAgenda == idTipoAgenda)
                                                               && (app.Data == data)
                                                               && (app.Ativo == true)
                                                               select new ControlePresencaMotoristaView
                                                               {
                                                                   idAgendamentoHorario = agendamentoHorario.ID,
                                                                   idAgendamentoTreinamento = agendamentoTreinamento.ID,
                                                                   HoraInicio = agendamentoHorario.HoraInicio,
                                                                   HoraFim = agendamentoHorario.HoraFim,
                                                                   LinhaNegocio = e1 != null ? e1.Nome : e2.Nome,
                                                                   Operacao = agendamentoHorario.Operacao,
                                                                   Cpf = m1 != null ? m1.MotoristaBrasil.CPF : agendamentoTreinamento.CPFCongenere,
                                                                   Nome = m1 != null ? m1.Nome : agendamentoTreinamento.NomeMotorista,
                                                                   Situacao = agendamentoTreinamento.IDSituacao,
                                                                   idMotorista = m1 != null ? m1.ID : 0,
                                                                   Usuario = agendamentoTreinamento.Usuario,
                                                                   Data = agendamentoTreinamento.Data,
                                                                   IDEmpresaCongenere = agendamentoTreinamento.IDEmpresaCongenere
                                                               });

            return query;
        }

        public bool ExcluirAgendamento(int id)
        {
            using (TransactionScope transactionScope = Transactions.CreateTransactionScope(1, IsolationLevel.ReadCommitted))
            {
                var horarios = _agendamentoTerminalHorarioBLL.Listar(w => w.IDAgendamentoTerminal == id);
                if (horarios != null && horarios.Any())
                {
                    var checklist = _agendamentoCheckListBLL.ListarPorAgendamentoTerminal(id);
                    if (checklist != null && checklist.Any())
                        checklist.ForEach(w => _agendamentoCheckListBLL.Excluir(w.ID));
                    var treinamentos = _agendamentoTreinamentoBLL.ListarPorAgendamentoTerminal(id);
                    if (treinamentos != null && treinamentos.Any())
                        treinamentos.ForEach(w => _agendamentoTreinamentoBLL.Excluir(w.ID));
                    horarios.ForEach(w => _agendamentoTerminalHorarioBLL.Excluir(w.ID));
                }
                this.Excluir(id);
                transactionScope.Complete();
                return true;
            }
        }

        public bool ExcluirAgendamentoHorario(int id)
        {
            using (TransactionScope transactionScope = Transactions.CreateTransactionScope(1, IsolationLevel.ReadCommitted))
            {
                var checklist = _agendamentoCheckListBLL.Listar(w => w.IDAgendamentoTerminalHorario == id);
                if (checklist != null && checklist.Any())
                    checklist.ForEach(w => _agendamentoCheckListBLL.Excluir(w.ID));
                var treinamento = _agendamentoTreinamentoBLL.Listar(w => w.IDAgendamentoTerminalHorario == id);
                if (treinamento != null && treinamento.Any())
                    treinamento.ForEach(w => _agendamentoTreinamentoBLL.Excluir(w.ID));
                _agendamentoTerminalHorarioBLL.Excluir(id);
                transactionScope.Complete();
                return true;
            }
        }

        public List<AgendamentoTerminalView> GetQueryControleAgendamento(AgendamentoTerminalFiltro filtro, bool isCount = false, long? paginalInicial = null, long? paginalFinal = null)
        {
            var paramIsCount = new SqlParameter("@IsCount", SqlDbType.Bit);
            var paramPrimeiraPagina = new SqlParameter("@PrimeiraPagina", SqlDbType.Int);
            var paramUltimaPagina = new SqlParameter("@UltimaPagina", SqlDbType.Int);
            var paramIdTerminal = new SqlParameter("@IdTerminal", SqlDbType.Int);
            var paramIdEmpresa = new SqlParameter("@IdEmpresa", SqlDbType.Int);
            var paramIdTipoTipoAgenda = new SqlParameter("IdTipoTipoAgenda", SqlDbType.Int);
            var paramOperacao = new SqlParameter("@Operacao", SqlDbType.VarChar);
            var paramIdTipoAgenda = new SqlParameter("@IdTipoAgenda", SqlDbType.Int);
            var paramInicio = new SqlParameter("@DataInicio", SqlDbType.Date);
            var paramFim = new SqlParameter("@DataFim", SqlDbType.Date);
            var paraData = new SqlParameter("@Data", SqlDbType.Date);

            paramIsCount.Value = isCount;
            paramPrimeiraPagina.Value = paginalInicial ?? (object)DBNull.Value;
            paramUltimaPagina.Value = paginalFinal ?? (object)DBNull.Value;
            paramIdEmpresa.Value = filtro.IdEmpresa ?? (object)DBNull.Value;
            paramIdTipoAgenda.Value = filtro.IdTipoAgenda ?? (object)DBNull.Value;
            paramIdTipoTipoAgenda.Value = filtro.IdTipoTipoAgenda ?? (object)DBNull.Value;
            paramOperacao.Value = string.IsNullOrEmpty(filtro.Operacao) ? (object)DBNull.Value : filtro.Operacao;
            paramIdTerminal.Value = filtro.IdTerminal ?? (object)DBNull.Value;
            paramInicio.Value = filtro.DtInicio ?? (object)DBNull.Value;
            paramFim.Value = filtro.DtFim ?? (object)DBNull.Value;
            paraData.Value = filtro.Data ?? (object)DBNull.Value;
            List<AgendamentoTerminalView> dadosRelatorio = ExecutarProcedureComRetorno<AgendamentoTerminalView>(
                "[dbo].[Proc_Pesquisa_Agendamento] @IsCount,@PrimeiraPagina,@UltimaPagina,@IdTerminal,@IdEmpresa,@IdTipoTipoAgenda,@Operacao,@IdTipoAgenda,@DataInicio,@DataFim,@Data",
                new object[] { paramIsCount, paramPrimeiraPagina, paramUltimaPagina, paramIdTerminal, paramIdEmpresa, paramIdTipoTipoAgenda, paramOperacao, paramIdTipoAgenda, paramInicio, paramFim, paraData });
            return dadosRelatorio;
        }

        public AgendamentoTerminalView SelecionarControleAgendamentos(AgendamentoTerminalFiltro filtro)
        {
            using (UniCadDalRepositorio<Motorista> repositorio = new UniCadDalRepositorio<Motorista>())
            {
                var resultado = GetQueryControleAgendamento(filtro);
                return resultado.FirstOrDefault();
            }
        }

        public List<AgendamentoTerminalView> ListarControleAgendamentos(AgendamentoTerminalFiltro filtro, PaginadorModel paginador)
        {
            using (UniCadDalRepositorio<Motorista> repositorio = new UniCadDalRepositorio<Motorista>())
            {
                int ultimaPagina = paginador.QtdeItensPagina * paginador.PaginaAtual;
                var resultado = GetQueryControleAgendamento(filtro, false, paginador.InicioPaginacao, ultimaPagina);
                return resultado;
            }
        }

        public int ListarControleAgendamentosCount(AgendamentoTerminalFiltro filtro)
        {
            using (UniCadDalRepositorio<Motorista> repositorio = new UniCadDalRepositorio<Motorista>())
            {
                var dados = GetQueryControleAgendamento(filtro, true);
                return dados[0].Linhas;
            }
        }

        public string Clonar(int id, DateTime[] datas)
        {
            StringBuilder retorno = new StringBuilder();
            var agendamentoTerminal = this.Selecionar(id);
            var agendamentoTerminalHorario = _agendamentoTerminalHorarioBLL.Listar(w => w.IDAgendamentoTerminal == id);
            var listAgendamentoTerminal = this.Listar(w =>
                w.Ativo == agendamentoTerminal.Ativo
                && w.IDTerminal == agendamentoTerminal.IDTerminal
                && w.IDTipoAgenda == agendamentoTerminal.IDTipoAgenda);
            using (TransactionScope transactionScope = Transactions.CreateTransactionScope(1, IsolationLevel.ReadCommitted))
            {
                foreach (var data in datas.OrderBy(w => w.Date))
                {
                    //validar se já existe essa agenda
                    if (listAgendamentoTerminal.Any(w => w.Data == data))
                        retorno.AppendLine($"A agenda para o dia {data:dd/MM/yyyy} não pode ser replicada, pois já existe agendamento para este dia.<br/><br/>");
                    else
                    {
                        //caso não existe, primeiramente adicionar o agendamento (data)
                        var agendamento = new AgendamentoTerminal
                        {
                            Ativo = agendamentoTerminal.Ativo,
                            Data = data,
                            IDTerminal = agendamentoTerminal.IDTerminal,
                            IDTipoAgenda = agendamentoTerminal.IDTipoAgenda
                        };

                        this.Adicionar(agendamento);

                        //adicionar os horários
                        foreach (var horario in agendamentoTerminalHorario)
                        {
                            _agendamentoTerminalHorarioBLL.Adicionar(
                                new AgendamentoTerminalHorario
                                {
                                    IDAgendamentoTerminal = agendamento.ID,
                                    HoraInicio = horario.HoraInicio,
                                    HoraFim = horario.HoraFim,
                                    IDEmpresa = horario.IDEmpresa,
                                    Operacao = horario.Operacao,
                                    Vagas = horario.Vagas
                                }
                            );
                        }
                    }

                }
                transactionScope.Complete();
            }
            return retorno.ToString();
        }

        private IQueryable<AgendamentoTerminalView> GetQueryListarAgendamentoTerminalHorario(AgendamentoTerminalFiltro filtro, UniCadDalRepositorio<AgendamentoTerminal> repositorio)
        {
            IQueryable<AgendamentoTerminalView> query = (from app in repositorio.ListComplex<AgendamentoTerminal>().AsNoTracking().OrderBy(i => i.Data)
                                                         join agendamentoHorario in repositorio.ListComplex<AgendamentoTerminalHorario>().AsNoTracking() on app.ID equals agendamentoHorario.IDAgendamentoTerminal
                                                         where (app.IDTerminal == filtro.IdTerminal)
                                                         && (app.IDTipoAgenda == filtro.IdTipoAgenda)
                                                         && (app.Data == filtro.Data)
                                                         select new AgendamentoTerminalView
                                                         {
                                                         });

            return query;
        }

        private IQueryable<AgendamentoTerminalView> GetQueryVerificarSeJaExisteHorario(AgendamentoTerminalFiltro filtro, UniCadDalRepositorio<AgendamentoTerminal> repositorio)
        {
            IQueryable<AgendamentoTerminalView> query = (from app in repositorio.ListComplex<AgendamentoTerminal>().AsNoTracking().OrderBy(i => i.Data)
                                                         join agendamentoHorario in repositorio.ListComplex<AgendamentoTerminalHorario>().AsNoTracking() on app.ID equals agendamentoHorario.IDAgendamentoTerminal
                                                         where (app.IDTerminal == filtro.IdTerminal)
                                                         && (app.IDTipoAgenda == filtro.IdTipoAgenda)
                                                         && (app.Data == filtro.Data)
                                                         && (app.Ativo == true)
                                                         && (agendamentoHorario.IDEmpresa == filtro.IdEmpresa)
                                                         && (agendamentoHorario.Operacao == filtro.Operacao)
                                                         && (agendamentoHorario.HoraInicio == filtro.HoraInicio)
                                                         && (agendamentoHorario.HoraFim == filtro.HoraFim)
                                                         && (filtro.IdHorario == 0 || agendamentoHorario.ID != filtro.IdHorario)
                                                         select new AgendamentoTerminalView
                                                         {
                                                         });

            return query;
        }

        public int ListarAgendamentoTerminalCount(AgendamentoTerminalFiltro filtro)
        {

            using (UniCadDalRepositorio<AgendamentoTerminal> repositorio = new UniCadDalRepositorio<AgendamentoTerminal>())
            {
                IQueryable<AgendamentoTerminalView> query = GetQueryAgendamentoTerminal(filtro, repositorio);
                return query.Count();
            }

        }

        private IQueryable<AgendamentoTerminalView> GetQueryAgendamentoTerminal(AgendamentoTerminalFiltro filtro, IUniCadDalRepositorio<AgendamentoTerminal> repositorio)
        {
            IQueryable<AgendamentoTerminalView> query = (from app in repositorio.ListComplex<AgendamentoTerminal>().AsNoTracking().OrderBy(i => i.Data)
                                                         join terminal in repositorio.ListComplex<Terminal>().AsNoTracking() on app.IDTerminal equals terminal.ID
                                                         join tipoAgenda in repositorio.ListComplex<TipoAgenda>().AsNoTracking() on app.IDTipoAgenda equals tipoAgenda.ID
                                                         where (app.IDTerminal == (filtro.IdTerminal.HasValue ? filtro.IdTerminal : app.IDTerminal))
                                                         && (app.IDTipoAgenda == (filtro.IdTipoAgenda.HasValue ? filtro.IdTipoAgenda : app.IDTipoAgenda))
                                                         && (!filtro.DtInicio.HasValue || DbFunctions.TruncateTime(app.Data) >= filtro.DtInicio)
                                                         && (!filtro.DtFim.HasValue || DbFunctions.TruncateTime(app.Data) <= filtro.DtFim)
                                                         && (app.Ativo == (filtro.IdStatus.HasValue ? filtro.IdStatus : app.Ativo))
                                                         select new AgendamentoTerminalView
                                                         {
                                                             ID = app.ID,
                                                             Data = app.Data,
                                                             Status = app.Ativo,
                                                             Terminal = terminal.Nome,
                                                             TipoAgenda = tipoAgenda.Nome
                                                         });
            return query;
        }

        public bool AdicionarHorario(AgendamentoTerminal agendamentoTerminal, AgendamentoTerminalHorario agendamentoTerminalHorario)
        {
            using (TransactionScope transactionScope = Transactions.CreateTransactionScope(1, IsolationLevel.ReadCommitted))
            {
                int id = BuscarId(agendamentoTerminal);
                agendamentoTerminal.ID = id;

                if (agendamentoTerminal.ID == 0)
                    this.Adicionar(agendamentoTerminal);

                agendamentoTerminalHorario.IDAgendamentoTerminal = agendamentoTerminal.ID;
                if (agendamentoTerminalHorario.idHoraAgenda > 0)
                {
                    agendamentoTerminalHorario.ID = agendamentoTerminalHorario.idHoraAgenda;
                    _agendamentoTerminalHorarioBLL.Atualizar(agendamentoTerminalHorario);
                }
                else
                {
                    _agendamentoTerminalHorarioBLL.Adicionar(agendamentoTerminalHorario);
                }
                transactionScope.Complete();
            }
            agendamentoTerminal.ListaAgendamentoTerminalHorario = _agendamentoTerminalHorarioBLL.ListarPorAgendamentoTerminal(agendamentoTerminal.ID);
            return true;
        }

        public int BuscarId(AgendamentoTerminal agendamentoTerminal)
        {
            //necessário buscar o agendamentoterminal cada vez que adicionar - caso mude os filtros muda os horários
            int id;
            id = (this.Listar(w => w.Data == agendamentoTerminal.Data
                && w.IDTerminal == agendamentoTerminal.IDTerminal
                && w.IDTipoAgenda == agendamentoTerminal.IDTipoAgenda).FirstOrDefault()?.ID) ?? 0;
            return id;
        }
    }
}

