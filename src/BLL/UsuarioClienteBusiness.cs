
using System.Collections.Generic;
using System.Linq;
using Raizen.UniCad.DAL;
using Raizen.UniCad.Model;
using Raizen.UniCad.Model.View;

namespace Raizen.UniCad.BLL
{
    public class UsuarioClienteBusiness : UniCadBusinessBase<UsuarioCliente>
    {
        public List<UsuarioClienteView> ListarClientesPorUsuario(int IDUsuario, int? IDEmpresa = null)
        {
            using (UniCadDalRepositorio<UsuarioCliente> repositorio = new UniCadDalRepositorio<UsuarioCliente>())
            {
                var query = GetQuery(repositorio, IDUsuario, IDEmpresa);

                return query.ToList();
            }
        }

        public void IncluirLista(List<UsuarioCliente> lista)
        {
			using (UniCadDalRepositorio<UsuarioCliente> repositorio = new UniCadDalRepositorio<UsuarioCliente>())
			{
				repositorio.BulkInsert(lista, "UsuarioCliente");
			}
		}

        private IQueryable<UsuarioClienteView> GetQuery(UniCadDalRepositorio<UsuarioCliente> repositorio, int IDUsuario, int? IDEmpresa)
        {
            var clientes = from usuarioCliente in repositorio.ListComplex<UsuarioCliente>().AsNoTracking()
                           join cliente in repositorio.ListComplex<Cliente>().AsNoTracking() on usuarioCliente.IDCliente equals cliente.ID
                           where usuarioCliente.IDUsuario == IDUsuario
                           && (cliente.IDEmpresa == IDEmpresa.Value || !IDEmpresa.HasValue)
                           select new UsuarioClienteView
                           {
                               ID = usuarioCliente.ID,
                               IDCliente = usuarioCliente.IDCliente,
                               IDUsuario = usuarioCliente.IDUsuario,
                               CPF_CNPJ = cliente.CNPJCPF,
                               RazaoSocial = cliente.RazaoSocial,
                               IBM = cliente.IBM
                           };

            return clientes;


        }
    }
}

