
using System.Transactions;
using Infraestructure.Extensions;
using Raizen.Framework.Utils.Transacao;
using Raizen.UniCad.BLL.Util;
using Raizen.UniCad.Model;

namespace Raizen.UniCad.BLL
{
    public class HistorioBloqueioMotoristaBusiness : UniCadBusinessBase<HistorioBloqueioMotorista>
    {
        public string AdicionarBloqueio(HistorioBloqueioMotorista historico, string emailSolicitante, EnumPais pais)
        {
            using (TransactionScope transactionScope = Transactions.CreateTransactionScope(TransactionScopeOption.Required, IsolationLevel.ReadCommitted, 5))
            {
                var motoPesquisaBll = new MotoristaPesquisaBusiness();
                var moto = motoPesquisaBll.Selecionar(historico.IDMotorista).Mapear();
                var motoBll = new MotoristaBusiness(pais);
                moto.IDStatus = historico.Bloqueado ? (int)EnumStatusMotorista.Bloqueado : (int)EnumStatusMotorista.Aprovado;
                moto.EmailSolicitante = emailSolicitante;
                moto.UsuarioAlterouStatus = historico.CodigoUsuario;
                historico.IDMotorista = moto.ID;
                Adicionar(historico);

                moto.Documentos = new MotoristaDocumentoBusiness().ListarDocumentos(moto.ID, moto.IdPais);
                moto.Clientes = new MotoristaClienteBusiness().ListarClientes(moto.ID);

                var retorno = motoBll.AtualizarMotorista(moto, false, false, true, historico.Bloqueado);
                if (retorno)
                {
                    transactionScope.Complete();
                    return Traducao.GetTextoPorLingua("Gravado com sucesso!", "¡Grabado con éxito!", pais);
                }

                return moto.Mensagem;
            }
        }
    }
}

