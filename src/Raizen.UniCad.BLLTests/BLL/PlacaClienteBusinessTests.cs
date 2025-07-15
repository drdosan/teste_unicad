using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Raizen.UniCad.BLL;
using Raizen.UniCad.BLL.Interfaces;
using Raizen.UniCad.BLLTests.Bases;
using Raizen.UniCad.DAL.Interfaces;
using Raizen.UniCad.Model;
using Raizen.UniCad.Model.View;
using Raizen.UniCad.SAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;

namespace Raizen.UniCad.BLLTests
{
    [TestClass()]
    public class PlacaClienteBusinessTests : BaseTest
    {
        private PlacaClienteBusiness business;
        private Mock<IComposicaoRepository> composicaoRepository;
        private Mock<IPlacaRepository> placaRepository;
        private Mock<IPlacaClienteRepository> placaClienteRepository;
        private Mock<IWsIntegraSAP> wsIntegraSap;

        [TestInitialize]
        public void InicializarTeste()
        {
            composicaoRepository = new Mock<IComposicaoRepository>();
            placaRepository = new Mock<IPlacaRepository>();
            placaClienteRepository = new Mock<IPlacaClienteRepository>();
            wsIntegraSap = new Mock<IWsIntegraSAP>();
            business = new PlacaClienteBusiness(placaRepository.Object, composicaoRepository.Object, placaClienteRepository.Object, wsIntegraSap.Object);
        }

        #region Mocks

        public void MockBuscaClientesPlaca()
        {
            var placaCliente = new PlacaClienteView
            {
                ID = 1,
                Ibm = "000150525",
                DataAprovacao = DateTime.Now,
                IDCliente = 1,
                IDPlaca = 1,
                RazaoSocial = "Cliente A",
                Colunas = 1
            };

            var listaPlacaCliente = new List<PlacaClienteView> { placaCliente };

            placaClienteRepository.Setup(s => s.BuscaClientesPlaca(It.IsAny<int>()))
                .Returns(listaPlacaCliente.AsQueryable());


            placaClienteRepository.Setup(s => s.BuscaClientesPlaca(It.IsAny<int>(), It.IsAny<int>()))
                .Returns(listaPlacaCliente.AsQueryable());
        }

        #endregion

        [TestMethod()]
        public void ListarClientesPorPlaca()
        {
            MockBuscaClientesPlaca();

            var retorno = business.ListarClientesPlaca(1);

            Assert.IsNotNull(retorno);
            Assert.IsTrue(retorno.Count > 0);
        }

        [TestMethod()]
        public void ListarClientesPorPlacaUsuario()
        {
            MockBuscaClientesPlaca();

            var retorno = business.ListarClientesPorPlaca(1, 1);

            Assert.IsNotNull(retorno);
            Assert.IsTrue(retorno.Count > 0);
        }

        #region ExcluirPlacaCliente

        [TestMethod]
        public void ExcluirPlacaCliente_Sucesso()
        {
            var composicao = new Composicao
            {
                ID = 1,
                IDEmpresa = (int)EnumEmpresa.Combustiveis,
                IDPlaca1 = 1
            };

            var placa = new Placa
            {
                ID = 1,
                IDPais = EnumPais.Brasil
            };

            MockBuscaClientesPlaca();

            composicaoRepository.Setup(s => s.Selecionar(It.IsAny<int>()))
                .Returns(composicao);

            placaRepository.Setup(s => s.Selecionar(It.IsAny<int>()))
                .Returns(placa);

            wsIntegraSap.Setup(s => s.ExcluirPlacaClienteSap(It.IsAny<Placa>())).Returns(string.Empty);

            placaClienteRepository.Setup(s => s.SelecionarLista(It.IsAny<Expression<Func<PlacaCliente, bool>>>()))
                .Returns(new List<PlacaCliente>().AsEnumerable());

            placaClienteRepository.Setup(s => s.ExcluirLista(It.IsAny<IEnumerable<PlacaCliente>>()));

            string retorno = business.ExcluirPlacaCliente(1, 1, new int[1]);

            Assert.AreEqual("Placa removida com Sucesso", retorno);
        }

        [TestMethod]
        public void ExcluirPlacaCliente_Argentina_Sucesso()
        {
            var composicao = new Composicao
            {
                ID = 1,
                IDEmpresa = (int)EnumEmpresa.Combustiveis,
                IDPlaca1 = 1
            };

            var placa = new Placa
            {
                ID = 1,
                IDPais = EnumPais.Argentina,
            };

            MockBuscaClientesPlaca();

            composicaoRepository.Setup(s => s.Selecionar(It.IsAny<int>()))
                .Returns(composicao);

            placaRepository.Setup(s => s.Selecionar(It.IsAny<int>()))
                .Returns(placa);

            wsIntegraSap.Setup(s => s.ExcluirPlacaClienteSap(It.IsAny<Placa>())).Returns(string.Empty);

            placaClienteRepository.Setup(s => s.SelecionarLista(It.IsAny<Expression<Func<PlacaCliente, bool>>>()))
                .Returns(new List<PlacaCliente>().AsEnumerable());

            placaClienteRepository.Setup(s => s.ExcluirLista(It.IsAny<IEnumerable<PlacaCliente>>()));

            string retorno = business.ExcluirPlacaCliente(1, 1, new int[1]);

            Assert.IsTrue(retorno.Contains("Placa eliminado"));
        }

        [TestMethod]
        public void ExcluirPlacaCliente_ErroSap()
        {
            var composicao = new Composicao
            {
                ID = 1,
                IDEmpresa = (int)EnumEmpresa.Combustiveis,
                IDPlaca1 = 1
            };

            var placa = new Placa
            {
                ID = 1,
                IDPais = EnumPais.Brasil
            };

            MockBuscaClientesPlaca();

            composicaoRepository.Setup(s => s.Selecionar(It.IsAny<int>()))
                .Returns(composicao);

            placaRepository.Setup(s => s.Selecionar(It.IsAny<int>()))
                .Returns(placa);

            wsIntegraSap.Setup(s => s.ExcluirPlacaClienteSap(It.IsAny<Placa>())).Returns("Problema no envio dos dados para o SAP");

            string retorno = business.ExcluirPlacaCliente(1, 1, new int[1]);

            Assert.AreEqual("Problema no envio dos dados para o SAP", retorno);
        }

        #endregion
    }
}