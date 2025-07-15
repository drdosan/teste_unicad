using Newtonsoft.Json;
using Raizen.UniCad.BLL;
using Raizen.UniCad.Model;
using Raizen.UniCad.Web.Models;
using System.Collections.Generic;

namespace Raizen.UniCad
{
    /// <summary>
    /// Classe que retorna os Tipos de Contratacion
    /// </summary>
    public class TipoContratacaoComposicaoDelegate
    {

        public static List<ModelTipoContratacion> Listar()
        {
            ConfiguracaoBusiness configuracaoBusiness = new ConfiguracaoBusiness();

            var result = new List<ModelTipoContratacion>();

            Configuracao configuracao = configuracaoBusiness.Selecionar(w => w.NmVariavel == "TipoContratacion");

            if (configuracao != null)
            {
                result = JsonConvert.DeserializeObject<List<ModelTipoContratacion>>(configuracao.Valor);
            }

            return result;
        }
    }
}