using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Raizen.UniCad.Model.Filtro;
using Raizen.UniCad.Model.View;
using Raizen.UniCad.Model;

namespace Raizen.UniCad.Web.Models
{
    public class ModelAgendamentoTreinamento : BaseModel, IValidatableObject
    {
        #region Constantes
        public AgendamentoTreinamentoFiltro Filtro { get; set; }
        public AgendamentoTreinamento AgendamentoTreinamento { get; set; }
        public List<AgendamentoTreinamentoView> ListaAgendamentoTreinamento { get; set; }
        public List<AgendamentoTreinamentoView> ListaAgendamentoTerminalHorario { get; set; }
        public int? IDComposicao { get; set; }
        public string Placas { get; set; }
        public string Mensagem { get; internal set; }
        public object NomeMotorista { get; internal set; }
        public bool isEditar { get; set; }
        #endregion

        #region Validação de Integridade
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var results = new List<ValidationResult>();

            //if (this.AgendamentoTreinamento != null)
            //{
            //    AgendamentoTreinamentoBusiness appBll = new AgendamentoTreinamentoBusiness();
            //}

            return results;
        }
        #endregion

    }
}