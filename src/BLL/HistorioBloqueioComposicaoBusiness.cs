
using System.Transactions;
using Raizen.Framework.Utils.Transacao;
using Raizen.UniCad.BLL.Util;
using Raizen.UniCad.Model;

namespace Raizen.UniCad.BLL
{
    public class HistorioBloqueioComposicaoBusiness : UniCadBusinessBase<HistorioBloqueioComposicao>
    {
        public string AdicionarBloqueio(HistorioBloqueioComposicao bloqueio, EnumPais pais)
        {
            using (TransactionScope transactionScope = Transactions.CreateTransactionScope(TransactionScopeOption.Required, IsolationLevel.ReadCommitted, 5))
            {
                Adicionar(bloqueio);
                var compBll = new ComposicaoBusiness(pais);
                var comp = compBll.Selecionar(bloqueio.IDComposicao);
                comp.UsuarioAlterouStatus = bloqueio.CodigoUsuario;
                comp.tipoIntegracao = bloqueio.Bloqueado ? EnumTipoIntegracaoSAP.Bloqueio : EnumTipoIntegracaoSAP.Desbloqueio;
                int idStatus = bloqueio.Bloqueado ? (int)EnumStatusComposicao.Bloqueado : (int)EnumStatusComposicao.Aprovado;
                var retorno = compBll.AtualizarComposicao(comp, false, true, idStatus: idStatus);
                if (retorno)
                {
                    transactionScope.Complete();
                    return Traducao.GetTextoPorLingua("Gravado com sucesso!", "¡Grabado con éxito!", pais);

                }

                return comp.Mensagem;
            }
        }
    }
}

