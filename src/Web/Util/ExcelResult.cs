using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Raizen.UniCad.Web.Util
{
    public class ExcelResult : ActionResult
    {
        private readonly string _nomeArquivo;
        private MemoryStream _ms;

        public ExcelResult(MemoryStream ms, string nomeArquivo)
        {
            if (ms == null || ms.Length == 0)
                throw new ArgumentException("Os dados para gerar um arquivo Excel são nulos ou inválidos.", "arquivo");

            if (string.IsNullOrEmpty(nomeArquivo))
                nomeArquivo = "ArquivoExcel.xlsx";
            else if (!nomeArquivo.EndsWith(".xlsx"))
                nomeArquivo += ".xlsx";

            this._ms = ms;
            this._nomeArquivo = nomeArquivo.Replace(",", "");
        }

        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null)
                throw new ArgumentException("O contexto está nulo.", "context");

            HttpResponseBase response = context.HttpContext.Response;
            response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

            //Tratar como download em qualquer browser.
            response.AddHeader("Content-Disposition",
                string.Format("attachment; filename={0}", this._nomeArquivo));

            //Gravar dados em memória para poder retornar.
            _ms.Position = 0;
            response.BinaryWrite(_ms.ToArray());
        }
    }
}