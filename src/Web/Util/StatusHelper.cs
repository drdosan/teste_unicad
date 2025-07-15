using Raizen.UniCad.Model;

namespace Raizen.UniCad.Web.Util
{
    public static class StatusHelper
    {
        public static string CorPorStaus(EnumStatusMotorista status)
        {
            string corHex;
            switch (status)
            {
                case EnumStatusMotorista.Aprovado:
                    corHex = "#c5dab2";
                    break;
                case EnumStatusMotorista.Bloqueado:
                    corHex = "#b2c2df";
                    break;
                case EnumStatusMotorista.EmAprovacao:
                    corHex = "#f9e39e";
                    break;
                case EnumStatusMotorista.Reprovado:
                    corHex = "#e9ae87";
                    break;
                default:
                    corHex = "#fff";
                    break;
            }

            return corHex;
        }

        public static string CorPorStaus(EnumStatusComposicao status)
        {
            string corHex;
            switch (status)
            {
                case EnumStatusComposicao.Aprovado:
                    corHex = "#c5dab2";
                    break;
                case EnumStatusComposicao.Bloqueado:
                    corHex = "#b2c2df";
                    break;
                case EnumStatusComposicao.EmAprovacao:
                    corHex = "#f9e39e";
                    break;
                case EnumStatusComposicao.Reprovado:
                    corHex = "#e9ae87";
                    break;
                default:
                    corHex = "#fff";
                    break;
            }

            return corHex;
        }
    }
}