using iTextSharp.text;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.draw;
using iTextSharp.text.pdf.parser;
using Raizen.Framework.Log.Bases;
using Raizen.UniCad.BLL.Util;
using Raizen.UniCad.Model;
using Raizen.UniCad.Model.View;
using Raizen.UniCad.SAL;
using Raizen.UniCad.SAL.WsIntegracaoSAPMotorista;
using Raizen.UniCad.Utils;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Zen.Barcode;

namespace Raizen.UniCad.BLL
{
    public class ArquivoBusiness
    {
        private PlacaBusiness _PlacaBusiness = new PlacaBusiness();
        private PlacaDocumentoBusiness _PlacaDocBusiness = new PlacaDocumentoBusiness();
        private ChecklistComposicaoBusiness _ChecklistBusiness = new ChecklistComposicaoBusiness();

        private MotoristaBusiness _MotoristaBusiness = new MotoristaBusiness();
        private MotoristaDocumentoBusiness _MotoristaDocBusiness = new MotoristaDocumentoBusiness();
        private MotoristaTreinamentoTerminalBusiness _TreinamentoBusiness = new MotoristaTreinamentoTerminalBusiness();
        private HistoricoTreinamentoTeoricoMotoristaBusiness _HistoricoBusiness = new HistoricoTreinamentoTeoricoMotoristaBusiness();
        private int _MultiploTamanhoCracha = 2;

        public void ProcessarArquivos()
        {
            var listaArquivos = new List<string>();

            var path = Config.GetConfig(Model.EnumConfig.CaminhoAnexos, (int)EnumPais.Padrao);
            //var path = "C:/Uploads/";
            var files = new DirectoryInfo(path).GetFiles();

            var placas = _PlacaBusiness.Listar();
            if (placas != null && placas.Any())
                listaArquivos.AddRange(placas.Select(p => p.Anexo));
            var placaDocs = _PlacaDocBusiness.Listar();
            if (placaDocs != null && placaDocs.Any())
                listaArquivos.AddRange(placaDocs.Select(p => p.Anexo));
            var checklists = _ChecklistBusiness.Listar();
            if (checklists != null && checklists.Any())
                listaArquivos.AddRange(checklists.Select(p => p.Anexo));
            var motoristas = _MotoristaBusiness.Listar();
            if (motoristas != null && motoristas.Any())
                listaArquivos.AddRange(motoristas.Select(p => p.Anexo));
            var motoDocs = _MotoristaDocBusiness.Listar();
            if (motoDocs != null && motoDocs.Any())
                listaArquivos.AddRange(motoDocs.Select(p => p.Anexo));
            var treinamentos = _TreinamentoBusiness.Listar();
            if (treinamentos != null && treinamentos.Any())
                listaArquivos.AddRange(treinamentos.Select(p => p.Anexo));
            var historicos = _HistoricoBusiness.Listar();
            if (historicos != null && historicos.Any())
                listaArquivos.AddRange(historicos.Select(p => p.Anexo));

            listaArquivos = listaArquivos.Where(p => p != null).ToList();

            foreach (var file in files)
            {
                {
                    ArquivoUtil.ExcluirArquivo(file.Name, path);
                }
            }
        }



        private PdfPCell MontarCell(string texto, bool download)
        {
            int border = 1;
            int horizontalAlignment = 1;
            int borderWidth = 1;

            int paddinTop = 3;
            int paddingBottom = 3;

            if (!download)
            {
                paddinTop = paddinTop * _MultiploTamanhoCracha;
                paddingBottom = paddingBottom * _MultiploTamanhoCracha;
            }

            var boldFont = download ? FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 8) : FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12);
            var phrase = new Phrase
            {
                new Chunk(texto, boldFont)
            };

            PdfPCell cell = new PdfPCell(phrase)
            {
                HorizontalAlignment = horizontalAlignment,
                BorderWidth = borderWidth,
                Border = border,
                PaddingTop = paddinTop,
                PaddingBottom = paddingBottom,
                BorderWidthLeft = borderWidth,
                BorderWidthRight = borderWidth
            };

            return cell;
        }


        private iTextSharp.text.Image GerarCodigoBarra(string cpfOuDni)
        {
            var draw = new Code128BarcodeDraw(Code128Checksum.Instance);
            var image = draw.Draw(cpfOuDni, 100, 4);

            byte[] arr;
            using (var memStream = new MemoryStream())
            {
                image.Save(memStream, ImageFormat.Png);
                arr = memStream.ToArray();
            }

            Image result = Image.GetInstance(arr);
            return result;
        }


        public string DownloadCrachaPDF(Motorista motorista, string nomeImagem, bool download)
        {
            ArquivoBusiness arquivoBLL = new ArquivoBusiness();
            int border = 1;
            int horizontalAlignment = 1;
            int borderWidth = 1;

            float widthColumn1 = 260f;
            float widthColumn2 = 3;
            float widthColumn3 = 240f;


            PdfPTable table = new PdfPTable(3);

            var cellCentro = arquivoBLL.MontarCell("", download);
            cellCentro.BorderWidthTop = 0;
            cellCentro.BorderWidthBottom = 0;


            if (!download)
            {
                widthColumn1 = widthColumn1 * _MultiploTamanhoCracha;
                widthColumn2 = widthColumn2 * _MultiploTamanhoCracha;
                widthColumn3 = widthColumn3 * _MultiploTamanhoCracha;
            }


            table.SetWidths(new float[] { widthColumn1, widthColumn2, widthColumn3 });


            //table.TotalWidth = 100;
            table.WidthPercentage = 100;

            var caminhoSalvar = Config.GetConfig(Model.EnumConfig.CaminhoAnexos, (int)EnumPais.Padrao);

            var uploadPath = caminhoSalvar;
#if DEBUG
            uploadPath = "C:\\Raizen\\";
#endif

            var caminhoCompletoArquivoFoto = uploadPath + nomeImagem;

            iTextSharp.text.Image img = iTextSharp.text.Image.GetInstance(caminhoCompletoArquivoFoto);

            if (download)
                img.ScaleToFit(54f, 64f);
            else
                img.ScaleToFit(54f * _MultiploTamanhoCracha, 64f * _MultiploTamanhoCracha);


            img.BorderWidthLeft = 1;
            img.BorderWidthRight = 1;
            img.BorderWidthTop = 1;
            img.BorderWidthBottom = 1;
            img.Alignment = iTextSharp.text.Image.ALIGN_LEFT;

            var pgSize = download ? new iTextSharp.text.Rectangle(481, 153) : new iTextSharp.text.Rectangle(481 * _MultiploTamanhoCracha, 153 * _MultiploTamanhoCracha);

            Document pdfDoc = new Document(pgSize, 1f, 1f, 1f, 1f);

            using (MemoryStream memoryStream = new MemoryStream())
            {
                PdfWriter writer = PdfWriter.GetInstance(pdfDoc, memoryStream);

                pdfDoc.Open();

                var pathDocument = System.Web.Hosting.HostingEnvironment.MapPath("~/Documents");
                var boldFont = download ? FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 8) : FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12);
                var phrase = new Phrase();


                #region Primeira Linha

                var cellCartaoIdentificacao = MontarCell(@"CARTÃO DE INDENTIFICAÇÃO", download);


                string cpfOuDni = motorista.IdPais == EnumPais.Brasil ? "CPF:   " + (motorista.MotoristaBrasil != null ? motorista.MotoristaBrasil.CPF : "") + "   " : "DNI:   " + (motorista.MotoristaArgentina != null ? motorista.MotoristaArgentina.DNI : "") + "   ";
                var imgCodeBar = this.GerarCodigoBarra(cpfOuDni);

                if (download)
                    imgCodeBar.ScaleToFit(100f, 20f);
                else
                    imgCodeBar.ScaleToFit(100f * _MultiploTamanhoCracha, 20f * _MultiploTamanhoCracha);

                imgCodeBar.BorderWidthLeft = 1;

                Paragraph paragraphCPF = new Paragraph(cpfOuDni)
                {
                    Font = boldFont,
                };

                paragraphCPF.IndentationLeft = download ? 3f : 3f * _MultiploTamanhoCracha;

                var cellCpf = MontarCell(cpfOuDni, download);

                cellCpf.BorderWidthBottom = 0;

                imgCodeBar.Alignment = Element.ALIGN_RIGHT;

                Chunk glue = new Chunk(imgCodeBar, 0, 0, true);
                paragraphCPF.Add(glue);
                cellCpf.AddElement(paragraphCPF);

                PdfPCell[] cellsPrimeiraLinha = new PdfPCell[] { cellCartaoIdentificacao, cellCentro, cellCpf };

                PdfPRow rowPrimeiraLinha = new PdfPRow(cellsPrimeiraLinha);

                //table.SpacingBefore = 80;
                //table.SpacingAfter = 80;
                table.Rows.Add(rowPrimeiraLinha);

                #endregion Primeira Linha

                #region Segunda Linha
                Transportadora transp = null;
                string rede = "";
                if (motorista.Operacao == "CIF")
                {
                    transp = new TransportadoraBusiness().Selecionar(p => p.ID == motorista.IDTransportadora);
                    if (transp != null)
                    {
                        motorista.NomeTransportadora = transp.RazaoSocial;
                        rede = motorista.NomeTransportadora;
                    }
                    else
                    {
                        if (motorista.IdPais == EnumPais.Brasil)
                        {
                            throw new Exception("Não foi possível realizar a impressão do motorista solicitado. Por gentileza, entrar em contato com o Time Raizen");
                        }
                        else
                        {
                            throw new Exception("No fue posible imprimir el conductor solicitado. Comuníquese con el equipo Raizen.");
                        }
                    }
                }
                else
                {
                    rede = "FOB";
                }

                var cellRede = MontarCell(rede, download);

                var cellValidade = MontarCell("Validades", download);

                cellValidade.BorderWidthBottom = 0;
                cellValidade.BorderWidthTop = 0;


                PdfPCell[] cellsSegundaLinha = new PdfPCell[] { cellRede, cellCentro, cellValidade };

                PdfPRow rowSegundaLinha = new PdfPRow(cellsSegundaLinha);

                table.Rows.Add(rowSegundaLinha);

                #endregion Segunda Linha

                #region Terceira Linha
                string nomeTransportadora = "Nome da Transportadora";
                var cellTransportadora = MontarCell(nomeTransportadora, download);
                var cellLinha3 = MontarCell(string.Empty, download);
                cellLinha3.BorderWidthBottom = 0;
                cellLinha3.BorderWidthTop = 0;

                PdfPCell[] cellsTerceiraLinha = new PdfPCell[] { cellTransportadora, cellCentro, cellLinha3 };
                PdfPRow rowTerceiraLinha = new PdfPRow(cellsTerceiraLinha);
                table.Rows.Add(rowTerceiraLinha);

                #endregion Terceira Linha

                #region Quarta Linha
                string nomeMotorista = motorista.Nome;
                var cellMotorista = MontarCell(nomeMotorista, download);
                var cellLinha2 = MontarCell("", download);
                cellLinha2.BorderWidthBottom = 0;
                cellLinha2.BorderWidthTop = 0;

                PdfPCell[] cellsQuartaLinha = new PdfPCell[] { cellMotorista, cellCentro, cellLinha2 };
                PdfPRow rowQuartaLinha = new PdfPRow(cellsQuartaLinha);
                table.Rows.Add(rowQuartaLinha);


                //cellValidade.BackgroundColor = new iTextSharp.text.BaseColor(51, 102, 102); 
                cellValidade.Rowspan = 5;

                #endregion Quarta Linha

                #region Quinta Linha
                string cnh = "CNH:  " + motorista.Documentos.Where(m => m.Sigla == "CNH").LastOrDefault()?.DataVencimento?.ToString("dd.MM.yyyy");
                string mopp = "MOPP:  " + motorista.Documentos.Where(m => m.Sigla == "MOPP").LastOrDefault()?.DataVencimento?.ToString("dd.MM.yyyy");
                string derrame = "OP. s/Derrame:  " + "          ";
                string treinamentoNR20 = "NR 20:  " + motorista.Documentos.Where(m => m.Sigla == "NR20").LastOrDefault()?.DataVencimento?.ToString("dd.MM.yyyy");
                string treinamentoNR35 = "NR 35:  " + motorista.Documentos.Where(m => m.Sigla == "NR35").LastOrDefault()?.DataVencimento?.ToString("dd.MM.yyyy");


                string trm = "";
                string cdd = "";
                string psi = "";
                string exame = "";
                if (motorista.Operacao == "CIF")
                {
                    trm = "                      TRM: " + motorista.Documentos.Where(m => m.Sigla == "TRM").LastOrDefault()?.DataVencimento?.ToString("dd.MM.yyyy"); ;
                    cdd = "                   CDD: " + motorista.Documentos.Where(m => m.Sigla == "CDD").LastOrDefault()?.DataVencimento?.ToString("dd.MM.yyyy"); ;
                    psi = "             PSI: " + motorista.Documentos.Where(m => m.Sigla == "PSI").LastOrDefault()?.DataVencimento?.ToString("dd.MM.yyyy"); ;
                    exame = "                    EXAME: " + motorista.Documentos.Where(m => m.Sigla == "EXAME").LastOrDefault()?.DataVencimento?.ToString("dd.MM.yyyy"); ;
                }

                var cellNomeMotorista = MontarCell("Nome do Motorista", download);


                var cellCnh = MontarCell("", download);
                cellCnh.HorizontalAlignment = Element.ALIGN_LEFT;
                cellCnh.BorderWidthTop = 0;
                cellCnh.BorderWidthBottom = 0;

                PdfPCell[] cellsQuintaLinha = new PdfPCell[] { cellNomeMotorista, cellCentro, cellCnh };
                PdfPRow rowQuintaLinha = new PdfPRow(cellsQuintaLinha);
                table.Rows.Add(rowQuintaLinha);

                #endregion Quinta Linha

                #region Sexta Linha

                #region Assinaturas
                Paragraph paragLinhas = new Paragraph(" _________________________  _________________")
                {
                    Alignment = Element.ALIGN_RIGHT,
                    Font = boldFont
                };

                var cellFoto = this.MontarCell("", download);
                cellFoto.BorderWidthBottom = 1;

                img.Alignment = 6;

                glue = new Chunk(new VerticalPositionMark());

                Phrase ph1 = new Phrase();

                //ph1.Font = (download) ? FontFactory.GetFont(FontFactory.HELVETICA, 8) : FontFactory.GetFont(FontFactory.HELVETICA, 13);
                Paragraph para = new Paragraph();
                Paragraph main = new Paragraph();
                ph1.Add(new Chunk(img, 0, 0, true));
                ph1.Add(glue);

                if (download)
                {
                    ph1.Add(new Chunk("P/Raízen Combustíveis S.A. P/Transaportadora"));
                }

                PdfContentByte cbLinhaAssinatura = writer.DirectContent;
                cbLinhaAssinatura.BeginText();

                var fbase = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);


                if (download)
                {
                    cbLinhaAssinatura.SetFontAndSize(fbase, 9);
                    cbLinhaAssinatura.SetTextMatrix(60, 25);
                    cbLinhaAssinatura.ShowText("------------------------------------   ------------------------");
                }
                else
                {
                    PdfContentByte cbAssinaturas = writer.DirectContent;
                    //cbAssinaturas.BeginText();
                    cbAssinaturas.SetFontAndSize(fbase, 11);
                    cbAssinaturas.SetTextMatrix(120, 50);
                    cbLinhaAssinatura.ShowText("P/Raízen Combustíveis S.A. P/Transaportadora");
                    //cbAssinaturas.EndText();

                    cbLinhaAssinatura.SetFontAndSize(fbase, 11);
                    cbLinhaAssinatura.SetTextMatrix(120, 67);
                    cbLinhaAssinatura.ShowText("------------------------------------   ------------------------");
                }

                cbLinhaAssinatura.EndText();

                main.Font = new Font(Font.FontFamily.COURIER, (download) ? 7 : 10, Font.BOLDITALIC, BaseColor.BLACK);

                main.Add(ph1);
                para.Add(main);
                cellFoto.AddElement(para);

                cellFoto.HorizontalAlignment = Element.ALIGN_BOTTOM;
                #endregion Assinaturas

                PdfPCell cellSextaLinha = new PdfPCell()
                {
                    HorizontalAlignment = horizontalAlignment,
                    BorderWidth = borderWidth,
                    Border = border,
                    BorderWidthLeft = borderWidth,
                    BorderWidthRight = borderWidth,
                    BorderWidthBottom = borderWidth
                };

                cellSextaLinha.BorderWidthTop = 0;

                Paragraph elementValidade = new Paragraph("Validades")
                {
                    Alignment = Element.ALIGN_CENTER,
                    Font = boldFont,
                };


                Paragraph elementCNH = new Paragraph(cnh + trm)
                {
                    Alignment = Element.ALIGN_LEFT,
                    Font = boldFont,
                };


                Paragraph elementMOPP = new Paragraph(mopp + cdd)
                {
                    Alignment = Element.ALIGN_LEFT,
                    Font = boldFont,
                };


                Paragraph elementOpDerrame = new Paragraph(derrame + psi)
                {
                    Alignment = Element.ALIGN_LEFT,
                    Font = boldFont
                };



                Paragraph elementTreinamentoNr35 = new Paragraph(treinamentoNR20 + exame)
                {
                    Alignment = Element.ALIGN_LEFT,
                    Font = boldFont
                };


                Paragraph elementTreinamentoNr20 = new Paragraph(treinamentoNR35)
                {
                    Alignment = Element.ALIGN_LEFT,
                    Font = boldFont
                };


                string dtBaseEmissao = "Base e Data de Emissão: BIP " + DateTime.Today.ToString("dd.MM.yyyy");
                Paragraph elementBaseDataEmissao = new Paragraph(dtBaseEmissao)
                {
                    Alignment = Element.ALIGN_CENTER,
                    Font = boldFont
                };

                Paragraph elementLinha = new Paragraph("__________________________________")
                {
                    Alignment = Element.ALIGN_CENTER,
                    Font = boldFont
                };

                Paragraph elementAssinatura = new Paragraph("Assinatura do Motorista")
                {
                    Alignment = Element.ALIGN_CENTER,
                    Font = boldFont
                };


                elementBaseDataEmissao.SpacingBefore = (download) ? 10 : 10 * _MultiploTamanhoCracha;

                if (!download)
                {
                    elementBaseDataEmissao.SpacingBefore = elementBaseDataEmissao.SpacingBefore * _MultiploTamanhoCracha;
                }


                float indentationLeft = 2;

                elementCNH.IndentationLeft = indentationLeft;
                elementMOPP.IndentationLeft = indentationLeft;
                elementOpDerrame.IndentationLeft = indentationLeft;
                elementTreinamentoNr35.IndentationLeft = indentationLeft;
                elementTreinamentoNr20.IndentationLeft = indentationLeft;
                elementBaseDataEmissao.IndentationLeft = indentationLeft;
                elementLinha.IndentationLeft = indentationLeft;
                elementAssinatura.IndentationLeft = indentationLeft;

                cellValidade.AddElement(elementValidade);
                cellValidade.AddElement(elementCNH);
                cellValidade.AddElement(elementMOPP);
                cellValidade.AddElement(elementOpDerrame);
                cellValidade.AddElement(elementTreinamentoNr35);
                cellValidade.AddElement(elementTreinamentoNr20);
                cellValidade.AddElement(elementBaseDataEmissao);
                cellValidade.AddElement(elementLinha);
                cellValidade.AddElement(elementAssinatura);


                PdfPCell[] cellsSextaLinha = new PdfPCell[] { cellFoto, cellCentro, cellSextaLinha };

                PdfPRow rowSextaLinha = new PdfPRow(cellsSextaLinha);

                table.Rows.Add(rowSextaLinha);

                #endregion Sexta Linha

                pdfDoc.Add(table);

                pdfDoc.Close();

                byte[] bytes = memoryStream.ToArray();
                memoryStream.Close();

                string caminhoCompletoDoc = download ? "Cracha_Motorista_" + motorista.ID + ".pdf" : "Visualizar_Cracha_Motorista_" + motorista.ID + ".pdf";

                pathDocument += @"/" + caminhoCompletoDoc;

                var docExiste = File.Exists(pathDocument);

                if (docExiste && download)
                {
                    File.Delete(pathDocument);
                    docExiste = false;
                }

                if (!docExiste)
                {
                    File.WriteAllBytes(pathDocument, bytes);
                }

                return pathDocument;
            }
        }

        public byte[] DownloadCrachaPDF(Motorista motorista, HttpPostedFileBase foto, bool download)
        {

            try
            {
                

                Imagem imagem = new Imagem();
                ArquivoBusiness arquivoBLL = new ArquivoBusiness();
                int border = 1;
                int horizontalAlignment = 1;
                int borderWidth = 1;

                float widthColumn1 = 260f;
                float widthColumn2 = 3;
                float widthColumn3 = 260f;


                PdfPTable table = new PdfPTable(3);

                var cellCentro = arquivoBLL.MontarCell("", download);
                cellCentro.BorderWidthTop = 0;
                cellCentro.BorderWidthBottom = 0;


                if (!download)
                {
                    widthColumn1 = widthColumn1 * _MultiploTamanhoCracha;
                    widthColumn2 = widthColumn2 * _MultiploTamanhoCracha;
                    widthColumn3 = widthColumn3 * _MultiploTamanhoCracha;
                }


                table.SetWidths(new float[] { widthColumn1, widthColumn2, widthColumn3 });


                //table.TotalWidth = 100;
                table.WidthPercentage = 100;

                System.Drawing.Image imgDrawing = System.Drawing.Image.FromStream(foto.InputStream);
                System.Drawing.Image imgDrawingResized = _MultiploTamanhoCracha >= 1 ? imagem.resizeImage(imgDrawing, 54f * _MultiploTamanhoCracha, 64f * _MultiploTamanhoCracha) : imagem.resizeImage(imgDrawing, 54f, 64f);


                iTextSharp.text.Image img = iTextSharp.text.Image.GetInstance(imgDrawingResized, BaseColor.WHITE);


                if (download)
                    img.ScaleAbsolute(54f, 64f);
                else
                    img.ScaleAbsolute(54f * _MultiploTamanhoCracha, 64f * _MultiploTamanhoCracha);


                img.BorderWidthLeft = 1;
                img.BorderWidthRight = 1;
                img.BorderWidthTop = 1;
                img.BorderWidthBottom = 1;
                img.Alignment = iTextSharp.text.Image.ALIGN_LEFT;

                var pgSize = download ? new iTextSharp.text.Rectangle(501, 153) : new iTextSharp.text.Rectangle(481 * _MultiploTamanhoCracha, 153 * _MultiploTamanhoCracha);

                Document pdfDoc = new Document(pgSize, 1f, 1f, 1f, 1f);

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    PdfWriter writer = PdfWriter.GetInstance(pdfDoc, memoryStream);

                    pdfDoc.Open();

                    var boldFont = download ? FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 8) : FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12);
                    var phrase = new Phrase();


                    #region Primeira Linha

                    var cellCartaoIdentificacao = MontarCell(@"CARTÃO DE INDENTIFICAÇÃO", download);


                    string cpfOuDni = motorista.IdPais == EnumPais.Brasil ? "CPF:   " + (motorista.MotoristaBrasil != null ? motorista.MotoristaBrasil.CPF : "") + "   " : "DNI:   " + (motorista.MotoristaArgentina != null ? motorista.MotoristaArgentina.DNI : "") + "   ";
                    var imgCodeBar = this.GerarCodigoBarra(cpfOuDni);

                    if (download)
                        imgCodeBar.ScaleToFit(100f, 20f);
                    else
                        imgCodeBar.ScaleToFit(100f * _MultiploTamanhoCracha, 20f * _MultiploTamanhoCracha);

                    imgCodeBar.BorderWidthLeft = 1;

                    Paragraph paragraphCPF = new Paragraph(cpfOuDni)
                    {
                        Font = boldFont,
                    };

                    paragraphCPF.IndentationLeft = download ? 3f : 3f * _MultiploTamanhoCracha;

                    var cellCpf = MontarCell(cpfOuDni, download);

                    cellCpf.BorderWidthBottom = 0;

                    imgCodeBar.Alignment = Element.ALIGN_RIGHT;

                    Chunk glue = new Chunk(imgCodeBar, 0, 0, true);
                    paragraphCPF.Add(glue);
                    cellCpf.AddElement(paragraphCPF);

                    PdfPCell[] cellsPrimeiraLinha = new PdfPCell[] { cellCartaoIdentificacao, cellCentro, cellCpf };

                    PdfPRow rowPrimeiraLinha = new PdfPRow(cellsPrimeiraLinha);

                    //table.SpacingBefore = 80;
                    //table.SpacingAfter = 80;
                    table.Rows.Add(rowPrimeiraLinha);

                    #endregion Primeira Linha

                    #region Segunda Linha
                    Transportadora transp = null;
                    string rede = "";
                    if (motorista.Operacao == "CIF")
                    {
                        transp = new TransportadoraBusiness().Selecionar(p => p.ID == motorista.IDTransportadora);
                        if (transp != null)
                        {
                            motorista.NomeTransportadora = transp.RazaoSocial;
                            rede = motorista.NomeTransportadora;
                        }
                        else
                        {
                            if (motorista.IdPais == EnumPais.Brasil)
                            {
                                throw new Exception("Não foi possível realizar a impressão do motorista solicitado. Por gentileza, entrar em contato com o Time Raizen");
                            }
                            else
                            {
                                throw new Exception("No fue posible imprimir el conductor solicitado. Comuníquese con el equipo Raizen.");
                            }
                        }
                    }
                    else
                    {
                        rede = "FOB";
                    }

                    var cellRede = MontarCell(rede, download);

                    var cellValidade = MontarCell("Validades", download);

                    cellValidade.BorderWidthBottom = 0;
                    cellValidade.BorderWidthTop = 0;


                    PdfPCell[] cellsSegundaLinha = new PdfPCell[] { cellRede, cellCentro, cellValidade };

                    PdfPRow rowSegundaLinha = new PdfPRow(cellsSegundaLinha);

                    table.Rows.Add(rowSegundaLinha);

                    #endregion Segunda Linha

                    #region Terceira Linha
                    string nomeTransportadora = "Nome da Transportadora";
                    var cellTransportadora = MontarCell(nomeTransportadora, download);
                    var cellLinha3 = MontarCell(string.Empty, download);
                    cellLinha3.BorderWidthBottom = 0;
                    cellLinha3.BorderWidthTop = 0;

                    PdfPCell[] cellsTerceiraLinha = new PdfPCell[] { cellTransportadora, cellCentro, cellLinha3 };
                    PdfPRow rowTerceiraLinha = new PdfPRow(cellsTerceiraLinha);
                    table.Rows.Add(rowTerceiraLinha);

                    #endregion Terceira Linha

                    #region Quarta Linha
                    string nomeMotorista = motorista.Nome;
                    var cellMotorista = MontarCell(nomeMotorista, download);
                    var cellLinha2 = MontarCell("", download);
                    cellLinha2.BorderWidthBottom = 0;
                    cellLinha2.BorderWidthTop = 0;

                    PdfPCell[] cellsQuartaLinha = new PdfPCell[] { cellMotorista, cellCentro, cellLinha2 };
                    PdfPRow rowQuartaLinha = new PdfPRow(cellsQuartaLinha);
                    table.Rows.Add(rowQuartaLinha);


                    //cellValidade.BackgroundColor = new iTextSharp.text.BaseColor(51, 102, 102); 
                    cellValidade.Rowspan = 5;

                    #endregion Quarta Linha

                    #region Quinta Linha
                    string cnhOuLnc = "";

                    if (motorista.IdPais == EnumPais.Brasil)
                    {
                        cnhOuLnc = "CNH:  " + motorista.Documentos.Where(m => m.Sigla == "CNH").LastOrDefault()?.DataVencimento?.ToString("dd.MM.yyyy");
                    }
                    else
                    {
                        cnhOuLnc = "LNC:  " + motorista.Documentos.Where(m => m.Sigla == "LNC").LastOrDefault()?.DataVencimento?.ToString("dd.MM.yyyy");
                    }

                    //string cnh = "CNH:  " + motorista.Documentos.Where(m => m.Sigla == "CNH").LastOrDefault()?.DataVencimento?.ToString("dd.MM.yyyy");
                    string mopp = "MOPP:  " + motorista.Documentos.Where(m => m.Sigla == "MOPP").LastOrDefault()?.DataVencimento?.ToString("dd.MM.yyyy");


                    PersistirResponse retorno = null;

                    if (motorista.IDEmpresa == (int)EnumEmpresa.Combustiveis)
                    {
                        switch (motorista.IdPais)
                        {
                            case EnumPais.Argentina:
                                WsIntegraSAPAR_Motorista integraSAPAR_Motorista = new WsIntegraSAPAR_Motorista();
                                retorno = integraSAPAR_Motorista.ConsultarMotorista(motorista);
                                break;

                            case EnumPais.Brasil:
                                WsIntegraSAP integraSAP = new WsIntegraSAP();
                                retorno = integraSAP.ConsultarMotorista(motorista);
                                break;
                        }
                    }
                    else if (motorista.IdPais == EnumPais.Brasil)
                    {
                        WsIntegraSAPEAB integraSAP = new WsIntegraSAPEAB();
                        retorno = integraSAP.ConsultarMotorista(motorista);
                    }

                    string derrame = string.Empty;

                    derrame = retorno?.MOTORISTA.FirstOrDefault()?.RELCUROPDER;
                    if (string.IsNullOrWhiteSpace(derrame) || derrame.Equals("0000-00-00"))
                    {
                        derrame = "          ";
                    }
                    else
                    {
                        derrame = DateTime.ParseExact(derrame, "yyyy-MM-dd", CultureInfo.InvariantCulture).ToString("dd.MM.yyyy");

                    }

                    derrame = "OP. s/Derrame:  " + derrame;




                    string treinamentoNR20 = "NR 20:  " + motorista.Documentos.Where(m => m.Sigla == "NR20").LastOrDefault()?.DataVencimento?.ToString("dd.MM.yyyy");
                    string treinamentoNR35 = "NR 35:  " + motorista.Documentos.Where(m => m.Sigla == "NR35").LastOrDefault()?.DataVencimento?.ToString("dd.MM.yyyy");


                    string trm = "";
                    string cdd = "";
                    string psi = "";
                    string exame = "";
                    if (motorista.Operacao == "CIF")
                    {
                        trm = "                      TRM: " + motorista.Documentos.Where(m => m.Sigla == "TRM").LastOrDefault()?.DataVencimento?.ToString("dd.MM.yyyy"); ;
                        cdd = "                   CDD: " + motorista.Documentos.Where(m => m.Sigla == "CDD").LastOrDefault()?.DataVencimento?.ToString("dd.MM.yyyy"); ;
                        psi = "             PSI: " + motorista.Documentos.Where(m => m.Sigla == "PSI").LastOrDefault()?.DataVencimento?.ToString("dd.MM.yyyy"); ;
                        exame = "                    EXAME: " + motorista.Documentos.Where(m => m.Sigla == "EXAME").LastOrDefault()?.DataVencimento?.ToString("dd.MM.yyyy"); ;
                    }

                    var cellNomeMotorista = MontarCell("Nome do Motorista", download);


                    var cellCnh = MontarCell("", download);
                    cellCnh.HorizontalAlignment = Element.ALIGN_LEFT;
                    cellCnh.BorderWidthTop = 0;
                    cellCnh.BorderWidthBottom = 0;

                    PdfPCell[] cellsQuintaLinha = new PdfPCell[] { cellNomeMotorista, cellCentro, cellCnh };
                    PdfPRow rowQuintaLinha = new PdfPRow(cellsQuintaLinha);
                    table.Rows.Add(rowQuintaLinha);

                    #endregion Quinta Linha

                    #region Sexta Linha

                    #region Assinaturas
                    Paragraph paragLinhas = new Paragraph(" _________________________  _________________")
                    {
                        Alignment = Element.ALIGN_RIGHT,
                        Font = boldFont
                    };

                    var cellFoto = this.MontarCell("", download);
                    cellFoto.BorderWidthBottom = 1;

                    img.Alignment = 6;

                    glue = new Chunk(new VerticalPositionMark());

                    Phrase ph1 = new Phrase();

                    //ph1.Font = (download) ? FontFactory.GetFont(FontFactory.HELVETICA, 8) : FontFactory.GetFont(FontFactory.HELVETICA, 13);
                    Paragraph para = new Paragraph();
                    Paragraph main = new Paragraph();
                    ph1.Add(new Chunk(img, 0, 0, true));
                    ph1.Add(glue);

                    if (download)
                    {
                        ph1.Add(new Chunk("P/Raízen Combustíveis S.A. P/Transaportadora"));
                    }

                    PdfContentByte cbLinhaAssinatura = writer.DirectContent;
                    cbLinhaAssinatura.BeginText();

                    var fbase = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);


                    if (download)
                    {
                        cbLinhaAssinatura.SetFontAndSize(fbase, 9);
                        cbLinhaAssinatura.SetTextMatrix(60, 25);
                        cbLinhaAssinatura.ShowText("------------------------------------   ------------------------");
                    }
                    else
                    {
                        PdfContentByte cbAssinaturas = writer.DirectContent;
                        //cbAssinaturas.BeginText();
                        cbAssinaturas.SetFontAndSize(fbase, 11);
                        cbAssinaturas.SetTextMatrix(120, 50);
                        cbLinhaAssinatura.ShowText("P/Raízen Combustíveis S.A. P/Transaportadora");
                        //cbAssinaturas.EndText();

                        cbLinhaAssinatura.SetFontAndSize(fbase, 11);
                        cbLinhaAssinatura.SetTextMatrix(120, 67);
                        cbLinhaAssinatura.ShowText("------------------------------------   ------------------------");
                    }

                    cbLinhaAssinatura.EndText();

                    main.Font = new Font(Font.FontFamily.COURIER, (download) ? 7 : 10, Font.BOLDITALIC, BaseColor.BLACK);

                    main.Add(ph1);
                    para.Add(main);
                    cellFoto.AddElement(para);

                    cellFoto.HorizontalAlignment = Element.ALIGN_BOTTOM;
                    #endregion Assinaturas

                    PdfPCell cellSextaLinha = new PdfPCell()
                    {
                        HorizontalAlignment = horizontalAlignment,
                        BorderWidth = borderWidth,
                        Border = border,
                        BorderWidthLeft = borderWidth,
                        BorderWidthRight = borderWidth,
                        BorderWidthBottom = borderWidth
                    };

                    cellSextaLinha.BorderWidthTop = 0;

                    Paragraph elementValidade = new Paragraph("Validades")
                    {
                        Alignment = Element.ALIGN_CENTER,
                        Font = boldFont,
                    };


                    Paragraph elementCNH = new Paragraph(cnhOuLnc + trm)
                    {
                        Alignment = Element.ALIGN_LEFT,
                        Font = boldFont,
                    };


                    Paragraph elementMOPP = new Paragraph(mopp + cdd)
                    {
                        Alignment = Element.ALIGN_LEFT,
                        Font = boldFont,
                    };


                    Paragraph elementOpDerrame = new Paragraph(derrame + psi)
                    {
                        Alignment = Element.ALIGN_LEFT,
                        Font = boldFont
                    };



                    Paragraph elementTreinamentoNr35 = new Paragraph(treinamentoNR20 + exame)
                    {
                        Alignment = Element.ALIGN_LEFT,
                        Font = boldFont
                    };


                    Paragraph elementTreinamentoNr20 = new Paragraph(treinamentoNR35)
                    {
                        Alignment = Element.ALIGN_LEFT,
                        Font = boldFont
                    };


                    string dtBaseEmissao = "Base e Data de Emissão: BIP " + DateTime.Today.ToString("dd.MM.yyyy");
                    Paragraph elementBaseDataEmissao = new Paragraph(dtBaseEmissao)
                    {
                        Alignment = Element.ALIGN_CENTER,
                        Font = boldFont
                    };

                    Paragraph elementLinha = new Paragraph("__________________________________")
                    {
                        Alignment = Element.ALIGN_CENTER,
                        Font = boldFont
                    };

                    Paragraph elementAssinatura = new Paragraph("Assinatura do Motorista")
                    {
                        Alignment = Element.ALIGN_CENTER,
                        Font = boldFont
                    };


                    elementBaseDataEmissao.SpacingBefore = (download) ? 10 : 10 * _MultiploTamanhoCracha;

                    if (!download)
                    {
                        elementBaseDataEmissao.SpacingBefore = elementBaseDataEmissao.SpacingBefore * _MultiploTamanhoCracha;
                    }


                    float indentationLeft = 12;

                    elementCNH.IndentationLeft = indentationLeft;
                    elementMOPP.IndentationLeft = indentationLeft;
                    elementOpDerrame.IndentationLeft = indentationLeft;
                    elementTreinamentoNr35.IndentationLeft = indentationLeft;
                    elementTreinamentoNr20.IndentationLeft = indentationLeft;
                    elementBaseDataEmissao.IndentationLeft = indentationLeft;
                    elementLinha.IndentationLeft = indentationLeft;
                    elementAssinatura.IndentationLeft = indentationLeft;

                    cellValidade.AddElement(elementValidade);
                    cellValidade.AddElement(elementCNH);
                    cellValidade.AddElement(elementMOPP);
                    cellValidade.AddElement(elementOpDerrame);
                    cellValidade.AddElement(elementTreinamentoNr35);
                    cellValidade.AddElement(elementTreinamentoNr20);
                    cellValidade.AddElement(elementBaseDataEmissao);
                    cellValidade.AddElement(elementLinha);
                    cellValidade.AddElement(elementAssinatura);


                    PdfPCell[] cellsSextaLinha = new PdfPCell[] { cellFoto, cellCentro, cellSextaLinha };

                    PdfPRow rowSextaLinha = new PdfPRow(cellsSextaLinha);

                    table.Rows.Add(rowSextaLinha);

                    #endregion Sexta Linha

                    pdfDoc.Add(table);

                    pdfDoc.Close();

                    byte[] bytes = memoryStream.ToArray();

                    memoryStream.Close();

                    imagem.Dispose();
                    return bytes;
                }
            }
            catch (Exception ex)
            {
                new RaizenException("Erro Download Cracha", ex).LogarErro();
            }
            return null;
        }
    }
}
