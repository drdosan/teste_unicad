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
    public class ModelConfiguracao : BaseModel, IValidatableObject
    {
        #region Constantes
        public ConfiguracaoFiltro Filtro { get; set; }
        public Configuracao Configuracao { get; set; }
        public List<Configuracao> ListaConfiguracao { get; set; }
        public List<Job> ListaJob { get; set; }
        public Usuario UsuarioLogado { get; set; }
        #endregion

        #region Validação de Integridade
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var results = new List<ValidationResult>();

            if (this.Configuracao != null && !string.IsNullOrEmpty(this.Configuracao.NmVariavel))
            {
                using (ConfiguracaoBusiness appBll = new ConfiguracaoBusiness())
                {
                    if (!string.IsNullOrEmpty(this.ChavePrimaria))
                    {
                        Configuracao AntigaConfiguracao = appBll.Selecionar(int.Parse(this.ChavePrimaria));

                        if (!AntigaConfiguracao.NmVariavel.Equals(this.Configuracao.NmVariavel))
                        {
                            return VerificaConfiguracao(appBll);
                        }
                    }
                    else
                    {
                        return VerificaConfiguracao(appBll);
                    }
                }
            }

            return results;
        }

        private List<ValidationResult> VerificaConfiguracao(ConfiguracaoBusiness appBll)
        {
            var results = new List<ValidationResult>();
            Configuracao ExisteConfiguracao = appBll.Selecionar(item => item.NmVariavel == this.Configuracao.NmVariavel && item.IdPais == this.Configuracao.IdPais);
            if (ExisteConfiguracao != null)
            {
                results.Add(new ValidationResult("Já existe Configuração com esse nome.", new string[] { "Configuracao_Nome" }));
                return results;
            }

            return results;
        }
        #endregion

    }
}