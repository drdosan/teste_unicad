using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Raizen.UniCad.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Raizen.UniCad.SAL.Utils
{
    public class WebserviceWrapper
    {
        private string _endpoint;
        private string _serviceName;
        private static readonly HttpClient _httpClient = new HttpClient();

        public WebserviceWrapper(string endpoint, string serviceName)
        {
            _endpoint = endpoint;
            _serviceName = serviceName;
        }

        public T EnviarJson<T>(object dados, HttpMethod method, string path, Dictionary<string, string> headers = null) where T : class
        {
            return EnviarJson<T>(dados, method, path, headers, null);
        }

        public T EnviarJson<T>(object dados, HttpMethod method, string path, Dictionary<string, string> headers, string token) where T : class
        {
            try
            {
                HttpRequestMessage message = new HttpRequestMessage()
                {
                    Method = method,
                    Content = new StringContent(JsonConvert.SerializeObject(dados), Encoding.UTF8, "application/json"),
                    RequestUri = new Uri($"{_endpoint}{path}"),
                };

                var response = Enviar(message, headers, token);

                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    LogUtil.GravarLog(_serviceName, string.Format("{0} - Executado ok !", "EnviarJson"), "url  " + _endpoint + " .Response.StatusCode == HttpStatusCode.Unauthorized", "usuario logado");
                }

                return response.IsSuccessStatusCode ? DeserializarResposta<T>(response.Content) : null;
            }
            catch (Exception ex)
            {
                LogUtil.GravarLog(_serviceName, string.Format("{0} - Executado com erro!", "EnviarJson"), ex.Message, "usuario logado");
                throw;
            }
        }

        private HttpResponseMessage Enviar(HttpRequestMessage message, Dictionary<string, string> headers, string token)
        {
            try
            {
                if (!String.IsNullOrEmpty(token))
                {
                    message.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }

                if (headers?.Count > 0)
                {
                    foreach(var kv in headers)
                    {
                        message.Headers.Add(kv.Key, kv.Value);
                    }
                }

                return _httpClient.SendAsync(message).GetAwaiter().GetResult();

            }
            catch (Exception)
            {
                throw;
            }
        }

        public T EnviarPostFormData<T>(string url, IEnumerable<KeyValuePair<string, string>> dados) where T : class
        {
            var content = new MultipartFormDataContent();

            foreach (var pair in dados)
            {
                content.Add(new StringContent(pair.Value), pair.Key);
            }

            var request = _httpClient.PostAsync(url, content).GetAwaiter().GetResult();

            return request.IsSuccessStatusCode ? DeserializarResposta<T>(request.Content) : null;
        }

        private T DeserializarResposta<T>(HttpContent resposta)
        {
            return JsonConvert.DeserializeObject<T>(resposta.ReadAsStringAsync().GetAwaiter().GetResult());
        }
    }
}
