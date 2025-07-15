using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Resources;

namespace Raizen.UniCad.Extensions
{
    public static class EnumExtensions
    {
        #region MoMo
        /// <summary>
        /// Recupera o valor informado no atributo Description para um item de uma enum
        /// </summary>
        /// <param name="valor"></param>
        /// <returns></returns>
        public static string GetDescription(this Enum valor)
        {
            if (valor == null) throw new ArgumentNullException("valor");

            var field = valor.GetType().GetField(valor.ToString());
            var attributes = (DescriptionAttribute[])field?.GetCustomAttributes(typeof(DescriptionAttribute), false);
            return attributes != null && attributes.Length > 0 ? attributes[0].Description : valor.ToString();
        }

        public static List<KeyValuePair<int, string>> GetKeyValueList<T>(bool description = true)
        {
            return EnumToList(typeof(T), description);
        }

        /// <summary>
        /// Retorna Enum no formato chave/Valor
        /// </summary>
        /// <param name="type"></param>
        /// <param name="description"></param>
        /// <returns></returns>
        public static List<KeyValuePair<int, string>> GetKeyValueList(Type type, bool description = true)
        {
            return EnumToList(type, description);
        }

        #region private
        private static List<KeyValuePair<int, string>> EnumToList(Type type, bool description)
        {
            return (from object en in Enum.GetValues(type)
                    select new KeyValuePair<int, string>(
                        (int)en,
                        description ? ((Enum)en).GetDescription() : en.ToString())).ToList();
        }
        #endregion
        #endregion

    }
}