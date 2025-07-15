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

namespace Raizen.UniCad.Web.Models
{
    public class ModelJob : BaseModel, IValidatableObject
    {
        #region Constantes
        public JobFiltro Filtro { get; set; }
        public Job Job { get; set; }
        public List<Job> ListaJob { get; set; }
        #endregion

        #region Validação de Integridade
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var results = new List<ValidationResult>();

            if (this.Job != null)
            {
                ConfiguracaoBusiness appBll = new ConfiguracaoBusiness();

                if (!string.IsNullOrEmpty(this.Job.Nome))
                {
                    if (!string.IsNullOrEmpty(this.ChavePrimaria))
                    {
                        Configuracao ConfiguracaoOld = appBll.Selecionar(int.Parse(this.ChavePrimaria));

                        if (!ConfiguracaoOld.NmVariavel.Equals(this.Job.Nome))
                        {
                            Configuracao Configuracao = appBll.Selecionar(item => item.NmVariavel.Equals(this.Job.Nome));

                            if (Configuracao != null)
                            {
                                results.Add(new ValidationResult("Já existe JOB com esse nome.", new string[] { "JOB_Nome" }));
                                return results;
                            }

                        }
                    }
                }
            }

            return results;
        }
        #endregion

    }
}