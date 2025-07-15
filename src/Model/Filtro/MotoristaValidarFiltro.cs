using Raizen.UniCad.Model.View;
using System;

namespace Raizen.UniCad.Model.Filtro
{
    /// <summary>
    /// Filtro para validar Documentos do motorista via API(MotoristaController)
    /// </summary>
    public class MotoristaValidarFiltro
    {
        public string CPF { get; set; }
        public string SiglaTerminal { get; set; }
        public int MonthQtdOperationWithoutEffusion { get; set; }
        public int MonthQtdNr35Driving { get; set; }
        public int MonthQtdDefensiveDriving { get; set; }
        public int WarningAdviceTime { get; set; }
    }
}
