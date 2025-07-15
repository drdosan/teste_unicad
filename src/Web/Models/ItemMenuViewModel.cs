using System.Collections.Generic;

namespace Raizen.UniCad.Web.Models
{
    public class ItemMenuViewModel
    {
        public ItemMenuViewModel()
        {
            Filhos = new List<ItemMenuViewModel>();
        }

        public string Nome { get; set; }
        public string Endereco { get; set; }
        public string Icone { get; set; }
        public bool RotaAtual { get; set; }
        public IEnumerable<ItemMenuViewModel> Filhos { get; set; }
    }
}