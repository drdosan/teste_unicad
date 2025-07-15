using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Raizen.Framework.Models;
using Raizen.UniCad.BLLTests.Bases;
using Raizen.UniCad.BLL;
using Raizen.UniCad.Model;
using Raizen.UniCad.Model.Filtro;
using Raizen.UniCad.Model.View;

namespace Raizen.UniCad.BLLTests
{
    [TestClass]
    public class TerminalBusinessTests : BaseTest
    {
        private readonly TerminalBusiness _terminalBll = new TerminalBusiness();
        private readonly TransportadoraBusiness _transpBll = new TransportadoraBusiness();
        private readonly MotoristaTreinamentoTerminalBusiness _mttBll = new MotoristaTreinamentoTerminalBusiness();
        private readonly MotoristaBusiness _motoBll = new MotoristaBusiness();
        private readonly TipoDocumentoBusiness _tipoDocBll = new TipoDocumentoBusiness();
        private readonly MotoristaDocumentoBusiness _motoristaDocumentoBll = new MotoristaDocumentoBusiness();
        //[TestCategory("Terminal")]
        //[TestMethod]
        //public void ListarPorMotorista()
        //{
        //    documento
        //    var tipoDoc1 = FuncoesCompartilhadasTests.IncluirDocumento("EXAME", "EXAME", EnumCategoriaVeiculo.Motorista);
        //    _tipoDocBll.Adicionar(tipoDoc1);
        //    var docs =
        //        new List<MotoristaDocumentoView>
        //        {
        //            new MotoristaDocumentoView
        //            {
        //                IDTipoDocumento = tipoDoc1.ID,
        //                Sigla = tipoDoc1.Sigla,
        //                DataVencimento = DateTime.Now.AddDays(3)
        //            }
        //        };

        //    transportadora
        //    var transportadora = new Transportadora
        //    {
        //        CNPJCPF = "300400500",
        //        Desativado = false,
        //        DtAtualizacao = DateTime.Now,
        //        IDEmpresa = (int)EnumEmpresa.EAB,
        //        RazaoSocial = "Teste Automatizado",
        //        IBM = "0000301014",
        //        DtInclusao = DateTime.Now
        //    };

        //    _transpBll.Adicionar(transportadora);

        //    terminal
        //    var terminal = new Terminal { Nome = "TAA" };
        //    _terminalBll.Adicionar(terminal);

        //    motorista
        //    var motorista = FuncoesCompartilhadasTests.CriarMotorista("98123721897", "D", transportadora.ID, "65412811493", docs, "teste@teste.com.br",
        //        (int)EnumEmpresa.Combustiveis, "São Paulo", new DateTime(1987, 09, 03), "Teste Automátizado",
        //        "CIF", "SSP/SP", "DETRAN/SP", "2139812312-1", "1998121678");
        //    _motoBll.Adicionar(motorista);

        //    motorista treinamento terminal
        //    var mtt = new MotoristaTreinamentoTerminal
        //    {
        //        IDMotorista = motorista.ID,
        //        IDTerminal = terminal.ID,
        //        CodigoUsuario = "tr00001",
        //        DataValidade = DateTime.Now.AddYears(1)
        //    };

        //    _mttBll.Adicionar(mtt);

        //    var result = _terminalBll.ListarPorMotorista(motorista.ID);
        //    Assert.IsNotNull(result);

        //    var result2 = _terminalBll.ListarTerminal(new TerminalFiltro(), new PaginadorModel());
        //    Assert.IsNotNull(result2);

        //    var resultCount = _terminalBll.ListarTerminalCount(new TerminalFiltro());
        //    Assert.AreNotEqual(0, resultCount);


        //    _mttBll.Excluir(mtt.ID);
        //    _motoBll.ExcluirRegistro(motorista.ID, motorista.IDStatus);
        //    _tipoDocBll.Excluir(tipoDoc1);
        //    _terminalBll.Excluir(terminal);
        //    _transpBll.Excluir(transportadora.ID);
        //}
    }
}
