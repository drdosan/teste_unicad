using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Raizen.UniCad.Web.Models
{
    public class ModelHome : BaseModel
    {
        [Required(ErrorMessage="Id obrigatório")]
        public int id { get; set; }
    }
}