using Raizen.UniCad.Model;
using Raizen.UniCad.SAL.WsIntegracaoSAPMotorista;

namespace Raizen.UniCad.SAL.Interfaces
{
    public interface IWsIntegraSAP
    {
        string VincularVeiculoClienteSap(Placa placa);
        string ExcluirPlacaClienteSap(Placa placa);
        string VincularClienteSap(VincularClienteRequestClienteMotorista[] motorista);
    }
}
