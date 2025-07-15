using System.Collections.Generic;
using System.Linq;
using Raizen.Framework.Models;
using Raizen.UniCad.DAL;
using Raizen.UniCad.Model;
using Raizen.UniCad.Model.Filtro;
using Raizen.UniCad.Model.View;
using System.Transactions;
using Raizen.Framework.Utils.Transacao;
using System;

namespace Raizen.UniCad.BLL
{
    public class TerminalBusiness : UniCadBusinessBase<Terminal>
    {
        private readonly TerminalEmpresaBusiness terminalEmpresaBLL = new TerminalEmpresaBusiness();
        public bool Adicionar(Terminal terminal)
        {
            using (TransactionScope ts = Transactions.CreateTransactionScope(1, IsolationLevel.ReadCommitted))
            {
                base.Adicionar(terminal);
                AdicionarClientes(terminal);
                ts.Complete();
                return true;
            }
        }

        private void AdicionarClientes(Terminal terminal)
        {
            if (terminal.isPool && terminal.TerminalEmpresa != null && terminal.TerminalEmpresa.Any())
            {
                terminal.TerminalEmpresa.ForEach(w =>
                {
                    w.IDTerminal = terminal.ID;
                    terminalEmpresaBLL.Adicionar(w);
                });
            }
        }

        public List<Terminal> ListarHojeDepoisDeHoje(string operacao, bool isChecklist)
        {
            var dataHoje = DateTime.Now.Date;
            var filtro = new TerminalFiltro { Data = dataHoje, Ativo = true, isCheckList = isChecklist, Operacao = operacao };

            var listaTerminais = ListarTerminalAgendamento(filtro);
            listaTerminais.ForEach(p => p.Nome = p.Sigla + " - " + p.Nome + (p.isPool ? " - POOL" : string.Empty));
            return listaTerminais;
        }

        private List<Terminal> ListarTerminalAgendamento(TerminalFiltro filtro)
        {
            using (UniCadDalRepositorio<Terminal> repositorio = new UniCadDalRepositorio<Terminal>())
            {
                IQueryable<Terminal> query = GetQueryTerminalAgendamento(filtro, repositorio);
                return query.ToList();
            }
        }
        public bool Atualizar(Terminal terminal)

        {
            using (TransactionScope ts = Transactions.CreateTransactionScope(1, IsolationLevel.ReadCommitted))
            {
                base.Atualizar(terminal);
                var empresas = terminalEmpresaBLL.Listar(w => w.IDTerminal == terminal.ID);
                if (empresas != null && empresas.Any())
                    empresas.ForEach(w => terminalEmpresaBLL.Excluir(w.ID));

                AdicionarClientes(terminal);
                ts.Complete();
                return true;
            }
        }

        public List<Terminal> ListarTerminal(TerminalFiltro filtro, PaginadorModel paginador)
        {


            using (UniCadDalRepositorio<Terminal> repositorio = new UniCadDalRepositorio<Terminal>())
            {
                IQueryable<Terminal> query = GetQueryTerminal(filtro, repositorio)
                                                        .Take(paginador.QtdeItensPagina * paginador.PaginaAtual)
                                                        .OrderBy(i => i.Nome)
                                                        .Skip(unchecked((int)paginador.InicioPaginacao));
                return query.ToList();
            }

        }

        public int ListarTerminalCount(TerminalFiltro filtro)
        {

            using (UniCadDalRepositorio<Terminal> repositorio = new UniCadDalRepositorio<Terminal>())
            {
                IQueryable<Terminal> query = GetQueryTerminal(filtro, repositorio);
                return query.Count();
            }

        }

        private IQueryable<Terminal> GetQueryTerminalAgendamento(TerminalFiltro filtro, IUniCadDalRepositorio<Terminal> repositorio)
        {
            IQueryable<Terminal> query =
                                (
                                    from app in repositorio.ListComplex<Terminal>().AsNoTracking().OrderBy(i => i.Nome)
                                    join ag in repositorio.ListComplex<AgendamentoTerminal>().AsNoTracking() on app.ID equals ag.IDTerminal
                                    join tp in repositorio.ListComplex<TipoAgenda>().AsNoTracking() on ag.IDTipoAgenda equals tp.ID
                                    where (ag.Data >= filtro.Data || !filtro.Data.HasValue)
                                    && (app.isPool == (filtro.Operacao == "CON" ? true : app.isPool))
                                    && (ag.Ativo == (filtro.Ativo.HasValue ? filtro.Ativo : ag.Ativo))
                                    && new[]
                                          {
                                              filtro.isCheckList ?
                                                  (int)EnumTipoAgenda.Checklist :
                                                  (int)EnumTipoAgenda.TreinamentoPratico,(int)EnumTipoAgenda.TreinamentoTeorico
                                          }.Contains(tp.IDTipo)
                                    select app
                                );
            return query;
        }

        private IQueryable<Terminal> GetQueryTerminal(TerminalFiltro filtro, IUniCadDalRepositorio<Terminal> repositorio)
        {
            IQueryable<Terminal> query = (from app in repositorio.ListComplex<Terminal>().AsNoTracking().OrderBy(i => i.Nome)
                                          where (app.Nome.Contains(string.IsNullOrEmpty(filtro.Nome) ? app.Nome : filtro.Nome))
                                          && (app.ID == filtro.ID || !filtro.ID.HasValue)
                                          select app);
            return query;
        }

        private IQueryable<TerminalTreinamentoView> GetQueryTerminalPorMotorista(int idMotorista, IUniCadDalRepositorio<Terminal> repositorio)
        {
            IQueryable<TerminalTreinamentoView> query = (from motorista in repositorio.ListComplex<Motorista>().AsNoTracking().OrderBy(i => i.Nome)
                                                         join motoristaTreinamentoTerminal in repositorio.ListComplex<MotoristaTreinamentoTerminal>().AsNoTracking() on motorista.ID equals motoristaTreinamentoTerminal.IDMotorista
                                                         join terminal in repositorio.ListComplex<Terminal>().AsNoTracking() on motoristaTreinamentoTerminal.IDTerminal equals terminal.ID
                                                         where (motorista.ID == idMotorista)
                                                         select new TerminalTreinamentoView
                                                         {
                                                             ID = motoristaTreinamentoTerminal.ID,
                                                             dataValidade = motoristaTreinamentoTerminal.DataValidade,
                                                             IDTerminal = terminal.ID,
                                                             Nome = terminal.Nome,
                                                             Anexo = motoristaTreinamentoTerminal.Anexo,
                                                             Usuario = motoristaTreinamentoTerminal.Usuario,
                                                             CodigoUsuario = motoristaTreinamentoTerminal.CodigoUsuario
                                                         });
            return query;
        }

        public List<TerminalTreinamentoView> ListarPorMotorista(int idMotorista)
        {

            using (UniCadDalRepositorio<Terminal> repositorio = new UniCadDalRepositorio<Terminal>())
            {
                IQueryable<TerminalTreinamentoView> query = GetQueryTerminalPorMotorista(idMotorista, repositorio);
                return query.ToList();
            }

        }
    }
}

