using Raizen.UniCad.Model;
using Raizen.UniCad.Model.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Raizen.UniCad.DAL.Interfaces
{
    public interface IPlacaClienteRepository
    {
        void ExcluirLista(IEnumerable<PlacaCliente> placaClientes);

        IEnumerable<PlacaCliente> SelecionarLista(Expression<Func<PlacaCliente, bool>> where);

        IQueryable<PlacaClienteView> BuscaClientesPlaca(int IDPlaca);

        IQueryable<PlacaClienteView> BuscaClientesPlaca(int IDPlaca, int IDUsuarioCliente);
    }
}
