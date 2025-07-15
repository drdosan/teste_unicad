using System;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Resources;
using System.Web.Routing;
using System.Web.Mvc;

namespace Raizen.UniCad.Extensions
{
    /// <summary>
    /// Extenções de Objeto.
    /// </summary>
    public static class ObjectExtensions
    {
        /// <summary>
        /// Combina as propriedades do objeto com as de outro objeto.
        /// </summary>
        /// <param name="self">Objeto.</param>
        /// <param name="other">Outro objeto.</param>
        /// <returns>
        /// Um objeto resultante da combinação dos dois objetos.
        /// </returns>
        public static RouteValueDictionary Combine(this RouteValueDictionary self, object other)
        {
            var atribute = HtmlHelper.AnonymousObjectToHtmlAttributes(other);
            foreach (var item in atribute)
            {
                if (!self.Any(p => p.Key == item.Key))
                    self.Add(item.Key, item.Value);
                else if (item.Key == "class")
                {
                    var value = self.First(p => p.Key == item.Key).Value;
                    self.Remove(item.Key);
                    self.Add(item.Key, value + " " + item.Value);
                }
            }
            return self;
        }
    }
}