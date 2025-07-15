using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Raizen.UniCad.Model;
using Raizen.UniCad.Model.Filtro;
using Raizen.UniCad.Model.View;
using Raizen.UniCad.BLL;

namespace Raizen.UniCad.Web.Models
{
    public class ModelImpressaoCracha : BaseModel, IValidatableObject
    {
        public ImpressaoCrachaFiltro Filtro { get; set; }
        public MotoristaView MotoristaView { get; set; }
        public HttpPostedFileBase Foto { get; set; }
        public ImpressaoCrachaRetornoView ImpressaoCrachaRetornoView { get; set; }
        public Motorista Motorista { get; set; }
        


        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var results = new List<ValidationResult>();

            return results;
        }
    }
}