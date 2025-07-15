using Raizen.UniCad.BLL;
using Raizen.UniCad.Model;
using Raizen.UniCad.Model.View;
using Raizen.UniCad.Utils;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web;
using System.Web.Http;

namespace Raizen.UniCad.Web.Controllers.Api
{
    [RoutePrefix("api/v1/composicoes")]
    public class ComposicaoController : ApiController
    {
        private ComposicaoBusiness _composicaoBusiness;

        public ComposicaoController() : this(new ComposicaoBusiness())
        {

        }

        public ComposicaoController(ComposicaoBusiness composicaoBusiness)
        {
            this._composicaoBusiness = composicaoBusiness;
        }

        /// <summary>
        /// Consulta Composição(ões) através de múltiplos parâmetros.
        /// </summary>
        /// <param name="DataHora">Data e hora da última execução no formato aaaa-mm-dd'T'HH:MM</param>
        /// <param name="LinhaNegocios">Linha de negócios, possíveis opções: 1-EAB, 2-COMB, 3-AMBAS</param>
        /// <param name="Operacao">Operação, possíveis opções: FOB e CIF</param>
        /// <param name="Placa">Uma das placa da composição (XXX9999)</param>

        [HttpGet]
        [SwaggerResponse(System.Net.HttpStatusCode.OK, null, typeof(List<ComposicaoServicoView>))]
        [SwaggerResponse(System.Net.HttpStatusCode.BadRequest, "Erro ao processar solicitação")]
        [Route("")]
        public IHttpActionResult Get(DateTime DataHora, int? LinhaNegocios = null, string Operacao = null, string Placa = null)
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

                return Ok(_composicaoBusiness.ListarComposicaoServico(new Model.Filtro.ComposicaoServicoFiltro
                {
                    DataAtualizacao = DataHora,
                    Operacao = Operacao,
                    PlacaVeiculo = Placa,
                    LinhaNegocio = LinhaNegocios
                }));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}