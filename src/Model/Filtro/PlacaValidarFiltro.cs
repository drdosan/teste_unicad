using System;

namespace Raizen.UniCad.Model.Filtro
{
    /// <summary>
    /// Filtro para validar Documentos da placa via API(PlacaController)
    /// </summary>
    public class PlacaValidarFiltro
    {
        /// <summary>
        /// Data e hora da última execução no formato aaaa-mm-dd'T'HH:MM
        /// </summary>
        public DateTime DataHora { get; set; }
        /// <summary>
        /// Placa do veículo (XXX9999)
        /// </summary>
        public string Placa { get; set; }
        /// <summary>
        /// Valida composição inteira, caso esteja false, valida apenas a 1ª carreta
        /// </summary>
        public bool IsValidarComposicao { get; set; }
        /// <summary>
        /// Dias que antecedem ao vencimento para alerta
        /// </summary>
        public int? WarningAdviceTime { get; set; }
        /// <summary>
        /// Linha de negócios, possíveis opções: 1-EAB, 2-COMB, 3-AMBAS
        /// </summary>
        public int? LinhaNegocios { get; set; }
        /// <summary>
        /// Operação, possíveis opções: FOB e CIF
        /// </summary>
        public string Operacao { get; set; }
    }
}
