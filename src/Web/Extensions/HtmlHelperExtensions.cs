using Raizen.UniCad.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace Raizen.UniCad
{
    public enum ButtonType
    {
        BUTTON,
        SUBMIT,
        RESET
    }

    public enum MessageType
    {
        SUCCESS,
        INFO,
        WARNING,
        ERROR
    }

    public enum TargetType
    {
        BLANK,
        PARENT,
        SELF,
        TOP
    }

    public class HtmlHelperOption
    {
        public string colunas { get; set; }

        public string icone { get; set; }

        public string iconeLabelSubItem { get; set; }

        public bool inLine { get; set; }

        public bool hideLabel { get; set; }

        public bool hideIcone { get; set; }

        public string inLineLabelColuna { get; set; }

        public string inLineControleColuna { get; set; }

        public bool isFormGroup { get; set; }

        //mostrar texto (selecione) nda dropdownlist
        public bool mostraTextoVazio { get; set; }

        public bool argentina { get; set; }

        public HtmlHelperOption()
        {
            hideLabel = false;
            inLine = false;
            mostraTextoVazio = true;
            hideIcone = false;
            isFormGroup = false;
        }
    }

    public static partial class HtmlHelperExtensions
    {
        const string stringLengthKey = "CARACTERES_OBSERVACAO";
        const string labelClasses = "control-label";
        const string dropDownEmptyText = "(Selecione)";
        const string dropDownEmptyArgText = "(Seleccione)";
        const string textClasses = "form-control";
        const string currencyClasses = "form-control text-right";
        const string dateClasses = "form-control";
        const string timeClasses = "form-control timepicker";
        const string textCPFMask = "999.999.999-99";
        const string textCNPJMask = "99.999.999/9999-99";
        const string textDNIMask = "99.999.999";
        const string textCEPMask = "99999-999";
        const string textCNPJRaizMask = "?99.999.999/9999-99";
        const string textDateMask = "99/99/9999";
        const string textTimeMask = "99:99";
        const string textDateFormat = "{0:dd/MM/yyyy}";
        const string textTimeFormat = "{0:HH:mm:ss}";
        const string textCurrencyFormat = "{0:N2}";
        const string attachmentClasses = "anexoCount";
        const string attachmentTypes = "file";

        #region TEXTBOX

        #region SIMPLE TEXT
        public static MvcHtmlString BootstrapTextBoxFor<TModel, TProperty>(this HtmlHelper<TModel> html, Expression<Func<TModel, TProperty>> expression, string format = null, string labelText = null, object htmlAttributes = null, HtmlHelperOption option = null, bool desabilitado = false, string valorAnterior = null)
        {
            var _attributes = HtmlHelper.AnonymousObjectToHtmlAttributes(new { @class = textClasses });
            object _disable = new { @disabled = true };
            if (desabilitado)
                _attributes = _attributes.Combine(_disable);
            if (htmlAttributes != null) _attributes = _attributes.Combine(htmlAttributes);
            var _label = BootstrapLabel<TModel, TProperty>(html, expression, labelText);
            var _textBox = html.TextBoxFor(expression, format, _attributes);
            return BootstrapFormContainter(_label, _textBox, option, valorAnterior);
        }

        public static MvcHtmlString BootstrapTextBox(this HtmlHelper html, string name, string format = null, string labelText = null, object htmlAttributes = null, HtmlHelperOption option = null, string valor = null)
        {
            var _attributes = HtmlHelper.AnonymousObjectToHtmlAttributes(new { @class = textClasses });
            if (htmlAttributes != null) _attributes = _attributes.Combine(htmlAttributes);
            var _label = BootstrapLabel(html, !string.IsNullOrEmpty(labelText) ? labelText : name, "lbl_" + name);

            var _textBox = html.TextBox(name, valor, format, _attributes);
            return BootstrapFormContainter(_label, _textBox, option);
        }
        #endregion

        #region CPF TEXT
        public static MvcHtmlString BootstrapCPFBoxFor<TModel, TProperty>(this HtmlHelper<TModel> html, Expression<Func<TModel, TProperty>> expression, string labelText = null, object htmlAttributes = null, HtmlHelperOption option = null)
        {
            var _attributes = HtmlHelper.AnonymousObjectToHtmlAttributes(new
            {
                @class = textClasses,
                data_mask = textCPFMask,
                maxlength = 18
            });
            if (htmlAttributes != null) _attributes = _attributes.Combine(htmlAttributes);
            var _label = BootstrapLabel<TModel, TProperty>(html, expression, labelText);
            var _textBox = html.TextBoxFor(expression, _attributes);
            return BootstrapFormContainter(_label, _textBox, option);
        }
        #endregion

        #region CNPJ TEXT
        public static MvcHtmlString BootstrapCNPJBoxFor<TModel, TProperty>(this HtmlHelper<TModel> html, Expression<Func<TModel, TProperty>> expression, string labelText = null, object htmlAttributes = null, HtmlHelperOption option = null)
        {
            var _attributes = HtmlHelper.AnonymousObjectToHtmlAttributes(new
            {
                @class = textClasses,
                data_mask = textCNPJMask,
                maxlength = 18
            });
            if (htmlAttributes != null) _attributes = _attributes.Combine(htmlAttributes);
            var _label = BootstrapLabel<TModel, TProperty>(html, expression, labelText);
            var _textBox = html.TextBoxFor(expression, _attributes);
            return BootstrapFormContainter(_label, _textBox, option);
        }
        public static MvcHtmlString BootstrapCEPBoxFor<TModel, TProperty>(this HtmlHelper<TModel> html, Expression<Func<TModel, TProperty>> expression, string labelText = null, object htmlAttributes = null, HtmlHelperOption option = null)
        {
            var _attributes = HtmlHelper.AnonymousObjectToHtmlAttributes(new
            {
                @class = textClasses,
                data_mask = textCEPMask,
                maxlength = 9
            });
            if (htmlAttributes != null) _attributes = _attributes.Combine(htmlAttributes);
            var _label = BootstrapLabel<TModel, TProperty>(html, expression, labelText);
            var _textBox = html.TextBoxFor(expression, _attributes);
            return BootstrapFormContainter(_label, _textBox, option);
        }
        public static MvcHtmlString BootstrapCNPJBox(this HtmlHelper html, string name, string labelText = null, object htmlAttributes = null)
        {
            var _attributes = HtmlHelper.AnonymousObjectToHtmlAttributes(new
            {
                @class = textClasses,
                data_mask = textCNPJMask,
                maxlength = 18
            });
            if (htmlAttributes != null) _attributes = _attributes.Combine(htmlAttributes);
            var _label = BootstrapLabel(html, !string.IsNullOrEmpty(labelText) ? labelText : name);
            var _textBox = html.TextBox(name, null, _attributes);
            return BootstrapFormContainter(_label, _textBox);
        }

        public static MvcHtmlString BootstrapCNPJRaizBoxFor<TModel, TProperty>(this HtmlHelper<TModel> html, Expression<Func<TModel, TProperty>> expression, string labelText = null, object htmlAttributes = null, HtmlHelperOption option = null)
        {
            var _attributes = HtmlHelper.AnonymousObjectToHtmlAttributes(new
            {
                @class = textClasses,
                data_mask = textCNPJRaizMask,
                maxlength = 11
            });
            if (htmlAttributes != null) _attributes = _attributes.Combine(htmlAttributes);
            var _label = BootstrapLabel<TModel, TProperty>(html, expression, labelText);
            var _textBox = html.TextBoxFor(expression, _attributes);
            return BootstrapFormContainter(_label, _textBox, option);
        }

        public static MvcHtmlString BootstrapCNPJRaizPesquisaBoxFor<TModel, TProperty>(this HtmlHelper<TModel> html, Expression<Func<TModel, TProperty>> expression, string labelText = null, object htmlAttributes = null, HtmlHelperOption option = null)
        {
            var _attributes = HtmlHelper.AnonymousObjectToHtmlAttributes(new
            {
                @class = textClasses,
                data_mask = textCNPJRaizMask,
                maxlength = 11
            });
            if (htmlAttributes != null) _attributes = _attributes.Combine(htmlAttributes);
            var _label = BootstrapLabel<TModel, TProperty>(html, expression, labelText);
            var _textBox = html.TextBoxFor(expression, _attributes);
            var _btn = BootstrapButtonFilterPesquisa<TModel, TProperty>(html, expression);
            return BootstrapFormComPesquisaContainter(_label, _textBox, _btn, option);
        }
        #endregion

        #region DNI TEXT
        public static MvcHtmlString BootstrapDNIBoxFor<TModel, TProperty>(this HtmlHelper<TModel> html, Expression<Func<TModel, TProperty>> expression, string labelText = null, object htmlAttributes = null, HtmlHelperOption option = null)
        {
            var _attributes = HtmlHelper.AnonymousObjectToHtmlAttributes(new
            {
                @class = textClasses,
                data_mask = textDNIMask,
                maxlength = 10
            });
            if (htmlAttributes != null) _attributes = _attributes.Combine(htmlAttributes);
            var _label = BootstrapLabel<TModel, TProperty>(html, expression, labelText);
            var _textBox = html.TextBoxFor(expression, _attributes);
            return BootstrapFormContainter(_label, _textBox, option);
        }
        #endregion

        #region IBM TEXT
        public static MvcHtmlString BootstrapIBMBoxFor<TModel, TProperty>(this HtmlHelper<TModel> html, Expression<Func<TModel, TProperty>> expression, string labelText = null, object htmlAttributes = null, HtmlHelperOption option = null, bool formataIBM = true)
        {
            if (option == null)
            {
                option = new HtmlHelperOption();
            }
            option.icone = "fa fa-qq";

            var _attributes = HtmlHelper.AnonymousObjectToHtmlAttributes(new
            {
                @class = textClasses,
                data_ibm = formataIBM,
                data_ibm_pesquisa = false,
                maxlength = 10
            });

            if (htmlAttributes != null) _attributes = _attributes.Combine(htmlAttributes);

            var _label = BootstrapLabel<TModel, TProperty>(html, expression, labelText);
            var _textBox = html.TextBoxFor(expression, _attributes);
            var _elementShow = BootstrapSmallHelp<TModel, TProperty>(html, expression);
            return BootstrapFormContainterComIcone(_label, _textBox, _elementShow, option);
        }

        public static MvcHtmlString BootstrapIBMBox(this HtmlHelper html, string name, string labelText = null, object htmlAttributes = null, bool formataIBM = true)
        {
            var _attributes = HtmlHelper.AnonymousObjectToHtmlAttributes(new
            {
                @class = textClasses,
                data_ibm = formataIBM,
                maxlength = 10
            });
            if (htmlAttributes != null) _attributes = _attributes.Combine(htmlAttributes);
            var _label = BootstrapLabel(html, !string.IsNullOrEmpty(labelText) ? labelText : name);
            var _textBox = html.TextBox(name, _attributes);
            return BootstrapFormContainter(_label, _textBox);
        }
        #endregion

        #region DATE TEXT
        public static MvcHtmlString BootstrapDateBoxFor<TModel, TProperty>(this HtmlHelper<TModel> html, Expression<Func<TModel, TProperty>> expression, string labelText = null, object htmlAttributes = null, HtmlHelperOption option = null, string valorAnterior = null, bool semMascara = false, bool desabilitado = false)
        {
            if (option == null)
            {
                option = new HtmlHelperOption();
            }
            option.icone = "fa fa-calendar";


            object __attributes;
            if (semMascara)
            {
                __attributes = new
                {
                    @class = dateClasses,
                    maxlength = 10,
                    data_type = "date",
                    value = ""
                };
            }
            else
            {
                __attributes = new
                {
                    @class = dateClasses,
                    data_mask = textDateMask,
                    maxlength = 10,
                    data_type = "date",
                    value = ""
                };
            }

            var _attributes = HtmlHelper.AnonymousObjectToHtmlAttributes(__attributes);
            if (desabilitado)
                _attributes.Add("disabled", true);

            if (htmlAttributes != null) _attributes = _attributes.Combine(htmlAttributes);
            var _label = BootstrapLabel<TModel, TProperty>(html, expression, labelText);
            var _textBox = html.TextBoxFor(expression, textDateFormat, _attributes);
            _textBox = new MvcHtmlString(_textBox.ToHtmlString().Replace("value=\"01/01/0001\"", ""));
            return BootstrapFormContainterComIcone(_label, _textBox, null, option, valorAnterior);
        }

        public static MvcHtmlString BootstrapDateBox(this HtmlHelper html, string name, string labelText = null, object htmlAttributes = null, HtmlHelperOption option = null)
        {
            if (option == null)
            {
                option = new HtmlHelperOption();
            }
            option.icone = "fa fa-calendar";
            var _attributes = HtmlHelper.AnonymousObjectToHtmlAttributes(new
            {
                @class = textClasses,
                data_mask = textDateMask,
                maxlength = 10,
                data_type = "date"
            });
            if (htmlAttributes != null) _attributes = _attributes.Combine(htmlAttributes);
            var _label = BootstrapLabel(html, !string.IsNullOrEmpty(labelText) ? labelText : name);
            var _textBox = html.TextBox(name, null, textDateFormat, _attributes);
            return BootstrapFormContainterComIcone(_label, _textBox, null, option);
        }
        #endregion

        #region TIME TEXT
        public static MvcHtmlString BootstrapTimeBoxFor<TModel, TProperty>(this HtmlHelper<TModel> html, Expression<Func<TModel, TProperty>> expression, string labelText = null, object htmlAttributes = null, HtmlHelperOption option = null)
        {
            if (option == null)
            {
                option = new HtmlHelperOption();
            }
            option.icone = "fa fa-clock-o";

            var _attributes = HtmlHelper.AnonymousObjectToHtmlAttributes(new
            {
                @class = timeClasses,
                data_mask = textTimeMask,
                maxlength = 5,
                data_type = "time"
            });
            if (htmlAttributes != null) _attributes = _attributes.Combine(htmlAttributes);
            var _label = BootstrapLabel<TModel, TProperty>(html, expression, labelText);
            var _textBox = html.TextBoxFor(expression, null, _attributes);

            return BootstrapFormContainterComIcone(_label, _textBox, null, option);
        }

        public static MvcHtmlString BootstrapTimeBox(this HtmlHelper html, string name, string labelText = null, object htmlAttributes = null)
        {
            var _attributes = HtmlHelper.AnonymousObjectToHtmlAttributes(new
            {
                @class = timeClasses,
                data_mask = textTimeMask,
                maxlength = 8
            });
            if (htmlAttributes != null) _attributes = _attributes.Combine(htmlAttributes);
            var _label = BootstrapLabel(html, !string.IsNullOrEmpty(labelText) ? labelText : name);
            var _textBox = html.TextBox(name, null, textTimeFormat, _attributes);
            return BootstrapFormContainter(_label, _textBox);
        }
        #endregion

        #region CURRENCY TEXT
        public static MvcHtmlString BootstrapCurrencyBoxFor<TModel, TProperty>(
            this HtmlHelper<TModel> html,
            Expression<Func<TModel, TProperty>> expression,
            string labelText = null,
            object htmlAttributes = null,
            HtmlHelperOption option = null,
            string currencyFormat = null,
            int quantidadeCasasDecimais = 0,
            bool desabilitado = false,
            string valorAnterior = null)
        {
            var _attributes = HtmlHelper.AnonymousObjectToHtmlAttributes(new
            {
                @class = currencyClasses,
                data_currency_field = true,
                data_currency_field_inline = option != null && option.inLine ? true : false,
                currency_precision = quantidadeCasasDecimais
            });
            object _disable = new { @disabled = true };

            if (desabilitado)
                _attributes = _attributes.Combine(_disable);

            if (htmlAttributes != null) _attributes = _attributes.Combine(htmlAttributes);
            var _label = BootstrapLabel(html, expression, labelText);
            var _textBox = html.TextBoxFor(expression, string.IsNullOrEmpty(currencyFormat) ? textCurrencyFormat : currencyFormat, _attributes);
            return BootstrapCurrencyBoxContainter(_label, _textBox, option, valorAnterior);
        }

        public static MvcHtmlString BootstrapLetterBoxFor<TModel, TProperty>(
           this HtmlHelper<TModel> html,
           Expression<Func<TModel, TProperty>> expression,
           string labelText = null,
           object htmlAttributes = null,
           HtmlHelperOption option = null)
        {
            var _attributes = HtmlHelper.AnonymousObjectToHtmlAttributes(new { @class = textClasses, data_letter = true, data_nospace = true, data_upper = true });
            if (htmlAttributes != null) _attributes = _attributes.Combine(htmlAttributes);
            var _label = BootstrapLabel(html, expression, labelText);
            var _textBox = html.TextBoxFor(expression, null, _attributes);
            return BootstrapFormContainter(_label, _textBox, option);
        }

        public static MvcHtmlString BootstrapCurrencyBox(
            this HtmlHelper html,
            string name,
            string labelText = null,
            object htmlAttributes = null,
            HtmlHelperOption option = null)
        {
            var _attributes = HtmlHelper.AnonymousObjectToHtmlAttributes(new { @class = textClasses, data_currency_field = true, data_currency_field_inline = option != null && option.inLine ? true : false });
            if (htmlAttributes != null) _attributes = _attributes.Combine(htmlAttributes);
            var _label = BootstrapLabel(html, !string.IsNullOrEmpty(labelText) ? labelText : name);
            var _textBox = html.TextBox(name, null, textCurrencyFormat, _attributes);
            return BootstrapCurrencyBoxContainter(_label, _textBox, option);
        }
        #endregion

        public static MvcHtmlString BootstrapNumberBoxFor<TModel, TProperty>(this HtmlHelper<TModel> html, Expression<Func<TModel, TProperty>> expression, string format = null, string labelText = null, object htmlAttributes = null, HtmlHelperOption option = null, string valorAnterior = null, bool iconeAjuda = false)
        {
            var _attributes = HtmlHelper.AnonymousObjectToHtmlAttributes(new { @class = textClasses, data_number_field = true, @type = "number", @maxlength = Math.Min(int.MaxValue.ToString().Length, int.MaxValue.ToString().Length) });
            if (htmlAttributes != null) _attributes = _attributes.Combine(htmlAttributes);
            var _label = BootstrapLabel<TModel, TProperty>(html, expression, labelText);
            var _textBox = html.TextBoxFor(expression, format, _attributes);
            return BootstrapFormContainter(_label, _textBox, option, valorAnterior, iconeAjuda);
        }

        #endregion

        #region TEXTAREA
        public static MvcHtmlString BootstrapTextAreaFor<TModel, TProperty>(this HtmlHelper<TModel> html, Expression<Func<TModel, TProperty>> expression, string labelText = null, object htmlAttributes = null, HtmlHelperOption option = null, bool desabilitado = false)
        {
            var member = expression.Body as MemberExpression;
            var stringLength = member.Member.GetCustomAttributes(typeof(StringLengthAttribute), false).FirstOrDefault() as StringLengthAttribute;

            var _attributes = HtmlHelper.AnonymousObjectToHtmlAttributes(new
            {
                @class = textClasses,
                maxlength = stringLength != null ? stringLength.MaximumLength : 8000
            });
            object _disable = new { @disabled = true };
            if (desabilitado)
                _attributes = _attributes.Combine(_disable);
            if (htmlAttributes != null) _attributes = _attributes.Combine(htmlAttributes);
            var _label = BootstrapLabel<TModel, TProperty>(html, expression, labelText);
            var _textarea = html.TextAreaFor(expression, _attributes);
            return BootstrapFormContainter(_label, _textarea, option);
        }

        public static MvcHtmlString BootstrapTextArea(this HtmlHelper html, string name, string labelText = null, object htmlAttributes = null)
        {
            var _attributes = HtmlHelper.AnonymousObjectToHtmlAttributes(new { @class = textClasses });
            if (htmlAttributes != null) _attributes = _attributes.Combine(htmlAttributes);
            var _label = BootstrapLabel(html, !string.IsNullOrEmpty(labelText) ? labelText : name);
            var _textarea = html.TextArea(name, _attributes);
            return BootstrapFormContainter(_label, _textarea);
        }
        #endregion

        #region POPOVER
        public static MvcHtmlString PopoverTextFor<TModel, TProperty>(this HtmlHelper<TModel> html, Expression<Func<TModel, TProperty>> expression, int? maxLength, string side = "left")
        {
            var _configLenght = Convert.ToInt32(ConfigurationManager.AppSettings.Get(stringLengthKey));
            var _maxlength = (maxLength.HasValue ? maxLength.Value : _configLenght) - 3;

            var _x = ModelMetadata.FromLambdaExpression(expression, html.ViewData);

            if (_x.Model == null) return new MvcHtmlString("");

            var _text = (string)_x.Model;
            if (_text.Length > _maxlength)
            {
                var _str = new StringBuilder();
                _str.AppendFormat("<p class=\"pop-hover \" data-content=\"{0}\" data-placement=\"" + side + "\">", _text.Replace("\"", "&#34;"));
                _str.AppendFormat("{0}...", _text.Substring(0, _maxlength));
                _str.Append("</p>");
                return new MvcHtmlString(_str.ToString());
            }
            else
            {
                return html.DisplayFor(expression);
            }

        }

        [SuppressMessage("Microsoft.Usage", "CA1801", Justification = "Apesar de não usado, 'html' é requerido como regra para extenção.")]
        public static MvcHtmlString PopoverText(this HtmlHelper html, string text, string popovertxt, int? maxLength, string side = "left")
        {
            var _str = new StringBuilder();
            var _configLenght = Convert.ToInt32(ConfigurationManager.AppSettings.Get(stringLengthKey));
            var _maxlength = (maxLength.HasValue ? maxLength.Value : _configLenght) - 3;

            if (text != null && text.Length > _maxlength)
            {
                _str.AppendFormat("<p class=\"pop-hover \" data-content=\"{0}\" data-placement=\"" + side + "\">", text);
                _str.AppendFormat("{0}...", text.Substring(0, _maxlength));
                _str.Append("</p>");
            }
            else
            {
                _str.AppendFormat(text);
            }

            return new MvcHtmlString(_str.ToString());
        }
        #endregion

        #region DROPDOWNLIST
        public static MvcHtmlString BootstrapDropDownListFor<TModel, TProperty>(this HtmlHelper<TModel> html, Expression<Func<TModel, TProperty>> expression, IEnumerable<SelectListItem> data = null, string labelText = null, object htmlAttributes = null, HtmlHelperOption option = null, bool desabilitado = false, string valorAnterior = null, bool iconeAjuda = false)
        {
            var _attributes = HtmlHelper.AnonymousObjectToHtmlAttributes(new { data_selectize = true, @class = textClasses });

            object _disable = new { @disabled = true };
            if (option == null) option = new HtmlHelperOption();
            if (htmlAttributes != null) _attributes = _attributes.Combine(htmlAttributes);

            if (desabilitado)
                _attributes.Add("disabled", true);

            if (data == null) data = new List<SelectListItem>();
            var _label = BootstrapLabel(html, expression, labelText, option);
            var _dropDown = html.DropDownListFor(expression, data, option.mostraTextoVazio ? (option.argentina ? dropDownEmptyArgText : dropDownEmptyText) : null, _attributes);

            return BootstrapDropDownListContainter(_label, _dropDown, option, valorAnterior, iconeAjuda);
        }

        public static MvcHtmlString BootstrapDropDownList(this HtmlHelper html, string name, IEnumerable<SelectListItem> data,
            string labelText = null, object htmlAttributes = null, HtmlHelperOption option = null)
        {
            var _attributes = HtmlHelper.AnonymousObjectToHtmlAttributes(new { data_selectize = true, @class = textClasses });
            if (option == null) option = new HtmlHelperOption();
            if (htmlAttributes != null) _attributes = _attributes.Combine(htmlAttributes);
            if (data == null) data = new List<SelectListItem>();
            var _label = BootstrapLabel(html, !string.IsNullOrEmpty(labelText) ? labelText : name);
            var _dropDown = html.DropDownList(name, data, option.mostraTextoVazio ? dropDownEmptyText : null, _attributes);
            return BootstrapDropDownListContainter(_label, _dropDown, option);
        }

        #region DropDownList

        public static MvcHtmlString DropDownListEnum<TEnum>(this HtmlHelper html, string nome = null, string labelText = null, bool incluirSelecione = true, object htmlAttributes = null) where TEnum : class
        {
            var _attributes = HtmlHelper.AnonymousObjectToHtmlAttributes(new { data_selectize = true });
            if (htmlAttributes != null) _attributes = _attributes.Combine(htmlAttributes);
            var enumList = EnumExtensions.GetKeyValueList<TEnum>();
            var selectList = (from e in enumList select new SelectListItem { Text = e.Value, Value = e.Key.ToString(CultureInfo.InvariantCulture) }).ToList();
            if (incluirSelecione) selectList.Insert(0, new SelectListItem { Value = "", Text = "(Selecione)" });
            nome = string.IsNullOrEmpty(nome) ? typeof(TEnum).Name : nome;
            var _label = BootstrapLabel(html, !string.IsNullOrEmpty(labelText) ? labelText : nome);
            var _dropDown = html.DropDownList(nome, selectList, dropDownEmptyText, _attributes);
            return BootstrapFormContainter(_label, _dropDown);
        }

        public static MvcHtmlString DropDownListEnumFor<TModel, TValue>(this HtmlHelper<TModel> html,
            Expression<Func<TModel, TValue>> expression,
            Type type,
            string labelText = null,
            bool incluirSelecione = true,
            object htmlAttributes = null,
            HtmlHelperOption option = null,
            bool valueKey = true)
        {
            if (option == null) option = new HtmlHelperOption();

            var _attributes = HtmlHelper.AnonymousObjectToHtmlAttributes(new { data_selectize = true });
            if (htmlAttributes != null) _attributes = _attributes.Combine(htmlAttributes);

            var enumList = EnumExtensions.GetKeyValueList(type);
            var selectList = (from e in enumList select new SelectListItem { Text = e.Value, Value = valueKey ? e.Key.ToString(CultureInfo.InvariantCulture) : e.Value }).ToList();
            if (incluirSelecione) selectList.Insert(0, new SelectListItem { Value = "", Text = "(Selecione)" });

            var _label = BootstrapLabel<TModel, TValue>(html, expression, labelText);
            var _dropDown = html.DropDownListFor(expression, selectList, option.mostraTextoVazio ? dropDownEmptyText : null, _attributes);
            return BootstrapFormContainter(_label, _dropDown, option);
        }
        #endregion
        #endregion

        #region CHECKBOX
        public static MvcHtmlString BootstrapCheckBoxFor<TModel>(this HtmlHelper<TModel> html, Expression<Func<TModel, Nullable<bool>>> expression, object htmlAttributes = null)
        {
            var _checkbox = html.EditorFor(expression, htmlAttributes);
            var _display = html.DisplayNameFor(expression);

            return BootstrapCheckBoxContainter(_display, _checkbox);
        }

        public static MvcHtmlString BootstrapCheckBoxFor<TModel>(this HtmlHelper<TModel> html, Expression<Func<TModel, bool>> expression, object htmlAttributes = null)
        {
            var _checkbox = html.EditorFor(expression, htmlAttributes);
            var _display = html.DisplayNameFor(expression);

            return BootstrapCheckBoxContainter(_display, _checkbox);
        }

        public static MvcHtmlString BootstrapCheckBox(this HtmlHelper html, string name, string labelText = null, object htmlAttributes = null)
        {
            var _checkbox = html.CheckBox(name, htmlAttributes);
            var _display = new MvcHtmlString(!string.IsNullOrEmpty(labelText) ? labelText : name);

            return BootstrapCheckBoxContainter(_display, _checkbox);
        }
        #endregion

        #region FILE
        public static MvcHtmlString BootstrapFileFor<TModel, TProperty>(this HtmlHelper<TModel> html, Expression<Func<TModel, TProperty>> expression, string labelText = null, bool multiple = false, object htmlAttributes = null, HtmlHelperOption option = null)
        {
            return BootstrapFile(html, ((MemberExpression)expression.Body).Member.Name, labelText, multiple, htmlAttributes, option);
        }

        public static MvcHtmlString BootstrapFile(this HtmlHelper html, string name, string labelText = null, bool multiple = false, object htmlAttributes = null, HtmlHelperOption option = null)
        {
            var _attributes = HtmlHelper.AnonymousObjectToHtmlAttributes(new
            {
                @class = attachmentClasses,
                type = attachmentTypes,
                name = name,
                multiple,
                @data_buttonText = "Selecione",
                @data_iconName = "glyphicon glyphicon-file"
            });

            if (!multiple)
            {
                _attributes.Remove("multiple");
            }

            if (htmlAttributes != null)
            {
                _attributes = _attributes.Combine(htmlAttributes);
            }
            var _label = BootstrapLabel(html, !string.IsNullOrEmpty(labelText) ? labelText : name);
            var _file = html.TextBox(name, null, _attributes);
            //var _file = new MvcHtmlString(string.Format("<input type=\"file\" class=\"form-control\" name=\"{0}\" id=\"{0}\" {1}>", name, multiple ? "multiple" : null));
            return BootstrapFormContainter(_label, _file, option);
        }
        #endregion

        #region BUTTON

        private static MvcHtmlString BootstrapButtonFilterPesquisa<TModel, TProperty>(HtmlHelper<TModel> html, Expression<Func<TModel, TProperty>> expression)
        {
            MvcHtmlString _btn;
            _btn = new MvcHtmlString(string.Format("<button type=\"button\" class=\"btn btn-default\" id=\"btn_{0}\"><span class=\"fa fa-search\"></span></button>", html.IdFor(expression).ToHtmlString()));

            return _btn;
        }




        /// <summary>
        /// Retorna um HTML de um Botão de Filtro formatado conforme as convenções do Bootstrap 3.
        /// </summary>
        /// <param name="html"></param>
        /// <param name="id">Nome do Botão</param>
        /// <param name="text">Opcional. Texto a ser exibido no Botão.</param>
        /// <param name="behaviour">Comportamento do botão. Se não for informado assumirá o comportamento de 'button'.</param>
        /// <returns>HTML do Botão de Filtro formatado conforme as convenções do Bootstrap 3.</returns>
        [SuppressMessage("Microsoft.Usage", "CA1801", Justification = "Apesar de não usado, 'html' é requerido como regra para extenção.")]
        public static MvcHtmlString BootstrapButtonFilter(this HtmlHelper html, string id, string text = null, ButtonType behaviour = ButtonType.BUTTON)
        {
            return BootstrapButton(id, text, "fa-filter", behaviour);
        }

        /// <summary>
        /// Retorna um HTML de um Botão de Limpar formatado conforme as convenções do Bootstrap 3.
        /// </summary>
        /// <param name="html"></param>
        /// <param name="id">Nome do Botão</param>
        /// <param name="text">Opcional. Texto a ser exibido no Botão.</param>
        /// <param name="behaviour">Comportamento do botão. Se não for informado assumirá o comportamento de 'button'.</param>
        /// <returns>HTML do Botão de Limpar filtro formatado conforme as convenções do Bootstrap 3.</returns>
        [SuppressMessage("Microsoft.Usage", "CA1801", Justification = "Apesar de não usado, 'html' é requerido como regra para extenção.")]
        public static MvcHtmlString BootstrapButtonClear(this HtmlHelper html, string id, string text = null, ButtonType behaviour = ButtonType.RESET)
        {
            return BootstrapButton(id, text, "fa-eraser", behaviour);
        }

        [SuppressMessage("Microsoft.Usage", "CA1801", Justification = "Apesar de não usado, 'html' é requerido como regra para extenção.")]
        public static MvcHtmlString BootstrapButtonClearFilter(this HtmlHelper html, string id, ButtonType behaviour = ButtonType.RESET)
        {
            return BootstrapButton(id, "Limpar Filtro", "fa-eraser", behaviour);
        }

        /// <summary>
        /// Retorna um HTML de um Botão de Busca formatado conforme as convenções do Bootstrap 3.
        /// </summary>
        /// <param name="html"></param>
        /// <param name="id">Nome do Botão</param>
        /// <param name="text">Opcional. Texto a ser exibido no Botão.</param>
        /// <param name="behaviour">Comportamento do botão. Se não for informado assumirá o comportamento de 'button'.</param>
        /// <returns>HTML do Botão de Busca formatado conforme as convenções do Bootstrap 3.</returns>
        [SuppressMessage("Microsoft.Usage", "CA1801", Justification = "Apesar de não usado, 'html' é requerido como regra para extenção.")]
        public static MvcHtmlString BootstrapButtonSearch(this HtmlHelper html, string id, string text = null, ButtonType behaviour = ButtonType.BUTTON)
        {
            return BootstrapButton(id, text, "fa fa-search", behaviour);
        }

        /// <summary>
        /// Retorna um HTML de um Botão de Adição formatado conforme as convenções do Bootstrap 3.
        /// </summary>
        /// <param name="html"></param>
        /// <param name="id">Nome do Botão</param>
        /// <param name="text">Opcional. Texto a ser exibido no Botão.</param>
        /// <param name="behaviour">Comportamento do botão. Se não for informado assumirá o comportamento de 'button'.</param>
        /// <param name="clientClick"></param>
        /// <param name="htmlAttributes"></param>
        /// <returns>HTML do Botão de Adição formatado conforme as convenções do Bootstrap 3.</returns>
        [SuppressMessage("Microsoft.Usage", "CA1801", Justification = "Apesar de não usado, 'html' é requerido como regra para extenção.")]
        public static MvcHtmlString BootstrapButtonInfo(this HtmlHelper html, string id, string text = null, ButtonType behaviour = ButtonType.BUTTON, string clientClick = null, string htmlAttributes = null)
        {
            return BootstrapButton(id, text, "fa-info", behaviour, clientClick, htmlAttributes);
        }

        /// <summary>
        /// Retorna um HTML de um Botão de Adição formatado conforme as convenções do Bootstrap 3.
        /// </summary>
        /// <param name="html"></param>
        /// <param name="id">Nome do Botão</param>
        /// <param name="text">Opcional. Texto a ser exibido no Botão.</param>
        /// <param name="behaviour">Comportamento do botão. Se não for informado assumirá o comportamento de 'button'.</param>
        /// <param name="clientClick"></param>
        /// <param name="htmlAttributes"></param>
        /// <returns>HTML do Botão de Adição formatado conforme as convenções do Bootstrap 3.</returns>
        [SuppressMessage("Microsoft.Usage", "CA1801", Justification = "Apesar de não usado, 'html' é requerido como regra para extenção.")]
        public static MvcHtmlString BootstrapButtonAdd(this HtmlHelper html, string id, string text = null, ButtonType behaviour = ButtonType.BUTTON, string clientClick = null, string htmlAttributes = null)
        {
            return BootstrapButton(id, text, "fa-plus", behaviour, clientClick, htmlAttributes);
        }

        /// <summary>
        /// Retorna um HTML de um Botão de Deleção formatado conforme as convenções do Bootstrap 3.
        /// </summary>
        /// <param name="html"></param>
        /// <param name="id">Nome do Botão</param>
        /// <param name="text">Opcional. Texto a ser exibido no Botão.</param>
        /// <param name="behaviour">Comportamento do botão. Se não for informado assumirá o comportamento de 'button'.</param>
        /// <param name="clientClick"></param>
        /// <param name="htmlAttributes"></param>
        /// <returns>HTML do Botão de Deleção formatado conforme as convenções do Bootstrap 3.</returns>
        [SuppressMessage("Microsoft.Usage", "CA1801", Justification = "Apesar de não usado, 'html' é requerido como regra para extenção.")]
        public static MvcHtmlString BootstrapButtonDelete(this HtmlHelper html, string id, string text = null, ButtonType behaviour = ButtonType.BUTTON, string clientClick = null, string htmlAttributes = null)
        {
            return BootstrapButton(id, text, "fa-trash-o", behaviour, clientClick, htmlAttributes);
        }

        /// <summary>
        /// Retorna um HTML de um Botão de Edição formatado conforme as convenções do Bootstrap 3.
        /// </summary>
        /// <param name="html"></param>
        /// <param name="id">Nome do Botão</param>
        /// <param name="text">Opcional. Texto a ser exibido no Botão.</param>
        /// <param name="behaviour">Comportamento do botão. Se não for informado assumirá o comportamento de 'button'.</param>
        /// <param name="clientClick"></param>
        /// <param name="htmlAttributes"></param>
        /// <returns>HTML do Botão de Edição formatado conforme as convenções do Bootstrap 3.</returns>
        [SuppressMessage("Microsoft.Usage", "CA1801", Justification = "Apesar de não usado, 'html' é requerido como regra para extenção.")]
        public static MvcHtmlString BootstrapButtonEdit(this HtmlHelper html, string id, string text = null, ButtonType behaviour = ButtonType.BUTTON, string clientClick = null, string htmlAttributes = null)
        {
            return BootstrapButton(id, text, "fa-pencil", behaviour, clientClick, htmlAttributes);
        }

        /// <summary>
        /// Retorna um HTML de um Botão formatado conforme as convenções do Bootstrap 3.
        /// </summary>
        /// <param name="html"></param>
        /// <param name="id">Nome do Botão</param>
        /// <param name="text">Opcional. Texto a ser exibido no Botão.</param>
        /// <param name="behaviour">Comportamento do botão. Se não for informado assumirá o comportamento de 'button'.</param>
        /// <param name="clientClick"></param>
        /// <param name="htmlAttributes"></param>
        /// <returns>HTML do Botão formatado conforme as convenções do Bootstrap 3.</returns>
        [SuppressMessage("Microsoft.Usage", "CA1801", Justification = "Apesar de não usado, 'html' é requerido como regra para extenção.")]
        public static MvcHtmlString BootstrapButton(this HtmlHelper html, string id, string text = null, ButtonType behaviour = ButtonType.BUTTON, string clientClick = null, string htmlAttributes = null)
        {
            return BootstrapButton(id, text, null, behaviour, clientClick, htmlAttributes);
        }
        /// <summary>
        /// Retorna um HTML de um Link formatado como Botão conforme as convenções do Bootstrap 3.
        /// </summary>
        /// <param name="html"></param>
        /// <param name="url">Endereço do link.</param>
        /// <param name="text">Texto</param>
        /// <param name="target">Alvo do link. Se não for informado assumirá o valor '_self'.</param>
        /// <returns>HTML de um Link formatado como Botão conforme as convenções do Bootstrap 3.</returns>
        [SuppressMessage("Microsoft.Usage", "CA1801", Justification = "Apesar de não usado, 'html' é requerido como regra para extenção.")]
        public static MvcHtmlString BootstrapLinkButton(this HtmlHelper html, string url, string text, TargetType target = TargetType.SELF)
        {
            return BootstrapLinkButton(url, text, null, target);
        }

        [SuppressMessage("Microsoft.Usage", "CA1801", Justification = "Apesar de não usado, 'html' é requerido como regra para extenção.")]
        public static MvcHtmlString GridAdd(this HtmlHelper html, string url, bool disabled, string modalwidth = "")
        {
            return html.GridAdd(url, null, disabled, modalwidth);
        }

        [SuppressMessage("Microsoft.Usage", "CA1801", Justification = "Apesar de não usado, 'html' é requerido como regra para extenção.")]
        public static MvcHtmlString GridAdd(this HtmlHelper html, string url, string callback = null, bool disabled = false, string modalwidth = "")
        {
            return html.GridForm(url, null, "fa-plus", callback, disabled, modalwidth);
        }

        [SuppressMessage("Microsoft.Usage", "CA1801", Justification = "Apesar de não usado, 'html' é requerido como regra para extenção.")]
        public static MvcHtmlString GridEdit(this HtmlHelper html, string url, bool disabled, string modalwidth = "")
        {
            return html.GridEdit(url, null, disabled, modalwidth);
        }

        [SuppressMessage("Microsoft.Usage", "CA1801", Justification = "Apesar de não usado, 'html' é requerido como regra para extenção.")]
        public static MvcHtmlString GridEdit(this HtmlHelper html, string url, string callback = null, bool disabled = false, string modalwidth = "")
        {
            return html.GridForm(url, null, "fa-pencil", callback, disabled, modalwidth);
        }
        [SuppressMessage("Microsoft.Usage", "CA1801", Justification = "Apesar de não usado, 'html' é requerido como regra para extenção.")]
        public static MvcHtmlString GridView(this HtmlHelper html, string url, string callback = null, bool disabled = false, string modalwidth = "")
        {
            return html.GridForm(url, null, "fa-eye", callback, disabled, modalwidth);
        }
        [SuppressMessage("Microsoft.Usage", "CA1801", Justification = "Apesar de não usado, 'html' é requerido como regra para extenção.")]
        public static MvcHtmlString GridForm(this HtmlHelper html, string url, string text, string iconClass = null, string callback = null, bool disabled = false, string modalwidth = "", string buttonClass = null, string titulo = null, bool isLink = false, string tooltip = null)
        {
            var _str = new StringBuilder();
            _str.Append(isLink ? "<a href=\"#\"" : "<button type=\"button\"");

            _str.AppendFormat(" class=\"{0} {1}\" data-grid-form-buttom data-form=\"{2}\" title=\"{3}\"", string.IsNullOrEmpty(buttonClass) ? (isLink ? "" : "btn btn-default") : buttonClass, disabled ? "disabled" : string.Empty, url, tooltip ?? string.Empty);

            if (!string.IsNullOrEmpty(callback)) _str.AppendFormat(" data-callback=\"{0}\"", callback);
            if (!string.IsNullOrEmpty(modalwidth)) _str.AppendFormat(" data-classname=\"{0}\"", modalwidth);
            if (!string.IsNullOrEmpty(titulo)) _str.AppendFormat(" data-titulo=\"{0}\"", titulo);

            _str.Append(">");

            if (!string.IsNullOrEmpty(iconClass)) _str.AppendFormat("<span class=\"fa {0}\"></span> ", iconClass);
            if (!string.IsNullOrEmpty(text)) _str.Append(text);

            if (isLink)
                _str.Append("</a>");
            else
                _str.Append("</button>");

            return new MvcHtmlString(_str.ToString());
        }

        [SuppressMessage("Microsoft.Usage", "CA1801", Justification = "Apesar de não usado, 'html' é requerido como regra para extenção.")]
        public static MvcHtmlString GridForm(this HtmlHelper html, string url, string text, string iconClass = null, string callback = null, bool disabled = false, TargetType target = TargetType.SELF, string buttonClass = null)
        {
            var _str = new StringBuilder("<a type=\"button\"");
            _str.AppendFormat(" class=\"{0} {1}\" href=\"{2}\" target=\"{3}\"",
                string.IsNullOrEmpty(buttonClass) ? "btn btn-default" : buttonClass,
                disabled ? "disabled" : string.Empty,
                url,
                target.ToString().ToLower(CultureInfo.InvariantCulture));

            if (!string.IsNullOrEmpty(callback)) _str.AppendFormat(" data-callback=\"{0}\"", callback);

            _str.Append(">");

            if (!string.IsNullOrEmpty(iconClass)) _str.AppendFormat("<span class=\"fa {0}\"></span> ", iconClass);
            if (!string.IsNullOrEmpty(text)) _str.Append(text);
            _str.Append("</a>");

            return new MvcHtmlString(_str.ToString());
        }

        [SuppressMessage("Microsoft.Usage", "CA1801", Justification = "Apesar de não usado, 'html' é requerido como regra para extenção.")]
        public static MvcHtmlString GridDelete(this HtmlHelper html, string url, int id, string target, string nome = null, bool disabled = false, string callback = null)
        {
            StringBuilder _str = new StringBuilder("<button type=\"button\"");
            _str.AppendFormat(" class=\"btn btn-danger {0}\"  data-botao-exluir data-id=\"{1}\" data-action=\"{2}\" data-target=\"{3}\"",
                disabled ? "disabled" : string.Empty,
                id,
                url,
                target);
            if (!string.IsNullOrEmpty(nome)) _str.AppendFormat(" data-nome=\"{0}\"", nome);
            if (!string.IsNullOrEmpty(callback)) _str.AppendFormat(" data-callback=\"{0}\"", callback);
            _str.Append("><span class=\"fa fa-times\"></span></button>");
            return new MvcHtmlString(_str.ToString());
        }
        #endregion

        #region MESSAGES
        /// <summary>
        /// Retorna um Html com uma mensagem formatada conforme as convenções do Bootstrap 3.
        /// </summary>
        /// <param name="html"></param>
        /// <param name="message">Mensagem a ser exibida.</param>
        /// <returns>Html com uma mensagem formatada conforme as convenções do Bootstrap 3.</returns>
        [SuppressMessage("Microsoft.Usage", "CA1801", Justification = "Apesar de não usado, 'html' é requerido como regra para extenção.")]
        public static MvcHtmlString BootstrapMessage(this HtmlHelper html, string message)
        {
            return BootstrapMessage(message, null);
        }
        /// <summary>
        /// Retorna um Html com uma mensagem de sucesso formatada conforme as convenções do Bootstrap 3.
        /// </summary>
        /// <param name="html"></param>
        /// <param name="message">Mensagem a ser exibida.</param>
        /// <returns>Html com uma mensagem formatada conforme as convenções do Bootstrap 3.</returns>
        [SuppressMessage("Microsoft.Usage", "CA1801", Justification = "Apesar de não usado, 'html' é requerido como regra para extenção.")]
        public static MvcHtmlString BootstrapMessageSuccess(this HtmlHelper html, string message)
        {
            return BootstrapMessage(message, MessageType.SUCCESS);
        }
        /// <summary>
        /// Retorna um Html com uma mensagem informativa formatada conforme as convenções do Bootstrap 3.
        /// </summary>
        /// <param name="html"></param>
        /// <param name="message">Mensagem a ser exibida.</param>
        /// <returns>Html com uma mensagem formatada conforme as convenções do Bootstrap 3.</returns>
        [SuppressMessage("Microsoft.Usage", "CA1801", Justification = "Apesar de não usado, 'html' é requerido como regra para extenção.")]
        public static MvcHtmlString BootstrapMessageInfo(this HtmlHelper html, string message)
        {
            return BootstrapMessage(message, MessageType.INFO);
        }
        /// <summary>
        /// Retorna um Html com uma mensagem de alerta formatada conforme as convenções do Bootstrap 3.
        /// </summary>
        /// <param name="html"></param>
        /// <param name="message">Mensagem a ser exibida.</param>
        /// <returns>Html com uma mensagem formatada conforme as convenções do Bootstrap 3.</returns>
        [SuppressMessage("Microsoft.Usage", "CA1801", Justification = "Apesar de não usado, 'html' é requerido como regra para extenção.")]
        public static MvcHtmlString BootstrapMessageWarning(this HtmlHelper html, string message)
        {
            return BootstrapMessage(message, MessageType.WARNING);
        }
        /// <summary>
        /// Retorna um Html com uma mensagem de erro formatada conforme as convenções do Bootstrap 3.
        /// </summary>
        /// <param name="html"></param>
        /// <param name="message">Mensagem a ser exibida.</param>
        /// <returns>Html com uma mensagem formatada conforme as convenções do Bootstrap 3.</returns>
        [SuppressMessage("Microsoft.Usage", "CA1801", Justification = "Apesar de não usado, 'html' é requerido como regra para extenção.")]
        public static MvcHtmlString BootstrapMessageError(this HtmlHelper html, string message)
        {
            return BootstrapMessage(message, MessageType.ERROR);
        }
        #endregion

        #region PAINEIS
        [SuppressMessage("Microsoft.Usage", "CA1801", Justification = "Apesar de não usado, 'html' é requerido como regra para extenção.")]
        public static MvcHtmlString BootstrapPanel(this HtmlHelper html, string title, string bodyId, bool tools = true, bool fechado = false)
        {
            var _strTools = new StringBuilder();
            _strTools.Append("<div class=\"tools hidden-xs\">");
            _strTools.AppendFormat("  <a class=\"{0}\" style=\"cursor:pointer\">", fechado ? "expand" : "collapse");
            _strTools.AppendFormat("      <i class=\"fa {0}\"></i>", (fechado ? "fa-chevron-down" : "fa-chevron-up"));
            _strTools.Append("  </a>");
            _strTools.Append("</div>");

            var _code = new StringBuilder();
            _code.Append("<div class=\"row\">")
                .Append("<div class=\"col-md-12\">")
                .Append("    <div class=\"box border raizen\">")
                .Append("        <div class=\"box-title\">")
                .AppendFormat("            <h4><i class=\"fa fa-list\"></i>{0}</h4>", title)
                .AppendFormat("            {0}", (tools ? _strTools.ToString() : ""))
                .Append("        </div>")
                .AppendFormat("        <div class=\"box-body\" id=\"{0}\" {1}>", bodyId, (fechado ? "style='display: none;'" : ""));
            _code.Append("</div></div></div></div>");
            return new MvcHtmlString(_code.ToString());
        }


        [SuppressMessage("Microsoft.Usage", "CA1801", Justification = "Apesar de não usado, 'html' é requerido como regra para extenção.")]
        public static MvcHtmlString BootstrapFieldset(this HtmlHelper html, string title, string bodyId)
        {
            var str = new StringBuilder();
            str.Append("<fieldset>");
            str.AppendFormat("    <legend>{0}</legend>", title);
            str.AppendFormat("    <div id=\"{0}\"></div>", bodyId);
            str.Append("</fieldset>");

            return new MvcHtmlString(str.ToString());
        }
        #endregion

        #region [ PAINEL RETRATIL ]

        [SuppressMessage("Microsoft.Usage", "CA1801", Justification = "Apesar de não usado, 'html' é requerido como regra para extensão.")]
        public static CollapsiblePanelHtmlHelper BootstrapCollapsiblePanel(this HtmlHelper html, string titulo, string idCorpo, bool fechado = true, string iconeHeader = null, ActionButtonCollapsiblePanel[] botoes = null, object htmlAttributes = null)
        {
            return new CollapsiblePanelHtmlHelper(html.ViewContext, titulo, idCorpo, fechado, iconeHeader, botoes, htmlAttributes);
        }

        #endregion

        #region PRIVATES
        private static MvcHtmlString BootstrapLabel(HtmlHelper html, string labelText, string id = null, string fort = null)
        {
            return html.Label(labelText, new { @class = labelClasses, @id = id, @for = fort });
        }
        private static MvcHtmlString BootstrapLabel<TModel, TProperty>(HtmlHelper<TModel> html, Expression<Func<TModel, TProperty>> expression, string labelText, HtmlHelperOption option = null)
        {
            if (option == null)
            {
                option = new HtmlHelperOption();
            }
            MvcHtmlString _label;
            var property = (MemberExpression)expression.Body;
            MemberExpression model;
            string MemberName = string.Empty;
            if (property.Expression.GetType() == typeof(MemberExpression))
            {
                model = (MemberExpression)property.Expression;
                MemberName = model.Member.Name;
            }
            else if (property.Expression.GetType().Name == "TypedParameterExpression")
            {
                MemberName = property.Expression.Type.Name;
            }
            else if (property.Expression.GetType().Name == "PropertyExpression")
            {
                //Tratamento adicionado para atender às validações de filtros.
                //TODO - Validar com o Daniel uma solução melhor.
                if (property.Expression.Type.FullName.Contains("Raizen.UniCad.Model.Filtro"))
                    MemberName = "Filtro";
                else
                    MemberName = property.Expression.Type.Name;
            }

            var id = string.Format("{0}_{1}_{2}", "lbl", MemberName, property.Member.Name);

            var fort = string.Format("{0}_{1}", MemberName, property.Member.Name);

            if (!string.IsNullOrEmpty(labelText))
                _label = BootstrapLabel(html, labelText, id, fort);
            else
            {

                _label = html.LabelFor(expression, new { @class = string.Format("{0} {1}", option.inLineLabelColuna, labelClasses), @id = id });
            }

            return _label;
        }
        private static MvcHtmlString BootstrapSmallHelp<TModel, TProperty>(HtmlHelper<TModel> html, Expression<Func<TModel, TProperty>> expression)
        {
            MvcHtmlString _small;
            _small = new MvcHtmlString(string.Format("<span class=\"help-block small\"><strong data-element-ref=\"{0}\">&nbsp;</strong></span>", html.NameFor(expression).ToHtmlString()));

            return _small;
        }
        private static MvcHtmlString BootstrapFormContainter(MvcHtmlString label, MvcHtmlString control, HtmlHelperOption option = null, string valorAnterior = null, bool iconeAjuda = false)
        {
            if (option == null)
            {
                option = new HtmlHelperOption();
            }

            var str = new StringBuilder();
            var classe = option.isFormGroup ? option.colunas + " form-group1" : option.colunas;
            str.AppendFormat("<div class=\"{0}\" style=\"padding-bottom: 10px;\">{1}{2}{3}{4}</div>",
                classe,
                (option.hideLabel ? "" : label.ToHtmlString()),
                control.ToHtmlString(),
                CriarIconeAlertExclamation(valorAnterior).ToString(),
                iconeAjuda ? CriarIconeAlertQuestion(getBetween(control.ToString(), "id", "\"")) : ""
            );
            return new MvcHtmlString(str.ToString());
        }

        //retornar o id do campo
        private static string getBetween(string strSource, string strStart, string strEnd)
        {
            int Start, End;
            if (strSource.Contains(strStart) && strSource.Contains(strEnd))
            {
                Start = strSource.IndexOf(strStart, 0) + strStart.Length;
                End = strSource.IndexOf(strEnd, Start + 2);
                return strSource.Substring(Start + 2, End - Start - 2);
            }
            else
            {
                return "";
            }
        }

        private static MvcHtmlString BootstrapFormComPesquisaContainter(MvcHtmlString label, MvcHtmlString control, MvcHtmlString btn, HtmlHelperOption option = null)
        {
            if (option == null)
            {
                option = new HtmlHelperOption();
            }

            var str = new StringBuilder();
            str.AppendFormat("<div class=\"{0}\">", option.colunas);
            str.AppendLine("	<div class=\"\">");
            str.AppendFormat("		{0}", label.ToHtmlString());
            str.AppendLine("		<div class=\"\">");
            str.AppendLine("			<div class=\"input-group\">");
            str.AppendFormat("				{0}", control.ToHtmlString());
            str.AppendLine("				<div class=\"input-group-btn\">");
            str.AppendFormat("					{0}", btn.ToHtmlString());
            str.AppendLine("				</div>");
            str.AppendLine("			</div>");
            str.AppendLine("		</div>");
            str.AppendLine("	</div>");
            str.AppendLine("</div>");

            return new MvcHtmlString(str.ToString());
        }
        private static MvcHtmlString BootstrapFormContainterComIcone(MvcHtmlString label, MvcHtmlString control, MvcHtmlString elementhelp = null, HtmlHelperOption option = null, string valorAnterior = null)
        {
            var str = new StringBuilder();

            str.AppendFormat("<div class=\"{0}\" style=\"padding-bottom: 10px;\">", option.colunas);
            str.AppendFormat("	{0}", (option.hideLabel ? "" : label.ToHtmlString()));
            str.AppendLine("	<div class=\"\">");
            str.AppendLine("		<div class=\"input-group\">");
            str.AppendFormat("			<span class=\"input-group-addon\"><i class=\"{0}\"></i></span>", option.icone);
            str.AppendFormat("			{0}", control.ToHtmlString());
            str.AppendLine("		</div>");
            str.AppendFormat("		{0}", (elementhelp != null ? elementhelp.ToString() : ""));
            str.AppendLine("	</div>");
            CriarIconeAlertExclamation(valorAnterior);
            str.AppendLine("</div>");


            return new MvcHtmlString(str.ToString());
        }
        private static MvcHtmlString BootstrapCheckBoxContainter(MvcHtmlString display, MvcHtmlString control)
        {
            var str = new StringBuilder();
            str.AppendFormat("<label class=\"checkbox-inline\"> {0} {1}</label>", control.ToHtmlString(), display.ToHtmlString());


            //str.AppendFormat("<div class=\"form-group\">{0}<div class=\"col-sm-9\"><div class=\"checkbox-inline\">{1}</div></div></div>",
            //    label.ToHtmlString(),
            //    control.ToHtmlString()
            //);
            return new MvcHtmlString(str.ToString());
        }

        private static MvcHtmlString BootstrapCurrencyBoxContainter(MvcHtmlString label, MvcHtmlString control, HtmlHelperOption option = null, string valorAnterior = null)
        {
            if (option == null)
            {
                option = new HtmlHelperOption();
            }

            if (string.IsNullOrEmpty(option.icone))
                option.icone = "fa fa-money";

            var str = new StringBuilder();
            if (option.inLine)
            {
                option.inLineLabelColuna = String.IsNullOrEmpty(option.inLineLabelColuna) ? "col-md-7" : option.inLineLabelColuna;
                option.inLineControleColuna = String.IsNullOrEmpty(option.inLineControleColuna) ? "col-md-5" : option.inLineControleColuna;

                str.AppendLine("<div class=\"\">");
                str.AppendLine("	<div class=\"form-group\">");
                str.AppendFormat("		<div class=\"{0} text-right\">", option.inLineLabelColuna);
                str.AppendFormat("			<i class=\"{0}\"></i>", option.iconeLabelSubItem);
                str.AppendFormat("			{0}", option.hideLabel ? "" : label.ToHtmlString());
                str.AppendLine("		</div>");
                str.AppendFormat("		<div class=\"{0}\">", option.inLineControleColuna);
                str.AppendLine("			<div class=\"input-group\" style=\"padding-bottom: 10px;\">");
                str.AppendFormat("				{0}", option.hideIcone ? "" : string.Format("<span class=\"input-group-addon\">{0}</span>", string.Format("<i class=\"{0}\"></i>", option.icone)));
                str.AppendFormat("				{0}", control.ToHtmlString());
                str.AppendLine("			</div>");
                str.AppendLine("		</div>");
                str.AppendLine("	</div>");
                str.AppendLine(CriarIconeAlertExclamation(valorAnterior));
                str.AppendLine("</div>");
            }
            else if (!option.hideLabel)
            {
                str.AppendFormat("<div class=\"{0}\">", option.colunas);
                str.AppendFormat(label.ToHtmlString());
                if (!option.hideIcone)
                {
                    str.AppendLine("			<div class=\"input-group\" style=\"padding-bottom: 10px;\">");
                    str.AppendFormat("				{0}", string.Format("<span class=\"input-group-addon\">{0}</span>", string.Format("<i class=\"{0}\"></i>", option.icone)));
                    str.AppendFormat("				{0}", control.ToHtmlString());
                    str.AppendLine("			</div>");
                }
                else
                {
                    str.AppendFormat("				{0}", control.ToHtmlString());
                }

                str.AppendLine(CriarIconeAlertExclamation(valorAnterior));

                str.AppendLine("</div>");
            }
            else
            {
                if (!option.hideIcone)
                {
                    str.AppendFormat("			<div class=\"input-group {0}\" style=\"padding-bottom: 10px;\">", option.colunas);
                    str.AppendFormat("				{0}", string.Format("<span class=\"input-group-addon\">{0}</span>", string.Format("<i class=\"{0}\"></i>", option.icone)));
                    str.AppendFormat("				{0}", control.ToHtmlString());
                    str.AppendLine("			</div>");
                }
                else
                {
                    str.AppendFormat("				{0}", control.ToHtmlString());
                }
            }

            return new MvcHtmlString(str.ToString());
        }

        private static string CriarIconeAlertExclamation(string valorAnterior)
        {
            if (valorAnterior != null)
                return String.Format("<div class=\"fa fa-exclamation alertExclamation\" title=\"{0}\"></div>", valorAnterior);
            else
                return string.Empty;
        }

        private static string CriarIconeAlertQuestion(string id)
        {
            return String.Format("<div class=\"fa fa-question alertQuestion\" id=\"{0}\"></div>", id);
        }

        private static MvcHtmlString BootstrapButton(string id, string text, string faclass, ButtonType behaviour, string clientClick = null, string htmlAttributes = null)
        {
            var _classCustom = "btn-custom";
            var _textLoading = "data-loading-text=\"<i class='fa fa-spin fa-refresh'></i> Aguarde...\"";
            switch (behaviour)
            {
                case ButtonType.RESET:
                    _classCustom = "btn-custom";
                    _textLoading = "";
                    break;
            }

            var str = string.Format("<button type=\"{0}\" {1} class=\"btn {2}\" {3} {6}value=\"true\" onclick=\"" + clientClick + "\">{4} {5}</button>",
                behaviour.ToString().ToLower(CultureInfo.InvariantCulture),
                _textLoading,
                _classCustom,
                !string.IsNullOrEmpty(id) ? string.Format("id=\"{0}\" name=\"{0}\"", id) : string.Empty,
                !string.IsNullOrEmpty(faclass) ? string.Format("<span class=\"fa {0}\"></span>", faclass) : string.Empty,
                !string.IsNullOrEmpty(text) ? text : string.Empty,
                htmlAttributes);

            return new MvcHtmlString(str);
        }

        private static MvcHtmlString BootstrapLinkButton(string url, string text, string faclass, TargetType target)
        {
            var str = string.Format("<a href=\"{0}\" class=\"btn btn-custom\" target=\"_{1}\">{2} {3}</a>",
                url,
                target.ToString().ToLower(CultureInfo.InvariantCulture),
                !string.IsNullOrEmpty(faclass) ? string.Format("<span class=\"fa {0}\"></span>", faclass) : string.Empty,
                !string.IsNullOrEmpty(text) ? text : string.Empty);
            return new MvcHtmlString(str);
        }

        private static MvcHtmlString BootstrapMessage(string message, MessageType? messageType)
        {
            var _messasgeType = messageType.GetValueOrDefault(MessageType.WARNING);
            var _alertClass = string.Empty;
            var _faIcon = string.Empty;
            switch (_messasgeType)
            {
                case MessageType.SUCCESS:
                    _alertClass = "alert-success";
                    _faIcon = "fa-check";
                    break;
                case MessageType.INFO:
                    _alertClass = "alert-info";
                    _faIcon = "fa-info-circle";
                    break;
                case MessageType.WARNING:
                    _alertClass = "alert-warning";
                    _faIcon = "fa-exclamation-triangle";
                    break;
                case MessageType.ERROR:
                    _alertClass = "alert-danger";
                    _faIcon = "fa-exclamation-circle";
                    break;
            }
            var _str =
                "<div class=\"row\">" +
                "<div class=\"col-md-12\">";
            _str += string.Format("<div class=\"alert alert-dismissible {0}\" role=\"alert\" >", _alertClass);
            _str += "<button type=\"button\" class=\"close\" data-dismiss=\"alert\" aria-label=\"Close\"><span aria-hidden=\"true\">&times;</span></button>";
            _str += string.Format("<span class=\"fa {0}\"></span> ", _faIcon);
            _str += message + "</div></div></div>";
            return new MvcHtmlString(_str);
        }

        private static MvcHtmlString BootstrapDropDownListContainter(MvcHtmlString label, MvcHtmlString control, HtmlHelperOption option = null, string valorAnterior = null, bool iconeAjuda = false)
        {
            if (option == null)
            {
                option = new HtmlHelperOption();
            }

            var str = new StringBuilder();
            var classe = option.isFormGroup ? option.colunas + " form-group1" : option.colunas;

            if (option.inLine)
            {
                str.AppendFormat("<div class=\"{0}\">", option.colunas);
                str.AppendLine("	<div class=\"row form-group\">");

                str.AppendFormat("		<div class=\"{0}\">", option.inLineLabelColuna);
                str.AppendFormat("			{0}", label.ToHtmlString());
                str.AppendLine("		</div>");

                str.AppendFormat("		<div class=\"{0}\">", option.inLineControleColuna);
                str.AppendFormat("				{0}", control.ToHtmlString());
                str.AppendLine("		</div>");

                str.AppendLine("	</div>");
                str.AppendLine(CriarIconeAlertExclamation(valorAnterior));
                str.AppendLine("</div>");
            }
            else if (!option.hideLabel)
            {
                str.AppendFormat("<div class=\"{0}\">{1}{2}{3}{4}</div>",
                classe,
                label.ToHtmlString(),
                control.ToHtmlString(),
                CriarIconeAlertExclamation(valorAnterior),
                iconeAjuda ? CriarIconeAlertQuestion(getBetween(control.ToString(), "id", "\"")) : "");
            }
            else
            {
                str.AppendFormat("<div class=\"{0}\">{1}{2}</div>",
                classe,
                control.ToHtmlString(),
                CriarIconeAlertExclamation(valorAnterior));

            }

            return new MvcHtmlString(str.ToString());


        }

        //private static string generateRows<TX, TY, TResult>(
        //    Dictionary<TY, Dictionary<TX, TResult>> data,
        //    string format = null,
        //    bool? columnSum = null,
        //    bool? columnAvg = null,
        //    bool? rowSum = null,
        //    bool? rowAvg = null,
        //    bool? avgAvg = null)
        //{

        //    var _format = string.IsNullOrEmpty(format) ? string.Empty : ":" + format;
        //    //SE FOR MONEY OU NÚMERO ALINHAR DIREITA
        //    string _formatedCellPattern = string.Empty;
        //    string _formatedFooterPattern = string.Empty;

        //    if (!string.IsNullOrWhiteSpace(format))
        //        _formatedCellPattern = format.Contains("N") || format.Contains("C") ? "<td style=\"text-align: right; \">" : "<td>";
        //    else
        //        _formatedCellPattern = "<td>";
        //    _formatedCellPattern += "{0" + _format + "}</td>";

        //    //FOOTER
        //    if (!string.IsNullOrWhiteSpace(format))
        //        _formatedFooterPattern = format.Contains("N") || format.Contains("C") ? "<th style=\"text-align: right; \">" : "<th>";
        //    else
        //        _formatedFooterPattern = "<th>";
        //    _formatedFooterPattern += "{0" + _format + "}</th>";

        //    var _outputCode = new StringBuilder();
        //    var _yProperties = typeof(TY).GetProperties();
        //    var _xs = data
        //        .SelectMany(s => s.Value.Select(ss => ss.Key))
        //        .Distinct();

        //    #region LINHAS DE DADOS
        //    foreach (var _yItem in data)
        //    {
        //        //CREATE THE ROW
        //        _outputCode.Append("<tr>");
        //        //CREATE CELL WITH THE Y AIXIS NAMES
        //        foreach (var _yProperty in _yProperties)
        //        {
        //            var _yPropertyValue = _yProperty.GetValue(_yItem.Key, null);
        //            var css = _yPropertyValue.Equals("ERRO") ? "erro" : string.Empty;
        //            _outputCode.AppendFormat("<th class='{0}'>{1}</th>", css, _yPropertyValue);
        //        }
        //        //CREATE THE CROSSING TABLE VALUES CELLS
        //        foreach (var _x in _xs)
        //        {
        //            var _value = default(TResult);
        //            if (_yItem.Value.ContainsKey(_x)) _value = _yItem.Value[_x];
        //            _outputCode.AppendFormat(_formatedCellPattern, _value);
        //        }
        //        //IF A ROW SUM IS REQUIRED
        //        if (rowSum.GetValueOrDefault(false))
        //        {
        //            var _rowSumValue = _yItem.Value.Select(s => s.Value).Sum();
        //            _outputCode.AppendFormat(_formatedCellPattern, _rowSumValue);
        //        }
        //        //IF A ROW AVG IS REQUIRED
        //        if (rowAvg.GetValueOrDefault(false))
        //        {
        //            //var _rowAvgValue = _yItem.Value.Select(s => s.Value).Average();
        //            var _values = new List<TResult>();
        //            foreach (var _x in _xs)
        //            {
        //                var _value = default(TResult);
        //                if (_yItem.Value.ContainsKey(_x)) _value = _yItem.Value[_x];
        //                _values.Add(_value);
        //            }
        //            _outputCode.AppendFormat(_formatedCellPattern, _values.Average());
        //        }
        //        _outputCode.Append("</tr>");
        //    }
        //    #endregion

        //    if (columnAvg.GetValueOrDefault(false) ||
        //        columnSum.GetValueOrDefault(false))
        //    {
        //        //DICTIONARY KEY IS THE COLUMN INDEX
        //        var _columnsData = new Dictionary<int, IList<TResult>>();
        //        foreach (var _yItem in data)
        //        {
        //            for (int _i = 0; _i < _xs.Count(); _i++)
        //            {
        //                if (!_columnsData.ContainsKey(_i))
        //                    _columnsData.Add(_i, new List<TResult>());
        //                var _x = _xs.ElementAt(_i);
        //                if (_yItem.Value.ContainsKey(_x)) _columnsData[_i].Add(_yItem.Value[_x]);
        //                else _columnsData[_i].Add(default(TResult));
        //            }
        //        }

        //        #region Linha de Somas
        //        if (columnSum.GetValueOrDefault(false))
        //        {
        //            _outputCode.Append("<tr>");
        //            for (var index = 0; index < _yProperties.Length; index++)
        //            {
        //                _outputCode.Append("<th>&nbsp;</th>");
        //            }
        //            //_outputCode.AppendFormat("<th colspan=\"{0}\">Total</th>", _yProperties.Length);
        //            var _sums = new List<TResult>();
        //            var _averages = new List<TResult>();
        //            foreach (var _columnData in _columnsData)
        //            {
        //                var _sum = _columnData.Value.Sum();
        //                _outputCode.AppendFormat(_formatedFooterPattern, _sum);

        //                try
        //                {
        //                    _sums.Add((TResult)_sum);
        //                }
        //                catch
        //                {
        //                    try
        //                    {
        //                        _sums.Add((TResult)Convert.ChangeType(_sum, typeof(TResult)));
        //                    }
        //                    catch
        //                    {
        //                        _sums.Add(default(TResult));
        //                    }
        //                }

        //                var _average = _columnData.Value.Average();
        //                try
        //                {
        //                    _averages.Add((TResult)_average);
        //                }
        //                catch
        //                {
        //                    try
        //                    {
        //                        _averages.Add((TResult)Convert.ChangeType(_average, typeof(TResult)));
        //                    }
        //                    catch
        //                    {
        //                        _averages.Add(default(TResult));
        //                    }
        //                }
        //            }

        //            if (rowSum.GetValueOrDefault(false))
        //            {
        //                _outputCode.AppendFormat(_formatedFooterPattern, _sums.Sum());
        //            }
        //            if (rowAvg.GetValueOrDefault(false))
        //            {
        //                _outputCode.AppendFormat(_formatedFooterPattern, avgAvg.GetValueOrDefault(false) ? _averages.Average() : _sums.Average());
        //            }
        //            _outputCode.Append("</tr>");
        //        }
        //        #endregion

        //        #region Linha de Médias
        //        if (columnAvg.GetValueOrDefault(false))
        //        {
        //            _outputCode.Append("<tr>");
        //            for (var index = 0; index < _yProperties.Length; index++)
        //            {
        //                _outputCode.Append("<th>&nbsp;</th>");
        //            }
        //            //_outputCode.AppendFormat("<th colspan=\"{0}\">Média</th>", _yProperties.Length);
        //            var _averages = new List<TResult>();
        //            foreach (var _columnData in _columnsData)
        //            {
        //                var _average = _columnData.Value.Average();
        //                _outputCode.AppendFormat(_formatedFooterPattern, _average);
        //                _averages.Add((TResult)Convert.ChangeType(_average, typeof(TResult)));
        //            }
        //            if (rowSum.GetValueOrDefault(false))
        //            {
        //                _outputCode.AppendFormat(_formatedFooterPattern, _averages.Sum());
        //            }
        //            if (rowAvg.GetValueOrDefault(false))
        //            {
        //                _outputCode.AppendFormat(_formatedFooterPattern, _averages.Average());
        //            }
        //            _outputCode.Append("</tr>");
        //        }
        //        #endregion
        //    }
        //    return _outputCode.ToString();
        //}

        #endregion

        #region Scripts
        //https://jadnb.wordpress.com/2011/02/16/rendering-scripts-from-partial-views-at-the-end-in-mvc/
        private class ScriptBlock : IDisposable
        {
            private const string ScriptsKey = "scripts";

            public static List<string> PageScripts
            {
                get
                {
                    if (HttpContext.Current.Items[ScriptsKey] == null)
                        HttpContext.Current.Items[ScriptsKey] = new List<string>();

                    return HttpContext.Current.Items[ScriptsKey] as List<string>;
                }
            }

            readonly WebViewPage _webPageBase;

            public ScriptBlock(WebViewPage webPageBase)
            {
                _webPageBase = webPageBase;
                _webPageBase.OutputStack.Push(new StringWriter());
            }

            public void Dispose()
            {
                PageScripts.Add(_webPageBase.OutputStack.Pop().ToString());
            }
        }

        public static IDisposable BeginScripts(this HtmlHelper helper)
        {
            return new ScriptBlock((WebViewPage)helper.ViewDataContainer);
        }

        [SuppressMessage("Microsoft.Usage", "CA1801", Justification = "Apesar de não usado, 'html' é requerido como regra para extenção.")]
        public static MvcHtmlString PageScripts(this HtmlHelper helper)
        {
            return MvcHtmlString.Create(string.Join(Environment.NewLine,
                ScriptBlock.PageScripts.Select(s => s.ToString(CultureInfo.InvariantCulture))));
        }
        #endregion
    }
}