using Raizen.UniCad.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raizen.UniCad.BLL.Interfaces
{
    public interface IComposicaoBusiness
    {
        Composicao Selecionar(int id);
        bool AtualizarComposicao(Composicao Composicao, bool comRessalvas, bool bloqueio = false, bool enviaEmail = true, int idStatus = 0, bool aprovacaoAutomatica = false);
    }
}
