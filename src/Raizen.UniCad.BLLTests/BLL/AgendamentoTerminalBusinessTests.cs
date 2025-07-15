using Microsoft.VisualStudio.TestTools.UnitTesting;
using Raizen.UniCad.BLL;
using Raizen.UniCad.BLLTests.Bases;
using Raizen.UniCad.Model;
using Raizen.UniCad.Model.Filtro;
using System;

namespace Raizen.UniCad.BLLTests
{
    [TestClass()]
    public class AgendamentoTerminalBusinessTests : BaseTest
    {
        private readonly AgendamentoTerminalHorarioBusiness _AgendamentoTerminalHorarioBLL = new AgendamentoTerminalHorarioBusiness();
        private readonly AgendamentoTerminalBusiness _AgendamentoTerminalBLL = new AgendamentoTerminalBusiness();
        private readonly TerminalBusiness _TerminalBLL = new TerminalBusiness();
        private readonly TipoAgendaBusiness _TipoAgendaBLL = new TipoAgendaBusiness();
        [TestMethod()]
        [TestCategory("Agendamento")]
        public void AdicionarNovoAgendamento()
        {
            Terminal terminal = new Terminal
            {
                Cidade = "Piracicaba",
                Endereco = "Rua dos Cajuzeiros, 801",
                IDEstado = new EstadoBusiness().Selecionar(w => w.Nome == "SP").ID,
                Nome = "TESTE",
                Sigla = "TS",
                isPool = false
            };
            _TerminalBLL.Adicionar(terminal);


            TipoAgenda tp = new TipoAgenda
            {
                Nome = "TipoAgenda",
                Liberado = true,
                IDTipo = (int)EnumTipoAgenda.Checklist,
                Status = true
            };
            _TipoAgendaBLL.Adicionar(tp);

            AgendamentoTerminal at = new AgendamentoTerminal
            {
                IDTerminal = terminal.ID,
                IDTipoAgenda = tp.ID,
                Data = DateTime.Now.Date,
                Ativo = true
            };

            AgendamentoTerminalHorario ath = new AgendamentoTerminalHorario
            {
                HoraInicio = new TimeSpan(9, 0, 0),
                HoraFim = new TimeSpan(10, 0, 0),
                IDEmpresa = 1,
                Operacao = "CIF",
                Vagas = 10
            };

            var retorno = _AgendamentoTerminalBLL.AdicionarHorario(at, ath);
            Assert.IsTrue(retorno);

            var rest = _AgendamentoTerminalBLL.GerarPdf(at.Data, at.IDTerminal, at.IDTipoAgenda, 10, 10);
            Assert.IsNotNull(rest);

            int count = _AgendamentoTerminalBLL.ListarAgendamentoTerminalCount(new AgendamentoTerminalFiltro());
            Assert.AreNotEqual(0, count);

            var jaExiste = _AgendamentoTerminalBLL.verificarSeJaExisteHorario(new AgendamentoTerminalFiltro
            {
                IdTipoAgenda = at.IDTipoAgenda,
                IdTerminal = at.IDTerminal,
                Data = at.Data,
                HoraInicio = ath.HoraInicio,
                HoraFim = ath.HoraFim,
                Operacao = ath.Operacao,
                IdEmpresa = ath.IDEmpresa,
                IdHorario = ath.idHoraAgenda

            });
            Assert.IsTrue(jaExiste);

            var id = _AgendamentoTerminalBLL.BuscarId(new AgendamentoTerminal
            {
                Ativo = true,
                Data = DateTime.Now.Date,
                IDTerminal = terminal.ID,
                IDTipoAgenda = tp.ID
            });
            Assert.AreEqual(at.ID, id);

            DateTime[] datas = new DateTime[1];
            var data = DateTime.Now.AddDays(1).Date;
            datas[0] = data;

            var retornoClone = _AgendamentoTerminalBLL.Clonar(at.ID, datas);
            Assert.AreEqual(string.Empty, retornoClone);

            var clone = _AgendamentoTerminalBLL.Selecionar(w => w.Ativo == true
                                                            && w.IDTerminal == at.IDTerminal
                                                            && w.Data == data
                                                            && w.IDTipoAgenda == at.IDTipoAgenda);

            var horarioClone = _AgendamentoTerminalHorarioBLL.Selecionar(w => w.IDAgendamentoTerminal == clone.ID);


            _AgendamentoTerminalHorarioBLL.Excluir(horarioClone);
            _AgendamentoTerminalBLL.ExcluirAgendamento(clone.ID);
            _AgendamentoTerminalBLL.ExcluirAgendamentoHorario(ath.ID);
            _AgendamentoTerminalBLL.Excluir(at);
            _TipoAgendaBLL.Excluir(tp);
            _TerminalBLL.Excluir(terminal);


        }

    }
}