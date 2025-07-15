using Raizen.UniCad.BLL;
using Raizen.UniCad.Model;
using Raizen.UniCad.Model.Filtro;
using Raizen.UniCad.Model.View;
using Raizen.UniCad.Utils;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Http;

namespace Raizen.UniCad.Web.Controllers.Api
{
    [RoutePrefix("api/v1/placas")]
    public class PlacaController : ApiController
    {
        private PlacaBusiness _placaBusiness;

        public PlacaController() : this(new PlacaBusiness())
        {

        }

        public PlacaController(PlacaBusiness placaBusiness)
        {
            this._placaBusiness = placaBusiness;
        }

        /// <summary>
        /// Consulta Composição(ões) através de múltiplos parâmetros.
        /// </summary>
        /// <param name="LinhaNegocios">Linha de negócios, possíveis opções: 1-EAB, 2-COMB, 3-AMBAS</param>
        /// <param name="Operacao">Operação, possíveis opções: FOB e CIF</param>
        /// <param name="Placa">Placa do veículo (XXX9999)</param>
        [HttpGet]
        [SwaggerResponse(System.Net.HttpStatusCode.OK, null, typeof(List<PlacaServicoView>))]
        [SwaggerResponse(System.Net.HttpStatusCode.BadRequest, "Erro so processar solicitação")]
        [Route("")]
        public IHttpActionResult Get(int LinhaNegocios, string Operacao, string Placa)
        {
            try
            {
                HttpContext httpContext = HttpContext.Current;
                NameValueCollection headerList = httpContext.Request.Headers;
                var authorizationField = headerList.Get("Authorization");
                if (string.IsNullOrEmpty(authorizationField))
                    throw new Exception("Token Vazio");
                if (!Utils.ValidacoesUtil.ValidarTokenServico(authorizationField, (int)EnumPais.Padrao))
                    throw new Exception("Token Inválido");

                return Ok(_placaBusiness.ListarPlacaServico(new Model.Filtro.PlacaServicoFiltro { PlacaVeiculo = Placa, LinhaNegocio = LinhaNegocios, Operacao = Operacao }));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        ///  Validar placa e documentos do veículo
        /// </summary>
        /// <param name="Placa">Placa do veículo (XXX9999)</param>
        /// <param name="Data">Data da atualizacao no formato aaaa-mm-dd</param>
        /// <param name="LinhaNegocios">Linha de negócios, possíveis opções: 1-EAB, 2-COMB, 3-AMBAS</param>
        /// <param name="Operacao">Operação, possíveis opções: FOB e CIF</param>
        /// <param name="WarningAdviceTime">WarningAdviceTime, dias para aviso antecipado de vencimento dos documentos</param>
        /// <param name="ValidarComposicao">Validar Composição, True valida a composição, false valida somente o cavalo</param>
        /// <returns></returns>
        [HttpGet]
        [SwaggerResponse(System.Net.HttpStatusCode.OK, "Solicitação processada com sucesso.", typeof(List<MensagemValidacaoView>))]
        [SwaggerResponse(System.Net.HttpStatusCode.NotFound, "Erro ao processar solicitação.")]
        [Route("validacao")]

        public IHttpActionResult Validar(string Placa, DateTime Data, int WarningAdviceTime, bool ValidarComposicao = false, string Operacao = null, int? LinhaNegocios = null)
        {
            if (string.IsNullOrEmpty(Placa))
            {
                throw new ArgumentNullException(nameof(Placa));
            }

            HttpContext httpContext = HttpContext.Current;
            NameValueCollection headerList = httpContext.Request.Headers;
            var authorizationField = headerList.Get("Authorization");

            if (String.IsNullOrEmpty(authorizationField))
                throw new Exception("Token Vazio");
            if (!Utils.ValidacoesUtil.ValidarTokenServico(authorizationField, (int)EnumPais.Padrao))
                throw new Exception("Token Inválido");
            if (Data == DateTime.MinValue || !ValidacoesUtil.ValidarRangeData(Data.Year))
                throw new Exception("Data e Hora obrigatório ou inválido, tente novamente");


            ComposicaoBusiness _composicaoBusiness = new ComposicaoBusiness();
            var lstComposicaoView = _composicaoBusiness.ListarComposicaoAAServico(new Model.Filtro.ComposicaoServicoFiltro
            {
                DataAtualizacao = Data,
                Operacao = string.IsNullOrWhiteSpace(Operacao) ? null : Operacao,
                PlacaVeiculo = Placa,
                LinhaNegocio = LinhaNegocios,
            });


            var listaComposicao = new ComposicaoPesquisaBusiness().ListarComposicao(new ComposicaoFiltro()
            {
                Placa = Placa,
            },
            new Framework.Models.PaginadorModel()
            {
                QtdeItensPagina = 100,
                PaginaAtual = 0,
                InicioPaginacao = 0
            });

            if (listaComposicao != null)
            {
                listaComposicao = listaComposicao.Where(x => x.IDStatus == (int)EnumStatusComposicao.Aprovado).ToList();

                if (listaComposicao.Any())
                {
                    var maxDateTime = listaComposicao.Max(x => x.DataAtualizacao);


                    var tipoComposicao = listaComposicao.First(x => x.DataAtualizacao == maxDateTime)?.TipoComposicao;

                    if (tipoComposicao != null)
                    {
                        ValidarComposicao = new List<string>() { "truck" }.Contains<string>(tipoComposicao, StringComparer.InvariantCultureIgnoreCase);
                    }
                }
            }

            PlacaValidarFiltro placaFiltro = new PlacaValidarFiltro
            {
                DataHora = Data,
                Placa = Placa,
                LinhaNegocios = LinhaNegocios,
                Operacao = Operacao,
                WarningAdviceTime = WarningAdviceTime,
                IsValidarComposicao = ValidarComposicao
            };

            try
            {
                List<MensagemValidacaoView> lstMensagemResult = new List<MensagemValidacaoView>();
                if (lstComposicaoView == null || lstComposicaoView.Count == 0)
                {
                    return new HttpActionResultHelper(HttpStatusCode.NotFound,
                        "[{\"mensagem\":  \"O veículo *" + placaFiltro.Placa + "* não foi encontrado no sistema.\",\"restringirOperacao\": true }]");
                }


                foreach (ComposicaoAAServicoView composicao in lstComposicaoView)
                {
                    var msgRetorno = _placaBusiness.ValidarPlacasDocumentos(composicao, placaFiltro);
                    if (msgRetorno != null && msgRetorno.Count > 0)
                        lstMensagemResult.AddRange(msgRetorno);
                    break;
                }

                return Ok(lstMensagemResult);

            }
            catch (Exception ex)
            {
                return new HttpActionResultHelper(HttpStatusCode.NotFound, ex.Message);
            }

        }


    }
}