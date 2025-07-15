using Newtonsoft.Json;
using Raizen.UniCad.Model.View;
using Raizen.UniCad.Utils;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web.Services.Description;

namespace Raizen.UniCad.SAL
{
    public class WsSalesForce
    {
        private static readonly object _lock = new object();
        private static readonly HttpClient _httpClient = new HttpClient();
        private static DateTime _tempoRefresh = DateTime.MinValue;
        private static string _token;

        private readonly string _urlSalesforce;
        private readonly string _clientId;
        private readonly string _clientSecret;

        public WsSalesForce(string urlSalesforce, string clientId, string clientSecret)
        {
            _urlSalesforce = urlSalesforce;
            _clientId = clientId;
            _clientSecret = clientSecret;
        }

        public string CriarNovoTicket(SalesForceCriarNovoTicketView parametros)
        {
            if (_tempoRefresh < DateTime.Now || _tempoRefresh == DateTime.MinValue)
            {
                var novoToken = BuscarToken(_clientId, _clientSecret);

                if (string.IsNullOrWhiteSpace(novoToken))

                    return "";
            }

            var resultado = EnviarJson<SalesForceResultadoView>(parametros, HttpMethod.Post, "/services/apexrest/cases/cases-by-unicad");

            return resultado?.Ticket;
        }

        public string EncerrarTicket(SalesForceEncerrarTicketView parametros)
        {
			if (_tempoRefresh < DateTime.Now || _tempoRefresh == DateTime.MinValue)
			{
				var novoToken = BuscarToken(_clientId, _clientSecret);

				if (string.IsNullOrWhiteSpace(novoToken))

					return "";
			}

			var resultado = EnviarJson<SalesForceResultadoView>(parametros, HttpMethod.Put, "/services/apexrest/cases/cases-by-service-now");

			return resultado?.Ticket;
		}

		private T EnviarJson<T>(object dados, HttpMethod method, string path) where T : class
        {
            try
            {

                HttpRequestMessage message = new HttpRequestMessage()
                {
                    Method = method,
                    Content = new StringContent(JsonConvert.SerializeObject(dados), Encoding.UTF8, "application/json"),
                    RequestUri = new Uri($"{_urlSalesforce}{path}")
                };

                var response = Enviar(message);

                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    LogUtil.GravarLog("WsSalesforce", string.Format("{0} - Executado ok !", "EnviarJson"), "url  " + _urlSalesforce + "token " + _token + " .Response.StatusCode == HttpStatusCode.Unauthorized", "usuario logado");

                    var novoToken = BuscarToken(_clientId, _clientSecret, force: true);

                    if (string.IsNullOrWhiteSpace(novoToken))
                        return null;

                    message = new HttpRequestMessage()
                    {
                        Method = method,
                        Content = new StringContent(JsonConvert.SerializeObject(dados), Encoding.UTF8, "application/json"),
                        RequestUri = new Uri(_urlSalesforce)
                    };

                    response = Enviar(message, novoToken);

                    if (response.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        LogUtil.GravarLog("WsSalesforce", string.Format("{0} - Executado ok !", "EnviarJson"), "url  " + _urlSalesforce + "token " + _token + " .Response.StatusCode == HttpStatusCode.Unauthorized", "usuario logado");
                    }
                }
                else if (!response.IsSuccessStatusCode)
                {
					LogUtil.GravarLog("WsSalesforce", string.Format("{0} - Executado com erro !", "EnviarJson"), "url  " + _urlSalesforce + "token " + _token + " .Response.StatusCode == " + response.StatusCode.ToString(), "usuario logado");
				}

				return response.IsSuccessStatusCode ? DeserializarResposta<T>(response.Content) : null;

            }
            catch (Exception ex)
            {
                LogUtil.GravarLog("AbrirChamadoSalesForce", string.Format("{0} - Executado com erro!", "EnviarJson"), ex.Message, "usuario logado");
                throw;
            }

        }


        private HttpResponseMessage Enviar(HttpRequestMessage message, string token)
        {
            try
            {
                message.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                return _httpClient.SendAsync(message).GetAwaiter().GetResult();

            }
            catch (Exception)
            {
                throw;
            }
        }

        private HttpResponseMessage Enviar(HttpRequestMessage message)
        {
            try
            {
                message.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _token);
                return _httpClient.SendAsync(message).GetAwaiter().GetResult();

            }
            catch (Exception)
            {
                throw;
            }
        }

        private static string BuscarToken(string clientId, string clientSecret, bool force = false)
        {
            lock (_lock)
            {
                if (!force && _tempoRefresh > DateTime.Now)
                {
                    return _token;
                }

                var wsUrl = ConfigurationManager.AppSettings["urlWSTokenSalesForce"];

                var dados = new Dictionary<string, string>()
                {
                    ["username"] = ConfigurationManager.AppSettings["usuarioSalesForce"],
                    ["password"] = ConfigurationManager.AppSettings["senhaSalesForce"],
                    ["client_id"] = clientId,
                    ["client_secret"] = clientSecret,
                    ["grant_type"] = "password"
                };

                var resultado = EnviarPostFormData<SalesForceTokenView>(wsUrl, dados);

                if (!string.IsNullOrEmpty(resultado?.Access_token))
                {
                    _token = resultado.Access_token;
                    _tempoRefresh = DateTime.Now.AddMinutes(30);
                }

                return resultado?.Access_token;
            }
        }

        private static T EnviarPostFormData<T>(string url, IEnumerable<KeyValuePair<string, string>> dados) where T : class
        {
            var content = new MultipartFormDataContent();

            foreach (var pair in dados)
            {
                content.Add(new StringContent(pair.Value), pair.Key);
            }

            var request = _httpClient.PostAsync(url, content).GetAwaiter().GetResult();

            return request.IsSuccessStatusCode ? DeserializarResposta<T>(request.Content) : null;
        }

        private static T DeserializarResposta<T>(HttpContent resposta)
        {
            return JsonConvert.DeserializeObject<T>(resposta.ReadAsStringAsync().GetAwaiter().GetResult());
        }
    }
}
