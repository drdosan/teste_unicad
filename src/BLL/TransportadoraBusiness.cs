
using System;
using System.Collections.Generic;
using System.Linq;
using Raizen.Framework.Log.Bases;
using Raizen.UniCad.DAL;
using Raizen.UniCad.Extensions;
using Raizen.UniCad.Model;
using Raizen.UniCad.Model.Filtro;
using Raizen.UniCad.Model.View;
using Raizen.UniCad.SAL;

namespace Raizen.UniCad.BLL
{
    public class TransportadoraBusiness : UniCadBusinessBase<Transportadora>
    {

        private readonly EnumPais _pais;

        #region Constructor

        public TransportadoraBusiness()
        {
            this._pais = EnumPais.Brasil;
        }

        public TransportadoraBusiness(EnumPais pais)
        {
            this._pais = pais;
        }
        #endregion

        public int Importar(DateTime? Data, EnumEmpresa origem)
        {
            return Importar(Data, origem, null);
        }
        public int Importar(DateTime? Data, EnumEmpresa origem, string cnpj)
        {
            return Importar(Data, origem, null, cnpj);
        }
        public int Importar(DateTime? Data, EnumEmpresa origem, List<String> ibms, string cnpj)
        {
            WsConsultaFornecedor transportador = new WsConsultaFornecedor();
            List<string> cnpjs = null;
            if (!string.IsNullOrEmpty(cnpj))
            {
                cnpjs = new List<string>();
                cnpjs.Add(cnpj);
            }
            var transportadoras = transportador.Importar(Data, origem, cnpjs);

            if (transportadoras != null && transportadoras.Any())
            {
                foreach (var c in transportadoras)
                {
                    var fornecedorSelecionado = Selecionar(w => w.IBM == c.IBM && w.IDEmpresa == c.IDEmpresa);
                    AtualizarIncluir(fornecedorSelecionado, c);
                }
            }
            return 0;
        }

        public List<ClienteTransportadoraView> ListarTransportadoras(TransportadoraFiltro filtro)
        {
            using (UniCadDalRepositorio<Transportadora> repositorio = new UniCadDalRepositorio<Transportadora>())
            {
                IQueryable<ClienteTransportadoraView> query = GetQuery(repositorio, filtro);
                return query.Distinct().ToList();
            }

        }

        public IEnumerable<ClienteTransportadoraView> ListarPorUsuario(int? idUsuario, string operacao, int linhaNegocio, int idPais)
        {
            using (UniCadDalRepositorio<Transportadora> repositorio = new UniCadDalRepositorio<Transportadora>())
            {
                IQueryable<ClienteTransportadoraView> query = GetQueryTransportadoraUsuario(idUsuario, operacao, linhaNegocio, repositorio, idPais);
                return query.ToList();
            }

        }

        public IEnumerable<ClienteTransportadoraView> ListarSemUsuario(string operacao, int linhaNegocio, int idPais)
        {
            using (UniCadDalRepositorio<Transportadora> repositorio = new UniCadDalRepositorio<Transportadora>())
            {
                IQueryable<ClienteTransportadoraView> query = GetQueryTransportadora(operacao, linhaNegocio, repositorio, idPais);
                return query.ToList();
            }

        }

        private IQueryable<ClienteTransportadoraView> GetQuery(UniCadDalRepositorio<Transportadora> repositorio, TransportadoraFiltro filtro)
        {
            var lista = from transportadora in repositorio.ListComplex<Transportadora>().AsNoTracking()
                        join usuarioTransportadora in repositorio.ListComplex<UsuarioTransportadora>().AsNoTracking() on transportadora.ID equals usuarioTransportadora.IDTransportadora
                        into j1
                        from j2 in j1.DefaultIfEmpty()
                        where (transportadora.RazaoSocial.Contains(filtro.Nome) 
                            || transportadora.IBM.Contains(filtro.Nome) 
                            || transportadora.CNPJCPF.Contains(filtro.Nome))
                        && (!filtro.IDEmpresa.HasValue || transportadora.IDEmpresa == filtro.IDEmpresa)
                        && (string.IsNullOrEmpty(filtro.Operacao) || transportadora.Operacao == filtro.Operacao)
                        && (transportadora.Desativado == filtro.Desativado)
                        && (transportadora.IdPais == (int)_pais)
                        orderby transportadora.RazaoSocial
                        select new ClienteTransportadoraView
                        {
                            ID = transportadora.ID,
                            IDClienteTransportadora = j2.ID,
                            RazaoSocial = transportadora.RazaoSocial,
                            IBM = transportadora.IBM,
                            CPF_CNPJ = transportadora.CNPJCPF
                        };
             
            return lista;
        }

        private IQueryable<ClienteTransportadoraView> GetQueryTransportadoraUsuario(int? idUsuario, string operacao, int linhaNegocio, UniCadDalRepositorio<Transportadora> repositorio, int idPais)
        {
            var lista = from transportadora in repositorio.ListComplex<Transportadora>().AsNoTracking()
                        join usuarioTransportadora in repositorio.ListComplex<UsuarioTransportadora>().AsNoTracking() on transportadora.ID equals usuarioTransportadora.IDTransportadora
                        into j1
                        from j2 in j1.DefaultIfEmpty()
                        where ((j2.IDUsuario == idUsuario) || (!idUsuario.HasValue))
                        && ((transportadora.Operacao == operacao) || (string.IsNullOrEmpty(operacao)))
                        && (transportadora.IDEmpresa == (linhaNegocio == (int)EnumEmpresa.Ambos ? transportadora.IDEmpresa : linhaNegocio))
                        && (transportadora.IdPais == idPais)
                        orderby transportadora.RazaoSocial
                        select new ClienteTransportadoraView
                        {
                            ID = transportadora.ID,
                            IDClienteTransportadora = j2.ID,
                            RazaoSocial = transportadora.RazaoSocial,
                            IBM = transportadora.IBM,
                            CPF_CNPJ = transportadora.CNPJCPF
                        };

            return lista;
        }

        private IQueryable<ClienteTransportadoraView> GetQueryTransportadora(string operacao, int linhaNegocio, UniCadDalRepositorio<Transportadora> repositorio, int idPais)
        {
            var lista = from transportadora in repositorio.ListComplex<Transportadora>().AsNoTracking()
                        where ((transportadora.Operacao == operacao) || (string.IsNullOrEmpty(operacao)))
                        && (transportadora.IDEmpresa == (linhaNegocio == (int)EnumEmpresa.Ambos ? transportadora.IDEmpresa : linhaNegocio))
                        && (transportadora.IdPais == idPais)
                        orderby transportadora.RazaoSocial
                        select new ClienteTransportadoraView
                        {
                            ID = transportadora.ID,
                            RazaoSocial = transportadora.RazaoSocial,
                            IBM = transportadora.IBM,
                            CPF_CNPJ = transportadora.CNPJCPF
                        };

            return lista; 
        }
        private IQueryable<string> GetQueryTransportadoraEmail(string cnpjcpf, UniCadDalRepositorio<Transportadora> repositorio)
        {
            var lista = from transportadora in repositorio.ListComplex<Transportadora>().AsNoTracking()
                        join usuarioTransportadora in repositorio.ListComplex<UsuarioTransportadora>().AsNoTracking() on transportadora.ID equals usuarioTransportadora.IDTransportadora
                        join usuario in repositorio.ListComplex<Usuario>().AsNoTracking() on usuarioTransportadora.IDUsuario equals usuario.ID
                        where (transportadora.CNPJCPF == cnpjcpf)
                        select usuario.Email;

            return lista;
        }

        private void AtualizarIncluir(Transportadora transSelecionado, Transportadora transp)
        {
            //ATUALIZAR
            if (transSelecionado != null)
            {
                transSelecionado.DtAtualizacao = DateTime.Now;
                transSelecionado.CNPJCPF = transp.CNPJCPF;
                transSelecionado.Operacao = transp.Operacao;
                transSelecionado.RazaoSocial = transp.RazaoSocial;
                transSelecionado.Desativado = transp.Desativado;
                Atualizar(transSelecionado);
            }
            else
            {
                transp.DtInclusao = transp.DtAtualizacao = DateTime.Now;
                Adicionar(transp);
            }
        }

        internal string SelecionarEmail(string cpfcnpj)
        {

            using (UniCadDalRepositorio<Transportadora> repositorio = new UniCadDalRepositorio<Transportadora>())
            {
                IQueryable<string> query = GetQueryTransportadoraEmail(cpfcnpj, repositorio);
                var resultado = query.FirstOrDefault();
                return resultado == null ? string.Empty : resultado;
            }

        }

        public Transportadora BuscarTranportadora(string cnpj, string frete, int idEmpresa)
        {
            Transportadora transp;
            cnpj = cnpj.RemoveCharacter();

            if (frete == "FOB" && idEmpresa == (int)EnumEmpresa.Combustiveis)
            {
                transp = new TransportadoraBusiness().Selecionar(p =>
                    p.CNPJCPF == cnpj &&
                    p.IDEmpresa == idEmpresa &&
                    p.Operacao == frete &&
                    !p.Desativado &&
                    (_pais != EnumPais.Brasil || p.IBM.StartsWith("T")));
            }
            else if (idEmpresa == (int)EnumEmpresa.EAB)
            {
                transp = new TransportadoraBusiness().Selecionar(p =>
                    p.CNPJCPF == cnpj &&
                    p.IDEmpresa == (int)EnumEmpresa.EAB &&
                    !p.Desativado);
            }
            else
            {
                transp = new TransportadoraBusiness().Selecionar(p =>
                    p.CNPJCPF == cnpj &&
                    p.IDEmpresa == idEmpresa &&
                    p.Operacao == frete &&
                    !p.Desativado);
            }

            return transp;
        }
    }
}

