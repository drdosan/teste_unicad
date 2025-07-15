using Raizen.Framework.Utils.Extensions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Raizen.UniCad.Extensions
{
    /// <summary>
    /// Extenções para String
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// REmove special characters
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string RemoveSpecialCharacters(this string s)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in s)
            {
                if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') ||  c == '_')
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// REmove no digit characters
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string RemoveCharacter(this string s)
        {
            if (s.IsNullOrEmpty()) return s;
            var arr = s.ToCharArray();
            arr = Array.FindAll(arr, (c => char.IsDigit(c) && !c.Equals('.')));
            var str = new string(arr);
            return str;
        }
    }
}
