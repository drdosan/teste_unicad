using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using Raizen.UniCad.BLL;
using System.Text;
using Raizen.UniCad.Model;
using Raizen.UniCad.Model.Filtro;
using Raizen.UniCad.Model.View;
using Raizen.UniCad.Extensions;
using Raizen.UniCad.Utils;

namespace Raizen.UniCad.Web.Models
{
    public class ModelAgendamentoTerminal : BaseModel, IValidatableObject
    {
        #region Constantes

        public AgendamentoTerminalFiltro Filtro { get; set; }
        public AgendamentoTerminal AgendamentoTerminal { get; set; }
        public AgendamentoTerminalHorario AgendamentoTerminalHorario { get; set; }
        public List<AgendamentoTerminalView> ListaAgendamentoTerminal { get; set; }
        public List<AgendamentoTerminalHorarioView> ListaAgendamentoTerminalHorario { get; set; }
        public bool isPesquisa { get; set; }
        public bool isSalvar { get; set; }
        #endregion

        #region Validação de Integridade
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var results = new List<ValidationResult>();
            AgendamentoTerminalBusiness agendamentoTerminalBLL = new AgendamentoTerminalBusiness();
            if (this.AgendamentoTerminalHorario != null && AgendamentoTerminal != null && !isPesquisa && !isSalvar)
            {

                AgendamentoTerminalFiltro filtroAgendamentoTerminal = new AgendamentoTerminalFiltro
                {
                    IdTipoAgenda = AgendamentoTerminal.IDTipoAgenda,
                    IdTerminal = AgendamentoTerminal.IDTerminal,
                    Data = AgendamentoTerminal.Data,
                    HoraInicio = AgendamentoTerminalHorario.HoraInicio,
                    HoraFim = AgendamentoTerminalHorario.HoraFim,
                    Operacao = AgendamentoTerminalHorario.Operacao,
                    IdEmpresa = AgendamentoTerminalHorario.IDEmpresa,
                    IdHorario = AgendamentoTerminalHorario.idHoraAgenda

                };

                if (AgendamentoTerminalHorario.Vagas <= 0)
                {
                    results.Add(new ValidationResult("O número de vagas deve ser maior que zero.", new string[] { "AgendamentoTerminalHorario_Vagas" }));
                    return results;
                }

                if (AgendamentoTerminal.Data.Date < DateTime.Now.Date)
                {
                    results.Add(new ValidationResult("A data não pode ser menor que a data atual.", new string[] { "AgendamentoTerminal_Data" }));
                    return results;
                }

                if (!ValidacoesUtil.ValidarRangeData(AgendamentoTerminal.Data.Year))
                {
                    results.Add(new ValidationResult("A Data deve ser maior que 1800 e menor que 2900.", new string[] { "AgendamentoTerminal_Data" }));
                    return results;
                }

                if (agendamentoTerminalBLL.verificarSeJaExisteHorario(filtroAgendamentoTerminal))
                {
                    results.Add(new ValidationResult("Essa faixa de horário já existe.", new string[] { "Horario" }));
                    return results;
                }
                if (AgendamentoTerminalHorario.HoraInicio >= AgendamentoTerminalHorario.HoraFim)
                {
                    results.Add(new ValidationResult("A hora de início não pode ser maior ou igual a hora de fim.", new string[] { "Horario" }));
                    return results;
                }
            }

            if (isSalvar && AgendamentoTerminal != null)
            {
                AgendamentoTerminalFiltro filtroAgendamentoTerminal = new AgendamentoTerminalFiltro
                {
                    IdTipoAgenda = AgendamentoTerminal.IDTipoAgenda,
                    IdTerminal = AgendamentoTerminal.IDTerminal,
                    Data = AgendamentoTerminal.Data,
                    HoraInicio = AgendamentoTerminalHorario.HoraInicio,
                    HoraFim = AgendamentoTerminalHorario.HoraFim,
                    Operacao = AgendamentoTerminalHorario.Operacao,
                    IdEmpresa = AgendamentoTerminalHorario.IDEmpresa,
                    IdHorario = AgendamentoTerminalHorario.idHoraAgenda

                };

                var lista = agendamentoTerminalBLL.ListarTerminalHorario(filtroAgendamentoTerminal);
                if (lista == 0)
                {
                     results.Add(new ValidationResult("É necessário inserir pelo menos uma faixa de horário.", new string[] { "Horario" }));
                    return results;
                }
            }

            return results;
        }
        #endregion


    }
}