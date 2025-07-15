using Microsoft.VisualStudio.TestTools.UnitTesting;
using Raizen.UniCad.BLL;
using Raizen.UniCad.BLLTests.Bases;
using Raizen.UniCad.Model;
using Raizen.UniCad.Model.Filtro;
using System;

namespace Raizen.UniCad.BLLTests
{
    [TestClass()]
    public class AgendamentoChecklistBusinessTests : BaseTest
    {
        private readonly AgendamentoTerminalHorarioBusiness _AgendamentoTerminalHorarioBLL = new AgendamentoTerminalHorarioBusiness();
        private readonly AgendamentoChecklistBusiness _AgendamentoChecklistBLL = new AgendamentoChecklistBusiness();
        private readonly AgendamentoTerminalBusiness _AgendamentoTerminalBLL = new AgendamentoTerminalBusiness();
        private readonly TerminalBusiness _TerminalBLL = new TerminalBusiness();
        private readonly TipoAgendaBusiness _TipoAgendaBLL = new TipoAgendaBusiness();
        //[TestMethod()]
        //[TestCategory("Agendamento")]
        //public void AdicionarNovoAgendamento()
        //{

        //    Terminal terminal = new Terminal
        //    {
        //        Cidade = "Piracicaba",
        //        Endereco = "Rua dos Cajuzeiros, 801",
        //        IDEstado = new EstadoBusiness().Selecionar(w => w.Nome == "SP").ID,
        //        Nome = "TESTE",
        //        Sigla = "TS"
        //    };
        //    _TerminalBLL.Adicionar(terminal);

        //    TipoAgenda tp = new TipoAgenda
        //    {
        //        Nome = "TipoAgenda",
        //        Liberado = true,
        //        IDTipo = (int)EnumTipoAgenda.Checklist,
        //        Status = true
        //    };
        //    _TipoAgendaBLL.Adicionar(tp);

        //    AgendamentoTerminal at = new AgendamentoTerminal
        //    {
        //        IDTerminal = terminal.ID,
        //        IDTipoAgenda = tp.ID,
        //        Data = DateTime.Now.Date,
        //        Ativo = true
        //    };

        //    AgendamentoTerminalHorario ath = new AgendamentoTerminalHorario
        //    {
        //        HoraInicio = new TimeSpan(9, 0, 0),
        //        HoraFim = new TimeSpan(10, 0, 0),
        //        IDEmpresa = 1,
        //        Operacao = "CIF",
        //        Vagas = 10
        //    };

        //    var retorno = _AgendamentoTerminalBLL.AdicionarHorario(at, ath);

        //    var comp = new ComposicaoBusiness().Selecionar(p => p.IDTipoComposicao == (int)EnumTipoComposicao.Truck);

        //    AgendamentoChecklist atk = new AgendamentoChecklist
        //    {
        //        Data = DateTime.Now,
        //        IDEmpresa = 1,
        //        Operacao = "CIF",
        //        Usuario = "testUnitario",
        //        IDAgendamentoTerminalHorario = ath.ID,
        //        IDComposicao = comp.ID,
        //        IDTerminal = terminal.ID
        //    };

        //    _AgendamentoChecklistBLL.Validar(atk);

        //    var ret = _AgendamentoChecklistBLL.Adicionar(atk);
        //    Assert.IsTrue(ret);

        //    var arqu = _AgendamentoChecklistBLL.GerarPdf(atk.ID);
        //    Assert.IsNotNull(arqu);

        //    _AgendamentoChecklistBLL.ListarAgendamentoChecklistCount(new AgendamentoChecklistFiltro() { IDComposicao = comp.ID });
        //    _AgendamentoChecklistBLL.ListarAgendamentoChecklist(new AgendamentoChecklistFiltro() { IDComposicao = comp.ID }, new Framework.Models.PaginadorModel() { });
        //    _AgendamentoChecklistBLL.ListarPorAgendamentoTerminal(at.ID);
        //    _AgendamentoChecklistBLL.SelecionarAgendamentoChecklist(atk.ID);
        //    _AgendamentoChecklistBLL.SelecionarAgendamentoChecklistPorComposicao(comp.ID);

        //    _AgendamentoChecklistBLL.Excluir(atk);
        //    _AgendamentoTerminalHorarioBLL.Excluir(ath);
        //    _AgendamentoTerminalBLL.Excluir(at);
        //    _TipoAgendaBLL.Excluir(tp);
        //    _TerminalBLL.Excluir(terminal);


        //}
        //[TestMethod()]
        //[TestCategory("Agendamento")]
        //public void Exportar()
        //{
        //    var arqu = _AgendamentoChecklistBLL.Exportar(new AgendamentoChecklistFiltro() { IDTerminal = 1 });
        //    Assert.IsNotNull(arqu);
        //}

    }
}