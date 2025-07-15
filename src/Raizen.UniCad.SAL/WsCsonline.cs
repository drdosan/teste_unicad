using Newtonsoft.Json.Linq;
using Raizen.UniCad.Model.View;
using Raizen.UniCad.SAL.Utils;
using Raizen.UniCad.Utils;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Services;
using System.Web.UI.WebControls;

namespace Raizen.UniCad.SAL
{
    public class WsCsonline
    {
        private string _pin;
        private string _applicationKey;
        private string _endpoint;
        private string _origin;
        private string _token;
        private DateTime _expiration;
        private WebserviceWrapper _webService;
        private Dictionary<string, string> _headers;

        public static string SERVICE_NAME = "CSOnline";
        public static string LOGIN = "/proxy/loginApplication";
        public static string UPDATE_DRIVER = "/proxy/unicad/driver";
        public static string UPDATE_VEHICLE = "/proxy/unicad/vehicle";

        public WsCsonline(string endpoint, string origin, string pin, string applicationKey)
        {
            _endpoint = endpoint;
            _origin = origin;
            _pin = pin;
            _applicationKey = applicationKey;
            _expiration = DateTime.MinValue;
            _webService = new WebserviceWrapper(_endpoint, SERVICE_NAME);
            _headers = new Dictionary<string, string>();

            _headers.Add("Accept-Language", "pt-BR");
            _headers.Add("Origin", _origin);
        }

        private bool TokenEstaExpirado()
        {
            return _expiration < DateTime.Now;
        }

        private void ObtemNovoToken()
        {
            var loginView = new CsonlineLoginRequestView
            {
                Pin = _pin,
                ApplicationKey = _applicationKey,
            };

            var loginResult = _webService.EnviarJson<CsonlineLoginResponseView>(loginView, HttpMethod.Post, LOGIN, _headers);

            if (loginResult != null)
            {
                _token = loginResult.Token;

                var jwtHandler = new JwtSecurityTokenHandler();
                var jwtTokenObj = jwtHandler.ReadJwtToken(_token);

                var expClaim = jwtTokenObj.Payload.Exp;

                _expiration = UnixTimeStampParaDateTime(double.Parse(expClaim.ToString()));
            }
            else
            {
                throw new Exception("Não foi possível obter um novo token JWT no CsOnline");
            }
        }

        static DateTime UnixTimeStampParaDateTime(double unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return epoch.AddSeconds(unixTimeStamp).ToLocalTime();
        }

        public void CriarOuAtualizarMotorista(CsonlineDriverUpdateRequestView motoristaRequest)
        {
            if (TokenEstaExpirado())
            {
                ObtemNovoToken();
            }

            var operacaoResult = _webService.EnviarJson<CsonlineUpdateResponseView>(
                motoristaRequest, HttpMethod.Post, UPDATE_DRIVER, _headers, _token);

            if (operacaoResult == null || !operacaoResult.Success)
            {
                LogarErros("CriarOuAtualizarMotorista", operacaoResult?.Error);
                throw new Exception("Não foi possível criar ou atualizar o motorista no CsOnline");
            }
        }


        public void CriarOuAtualizarVeiculo(CsonlineVehicleUpdateRequestView vehicleRequest)
        {
            if (TokenEstaExpirado())
            {
                ObtemNovoToken();
            }

            var operacaoResult = _webService.EnviarJson<CsonlineUpdateResponseView>(
                vehicleRequest, HttpMethod.Post, UPDATE_VEHICLE, _headers, _token);

            if (operacaoResult == null || !operacaoResult.Success)
            {
                LogarErros("CriarOuAtualizarVeiculo", operacaoResult?.Error);
                throw new Exception("Não foi possível criar ou atualizar o veículo no CsOnline");
            }
        }

        private void LogarErros(string descricao, IEnumerable<string> erros)
        {
            LogUtil.GravarLog(SERVICE_NAME, descricao, erros != null ? string.Join(Environment.NewLine, erros) : "(vazio)", "usuario logado");
        }
    }
}
