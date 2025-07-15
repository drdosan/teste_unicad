using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Raizen.UniCad.BLL;
using Raizen.UniCad.BLLTests.Bases;
using Raizen.UniCad.Model;
using Raizen.UniCad.Model.View;

namespace Raizen.UniCad.BLLTests
{
    [TestClass()]
    public class MotoristaDocumentoBusinessTests : BaseTest
    {
        private readonly MotoristaDocumentoBusiness _pdBll = new MotoristaDocumentoBusiness();
        private readonly MotoristaBusiness _motoristaBll = new MotoristaBusiness();
        private readonly UsuarioBusiness _usuarioBll = new UsuarioBusiness();
        private readonly UsuarioTransportadoraBusiness _utBll = new UsuarioTransportadoraBusiness();
        private readonly TransportadoraBusiness _transpBll = new TransportadoraBusiness();
        private readonly TipoDocumentoBusiness _tipoDocumentoBll = new TipoDocumentoBusiness();

        private Mock<DAL.Interfaces.IMotoristaDocumentoRepository> motoristaDocumentoRepository;
        private Mock<BLL.Interfaces.IMotoristaBusiness> motoristaBusiness;
        private Mock<BLL.Interfaces.IConfigBusiness> configBusiness;

        private MotoristaDocumentoBusiness _mdBusiness;

        [TestInitialize]
        public void InicializarTeste()
        {
            motoristaDocumentoRepository = new Mock<DAL.Interfaces.IMotoristaDocumentoRepository>();
            motoristaBusiness = new Mock<BLL.Interfaces.IMotoristaBusiness>();
            configBusiness = new Mock<BLL.Interfaces.IConfigBusiness>();
            _mdBusiness = new MotoristaDocumentoBusiness(motoristaDocumentoRepository.Object, motoristaBusiness.Object, motoristaBusiness.Object, configBusiness.Object, EnumPais.Padrao);
        }

        #region ProcessarDocumentosVencidos

        [TestMethod()]
        public void ProcessarDocumentosVencidosTest_NoItens_NoError()
        {
            var documentos = new List<MotoristaDocumentoView>() { };

            motoristaDocumentoRepository.Setup(s => s.GetDocumentosBloqueados(It.IsAny<DateTime>())).Returns(documentos);

            var retorno = _mdBusiness.ProcessarDocumentosVencidos(DateTime.Now);

            Assert.IsTrue(retorno == 0);
        }

        [TestMethod()]
        public void ProcessarDocumentosVencidosTest_BloqueioImediato_Success()
        {
            var dataExecucao = DateTime.Now.Date;

            var documentos = new List<MotoristaDocumentoView>()
            {
                new MotoristaDocumentoView()
                {
                    ID = 1,
                    TipoAlerta = 2,
                    TipoBloqueioImediato = EnumTipoBloqueioImediato.Sim,
                    TipoAcaoVencimento = EnumTipoAcaoVencimento.Bloquear
                }
            };

            var motoristaDocumento = new MotoristaDocumento()
            {
                ID = 1,
                Alerta1Enviado = false
            };

            var composicao = new Composicao()
            {
                ID = 1,
            };



            motoristaDocumentoRepository.Setup(s => s.GetDocumentosBloqueados(dataExecucao)).Returns(documentos);
            motoristaDocumentoRepository.Setup(s => s.Selecionar(documentos[0].ID)).Returns(motoristaDocumento);
            configBusiness.Setup(s => s.GetConfigInt(EnumConfig.habilitarVectoMotorista, It.IsAny<int>())).Returns(-1);
            configBusiness.Setup(s => s.GetConfigInt(EnumConfig.habilitarBloqueioDocMotorista, It.IsAny<int>())).Returns(-1);

            var retorno = _mdBusiness.ProcessarDocumentosVencidos(dataExecucao);

            Assert.IsTrue(retorno == 0);
            Assert.IsTrue(documentos[0].TipoAlerta == 1);
            Assert.IsTrue(motoristaDocumento.Vencido);
            Assert.IsTrue(motoristaDocumento.Bloqueado);
            Assert.IsTrue(motoristaDocumento.Processado);
        }

        [TestMethod()]
        public void ProcessarDocumentosVencidosTest_NaoBloqueioImediato_Success()
        {
            var dataExecucao = DateTime.Now.Date;

            var documentos = new List<MotoristaDocumentoView>()
            {
                new MotoristaDocumentoView()
                {
                    ID = 1,
                    TipoAlerta = 2,
                    TipoBloqueioImediato = EnumTipoBloqueioImediato.Nao,
                    TipoAcaoVencimento = EnumTipoAcaoVencimento.Bloquear,
                    QuantidadeDiasBloqueio = 10,
                    DataVencimento = dataExecucao.AddDays(-15)
                }
            };

            var motoristaDocumento = new MotoristaDocumento()
            {
                ID = 1,
                Alerta1Enviado = false
            };

            motoristaDocumentoRepository.Setup(s => s.GetDocumentosBloqueados(dataExecucao)).Returns(documentos);
            motoristaDocumentoRepository.Setup(s => s.Selecionar(documentos[0].ID)).Returns(motoristaDocumento);
            configBusiness.Setup(s => s.GetConfigInt(EnumConfig.habilitarVectoMotorista, It.IsAny<int>())).Returns(-1);
            configBusiness.Setup(s => s.GetConfigInt(EnumConfig.habilitarBloqueioDocMotorista, It.IsAny<int>())).Returns(-1);

            var retorno = _mdBusiness.ProcessarDocumentosVencidos(dataExecucao);

            Assert.IsTrue(retorno == 0);
            Assert.IsTrue(documentos[0].TipoAlerta == 1);
            Assert.IsTrue(motoristaDocumento.Vencido);
            Assert.IsTrue(motoristaDocumento.Bloqueado);
            Assert.IsTrue(motoristaDocumento.Processado);
        }

        [TestMethod()]
        public void ProcessarDocumentosVencidosTest_BloqueioImediato_SemAcao_Success()
        {
            var documentos = new List<MotoristaDocumentoView>()
            {
                new MotoristaDocumentoView()
                {
                    ID = 1,
                    TipoAlerta = 2,
                    TipoBloqueioImediato = EnumTipoBloqueioImediato.Sim,
                    TipoAcaoVencimento = EnumTipoAcaoVencimento.SemAcao
                }
            };

            var motoristaDocumento = new MotoristaDocumento()
            {
                ID = 1,
                Alerta1Enviado = false
            };

            var data = DateTime.Now.Date;

            motoristaDocumentoRepository.Setup(s => s.GetDocumentosBloqueados(data)).Returns(documentos);
            motoristaDocumentoRepository.Setup(s => s.Selecionar(documentos[0].ID)).Returns(motoristaDocumento);
            configBusiness.Setup(s => s.GetConfigInt(EnumConfig.habilitarVectoMotorista, It.IsAny<int>())).Returns(-1);
            configBusiness.Setup(s => s.GetConfigInt(EnumConfig.habilitarBloqueioDocMotorista, It.IsAny<int>())).Returns(0);

            var retorno = _mdBusiness.ProcessarDocumentosVencidos(data);

            Assert.IsTrue(retorno == 0);
            Assert.IsTrue(documentos[0].TipoAlerta == 1);
            Assert.IsTrue(motoristaDocumento.Vencido);
            Assert.IsFalse(motoristaDocumento.Bloqueado);
            Assert.IsTrue(motoristaDocumento.Processado);
        }

        [TestMethod()]
        public void ProcessarDocumentosVencidosTest_Itens_Reprovar_Success()
        {
            var documentos = new List<MotoristaDocumentoView>()
            {
                new MotoristaDocumentoView()
                {
                    ID = 1,
                    TipoAlerta = 2,
                    TipoBloqueioImediato = EnumTipoBloqueioImediato.Sim,
                    TipoAcaoVencimento = EnumTipoAcaoVencimento.Reprovar
                }
            };

            var motoristaDocumento = new MotoristaDocumento()
            {
                ID = 1,
                Alerta1Enviado = false
            };

            var data = DateTime.Now.Date;

            motoristaDocumentoRepository.Setup(s => s.GetDocumentosBloqueados(data)).Returns(documentos);
            motoristaDocumentoRepository.Setup(s => s.Selecionar(documentos[0].ID)).Returns(motoristaDocumento);

            var retorno = _mdBusiness.ProcessarDocumentosVencidos(data);

            Assert.IsTrue(retorno == 0);
        }

        [TestMethod()]
        public void ProcessarDocumentosVencidosTest_Bloquear_Success()
        {
            var documentos = new List<MotoristaDocumentoView>()
            {
                new MotoristaDocumentoView()
                {
                    ID = 1,
                    IDMotorista = 1,
                    TipoAlerta = 1,
                    TipoBloqueioImediato = EnumTipoBloqueioImediato.Sim,
                    TipoAcaoVencimento = EnumTipoAcaoVencimento.Bloquear
                }
            };

            var motoristaDocumento = new MotoristaDocumento()
            {
                ID = 1,
                IDMotorista = 1,
                Alerta1Enviado = false
            };

            var motorista = new Motorista()
            {
                ID = 1,
                IdPais = EnumPais.Padrao
            };

            var data = DateTime.Now.Date;

            motoristaDocumentoRepository.Setup(s => s.GetDocumentosBloqueados(data)).Returns(documentos);
            motoristaDocumentoRepository.Setup(s => s.Selecionar(documentos[0].ID)).Returns(motoristaDocumento);
            configBusiness.Setup(s => s.GetConfigInt(EnumConfig.habilitarVectoMotorista, It.IsAny<int>())).Returns(-1);
            configBusiness.Setup(s => s.GetConfigInt(EnumConfig.habilitarBloqueioMotorista, It.IsAny<int>())).Returns(-1);
            configBusiness.Setup(s => s.GetConfig(EnumConfig.JustificativaBloqueioAutomatico, It.IsAny<int>())).Returns("teste");
            motoristaBusiness.Setup(s => s.Selecionar(motoristaDocumento.ID)).Returns(motorista);

            var retorno = _mdBusiness.ProcessarDocumentosVencidos(data);

            Assert.IsTrue(retorno == 0);
            Assert.IsTrue(documentos[0].TipoAlerta == 1);
            Assert.IsTrue(motoristaDocumento.Vencido);
            Assert.IsFalse(motoristaDocumento.Bloqueado);
            Assert.IsFalse(motoristaDocumento.Processado);
            Assert.IsTrue(motorista.Justificativa == "teste");
        }

        [TestMethod()]
        public void ProcessarDocumentosVencidosTest_ReprovarComposicao_Success()
        {
            var documentos = new List<MotoristaDocumentoView>()
            {
                new MotoristaDocumentoView()
                {
                    ID = 1,
                    IDMotorista = 1,
                    TipoAlerta = 1,
                    TipoBloqueioImediato = EnumTipoBloqueioImediato.Sim,
                    TipoAcaoVencimento = EnumTipoAcaoVencimento.Reprovar
                }
            };

            var motoristaDocumento = new MotoristaDocumento()
            {
                ID = 1,
                IDMotorista = 1,
                Alerta1Enviado = false
            };

            var motorista = new Motorista()
            {
                ID = 1,
                IdPais = EnumPais.Padrao
            };

            var data = DateTime.Now.Date;

            motoristaDocumentoRepository.Setup(s => s.GetDocumentosBloqueados(data)).Returns(documentos);
            motoristaDocumentoRepository.Setup(s => s.Selecionar(documentos[0].ID)).Returns(motoristaDocumento);
            configBusiness.Setup(s => s.GetConfigInt(EnumConfig.habilitarVectoMotorista, It.IsAny<int>())).Returns(-1);
            configBusiness.Setup(s => s.GetConfigInt(EnumConfig.habilitarReprovaDocMotoristaAutomatica, It.IsAny<int>())).Returns(-1);
            configBusiness.Setup(s => s.GetConfig(EnumConfig.JustificativaReprovaAutomatica, It.IsAny<int>())).Returns("teste");
            motoristaBusiness.Setup(s => s.Selecionar(motoristaDocumento.ID)).Returns(motorista);

            var retorno = _mdBusiness.ProcessarDocumentosVencidos(data);

            Assert.IsTrue(retorno == 0);
            Assert.IsTrue(documentos[0].TipoAlerta == 1);
            Assert.IsTrue(motoristaDocumento.Vencido);
            Assert.IsTrue(motoristaDocumento.Bloqueado);
            Assert.IsTrue(motoristaDocumento.Processado);
            Assert.IsTrue(motorista.Justificativa == "teste");
        }

        #endregion

        #region ProcessarAlertaDocumentosMotorista

        [TestMethod]
        public void ProcessarAlertaDocumentosMotorista_NoItens_NoError()
        {
            var documentos = new List<MotoristaDocumentoView>();

            var data = DateTime.Now.Date;

            motoristaDocumentoRepository.Setup(s => s.GetDocumentosAVencer(data)).Returns(documentos);

            var retorno = _mdBusiness.ProcessarAlertaDocumentosMotorista(data);

            Assert.IsTrue(retorno == 0);
        }

        [TestMethod]
        public void ProcessarAlertaDocumentosMotorista_NoEmail_NoError()
        {
            var documentos = new List<MotoristaDocumentoView>()
            {
                new MotoristaDocumentoView() { ID = 0, Operacao = "FOB", Email = "", IdPais = EnumPais.Padrao }
            };

            var data = DateTime.Now.Date;

            motoristaDocumentoRepository.Setup(s => s.GetDocumentosAVencer(data)).Returns(documentos);

            var retorno = _mdBusiness.ProcessarAlertaDocumentosMotorista(data);

            Assert.IsTrue(retorno == 0);
        }

        [TestMethod]
        public void ProcessarAlertaDocumentosMotorista_Itens_NoError()
        {
            var documentos = new List<MotoristaDocumentoView>()
            {
                new MotoristaDocumentoView() { ID = 1, Operacao = "FOB", Email = "email@teste.com.br", IdPais = EnumPais.Padrao }
            };

            var data = DateTime.Now.Date;

            motoristaDocumentoRepository.Setup(s => s.GetDocumentosAVencer(data)).Returns(documentos);

            var retorno = _mdBusiness.ProcessarAlertaDocumentosMotorista(data);

            Assert.IsTrue(retorno == 0);
        }

        [TestMethod]
        public void ProcessarAlertaDocumentosMotorista_Alerta1_Sent()
        {
            var data = DateTime.Now.Date;

            var documentos = new List<MotoristaDocumentoView>()
            {
                new MotoristaDocumentoView()
                {
                    ID = 1,
                    Operacao = "FOB",
                    Alerta1Enviado = false,
                    DiasVencimento = 10,
                    DiasVencimentoA2 = 15,
                    DataVencimento = data.AddDays(10),
                    Email = "email@teste.com.br",
                    IdPais = EnumPais.Padrao
                }
            };

            var motoristaDocumento = new MotoristaDocumento()
            {
                ID = 1,
                Alerta1Enviado = false
            };

            motoristaDocumentoRepository.Setup(s => s.GetDocumentosAVencer(data)).Returns(documentos);
            motoristaDocumentoRepository.Setup(s => s.Selecionar(documentos[0].ID)).Returns(motoristaDocumento);

            var retorno = _mdBusiness.ProcessarAlertaDocumentosMotorista(data);

            Assert.IsTrue(retorno == 0);
            Assert.IsTrue(motoristaDocumento.Alerta1Enviado);
            Assert.IsFalse(motoristaDocumento.Alerta2Enviado);
        }

        [TestMethod]
        public void ProcessarAlertaDocumentosMotorista_Alerta2_Sent()
        {
            var data = DateTime.Now.Date;

            var documentos = new List<MotoristaDocumentoView>()
            {
                new MotoristaDocumentoView()
                {
                    ID = 1,
                    Operacao = "FOB",
                    Alerta1Enviado = true,
                    Alerta2Enviado = false,
                    DiasVencimento = 10,
                    DiasVencimentoA2 = 15,
                    DataVencimento = data.AddDays(15),
                    Email = "email@teste.com.br",
                    IdPais = EnumPais.Padrao
                }
            };

            var motoristaDocumento = new MotoristaDocumento()
            {
                ID = 1,
                Alerta1Enviado = true,
                Alerta2Enviado = false
            };

            motoristaDocumentoRepository.Setup(s => s.GetDocumentosAVencer(data)).Returns(documentos);
            motoristaDocumentoRepository.Setup(s => s.Selecionar(documentos[0].ID)).Returns(motoristaDocumento);

            var retorno = _mdBusiness.ProcessarAlertaDocumentosMotorista(data);

            Assert.IsTrue(retorno == 0);
            Assert.IsTrue(motoristaDocumento.Alerta1Enviado);
            Assert.IsTrue(motoristaDocumento.Alerta2Enviado);
        }

        [TestMethod]
        public void ProcessarAlertaDocumentosMotorista_ValidateEmail_Success()
        {
            var documentos = new List<MotoristaDocumentoView>()
            {
                new MotoristaDocumentoView() { ID = 0, Operacao = "FOB", Email = "fob@email.com", EmailTransportadora = "cif@email.com", IdPais = EnumPais.Padrao },
                new MotoristaDocumentoView() { ID = 0, Operacao = "CIF", Email = "fob@email.com", EmailTransportadora = "cif@email.com", IdPais = EnumPais.Padrao }
            };

            Assert.IsTrue(documentos[0].EmailEnviar == "fob@email.com");
            Assert.IsTrue(documentos[1].EmailEnviar == "cif@email.com");
        }

        [TestMethod]
        public void ProcessarAlertaDocumentosMotorista_ErrorExpected()
        {
            var data = DateTime.Now.Date;

            var documentos = new List<MotoristaDocumentoView>()
            {
                new MotoristaDocumentoView()
                {
                    ID = 1,
                    Operacao = "FOB",
                    Alerta1Enviado = true,
                    Alerta2Enviado = false,
                    DiasVencimento = 10,
                    DiasVencimentoA2 = 15,
                    DataVencimento = data.AddDays(15),
                    Email = "email@teste.com.br",
                    IdPais = EnumPais.Padrao
                }
            };

            var motoristaDocumento = new PlacaDocumento()
            {
                ID = 1,
                Alerta1Enviado = true,
                Alerta2Enviado = false
            };

            motoristaDocumentoRepository.Setup(s => s.GetDocumentosAVencer(data)).Returns(documentos);
            motoristaDocumentoRepository.Setup(s => s.Selecionar(documentos[0].ID)).Throws(new Exception());

            var retorno = _mdBusiness.ProcessarAlertaDocumentosMotorista(data);

            Assert.IsTrue(retorno == 1);
        }

        #endregion


        [TestMethod]
        public void ListarMotoristaDocumentoTest()
        {
            var listaDocumentosMotorista = _pdBll.ListarMotoristaDocumento(1, "CIF");
            Assert.IsNotNull(listaDocumentosMotorista);
        }

        [IgnoreAttribute("Teste desligado até resolver a questão da ordem de execução no Jenkins")]
        [TestMethod()]
        public void ListarMotoristaDocumentoPorMotoristaTest()
        {
            var motoristaSelecionar = _motoristaBll.Listar().FirstOrDefault()?.ID;
            if (motoristaSelecionar != null)
                Assert.IsNotNull(_pdBll.ListarMotoristaDocumentoPorMotorista(motoristaSelecionar.Value));
        }


    }
}