using Raizen.UniCad.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raizen.UniCad.BLL.Interfaces
{
    public interface IMotoristaBusiness
    {
        Motorista Selecionar(int id);

        bool AtualizarMotorista(Motorista motorista, bool comRessalvas, bool bloqueio = false);
        bool VerificarAlteracoesApenasTelefoneEmail(Motorista motorista, bool naoVerificarCliente = false);
        bool AlterarTelefoneEmailMotorista(Motorista motorista);
        string EnviarDadosQuickTAS(Motorista motorista);
    }
}
