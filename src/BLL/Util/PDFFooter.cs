using System;
using System.Globalization;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace Raizen.UniCad.BLL.Util
{
    public class PdfFooter : PdfPageEventHelper
    {

        // write on end of each page
        public override void OnEndPage(PdfWriter writer, Document document)
        {
            base.OnEndPage(writer, document);
            PdfPTable tabFot = new PdfPTable(new[] { 100f, 30f, 30f, 30f, 30f, 30f, 30f, 30f, 30f, 30f, 30f, 30f, 30f, 30f, 30f, 40f });
            PdfPCell cell;

            Font oFontForLabel = new Font(Font.FontFamily.HELVETICA, 5, Font.BOLD);

            tabFot.TotalWidth = 580f;
            cell = new PdfPCell(new Phrase(DateTime.Now.ToString("dd-MMMM-yyyy  HH:mm", new CultureInfo("pt-BR")) + " Página " + document.PageNumber, oFontForLabel));
            cell.Colspan = 16;
            cell.Border = Rectangle.NO_BORDER;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            tabFot.AddCell(cell);
            tabFot.WriteSelectedRows(0, -1, 10, document.Bottom, writer.DirectContent);
        }

        //write on close of document
        public override void OnCloseDocument(PdfWriter writer, Document document)
        {
            base.OnCloseDocument(writer, document);
        }
    }
}
