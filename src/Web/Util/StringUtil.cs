using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Globalization;
using System.Text;

namespace Raizen.UniCad.Utils
{
    public static class StringUtil
    {
        public static string ReplaceFirst(this string text, string search, string replace)
        {
            int pos = text.IndexOf(search, StringComparison.InvariantCultureIgnoreCase);
            if (pos < 0)
            {
                return text;
            }
            return text.Substring(0, pos) + replace + text.Substring(pos + search.Length);
        }

        public static string GetDebugParameters(IList<DbParameter> parametros, string query)
        {
            return GetDebugParameters(parametros) + query;
        }

        public static string GetDebugParameters(IList<DbParameter> parametros)
        {
            StringBuilder sb = new StringBuilder();

            foreach (DbParameter i in parametros)
            {
                string tipo;
                switch (i.DbType.ToString().ToLower(CultureInfo.InvariantCulture))
                {
                    case "int32": tipo = "int"; break;
                    case "boolean": tipo = "bit"; break;
                    case "datetime": tipo = "datetime"; break;
                    case "ansistring": tipo = "varchar(max)"; break;

                    default: tipo = i.DbType.ToString(); break;
                }

                string valor;
                switch (i.DbType.ToString().ToLower(CultureInfo.InvariantCulture))
                {
                    case "boolean": valor = i.Value.ToString().ToLower(CultureInfo.InvariantCulture) == "true" ? "1" : "0"; break;
                    case "datetime": valor = "'" + ((DateTime)i.Value).ToString("yyyy-MM-dd HH:mm:ss") + "'"; break;
                    case "ansistring": valor = "'" + i.Value.ToString() + "'"; break;

                    case "int32": 
                    default: valor = i.Value.ToString(); break;
                }

                sb.Append(Environment.NewLine + " DECLARE @" + i.ParameterName + " AS " + tipo + " = " + valor);
            }

            return sb.ToString();
        }

        #region Exception Text

        public static string ExceptionText(Exception ex)
        {
            return ExceptionText(ex, "");

        }

        public static string ExceptionText(Exception ex, string text)
        {
            StringBuilder sb = new StringBuilder(text);

            sb.AppendLine(ex.Message);
            sb.AppendLine(ex.StackTrace);

            if (ex.InnerException != null)
            {
                return ExceptionText(ex.InnerException, sb.ToString());
            }
            else
            {
                return sb.ToString();
            }
        }

        #endregion

        public static string RemoverAcentos(this string text)
        {
            if (text == null)
                return string.Empty;
            StringBuilder sbReturn = new StringBuilder();
            var arrayText = text.Normalize(NormalizationForm.FormD).ToCharArray();
            foreach (char letter in arrayText)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(letter) != UnicodeCategory.NonSpacingMark)
                    sbReturn.Append(letter);
            }
            return sbReturn.ToString();
        } 
    }
}
