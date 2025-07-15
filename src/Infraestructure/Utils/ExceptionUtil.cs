using Raizen.UniCad.Model;
using System;
using System.Text;

namespace Raizen.UniCad.Utils
{
    public static class ExceptionUtil
    {
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
    }
}
