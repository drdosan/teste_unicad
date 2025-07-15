
using System.Transactions;
using Raizen.Framework.Utils.Transacao;
using Raizen.UniCad.Model;

namespace Raizen.UniCad.BLL
{
    public class HistorioAtivarMotoristaBusiness : UniCadBusinessBase<HistorioAtivarMotorista>
    {
        public string AdicionarAtivar(HistorioAtivarMotorista historico, string emailSolicitante)
        {
            using (TransactionScope transactionScope = Transactions.CreateTransactionScope(TransactionScopeOption.Required, IsolationLevel.ReadCommitted, 5))
            {
                var motoBll = new MotoristaBusiness();
                var moto = motoBll.Selecionar(historico.IDMotorista);
                moto.Ativo = historico.Ativo;
                moto.EmailSolicitante = emailSolicitante;
                historico.IDMotorista = moto.ID;
                Adicionar(historico);
                var retorno = motoBll.AtualizarMotorista(moto, false, false, true);
                if (retorno)
                {
                    transactionScope.Complete();
                    return "Gravado com sucesso!";

                }

                return moto.Mensagem;
            }
        }
    }
}

