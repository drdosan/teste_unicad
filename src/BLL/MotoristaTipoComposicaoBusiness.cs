using Raizen.UniCad.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raizen.UniCad.BLL
{
	public class MotoristaTipoComposicaoBusiness : UniCadBusinessBase<MotoristaTipoComposicao>
	{
        public List<TipoComposicao> ListarTipoComposicaoPorMotorista(int idMotorista)
        {
            var tcBLL = new TipoComposicaoBusiness();
            var tiposComposicaoIdList = Listar(w => w.IDMotorista == idMotorista).Select(w => w.IDTipoComposicao);

            return tcBLL.Listar(w => tiposComposicaoIdList.Contains(w.ID));
        }
	}
}
