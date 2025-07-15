using Raizen.UniCad.Model;
using System.Globalization;

namespace Infraestructure.Utils
{
    public static class CultureUtil
    {
        public static CultureInfo CultureInfoPorPais(EnumPais pais)
        {
            switch (pais)
            {
                case EnumPais.Argentina:
                    return new CultureInfo("es-AR");
                default:
                    return new CultureInfo("pt-BR");
            }
        }

        public static CultureInfo CultureInfoPorPais(int idPais)
        {
            switch (idPais)
            {
                case (int)EnumPais.Argentina:
                    return new CultureInfo("es-AR");
                default:
                    return new CultureInfo("pt-BR");
            }
        }
    }
}
