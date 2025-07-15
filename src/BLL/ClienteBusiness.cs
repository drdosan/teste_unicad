
using System;
using System.Collections.Generic;
using System.Linq;
using DocumentFormat.OpenXml.EMMA;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
using Raizen.Framework.Log.Bases;
using Raizen.UniCad.DAL;
using Raizen.UniCad.Model;
using Raizen.UniCad.Model.Filtro;
using Raizen.UniCad.Model.View;
using Raizen.UniCad.SAL;

namespace Raizen.UniCad.BLL
{
	public class ClienteBusiness : UniCadBusinessBase<Cliente>
	{
		private readonly EnumPais _pais;

		#region Constructor

		public ClienteBusiness()
		{
			this._pais = EnumPais.Brasil;
		}

		public ClienteBusiness(EnumPais pais)
		{
			this._pais = pais;
		}

		#endregion

		public int Importar(DateTime? Data, EnumEmpresa origem)
		{
			return Importar(Data, null, origem);
		}

		public int Importar(DateTime? Data, List<String> ibms, EnumEmpresa origem)
		{
			WsConsultaCliente cliente = new WsConsultaCliente();
			var clientes = cliente.Importar(Data, origem, ibms);

			if (clientes != null && clientes.Any())
			{
				foreach (var c in clientes)
				{
					var clienteSelecionado = Selecionar(w => w.IBM == c.IBM && w.IDEmpresa == c.IDEmpresa);
					AtualizarIncluir(clienteSelecionado, c);
				}
			}
			return 0;
		}

		public List<ClienteTransportadoraView> ListarClientes(ClienteFiltro filtro)
		{
			using (UniCadDalRepositorio<Cliente> repositorio = new UniCadDalRepositorio<Cliente>())
			{
				IQueryable<ClienteTransportadoraView> query = GetQuery(repositorio, filtro);
				return query.Distinct().ToList();
			}

		}

		private IQueryable<ClienteTransportadoraView> GetQuery(UniCadDalRepositorio<Cliente> repositorio, ClienteFiltro filtro)
		{
			var lista = from cliente in repositorio.ListComplex<Cliente>().AsNoTracking()
						join usuarioCliente in repositorio.ListComplex<UsuarioCliente>().AsNoTracking() on cliente.ID equals usuarioCliente.IDCliente
						into j1
						from j2 in j1.DefaultIfEmpty()
						where ((cliente.RazaoSocial.Contains(filtro.Nome) || string.IsNullOrEmpty(filtro.Nome))
							|| (cliente.IBM.Contains(filtro.Nome) || string.IsNullOrEmpty(filtro.Nome))
							|| (cliente.CNPJCPF.Contains(filtro.Nome) || string.IsNullOrEmpty(filtro.Nome)))
						&& ((j2.IDUsuario == filtro.ID) || (filtro.ID == 0))
						&& (cliente.IDEmpresa == (filtro.IDEmpresa.HasValue || (filtro.IDEmpresa.HasValue && filtro.IDEmpresa.Value != (int)EnumEmpresa.Ambos) ? filtro.IDEmpresa : cliente.IDEmpresa))
						&& (!cliente.Desativado)
						&& (cliente.IdPais == (int)_pais)
						select new ClienteTransportadoraView { ID = cliente.ID, IDClienteTransportadora = cliente.ID, CPF_CNPJ = cliente.CNPJCPF, RazaoSocial = cliente.RazaoSocial, IBM = cliente.IBM };

			return lista;
		}

		private IQueryable<string> GetQuerySelecionarEmailCliente(UniCadDalRepositorio<Cliente> repositorio, string cnpj)
		{
			var lista = from cliente in repositorio.ListComplex<Cliente>().AsNoTracking()
						join usuarioCliente in repositorio.ListComplex<UsuarioCliente>().AsNoTracking() on cliente.ID equals usuarioCliente.IDCliente
						join usuario in repositorio.ListComplex<Usuario>().AsNoTracking() on usuarioCliente.IDUsuario equals usuario.ID
						where (cliente.CNPJCPF == cnpj)
						select usuario.Email;

			return lista;
		}

		private void AtualizarIncluir(Cliente clienteSelecionado, Cliente cli)
		{

			//ATUALIZAR
			if (clienteSelecionado != null)
			{
				clienteSelecionado.CNPJCPF = cli.CNPJCPF;
				clienteSelecionado.RazaoSocial = cli.RazaoSocial;
				clienteSelecionado.DtAtualizacao = DateTime.Now;
				clienteSelecionado.Desativado = cli.Desativado;
				Atualizar(clienteSelecionado);
			}
			else
			{
				cli.DtInclusao = cli.DtAtualizacao = DateTime.Now;
				Adicionar(cli);
			}
		}

		internal string SelecionarEmail(string cpfcnpj)
		{

			using (UniCadDalRepositorio<Cliente> repositorio = new UniCadDalRepositorio<Cliente>())
			{
				IQueryable<string> query = GetQuerySelecionarEmailCliente(repositorio, cpfcnpj);
				var resultado = query.FirstOrDefault();
				return resultado ?? string.Empty;
			}

		}
	}
}

