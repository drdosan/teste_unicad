using Raizen.UniCad.Model.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace Raizen.UniCad.Model
  {
    public class ChecklistComposicao : ChecklistComposicaoBase
    {
        [NotMapped]
        public bool isReplicarEab;
    }
}
  
