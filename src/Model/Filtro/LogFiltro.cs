using System;
using System.ComponentModel.DataAnnotations;

namespace Raizen.UniCad.Model.Filtro
{
    public class LogFiltro
    {
        /// <summary>
        /// Origem do log.
        /// </summary>
        public string Origem { get; set; }

        /// <summary>
        /// Data inicial do período de ocorrência dos logs.
        /// </summary>
        public DateTime? DataInicial { get; set; }

        /// <summary>
        /// Data final do período de ocorrência dos logs.
        /// </summary>
        public DateTime? DataFinal { get; set; }
    }
}
