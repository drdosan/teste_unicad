using System;
using System.Reflection;

namespace Raizen.UniCad.BLL.Extensions
{
    public static class DefaultExtension
    {
        /// <summary>
        /// Extension method to set the default value, used only by unit tests project.
        /// </summary>
        /// <typeparam name="T">Class to get the property collection</typeparam>
        /// <param name="_class">Class to get the property collection</param>
        public static void SetGetDefaults<T>(this T _class)
        {
            //Setando um valor para todas as propriedades da Classe que tenham um método Set()
            foreach (PropertyInfo property in _class.GetType().GetProperties())
                if (property.GetSetMethod() != null)
                    property.SetValue(_class, ValorDefault(property.PropertyType), null);

            //Lendo o valor de todas as propriedades da Classe
            foreach (PropertyInfo property in _class.GetType().GetProperties())
                property.GetValue(_class);
        }

        private static object ValorDefault(Type propertyType)
        {
            if (propertyType == typeof(string))
                return "x";

            if (propertyType == typeof(short) ||
                propertyType == typeof(int) ||
                propertyType == typeof(long) ||
                propertyType == typeof(double) ||
                propertyType == typeof(decimal))
                return Convert.ChangeType(1, propertyType);

            if (propertyType == typeof(bool))
                return true;

            if (propertyType == typeof(DateTime))
                return DateTime.MaxValue;

            if (propertyType.IsEnum)
                return Enum.ToObject(propertyType, 1);

            if (propertyType == typeof(TimeSpan))
                return new TimeSpan(23, 59, 59);

            return null;
        }

        /// <summary>
        /// Extension method to get the default value set previously by SetGetDefaults() extension method, used only by unit tests project.
        /// </summary>
        /// <param name="property">Class property</param>
        /// <returns>Property value</returns>
        public static object GetDefaultValue(this PropertyInfo property)
        {
            return ValorDefault(property.PropertyType);
        }
    }
}
