using System.Collections.Generic;
using System.Linq;
using Raizen.UniCad.DAL;
using Raizen.UniCad.Model;
using Raizen.UniCad.Model.View;

namespace Raizen.UniCad.BLL
{
    public class MotoristaClienteBusiness : UniCadBusinessBase<MotoristaCliente>
    {

        public List<MotoristaClienteView> ListarMotoristaClientePorMotorista(int idMotorista, int idEmpresa, int? idUsuario = null, bool isExcetoClientesUsuario = false)
        {
            using (UniCadDalRepositorio<MotoristaCliente> repositorio = new UniCadDalRepositorio<MotoristaCliente>())
            {
                var query = GetQueryClientes(repositorio, idMotorista, idEmpresa, idUsuario, isExcetoClientesUsuario);

                return query.ToList();
            }
        }

        private IQueryable<MotoristaClienteView> GetQueryClientes(UniCadDalRepositorio<MotoristaCliente> repositorio, int idMotorista, int idEmpresa, int? idUsuario, bool isExcetoClientesUsuario)
        {
            IQueryable<MotoristaClienteView> motoristaDocumentos;
            if (isExcetoClientesUsuario)
            {
                motoristaDocumentos = from motoristaCliente in repositorio.ListComplex<MotoristaCliente>().AsNoTracking()
                                      join cliente in repositorio.ListComplex<Cliente>().AsNoTracking() on motoristaCliente.IDCliente equals cliente.ID
                                      join usuarioCliente in repositorio.ListComplex<UsuarioCliente>().AsNoTracking() on cliente.ID equals usuarioCliente.IDCliente into uc
                                      from uCliente in uc.DefaultIfEmpty()
                                      where (motoristaCliente.IDMotorista == idMotorista)
                                      && (cliente.IDEmpresa == idEmpresa)
                                      && (uCliente.IDUsuario != idUsuario)
                                      group motoristaCliente by
                                      new
                                      {
                                          motoristaCliente.IDCliente,
                                          cliente.RazaoSocial,
                                          cliente.IBM,
                                          cliente.CNPJCPF,
                                          motoristaCliente.DataAprovacao
                                      } into g
                                      select new MotoristaClienteView
                                      {
                                          IDCliente = g.Key.IDCliente,
                                          RazaoSocial = g.Key.RazaoSocial,
                                          IBM = g.Key.IBM,
                                          CNPJCPF = g.Key.CNPJCPF,
                                          DataAprovacao = g.Key.DataAprovacao
                                      };

            }
            else
            {
                motoristaDocumentos = from motoristaCliente in repositorio.ListComplex<MotoristaCliente>().AsNoTracking()
                                      join cliente in repositorio.ListComplex<Cliente>().AsNoTracking() on motoristaCliente.IDCliente equals cliente.ID
                                      join usuarioCliente in repositorio.ListComplex<UsuarioCliente>().AsNoTracking() on cliente.ID equals usuarioCliente.IDCliente into uc
                                      from uCliente in uc.DefaultIfEmpty()
                                      where (motoristaCliente.IDMotorista == idMotorista)
                                      && (cliente.IDEmpresa == idEmpresa)
                                      && (uCliente.IDUsuario == (idUsuario ?? uCliente.IDUsuario))
                                      group motoristaCliente by
                                      new
                                      {
                                          motoristaCliente.IDCliente,
                                          cliente.RazaoSocial,
                                          cliente.IBM,
                                          cliente.CNPJCPF,
                                          motoristaCliente.DataAprovacao
                                      } into g
                                      select new MotoristaClienteView
                                      {
                                          IDCliente = g.Key.IDCliente,
                                          RazaoSocial = g.Key.RazaoSocial,
                                          IBM = g.Key.IBM,
                                          CNPJCPF = g.Key.CNPJCPF,
                                          DataAprovacao = g.Key.DataAprovacao
                                      };
            }
            return motoristaDocumentos;
        }

        public List<MotoristaClienteView> ListarClientes(int idMotorista)
        {
            using (UniCadDalRepositorio<MotoristaCliente> repositorio = new UniCadDalRepositorio<MotoristaCliente>())
            {
                var clientes = from motoristaCliente in repositorio.ListComplex<MotoristaCliente>().AsNoTracking()
                               join cliente in repositorio.ListComplex<Cliente>().AsNoTracking() on motoristaCliente.IDCliente equals cliente.ID
                               where (motoristaCliente.IDMotorista == idMotorista)
                               select new MotoristaClienteView
                               {
                                   IDCliente = motoristaCliente.IDCliente,
                                   RazaoSocial = cliente.RazaoSocial,
                                   IBM = cliente.IBM,
                                   CNPJCPF = cliente.CNPJCPF
                               };

                return clientes.ToList();
            }
        }
    }
}
