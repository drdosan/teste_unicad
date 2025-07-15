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
    public class AgendamentoTreinamentoBusinessTests : BaseTest
    {
        private readonly AgendamentoTerminalHorarioBusiness _AgendamentoTerminalHorarioBLL = new AgendamentoTerminalHorarioBusiness();
        private readonly AgendamentoTreinamentoBusiness _AgendamentoTreinamentoBLL = new AgendamentoTreinamentoBusiness();
        private readonly AgendamentoTerminalBusiness _AgendamentoTerminalBLL = new AgendamentoTerminalBusiness();
        private readonly TerminalBusiness _TerminalBLL = new TerminalBusiness();
        private readonly TipoAgendaBusiness _TipoAgendaBLL = new TipoAgendaBusiness();
        
        [IgnoreAttribute("Teste desligado até resolver a questão da ordem de execução no Jenkins")]
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
                IDTipo = (int)EnumTipoAgenda.TreinamentoPratico,
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

            var moto = new MotoristaBusiness().Selecionar(p => p.Ativo);

            AgendamentoTreinamento atk = new AgendamentoTreinamento
            {
                Data = DateTime.Now,
                Usuario = "testUnitario",
                IDAgendamentoTerminalHorario = ath.ID,
                IDMotorista = moto.ID                 
            };

            var ret = _AgendamentoTreinamentoBLL.Adicionar(atk);
            Assert.IsTrue(ret);

            var arqu = _AgendamentoTreinamentoBLL.GerarPdf(atk.ID);
            Assert.IsNotNull(arqu);

            _AgendamentoTreinamentoBLL.ListarAgendamentoTreinamentoCount(new AgendamentoTreinamentoFiltro() { CPF = moto.MotoristaBrasil.CPF });
            _AgendamentoTreinamentoBLL.ListarAgendamentoTreinamento(new AgendamentoTreinamentoFiltro() { CPF = moto.MotoristaBrasil.CPF }, new Framework.Models.PaginadorModel() { });
            _AgendamentoTreinamentoBLL.ListarPorAgendamentoTerminal(at.ID);
            _AgendamentoTreinamentoBLL.SelecionarAgendamentoTreinamento(atk.ID);

            _AgendamentoTreinamentoBLL.Excluir(atk);
            _AgendamentoTerminalHorarioBLL.Excluir(ath);
            _AgendamentoTerminalBLL.Excluir(at);
            _TipoAgendaBLL.Excluir(tp);
            _TerminalBLL.Excluir(terminal);


        }
        [TestMethod()]
        [TestCategory("Agendamento")]
        public void Exportar()
        {
            var arqu = _AgendamentoTreinamentoBLL.Exportar(new AgendamentoTreinamentoFiltro() { IDTerminal = 1 });
            Assert.IsNotNull(arqu);
        }

        #region MontarLinhasAgendamentoTreinamento

        [TestMethod]
        public void MontarLinhasAgendamentoTreinamentoTest()
        {
            /* Foi acordado junto ao time de arquitetura da Raizen o uso desta abordagem dos cenários abaixo,
               visto que o Jenkins da Raizen não aceita o uso do atributo [DataRows()] nos testes */
            #region Cenários

            var cenarios = new List<MontarLinhasAgendamentoTreinamentoTest_Cenario>()
            {
                new MontarLinhasAgendamentoTreinamentoTest_Cenario(
                    cenario: "Cenário 1",
                    pais: EnumPais.Brasil,
                    linha: 1,
                    agendamento: new AgendamentoTreinamentoView()
                    {
                        Data = new DateTime(2019,01,01),
                        HoraInicio = new TimeSpan(10, 0, 0),
                        Empresa = "EAB",
                        Operacao = "CIF",
                        TipoTreinamento = "1",
                        Motorista = "Motorista",
                        CPF = "214.544.657-74",
                        Terminal = "1",
                        idSituacao = 1
                    },
                    worksheet: new XLWorksheetFake()),

                new MontarLinhasAgendamentoTreinamentoTest_Cenario(
                    cenario: "Cenário 2",
                    pais: EnumPais.Argentina,
                    linha: 1,
                    agendamento: new AgendamentoTreinamentoView()
                    {
                        Data = new DateTime(2019,01,01),
                        HoraInicio = new TimeSpan(10, 0, 0),
                        Empresa = "EAB",
                        Operacao = "CIF",
                        TipoTreinamento = "1",
                        Motorista = "Motorista",
                        CPF = "214.544.657-74",
                        Terminal = "1",
                        idSituacao = 1
                    },
                    worksheet: new XLWorksheetFake())
            };

            #endregion

            foreach (var c in cenarios)
            {
                //Arrange
                AgendamentoTreinamentoBusiness agendamentooBusiness = new AgendamentoTreinamentoBusiness();
                PrivateObject obj = new PrivateObject(agendamentooBusiness);

                //Act
                obj.Invoke("MontarLinhasAgendamentoTreinamento", c.Worksheet, c.Linha, c.Agendamento);

                //Assert 
                Assert.IsNotNull(c.Worksheet);
            }
        }

        private class MontarLinhasAgendamentoTreinamentoTest_Cenario
        {
            public MontarLinhasAgendamentoTreinamentoTest_Cenario(string cenario, EnumPais pais, int linha, AgendamentoTreinamentoView agendamento, IXLWorksheet worksheet)
            {
                Cenario = cenario;
                Pais = pais;
                Linha = linha;
                Agendamento = agendamento;
                Worksheet = worksheet;
            }

            public int Linha { get; set; }
            public string Cenario { get; set; }
            public EnumPais Pais { get; set; }
            public AgendamentoTreinamentoView Agendamento { get; set; }
            public IXLWorksheet Worksheet { get; set; }
        }

        #endregion
    }
}