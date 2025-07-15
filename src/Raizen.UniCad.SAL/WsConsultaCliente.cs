using Raizen.UniCad.Model;
using Raizen.UniCad.SAL.WsConsultarCliente;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Net;

namespace Raizen.UniCad.SAL
{
    public class WsConsultaCliente
    {
        Cliente_OutService client = new Cliente_OutService();
        public List<Cliente> Importar(DateTime? dataInicial, EnumEmpresa origemSAP, List<String> ibms = null)
        {
            List<Cliente> clientes = new List<Cliente>();
            string UserName = ConfigurationManager.AppSettings["usuarioWSCliente"];
            string PassWord = ConfigurationManager.AppSettings["senhaWSCliente"];
            string URL = ConfigurationManager.AppSettings["urlWSCliente"];

            //CREDENCIAIS
            ICredentials credentials = new NetworkCredential(UserName, PassWord);
            client.Credentials = credentials;
            client.Url = URL;
            client.Timeout = 1200000;
            //PARÂMETRO OPCIONAL DE CLIENTES (IBMS) ESPECÍFICOS

            ConsultarRequest request = new ConsultarRequest();
            request.Ambiente = origemSAP == EnumEmpresa.Combustiveis ? "FUELS" : "EAB";
            if (ibms != null)
            {
                request.Clientes = new ConsultarRequestItem[ibms.Count];
                for (int x = 0; x < ibms.Count; x++)
                    request.Clientes[x] = new ConsultarRequestItem { NoCliente = ibms[x] };
            }

            //SE NÃO TIVER DATA, IRÁ TRAZER TUDO
            if (dataInicial != null)
                request.Data = dataInicial.Value.ToString("yyyy-MM-dd HH:mm");
            else
                request.Data = "2007-01-01";
            ConsultarResponseCliente[] response = client.Consultar_Sync(request);
            if (response != null && response.Length > 0)
            {
                for (int y = 0; y < response.Length; y++)
                {
                    Cliente c = new Cliente();
                    c.IBM = response[y].NoCliente;
                    c.CNPJCPF = !string.IsNullOrEmpty(response[y].Cnpj) ? response[y].Cnpj : response[y].Cpf;
                    c.RazaoSocial = response[y].Nome;
                    c.IDEmpresa = (int)origemSAP;
                    c.Desativado = string.IsNullOrEmpty(response[y].Deletado) ? false : response[y].Deletado.ToLower(CultureInfo.InvariantCulture) == "x";
                    c.IdPais = VerificarPaisCliente(response[y].GrpConta);
                    clientes.Add(c);
                }
            }

            return clientes;
        }

        private int VerificarPaisCliente(string grpConta)
        {
            //Grupo de contas das Argentina
            if ((new string[] { "Z001", "Z002", "Z003", "Z004", "Z005", "Z006" }).Contains(grpConta))
                return (int)EnumPais.Argentina;

            return (int)EnumPais.Brasil;
        }
    }
}
