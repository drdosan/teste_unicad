using Raizen.UniCad.BLL;
using Raizen.UniCad.Model;
using Raizen.UniCad.Model.View;
using Raizen.UniCad.Utils;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Web;
using System.Linq;
using Raizen.UniCad.Model.Filtro;
using System.Net;
using System.Web.Http;
using System.Net.Http;
using System.Threading.Tasks;

namespace Raizen.UniCad.Web.Controllers.Api
{
    [RoutePrefix("api/v1/motoristas")]
    public class MotoristaController : ApiController
    {
        private MotoristaBusiness _motoristaBusiness;
        private DateTime Today { get { return DateTime.Today; } }
        public MotoristaController() : this(new MotoristaBusiness())
        {

        }

        public MotoristaController(MotoristaBusiness motoristaBusiness)
        {
            this._motoristaBusiness = motoristaBusiness;
        }

        /// <summary>
        /// Consulta Motorista(s) através de múltiplos parâmetros.
        /// </summary>
        /// <param name="DataHora">Data e hora da última execução no formato aaaa-mm-dd'T'HH:MM</param>
        /// <param name="LinhaNegocios">Linha de negócios, possíveis opções: 1-EAB, 2-COMB, 3-AMBAS</param>
        /// <param name="Operacao">Operação, possíveis opções: FOB e CIF</param>
        /// <param name="CPF">CPF do Motorista (XXXXXXXXXXX)</param>
        /// <param name="Terminal">Terminal a qual aquele motorista fez/fará treinamento</param>
        [HttpGet]
        [SwaggerResponse(System.Net.HttpStatusCode.OK, null, typeof(List<MotoristaServicoView>))]
        [SwaggerResponse(System.Net.HttpStatusCode.BadRequest, "Erro ao processar solicitação")]
        [Route("")]
        public IHttpActionResult Get(DateTime DataHora, int? LinhaNegocios = null, string Operacao = null, string CPF = null, string Terminal = null)
        {
            try
            {
                HttpContext httpContext = HttpContext.Current;
                NameValueCollection headerList = httpContext.Request.Headers;
                var authorizationField = headerList.Get("Authorization");
                if (String.IsNullOrEmpty(authorizationField))
                    throw new Exception("Token Vazio");
                if (!Utils.ValidacoesUtil.ValidarTokenServico(authorizationField, (int)EnumPais.Padrao))
                    throw new Exception("Token Inválido");
                if (DataHora == DateTime.MinValue || !ValidacoesUtil.ValidarRangeData(DataHora.Year))
                    throw new Exception("Data e Hora obrigatório ou inválido, tente novamente");
                if (!String.IsNullOrEmpty(Operacao) && Operacao.Length > 10)
                    throw new Exception("Operação inválida, tenta novamente");
                if (!String.IsNullOrEmpty(Terminal) && Terminal.Length > 10)
                    throw new Exception("Terminal inválido, tenta novamente");
                if (!String.IsNullOrEmpty(CPF) && CPF.Length > 11)
                    throw new Exception("CPF inválido, tenta novamente");
                return Ok(_motoristaBusiness.ListarMotoristaServico(new Model.Filtro.MotoristaServicoFiltro
                {
                    CPF = CPF,
                    DataAtualizacao = DataHora,
                    LinhaNegocio = LinhaNegocios,
                    Operacao = Operacao,
                    Terminal = Terminal
                }));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Validar CPF, Documentos e treinamentos de motoristas
        /// </summary>
        /// <param name="CPF">CPF do Motorista (XXXXXXXXXXX)</param>
        /// <param name="SiglaTerminal">Terminal a qual aquele motorista fará carga/descarga</param>
        /// <param name="WarningAdviceTime">WarningAdviceTime, dias para aviso antecipado de vencimento dos documentos</param>
        /// <param name="MonthQtdDefensiveDriving">MonthQtdDefensiveDriving, meses para vencimento "Direção Defensiva"</param>
        /// <param name="MonthQtdNr35Driving">MonthQtdNr35Driving, meses para vencimento da NR35</param>
        /// <param name="MonthQtdOperationWithoutEffusion">MonthQtdOperationWithoutEffusion, meses para vencimento de operação "Sem Derrame"</param>
        /// <returns></returns>
        [HttpGet]
        [SwaggerResponse(System.Net.HttpStatusCode.OK, "Solicitação processada com sucesso.", typeof(List<MensagemValidacaoView>))]
        [SwaggerResponse(System.Net.HttpStatusCode.NotFound, "Erro ao processar solicitação.")]
        [Route("validacao")]
        public IHttpActionResult Validar(string CPF, string SiglaTerminal, int WarningAdviceTime
            , int MonthQtdDefensiveDriving, int MonthQtdNr35Driving, int MonthQtdOperationWithoutEffusion)
        {
            try
            {
                HttpContext httpContext = HttpContext.Current;
                NameValueCollection headerList = httpContext.Request.Headers;
                var authorizationField = headerList.Get("Authorization");
                if (String.IsNullOrEmpty(authorizationField))
                    throw new Exception("Token Vazio");
                if (!Utils.ValidacoesUtil.ValidarTokenServico(authorizationField, (int)EnumPais.Padrao))
                    throw new Exception("Token Inválido");
                if (string.IsNullOrEmpty(SiglaTerminal) || SiglaTerminal.Length > 10)
                    throw new Exception("Terminal inválido, tenta novamente");
                if (string.IsNullOrEmpty(CPF) || CPF.Length > 11)
                    throw new Exception("CPF inválido, tentar novamente");

                //// Data de Início para considerar atualizações. 
                //// Consideraremos as atualizações à partir de um ano, até a data atual
                var dataHoraAtualizacao = DateTime.Now.AddYears(-1);
                var motoristas = _motoristaBusiness.ListarMotoristaAAServico(new Model.Filtro.MotoristaServicoFiltro
                {
                    CPF = CPF,
                    DataAtualizacao = dataHoraAtualizacao,
                    Terminal = SiglaTerminal,
                    //Operacao = operacao,
                });

                if (motoristas.Count < 1)
                {
                    return new HttpActionResultHelper(HttpStatusCode.NotFound, "[{\"mensagem\":  \"CPF *" + CPF + "* não encontrado. Verifique o número digitado.\",\"restringirOperacao\": true }]");
                }

                MotoristaValidarFiltro motoristaFiltro = new MotoristaValidarFiltro
                {
                    CPF = CPF,
                    SiglaTerminal = SiglaTerminal,
                    MonthQtdDefensiveDriving = MonthQtdDefensiveDriving,
                    MonthQtdNr35Driving = MonthQtdNr35Driving,
                    MonthQtdOperationWithoutEffusion = MonthQtdOperationWithoutEffusion
                };

                var msgValidacaoMotorista = _motoristaBusiness.ValidarMotorista(motoristas, motoristaFiltro);
                return Ok(msgValidacaoMotorista);

            }
            catch (Exception ex)
            {
                return new HttpActionResultHelper(HttpStatusCode.NotFound, ex.Message);
            }
        }
    }
}