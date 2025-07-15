using Raizen.UniCad.DAL.CodeFirst;
using Raizen.UniCad.DAL.Interfaces;
using Raizen.UniCad.Model;
using Raizen.UniCad.Model.View;
using System.Linq;

namespace Raizen.UniCad.DAL.Repositories
{
    public class PlacaClienteRepository : Repository<PlacaCliente>, IPlacaClienteRepository
    {
        public PlacaClienteRepository(UniCadContexto contexto) : base(contexto)
        {
        }

        public IQueryable<PlacaClienteView> BuscaClientesPlaca(int IDPlaca)
        {
            var clientes = from PlacaCliente in DbContext.Set<PlacaCliente>().AsNoTracking()
                           join cliente in DbContext.Set<Cliente>().AsNoTracking() on PlacaCliente.IDCliente equals cliente.ID
                           where PlacaCliente.IDPlaca == IDPlaca
                           select new PlacaClienteView
                           {
                               ID = PlacaCliente.ID,
                               IDCliente = PlacaCliente.IDCliente,
                               IDPlaca = PlacaCliente.IDPlaca,
                               RazaoSocial = cliente.IBM + " - " + cliente.CNPJCPF + " - " + cliente.RazaoSocial,
                               DataAprovacao = PlacaCliente.DataAprovacao
                           };

            return clientes;
        }

        public IQueryable<PlacaClienteView> BuscaClientesPlaca(int IDPlaca, int IDUsuarioCliente)
        {
            var clientes = from PlacaCliente in DbContext.Set<PlacaCliente>().AsNoTracking()
                           join cliente in DbContext.Set<Cliente>().AsNoTracking() on PlacaCliente.IDCliente equals cliente.ID
                           join UsuarioCliente in DbContext.Set<UsuarioCliente>() on PlacaCliente.IDCliente equals UsuarioCliente.IDCliente into j1
                           from uc in j1.DefaultIfEmpty()
                           where PlacaCliente.IDPlaca == IDPlaca && (IDUsuarioCliente == 0 || uc.IDUsuario == IDUsuarioCliente)
                           select new PlacaClienteView
                           {
                               ID = PlacaCliente.ID,
                               IDCliente = PlacaCliente.IDCliente,
                               Ibm = cliente.IBM,
                               IDPlaca = PlacaCliente.IDPlaca,
                               RazaoSocial = cliente.IBM + " - " + cliente.CNPJCPF + " - " + cliente.RazaoSocial,
                               DataAprovacao = PlacaCliente.DataAprovacao
                           };

            return clientes;
        }
    }
}
