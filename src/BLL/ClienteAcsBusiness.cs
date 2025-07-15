using Raizen.Framework.UserSystem.Client;
using Raizen.Framework.Web.Common;
using Raizen.UniCad.Model;
using Raizen.UniCad.Model.View;
using Raizen.UserSystem.SAL.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
//using System.Text.Json;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using Raizen.SSO.PipeLine;
using System.Web;
using DocumentFormat.OpenXml.Presentation;
namespace Raizen.UniCad.BLL
{
    public class ClienteAcsBusiness
    {
        public static string GetMensagemTraduzida(EnumPais pais, string msgPortugues, string msgEspanhol)
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

        public string AutenticarTrading(string nome, EnumPais pais, out string tokenSso)
        {
            tokenSso = null;

            try
            {
                var userName = ObtemUsuarioUserSystem(pais, true);

                if (userName == null)
                    return GetMensagemTraduzida(pais,
                        "Usuário ClienteAcsUnicadServico não configurado!",
                        "Cliente UserAcsUnicadService no configurado!");

                var password = ObtemSenhaUserSystem(pais, true);

                if (password == null)
                    return GetMensagemTraduzida(pais,
                        "Senha do  ClienteAcsUnicadServico não configurado!",
                        "Client PasswordAcsUnicadService no establecido!");

                var sigla = ConfigurationManager.AppSettings["SIGLA_APP"];

                var resultado = LoginUserSystem(userName, password, sigla);

                if (!string.IsNullOrEmpty(resultado?.Token))
                {
                    resultado.InfoUserSystem.InformacoesUsuario.Nome = nome;
                    tokenSso = resultado.Token;

                    resultado.InfoUserSystem.InformacoesUsuario.Senha = password;
                    UserSession.SetCurrentInfoUserSystem(resultado.InfoUserSystem);

                    if (resultado.InfoUserSystem.InformacoesApp != null)
                    {
                        ApplicationSession.IdApplication = resultado.InfoUserSystem.InformacoesApp.Id;
                        ApplicationSession.SiglaApplication = sigla;
                    }
                }
            }
            catch (Exception ex)
            {
                return GetMensagemTraduzida(pais,
                    $"Problemas ao autenticar no CSOnline Trading - {ex}",
                    $"Problemas ao autenticar no CSOnline Trading - {ex.Message}");
            }

            return null;
        }

        public string Autenticar(string Dv, string Token, out string usuario, out string tokenSso, EnumPais pais)
        {
            usuario = null;
            tokenSso = null;

            try
            {
                var userName = ObtemUsuarioUserSystem(pais);

                if (userName == null)
                    return GetMensagemTraduzida(pais,
                        "Usuário ClienteAcsUnicadServico não configurado!",
                        "Cliente UserAcsUnicadService no configurado!");

                var password = ObtemSenhaUserSystem(pais);

                if (password == null)
                    return GetMensagemTraduzida(pais,
                        "Senha do  ClienteAcsUnicadServico não configurado!",
                        "Client PasswordAcsUnicadService no establecido!");

                var sigla = ConfigurationManager.AppSettings["SIGLA_APP"];

                if (string.IsNullOrEmpty(Token))
                    return GetMensagemTraduzida(pais,
                        "O Token enviado está inválido!",
                        "¡El token cargado no es válido!");

                if (string.IsNullOrEmpty(Dv))
                    return GetMensagemTraduzida(pais,
                        "O Dv enviado está inválido!",
                        "¡El Dv cargado no es válido!");

                var handler = new JwtSecurityTokenHandler();
                var tokenS = handler.ReadToken(Token) as JwtSecurityToken;

                if (tokenS?.Claims == null)
                    return GetMensagemTraduzida(pais,
                        "Não foi possivel decodificar o token!",
                        "¡No se pudo decodificar el token!");

                var ibm = tokenS.Claims.First(claim => claim.Type == "ibm").Value;

                if (tokenS.Claims == null)
                    return GetMensagemTraduzida(pais,
                        "Não foi possivel decodificar o token!",
                        "¡No se pudo decodificar el token!");

                var resultado = LoginUserSystem(userName, password, sigla);

                if (!string.IsNullOrEmpty(resultado?.Token))
                {
                    System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
                    var urlCs = ConfigurationManager.AppSettings["urlAcsWebApi"] + ibm + "&division=" + Dv;
                    var request = (HttpWebRequest)WebRequest.Create(urlCs);
                    request.ContentType = "application/json";
                    request.ContentLength = 0;
                    request.Method = "GET";
                    request.Headers.Add("authorization", "Bearer " + Token);

                    var response = (HttpWebResponse)request.GetResponse();

                    if (response != null)
                    {
                        using (var reader = new StreamReader(response.GetResponseStream()))
                        {
                            var resultCs = reader.ReadToEnd();



                            var resultadoCs = JsonConvert.DeserializeObject<List<ClientesCsOnline>>(resultCs);

                            if (resultadoCs != null)
                            {
                                var nome = resultadoCs.First(p => p.id == ibm).name;
                                resultado.InfoUserSystem.InformacoesUsuario.Nome = nome;
                                var userBll = new UsuarioBusiness();
                                var user = userBll.Selecionar(p => p.Login == ibm);
                                if (user != null && user.ID > 0)
                                {
                                    usuario = user.Login;
                                }
                                else
                                {
                                    var newUsuario = new Usuario
                                    {
                                        Login = ibm,
                                        Nome = nome,
                                        Externo = true,
                                        IDEmpresa = (int)EnumEmpresa.Combustiveis,
                                        Operacao = "FOB",
                                        Perfil = (pais == EnumPais.Argentina ? EnumPerfil.CLIENTE_ACS_ARGENTINA : EnumPerfil.CLIENTE_ACS),
                                        Status = true,
                                        IDPais = pais
                                    };
                                    userBll.AdicionarUsuario(newUsuario, false);
                                    user = newUsuario;
                                    usuario = ibm;
                                }
                                var clienteBll = new UsuarioClienteBusiness();
                                clienteBll.ExcluirLista(p => p.IDUsuario == user.ID);
                                user.Clientes = new List<UsuarioClienteView>();
                                foreach (var item in resultadoCs)
                                {
                                    var clienteUnicad = new ClienteBusiness(pais).Listar(p => p.IBM == item.id);
                                    if (clienteUnicad != null && clienteUnicad.Any())
                                        user.Clientes.Add(new UsuarioClienteView
                                        {
                                            IDCliente = clienteUnicad.First().ID,
                                            RazaoSocial = item.name
                                        });
                                }

                                userBll.AtualizarUsuario(user, false);

                            }
                            else
                            {
                                return GetMensagemTraduzida(pais,
                                    "Não foi possivel obter a rede de clientes do CSOnline",
                                    "No se puede obtener la red de clientes CSOnline");
                            }
                        }

                        tokenSso = resultado.Token;

                        resultado.InfoUserSystem.InformacoesUsuario.Senha = password;
                        UserSession.SetCurrentInfoUserSystem(resultado.InfoUserSystem);

                        if (resultado.InfoUserSystem.InformacoesApp != null)
                        {
                            ApplicationSession.IdApplication = resultado.InfoUserSystem.InformacoesApp.Id;
                            ApplicationSession.SiglaApplication = sigla;
                        }
                    }
                    else
                    {
                        return GetMensagemTraduzida(pais,
                            "Não foi possivel obter a rede de clientes do CSOnline",
                            "No se puede obtener la red de clientes CSOnline");
                    }
                }
            }
            catch (Exception ex)
            {
                return GetMensagemTraduzida(pais,
                    $"Problemas ao autenticar no CSOnline - {ex.Message}",
                    $"Problemas ao autenticar no CSOnline - {ex.Message}");
            }

            return null;
        }

        public string RegistrarTokenTrading(string tokenTrading, EnumPais pais, out string usuario, out string nome)
        {
            usuario = null;
            nome = null;

            try
            {
                var userName = ObtemUsuarioUserSystem(pais, true);

                if (userName == null)
                    return GetMensagemTraduzida(pais,
                        "Usuário ClienteAcsUnicadServico não configurado!",
                        "Cliente UserAcsUnicadService no configurado!");

                var password = ObtemSenhaUserSystem(pais, true);

                if (password == null)
                    return GetMensagemTraduzida(pais,
                        "Senha do  ClienteAcsUnicadServico não configurado!",
                        "Client PasswordAcsUnicadService no establecido!");

                var sigla = ConfigurationManager.AppSettings["SIGLA_APP"];

                var handler = new JwtSecurityTokenHandler();
                var tokenS = handler.ReadToken(tokenTrading) as JwtSecurityToken;
                string customerNetworks = tokenS.Payload["customerNetworks"] as string;
                if (string.IsNullOrEmpty(customerNetworks))
                {
                    return GetMensagemTraduzida(pais,
                        "Parâmetro incorreto",
                        String.Empty);
                }


                var clientes = JsonConvert.DeserializeObject<IList<ClientesCsTrading>>(customerNetworks);

                if (clientes.Count() <= 0)
                {
                    return GetMensagemTraduzida(pais,
                        "Cnpj não informado",
                        String.Empty);
                }

                var resultado = LoginUserSystem(userName, password, sigla);

                if (!string.IsNullOrEmpty(resultado?.Token))
                {
                    nome = tokenS.Payload["name"].ToString();
                    Usuario user;
                    string cnpj = tokenS.Payload["cnpj"].ToString();
                    var userBll = new UsuarioBusiness();
                    if (TryGetUserByCnpj(cnpj, out user))
                    {
                        usuario = user.Login;
                    }
                    else
                    {
                        var newUsuario = new Usuario
                        {
                            Login = cnpj,
                            Nome = nome,
                            Externo = true,
                            IDEmpresa = (int)EnumEmpresa.Combustiveis,
                            Operacao = "FOB",
                            Perfil = (pais == EnumPais.Argentina ? EnumPerfil.CLIENTE_ACS_ARGENTINA : EnumPerfil.CLIENTE_ACS),
                            Status = true,
                            IDPais = pais
                        };
                        userBll.AdicionarUsuario(newUsuario, false);
                        user = newUsuario;
                        usuario = cnpj;
                    }
                    var clienteBll = new UsuarioClienteBusiness();
                    clienteBll.ExcluirLista(p => p.IDUsuario == user.ID);
                    user.Clientes = new List<UsuarioClienteView>();
                    userBll.AtualizarUsuario(user, false);

                    var usuariosClientes = new List<UsuarioCliente>();
                    foreach (var item in clientes)
                    {
                        var clienteUnicad = new ClienteBusiness(pais).Listar(p => p.CNPJCPF == item.Cnpj) ?? new List<Cliente>();
                        foreach (var cliente in clienteUnicad)
                        {
                            usuariosClientes.Add(new UsuarioCliente
                            {
                                IDCliente = cliente.ID,
                                IDUsuario = user.ID,
                                //RazaoSocial = item.CompanyName
                            });
                        }
                    }
                    clienteBll.IncluirLista(usuariosClientes);
                }
            }
            catch (Exception ex)
            {
                return GetMensagemTraduzida(pais,
                    $"Problemas ao registrar token trading - {ex.Message}",
                    $"Problemas ao registrar token trading - {ex.Message}");
            }

            return null;
        }

        private static bool TryGetUserByCnpj(string cnpj, out Usuario usuario)
        {
            var userBll = new UsuarioBusiness();
            usuario = userBll.Selecionar(p => p.Login == cnpj);
            return usuario != null && usuario.ID > 0;
        }

        private static string ObtemSenhaUserSystem(EnumPais pais, bool trading = false)
        {
            if (trading && ConfigurationManager.AppSettings.AllKeys.Contains("SenhaAcsUnicadServicoTrading"))
            {
                return ConfigurationManager.AppSettings["SenhaAcsUnicadServicoTrading"];
            }
            if (pais == EnumPais.Argentina)
                return ConfigurationManager.AppSettings["SenhaAcsUnicadServicoArgentina"];

            return ConfigurationManager.AppSettings["SenhaAcsUnicadServico"];
        }

        private static string ObtemUsuarioUserSystem(EnumPais pais, bool trading = false)
        {
            if (trading && ConfigurationManager.AppSettings.AllKeys.Contains("ClienteAcsUnicadServicoTrading"))
            {
                return ConfigurationManager.AppSettings["ClienteAcsUnicadServicoTrading"];
            }
            if (pais == EnumPais.Argentina)
                return ConfigurationManager.AppSettings["ClienteAcsUnicadServicoArgentina"];

            return ConfigurationManager.AppSettings["ClienteAcsUnicadServico"];
        }

        public RetornoUserSystem LoginUserSystem(string userName, string password, string sigla)
        {
            var context = HttpContext.Current;
            var guid = string.Empty;
            if (context != null)
            {
                // Passa o HttpContext para a criação do GUID
                guid = SSOHelper.CreateGuid(context);
            }

            var url = ConfigurationManager.AppSettings["urlUserSystem"];
            String encoded = Convert.ToBase64String(Encoding.GetEncoding("ISO-8859-1").GetBytes(userName + ":" + password));

            var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Expect = "application/json";
            httpWebRequest.Method = "POST";
            httpWebRequest.Headers.Add("Authorization", "Basic " + encoded);
            

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                string json = "{\"DiasExpiracao\":\"1\"," +
                              "\"Login\":\"" + userName + "\"," +
                              "\"Senha\":\"" + password + "\"," +
                              "\"SiglaApp\":\"" + sigla + "\"," +
                               "\"GUID\":\"" + guid + "\"}";

                streamWriter.Write(json);
                streamWriter.Flush();
                streamWriter.Close();
            }

            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            RetornoUserSystem resultado;
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();

                resultado = (RetornoUserSystem)JsonConvert.DeserializeObject<RetornoUserSystem>(result);
            }
            return resultado;
        }
    }

    public class RetornoUserSystem
    {
        public string Token { get; set; }
        public InfoUserSystem InfoUserSystem = new InfoUserSystem();
    }

    public class ClientesCsOnline
    {
        public string id { get; set; }
        public string name { get; set; }
        public string shippingCondition { get; set; }
        public bool cAllowOrderFob { get; set; }
    }

    public class ClientesCsTrading
    {
        public string Cnpj { get; set; }
        public string CompanyName { get; set; }
        public int NetworkId { get; set; }
        public string NetworkDescription { get; set; }
        public int IsActive { get; set; }
    }
}
