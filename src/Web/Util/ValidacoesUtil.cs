using Raizen.UniCad.BLL.Util;
using Raizen.UniCad.Extensions;
using System.Text.RegularExpressions;

namespace Raizen.UniCad.Utils
{
    public static class ValidacoesUtil
    {
        public static bool ValidarTokenServico(string valor, int idPais) =>  valor == Config.GetConfig(Model.EnumConfig.token,true, idPais);

        public static bool ValidarRangeData(int ano) => ano > 1800 && ano < 2900;

        public static bool ValidaEmail(string vrEmail)
        {
            Regex rg = new Regex(@"^[A-Za-z0-9](([_\.\-]?[a-zA-Z0-9]+)*)@([A-Za-z0-9]+)(([\.\-]?[a-zA-Z0-9]+)*)\.([A-Za-z]{2,})$");
            return rg.IsMatch(vrEmail);
        }

        public static bool ValidaTelefone(string vrTelefone)
        {
            var telefone = vrTelefone.RemoveSpecialCharacters();
            Regex rg = new Regex(@"^\([1-9]{2}\) [2-9][0-9]{3,4}\-[0-9]{4}$");
            return rg.IsMatch(vrTelefone) && (telefone.Length > 10);
        }

        //TODO: Algoritmo de validação do DNI
        public static bool ValidaDNI(string vrDNI)
        {
            return true;
        }

        public static bool ValidaCPF(string vrCPF)
        {
            string valor = vrCPF.Replace(".", "");
            valor = valor.Replace("-", "");

            if (valor.Length != 11)
                return false;

            bool igual = true;

            for (int i = 1; i < 11 && igual; i++)
                if (valor[i] != valor[0])
                    igual = false;

            if (igual || valor == "12345678909")
                return false;

            int[] numeros = new int[11];

            for (int i = 0; i < 11; i++)
                numeros[i] = int.Parse(
                  valor[i].ToString());

            int soma = 0;
            for (int i = 0; i < 9; i++)
                soma += (10 - i) * numeros[i];

            int resultado = soma % 11;

            if (resultado == 1 || resultado == 0)
            {
                if (numeros[9] != 0)
                    return false;
            }
            else if (numeros[9] != 11 - resultado)
                return false;

            soma = 0;

            for (int i = 0; i < 10; i++)
                soma += (11 - i) * numeros[i];

            resultado = soma % 11;

            if (resultado == 1 || resultado == 0)
            {
                if (numeros[10] != 0)
                    return false;
            }
            else
                if (numeros[10] != 11 - resultado)
                return false;

            return true;
        }

        /// <summary>
        /// Validates CNPJ/CUIT allowing alphanumeric values (14 to 18 characters).
        /// </summary>
        public static bool ValidaCNPJ(string vrCNPJ)
        {
            if (string.IsNullOrEmpty(vrCNPJ))
                return false;
            // Remove common separators
            var clean = Regex.Replace(vrCNPJ, "[.\-/]", string.Empty);
            // Remove any other non-alphanumeric characters
            clean = Regex.Replace(clean, "[^A-Za-z0-9]", string.Empty);
            // Validate length and alphanumeric content
            return Regex.IsMatch(clean, "^[A-Za-z0-9]{14,18}$");
        }

        public static bool IsTransportadora(this string perfil)
        {
            perfil = perfil.Trim();
            perfil = perfil.ToLowerInvariant();

            if (perfil.Equals("transportadora") || perfil.Equals("transportadora argentina"))
                return true;

            return false;
        }

        public static bool IsClienteEAB(this string perfil)
        {
            perfil = perfil.Trim();
            perfil = perfil.ToLowerInvariant();

            if (perfil.Equals("cliente eab"))
                return true;

            return false;
        }

        public static bool IsQuality(this string perfil)
        {
            perfil = perfil.Trim();
            perfil = perfil.ToLowerInvariant();

            if (perfil.Equals("quality"))
                return true;

            return false;
        }
    }
}
