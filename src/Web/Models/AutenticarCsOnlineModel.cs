using Raizen.UniCad.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Raizen.UniCad.Web.Models
{
    public class AutenticarCsOnlineModel : BaseModel, IValidatableObject
    {
        public string Mensagem { get; set; }
        public int Usuario { get; set; }
        public string Email { get; set; }
        public string Redirect { get; set; }
        public int IdPais { get; set; }

        #region Validação de Integridade

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Equals(this.IdPais, (int)EnumPais.Brasil))
                return ValidateBrasil();

            return ValidateArgentina();
        }
        private IEnumerable<ValidationResult> ValidateArgentina()
        {
            List<ValidationResult> results = new List<ValidationResult>();

            if (string.IsNullOrEmpty(this.Email))
                results.Add(new ValidationResult("Se requiere correo electrónico", new string[] { "Email" }));

            return results;
        }
        private IEnumerable<ValidationResult> ValidateBrasil()
        {
            List<ValidationResult> results = new List<ValidationResult>();

            if (EmailIsValid())
                results.Add(new ValidationResult("O Email é obrigatório", new string[] { "Email" }));

            return results;
        }

        #endregion

        #region Private methods

        private bool EmailIsValid()
        {
            return string.IsNullOrEmpty(this.Email);
        }
        
        #endregion
    }
}