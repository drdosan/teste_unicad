using ClosedXML.Excel;
using Infraestructure.Extensions;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Raizen.Framework.Models;
using Raizen.Framework.UserSystem.Client;
using Raizen.Framework.Utils.Extensions;
using Raizen.Framework.Utils.Transacao;
using Raizen.UniCad.BLL.Extensions;
using Raizen.UniCad.BLL.Interfaces;
using Raizen.UniCad.BLL.Util;
using Raizen.UniCad.DAL;
using Raizen.UniCad.Model;
using Raizen.UniCad.Model.Filtro;
using Raizen.UniCad.Model.View;
using Raizen.UniCad.SAL;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Transactions;
using EnumExtensions = Raizen.UniCad.Extensions.EnumExtensions;
using IsolationLevel = System.Transactions.IsolationLevel;
using Raizen.UniCad.Domain.Entities;
using Raizen.Framework.Log.Bases;
using Raizen.UniCad.BLL.Log;
using System.Reflection;
using System.Text.Json;
using Raizen.UniCad.SAL.WsIntegracaoSAPMotorista;
using DocumentFormat.OpenXml.EMMA;

namespace Raizen.UniCad.BLL
{
    public class MotoristaBusiness : UniCadBusinessBase<Motorista>, IMotoristaBusiness
    {
        private Action<string, string> _logar;
        private readonly MotoristaPesquisaBusiness _motoristaPesquisaBll = new MotoristaPesquisaBusiness();
        private readonly IConfigBusiness _configBusiness;
        private readonly EnumPais _pais;


        /// <summary>
        /// Definir um logger, que sera utilizado para registrar passo a passo de execucao de Job.
        /// </summary>
        /// <param name="logar">Action de Log</param>
        public void DefinirLogger(ILogExecucao logar)
        {
            _logar = new Action<string, string>((titulo, descricao) =>
            {
                logar.Log(titulo, descricao, CodigoExecucao.Descricao);
            });
        }


        public MotoristaBusiness()
        {
            _pais = EnumPais.Brasil;
            _configBusiness = new ConfigBusiness();
        }

        public MotoristaBusiness(EnumPais pais)
        {
            _pais = pais;
            _configBusiness = new ConfigBusiness();
        }

        public MemoryStream GerarPdfCarteirinha(int id)
        {
            #region parametros e configura��es
            MemoryStream memoryStream = new MemoryStream { Position = 0 };
            CarteirinhaView carteirinha = SelecionarMotoristaCarteirinha(id);
            carteirinha.CPFComMascara = Convert.ToUInt64(carteirinha.CPF).ToString(@"000\.000\.000\-00");
            carteirinha.DataEmissao = DateTime.Now.ToString("dd/MM/yyyy HH:mm");
            carteirinha.CNHVencimento = carteirinha.dataVencimentoCNH?.ToString("dd/MM/yyyy") ?? "";
            carteirinha.MOPPVencimento = carteirinha.dataVencimentoMOPP?.ToString("dd/MM/yyyy") ?? "";
            carteirinha.NR20Vencimento = carteirinha.dataVencimentoNR20?.ToString("dd/MM/yyyy") ?? "";
            carteirinha.NR35Vencimento = carteirinha.dataVencimentoNr35?.ToString("dd/MM/yyyy") ?? "";
            carteirinha.CDDSVencimento = carteirinha.dataVencimentoCdds?.ToString("dd/MM/yyyy") ?? "";



            //Cria o documento e informa o caminho/nome que ser� usado para salvar o arquivo.
            //A4 size - 210mm x 297mm, or 8.26 inches x 11.69 inches            
            Document oDocument = new Document(PageSize.A4, 0, 0, 1, 0);

            //USAR COMO PAISAGEM
            //oDocument.SetPageSize(PageSize.A4.Rotate());

            //Cria uma inst�ncia do PdfWriter e escreve no Response.OutputStream.
            PdfWriter pdfWriter = PdfWriter.GetInstance(oDocument, memoryStream);
            pdfWriter.PageEvent = new PdfFooter();

            #endregion parametros e configura��es

            try
            {
                //Abre o documento para iniciar a constru��o.
                oDocument.Open();
                PdfContentByte cb = pdfWriter.DirectContent;

                #region [ Objetos gen�ricos e constantes ]
                //Cria uma tabela generica que servir� para transportar os detalhes de todas as areas.
                PdfPTable oPdfPTable;

                //Cria uma celular generica que servir� para transportar as tabelas com detalhes de todas as �reas.
                PdfPCell oPdfPCellMaster;


                //Cria uma cor generica Cinza CSC                
                BaseColor oRoxoBackgroundHeader = new BaseColor(146, 41, 128);
                BaseColor oCinzaSubFonte = new BaseColor(120, 120, 120);
                var oBrancoBackground = new BaseColor(255, 255, 255);
                //BaseColor oBrancoFonteHeader = new BaseColor(255, 255, 240);

                //Fontes que ser�o utilizadas
                Font oFontForHeader = new Font(Font.FontFamily.HELVETICA, 14, Font.NORMAL, BaseColor.BLACK);
                Font oFontForValue = new Font(Font.FontFamily.HELVETICA, 10, Font.NORMAL, BaseColor.BLACK);
                Font oFontForAssinatura = new Font(Font.FontFamily.HELVETICA, 7, Font.NORMAL, BaseColor.BLACK);
                Font oFontCabecalhoForValue = new Font(Font.FontFamily.HELVETICA, 20, Font.BOLD, BaseColor.WHITE);
                Font oFontSubCabecalhoForValue = new Font(Font.FontFamily.HELVETICA, 15, Font.BOLD, oCinzaSubFonte);
                #endregion

                //Cria a tabela que servir� de container para todas as �reas
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
                var oPdfEsq = new PdfPTable(new float[] { 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10 });

                opdfCellEsquerdo = new PdfPCell(new Phrase("Cart�o de Identifica��o", oFontForHeader)) { Colspan = 20 };
                opdfCellEsquerdo.HorizontalAlignment = Element.ALIGN_CENTER;
                opdfCellEsquerdo.VerticalAlignment = Element.ALIGN_CENTER;
                opdfCellEsquerdo.Border = Rectangle.NO_BORDER;
                opdfCellEsquerdo.PaddingBottom = 3f;
                oPdfEsq.AddCell(opdfCellEsquerdo);

                opdfCellEsquerdo = new PdfPCell(new Phrase(string.IsNullOrEmpty(carteirinha.Transportadora) ? " " : carteirinha.Transportadora, oFontForValue)) { Colspan = 20 };
                opdfCellEsquerdo.HorizontalAlignment = Element.ALIGN_CENTER;
                opdfCellEsquerdo.VerticalAlignment = Element.ALIGN_CENTER;
                opdfCellEsquerdo.Border = Rectangle.NO_BORDER;
                opdfCellEsquerdo.PaddingBottom = 3f;
                oPdfEsq.AddCell(opdfCellEsquerdo);

                opdfCellEsquerdo = new PdfPCell(new Phrase(carteirinha.Nome, oFontForValue)) { Colspan = 20 };
                opdfCellEsquerdo.HorizontalAlignment = Element.ALIGN_CENTER;
                opdfCellEsquerdo.VerticalAlignment = Element.ALIGN_CENTER;
                opdfCellEsquerdo.Border = Rectangle.NO_BORDER;
                opdfCellEsquerdo.PaddingBottom = 3f;
                oPdfEsq.AddCell(opdfCellEsquerdo);

                opdfCellEsquerdo = new PdfPCell(new Phrase(" ", oFontForValue)) { Colspan = 20 };
                opdfCellEsquerdo.HorizontalAlignment = Element.ALIGN_CENTER;
                opdfCellEsquerdo.VerticalAlignment = Element.ALIGN_CENTER;
                opdfCellEsquerdo.Border = Rectangle.NO_BORDER;
                opdfCellEsquerdo.PaddingBottom = 3f;
                oPdfEsq.AddCell(opdfCellEsquerdo);

                opdfCellEsquerdo = new PdfPCell(new Phrase(" ", oFontForValue)) { Colspan = 20 };
                opdfCellEsquerdo.HorizontalAlignment = Element.ALIGN_CENTER;
                opdfCellEsquerdo.VerticalAlignment = Element.ALIGN_CENTER;
                opdfCellEsquerdo.Border = Rectangle.NO_BORDER;
                opdfCellEsquerdo.PaddingBottom = 3f;
                oPdfEsq.AddCell(opdfCellEsquerdo);


                opdfCellEsquerdo = new PdfPCell(new Phrase(" ", oFontForValue)) { Colspan = 20 };
                opdfCellEsquerdo.HorizontalAlignment = Element.ALIGN_CENTER;
                opdfCellEsquerdo.VerticalAlignment = Element.ALIGN_CENTER;
                opdfCellEsquerdo.Border = Rectangle.NO_BORDER;
                opdfCellEsquerdo.PaddingBottom = 3f;
                oPdfEsq.AddCell(opdfCellEsquerdo);
                opdfCellEsquerdo = new PdfPCell(new Phrase(" ", oFontForValue)) { Colspan = 20 };
                opdfCellEsquerdo.HorizontalAlignment = Element.ALIGN_CENTER;
                opdfCellEsquerdo.VerticalAlignment = Element.ALIGN_CENTER;
                opdfCellEsquerdo.Border = Rectangle.NO_BORDER;
                opdfCellEsquerdo.PaddingBottom = 3f;
                oPdfEsq.AddCell(opdfCellEsquerdo);

                opdfCellEsquerdo = new PdfPCell(new Phrase(" ", oFontForValue)) { Colspan = 3 };
                opdfCellEsquerdo.HorizontalAlignment = Element.ALIGN_LEFT;
                opdfCellEsquerdo.VerticalAlignment = Element.ALIGN_CENTER;
                opdfCellEsquerdo.Border = Rectangle.NO_BORDER;
                opdfCellEsquerdo.PaddingBottom = 3f;
                oPdfEsq.AddCell(opdfCellEsquerdo);

                opdfCellEsquerdo = new PdfPCell(new Phrase("Foto", oFontForValue)) { Colspan = 17 };
                opdfCellEsquerdo.HorizontalAlignment = Element.ALIGN_LEFT;
                opdfCellEsquerdo.VerticalAlignment = Element.ALIGN_CENTER;
                opdfCellEsquerdo.Border = Rectangle.NO_BORDER;
                opdfCellEsquerdo.PaddingBottom = 3f;
                oPdfEsq.AddCell(opdfCellEsquerdo);

                opdfCellEsquerdo = new PdfPCell(new Phrase(" ", oFontForValue)) { Colspan = 20 };
                opdfCellEsquerdo.HorizontalAlignment = Element.ALIGN_CENTER;
                opdfCellEsquerdo.VerticalAlignment = Element.ALIGN_BOTTOM;
                opdfCellEsquerdo.Border = Rectangle.NO_BORDER;
                opdfCellEsquerdo.PaddingBottom = 3f;
                oPdfEsq.AddCell(opdfCellEsquerdo);

                opdfCellEsquerdo = new PdfPCell(new Phrase(" ", oFontForValue)) { Colspan = 20 };
                opdfCellEsquerdo.HorizontalAlignment = Element.ALIGN_CENTER;
                opdfCellEsquerdo.VerticalAlignment = Element.ALIGN_BOTTOM;
                opdfCellEsquerdo.Border = Rectangle.NO_BORDER;
                opdfCellEsquerdo.PaddingBottom = 3f;
                oPdfEsq.AddCell(opdfCellEsquerdo);


                opdfCellEsquerdo = new PdfPCell(new Phrase(" ", oFontForValue)) { Colspan = 8 };
                opdfCellEsquerdo.HorizontalAlignment = Element.ALIGN_CENTER;
                opdfCellEsquerdo.VerticalAlignment = Element.ALIGN_BOTTOM;
                opdfCellEsquerdo.Border = Rectangle.NO_BORDER;
                opdfCellEsquerdo.PaddingBottom = 3f;
                oPdfEsq.AddCell(opdfCellEsquerdo);

                opdfCellEsquerdo = new PdfPCell(new Phrase("Assinatura do Motorista", oFontForAssinatura)) { Colspan = 12 };
                opdfCellEsquerdo.HorizontalAlignment = Element.ALIGN_CENTER;
                opdfCellEsquerdo.VerticalAlignment = Element.ALIGN_BOTTOM;
                opdfCellEsquerdo.Border = Rectangle.NO_BORDER;
                opdfCellEsquerdo.PaddingTop = 18f;
                oPdfEsq.AddCell(opdfCellEsquerdo);

                oPdfPCellMaster = new PdfPCell(oPdfEsq) { Colspan = 20 };
                oPdfPCellMaster.HorizontalAlignment = Element.ALIGN_CENTER;
                oPdfPCellMaster.VerticalAlignment = Element.ALIGN_CENTER;
                oPdfPCellMaster.Border = Rectangle.NO_BORDER;
                oPdfPCellMaster.BackgroundColor = oBrancoBackground;
                oPdfPCellMaster.PaddingBottom = 3f;
                oPdfPTable.AddCell(oPdfPCellMaster);


                PdfPCell opdfCellDireito = null;
                var oPdfDir = new PdfPTable(new float[] { 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10 });

                Barcode128 code128 = new Barcode128();
                code128.CodeType = Barcode.CODE128;
                code128.ChecksumText = true;
                code128.GenerateChecksum = true;
                code128.StartStopText = false;
                code128.BarHeight = 60;
                code128.X = 2;
                code128.Size = 5;
                code128.Code = carteirinha.CPF;
                Image code128Image = code128.CreateImageWithBarcode(cb, BaseColor.BLACK, BaseColor.WHITE);
                //System.Drawing.Bitmap bm = new System.Drawing.Bitmap(code128.CreateDrawingImage(System.Drawing.Color.Black, System.Drawing.Color.White));

                opdfCellDireito = new PdfPCell(new Phrase(" ", oFontForValue)) { Colspan = 20 };
                opdfCellDireito.HorizontalAlignment = Element.ALIGN_LEFT;
                opdfCellDireito.VerticalAlignment = Element.ALIGN_CENTER;
                opdfCellDireito.Border = Rectangle.NO_BORDER;
                opdfCellDireito.PaddingBottom = 3f;
                oPdfDir.AddCell(opdfCellDireito);

                opdfCellDireito = new PdfPCell(new Phrase(" ", oFontForValue)) { Colspan = 2 };
                opdfCellDireito.HorizontalAlignment = Element.ALIGN_LEFT;
                opdfCellDireito.VerticalAlignment = Element.ALIGN_LEFT;
                opdfCellDireito.Border = Rectangle.NO_BORDER;
                opdfCellDireito.PaddingBottom = 3f;
                oPdfDir.AddCell(opdfCellDireito);

                opdfCellDireito = new PdfPCell(new Phrase("CPF: " + carteirinha.CPFComMascara, oFontForValue)) { Colspan = 10 };
                opdfCellDireito.HorizontalAlignment = Element.ALIGN_LEFT;
                opdfCellDireito.VerticalAlignment = Element.ALIGN_CENTER;
                opdfCellDireito.Border = Rectangle.NO_BORDER;
                opdfCellDireito.PaddingBottom = 3f;
                oPdfDir.AddCell(opdfCellDireito);



                opdfCellDireito = new PdfPCell(new Phrase(new Chunk(code128Image, -20, -25))) { Colspan = 8 };
                opdfCellDireito.HorizontalAlignment = Element.ALIGN_RIGHT;
                opdfCellDireito.VerticalAlignment = Element.ALIGN_CENTER;
                opdfCellDireito.Border = Rectangle.NO_BORDER;
                opdfCellDireito.PaddingBottom = 3f;
                oPdfDir.AddCell(opdfCellDireito);


                opdfCellDireito = new PdfPCell(new Phrase(" ", oFontForValue)) { Colspan = 20 };
                opdfCellDireito.HorizontalAlignment = Element.ALIGN_LEFT;
                opdfCellDireito.VerticalAlignment = Element.ALIGN_CENTER;
                opdfCellDireito.Border = Rectangle.NO_BORDER;
                opdfCellDireito.PaddingBottom = 3f;
                oPdfDir.AddCell(opdfCellDireito);


                opdfCellDireito = new PdfPCell(new Phrase(" ", oFontForValue)) { Colspan = 20 };
                opdfCellDireito.HorizontalAlignment = Element.ALIGN_LEFT;
                opdfCellDireito.VerticalAlignment = Element.ALIGN_CENTER;
                opdfCellDireito.Border = Rectangle.NO_BORDER;
                opdfCellDireito.PaddingBottom = 3f;
                oPdfDir.AddCell(opdfCellDireito);

                opdfCellDireito = new PdfPCell(new Phrase("CNH: ", oFontForValue)) { Colspan = 7 };
                opdfCellDireito.HorizontalAlignment = Element.ALIGN_LEFT;
                opdfCellDireito.VerticalAlignment = Element.ALIGN_CENTER;
                opdfCellDireito.Border = Rectangle.NO_BORDER;
                opdfCellDireito.PaddingBottom = 3f;
                oPdfDir.AddCell(opdfCellDireito);

                opdfCellDireito = new PdfPCell(new Phrase(carteirinha.CNHVencimento, oFontForValue)) { Colspan = 11 };
                opdfCellDireito.HorizontalAlignment = Element.ALIGN_LEFT;
                opdfCellDireito.VerticalAlignment = Element.ALIGN_CENTER;
                opdfCellDireito.Border = Rectangle.NO_BORDER;
                opdfCellDireito.PaddingBottom = 3f;
                oPdfDir.AddCell(opdfCellDireito);

                opdfCellDireito = new PdfPCell(new Phrase(" ", oFontForValue)) { Colspan = 2 };
                opdfCellDireito.HorizontalAlignment = Element.ALIGN_LEFT;
                opdfCellDireito.VerticalAlignment = Element.ALIGN_CENTER;
                opdfCellDireito.Border = Rectangle.NO_BORDER;
                opdfCellDireito.PaddingBottom = 3f;
                oPdfDir.AddCell(opdfCellDireito);

                opdfCellDireito = new PdfPCell(new Phrase("MOPP: ", oFontForValue)) { Colspan = 7 };
                opdfCellDireito.HorizontalAlignment = Element.ALIGN_LEFT;
                opdfCellDireito.VerticalAlignment = Element.ALIGN_CENTER;
                opdfCellDireito.Border = Rectangle.NO_BORDER;
                opdfCellDireito.PaddingBottom = 3f;
                oPdfDir.AddCell(opdfCellDireito);

                opdfCellDireito = new PdfPCell(new Phrase(carteirinha.MOPPVencimento, oFontForValue)) { Colspan = 11 };
                opdfCellDireito.HorizontalAlignment = Element.ALIGN_LEFT;
                opdfCellDireito.VerticalAlignment = Element.ALIGN_CENTER;
                opdfCellDireito.Border = Rectangle.NO_BORDER;
                opdfCellDireito.PaddingBottom = 3f;
                oPdfDir.AddCell(opdfCellDireito);


                opdfCellDireito = new PdfPCell(new Phrase(" ", oFontForValue)) { Colspan = 2 };
                opdfCellDireito.HorizontalAlignment = Element.ALIGN_LEFT;
                opdfCellDireito.VerticalAlignment = Element.ALIGN_CENTER;
                opdfCellDireito.Border = Rectangle.NO_BORDER;
                opdfCellDireito.PaddingBottom = 3f;
                oPdfDir.AddCell(opdfCellDireito);

                opdfCellDireito = new PdfPCell(new Phrase("NR20: ", oFontForValue)) { Colspan = 7 };
                opdfCellDireito.HorizontalAlignment = Element.ALIGN_LEFT;
                opdfCellDireito.VerticalAlignment = Element.ALIGN_CENTER;
                opdfCellDireito.Border = Rectangle.NO_BORDER;
                opdfCellDireito.PaddingBottom = 3f;
                oPdfDir.AddCell(opdfCellDireito);

                opdfCellDireito = new PdfPCell(new Phrase(carteirinha.NR20Vencimento, oFontForValue)) { Colspan = 11 };
                opdfCellDireito.HorizontalAlignment = Element.ALIGN_LEFT;
                opdfCellDireito.VerticalAlignment = Element.ALIGN_CENTER;
                opdfCellDireito.Border = Rectangle.NO_BORDER;
                opdfCellDireito.PaddingBottom = 3f;
                oPdfDir.AddCell(opdfCellDireito);

                opdfCellDireito = new PdfPCell(new Phrase(" ", oFontForValue)) { Colspan = 2 };
                opdfCellDireito.HorizontalAlignment = Element.ALIGN_LEFT;
                opdfCellDireito.VerticalAlignment = Element.ALIGN_CENTER;
                opdfCellDireito.Border = Rectangle.NO_BORDER;
                opdfCellDireito.PaddingBottom = 3f;
                oPdfDir.AddCell(opdfCellDireito);

                opdfCellDireito = new PdfPCell(new Phrase("NR35: ", oFontForValue)) { Colspan = 7 };
                opdfCellDireito.HorizontalAlignment = Element.ALIGN_LEFT;
                opdfCellDireito.VerticalAlignment = Element.ALIGN_CENTER;
                opdfCellDireito.Border = Rectangle.NO_BORDER;
                opdfCellDireito.PaddingBottom = 3f;
                oPdfDir.AddCell(opdfCellDireito);

                opdfCellDireito = new PdfPCell(new Phrase(carteirinha.NR35Vencimento, oFontForValue)) { Colspan = 11 };
                opdfCellDireito.HorizontalAlignment = Element.ALIGN_LEFT;
                opdfCellDireito.VerticalAlignment = Element.ALIGN_CENTER;
                opdfCellDireito.Border = Rectangle.NO_BORDER;
                opdfCellDireito.PaddingBottom = 3f;
                oPdfDir.AddCell(opdfCellDireito);

                opdfCellDireito = new PdfPCell(new Phrase(" ", oFontForValue)) { Colspan = 2 };
                opdfCellDireito.HorizontalAlignment = Element.ALIGN_LEFT;
                opdfCellDireito.VerticalAlignment = Element.ALIGN_CENTER;
                opdfCellDireito.Border = Rectangle.NO_BORDER;
                opdfCellDireito.PaddingBottom = 3f;
                oPdfDir.AddCell(opdfCellDireito);

                opdfCellDireito = new PdfPCell(new Phrase("Treinamento OSD: ", oFontForValue)) { Colspan = 7 };
                opdfCellDireito.HorizontalAlignment = Element.ALIGN_LEFT;
                opdfCellDireito.VerticalAlignment = Element.ALIGN_CENTER;
                opdfCellDireito.Border = Rectangle.NO_BORDER;
                opdfCellDireito.PaddingBottom = 3f;
                oPdfDir.AddCell(opdfCellDireito);

                opdfCellDireito = new PdfPCell(new Phrase(carteirinha.CDDSVencimento, oFontForValue)) { Colspan = 11 };
                opdfCellDireito.HorizontalAlignment = Element.ALIGN_LEFT;
                opdfCellDireito.VerticalAlignment = Element.ALIGN_CENTER;
                opdfCellDireito.Border = Rectangle.NO_BORDER;
                opdfCellDireito.PaddingBottom = 3f;
                oPdfDir.AddCell(opdfCellDireito);

                opdfCellDireito = new PdfPCell(new Phrase(" ", oFontForValue)) { Colspan = 2 };
                opdfCellDireito.HorizontalAlignment = Element.ALIGN_LEFT;
                opdfCellDireito.VerticalAlignment = Element.ALIGN_CENTER;
                opdfCellDireito.Border = Rectangle.NO_BORDER;
                opdfCellDireito.PaddingBottom = 3f;
                oPdfDir.AddCell(opdfCellDireito);

                opdfCellDireito = new PdfPCell(new Phrase("Emiss�o: " + carteirinha.DataEmissao, oFontForAssinatura)) { Colspan = 20 };
                opdfCellDireito.HorizontalAlignment = Element.ALIGN_CENTER;
                opdfCellDireito.VerticalAlignment = Element.ALIGN_CENTER;
                opdfCellDireito.Border = Rectangle.NO_BORDER;
                opdfCellDireito.PaddingBottom = 3f;
                oPdfDir.AddCell(opdfCellDireito);

                opdfCellDireito = new PdfPCell(new Phrase(" ", oFontForAssinatura)) { Colspan = 20 };
                opdfCellDireito.HorizontalAlignment = Element.ALIGN_LEFT;
                opdfCellDireito.VerticalAlignment = Element.ALIGN_CENTER;
                opdfCellDireito.Border = Rectangle.NO_BORDER;
                opdfCellDireito.PaddingBottom = 1f;
                oPdfDir.AddCell(opdfCellDireito);

                opdfCellDireito = new PdfPCell(new Phrase("Documento emitido pela Ra�zen atrav�s do sistema de cadastro para uso exclusivo em suas instala��es", oFontForAssinatura)) { Colspan = 20 };
                opdfCellDireito.HorizontalAlignment = Element.ALIGN_CENTER;
                opdfCellDireito.VerticalAlignment = Element.ALIGN_BOTTOM;
                opdfCellDireito.Border = Rectangle.NO_BORDER;
                opdfCellDireito.PaddingTop = 10f;
                oPdfDir.AddCell(opdfCellDireito);


                opdfCellDireito = new PdfPCell(oPdfDir) { Colspan = 20 };
                opdfCellDireito.HorizontalAlignment = Element.ALIGN_CENTER;
                opdfCellDireito.VerticalAlignment = Element.ALIGN_CENTER;
                opdfCellDireito.Border = Rectangle.NO_BORDER;
                opdfCellDireito.BackgroundColor = oBrancoBackground;
                opdfCellDireito.PaddingBottom = 6f;
                oPdfPTable.AddCell(opdfCellDireito);

                //Cria a celula master que ir� abrigar a tabela formul�rio.
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


                Rectangle rectangle = new Rectangle(oDocument.PageSize.Width - 10, 840, 10, 640);
                rectangle.Left += oDocument.LeftMargin;
                rectangle.Right -= oDocument.RightMargin;
                rectangle.Top -= oDocument.TopMargin;
                rectangle.Bottom += oDocument.BottomMargin;
                rectangle.Border = Rectangle.RECTANGLE;
                //content.SetColorFill(oRoxoBackgroundHeader);
                content.SetColorStroke(BaseColor.BLACK);
                content.Rectangle(rectangle.Left, rectangle.Bottom, rectangle.Width, rectangle.Height);
                content.FillStroke();

                //content.RestoreState();

                PdfContentByte content1 = pdfWriter.DirectContent;
                Rectangle rectangle1 = new Rectangle(249, 816, 249, 670);
                rectangle1.Left += oDocument.LeftMargin + 48;
                rectangle1.Right -= oDocument.RightMargin - 48;
                rectangle1.Top -= oDocument.TopMargin;
                rectangle1.Bottom += oDocument.BottomMargin;
                rectangle1.Border = Rectangle.NO_BORDER;
                content1.SetColorFill(BaseColor.BLACK);
                content1.SetColorStroke(BaseColor.BLACK);
                content1.Rectangle(rectangle1.Left, rectangle1.Bottom, rectangle1.Width, rectangle1.Height);
                content1.FillStroke();

                Rectangle foto = new Rectangle(20, 780, 111, 663);
                foto.Border = Rectangle.RECTANGLE;
                //content.SetColorFill(oRoxoBackgroundHeader);
                content.SetColorStroke(BaseColor.BLACK);
                content.Rectangle(foto.Left, foto.Bottom, foto.Width, foto.Height);
                content.FillStroke();

                Rectangle ass1 = new Rectangle(144, 670, 276, 670);
                ass1.Border = Rectangle.NO_BORDER;
                //content.SetColorFill(oRoxoBackgroundHeader);
                content.SetColorStroke(BaseColor.BLACK);
                content.Rectangle(ass1.Left, ass1.Bottom, ass1.Width, ass1.Height);
                content.FillStroke();


                #endregion
                oDocument.Close();
                return memoryStream;
            }

            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            //finally
            //{
            //    //Fecha o documento
            //    oDocument.Close();
            //}
        }

        public bool VerificarAlteracoesApenasTelefoneEmail(Motorista motorista, bool naoVerificarCliente = false)
        {
            bool result = false;
            string nomeMetodo = "VerificarAlteracoesApenasTelefoneEmail";
            MotoristaAlteradoView alteracoes = new MotoristaAlteradoView();
            try
            {
                if (motorista.IDMotorista.HasValue)
                {
                    var motoristaAnterior = new MotoristaPesquisaBusiness().Selecionar(motorista.IDMotorista.Value).Mapear();

                    if (motoristaAnterior != null)
                    {
                        alteracoes = this.CarregarAlteracoes(motorista, naoVerificarCliente);

                        if (motorista.IDTransportadora != motoristaAnterior.IDTransportadora)
                        {
                            _logar?.Invoke(nomeMetodo, string.Format("IDTransportadora alterado -  atual:{0} , anterior: {1} ", motorista.IDTransportadora, motoristaAnterior.IDTransportadora));
                            return true;
                        }

                        if (!string.Equals(motorista.Observacao, motoristaAnterior.Observacao))
                        {
                            _logar?.Invoke(nomeMetodo, string.Format("Observacao alterado -  atual:{0} , anterior: {1} ", motorista.Observacao, motoristaAnterior.Observacao));
                            return true;
                        }

                        var listDocAtual = motorista.Documentos;

                        var listDocAnaterior = new MotoristaDocumentoBusiness().ListarMotoristaDocumentoPorMotorista(motoristaAnterior.ID);
                        motoristaAnterior.Documentos = listDocAnaterior;

                        if (listDocAtual.Count != listDocAnaterior.Count)
                        {
                            _logar?.Invoke(nomeMetodo, string.Format("Lista Documentos alterado -  atual:{0} , anterior: {1} ", listDocAtual.Count, listDocAnaterior.Count));
                            return true;
                        }


                        Type type = typeof(MotoristaDocumentoView);

                        var listPropertyInfo = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

                        listDocAtual = listDocAtual.OrderBy(o => o.Descricao).ToList();
                        listDocAnaterior = listDocAnaterior.OrderBy(o => o.Descricao).ToList();


                        //var countAnexoAtual = listDocAtual.Where(w => w.Anexo != null).ToList().Count();
                        //if (countAnexoAtual > 0)
                        //{
                        //    _logar?.Invoke(nomeMetodo, "Lista Documentos alterado." );
                        //    return true;
                        //}


                        var docAtual = listDocAtual.Select(s => s.Anexo).ToList();
                        var docAnterior = listDocAnaterior.Select(s => s.Anexo).ToList();

                        var naoExisteAtual = docAtual.Except(docAnterior).ToList();
                        var naoExisteAnterior = docAnterior.Except(docAtual).ToList();

                        if (naoExisteAtual.Count > 0 || naoExisteAnterior.Count > 0 || (docAtual.Count != docAnterior.Count))
                        {
                            _logar?.Invoke(nomeMetodo, "Lista Documentos alterado.");
                            return true;
                        }


                        for (int i = 0; i < listDocAtual.Count; i++)
                        {
                            foreach (PropertyInfo propertyInfo in listPropertyInfo)
                            {
                                var prop = propertyInfo;

                                var valorAtual = prop.GetValue(listDocAtual[i]);
                                var valorAnterior = prop.GetValue(listDocAnaterior[i]);

                                if (!string.Equals(valorAnterior, valorAtual))
                                {
                                    var propName = prop.Name;

                                    if (!string.Equals(propName, "DocumentoIdentificacao") && !string.Equals(propName, "CPF")
                                        && !string.Equals(propName, "Pendente") && !string.Equals(propName, "ID")
                                        && !string.Equals(propName, "Pendente") && !string.Equals(propName, "IDMotorista"))
                                    {
                                        _logar?.Invoke(nomeMetodo, string.Format("{0} alterado -  atual:{1} , anterior: {2} ", propName, listDocAtual.Count, listDocAnaterior.Count));
                                        return true;
                                    }
                                }
                            }
                        }

                        foreach (PropertyInfo propertyInfo in motorista.GetType().GetProperties())
                        {
                            var prop = propertyInfo;

                            var valorAtual = prop.GetValue(motorista);
                            var valorAnterior = prop.GetValue(motoristaAnterior);

                            if (!string.Equals(valorAnterior, valorAtual))
                            {
                                var propName = prop.Name;

                                if (!string.Equals(propName, "MotoristaBrasil") && !string.Equals(propName, "ID")
                                     && !string.Equals(propName, "DataAtualizazao") && !string.Equals(propName, "Ativo")
                                     && !string.Equals(propName, "Telefone") && !string.Equals(propName, "IdPais")
                                     && !string.Equals(propName, "UsuarioAlterouStatus")
                                     && !string.Equals(propName, "Documentos") && !string.Equals(propName, "Clientes")
                                     && !string.Equals(propName, "MotoristaBrasil") && !string.Equals(propName, "Email")
                                     && !string.Equals(propName, "IDMotorista") && !string.Equals(propName, "UsuarioAlterouStatus")
                                     && !string.Equals(propName, "IDStatus") && !string.Equals(propName, "CodigoSalesForce")
                                     && !string.Equals(propName, "CodigoSalesForce") && !string.Equals(propName, "CodigoEasyQuery")
                                     && !string.Equals(propName, "EmailSolicitante") && !string.Equals(propName, "LoginUsuario"))
                                     //UsuarioAlterouStatus CodigoEasyQuery
                                {
                                    _logar?.Invoke(nomeMetodo, string.Format("{0} alterado -  atual:{1} , anterior: {2} ", propName, listDocAtual.Count, listDocAnaterior.Count));
                                    return true;
                                }

                            }
                        }


                        foreach (PropertyInfo propertyInfo in alteracoes.GetType().GetProperties())
                        {

                            if (propertyInfo.Name.StartsWith("Is"))
                            {
                                var prop = propertyInfo;

                                var valor = (Boolean)prop.GetValue(alteracoes);

                                if (valor)
                                {
                                    var propName = prop.Name;

                                    if (!string.Equals(propName, "IsCPFAlterado") && !string.Equals(propName, "IsTelefoneAlterado") && !string.Equals(propName, "IsEmailAlterado"))
                                    {
                                        _logar?.Invoke(nomeMetodo, string.Format("{0} alterado -  atual:{1} , anterior: {2} ", propName, listDocAtual.Count, listDocAnaterior.Count));
                                        return true;
                                    }
                                }

                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                var error = ex.Message;
                throw;
            }

            return result;
        }


        private MotoristaAlteradoView CarregarAlteracoes(Motorista motorista, bool naoVerificarCliente = false)
        {
            MotoristaAlteradoView result = new MotoristaAlteradoView();

            if (motorista.IDMotorista.HasValue)
            {
                var motoristaAnterior = new MotoristaPesquisaBusiness().Selecionar(motorista.IDMotorista.Value).Mapear();

                if (motoristaAnterior != null)
                {
                    result = this.CarregarAlteracoes(motorista, motoristaAnterior, naoVerificarCliente);
                }
            }

            return result;
        }



        public string EnviarDadosQuickTAS(Motorista motorista)
        {
            string result = string.Empty;

            string cpf = motorista.MotoristaBrasil.CPF;
            string telefone = motorista.Telefone;

            TransportadoraBusiness transportadoraBusiness = new TransportadoraBusiness();

            try
            {

                if (_configBusiness.GetConfigInt(EnumConfig.habilitarEnvioQuickTasCIF, (int)EnumPais.Padrao) == 0 && motorista.Operacao == "CIF")
                {
                    return string.Empty;
                }
                else
                    if (_configBusiness.GetConfigInt(EnumConfig.habilitarEnvioQuickTasFOB, (int)EnumPais.Padrao) == 0 && motorista.Operacao == "FOB")
                {
                    return string.Empty;
                }

                string emailTransportadora = string.Empty;

                if (motorista.IDTransportadora.HasValue)
                {
                    var transportadora = transportadoraBusiness.Selecionar(motorista.IDTransportadora.Value);
                    emailTransportadora = transportadora != null ? transportadoraBusiness.SelecionarEmail(transportadora.CNPJCPF) : string.Empty;
                }

                if (!string.IsNullOrEmpty(telefone))
                {
                    telefone = "55" + motorista.Telefone.Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", "");
                }

                //motorista.MotoristaBrasil.CPF
                if (!string.IsNullOrEmpty(cpf))
                {
                    if (!cpf.Contains(".") && cpf.Length == 11)
                    {

                        cpf = string.Format("{0}.{1}.{2}-{3}",
                           motorista.MotoristaBrasil.CPF.Substring(0, 3),
                           motorista.MotoristaBrasil.CPF.Substring(3, 3),
                           motorista.MotoristaBrasil.CPF.Substring(6, 3),
                           motorista.MotoristaBrasil.CPF.Substring(9, 2));
                    }
                }
                                        
                var emailRementePadrao = ConfigurationManager.AppSettings["EmailRemetentePadrao"].ToString();

                UsersAAWebDTO usersAA = new UsersAAWebDTO
                {
                    Celular = telefone,
                    Email = motorista.Email,
                    CPF = cpf,
                    EmailTransportodora = emailRementePadrao,
                    Name = motorista.Nome,
                };

                ExternalServices.CallAPIExternalService callAPIExternalService = new ExternalServices.CallAPIExternalService();


                var jsonBody = JsonSerializer.Serialize(usersAA);

                RequestConfig requestConfig = new RequestConfig
                {
                    JsonBody = usersAA,
                    Method = "POST"
                };

                var path = Config.GetConfig(EnumConfig.UrlApiAAWeb, (int)EnumPais.Padrao);

                requestConfig.BaseUrl = path;

                result = callAPIExternalService.CallApiRestSharp(requestConfig);

                if (result != null)
                {
                    var jResult = JsonDocument.Parse(result); //JObject.Parse(result);

                    result = jResult != null ? jResult.ToString() : ""; //jResult["value"].Value<string>() : "";

                    return "QuickTas:" + result;
                }

            }
            catch (Exception ex)
            {
                var exc = new Exception(result);
                new RaizenException("Erro ao chamar API do QuickTas", ex).LogarErro();
                new RaizenException(result, exc).LogarErro();
                return "Erro ao enviar dados para o QuickTAS";
            }

            return result;
        }

        public MotoristaAlteradoView CarregarAlteracoes(Motorista motoristaAtual, Motorista motoristaAnterior, bool naoVerificarCliente = false)
        {
            MotoristaClienteBusiness motoristaClienteBusiness = new MotoristaClienteBusiness();
            MotoristaAlteradoView motoristaAlteracoes = new MotoristaAlteradoView();

            motoristaAlteracoes.IsAnexoAlterado = motoristaAtual.Anexo != motoristaAnterior.Anexo;
            motoristaAlteracoes.IsEmailAlterado = motoristaAtual.Email != motoristaAnterior.Email;
            motoristaAlteracoes.IsEmpresaAlterado = motoristaAtual.IDEmpresa != motoristaAnterior.IDEmpresa;
            motoristaAlteracoes.IsNomeAlterado = motoristaAtual.Nome != motoristaAnterior.Nome;
            motoristaAlteracoes.IsOperacaoAlterado = motoristaAtual.Operacao != motoristaAnterior.Operacao;


            if (motoristaAnterior.Telefone != null)
            {
                motoristaAlteracoes.IsTelefoneAlterado = motoristaAtual.Telefone.Replace("(", string.Empty).Replace(")", string.Empty).Replace("-", string.Empty).Replace(" ", string.Empty)
                    != motoristaAnterior.Telefone.Replace("(", string.Empty).Replace(")", string.Empty).Replace("-", string.Empty);
            }

            motoristaAlteracoes.IsPisAlterado = motoristaAtual.PIS != motoristaAnterior.PIS;
            motoristaAlteracoes.IsTransportadoraAlterado = motoristaAtual.IDTransportadora != motoristaAnterior.IDTransportadora;

            if (motoristaAnterior.IdPais == EnumPais.Brasil)
            {
                motoristaAlteracoes.IsCategoriaCNHAlterado = motoristaAtual.MotoristaBrasil.CategoriaCNH != motoristaAnterior.MotoristaBrasil.CategoriaCNH;
                motoristaAlteracoes.IsCNHAlterado = motoristaAtual.MotoristaBrasil.CNH != motoristaAnterior.MotoristaBrasil.CNH;
                motoristaAlteracoes.IsCPFAlterado = motoristaAtual.MotoristaBrasil.CPF.Replace("-", string.Empty).Replace(".", string.Empty) != motoristaAnterior.MotoristaBrasil.CPF.Replace("-", string.Empty).Replace(".", string.Empty);
                motoristaAlteracoes.IsLocalNascimentoAlterado = motoristaAtual.MotoristaBrasil.LocalNascimento != motoristaAnterior.MotoristaBrasil.LocalNascimento;
                motoristaAlteracoes.IsNascimentoAlterado = motoristaAtual.MotoristaBrasil.Nascimento != motoristaAnterior.MotoristaBrasil.Nascimento;
                motoristaAlteracoes.IsOrgaoEmissorAlterado = motoristaAtual.MotoristaBrasil.OrgaoEmissor != motoristaAnterior.MotoristaBrasil.OrgaoEmissor;
                motoristaAlteracoes.IsOrgaoEmissorCNHAlterado = motoristaAtual.MotoristaBrasil.OrgaoEmissorCNH != motoristaAnterior.MotoristaBrasil.OrgaoEmissorCNH;
                motoristaAlteracoes.IsRGAlterado = motoristaAtual.MotoristaBrasil.RG != motoristaAnterior.MotoristaBrasil.RG;

                if (motoristaAlteracoes.IsCategoriaCNHAlterado)
                    motoristaAlteracoes.CategoriaCNHAlterado = motoristaAnterior.MotoristaBrasil.CategoriaCNH;

                if (motoristaAlteracoes.IsCNHAlterado)
                    motoristaAlteracoes.CNHAlterado = motoristaAnterior.MotoristaBrasil.CNH;

                if (motoristaAlteracoes.IsCPFAlterado)
                    motoristaAlteracoes.CPFAlterado = motoristaAnterior.MotoristaBrasil.CPF;

                if (motoristaAlteracoes.IsLocalNascimentoAlterado)
                    motoristaAlteracoes.LocalNascimentoAlterado = motoristaAnterior.MotoristaBrasil.LocalNascimento;

                if (motoristaAlteracoes.IsOrgaoEmissorAlterado)
                    motoristaAlteracoes.OrgaoEmissorAlterado = motoristaAnterior.MotoristaBrasil.OrgaoEmissor;

                if (motoristaAlteracoes.IsOrgaoEmissorCNHAlterado)
                    motoristaAlteracoes.OrgaoEmissorCNHAlterado = motoristaAnterior.MotoristaBrasil.OrgaoEmissorCNH;

                if (motoristaAlteracoes.IsNascimentoAlterado)
                    motoristaAlteracoes.NascimentoAlterado = motoristaAnterior.MotoristaBrasil.Nascimento != null ? motoristaAnterior.MotoristaBrasil.Nascimento.Value.ToShortDateString() : string.Empty;

                if (motoristaAlteracoes.IsRGAlterado)
                    motoristaAlteracoes.RGAlterado = motoristaAnterior.MotoristaBrasil.RG;
            }

            if (motoristaAnterior.IdPais == EnumPais.Argentina)
            {
                motoristaAlteracoes.IsDNIAlterado = motoristaAtual.MotoristaArgentina.DNI != motoristaAnterior.MotoristaArgentina.DNI;
                motoristaAlteracoes.IsLNCAlterado = motoristaAtual.MotoristaArgentina.LicenciaNacionalConducir != motoristaAnterior.MotoristaArgentina.LicenciaNacionalConducir;
                motoristaAlteracoes.IsLNHAlterado = motoristaAtual.MotoristaArgentina.LicenciaNacionalHabilitante != motoristaAnterior.MotoristaArgentina.LicenciaNacionalHabilitante;
                motoristaAlteracoes.IsLNHAlterado = motoristaAtual.MotoristaArgentina.LicenciaNacionalHabilitante != motoristaAnterior.MotoristaArgentina.LicenciaNacionalHabilitante;
                motoristaAlteracoes.IsCUITAlterado = motoristaAtual.MotoristaArgentina.CUITTransportista != motoristaAnterior.MotoristaArgentina.CUITTransportista;
                motoristaAlteracoes.IsTarjetalterado = motoristaAtual.MotoristaArgentina.Tarjeta != motoristaAnterior.MotoristaArgentina.Tarjeta;

                if (motoristaAlteracoes.IsDNIAlterado)
                    motoristaAlteracoes.DNIAlterado = motoristaAnterior.MotoristaArgentina.DNI;

                if (motoristaAlteracoes.IsLNCAlterado)
                    motoristaAlteracoes.LNCAlterado = motoristaAnterior.MotoristaArgentina.LicenciaNacionalConducir;

                if (motoristaAlteracoes.IsLNHAlterado)
                    motoristaAlteracoes.LNHAlterado = motoristaAnterior.MotoristaArgentina.LicenciaNacionalHabilitante;

                if (motoristaAlteracoes.IsCUITAlterado)
                    motoristaAlteracoes.CUITAlterado = motoristaAnterior.MotoristaArgentina.CUITTransportista;

                if (motoristaAlteracoes.IsTarjetalterado)
                    motoristaAlteracoes.TarjetaAlterado = motoristaAnterior.MotoristaArgentina.Tarjeta;
            }

            if (motoristaAlteracoes.IsEmpresaAlterado)
                motoristaAlteracoes.EmpresaAlterado = EnumExtensions.GetDescription((EnumEmpresa)motoristaAnterior.IDEmpresa);

            if (motoristaAlteracoes.IsPisAlterado)
                motoristaAlteracoes.PisAlterado = motoristaAnterior.PIS;

            if (motoristaAlteracoes.IsAnexoAlterado)
                motoristaAlteracoes.AnexoAlterado = motoristaAnterior.Anexo;

            if (motoristaAlteracoes.IsEmailAlterado)
                motoristaAlteracoes.EmailAlterado = motoristaAnterior.Email;

            if (motoristaAlteracoes.IsNomeAlterado)
                motoristaAlteracoes.NomeAlterado = motoristaAnterior.Nome;

            if (motoristaAlteracoes.IsOperacaoAlterado)
                motoristaAlteracoes.OperacaoAlterado = motoristaAnterior.Operacao;

            if (motoristaAlteracoes.IsTelefoneAlterado)
                motoristaAlteracoes.TelefoneAlterado = motoristaAnterior.Telefone;

            if (motoristaAlteracoes.IsTransportadoraAlterado)
                motoristaAlteracoes.TransportadoraAlterado = motoristaAnterior.IDTransportadora != null ? new TransportadoraBusiness().Selecionar(motoristaAnterior.IDTransportadora != null ? motoristaAnterior.IDTransportadora.Value : -1).RazaoSocial : string.Empty;


            if (!naoVerificarCliente)
            {
				motoristaAnterior.Clientes = motoristaClienteBusiness.ListarClientes(motoristaAnterior.ID);

				if (motoristaAnterior.Clientes != null && motoristaAtual.Clientes != null)
				{
					var idsAtual = motoristaAtual.Clientes.Select(s => s.IDCliente).ToList();
					var idsAnterior = motoristaAnterior.Clientes.Select(s => s.IDCliente).ToList();

					var naoExisteAtual = idsAtual.Except(idsAnterior).ToList();
					var naoExisteAnterior = idsAnterior.Except(idsAtual).ToList();

					motoristaAlteracoes.IsClientesAlterado = (naoExisteAtual.Count > 0 || naoExisteAnterior.Count > 0 || (motoristaAnterior.Clientes.Count != motoristaAtual.Clientes.Count));
				}
			}

			return motoristaAlteracoes;
        }

        public override Motorista Selecionar(Expression<Func<Motorista, bool>> where)
        {
            var motorista = base.Selecionar(where);

            if (motorista != null)
            {
                if (_pais == EnumPais.Brasil)
                    using (var placaDerivada = new UniCadDalRepositorio<MotoristaBrasil>())
                        motorista.MotoristaBrasil = placaDerivada.Get(motorista.ID);

                if (_pais == EnumPais.Argentina)
                    using (var placaDerivada = new UniCadDalRepositorio<MotoristaArgentina>())
                        motorista.MotoristaArgentina = placaDerivada.Get(motorista.ID);
            }

            return motorista;
        }

        public override List<Motorista> Listar()
        {
            var motoristas = base.Listar();

            List<MotoristaBrasil> motoristasBrasil = new List<MotoristaBrasil>();
            if (_pais == EnumPais.Brasil)
                using (var motoristaDerivado = new UniCadDalRepositorio<MotoristaBrasil>())
                    motoristasBrasil = motoristaDerivado.List();

            List<MotoristaArgentina> motoristasArgentina = new List<MotoristaArgentina>();
            if (_pais == EnumPais.Argentina)
                using (var motoristaDerivado = new UniCadDalRepositorio<MotoristaArgentina>())
                    motoristasArgentina = motoristaDerivado.List();

            foreach (var motorista in motoristas)
            {
                if (_pais == EnumPais.Brasil)
                    motorista.MotoristaBrasil = motoristasBrasil.FirstOrDefault(m => m.IDMotorista == motorista.ID);

                if (_pais == EnumPais.Argentina)
                    motorista.MotoristaArgentina = motoristasArgentina.FirstOrDefault(m => m.IDMotorista == motorista.ID);
            }

            return motoristas;
        }

        public bool ValidarAcesso(string login, Motorista motorista)
        {
            var valido = false;

            if (motorista == null || motorista.ID == 0)
                return true;

            Usuario user = new UsuarioBusiness().Selecionar(p => p.Login == login);

            if (user != null)
            {
                if (!user.Externo)
                    return true;

                var clientesMotorista = new MotoristaClienteBusiness().Listar(p => p.IDMotorista == motorista.ID);
                var clientesUsuario = new UsuarioClienteBusiness().Listar(p => p.IDUsuario == user.ID);
                var transportadoras = new UsuarioTransportadoraBusiness().Listar(p => p.IDUsuario == user.ID);

                valido = (clientesUsuario.Any(p => clientesMotorista.Any(b => b.IDCliente == p.IDCliente)));
                if (!valido)
                    valido = (transportadoras.Any(p => p.IDTransportadora == motorista.IDTransportadora));
            }

            return valido;
        }

        private List<MotoristaServicoView> GetQueryMotoristaServico(MotoristaServicoFiltro filtro)
        {
            var paramDataAtualizacao = new SqlParameter("@DataAtualizacao", SqlDbType.DateTime);
            var paramLinhaNegocio = new SqlParameter("@LinhaNegocio", SqlDbType.Int);
            var paramOperacao = new SqlParameter("@Operacao", SqlDbType.VarChar);
            var paramCPF = new SqlParameter("@CPF", SqlDbType.VarChar);
            var paramTerminal = new SqlParameter("Terminal", SqlDbType.VarChar);


            paramDataAtualizacao.Value = filtro.DataAtualizacao;
            paramLinhaNegocio.Value = filtro.LinhaNegocio ?? (object)DBNull.Value;
            paramOperacao.Value = filtro.Operacao ?? (object)DBNull.Value;
            paramCPF.Value = filtro.CPF ?? (object)DBNull.Value;
            paramTerminal.Value = filtro.Terminal ?? (object)DBNull.Value;

            List<MotoristaServicoView> dadosRelatorio = ExecutarProcedureComRetorno<MotoristaServicoView>(
                "[dbo].[Proc_Listar_Motoristas_Servico] @LinhaNegocio,@Operacao,@CPF,@Terminal,@DataAtualizacao",
                new object[] { paramLinhaNegocio, paramOperacao, paramCPF, paramTerminal, paramDataAtualizacao });
            return dadosRelatorio;
        }

        public List<MotoristaServicoView> ListarMotoristaServico(MotoristaServicoFiltro filtro)
        {
            using (UniCadDalRepositorio<Motorista> repositorio = new UniCadDalRepositorio<Motorista>())
            {
                var resultado = GetQueryMotoristaServico(filtro);
                if (resultado != null && resultado.Any())
                    resultado.ForEach(x =>
                    {
                        x.ListaDocumentos = ListarDocumentosPorIdMotorista(x.ID, repositorio);
                        x.ListaPermissoes = ListarPermissoesPorIdMotorista(x.ID, x.Operacao, repositorio);
                        x.ListaTreinamentosPraticos = ListarTreinamentosPraticosPorIdMotorista(x.ID, filtro.Terminal, repositorio);
                    });
                return resultado;
            }
        }

        private List<MotoristaTreinamentoPraticoServicoView> ListarTreinamentosPraticosPorIdMotorista(int Id, string Terminal, UniCadDalRepositorio<Motorista> repositorio)
        {
            var query = (from app in repositorio.ListComplex<Motorista>().AsNoTracking().OrderBy(i => i.ID)
                         join treinamentoTerminal in repositorio.ListComplex<MotoristaTreinamentoTerminal>().AsNoTracking() on app.ID equals treinamentoTerminal.IDMotorista
                         join terminal in repositorio.ListComplex<Terminal>().AsNoTracking() on treinamentoTerminal.IDTerminal equals terminal.ID
                         where (app.ID == Id)
                         && (terminal.Sigla.Contains(string.IsNullOrEmpty(Terminal) ? terminal.Sigla : Terminal))
                         select new MotoristaTreinamentoPraticoServicoView
                         {
                             SiglaTerminal = terminal.Sigla,
                             Data = treinamentoTerminal.DataValidade.Value
                         });
            return query.ToList();
        }

        private List<MotoristaPermissaoServicoView> ListarPermissoesPorIdMotorista(int Id, string Operacao, UniCadDalRepositorio<Motorista> repositorio)
        {
            IQueryable<MotoristaPermissaoServicoView> permissoes;
            if (Operacao == "FOB")
                permissoes = (from app in repositorio.ListComplex<Motorista>().AsNoTracking().OrderBy(i => i.ID)
                              join pc in repositorio.ListComplex<MotoristaCliente>().AsNoTracking() on app.ID equals pc.IDMotorista
                              join cli in repositorio.ListComplex<Cliente>().AsNoTracking() on pc.IDCliente equals cli.ID
                              where (app.ID == Id)
                              select new MotoristaPermissaoServicoView
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
                              select new MotoristaPermissaoServicoView
                              {
                                  IBM = transp.IBM,
                                  CpfCnpj = transp.CNPJCPF,
                                  NomeRazaoSocial = transp.RazaoSocial
                              });
            }
            return permissoes.ToList();
        }

        private List<MotoristaDocumentoServicoView> ListarDocumentosPorIdMotorista(int Id, UniCadDalRepositorio<Motorista> repositorio)
        {
            var documentos = from app in repositorio.ListComplex<Motorista>().AsNoTracking().OrderBy(i => i.ID)
                             join docs in repositorio.ListComplex<MotoristaDocumento>().AsNoTracking() on app.ID equals docs.IDMotorista
                             join tipoDoc in repositorio.ListComplex<TipoDocumento>().AsNoTracking() on docs.IDTipoDocumento equals tipoDoc.ID
                             where (app.ID == Id)
                             select new MotoristaDocumentoServicoView
                             {
                                 Sigla = tipoDoc.Sigla,
                                 Descricao = tipoDoc.Descricao,
                                 DataVencimento = docs.DataVencimento,
                             };
            return documentos.ToList();
        }

        public static bool ValidarAbrirChamadoEasyQuery(Motorista motorista, int idPais)
        {
            var easy = Config.GetConfigInt(EnumConfig.EasyQuery, idPais);

            if (easy == 0)
            {
                return false;
            }

            switch (motorista.IDEmpresa)
            {
                case (int)EnumEmpresa.EAB:
                    switch (motorista.Operacao)
                    {
                        case "CIF":
                            return Config.GetConfigInt(EnumConfig.EqMotoEabCif, idPais) != 0;
                        case "FOB":
                            return Config.GetConfigInt(EnumConfig.EqMotoEabFob, idPais) != 0;
                    }
                    break;
                case (int)EnumEmpresa.Combustiveis:
                    switch (motorista.Operacao)
                    {
                        case "CIF":
                            return Config.GetConfigInt(EnumConfig.EqMotoCombCif, idPais) != 0;
                        case "FOB":
                            return Config.GetConfigInt(EnumConfig.EqMotoCombFob, idPais) != 0;
                    }
                    break;
            }

            return true;
        }

        public string ExcluirRegistro(int id, int status)
        {
            using (TransactionScope ts = Transactions.CreateTransactionScope(1, IsolationLevel.ReadCommitted))
            {
                //para esses status apenas excluir a solicita��o
                if (status == (int)EnumStatusMotorista.EmAprovacao || status == (int)EnumStatusMotorista.Reprovado)
                {
                    LimparRegistrosMotorista(id);

                    Excluir(id);
                }
                //para casos de bloqueado e aprovado dever� excluir todas as solicita��es tamb�m
                else
                {

                    Motorista moto = Selecionar(id);
                    Motorista motoIntegrarSap = new MotoristaPesquisaBusiness().Selecionar(id).Mapear();
                    do
                    {
                        LimparRegistrosMotorista(moto.ID);
                        Excluir(moto);
                        moto = Selecionar(w => w.IDMotorista == moto.ID);
                    }
                    while (moto != null);



                    //S� vai excluir no SAP o motorista j� aprovado/bloqueado
                    var retorno = IntegrarSAP(motoIntegrarSap, EnumTipoIntegracaoSAP.Excluir);

                    if (!string.IsNullOrEmpty(retorno))
                    {
                        motoIntegrarSap.Mensagem = retorno;
                        return retorno;
                    }
                }



                ts.Complete();
                return string.Empty;
            }
        }

        private void LimparRegistrosMotorista(int id)
        {
            Motorista moto = Selecionar(id);
            if (moto.Operacao == "FOB")
                using (var motoristaClienteBll = new MotoristaClienteBusiness())
                    motoristaClienteBll.ExcluirLista(w => w.IDMotorista == moto.ID);

            using (var motoristaDocumentoBll = new MotoristaDocumentoBusiness())
                motoristaDocumentoBll.ExcluirLista(w => w.IDMotorista == moto.ID);

            using (var motoristahistTreinamentoBll = new HistoricoTreinamentoTeoricoMotoristaBusiness())
                motoristahistTreinamentoBll.ExcluirLista(w => w.IDMotorista == moto.ID);

            using (var motoristahistAtivarBll = new HistorioAtivarMotoristaBusiness())
                motoristahistAtivarBll.ExcluirLista(w => w.IDMotorista == moto.ID);

            using (var motoristahistBloqueioBll = new HistorioBloqueioMotoristaBusiness())
                motoristahistBloqueioBll.ExcluirLista(w => w.IDMotorista == moto.ID);

            using (var motoristatreinamentoTerminalBll = new MotoristaTreinamentoTerminalBusiness())
                motoristatreinamentoTerminalBll.ExcluirLista(w => w.IDMotorista == moto.ID);

            using (var motoristaTipoProdutoBll = new MotoristaTipoProdutoBusiness())
                motoristaTipoProdutoBll.ExcluirLista(w => w.IDMotorista == moto.ID);

            using (var motoristaTipoComposicaoBll = new MotoristaTipoComposicaoBusiness())
                motoristaTipoComposicaoBll.ExcluirLista(w => w.IDMotorista == moto.ID);

        }

        public bool SalvarTreinamento(List<TerminalTreinamentoView> listaTerminais, TreinamentoView treinamento)
        {
            using (TransactionScope ts = Transactions.CreateTransactionScope(1, IsolationLevel.ReadCommitted))
            {

                if (treinamento.dataValidade.HasValue || treinamento.TreinamentoAprovado.HasValue && !treinamento.TreinamentoAprovado.Value)
                {
                    var historico = new HistoricoTreinamentoTeoricoMotorista();
                    historico.IDMotorista = treinamento.IDMotorista;
                    historico.Justificativa = treinamento.Justificativa;

                    var motorista = Selecionar(treinamento.IDMotorista);
                    motorista.MesesTreinamentoTeorico = Config.GetConfigInt(EnumConfig.validadeTreinamentoTeorico, (int)EnumPais.Padrao);
                    if (treinamento.TreinamentoAprovado.HasValue && treinamento.TreinamentoAprovado.Value)
                    {
                        motorista.DataValidadeTreinamento = treinamento.dataValidade.Value;
                        historico.Data = treinamento.dataValidade.Value;
                    }
                    else
                    {
                        motorista.DataValidadeTreinamento = DateTime.Now;
                        historico.Data = DateTime.Now;
                        motorista.IDStatus = (int)EnumStatusMotorista.Bloqueado;
                    }

                    historico.Anexo = treinamento.Anexo;
                    historico.TreinamentoAprovado = treinamento.TreinamentoAprovado;
                    historico.DataCadastro = DateTime.Now;
                    historico.Usuario = UserSession.GetCurrentInfoUserSystem().InformacoesUsuario.Nome;
                    historico.CodigoUsuario = UserSession.GetCurrentInfoUserSystem().InformacoesUsuario.Login;
                    new HistoricoTreinamentoTeoricoMotoristaBusiness().Adicionar(historico);

                    var docs = new MotoristaDocumentoBusiness().ListarMotoristaDocumentoPorMotorista(treinamento.IDMotorista);
                    motorista.Documentos = docs;

                    if (treinamento.TreinamentoAprovado.HasValue && !treinamento.TreinamentoAprovado.Value)
                    {
                        AtualizarMotorista(motorista);
                    }
                    else
                    {
                        IntegrarSAP(motorista, EnumTipoIntegracaoSAP.Inclusao);
                    }
                }

                var terminaisDelete = new MotoristaTreinamentoTerminalBusiness().Listar(w => w.IDMotorista == treinamento.IDMotorista);
                terminaisDelete.ForEach(x => new MotoristaTreinamentoTerminalBusiness().Excluir(x));
                if (listaTerminais != null && listaTerminais.Any())
                {
                    foreach (var item in listaTerminais)
                    {
                        MotoristaTreinamentoTerminal motoTreinamentoTerminal = new MotoristaTreinamentoTerminal();
                        motoTreinamentoTerminal.IDMotorista = treinamento.IDMotorista;
                        motoTreinamentoTerminal.IDTerminal = item.IDTerminal;
                        motoTreinamentoTerminal.DataValidade = item.dataValidade;
                        motoTreinamentoTerminal.Usuario = item.Usuario;
                        motoTreinamentoTerminal.Anexo = item.Anexo;
                        motoTreinamentoTerminal.CodigoUsuario = item.CodigoUsuario;
                        new MotoristaTreinamentoTerminalBusiness().Adicionar(motoTreinamentoTerminal);
                    }
                }
                ts.Complete();
                return true;
            }
        }
        #region AdicionarMotorista
        public bool AdicionarMotorista(Motorista motorista)
        {
            return AdicionarMotorista(motorista, null, null);
        }

        public bool AdicionarMotorista(Motorista motorista, List<int> tipoProdutoList)
        {
            return AdicionarMotorista(motorista, tipoProdutoList, null);
        }

        public bool AdicionarMotorista(Motorista motorista, List<int> tipoProdutoList, List<int> tipoComposicaoList)
        {
            using (TransactionScope ts = Transactions.CreateTransactionScope(1, IsolationLevel.ReadCommitted))
            {
                Adicionar(motorista);

                if (!motorista.Clientes.IsNullOrEmpty())
                {
                    MotoristaClienteBusiness mcBLL = new MotoristaClienteBusiness();

                    foreach (var cliente in motorista.Clientes)
                    {
                        MotoristaCliente mc = new MotoristaCliente();
                        mc.IDMotorista = motorista.ID;
                        mc.IDCliente = cliente.IDCliente;
                        mc.DataAprovacao = cliente.DataAprovacao;
                        mcBLL.Adicionar(mc);
                    }
                }

                if (!motorista.Documentos.IsNullOrEmpty())
                {
                    MotoristaDocumentoBusiness mdBLL = new MotoristaDocumentoBusiness();

                    var documentosExistentes = mdBLL.Listar(w => w.IDMotorista == motorista.ID && motorista.IdPais == _pais).Select(w => w.IDTipoDocumento);

                    //Adiciona Documentos
                    foreach (var documento in motorista.Documentos)
                    {
                        if (!documentosExistentes.Contains(documento.IDTipoDocumento))
                        {
                            MotoristaDocumento md = new MotoristaDocumento();
                            md.Anexo = documento.Anexo;
                            md.DataVencimento = documento.DataVencimento;
                            md.IDMotorista = motorista.ID;
                            md.IDTipoDocumento = documento.IDTipoDocumento;
                            mdBLL.Adicionar(md);
                            documento.ID = md.ID;
                        }
                    }
                }

                if (tipoProdutoList != null)
                {
                    var mtpBLL = new MotoristaTipoProdutoBusiness();

                    var tiposExistentes = mtpBLL.Listar(w => w.IDMotorista == motorista.ID && motorista.IdPais == _pais).Select(w => w.IDTipoProduto);

                    foreach (var idTipoProduto in tipoProdutoList)
                    {
                        if (!tiposExistentes.Contains(idTipoProduto))
                        {
                            var motoristaTipoProduto = new MotoristaTipoProduto
                            {
                                IDMotorista = motorista.ID,
                                IDTipoProduto = idTipoProduto
                            };

                            mtpBLL.Adicionar(motoristaTipoProduto);
                        }
                    }
                }

                if (tipoComposicaoList != null)
                {
                    var mtcBLL = new MotoristaTipoComposicaoBusiness();

                    var tiposExistentes = mtcBLL.Listar(w => w.IDMotorista == motorista.ID && motorista.IdPais == _pais).Select(w => w.IDTipoComposicao);

                    foreach (var idTipoComposicao in tipoComposicaoList)
                    {
                        if (!tiposExistentes.Contains(idTipoComposicao))
                        {
                            var motoristaTipoComposicao = new MotoristaTipoComposicao
                            {
                                IDMotorista = motorista.ID,
                                IDTipoComposicao = idTipoComposicao
                            };

                            mtcBLL.Adicionar(motoristaTipoComposicao);
                        }
                    }
                }

                if (motorista.IDMotorista.HasValue)
                {
                    HistorioBloqueioMotoristaBusiness bllb = new HistorioBloqueioMotoristaBusiness();
                    var historicos = bllb.Listar(p => p.IDMotorista == motorista.IDMotorista);
                    foreach (var item in historicos)
                    {
                        item.ID = 0;
                        item.IDMotorista = motorista.ID;
                        bllb.Adicionar(item);
                    }

                    HistoricoTreinamentoTeoricoMotoristaBusiness bll = new HistoricoTreinamentoTeoricoMotoristaBusiness();
                    var historicoTeorico = bll.Listar(p => p.IDMotorista == motorista.IDMotorista);
                    foreach (var item in historicoTeorico)
                    {
                        item.ID = 0;
                        item.IDMotorista = motorista.ID;
                        bll.Adicionar(item);
                    }

                    MotoristaTreinamentoTerminalBusiness bllt = new MotoristaTreinamentoTerminalBusiness();
                    var historicoTerminal = bllt.Listar(p => p.IDMotorista == motorista.IDMotorista);
                    foreach (var item in historicoTerminal)
                    {
                        item.ID = 0;
                        item.IDMotorista = motorista.ID;
                        bllt.Adicionar(item);
                    }
                }

                var idPais = (int)motorista.IdPais;

                //verifica se para esse tipo de empresa e opera��o o par�metro est� setado como true
                if (ValidarAbrirChamadoEasyQuery(motorista, idPais))
                {
                    AbrirChamadoEasyQuery(motorista, idPais);
                }

                if (ValidarAbrirChamadoSalesForce(motorista, idPais))
                {
                    AbrirChamadoSalesForce(motorista);
                }

                Atualizar(motorista);

                GravaTabelasEspecificas(motorista);
				ts.Complete();
			}

            var _motoristaDocumento = new MotoristaDocumentoBusiness();
			if (motorista.IDMotorista.HasValue && motorista.IDStatus == (int)EnumStatusMotorista.EmAprovacao && !VerificarAlteracoesApenasTelefoneEmail(motorista, naoVerificarCliente: true) && !_motoristaDocumento.VerificarDocumentosVencidosPorMotorista(motorista.ID))
            {
                var motoristaAprovar = Selecionar(motorista.ID);

                motoristaAprovar.IDStatus = (int)EnumStatusMotorista.Aprovado;
                motoristaAprovar.Documentos = new MotoristaDocumentoBusiness().ListarDocumentos(motoristaAprovar.ID);
                motoristaAprovar.Clientes = new MotoristaClienteBusiness().ListarMotoristaClientePorMotorista(motoristaAprovar.ID, motoristaAprovar.IDEmpresa);

				var resultadoAprovar = AtualizarMotorista(motoristaAprovar, comRessalvas: false, bloqueio: false, aprovacaoAutomatica: true);

                if (resultadoAprovar)
                {
                    motorista.Mensagem = "APROVACAO_AUTOMATICA";
                }

                return resultadoAprovar;
            }

            return true;
		}
		#endregion

		public bool TransportadoraArgentinaValida(Motorista motorista)
        {
            string cuit = ObtemTrasnportadoraCUIT(motorista);

            Transportadora transp = new TransportadoraBusiness(motorista.IdPais).BuscarTranportadora(cuit, motorista.Operacao, motorista.IDEmpresa);

            return transp != null;
        }

        private string ObtemTrasnportadoraCUIT(Motorista motorista)
        {
            //Se o motorista for do tipo FOB, ser� utilizado o CUIT da aba "Dados generales"
            if (motorista.Operacao == "FOB")
                return motorista.MotoristaArgentina.CUITTransportista;

            //Se o motorista for do tipo CIF, ser� utilizado o CUIT da trasnportadora selecionada na aba "Permiso"
            if (motorista.Operacao == "CIF")
            {
                if (motorista.IDTransportadora == null && motorista.IDTransportadora.HasValue)
                    return string.Empty;

                return new TransportadoraBusiness().Selecionar((int)motorista.IDTransportadora).CNPJCPF;
            }

            return string.Empty;
        }

        private void GravaTabelasEspecificas(Motorista motorista)
        {
            switch (_pais)
            {
                case EnumPais.Brasil:
                    AdicionarMotoristaBrasil(motorista.MotoristaBrasil, motorista.ID);
                    break;
                case EnumPais.Argentina:
                    AdicionarMotoristaArgentina(motorista.MotoristaArgentina, motorista.ID);
                    break;
            }
        }

        private void AtualizaTabelasEspecificas(Motorista motorista)
        {
            switch (motorista.IdPais)
            {
                case EnumPais.Brasil:
                    AtualizarMotoristaBrasil(motorista.MotoristaBrasil, motorista.ID);
                    break;
                case EnumPais.Argentina:
                    AtualizarMotoristaArgentina(motorista.MotoristaArgentina, motorista.ID);
                    break;
            }
        }

        private void AdicionarMotoristaArgentina(MotoristaArgentina motoristaArgentina, int idMotorista)
        {
            using (UniCadDalRepositorio<MotoristaArgentina> motoristaArgentinaRepositorio = new UniCadDalRepositorio<MotoristaArgentina>())
            {
                motoristaArgentina.IDMotorista = idMotorista;
                motoristaArgentina.DNI = motoristaArgentina.DNI.RemoveSpecialCharacters();

                motoristaArgentinaRepositorio.Add(motoristaArgentina);
            }
        }

        private void AdicionarMotoristaBrasil(MotoristaBrasil motoristaBrasil, int idMotorista)
        {
            using (UniCadDalRepositorio<MotoristaBrasil> motoristaBrasilRepositorio = new UniCadDalRepositorio<MotoristaBrasil>())
            {
                motoristaBrasil.IDMotorista = idMotorista;
                motoristaBrasil.CPF = motoristaBrasil.CPF.RemoveSpecialCharacters();

                motoristaBrasilRepositorio.Add(motoristaBrasil);
            }
        }

        private void AtualizarMotoristaArgentina(MotoristaArgentina motoristaArgentina, int idMotorista)
        {
            using (UniCadDalRepositorio<MotoristaArgentina> motoristaArgentinaRepositorio = new UniCadDalRepositorio<MotoristaArgentina>())
            {
                motoristaArgentina.IDMotorista = idMotorista;
                motoristaArgentina.DNI = motoristaArgentina.DNI.RemoveSpecialCharacters();

                motoristaArgentinaRepositorio.Update(motoristaArgentina);
            }
        }

        private void AtualizarMotoristaBrasil(MotoristaBrasil motoristaBrasil, int idMotorista)
        {
            using (UniCadDalRepositorio<MotoristaBrasil> motoristaBrasilRepositorio = new UniCadDalRepositorio<MotoristaBrasil>())
            {
                motoristaBrasil.IDMotorista = idMotorista;
                motoristaBrasil.CPF = motoristaBrasil.CPF.RemoveSpecialCharacters().Replace(".", "");

                motoristaBrasilRepositorio.Update(motoristaBrasil);
            }
        }

        #region AtualizarMotorista

        public bool AlterarTelefoneEmailMotorista(Motorista motorista)
        {
            return Atualizar(motorista);
        }

        public bool AtualizarMotorista(Motorista motorista, bool comRessalvas, bool bloqueio = false)
        {
            return AtualizarMotorista(motorista, comRessalvas, bloqueio, false);
        }

        public bool AtualizarMotorista(Motorista motorista,
                                       bool comRessalvas = false,
                                       bool bloqueio = false,
                                       bool desativar = false,
                                       bool enviaEmail = true,
                                       List<TipoProduto> tipoProdutoList = null,
                                       List<TipoComposicao> tipoComposicaoList = null,
									   bool aprovacaoAutomatica = false)
        {
            bool novaAprovacao = false;

            Motorista motoristaAntiga = null;

            if (motorista.IDMotorista.HasValue)
            {
                motoristaAntiga = Selecionar(motorista.IDMotorista.Value);
            }

            using (var ts = Transactions.CreateTransactionScope(TransactionScopeOption.Required, IsolationLevel.ReadCommitted, 5))
            {
                if (motorista.Telefone != null)
                {
                    //atualizar o status
                    if (motorista.Telefone.Length > 11 && motorista.Telefone.StartsWith("55"))
                    {
                        motorista.Telefone = motorista.Telefone.Remove(0, 2);
                    }
                }
                Atualizar(motorista);
                if (motorista.IDStatus == (int)EnumStatusMotorista.Aprovado && motorista.IDMotorista.HasValue)
                {

                    if (motoristaAntiga != null)
                    {
                        motoristaAntiga.IDStatus = 3;

                        motoristaAntiga.Email = motorista.Email;
                        motoristaAntiga.Telefone = motorista.Telefone;

                        Atualizar(motoristaAntiga);
                        novaAprovacao = true;
                    }
                }

                motorista.IBMTransportadora = ObtemTransportadoraIBM(motorista);

                //caso o usu�rio mude a opera��o 
                if (motorista.Operacao == "FOB")
                    motorista.IDTransportadora = null;

                //verifica se para esse tipo de empresa e opera��o o par�metro est� setado como true
                var abrirEq = ValidarAbrirChamadoEasyQuery(motorista, (int)motorista.IdPais);
                var abrirSf = ValidarAbrirChamadoSalesForce(motorista, (int)motorista.IdPais, novaAprovacao, aprovacaoAutomatica);


                if (abrirSf)
                {
                    AbrirChamadoSalesForce(motorista, aprovacaoAutomatica);
                }

                Atualizar(motorista);

                AtualizaTabelasEspecificas(motorista);

                new MotoristaClienteBusiness().Listar(w => w.IDMotorista == motorista.ID).ForEach(x => new MotoristaClienteBusiness().Excluir(x));
                if (!motorista.Clientes.IsNullOrEmpty())
                {
                    MotoristaClienteBusiness mcBLL = new MotoristaClienteBusiness();
                    DateTime? dataAprovacao = null;

                    if (motorista.IDStatus == (int)EnumStatusMotorista.Aprovado)
                    {
                        dataAprovacao = DateTime.Now;
                    }

                    foreach (var cliente in motorista.Clientes)
                    {
                        MotoristaCliente mc = new MotoristaCliente();
                        mc.IDMotorista = motorista.ID;
                        mc.IDCliente = cliente.IDCliente;
                        mc.DataAprovacao = cliente.DataAprovacao ?? dataAprovacao;

                        mcBLL.Adicionar(mc);
                    }
                }

                #region Documentos
                MotoristaDocumentoBusiness mdBLL = new MotoristaDocumentoBusiness();
                var documentosExistentes = mdBLL.Listar(w => w.IDMotorista == motorista.ID && motorista.IdPais == _pais);
                //Atualizar a data de vencimento/anexo dos documentos
                if (!motorista.Documentos.IsNullOrEmpty())
                {

                    foreach (var documento in motorista.Documentos)
                    {
                        //Se n�o existe, adiciona o documento
                        if (!documentosExistentes.Select(w => w.IDTipoDocumento).Contains(documento.IDTipoDocumento))
                        {
                            MotoristaDocumento md = new MotoristaDocumento();
                            md.Anexo = documento.Anexo;
                            md.DataVencimento = documento.DataVencimento;
                            md.IDMotorista = motorista.ID;
                            md.IDTipoDocumento = documento.IDTipoDocumento;
                            mdBLL.Adicionar(md);
                            documento.ID = md.ID;
                        }

                        //Se existe, atualiza o documento
                        else
                        {
                            MotoristaDocumento md = new MotoristaDocumento();

                            md.ID = documento.ID == 0 ? documentosExistentes.First(w => w.IDTipoDocumento == documento.IDTipoDocumento).ID : documento.ID;
                            md.Anexo = documento.Anexo;
                            md.DataVencimento = documento.DataVencimento;
                            md.IDMotorista = motorista.ID;
                            md.IDTipoDocumento = documento.IDTipoDocumento;
                            md.Alerta1Enviado = documento.Alerta1Enviado;
                            md.Alerta2Enviado = documento.Alerta2Enviado;
                            md.Vencido = documento.Vencido;
                            md.Bloqueado = documento.Bloqueado;
                            md.Processado = documento.Processado;
                            md.UsuarioAlterouStatus = documento.UsuarioAlterouStatus;
                            if (motorista.IDStatus == (int)EnumStatusMotorista.Aprovado)
                            {
                                md.Processado = false;
                            }
                            mdBLL.Atualizar(md);
                            documento.ID = md.ID;
                        }
                    }

                    //Exclui Documentos n�o relacionados
                    foreach (var tipoDocumento in documentosExistentes)
                    {
                        if (!motorista.Documentos.Select(w => w.IDTipoDocumento).Contains(tipoDocumento.IDTipoDocumento))
                            mdBLL.Excluir(tipoDocumento.ID);
                    }
                }
                else
                {
                    foreach (var tipoDocumento in documentosExistentes)
                        mdBLL.ExcluirLista(w => w.IDMotorista == motorista.ID);
                }

                #endregion

                #region Tipos de Produto

                var mtpBLL = new MotoristaTipoProdutoBusiness();
                var tiposExistentes = mtpBLL.Listar(w => w.IDMotorista == motorista.ID).Select(w => w.IDTipoProduto);

                if (tipoProdutoList != null)
                {

                    foreach (var idTipoProduto in tipoProdutoList.Select(w => w.ID))
                    {
                        if (!tiposExistentes.Contains(idTipoProduto))
                        {
                            var motoristaTipoProduto = new MotoristaTipoProduto
                            {
                                IDMotorista = motorista.ID,
                                IDTipoProduto = idTipoProduto
                            };

                            mtpBLL.Adicionar(motoristaTipoProduto);
                        }
                    }

                    foreach (var tipoExistente in tiposExistentes)
                    {
                        if (!tipoProdutoList.Select(w => w.ID).Contains(tipoExistente))
                            mtpBLL.ExcluirLista(w => w.IDTipoProduto == tipoExistente && w.IDMotorista == motorista.ID);
                    }
                }
                else
                {
                    mtpBLL.ExcluirLista(w => w.IDMotorista == motorista.ID);
                }

                #endregion

                #region Tipos de Composi��o

                var mtcBLL = new MotoristaTipoComposicaoBusiness();
                tiposExistentes = mtcBLL.Listar(w => w.IDMotorista == motorista.ID).Select(w => w.IDTipoComposicao);

                if (tipoComposicaoList != null)
                {

                    foreach (var idTipoComposicao in tipoComposicaoList.Select(w => w.ID))
                    {
                        if (!tiposExistentes.Contains(idTipoComposicao))
                        {
                            var motoristaTipoComposicao = new MotoristaTipoComposicao
                            {
                                IDMotorista = motorista.ID,
                                IDTipoComposicao = idTipoComposicao
                            };

                            mtcBLL.Adicionar(motoristaTipoComposicao);
                        }
                    }

                    foreach (var tipoExistente in tiposExistentes)
                    {
                        if (!tipoComposicaoList.Select(w => w.ID).Contains(tipoExistente))
                            mtcBLL.ExcluirLista(w => w.IDTipoComposicao == tipoExistente && w.IDMotorista == motorista.ID);
                    }
                }
                else
                {
                    mtcBLL.ExcluirLista(w => w.IDMotorista == motorista.ID);
                }

                #endregion


                if (!bloqueio && (motorista.IDStatus == (int)EnumStatusMotorista.Aprovado || motorista.IDStatus == (int)EnumStatusMotorista.Reprovado) && enviaEmail)
                {
                    var motoristaDocBLL = new MotoristaDocumentoBusiness();
                    var tipoDocBLL = new TipoDocumentoBusiness();
                    if (comRessalvas)
                    {

                        int diasVenc = 10;
                        var dias = Config.GetConfig(EnumConfig.SomarDiasRessalvaDocumentos, (int)EnumPais.Padrao);
                        if (!string.IsNullOrEmpty(dias))
                            diasVenc = Convert.ToInt32(dias);

                        motorista.Documentos = motoristaDocBLL.ListarMotoristaDocumentoPorMotorista(motorista.ID);
                        if (motorista.Documentos != null && motorista.Documentos.Any())
                            motorista.Documentos.ForEach(w => w.DataVencimento = DateTime.Now.AddDays(diasVenc).Date);
                    }

                    if (motorista.Documentos != null)
                    {
                        foreach (var item in motorista.Documentos)
                        {
                            var documento = motoristaDocBLL.Selecionar(item.ID);
                            var tipoDoc = tipoDocBLL.Selecionar(documento.IDTipoDocumento);
                            documento.DataVencimento = item.DataVencimento;
                            item.MesesValidade = tipoDoc.MesesValidade.HasValue ? tipoDoc.MesesValidade.Value : 0;
                            if (comRessalvas)
                                motoristaDocBLL.Atualizar(documento);
                        }
                    }

                }
                string retorno = string.Empty;
                if (motorista.IDStatus != (int)EnumStatusComposicao.Reprovado && motorista.IDStatus != (int)EnumStatusMotorista.EmAprovacao)
                    retorno = IntegrarSAP(motorista, EnumTipoIntegracaoSAP.Inclusao);

                if (!string.IsNullOrEmpty(retorno))
                {
                    motorista.Mensagem = retorno;
                    return false;
                }

                if (!bloqueio && (motorista.IDStatus == (int)EnumStatusMotorista.Aprovado || motorista.IDStatus == (int)EnumStatusMotorista.Reprovado) && enviaEmail)
                {
                    EnviarEmailAlertaSituacao(motorista, (int)motorista.IdPais);
				}

				if ((motorista.IDStatus == (int)EnumStatusMotorista.Aprovado || motorista.IDStatus == (int)EnumStatusMotorista.Reprovado) && ValidarEncerrarChamadoSalesForce(motorista))
				{
					EncerrarChamadoSalesForce(motorista, comRessalvas);
				}

				if (motorista.IDStatus == (int)EnumStatusMotorista.Aprovado && ValidarEnviarDadosCSOnline(motorista))
                {
                    CriarOuAtualizarMotoristaNoCsOnline(motorista);
					VincularClienteSAP(motorista);
				}

				ts.Complete();

                return true;
            }
        }

		private void VincularClienteSAP(Motorista motorista)
		{
            if (motorista.MotoristaBrasil == null)
            {
                return;
            }

			MotoristaClienteBusiness motoristaClienteBusiness = new MotoristaClienteBusiness();

			var clientes = motoristaClienteBusiness.ListarClientes(motorista.ID);

			var clienteMotorista = clientes
				.Select(m => new VincularClienteRequestClienteMotorista
				{
		            cliente = m.IBM,
		            cpf = motorista.MotoristaBrasil.CPF,
		            cancelado = "",
	            })
	            .ToArray();

			WsIntegraSAP integraSAP = new WsIntegraSAP();
			var retorno = integraSAP.VincularClienteSap(clienteMotorista);
		}

		private void CriarOuAtualizarMotoristaNoCsOnline(Motorista motorista)
        {
            var wsCsonline = new WsCsonline(
                ConfigurationManager.AppSettings["CsOnlineEndpoint"],
                ConfigurationManager.AppSettings["CsOnlineOrigin"],
                ConfigurationManager.AppSettings["CsOnlinePin"],
                ConfigurationManager.AppSettings["CsOnlineApplicationKey"]
            );
            wsCsonline.CriarOuAtualizarMotorista(CriaRequisicaoCsonlineMotorista(motorista));
        }

        private CsonlineDriverUpdateRequestView CriaRequisicaoCsonlineMotorista(Motorista motorista)
        {
			MotoristaClienteBusiness motoristaClienteBusiness = new MotoristaClienteBusiness();
			
            var documentoMOP = motorista.Documentos.Where(d => d.Sigla == "MOPP").FirstOrDefault();
            var documentoNR20 = motorista.Documentos.Where(d => d.Sigla == "NR20").FirstOrDefault();
            var documentoNR35 = motorista.Documentos.Where(d => d.Sigla == "NR35").FirstOrDefault();

            var clientes = motoristaClienteBusiness.ListarClientes(motorista.ID);
            var customers = clientes.Select(c => c.IBM).ToList();

            return new CsonlineDriverUpdateRequestView
            {
                Driver = new CsonlineDriverRequestView
                {
                    Name = motorista.Nome,
                    Cpf = motorista.MotoristaBrasil.CPF,
                    DtMOPEExpire = documentoMOP?.DataVencimento,
                    DtNR20Expire = documentoNR20?.DataVencimento,
                    DtNR35Expire = documentoNR35?.DataVencimento,
                },
                Customers = customers,
            };
        }

        private bool ValidarEnviarDadosCSOnline(Motorista motorista)
        {
            var ativaIntegracaoCsonline = Config.GetConfigInt(EnumConfig.AtivaIntegracaoCsonline, (int)motorista.IdPais);

            return ativaIntegracaoCsonline != 0 && motorista.IdPais == EnumPais.Brasil && motorista.MotoristaBrasil != null && motorista.Operacao == "FOB";
        }

        public override Motorista Selecionar(int id)
        {
            var motorista = base.Selecionar(id);

            if (motorista != null)
            {
                if (motorista.IdPais == EnumPais.Brasil)
                    using (UniCadDalRepositorio<MotoristaBrasil> motoristaDerivado = new UniCadDalRepositorio<MotoristaBrasil>())
                        motorista.MotoristaBrasil = motoristaDerivado.Get(id);

                if (motorista.IdPais == EnumPais.Argentina)
                    using (UniCadDalRepositorio<MotoristaArgentina> motoristaDerivado = new UniCadDalRepositorio<MotoristaArgentina>())
                        motorista.MotoristaArgentina = motoristaDerivado.Get(id);
            }

            return motorista;
        }

        private string ObtemTransportadoraIBM(Motorista motorista)
        {
            string transportadoraIbm = string.Empty;

            switch (_pais)
            {
                case EnumPais.Brasil:
                    {
                        if (motorista.IDTransportadora.HasValue)
                            transportadoraIbm = new TransportadoraBusiness().Selecionar(motorista.IDTransportadora.Value)?.IBM;
                        break;
                    }

                case EnumPais.Argentina:
                    {
                        string cuit = ObtemTrasnportadoraCUIT(motorista);

                        if (!string.IsNullOrWhiteSpace(cuit))
                            transportadoraIbm = new TransportadoraBusiness().Selecionar(c => c.CNPJCPF == cuit && c.Operacao == motorista.Operacao && c.IdPais == (int)_pais)?.IBM;
                        break;
                    }
            }

            return transportadoraIbm;
        }

        #endregion
        private void EnviarEmailAlertaSituacao(Motorista motorista, int idPais)
        {
            string ibm = string.Empty;
            string razaoSocial = string.Empty;
            Usuario usuario = new UsuarioBusiness().Selecionar(w => w.Login == motorista.LoginUsuario && w.Status);
            if (usuario != null)
            {
                if (motorista.Operacao == "FOB")
                {
                    UsuarioCliente usuarioCliente = new UsuarioClienteBusiness().Selecionar(w => w.IDUsuario == usuario.ID);
                    if (usuarioCliente != null)
                    {
                        Cliente cliente = new ClienteBusiness().Selecionar(usuarioCliente.IDCliente);
                        ibm = cliente.IBM;
                        razaoSocial = cliente.RazaoSocial;
                    }
                }
                else
                {
                    UsuarioTransportadora usuarioTransportadora = new UsuarioTransportadoraBusiness().Selecionar(w => w.IDUsuario == usuario.ID);
                    if (usuarioTransportadora != null)
                    {
                        Transportadora Transportadora = new TransportadoraBusiness().Selecionar(usuarioTransportadora.IDTransportadora);
                        ibm = Transportadora.IBM;
                        razaoSocial = Transportadora.RazaoSocial;
                    }
                }

                StringBuilder corpoEmail = new StringBuilder();

                string assunto;
                string documento = "";
                string mascara = "";

                switch (_pais)
                {
                    case EnumPais.Brasil:
                        documento = motorista.MotoristaBrasil.CPF;
                        mascara = @"000\.000\.000\-00";
                        break;

                    case EnumPais.Argentina:
                        documento = motorista.MotoristaArgentina.DNI;
                        mascara = @"00\.000\.000";
                        break;
                }

                // Motorista Aprovado
                if (motorista.IDStatus == (int)EnumStatusMotorista.Aprovado)
                {
                    assunto = Config.GetConfig(EnumConfig.TituloMotoristaAprovado, idPais);
                    corpoEmail.AppendFormat(Config.GetConfig(EnumConfig.CorpoMotoristaAprovado, idPais), motorista.Nome, Convert.ToUInt64(documento).ToString(mascara));
                }
                // Motorista Reprovado
                else
                {
                    assunto = Config.GetConfig(EnumConfig.TituloMotoristaReprovado, idPais);
                    corpoEmail.AppendFormat(Config.GetConfig(EnumConfig.CorpoMotoristaReprovado, idPais), motorista.Nome, Convert.ToUInt64(documento).ToString(mascara));
                }

                Email.Enviar(usuario.Email, assunto, corpoEmail.ToString());
            }
        }

        private string IntegrarSAP(Motorista motorista, EnumTipoIntegracaoSAP tipoIntegracao)
        {
            var transp = new TransportadoraBusiness().Selecionar(p => p.ID == motorista.IDTransportadora);
            if (transp != null)
            {
                motorista.NomeTransportadora = transp.RazaoSocial;
            }

            string retorno = "";
            if (motorista.IDEmpresa == (int)EnumEmpresa.Combustiveis)
            {
                switch (_pais)
                {
                    case EnumPais.Argentina:
                        WsIntegraSAPAR_Motorista integraSAPAR_Motorista = new WsIntegraSAPAR_Motorista();
                        retorno = integraSAPAR_Motorista.IntegrarMotorista(motorista, tipoIntegracao);
                        break;

                    case EnumPais.Brasil:
                        WsIntegraSAP integraSAP = new WsIntegraSAP();
                        retorno = integraSAP.IntegrarMotorista(motorista, tipoIntegracao);
                        break;
                }
            }
            else if (_pais == EnumPais.Brasil)
            {
                WsIntegraSAPEAB integraSAP = new WsIntegraSAPEAB();
                retorno = integraSAP.IntegrarMotorista(motorista, tipoIntegracao);
            }

            return retorno;
        }

        public List<MotoristaView> ListarMotorista(MotoristaFiltro filtro)
        {

            using (UniCadDalRepositorio<Motorista> repositorio = new UniCadDalRepositorio<Motorista>())
            {
                var resultado = GetQueryMotorista(filtro, false, false);
                if (resultado != null && resultado.Any())
                    resultado.ForEach(x => x.IsClonar = !_motoristaPesquisaBll.Listar(w => w.CPF == x.CPF && x.IDEmpresa != w.IDEmpresa).Any());
                return resultado;
            }
        }

        public List<MotoristaView> ListarMotorista(MotoristaFiltro filtro, PaginadorModel paginador)
        {

            using (UniCadDalRepositorio<Motorista> repositorio = new UniCadDalRepositorio<Motorista>())
            {
                int ultimaPagina = paginador.QtdeItensPagina * paginador.PaginaAtual;
                var resultado = GetQueryMotorista(filtro, false, false, paginador.InicioPaginacao, ultimaPagina);

                if (resultado != null && resultado.Any())
                    resultado.ForEach(x => x.IsClonar = !_motoristaPesquisaBll.Listar(w => w.CPF == x.CPF && x.IDEmpresa != w.IDEmpresa).Any());
                return resultado;
            }
        }

        public int ListarMotoristaCount(MotoristaFiltro filtro)
        {
            using (UniCadDalRepositorio<Motorista> repositorio = new UniCadDalRepositorio<Motorista>())
            {
                var dados = GetQueryMotorista(filtro, true, false);
                return dados[0].Linhas;
            }
        }

        public Stream Exportar(MotoristaFiltro filtro)
        {
            var resultado = new DataTable();

            if (_pais == EnumPais.Brasil)
                resultado = GetQueryMotoristaExcel(filtro, false, true);
            else
                resultado = GetQueryMotoristaExcelArgentina(filtro, false, true);

            Stream fs = new MemoryStream();
            var Workbook = new XLWorkbook();
            var worksheet = Workbook.Worksheets.Add("Motorista");

            var tipoDocs =
                new TipoDocumentoBusiness().Listar(w => w.tipoCadastro == (int)EnumTipoCadastroDocumento.Motorista && w.IDPais == (int)_pais);

            int numeroColunas = MontarColunasMotorista(worksheet, tipoDocs, filtro.UsuarioExterno);

            int linha = 2;
            {
                foreach (DataRow item in resultado.Rows)
                {
                    MonstarLinhasMotorista(worksheet, linha, item, tipoDocs, filtro.UsuarioExterno);
                    linha++;
                }
            }

            using (IXLRange range = worksheet.Range(2, 1, linha - 1, numeroColunas))
            {
                Excel.DesenharBorda(range);
            }

            Workbook.SaveAs(fs, false);
            fs.Position = 0;

            return fs;
        }

        private void MonstarLinhasMotorista(IXLWorksheet worksheet, int linha, DataRow item, List<TipoDocumento> tipoDocs, bool usuarioExterno)
        {
            var numeroColunas = 0;
            switch (_pais)
            {
                case EnumPais.Brasil:

                    worksheet.Cell(linha, 1).Value = item["CPF"];
                    worksheet.Cell(linha, 1).SetDataType(XLCellValues.Text);

                    worksheet.Cell(linha, 2).Value = item["Nome"];
                    worksheet.Cell(linha, 3).Value = EnumExtensions.GetDescription((EnumEmpresa)item["IDEmpresa"]);
                    worksheet.Cell(linha, 4).Value = item["Operacao"];
                    worksheet.Cell(linha, 5).Value = item["RG"];
                    worksheet.Cell(linha, 5).SetDataType(XLCellValues.Text);
                    worksheet.Cell(linha, 6).Value = item["OrgaoEmissor"];
                    worksheet.Cell(linha, 7).Value = item["CNHMotorista"];
                    worksheet.Cell(linha, 7).SetDataType(XLCellValues.Text);
                    worksheet.Cell(linha, 8).Value = item["CategoriaCNH"];
                    worksheet.Cell(linha, 9).Value = item["OrgaoEmissorCNH"];
                    worksheet.Cell(linha, 10).Value = item["Nascimento"];
                    worksheet.Cell(linha, 10).SetDataType(XLCellValues.DateTime);
                    worksheet.Cell(linha, 11).Value = item["LocalNascimento"];
                    worksheet.Cell(linha, 12).Value = item["Telefone"];
                    worksheet.Cell(linha, 13).Value = item["Email"];
                    worksheet.Cell(linha, 14).Value = item["IBM"];
                    worksheet.Cell(linha, 14).SetDataType(XLCellValues.Text);
                    worksheet.Cell(linha, 15).Value = item["NomeCliente"];
                    worksheet.Cell(linha, 16).Value = item["PISCampo"];
                    worksheet.Cell(linha, 17).Value = EnumExtensions.GetDescription((EnumStatusComposicao)item["IDStatus"]);
                    worksheet.Cell(linha, 18).Value = item["DataValidadeTreinamentoTeorico"];
                    worksheet.Cell(linha, 19).Value = item["TreinamentoAprovadoTreinamentoTeorico"];
                    worksheet.Cell(linha, 20).Value = item["DataValidadeTreinamentoPratico"];
                    worksheet.Cell(linha, 21).Value = item["TerminalTreinamentoPratico"];

                    numeroColunas = 21;

                    break;
                case EnumPais.Argentina:
                    worksheet.Cell(linha, 1).Value = item["DNI"];
                    worksheet.Cell(linha, 1).SetDataType(XLCellValues.Text);
                    worksheet.Cell(linha, 2).Value = item["Apellido"];
                    worksheet.Cell(linha, 3).Value = item["Nome"];
                    worksheet.Cell(linha, 4).Value = item["Operacao"];
                    worksheet.Cell(linha, 5).Value = item["LicenciaNacionalConducir"];
                    worksheet.Cell(linha, 6).Value = item["LicenciaNacionalHabilitante"];
                    worksheet.Cell(linha, 7).Value = item["CUITTransportista"];
                    worksheet.Cell(linha, 8).Value = item["Tarjeta"];
                    worksheet.Cell(linha, 9).Value = item["IBM"];
                    worksheet.Cell(linha, 10).Value = item["NomeCliente"];
                    worksheet.Cell(linha, 11).Value = EnumExtensions.GetDescription((EnumStatusComposicaoArg)item["IDStatus"]);
                    worksheet.Cell(linha, 12).Value = item["VctoLicenciaNacionalConducir"];
                    worksheet.Cell(linha, 13).Value = item["VctoLicenciaNacionalHabilitante"];
                    worksheet.Cell(linha, 14).Value = item["VctoARTPoliza"];
                    worksheet.Cell(linha, 15).Value = item["VctoARTCuota"];
                    worksheet.Cell(linha, 16).Value = item["VctoSSMAManejoDefensivo"];
                    worksheet.Cell(linha, 17).Value = item["VctoSSMARespuestaEmergencia"];
                    worksheet.Cell(linha, 18).Value = item["VctoSSMAPoleticas"];
                    worksheet.Cell(linha, 19).Value = item["VctoSeguroVidaCuota"];
                    worksheet.Cell(linha, 20).Value = item["VctoSeguroVidaPoliza"];
                    worksheet.Cell(linha, 21).Value = item["DataAtualizazao"];
                    worksheet.Cell(linha, 22).Value = item["UsuarioAlterouStatus"];

                    numeroColunas = 22;

                    break;
            }

            if (_pais == EnumPais.Brasil)
            {
                if (!usuarioExterno)
                {
                    numeroColunas++;
                    worksheet.Cell(linha, numeroColunas).Value = item["UsuarioAlterouStatus"];
                }

                for (int i = 0; i < tipoDocs.Count; i++)
                {
                    worksheet.Cell(linha, numeroColunas + i + 1).Value = item[tipoDocs[i].Sigla];
                }
            }

            worksheet.Row(linha).AdjustToContents();
        }

        private int MontarColunasMotorista(IXLWorksheet worksheet, List<TipoDocumento> tipoDocs, bool usuarioExterno)
        {
            var nomeColunas = new List<string>();
            switch (_pais)
            {
                case EnumPais.Argentina:
                    nomeColunas.Add("DNI");
                    nomeColunas.Add("Apellido");
                    nomeColunas.Add("Nombre");
                    nomeColunas.Add("Operaci�n");
                    nomeColunas.Add("Licencia Nac. Conducir");
                    nomeColunas.Add("Licencia.Nac. Habilitante");
                    nomeColunas.Add("Cuit del Transportista");
                    nomeColunas.Add("Tarjeta de Acceso");
                    nomeColunas.Add("Permiso - Cod");
                    nomeColunas.Add("Permiso - Nombre");
                    nomeColunas.Add("Estado");
                    nomeColunas.Add("Vcto Licencia Nacional de Conducir");
                    nomeColunas.Add("Vcto Licencia Nacional Habilitante");
                    nomeColunas.Add("Vcto ART - Poliza");
                    nomeColunas.Add("Vcto ART -Cuota");
                    nomeColunas.Add("Vcto SSMA Manejo Defensivo");
                    nomeColunas.Add("Vcto SSMA Respuesta a la Emergencia");
                    nomeColunas.Add("Vcto SSMA Pol�tica");
                    nomeColunas.Add("Vcto Seguro de Vida - Cuota");
                    nomeColunas.Add("Vcto Seguro de Vida - Poliza");
                    nomeColunas.Add("Fecha de actualizacion");
                    nomeColunas.Add("Usuario");

                    break;
                case EnumPais.Brasil:

                    nomeColunas.Add("CPF");
                    nomeColunas.Add("Nome");
                    nomeColunas.Add("Linha de Neg�cio");
                    nomeColunas.Add("Opera��o");
                    nomeColunas.Add("RG");
                    nomeColunas.Add("Emissor RG");
                    nomeColunas.Add("CNH");
                    nomeColunas.Add("Categoria");
                    nomeColunas.Add("Emissor CNH");
                    nomeColunas.Add("Nascimento");
                    nomeColunas.Add("Local Nascimento");
                    nomeColunas.Add("Telefone");
                    nomeColunas.Add("E-Mail");
                    nomeColunas.Add("Permiss�o");
                    nomeColunas.Add("Nome");
                    nomeColunas.Add("PIS");
                    nomeColunas.Add("Status");
                    nomeColunas.Add("Val. Treinamento Te�rico");
                    nomeColunas.Add("Aprovado Treinamento Te�rico");
                    nomeColunas.Add("Val. Treinamento Pr�tico");
                    nomeColunas.Add("Terminal Treinamento Pr�tico");

                    break;
            }

            if (_pais == EnumPais.Brasil)
            {
                if (!usuarioExterno)
                {
                    nomeColunas.Add(Traducao.GetTextoPorLingua("�ltima Altera��o do Status(Usu�rio)", "Ultimo Cambio de Estado (Usuario)", _pais));
                }

                nomeColunas = nomeColunas.Concat(tipoDocs.Select(td => td.Descricao)).ToList();
            }

            return Excel.PreencheColunas(worksheet, nomeColunas);
        }

        private List<MotoristaView> GetQueryMotorista(MotoristaFiltro filtro, bool isCount, bool isExportar, long? paginalInicial = null, long? paginalFinal = null)
        {
            SqlParameter paramIsCount, paramIsExportar, paramPrimeiraPagina, paramUltimaPagina, paramNome, paramRg, paramCnh, paramCpf, paramAtivo, paramIdEmpresa, paramIdStatus, paramOperacao, paramChamado, paramInicio, paramFim, paramIdUsuarioTransportadora, paramIdUsuarioCliente, paramIdTransportadora, paramIdPais, paramApellido, paramIdCliente, paramDni;
            PrepararParametrosMotorista(filtro, isCount, isExportar, paginalInicial, paginalFinal, out paramIsCount, out paramIsExportar, out paramPrimeiraPagina, out paramUltimaPagina, out paramNome, out paramRg, out paramCnh, out paramCpf, out paramAtivo, out paramIdEmpresa, out paramIdStatus, out paramOperacao, out paramChamado, out paramInicio, out paramFim, out paramIdUsuarioTransportadora, out paramIdUsuarioCliente, out paramIdTransportadora, out paramIdPais, out paramApellido, out paramIdCliente, out paramDni);

            List<MotoristaView> dadosRelatorio = ExecutarProcedureComRetorno<MotoristaView>(
                "[dbo].[Proc_Pesquisa_Motorista] @IsCount,@IsExportar,@PrimeiraPagina,@UltimaPagina,@Nome,@IDEmpresa,@IDStatus,@Operacao,@IsAtivo,@CNH,@CPF,@RG,@DataInicio,@DataFim,@IDUsuarioCliente,@IDUsuarioTransportadora,@Chamado,@IDTransportadora,@IdPais,@DNI,@Apellido,@IDCliente",
                new object[] { paramIsCount, paramIsExportar, paramPrimeiraPagina, paramUltimaPagina, paramNome, paramIdEmpresa, paramIdStatus, paramOperacao, paramAtivo, paramCnh, paramCpf, paramRg, paramInicio, paramFim, paramIdUsuarioCliente, paramIdUsuarioTransportadora, paramChamado, paramIdTransportadora, paramIdPais, paramDni, paramApellido, paramIdCliente });
            return dadosRelatorio;
        }

        private static void PrepararParametrosMotorista(MotoristaFiltro filtro, bool isCount, bool isExportar, long? paginalInicial, long? paginalFinal, out SqlParameter paramIsCount, out SqlParameter paramIsExportar, out SqlParameter paramPrimeiraPagina, out SqlParameter paramUltimaPagina, out SqlParameter paramNome, out SqlParameter paramRg, out SqlParameter paramCnh, out SqlParameter paramCpf, out SqlParameter paramAtivo, out SqlParameter paramIdEmpresa, out SqlParameter paramIdStatus, out SqlParameter paramOperacao, out SqlParameter paramChamado, out SqlParameter paramInicio, out SqlParameter paramFim, out SqlParameter paramIdUsuarioTransportadora, out SqlParameter paramIdUsuarioCliente, out SqlParameter paramIdTransportadora, out SqlParameter paramIdPais, out SqlParameter paramApellido, out SqlParameter paramIdCliente, out SqlParameter paramDni)
        {
            paramIsCount = new SqlParameter("@IsCount", SqlDbType.Bit);
            paramIsExportar = new SqlParameter("@IsExportar", SqlDbType.Bit);
            paramPrimeiraPagina = new SqlParameter("@PrimeiraPagina", SqlDbType.Int);
            paramUltimaPagina = new SqlParameter("@UltimaPagina", SqlDbType.Int);
            paramNome = new SqlParameter("@Nome", SqlDbType.VarChar);
            paramRg = new SqlParameter("@RG", SqlDbType.VarChar);
            paramCnh = new SqlParameter("@CNH", SqlDbType.VarChar);
            paramCpf = new SqlParameter("@CPF", SqlDbType.VarChar);
            paramAtivo = new SqlParameter("@IsAtivo", SqlDbType.Bit);
            paramIdEmpresa = new SqlParameter("@IDEmpresa", SqlDbType.Int);
            paramIdStatus = new SqlParameter("@IDStatus", SqlDbType.Int);
            paramOperacao = new SqlParameter("@Operacao", SqlDbType.VarChar);
            paramChamado = new SqlParameter("@Chamado", SqlDbType.VarChar);
            paramInicio = new SqlParameter("@DataInicio", SqlDbType.DateTime);
            paramFim = new SqlParameter("@DataFim", SqlDbType.DateTime);
            paramIdUsuarioTransportadora = new SqlParameter("@IDUsuarioTransportadora", SqlDbType.Int);
            paramIdUsuarioCliente = new SqlParameter("@IDUsuarioCliente", SqlDbType.Int);
            paramIdTransportadora = new SqlParameter("@IDTransportadora", SqlDbType.Int);
            paramIdPais = new SqlParameter("@IdPais", SqlDbType.Int);
            paramApellido = new SqlParameter("@Apellido", SqlDbType.VarChar);
            paramIdCliente = new SqlParameter("@IDCliente", SqlDbType.VarChar);
            paramDni = new SqlParameter("@DNI", SqlDbType.VarChar);
            paramIsCount.Value = isCount;
            paramIsExportar.Value = isExportar;
            paramPrimeiraPagina.Value = paginalInicial ?? (object)DBNull.Value;
            paramUltimaPagina.Value = paginalFinal ?? (object)DBNull.Value;
            paramNome.Value = string.IsNullOrEmpty(filtro.Nome) ? (object)DBNull.Value : filtro.Nome;
            paramRg.Value = string.IsNullOrEmpty(filtro.RG) ? (object)DBNull.Value : filtro.RG;
            paramCnh.Value = string.IsNullOrEmpty(filtro.CNH) ? (object)DBNull.Value : filtro.CNH;
            paramCpf.Value = string.IsNullOrEmpty(filtro.CPF) ? (object)DBNull.Value : filtro.CPF;
            paramAtivo.Value = filtro.Ativo ?? (object)DBNull.Value;
            paramIdEmpresa.Value = filtro.IDEmpresa ?? (object)DBNull.Value;
            paramIdStatus.Value = filtro.IDStatus ?? (object)DBNull.Value;
            paramOperacao.Value = string.IsNullOrEmpty(filtro.Operacao) ? (object)DBNull.Value : filtro.Operacao;
            paramChamado.Value = string.IsNullOrEmpty(filtro.Chamado) ? (object)DBNull.Value : filtro.Chamado;
            paramInicio.Value = filtro.DataInicio ?? (object)DBNull.Value;
            paramFim.Value = filtro.DataFim ?? (object)DBNull.Value;
            paramIdUsuarioTransportadora.Value = filtro.IDUsuarioTransportadora ?? (object)DBNull.Value;
            paramIdUsuarioCliente.Value = filtro.IDUsuarioCliente ?? (object)DBNull.Value;
            paramIdTransportadora.Value = filtro.IDTransportadora ?? (object)DBNull.Value;
            paramIdPais.Value = filtro.IdPais ?? (object)DBNull.Value;
            paramDni.Value = filtro.DNI ?? (object)DBNull.Value;
            paramApellido.Value = filtro.Apellido ?? (object)DBNull.Value;
            paramIdCliente.Value = filtro.IDCliente ?? (object)DBNull.Value;
        }

        private DataTable GetQueryMotoristaExcel(MotoristaFiltro filtro, bool isCount, bool isExportar, long? paginalInicial = null, long? paginalFinal = null)
        {
            filtro.IdPais = (int)_pais;
            SqlParameter paramIsCount, paramIsExportar, paramPrimeiraPagina, paramUltimaPagina, paramNome, paramRg, paramCnh, paramCpf, paramAtivo, paramIdEmpresa, paramIdStatus, paramOperacao, paramChamado, paramInicio, paramFim, paramIdUsuarioTransportadora, paramIdUsuarioCliente, paramIdTransportadora, paramIdPais, paramApellido, paramIdCliente, paramDni;
            PrepararParametrosMotorista(filtro, isCount, isExportar, paginalInicial, paginalFinal, out paramIsCount, out paramIsExportar, out paramPrimeiraPagina, out paramUltimaPagina, out paramNome, out paramRg, out paramCnh, out paramCpf, out paramAtivo, out paramIdEmpresa, out paramIdStatus, out paramOperacao, out paramChamado, out paramInicio, out paramFim, out paramIdUsuarioTransportadora, out paramIdUsuarioCliente, out paramIdTransportadora, out paramIdPais, out paramApellido, out paramIdCliente, out paramDni);

            DataTable dadosRelatorio = ExecutarProcedureComRetornoDataTable(
                "Proc_Pesquisa_Motorista",
                new SqlParameter[] { paramIsCount, paramIsExportar, paramPrimeiraPagina, paramUltimaPagina, paramNome, paramIdEmpresa, paramIdStatus, paramOperacao, paramAtivo, paramCnh, paramCpf, paramRg, paramInicio, paramFim, paramIdUsuarioCliente, paramIdUsuarioTransportadora, paramChamado, paramIdTransportadora, paramIdPais, paramDni, paramApellido, paramIdCliente });
            return dadosRelatorio;
        }

        private DataTable GetQueryMotoristaExcelArgentina(MotoristaFiltro filtro, bool isCount, bool isExportar, long? paginalInicial = null, long? paginalFinal = null)
        {
            filtro.IdPais = (int)_pais;
            SqlParameter paramIsCount, paramIsExportar, paramPrimeiraPagina, paramUltimaPagina, paramNome, paramRg, paramCnh, paramCpf, paramAtivo, paramIdEmpresa, paramIdStatus, paramOperacao, paramChamado, paramInicio, paramFim, paramIdUsuarioTransportadora, paramIdUsuarioCliente, paramIdTransportadora, paramIdPais, paramApellido, paramIdCliente, paramDni;
            PrepararParametrosMotorista(filtro, isCount, isExportar, paginalInicial, paginalFinal, out paramIsCount, out paramIsExportar, out paramPrimeiraPagina, out paramUltimaPagina, out paramNome, out paramRg, out paramCnh, out paramCpf, out paramAtivo, out paramIdEmpresa, out paramIdStatus, out paramOperacao, out paramChamado, out paramInicio, out paramFim, out paramIdUsuarioTransportadora, out paramIdUsuarioCliente, out paramIdTransportadora, out paramIdPais, out paramApellido, out paramIdCliente, out paramDni);

            DataTable dadosRelatorio = ExecutarProcedureComRetornoDataTable(
                "Proc_Export_MotoristaArgentina",
                new SqlParameter[] { paramIsCount, paramIsExportar, paramPrimeiraPagina, paramUltimaPagina, paramNome, paramIdEmpresa, paramIdStatus, paramOperacao, paramAtivo, paramCnh, paramCpf, paramRg, paramInicio, paramFim, paramIdUsuarioCliente, paramIdUsuarioTransportadora, paramChamado, paramIdTransportadora, paramIdPais, paramDni, paramApellido, paramIdCliente });
            return dadosRelatorio;
        }

        private IQueryable<CarteirinhaView> GetQueryCarteirinha(int id, IUniCadDalRepositorio<Motorista> repositorio)
        {
            var doc = from docs in repositorio.ListComplex<MotoristaDocumento>().AsNoTracking()
                      join tipo in repositorio.ListComplex<TipoDocumento>().AsNoTracking() on docs.IDTipoDocumento equals tipo.ID
                      where docs.IDMotorista == id
                      select new MotoristaDocumentoView { DataVencimento = docs.DataVencimento, Sigla = tipo.Sigla };

            IQueryable<CarteirinhaView> query = (from app in repositorio.ListComplex<Motorista>().AsNoTracking().OrderBy(i => i.ID)
                                                 join t in repositorio.ListComplex<Transportadora>().AsNoTracking() on app.IDTransportadora equals t.ID into transp
                                                 from t1 in transp.DefaultIfEmpty()
                                                 let dataVencimentoCNH = (from cnh in doc
                                                                          where cnh.Sigla.Equals("CNH", StringComparison.OrdinalIgnoreCase)
                                                                          select cnh.DataVencimento).FirstOrDefault()
                                                 let dataVencimentoMOPP = (from mopp in doc
                                                                           where mopp.Sigla.Equals("MOPP", StringComparison.OrdinalIgnoreCase)
                                                                           select mopp.DataVencimento).FirstOrDefault()
                                                 let dataVencimentoNr20 = (from nr20 in doc
                                                                           where nr20.Sigla.Equals("NR20", StringComparison.OrdinalIgnoreCase)
                                                                           select nr20.DataVencimento).FirstOrDefault()
                                                 let dataVencimentoNr35 = (from nr35 in doc
                                                                           where nr35.Sigla.Equals("NR35", StringComparison.OrdinalIgnoreCase)
                                                                           select nr35.DataVencimento).FirstOrDefault()
                                                 let dataVencimentoCdds = (from cdds in doc
                                                                           where cdds.Sigla.Equals("CDDS", StringComparison.OrdinalIgnoreCase)
                                                                           select cdds.DataVencimento).FirstOrDefault()
                                                 where (app.ID == id)
                                                 select new CarteirinhaView
                                                 {
                                                     Nome = app.Nome,
                                                     CPF = app.MotoristaBrasil.CPF,
                                                     Transportadora = t1.RazaoSocial,
                                                     dataVencimentoCNH = dataVencimentoCNH,
                                                     dataVencimentoMOPP = dataVencimentoMOPP,
                                                     dataVencimentoNR20 = dataVencimentoNr20,
                                                     dataVencimentoNr35 = dataVencimentoNr35,
                                                     dataVencimentoCdds = dataVencimentoCdds
                                                 });
            return query;
        }

        #region [ SalesForce ]

        public static bool ValidarEncerrarChamadoSalesForce(Motorista motorista)
        {
			var salesforce = Config.GetConfigInt(EnumConfig.SalesForce, (int)motorista.IdPais);
            
			if (salesforce != 0 && !string.IsNullOrEmpty(motorista.CodigoSalesForce))
			{
				return true;
			}

            return false;
		}

		public static bool ValidarAbrirChamadoSalesForce(Motorista motorista, int idPais, bool novaAprovacao = false, bool aprovacaoAutomatica = false)
        {
            var salesforce = Config.GetConfigInt(EnumConfig.SalesForce, idPais);

            if (salesforce == 0)
            {
                return false;
            }

            if (!string.IsNullOrEmpty(motorista.CodigoSalesForce) && motorista.IDStatus == (int)EnumStatusMotorista.Aprovado && !aprovacaoAutomatica)
            {
                return false;
            }

            if (!novaAprovacao && !aprovacaoAutomatica)
            {
                if (motorista.IDStatus != (int)EnumStatusMotorista.EmAprovacao)
                {
                    return false;
                }
            }

            var tipoEmpresa = motorista.IDEmpresa == (int)EnumEmpresa.EAB ? "Eab" : "Comb";
            var config = $"SfMoto{tipoEmpresa}{motorista.Operacao}";

            EnumConfig key;
            if (!Enum.TryParse(config, true, out key))
            {
                return true;
            }

            return Config.GetConfigInt(key, idPais) != 0;
        }

        private SalesForceCriarNovoTicketView MontarDadosCriarNovoTicketSalesForce(Motorista motorista, int idPais, bool aprovadoAutomaticamente)
        {
            var usuarios = new UsuarioBusiness().Listar(x => x.Login == motorista.LoginUsuario);

            var sf = new SalesForceCriarNovoTicketView
            {
                Subject = MontarAssuntoTicketSalesForce(motorista, (EnumPais)idPais, aprovadoAutomaticamente),
                Description = MontarDescricaoTicketSalesForce(motorista, (EnumPais)idPais),
                Profile = usuarios.FirstOrDefault()?.Perfil,
                User = motorista.LoginUsuario?.RemoverZerosAEsquerda()
            };

            var tipoEmpresa = motorista.IDEmpresa == (int)EnumEmpresa.Combustiveis ? "Comb" : "EAB";

            var config = $"IDSFMoto{tipoEmpresa}{motorista.Operacao}";

            EnumConfig key;
            if (Enum.TryParse(config, true, out key))
            {
                sf.CategoriaAtendimentoId = Config.GetConfig(key, idPais);
            }

            return sf;
        }

		private SalesForceEncerrarTicketView MontarDadosEncerrarTicketSalesForce(Motorista motorista, bool comRessalvas, int idPais)
		{
            string approval = String.Empty;
            string justificativa = String.IsNullOrEmpty(motorista.Justificativa) ? String.Empty : motorista.Justificativa.Trim().Trim(Environment.NewLine.ToCharArray());

            switch (motorista.IDStatus)
            {
                case (int)EnumStatusMotorista.Aprovado:
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
                Ticket = motorista.CodigoSalesForce,
				UserCS = informacoesUsuario.Login.RemoverZerosAEsquerda(),
				UserName = informacoesUsuario.Nome,
				Approval = approval,
                Status = "Encerrado",
                CustomerResponse = justificativa,
			};

			return sf;
		}

		private string MontarAssuntoTicketSalesForce(Motorista motorista, EnumPais pais, bool aprovadoAutomaticamente)
        {
            var negocio = EnumExtensions.GetDescription((EnumEmpresa)motorista.IDEmpresa);
            var nomePais = pais == EnumPais.Argentina ? "ARG" : "BR";
			var tipo = aprovadoAutomaticamente ? "Réplica" : "Cadastro";
			return $"{tipo} {negocio} Motorista {motorista.Operacao} - {nomePais}";
        }

        private string MontarDescricaoTicketSalesForce(Motorista motorista, EnumPais pais)
        {
            var cpf = pais == EnumPais.Argentina ? motorista.MotoristaArgentina.DNI : motorista.MotoristaBrasil.CPF;

            return new StringBuilder()
                .AppendLine($"CPF: {cpf}")
                .AppendLine($"Nome: {motorista.Nome}")
                .AppendLine($"Telefone: {motorista.Telefone}")
                .AppendLine($"e-mail: {motorista.Email}")
                .AppendLine("Link UNICAD: https://apps.raizen.com/unicad/Motorista")
                .ToString();
        }

        private void AbrirChamadoSalesForce(Motorista motorista, bool aprovadoAutomaticamente = false)
        {
            ///ConfigurationManager.AppSettings["SIGLA_APP"];

            var urlSalesforce = ConfigurationManager.AppSettings["LinkIntegracaoSalesForce"];
            var clientIdSalesforce = ConfigurationManager.AppSettings["clientIdSalesForce"];
            var clientSecretSalesforce = ConfigurationManager.AppSettings["clientSecretSalesForce"];

            motorista.CodigoSalesForce = new WsSalesForce(urlSalesforce, clientIdSalesforce, clientSecretSalesforce)
                .CriarNovoTicket(MontarDadosCriarNovoTicketSalesForce(motorista, (int)motorista.IdPais, aprovadoAutomaticamente));
            motorista.CodigoEasyQuery = null;
        }

        private void EncerrarChamadoSalesForce(Motorista motorista, bool comRessalvas)
        {
			var urlSalesforce = ConfigurationManager.AppSettings["LinkIntegracaoSalesForce"];
			var clientIdSalesforce = ConfigurationManager.AppSettings["clientIdSalesForce"];
			var clientSecretSalesforce = ConfigurationManager.AppSettings["clientSecretSalesForce"];

			new WsSalesForce(urlSalesforce, clientIdSalesforce, clientSecretSalesforce)
				.EncerrarTicket(MontarDadosEncerrarTicketSalesForce(motorista, comRessalvas, (int)motorista.IdPais));
		}

		#endregion

		#region [ Easy Query ]

		private bool NecessarioGerarNovoTicket(Motorista motorista, Motorista motoristaDB)
        {
            if (motorista.MotoristaBrasil.CategoriaCNH != motoristaDB.MotoristaBrasil.CategoriaCNH ||
                motorista.MotoristaBrasil.CNH != motoristaDB.MotoristaBrasil.CNH ||
                motorista.MotoristaBrasil.CPF != motoristaDB.MotoristaBrasil.CPF ||
                motorista.Email != motoristaDB.Email ||
                motorista.IDEmpresa != motoristaDB.IDEmpresa ||
                motorista.IDStatus != motoristaDB.IDStatus ||
                motorista.IDTransportadora != motoristaDB.IDTransportadora ||
                motorista.MotoristaBrasil.LocalNascimento != motoristaDB.MotoristaBrasil.LocalNascimento ||
                motorista.MotoristaBrasil.Nascimento != motoristaDB.MotoristaBrasil.Nascimento ||
                motorista.Nome != motoristaDB.Nome ||
                motorista.Operacao != motoristaDB.Operacao ||
                motorista.MotoristaBrasil.OrgaoEmissor != motoristaDB.MotoristaBrasil.OrgaoEmissor ||
                motorista.MotoristaBrasil.OrgaoEmissorCNH != motoristaDB.MotoristaBrasil.OrgaoEmissorCNH ||
                motorista.MotoristaBrasil.RG != motoristaDB.MotoristaBrasil.RG ||
                motorista.Telefone != motoristaDB.Telefone)
            {
                return true;
            }

            return true;
        }

        private void AbrirChamadoEasyQuery(Motorista motorista, int idPais)
        {
            motorista.CodigoEasyQuery = new EasyQueryBusiness().CriarNovoTicket(MontarDadosTicket(motorista, idPais));
            motorista.CodigoSalesForce = null;
        }

        private EasyQueryView MontarDadosTicket(Motorista motorista, int idPais)
        {
            var eq = new EasyQueryView
            {
                ContactEmail = motorista.EmailSolicitante,
                Subject = Config.GetConfig(EnumConfig.TituloChamadoEasyQueryMotorista, idPais)
            };

            switch (idPais)
            {
                case (int)EnumPais.Brasil:
                    eq.CustomerCode = motorista.MotoristaBrasil.CPF;
                    eq.Description = MontarDescricaoTicket(motorista);
                    break;
                case (int)EnumPais.Argentina:
                    eq.CustomerCode = motorista.MotoristaArgentina.DNI;
                    eq.Description = MontarDescricaoTicketArgentina(motorista);
                    break;
            }

            if (motorista.Operacao == "FOB")
            {
                eq.ResolutionGroupID = Config.GetConfigInt(EnumConfig.ResolutionGroupMotoristaId, idPais);

                if (motorista.IDEmpresa == (int)EnumEmpresa.Combustiveis)
                {
                    eq.idSubcategoria = Config.GetConfigInt(EnumConfig.SubCategoryMotoristaCombId, idPais);
                }
                else if (motorista.IDEmpresa == (int)EnumEmpresa.EAB)
                {
                    eq.idSubcategoria = Config.GetConfigInt(EnumConfig.SubCategoryMotoristaEabId, idPais);
                }
            }
            else if (motorista.Operacao == "CIF")
            {
                //CIF
                eq.ResolutionGroupID = Config.GetConfigInt(EnumConfig.ResolutionGroupMotoristaIdCIF, idPais);

                if (motorista.IDEmpresa == (int)EnumEmpresa.Combustiveis)
                {
                    eq.idSubcategoria = Config.GetConfigInt(EnumConfig.SubCategoryMotoristaCombIdCIF, idPais);
                }
                else if (motorista.IDEmpresa == (int)EnumEmpresa.EAB)
                {
                    eq.idSubcategoria = Config.GetConfigInt(EnumConfig.SubCategoryMotoristaEabId, idPais);
                }
            }

            return eq;
        }

        private string MontarDescricaoTicket(Motorista motorista)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("CPF: " + motorista.MotoristaBrasil.CPF);
            sb.AppendLine("Nome: " + motorista.Nome);
            sb.AppendLine("Linha de Neg�cio: " + EnumExtensions.GetDescription((EnumEmpresa)motorista.IDEmpresa));
            sb.AppendLine("Opera��o: " + motorista.Operacao);
            sb.AppendLine("RG: " + motorista.MotoristaBrasil.RG);
            sb.AppendLine("Emissor RG: " + motorista.MotoristaBrasil.OrgaoEmissor);
            sb.AppendLine("CNH: " + motorista.MotoristaBrasil.CNH);
            sb.AppendLine("Categoria: " + motorista.MotoristaBrasil.CategoriaCNH);
            sb.AppendLine("Emissor CNH: " + motorista.MotoristaBrasil.OrgaoEmissorCNH);
            sb.AppendLine("Nascimento: " + (motorista.MotoristaBrasil.Nascimento.HasValue ? motorista.MotoristaBrasil.Nascimento.Value.ToString("dd/MM/yyyy") : string.Empty));
            sb.AppendLine("Local Nascimento: " + motorista.MotoristaBrasil.LocalNascimento);
            sb.AppendLine("Telefone: " + motorista.Telefone);
            sb.AppendLine("E-mail: " + motorista.Email);
            return sb.ToString();
        }

        private string MontarDescricaoTicketArgentina(Motorista motorista)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("Nombre: " + motorista.Nome);
            sb.AppendLine("L�nea de negocio: " + EnumExtensions.GetDescription((EnumEmpresa)motorista.IDEmpresa));
            sb.AppendLine("Operaci�n: " + motorista.Operacao);
            sb.AppendLine("DNI: " + motorista.MotoristaArgentina.DNI);
            sb.AppendLine("Licencia Nacional Conducir: " + motorista.MotoristaArgentina.LicenciaNacionalConducir);
            sb.AppendLine("Licencia Nacional Habilitante: " + motorista.MotoristaArgentina.LicenciaNacionalHabilitante);
            sb.AppendLine("N�mero de tel�fono: " + motorista.Telefone);
            sb.AppendLine("E-mail: " + motorista.Email);
            return sb.ToString();
        }

        public CarteirinhaView SelecionarMotoristaCarteirinha(int id)
        {
            using (UniCadDalRepositorio<Motorista> repositorio = new UniCadDalRepositorio<Motorista>())
            {
                IQueryable<CarteirinhaView> query = GetQueryCarteirinha(id, repositorio);
                return query.FirstOrDefault();
            }

        }

        public List<MensagemValidacaoView> ValidarMotorista(List<MotoristaAAServicoView> motoristas, MotoristaValidarFiltro motoristaFiltro)
        {
            try
            {
                List<MensagemValidacaoView> lstMessageResult = new List<MensagemValidacaoView>();

                //// Data de In�cio para considerar atualiza��es. 
                //// Consideraremos as atualiza��es � partir de um ano, at� a data atual
                //var dataHoraAtualizacao = DateTime.Now.AddYears(-1);

                if (motoristas.Count() > 0)
                {
                    foreach (var dadosMotorista in motoristas)
                    {
                        var msgValidacaoDocumento = ValidarMotoristaDocumento(dadosMotorista, motoristaFiltro);
                        if (msgValidacaoDocumento != null && msgValidacaoDocumento.Count > 0)
                            lstMessageResult.AddRange(msgValidacaoDocumento);

                        break;
                    }
                }

                if (lstMessageResult != null && lstMessageResult.Count < 1)
                {
                    lstMessageResult.Add(new MensagemValidacaoView
                    {
                        Mensagem = "Motorista validado com sucesso, sem restri��o de documento",
                        RestringirOperacao = false
                    });
                }

                return lstMessageResult;
            }
            catch (Exception ex)
            {
                throw new Exception("Falha ao validar motorista. \r\nDescri��o: " + ex.Message);
            }

        }

        private MensagemValidacaoView ValidarTreinamentoTeoricoPraticoMotorista(MotoristaAAServicoView dadosMotorista)
        {
            MensagemValidacaoView msgResult = new MensagemValidacaoView();
            if (dadosMotorista.DataTreinamentoTeorico == null)
            {
                msgResult.RestringirOperacao = true;
                msgResult.Mensagem = "Treinamento te�rico n�o realizado.";
            }
            else
            {
                if (Convert.ToDateTime(dadosMotorista.DataTreinamentoTeorico).Date < DateTime.Now.Date)
                {
                    msgResult.RestringirOperacao = true;
                    msgResult.Mensagem = "Treinamento te�rico vencido.";
                }
            }

            if (dadosMotorista.ListaTreinamentosPraticos.Count > 0)
            {
                foreach (var treinamentoPratico in dadosMotorista.ListaTreinamentosPraticos)
                {
                    if (treinamentoPratico.Data != null && Convert.ToDateTime(treinamentoPratico.Data).Date < DateTime.Now.Date)
                    {
                        msgResult.RestringirOperacao = true;
                        msgResult.Mensagem = "Treinamento pr�tico vencido.";
                        break;
                    }
                }
            }
            else
            {
                msgResult.RestringirOperacao = true;
                msgResult.Mensagem = "Treinamento pr�tico n�o realizado.";
            }
            return msgResult;
        }

        private List<MensagemValidacaoView> ValidarMotoristaDocumento(MotoristaAAServicoView dadosMotorista, MotoristaValidarFiltro motoristaFiltro)
        {
            List<MensagemValidacaoView> lstMsgResult = new List<MensagemValidacaoView>();
            MensagemValidacaoView msgResult = new MensagemValidacaoView();

            if (dadosMotorista.ListaDocumentos != null)
            {
                foreach (MotoristaDocumentoAAServicoView documento in dadosMotorista.ListaDocumentos)
                {
                    if (documento.Sigla == "NR35")
                    {
                        DateTime validatyNR35 = documento.DataVencimento.Value;
                        if (documento.DataVencimento != null && motoristaFiltro.MonthQtdNr35Driving > 0)
                            validatyNR35 = Convert.ToDateTime(documento.DataVencimento).AddYears(motoristaFiltro.MonthQtdNr35Driving / 12);
                        msgResult = FormatarMensagem(dadosMotorista.ID, dadosMotorista.Nome, documento.Sigla, validatyNR35, motoristaFiltro.WarningAdviceTime, documento.Obrigatorio);
                    }
                    else
                        msgResult = FormatarMensagem(dadosMotorista.ID, dadosMotorista.Nome, documento.Sigla, documento.DataVencimento, motoristaFiltro.WarningAdviceTime, documento.Obrigatorio);

                    if (msgResult != null && !string.IsNullOrWhiteSpace(msgResult.Mensagem))
                        lstMsgResult.Add(msgResult);
                }
            }
            return lstMsgResult;
        }

        private MensagemValidacaoView FormatarMensagem(int cMotorista, string motoristaName, string siglaDocumento, DateTime? dataVencimento, int warningAdviceTime, bool obrigatorio)
        {
            MensagemValidacaoView msgResult = new MensagemValidacaoView();
            if (dataVencimento < DateTime.Now && obrigatorio)
            {
                msgResult.Mensagem = $"{siglaDocumento} vencida desde {dataVencimento?.ToString("dd/MM/yyyy")}.";
                msgResult.RestringirOperacao = true;
            }
            else if (warningAdviceTime > 0 && dataVencimento.Value.AddDays(warningAdviceTime) < DateTime.Today)
            {
                msgResult.Mensagem = $"{siglaDocumento} vencida at� {dataVencimento?.ToString("dd/MM/yyyy")}. Continue o carregamento hoje, mas providencie a atualiza��o at� esta data.";
                msgResult.RestringirOperacao = false;
            }
            return msgResult;
        }


        private List<MotoristaDocumentoAAServicoView> ListarDocumentosAAPorIdMotorista(int Id, UniCadDalRepositorio<Motorista> repositorio)
        {
            var documentos = (from app in repositorio.ListComplex<Motorista>().AsNoTracking().OrderBy(i => i.ID)
                              join docs in repositorio.ListComplex<MotoristaDocumento>().AsNoTracking() on app.ID equals docs.IDMotorista
                              join tipoDoc in repositorio.ListComplex<TipoDocumento>().AsNoTracking() on docs.IDTipoDocumento equals tipoDoc.ID
                              where (app.ID == Id)
                              select new MotoristaDocumentoAAServicoView
                              {
                                  Sigla = tipoDoc.Sigla,
                                  Descricao = tipoDoc.Descricao,
                                  DataVencimento = docs.DataVencimento,
                                  Obrigatorio = tipoDoc.Obrigatorio
                              });
            return documentos.ToList();
        }

        public List<MotoristaAAServicoView> ListarMotoristaAAServico(MotoristaServicoFiltro filtro)
        {
            using (UniCadDalRepositorio<Motorista> repositorio = new UniCadDalRepositorio<Motorista>())
            {
                var resultado = GetQueryMotoristaServico(filtro);
                List<MotoristaAAServicoView> lstView = new List<MotoristaAAServicoView>();

                resultado.ForEach(p =>
                {
                    lstView.Add(new MotoristaAAServicoView
                    {
                        ID = p.ID,
                        LinhaNegocio = p.LinhaNegocio,
                        UsuarioTreinamentoPratico = p.UsuarioTreinamentoPratico,
                        UsuarioTreinamentoTeorico = p.UsuarioTreinamentoTeorico
                    });
                });

                if (lstView != null && lstView.Any())
                    lstView.ForEach(x =>
                    {
                        x.ListaDocumentos = ListarDocumentosAAPorIdMotorista(x.ID, repositorio);
                        x.ListaTreinamentosPraticos = ListarTreinamentosPraticosPorIdMotorista(x.ID, filtro.Terminal, repositorio);
                    });
                return lstView;
            }
        }
        #endregion
    }
}