using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Raizen.UniCad
{
    public class CollapsiblePanelHtmlHelper : IDisposable
    {
        private readonly ViewContext _viewContext;
        private Boolean _isDisposed;
        private readonly string _titulo;
        private readonly string _idCorpo;
        private readonly string _colunas;
        private readonly bool _fechado;
        private readonly string _iconeHeader;
        private readonly ActionButtonCollapsiblePanel[] _botoes;
        private readonly object _htmlAttributes;

        public CollapsiblePanelHtmlHelper(ViewContext viewContext, string titulo, string idCorpo, bool fechado = true, string iconeHeader = null, ActionButtonCollapsiblePanel[] botoes = null, object htmlAttributes = null, string colunas = "col-md-12")
        {
            this._viewContext = viewContext;
            this._titulo = titulo;
            this._colunas = colunas;
            this._idCorpo = idCorpo;
            this._fechado = fechado;
            this._iconeHeader = iconeHeader;
            this._botoes = botoes;
            this._htmlAttributes = htmlAttributes;

            this.BeginHelper();
        }

        #region [ IDisposable ]

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(Boolean disposing)
        {
            if (!this._isDisposed)
            {
                this._isDisposed = true;
                this.EndHelper();
                this._viewContext.Writer.Flush();
            }
        }

        #endregion

        #region [ Métodos de escrita ]

        private void BeginHelper()
        {
            StringBuilder codigo = new StringBuilder();
            codigo.AppendFormat("<div {0}>", this.ObterAtributosHTML())
                  .Append("     <div class=\""+this._colunas+"\">")
                  .Append("         <div style=\"overflow-x:auto;\" class=\"box border raizen\">")
                  .Append("             <div class=\"box-title\">")
                  .AppendFormat("           <h4><i class=\"fa {0}\"></i>{1}</h4>", this._iconeHeader ?? "fa-list", this._titulo)
                  .AppendFormat("{0}", this.GerarBotoesAcao())
                  .Append("         </div>")
                  .AppendFormat("<div style=\"padding-right:15px\" class=\"box-body\" id=\"{0}\" {1}>", this._idCorpo, (this._fechado ? "style='display: none;'" : ""))
                  .Append("     <div class=\"row\">")
                  .Append("     <div class=\"col-md-12\">");


            this._viewContext.Writer.WriteLine(codigo.ToString());
        }

        private string ObterAtributosHTML()
        {
            //if (this._htmlAttributes == null)
            //    return string.Empty;

            var customAttributes = HtmlHelper.AnonymousObjectToHtmlAttributes(this._htmlAttributes);

            StringBuilder sb = new StringBuilder();
            if (customAttributes.ContainsKey("class"))
            {
                customAttributes["class"] = string.Concat("row collapsible-panel ", customAttributes["class"]);
            }
            else
            {
                customAttributes.Add("class", "row collapsible-panel");
            }

            foreach (var item in customAttributes)
            {
                sb.AppendFormat("{0}=\"{1}\"", item.Key, item.Value);
            }

            return sb.ToString();
        }

        private string GerarBotoesAcao()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<div class=\"tools hidden-xs\">");
            if (this._botoes != null && this._botoes.Length > 0)
            {
                this._botoes.ToList().ForEach(b =>
                {
                    sb.Append(b.SerializarParaHTML());
                });
            }
            sb.AppendFormat("  <a class=\"{0}\" style=\"cursor:pointer\">", this._fechado ? "expand" : "collapse");
            sb.AppendFormat("      <i class=\"fa {0}\"></i>", (this._fechado ? "fa-chevron-down" : "fa-chevron-up"));
            sb.Append("  </a>");
            sb.Append("</div>");

            return sb.ToString();
        }

        private void EndHelper()
        {
            this._viewContext.Writer.WriteLine("</div></div></div></div></div></div>");
        }

        #endregion
    }

    public class ActionButtonCollapsiblePanel
    {
        public string Icone { get; set; }
        public string FuncaoJS { get; set; }

        public string SerializarParaHTML()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("  <a style=\"cursor:pointer\">");
            sb.AppendFormat("      <i class=\"fa {0}\" {1}></i>",
                this.Icone,
                string.IsNullOrEmpty(this.FuncaoJS) ? string.Empty : $"onclick=\"{this.FuncaoJS}\"");
            sb.Append("  </a>");

            return sb.ToString();
        }
    }
}