using Raizen.UniCad.Model;

namespace Raizen.UniCad.BLL.Util
{
    public static class Traducao
    {

        public static string GetTextoPorLingua(string msgPortugues, string msgEspanhol)
        {
            return GetTextoPorLingua(msgPortugues, msgEspanhol, EnumPais.Padrao);
        }
        public static string GetTextoPorLingua(string msgPortugues, string msgEspanhol, EnumPais pais)
        {
            switch (pais)
            {
                case EnumPais.Brasil:
                    return msgPortugues;

                case EnumPais.Argentina:
                    return msgEspanhol;

                default:
                    return msgPortugues;
            }
        }
    }
}
