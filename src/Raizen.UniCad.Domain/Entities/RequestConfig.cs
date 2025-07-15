using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raizen.UniCad.Domain.Entities
{
    public class RequestConfig
    {
        public string BaseUrl { get; set; }
        public int DefaultCacheExpirationInMinutes { get; set; }
        public string Method { get; set; }

        public IDictionary<string, string> Headers { get; set; }
        public IDictionary<string, string> Parameters { get; set; }
        public IDictionary<string, string> Body { get; set; }

        public object JsonBody { get; set; }
        public string CompleteFilePath { get; set; }
        public string Soap { get; set; }
        public string SoapRequest { get; set; }
        public string ApiRequest { get; set; }
        public string AuhthenticationUserName { get; set; }
        public string AuhthenticationPassword { get; set; }

        public RequestConfig()
        {
            Method = "Get";
            Headers = new Dictionary<string, string>();
            Parameters = new Dictionary<string, string>();
            Body = new Dictionary<string, string>();
        }
    }
}
