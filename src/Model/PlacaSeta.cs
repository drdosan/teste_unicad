
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using DataAnnotationsExtensions;
using Raizen.Framework.Models;

using Raizen.UniCad.Model.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace Raizen.UniCad.Model
  {
    public class PlacaSeta : PlacaSetaBase
    {
        public bool isPrincipalAlterado;

        [NotMapped]
        public bool isVolumeAlterado { get; set; }

        [NotMapped]
        public List<Produto> Produtos { get; set; }
        [NotMapped]
        public decimal VolumeTotalCompartimento { get; set; }

        public PlacaSeta()
        {

        }

        public PlacaSeta(int colunas, int sequencial, bool multiSeta) : base(colunas, sequencial, multiSeta)
        {

        }
    }
}
  
