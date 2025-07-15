using Raizen.UniCad.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raizen.UniCad.BLL
{
	public class MotoristaTipoProdutoBusiness : UniCadBusinessBase<MotoristaTipoProduto>
	{
        public List<TipoProduto> ListarTipoProdutoPorMotorista(int idMotorista)
        {
            var tcBLL = new TipoProdutoBusiness();
            var tiposProdutoIdList = Listar(w => w.IDMotorista == idMotorista).Select(w => w.IDTipoProduto);

            return tcBLL.Listar(w => tiposProdutoIdList.Contains(w.ID));
        }
    }
}
