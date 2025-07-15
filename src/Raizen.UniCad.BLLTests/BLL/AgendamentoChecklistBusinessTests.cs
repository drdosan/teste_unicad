using ClosedXML.Excel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Raizen.UniCad.BLL;
using Raizen.UniCad.BLLTests.Bases;
using Raizen.UniCad.BLLTests.Fakes;
using Raizen.UniCad.Model;
using Raizen.UniCad.Model.Filtro;
using Raizen.UniCad.Model.View;
using System;
using System.Collections.Generic;

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


        #region MontarColunasAgendamentoChecklist

        [TestMethod]
        public void MontarColunasAgendamentoChecklistTest()
        {
            /* Foi acordado junto ao time de arquitetura da Raizen o uso desta abordagem dos cenários abaixo,
               visto que o Jenkins da Raizen não aceita o uso do atributo [DataRows()] nos testes */
            #region Cenários

            var cenarios = new List<MontarColunasAgendamentoChecklistTest_Cenario>()
            {
                new MontarColunasAgendamentoChecklistTest_Cenario(
                    cenario: "Cenário 1",
                    pais: EnumPais.Brasil,
                    worksheet: new XLWorksheetFake()),

                new MontarColunasAgendamentoChecklistTest_Cenario(
                    cenario: "Cenário 2",
                    pais: EnumPais.Argentina,
                    worksheet: new XLWorksheetFake())
            };

            #endregion

            foreach (var c in cenarios)
            {
                //Arrange
                AgendamentoChecklistBusiness agendamentooBusiness = new AgendamentoChecklistBusiness();
                PrivateObject obj = new PrivateObject(agendamentooBusiness);

                //Act
                obj.Invoke("MontarColunasAgendamentoChecklist", c.Worksheet);

                //Assert 
                Assert.IsNotNull(c.Worksheet);
            }
        }

        private class MontarColunasAgendamentoChecklistTest_Cenario
        {
            public MontarColunasAgendamentoChecklistTest_Cenario(string cenario, EnumPais pais, IXLWorksheet worksheet)
            {
                Cenario = cenario;
                Pais = pais;
                Worksheet = worksheet;
            }

            public string Cenario { get; set; }
            public EnumPais Pais { get; set; }
            public IXLWorksheet Worksheet { get; set; }
        }

        #endregion

        #region MontarLinhasAgendamentoChecklist

        [TestMethod]
        public void MontarLinhasAgendamentoChecklistTest()
        {
            /* Foi acordado junto ao time de arquitetura da Raizen o uso desta abordagem dos cenários abaixo,
               visto que o Jenkins da Raizen não aceita o uso do atributo [DataRows()] nos testes */
            #region Cenários

            var cenarios = new List<MontarLinhasAgendamentoChecklistTest_Cenario>()
            {
                new MontarLinhasAgendamentoChecklistTest_Cenario(
                    cenario: "Cenário 1",
                    pais: EnumPais.Brasil,
                    worksheet: new XLWorksheetFake(),
                    linha: 1,
                    agendamento: new AgendamentoChecklistView()
                    {
                        Data = new DateTime(2019,01,01),
                        HoraInicio = new TimeSpan(10, 0, 0),
                        Empresa = "EAB",
                        Operacao = "CIF",
                        TipoComposicao = "1",
                        Placa1 = "PLA0001",
                        Placa2 = "PLA0002",
                        Placa3 = "PLA0003",
                        Placa4 = "PLA0004",
                        Terminal = "1"
                    }),

                new MontarLinhasAgendamentoChecklistTest_Cenario(
                    cenario: "Cenário 2",
                    pais: EnumPais.Argentina,
                    worksheet: new XLWorksheetFake(),
                    linha: 1,
                    agendamento: new AgendamentoChecklistView()
                    {
                        Data = new DateTime(2019,01,01),
                        HoraInicio = new TimeSpan(10, 0, 0),
                        Empresa = "EAB",
                        Operacao = "CIF",
                        TipoComposicao = "1",
                        Placa1 = "PLA0001",
                        Placa2 = "PLA0002",
                        Placa3 = "PLA0003",
                        Placa4 = "PLA0004",
                        Terminal = "1"
                    })
            };

            #endregion

            foreach (var c in cenarios)
            {
                //Arrange
                AgendamentoChecklistBusiness agendamentooBusiness = new AgendamentoChecklistBusiness();
                PrivateObject obj = new PrivateObject(agendamentooBusiness);

                //Act
                obj.Invoke("MontarLinhasAgendamentoChecklist", c.Worksheet, c.Linha, c.Agendamento);

                //Assert 
                Assert.IsNotNull(c.Worksheet);
            }
        }

        private class MontarLinhasAgendamentoChecklistTest_Cenario
        {
            public MontarLinhasAgendamentoChecklistTest_Cenario(string cenario, EnumPais pais, IXLWorksheet worksheet, int linha, AgendamentoChecklistView agendamento)
            {
                Cenario = cenario;
                Pais = pais;
                Worksheet = worksheet;
                Linha = linha;
                Agendamento = agendamento;
            }

            public string Cenario { get; set; }
            public EnumPais Pais { get; set; }
            public IXLWorksheet Worksheet { get; set; }
            public int Linha { get; set; }
            public AgendamentoChecklistView Agendamento { get; set; }
        }

        #endregion

    }
}