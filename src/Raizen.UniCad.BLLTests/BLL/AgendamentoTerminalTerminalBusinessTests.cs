using Microsoft.VisualStudio.TestTools.UnitTesting;
using Raizen.UniCad.BLL;
using Raizen.UniCad.BLLTests.Bases;
using Raizen.UniCad.Model;
using Raizen.UniCad.Model.Filtro;
using System;

namespace Raizen.UniCad.BLLTests
{
    [TestClass()]
    public class AgendamentoTerminalHorarioBusinessTests : BaseTest
    {
        private readonly AgendamentoTerminalHorarioBusiness _AgendamentoTerminalHorarioBLL = new AgendamentoTerminalHorarioBusiness();
        private readonly AgendamentoTerminalBusiness _AgendamentoTerminalBLL = new AgendamentoTerminalBusiness();
        private readonly TerminalBusiness _TerminalBLL = new TerminalBusiness();
        private readonly TipoAgendaBusiness _TipoAgendaBLL = new TipoAgendaBusiness();
        //[TestMethod()]
        //[TestCategory("Agendamento")]
        //public void ListarAgendamentoTerminalHorario()
        //{
        //    var lista = _AgendamentoTerminalHorarioBLL.ListarAgendamentoTerminalHorario(new AgendamentoTerminalHorarioFiltro(), new Framework.Models.PaginadorModel());
        //    Assert.IsNotNull(lista);
        //}

        [TestMethod()]
        [TestCategory("Agendamento")]
        public void ListarAgendamentoTerminalHorarioCount()
        {
            var lista = _AgendamentoTerminalHorarioBLL.ListarAgendamentoTerminalHorarioCount(new AgendamentoTerminalHorarioFiltro());
            Assert.IsNotNull(lista);
        }

        [TestMethod()]
        [TestCategory("Agendamento")]
        public void ListarPorAgendamentoTerminal()
        {
            var lista = _AgendamentoTerminalHorarioBLL.ListarPorAgendamentoTerminal(1);
            Assert.IsNotNull(lista);
        }

        [TestMethod()]
        [TestCategory("Agendamento")]
        public void ListarAgendamentoTerminalHorarioPorTerminalPorTipoAgenda()
        {
            var lista = _AgendamentoTerminalHorarioBLL.ListarAgendamentoTerminalHorarioPorTerminalPorTipoAgenda(1, "CIF", 1, 1, DateTime.Now);
            Assert.IsNotNull(lista);
        }

        [TestMethod()]
        [TestCategory("Agendamento")]
        public void ListarAgendamentoTerminalHorarioPorTerminal()
        {
            var lista = _AgendamentoTerminalHorarioBLL.ListarAgendamentoTerminalHorarioPorTerminal(1, "CIF", 1, DateTime.Now);
            Assert.IsNotNull(lista);
        }

        [TestMethod()]
        [TestCategory("Agendamento")]
        public void SelecionarAgendamentoTerminalHorario()
        {
            var lista = _AgendamentoTerminalHorarioBLL.SelecionarAgendamentoTerminalHorario(1);
            Assert.IsNotNull(lista);
        }
    }
}