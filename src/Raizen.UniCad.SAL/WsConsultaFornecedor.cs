using Raizen.UniCad.Model;
using Raizen.UniCad.SAL.WsConsultarFornecedor;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Net;

namespace Raizen.UniCad.SAL
{
    public class WsConsultaFornecedor
    {
        Fornecedor_OutService client = new Fornecedor_OutService();
        public List<Transportadora> Importar(DateTime? dataInicial,EnumEmpresa origemSAP, List<String> cnpjs = null, List<String> cpfs = null)
        {
            List<Transportadora> transportadoras = new List<Transportadora>();
            string UserName = ConfigurationManager.AppSettings["usuarioWSFornecedor"];
            string PassWord = ConfigurationManager.AppSettings["senhaWSFornecedor"];
            string URL = ConfigurationManager.AppSettings["urlWSFornecedor"];

            //CREDENCIAIS
            ICredentials credentials = new NetworkCredential(UserName, PassWord);
            client.Credentials = credentials;
            client.Url = URL;
            client.Timeout = 1200000;

            ConsultarRequest request = new ConsultarRequest();
            request.GpContas = new ConsultarRequestItem[2];
            if (origemSAP == EnumEmpresa.Combustiveis)
            {
                request.GpContas = new ConsultarRequestItem[4];
                request.GpContas[0] = new ConsultarRequestItem { GpConta = "FRET" };
                request.GpContas[1] = new ConsultarRequestItem { GpConta = "PLAC" };
                request.GpContas[2] = new ConsultarRequestItem { GpConta = "PRTR" };
                request.GpContas[3] = new ConsultarRequestItem { GpConta = "PRT1" };
            }
            else
            {
                request.GpContas = new ConsultarRequestItem[4];
                request.GpContas[0] = new ConsultarRequestItem { GpConta = "9400" };
                request.GpContas[1] = new ConsultarRequestItem { GpConta = "9600" };
                request.GpContas[2] = new ConsultarRequestItem { GpConta = "9601" };
                request.GpContas[3] = new ConsultarRequestItem { GpConta = "9700" };
            }
            request.Ambiente = origemSAP == EnumEmpresa.Combustiveis ? "FUELS" : "EAB";
          
            //PARÂMETRO OPCIONAL DE CPFS ESPECÍFICOS            
            if (cnpjs != null)
            {
                request.Cnpjs = new ConsultarRequestItem1[cnpjs.Count];
                for (int x = 0; x < cnpjs.Count; x++)
                    request.Cnpjs[x] = new ConsultarRequestItem1 {  Cnpj  = cnpjs[x] };
            }
            else
                request.Cnpjs = new ConsultarRequestItem1[1];

            //PARÂMETRO OPCIONAL DE CNPJS ESPECÍFICOS
            if (cpfs != null)
            {
                request.Cpfs = new ConsultarRequestItem2[cpfs.Count];
                for (int x = 0; x < cpfs.Count; x++)
                    request.Cpfs[x] = new ConsultarRequestItem2 { Cpf = cpfs[x] };
            }
            else
                request.Cpfs = new ConsultarRequestItem2[1];

            //SE NÃO TIVER DATA, IRÁ TRAZER TUDO
            if (dataInicial != null)
                request.Data = dataInicial.Value.ToString("yyyy-MM-dd HH:mm");
            else
                request.Data = "2007-01-01";


            ConsultarResponseFornecedor[] response = client.Consultar_Sync(request);
            if (response != null && response.Length > 0)
            {
                for (int y = 0; y < response.Length; y++)
                {
                    Transportadora t = new Transportadora();
                    t.IBM = response[y].NoFornecedor;
                    t.CNPJCPF = !string.IsNullOrEmpty(response[y].Cnpj) ? response[y].Cnpj : response[y].Cpf;
                    t.RazaoSocial = response[y].Nome;
                    t.IDEmpresa = (int)origemSAP;
                    t.IdPais = VerificarPais(response[y].GpConta);
                    t.Operacao = VerificarOperacao(response[y].GpConta, t.IdPais);
                    t.Desativado = string.IsNullOrEmpty(response[y].Deletado) ? false : response[y].Deletado.ToLower(CultureInfo.InvariantCulture) == "x";
                    transportadoras.Add(t);
                }
            }

            return transportadoras;
        }

        private string VerificarOperacao(string grpConta, int idPais)
        {
            if (idPais == (int)EnumPais.Argentina)
                return grpConta == "PRT1" ? "FOB" : "CIF";

            return grpConta == "PLAC" ? "FOB" : "CIF";
        }

        private int VerificarPais(string grpConta)
        {
            if ((new string[] { "PRTR", "PRT1" }).Contains(grpConta))
                return (int)EnumPais.Argentina;

            return (int)EnumPais.Brasil;
        }
    }
}
