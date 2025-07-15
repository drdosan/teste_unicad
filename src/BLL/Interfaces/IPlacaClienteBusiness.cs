using Raizen.UniCad.Model;
using Raizen.UniCad.Model.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Raizen.UniCad.BLL.Interfaces
{
    public interface IPlacaClienteBusiness
    {
        bool ExcluirLista(Expression<Func<PlacaCliente, bool>> where);

        List<PlacaClienteView> ListarClientesPorPlaca(int IDPlaca, int IDUsuarioCliente = 0);
    }
}
