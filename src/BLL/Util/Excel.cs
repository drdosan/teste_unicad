using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raizen.UniCad.BLL.Util
{
	public static class Excel
	{
		public static int PreencheColunas(IXLWorksheet worksheet, IList<string> nomeColunas)
		{
			for (int i = 0; i < nomeColunas.Count; i++)
			{
				worksheet.Cell(1, i + 1).Value = nomeColunas[i];
			}

			using (IXLRange range = worksheet.Range(1, 1, 1, nomeColunas.Count))
			{
				range.Style.Font.Bold = true;
				range.Style.Font.SetFontColor(XLColor.White);
				range.Style.Fill.PatternType = XLFillPatternValues.Solid;
				range.Style.Fill.SetBackgroundColor(XLColor.FromArgb(150, 26, 141)); //Roxo Raízen.
				range.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

				range.SetAutoFilter();

				DesenharBorda(range);
			}

			worksheet.Row(1).AdjustToContents();

			return nomeColunas.Count;
		}

		public static void DesenharBorda(IXLRange celulas)
		{
			celulas.Style.Border.TopBorder = XLBorderStyleValues.Thin;
			celulas.Style.Border.LeftBorder = XLBorderStyleValues.Thin;
			celulas.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
			celulas.Style.Border.RightBorder = XLBorderStyleValues.Thin;
		}

	}
}
