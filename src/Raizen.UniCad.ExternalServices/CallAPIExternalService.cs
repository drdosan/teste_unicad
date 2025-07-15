using Raizen.UniCad.Domain.Entities;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Raizen.UniCad.ExternalServices
{
    public class CallAPIExternalService
    {

        public string CallApiRestSharp(RequestConfig config)
        {
            try
            {
                var client = new RestClient(config.BaseUrl);

                var method = string.IsNullOrEmpty(config.Method) || string.Equals(config.Method, "POST") ? Method.POST :
                             string.Equals(config.Method, "PUT") ? Method.PUT :
                             string.Equals(config.Method, "DELETE") ? Method.DELETE : Method.GET;

                var request = new RestRequest();
                request.Method = method;

                foreach (var item in config.Headers)
                {
                    request.AddHeader(item.Key, item.Value);
                }

                foreach (var item in config.Parameters)
                {
                    request.AddParameter(item.Key, item.Value);
                }

          
                if (config.JsonBody != null)
                {
                    request.AddHeader("Content-type", "application/json-patch+json");
                    request.AddJsonBody(config.JsonBody);
                }

                if (!string.IsNullOrEmpty(config.CompleteFilePath))
                {
                    request.AddFile("source", config.CompleteFilePath);
                }

                var cancellationTokenSource = new CancellationTokenSource();

                var restResponse = client.ExecuteTaskAsync(request, cancellationTokenSource.Token).Result;

                return restResponse.Content;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
