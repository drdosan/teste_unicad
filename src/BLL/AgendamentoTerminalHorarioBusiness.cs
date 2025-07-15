using System;
using System.Collections.Generic;
using System.Linq;
using Raizen.Framework.Log.Bases;
using Raizen.Framework.Models;
using Raizen.UniCad.DAL;
using Raizen.UniCad.Model;
using Raizen.UniCad.Model.Filtro;
using Raizen.UniCad.Model.View;
using System.Transactions;
using Raizen.Framework.Utils.Transacao;
using Raizen.Framework.Utils.Extensions;
using System.Data.Entity.SqlServer;
using System.Data.Entity;

namespace Raizen.UniCad.BLL
{
    public class AgendamentoTerminalHorarioBusiness : UniCadBusinessBase<AgendamentoTerminalHorario>
    {

        public List<AgendamentoTerminalHorarioView> ListarAgendamentoTerminalHorario(AgendamentoTerminalHorarioFiltro filtro, PaginadorModel paginador)
        {

            using (UniCadDalRepositorio<AgendamentoTerminalHorario> repositorio = new UniCadDalRepositorio<AgendamentoTerminalHorario>())
            {
                IQueryable<AgendamentoTerminalHorarioView> query = GetQueryAgendamentoTerminalHorario(filtro, repositorio)
                                                        .Take(paginador.QtdeItensPagina * paginador.PaginaAtual)
                                                        .OrderBy(i => i.HoraInicio)
                                                        .Skip(unchecked((int)paginador.InicioPaginacao));
                return query.ToList();
            }
        }

        public int ListarAgendamentoTerminalHorarioCount(AgendamentoTerminalHorarioFiltro filtro)
        {

            using (UniCadDalRepositorio<AgendamentoTerminalHorario> repositorio = new UniCadDalRepositorio<AgendamentoTerminalHorario>())
            {
                IQueryable<AgendamentoTerminalHorarioView> query = GetQueryAgendamentoTerminalHorario(filtro, repositorio);
                return query.Count();
            }

        }

        private IQueryable<AgendamentoTerminalHorarioView> GetQueryAgendamentoTerminalHorario(AgendamentoTerminalHorarioFiltro filtro, IUniCadDalRepositorio<AgendamentoTerminalHorario> repositorio)
        {
            IQueryable<AgendamentoTerminalHorarioView> query = (from app in repositorio.ListComplex<AgendamentoTerminalHorario>().AsNoTracking().OrderBy(i => i.HoraFim)
                                                                select new AgendamentoTerminalHorarioView
                                                                {
                                                                });
            return query;
        }

        public List<AgendamentoTerminalHorarioView> ListarPorAgendamentoTerminal(int ID)
        {

            using (UniCadDalRepositorio<AgendamentoTerminalHorario> repositorio = new UniCadDalRepositorio<AgendamentoTerminalHorario>())
            {
                IQueryable<AgendamentoTerminalHorarioView> query = GetQueryListarPorAgendamentoTerminal(ID, repositorio)
                                                        .OrderBy(i => i.HoraInicio);
                return query.ToList();
            }

        }

        private IQueryable<AgendamentoTerminalHorarioView> GetQueryListarPorAgendamentoTerminal(int idAgendamentoTerminal, IUniCadDalRepositorio<AgendamentoTerminalHorario> repositorio)
        {
            IQueryable<AgendamentoTerminalHorarioView> query = (from app in repositorio.ListComplex<AgendamentoTerminalHorario>().AsNoTracking().OrderBy(i => i.HoraFim)
                                                                join empresa in repositorio.ListComplex<Empresa>().AsNoTracking() on app.IDEmpresa equals empresa.ID
                                                                where app.IDAgendamentoTerminal == idAgendamentoTerminal
                                                                select new AgendamentoTerminalHorarioView
                                                                {
                                                                    ID = app.ID,
                                                                    HoraInicio = app.HoraInicio,
                                                                    HoraFim = app.HoraFim,
                                                                    IdLinhaNegocios = app.IDEmpresa.Value,
                                                                    LinhaNegocios = empresa.Nome,
                                                                    NumVagas = app.Vagas,
                                                                    Operacao = app.Operacao
                                                                });
            return query;
        }

        public List<AgendamentoTreinamentoView> ListarAgendamentoTerminalHorarioPorTerminalPorTipoAgenda(int iDEmpresa, string operacao, int iDTerminal, int IDTipoTreinamento, DateTime Data)
        {

            using (UniCadDalRepositorio<AgendamentoTerminalHorario> repositorio = new UniCadDalRepositorio<AgendamentoTerminalHorario>())
            {

                var checklists = (from chkls in repositorio.ListComplex<AgendamentoTreinamento>().AsNoTracking()
                                  join agendamentoTerminalHorario in repositorio.ListComplex<AgendamentoTerminalHorario>().AsNoTracking() on chkls.IDAgendamentoTerminalHorario equals agendamentoTerminalHorario.ID
                                  join agendamentoTerminal in repositorio.ListComplex<AgendamentoTerminal>().AsNoTracking() on agendamentoTerminalHorario.IDAgendamentoTerminal equals agendamentoTerminal.ID
                                  where agendamentoTerminal.IDTerminal == iDTerminal
                                    && (agendamentoTerminalHorario.IDEmpresa == iDEmpresa || agendamentoTerminalHorario.IDEmpresa == (int)EnumEmpresa.Ambos)
                                    && (agendamentoTerminalHorario.Operacao == operacao || agendamentoTerminalHorario.Operacao == "Ambos")
                                    && agendamentoTerminal.Data == Data
                                    && agendamentoTerminal.IDTipoAgenda == IDTipoTreinamento
                                  select chkls);
                IQueryable<AgendamentoTreinamentoView> query = (from app in repositorio.ListComplex<AgendamentoTerminalHorario>().AsNoTracking().OrderBy(i => i.HoraFim)
                                                                join empresa in repositorio.ListComplex<Empresa>().AsNoTracking() on app.IDEmpresa equals empresa.ID
                                                                join agendamentoTerminal in repositorio.ListComplex<AgendamentoTerminal>().AsNoTracking() on app.IDAgendamentoTerminal equals agendamentoTerminal.ID
                                                                join tipoAgenda in repositorio.ListComplex<TipoAgenda>().AsNoTracking() on agendamentoTerminal.IDTipoAgenda equals tipoAgenda.ID
                                                                join terminal in repositorio.ListComplex<Terminal>().AsNoTracking() on agendamentoTerminal.IDTerminal equals terminal.ID
                                                                let vagas = app.Vagas - (from vagas in checklists
                                                                                         where vagas.IDAgendamentoTerminalHorario == app.ID
                                                                                         select vagas.ID).Count()
                                                                where terminal.ID == iDTerminal
                                                                && (app.IDEmpresa == iDEmpresa || app.IDEmpresa == (int)EnumEmpresa.Ambos)
                                                                && (app.Operacao == operacao || app.Operacao == "Ambos")
                                                                && agendamentoTerminal.Data == Data
                                                                && agendamentoTerminal.IDTipoAgenda == IDTipoTreinamento
                                                                && vagas > 0
                                                                select new AgendamentoTreinamentoView
                                                                {
                                                                    ID = app.ID,
                                                                    HoraInicio = app.HoraInicio,
                                                                    HoraFim = app.HoraFim,
                                                                    IDEmpresa = empresa.ID,
                                                                    LinhaNegocios = empresa.Nome,
                                                                    NumVagas = vagas,
                                                                    Operacao = app.Operacao,
                                                                    Cidade = terminal.Cidade,
                                                                    Endereco = terminal.Endereco,
                                                                    Anexo = tipoAgenda.Anexo,
                                                                    IDTerminal = agendamentoTerminal.IDTerminal,
                                                                    IDTipoTreinamento = tipoAgenda.IDTipo,
                                                                    Data = agendamentoTerminal.Data,
                                                                    TipoTreinamento = tipoAgenda.Nome
                                                                });
                return query.ToList();
            }

        }

        public List<AgendamentoTerminalHorarioView> ListarAgendamentoTerminalHorarioPorTerminal(int iDEmpresa, string operacao, int iDTerminal, DateTime Data)
        {

            using (UniCadDalRepositorio<AgendamentoTerminalHorario> repositorio = new UniCadDalRepositorio<AgendamentoTerminalHorario>())
            {
                var timeOfDay = DateTime.Now.TimeOfDay;
                var today = DateTime.Now.Date;
                var checklists = (from chkls in repositorio.ListComplex<AgendamentoChecklist>().AsNoTracking()
                                  join agendamentoTerminalHorario in repositorio.ListComplex<AgendamentoTerminalHorario>().AsNoTracking() on chkls.IDAgendamentoTerminalHorario equals agendamentoTerminalHorario.ID
                                  join agendamentoTerminal in repositorio.ListComplex<AgendamentoTerminal>().AsNoTracking() on agendamentoTerminalHorario.IDAgendamentoTerminal equals agendamentoTerminal.ID
                                  join tipoAgenda in repositorio.ListComplex<TipoAgenda>().AsNoTracking() on agendamentoTerminal.IDTipoAgenda equals tipoAgenda.ID
                                  where agendamentoTerminal.IDTerminal == iDTerminal
                                    && (iDEmpresa == (int)EnumEmpresa.Ambos || (agendamentoTerminalHorario.IDEmpresa == iDEmpresa || agendamentoTerminalHorario.IDEmpresa == (int)EnumEmpresa.Ambos))
                                    && (operacao == "Ambos" || (agendamentoTerminalHorario.Operacao == operacao || agendamentoTerminalHorario.Operacao == "Ambos"))
                                    && agendamentoTerminal.Data == Data
                                    && tipoAgenda.IDTipo == (int)EnumTipoAgenda.Checklist
                                  select chkls);
                IQueryable<AgendamentoTerminalHorarioView> query = (from app in repositorio.ListComplex<AgendamentoTerminalHorario>().AsNoTracking().OrderBy(i => i.HoraFim)
                                                                    join empresa in repositorio.ListComplex<Empresa>().AsNoTracking() on app.IDEmpresa equals empresa.ID
                                                                    join agendamentoTerminal in repositorio.ListComplex<AgendamentoTerminal>().AsNoTracking() on app.IDAgendamentoTerminal equals agendamentoTerminal.ID
                                                                    join tipoAgenda in repositorio.ListComplex<TipoAgenda>().AsNoTracking() on agendamentoTerminal.IDTipoAgenda equals tipoAgenda.ID
                                                                    join terminal in repositorio.ListComplex<Terminal>().AsNoTracking() on agendamentoTerminal.IDTerminal equals terminal.ID
                                                                    let vagas = app.Vagas - (from vagas in checklists
                                                                                             where vagas.IDAgendamentoTerminalHorario == app.ID
                                                                                             select vagas.ID).Count()
                                                                    where terminal.ID == iDTerminal
                                                                    && (iDEmpresa == (int)EnumEmpresa.Ambos || (app.IDEmpresa == iDEmpresa || app.IDEmpresa == (int)EnumEmpresa.Ambos))
                                                                    && (operacao == "Ambos" || (app.Operacao == operacao || app.Operacao == "Ambos"))
                                                                    && agendamentoTerminal.Data == Data
                                                                    && tipoAgenda.IDTipo == (int)EnumTipoAgenda.Checklist
                                                                    && vagas > 0
                                                                    && ((DbFunctions.TruncateTime(agendamentoTerminal.Data) == today && DbFunctions.DiffMinutes(app.HoraInicio, timeOfDay) < 0)
                                                                        || DbFunctions.TruncateTime(agendamentoTerminal.Data) > today)
                                                                    select new AgendamentoTerminalHorarioView
                                                                    {
                                                                        ID = app.ID,
                                                                        HoraInicio = app.HoraInicio,
                                                                        HoraFim = app.HoraFim,
                                                                        LinhaNegocios = empresa.Nome,
                                                                        NumVagas = vagas,
                                                                        Operacao = app.Operacao,
                                                                        Cidade = terminal.Cidade,
                                                                        Endereco = terminal.Endereco,
                                                                        Anexo = tipoAgenda.Anexo,
                                                                        Data = agendamentoTerminal.Data
                                                                    });
                return query.ToList();
            }

        }

        public List<AgendamentoTerminalHorarioView> SelecionarAgendamentoTerminalHorario(int id)
        {

            using (UniCadDalRepositorio<AgendamentoTerminalHorario> repositorio = new UniCadDalRepositorio<AgendamentoTerminalHorario>())
            {
                IQueryable<AgendamentoTerminalHorarioView> query = (from app in repositorio.ListComplex<AgendamentoTerminalHorario>().AsNoTracking().OrderBy(i => i.HoraFim)
                                                                    join empresa in repositorio.ListComplex<Empresa>().AsNoTracking() on app.IDEmpresa equals empresa.ID
                                                                    join agendamentoTerminal in repositorio.ListComplex<AgendamentoTerminal>().AsNoTracking() on app.IDAgendamentoTerminal equals agendamentoTerminal.ID
                                                                    join tipoAgenda in repositorio.ListComplex<TipoAgenda>().AsNoTracking() on agendamentoTerminal.IDTipoAgenda equals tipoAgenda.ID
                                                                    join terminal in repositorio.ListComplex<Terminal>().AsNoTracking() on agendamentoTerminal.IDTerminal equals terminal.ID
                                                                    where app.ID == id
                                                                    select new AgendamentoTerminalHorarioView
                                                                    {
                                                                        ID = app.ID,
                                                                        HoraInicio = app.HoraInicio,
                                                                        HoraFim = app.HoraFim,
                                                                        LinhaNegocios = empresa.Nome,
                                                                        NumVagas = app.Vagas,
                                                                        Operacao = app.Operacao,
                                                                        Cidade = terminal.Cidade,
                                                                        Endereco = terminal.Endereco,
                                                                        Anexo = tipoAgenda.Anexo,
                                                                        IDTerminal = agendamentoTerminal.IDTerminal,
                                                                        IDEmpresa = empresa.ID,
                                                                        Data = agendamentoTerminal.Data,
                                                                        IDTipoAgendamento = tipoAgenda.IDTipo,
                                                                        TipoTreinamento = tipoAgenda.Nome
                                                                    });
                return query.ToList();
            }

        }
    }
}

