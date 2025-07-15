using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Raizen.UniCad.BLL;
using Raizen.UniCad.BLL.Interfaces;
using Raizen.UniCad.BLLTests.Bases;
using Raizen.UniCad.Model;
using Raizen.UniCad.Model.View;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Raizen.UniCad.BLLTests
{
    [TestClass()]
    public class PlacaDocumentoBusinessTests : BaseTest
    {
        readonly PlacaDocumentoBusiness _pdBll = new PlacaDocumentoBusiness();
        readonly PlacaBusiness _placaBll = new PlacaBusiness();
        readonly UsuarioBusiness _usuarioBll = new UsuarioBusiness();
        readonly ClienteBusiness _clienteBll = new ClienteBusiness();
        readonly PlacaClienteBusiness _pcBll = new PlacaClienteBusiness();
        readonly UsuarioClienteBusiness _ucBll = new UsuarioClienteBusiness();


        private Mock<DAL.Interfaces.IPlacaDocumentoRepository> placaDocumentoRepository;
        private Mock<DAL.Interfaces.IRepository<Composicao>> composicaoRepository;
        private Mock<IComposicaoBusiness> composicaoBusiness;
        private Mock<IConfigBusiness> configBusiness;
        private Mock<IPlacaBusiness> placaBusiness;
        private PlacaDocumentoBusiness _pdBusiness;

        [TestInitialize]
        public void InicializarTeste()
        {
            placaDocumentoRepository = new Mock<DAL.Interfaces.IPlacaDocumentoRepository>();
            composicaoRepository = new Mock<DAL.Interfaces.IRepository<Composicao>>();
            composicaoBusiness = new Mock<IComposicaoBusiness>();
            configBusiness = new Mock<IConfigBusiness>();
            placaBusiness = new Mock<IPlacaBusiness>();
            _pdBusiness = new PlacaDocumentoBusiness(placaDocumentoRepository.Object, composicaoBusiness.Object, composicaoBusiness.Object, configBusiness.Object, placaBusiness.Object, EnumPais.Padrao);
        }

        #region ProcessarDocumentosVencidos

        [TestMethod()]
        public void ProcessarDocumentosVencidosTest_NoItens_NoError()
        {
            var documentos = new List<PlacaDocumentoView>() { };

            placaDocumentoRepository.Setup(s => s.GetDocumentosBloqueados(It.IsAny<DateTime>())).Returns(documentos);

            var retorno = _pdBusiness.ProcessarDocumentosVencidos(DateTime.Now);

            Assert.IsTrue(retorno == 0);
        }

        [TestMethod()]
        public void ProcessarDocumentosVencidosTest_BloqueioImediato_Success()
        {
            var dataExecucao = DateTime.Now.Date;

            var documentos = new List<PlacaDocumentoView>()
            {
                new PlacaDocumentoView()
                {
                    ID = 1,
                    TipoAlerta = 2,
                    TipoBloqueioImediato = EnumTipoBloqueioImediato.Sim,
                    TipoAcaoVencimento = EnumTipoAcaoVencimento.Bloquear
                }
            };

            var placaDocumento = new PlacaDocumento()
            {
                ID = 1,
                Alerta1Enviado = false
            };

            var composicao = new Composicao()
            {
                ID = 1,
            };



            placaDocumentoRepository.Setup(s => s.GetDocumentosBloqueados(dataExecucao)).Returns(documentos);
            composicaoBusiness.Setup(s => s.Selecionar(documentos[0].ID)).Returns(composicao);
            composicaoBusiness.Setup(s => s.AtualizarComposicao(composicao, false, false, false, 0)).Returns(true);
            placaDocumentoRepository.Setup(s => s.Selecionar(documentos[0].ID)).Returns(placaDocumento);
            configBusiness.Setup(s => s.GetConfigInt(EnumConfig.habilitarVectoPlaca, It.IsAny<int>())).Returns(-1);
            configBusiness.Setup(s => s.GetConfigInt(EnumConfig.habilitarBloqueioDocPlaca, It.IsAny<int>())).Returns(-1);

            var retorno = _pdBusiness.ProcessarDocumentosVencidos(dataExecucao);

            Assert.IsTrue(retorno == 0);
            Assert.IsTrue(documentos[0].TipoAlerta == 1);
            Assert.IsTrue(placaDocumento.Vencido);
            Assert.IsTrue(placaDocumento.Bloqueado);
            Assert.IsTrue(placaDocumento.Processado);
        }

        [TestMethod()]
        public void ProcessarDocumentosVencidosTest_NaoBloqueioImediato_Success()
        {
            var dataExecucao = DateTime.Now.Date;

            var documentos = new List<PlacaDocumentoView>()
            {
                new PlacaDocumentoView()
                {
                    ID = 1,
                    TipoAlerta = 2,
                    TipoBloqueioImediato = EnumTipoBloqueioImediato.Nao,
                    TipoAcaoVencimento = EnumTipoAcaoVencimento.Bloquear,
                    QuantidadeDiasBloqueio = 10,
                    DataVencimento = dataExecucao.AddDays(-15)
                }
            };

            var placaDocumento = new PlacaDocumento()
            {
                ID = 1,
                Alerta1Enviado = false
            };

            var composicao = new Composicao()
            {
                ID = 1,
            };



            placaDocumentoRepository.Setup(s => s.GetDocumentosBloqueados(dataExecucao)).Returns(documentos);
            composicaoBusiness.Setup(s => s.Selecionar(documentos[0].ID)).Returns(composicao);
            composicaoBusiness.Setup(s => s.AtualizarComposicao(composicao, false, false, false, 0)).Returns(true);
            placaDocumentoRepository.Setup(s => s.Selecionar(documentos[0].ID)).Returns(placaDocumento);
            configBusiness.Setup(s => s.GetConfigInt(EnumConfig.habilitarVectoPlaca, It.IsAny<int>())).Returns(-1);
            configBusiness.Setup(s => s.GetConfigInt(EnumConfig.habilitarBloqueioDocPlaca, It.IsAny<int>())).Returns(-1);

            var retorno = _pdBusiness.ProcessarDocumentosVencidos(dataExecucao);

            Assert.IsTrue(retorno == 0);
            Assert.IsTrue(documentos[0].TipoAlerta == 1);
            Assert.IsTrue(placaDocumento.Vencido);
            Assert.IsTrue(placaDocumento.Bloqueado);
            Assert.IsTrue(placaDocumento.Processado);
        }

        [TestMethod()]
        public void ProcessarDocumentosVencidosTest_BloqueioImediato_SemAcao_Success()
        {
            var documentos = new List<PlacaDocumentoView>()
            {
                new PlacaDocumentoView()
                {
                    ID = 1,
                    TipoAlerta = 2,
                    TipoBloqueioImediato = EnumTipoBloqueioImediato.Sim,
                    TipoAcaoVencimento = EnumTipoAcaoVencimento.SemAcao
                }
            };

            var placaDocumento = new PlacaDocumento()
            {
                ID = 1,
                Alerta1Enviado = false
            };

            var composicao = new Composicao()
            {
                ID = 1,
            };

            var data = DateTime.Now.Date;

            placaDocumentoRepository.Setup(s => s.GetDocumentosBloqueados(data)).Returns(documentos);
            composicaoBusiness.Setup(s => s.Selecionar(documentos[0].ID)).Returns(composicao);
            composicaoBusiness.Setup(s => s.AtualizarComposicao(composicao, false, false, false, 0)).Returns(true);
            placaDocumentoRepository.Setup(s => s.Selecionar(documentos[0].ID)).Returns(placaDocumento);
            configBusiness.Setup(s => s.GetConfigInt(EnumConfig.habilitarVectoPlaca, It.IsAny<int>())).Returns(-1);
            configBusiness.Setup(s => s.GetConfigInt(EnumConfig.habilitarBloqueioDocPlaca, It.IsAny<int>())).Returns(0);

            var retorno = _pdBusiness.ProcessarDocumentosVencidos(data);

            Assert.IsTrue(retorno == 0);
            Assert.IsTrue(documentos[0].TipoAlerta == 1);
            Assert.IsTrue(placaDocumento.Vencido);
            Assert.IsFalse(placaDocumento.Bloqueado);
            Assert.IsTrue(placaDocumento.Processado);
        }

        [TestMethod()]
        public void ProcessarDocumentosVencidosTest_Itens_Reprovar_Success()
        {
            var documentos = new List<PlacaDocumentoView>()
            {
                new PlacaDocumentoView()
                {
                    ID = 1,
                    TipoAlerta = 2,
                    TipoBloqueioImediato = EnumTipoBloqueioImediato.Sim,
                    TipoAcaoVencimento = EnumTipoAcaoVencimento.Reprovar
                }
            };

            var placaDocumento = new PlacaDocumento()
            {
                ID = 1,
                Alerta1Enviado = false
            };

            var composicao = new Composicao()
            {

            };

            var data = DateTime.Now.Date;

            placaDocumentoRepository.Setup(s => s.GetDocumentosBloqueados(data)).Returns(documentos);
            composicaoBusiness.Setup(s => s.Selecionar(documentos[0].ID)).Returns(composicao);
            composicaoBusiness.Setup(s => s.AtualizarComposicao(composicao, false, false, false, 0)).Returns(true);
            placaDocumentoRepository.Setup(s => s.Selecionar(documentos[0].ID)).Returns(placaDocumento);

            var retorno = _pdBusiness.ProcessarDocumentosVencidos(data);

            Assert.IsTrue(retorno == 0);
        }

        [TestMethod()]
        public void ProcessarDocumentosVencidosTest_BloquearComposicao_Success_WithSentEmail()
        {
            var documentos = new List<PlacaDocumentoView>()
            {
                new PlacaDocumentoView()
                {
                    ID = 1,
                    IDComposicao = 1,
                    TipoAlerta = 1,
                    TipoBloqueioImediato = EnumTipoBloqueioImediato.Sim,
                    TipoAcaoVencimento = EnumTipoAcaoVencimento.Bloquear,
                    Email = "email@empresa.com.br",
                    EmailCif = "emailcif@empresa.com.br",
                    QtdeAlertas = 1
                }
            };

            var placaDocumento = new PlacaDocumento()
            {
                ID = 1,
                Alerta1Enviado = false
            };

            var placa = new Placa()
            {
                ID = 1,
                IDPais = EnumPais.Padrao
            };

            var composicao = new Composicao()
            {
                ID = 1,
                IDPlaca1 = 1
            };

            var data = DateTime.Now.Date;

            placaDocumentoRepository.Setup(s => s.GetDocumentosBloqueados(data)).Returns(documentos);
            composicaoBusiness.Setup(s => s.Selecionar(documentos[0].ID)).Returns(composicao);
            composicaoBusiness.Setup(s => s.AtualizarComposicao(composicao, false, false, false, 0)).Returns(true);
            placaDocumentoRepository.Setup(s => s.Selecionar(documentos[0].ID)).Returns(placaDocumento);
            configBusiness.Setup(s => s.GetConfigInt(EnumConfig.habilitarVectoPlaca, It.IsAny<int>())).Returns(-1);
            configBusiness.Setup(s => s.GetConfigInt(EnumConfig.habilitarBloqueioPlaca, It.IsAny<int>())).Returns(-1);
            configBusiness.Setup(s => s.GetConfig(EnumConfig.JustificativaBloqueioAutomatico, It.IsAny<int>())).Returns("teste");
            placaBusiness.Setup(s => s.Selecionar(placaDocumento.ID)).Returns(placa);
            composicaoBusiness.Setup(s => s.AtualizarComposicao(It.IsAny<Composicao>(), false, true, true, (int)EnumStatusComposicao.Bloqueado)).Returns(true);

            var retorno = _pdBusiness.ProcessarDocumentosVencidos(data);

            Assert.IsTrue(retorno == 0);
            Assert.IsTrue(documentos[0].TipoAlerta == 1);
            Assert.IsTrue(documentos[0].Enviado);
            Assert.IsTrue(placaDocumento.Vencido);
            Assert.IsFalse(placaDocumento.Bloqueado);
            Assert.IsFalse(placaDocumento.Processado);
            Assert.IsTrue(composicao.Justificativa == "teste");
        }

        [TestMethod()]
        public void ProcessarDocumentosVencidosTest_BloquearComposicao_Success_WithoutSentEmail()
        {
            var documentos = new List<PlacaDocumentoView>()
            {
                new PlacaDocumentoView()
                {
                    ID = 1,
                    IDComposicao = 1,
                    TipoAlerta = 1,
                    TipoBloqueioImediato = EnumTipoBloqueioImediato.Sim,
                    TipoAcaoVencimento = EnumTipoAcaoVencimento.Bloquear,
                    Email = "email@empresa.com.br",
                    EmailCif = "emailcif@empresa.com.br",
                    QtdeAlertas = 0
                }
            };

            var placaDocumento = new PlacaDocumento()
            {
                ID = 1,
                Alerta1Enviado = false
            };

            var placa = new Placa()
            {
                ID = 1,
                IDPais = EnumPais.Padrao
            };

            var composicao = new Composicao()
            {
                ID = 1,
                IDPlaca1 = 1
            };

            var data = DateTime.Now.Date;

            placaDocumentoRepository.Setup(s => s.GetDocumentosBloqueados(data)).Returns(documentos);
            composicaoBusiness.Setup(s => s.Selecionar(documentos[0].ID)).Returns(composicao);
            composicaoBusiness.Setup(s => s.AtualizarComposicao(composicao, false, false, false, 0)).Returns(true);
            placaDocumentoRepository.Setup(s => s.Selecionar(documentos[0].ID)).Returns(placaDocumento);
            configBusiness.Setup(s => s.GetConfigInt(EnumConfig.habilitarVectoPlaca, It.IsAny<int>())).Returns(-1);
            configBusiness.Setup(s => s.GetConfigInt(EnumConfig.habilitarBloqueioPlaca, It.IsAny<int>())).Returns(-1);
            configBusiness.Setup(s => s.GetConfig(EnumConfig.JustificativaBloqueioAutomatico, It.IsAny<int>())).Returns("teste");
            placaBusiness.Setup(s => s.Selecionar(placaDocumento.ID)).Returns(placa);
            composicaoBusiness.Setup(s => s.AtualizarComposicao(It.IsAny<Composicao>(), false, true, true, (int)EnumStatusComposicao.Bloqueado)).Returns(true);

            var retorno = _pdBusiness.ProcessarDocumentosVencidos(data);

            Assert.IsTrue(retorno == 0);
            Assert.IsTrue(documentos[0].TipoAlerta == 1);
            Assert.IsFalse(documentos[0].Enviado);
            Assert.IsTrue(placaDocumento.Vencido);
            Assert.IsFalse(placaDocumento.Bloqueado);
            Assert.IsFalse(placaDocumento.Processado);
            Assert.IsTrue(composicao.Justificativa == "teste");
        }

        [TestMethod()]
        public void ProcessarDocumentosVencidosTest_BloquearComposicao_Success_OneWithSentEmail_OtherWihoutSentEmail()
        {
            var documentos = new List<PlacaDocumentoView>()
            {
                new PlacaDocumentoView()
                {
                    ID = 1,
                    IDComposicao = 1,
                    TipoAlerta = 1,
                    Sigla = "DOC1",
                    TipoBloqueioImediato = EnumTipoBloqueioImediato.Sim,
                    TipoAcaoVencimento = EnumTipoAcaoVencimento.Bloquear,
                    Email = "email@empresa.com.br",
                    EmailCif = "emailcif@empresa.com.br",
                    QtdeAlertas = 1
                },
                 new PlacaDocumentoView()
                {
                    ID = 2,
                    IDComposicao = 2,
                    TipoAlerta = 2,
                    Sigla = "DOC2",
                    TipoBloqueioImediato = EnumTipoBloqueioImediato.Sim,
                    TipoAcaoVencimento = EnumTipoAcaoVencimento.Bloquear,
                    Email = "email@empresa.com.br",
                    EmailCif = "emailcif@empresa.com.br",
                    QtdeAlertas = 0
                }
            };

            var placaDocumento1 = new PlacaDocumento()
            {
                ID = 1,
                Alerta1Enviado = false
            };

            var placa1 = new Placa()
            {
                ID = 1,
                IDPais = EnumPais.Padrao
            };

            var composicao1 = new Composicao()
            {
                ID = 1,
                IDPlaca1 = 1
            };

            var placaDocumento2 = new PlacaDocumento()
            {
                ID = 2,
                Alerta1Enviado = false
            };

            var placa2 = new Placa()
            {
                ID = 2,
                IDPais = EnumPais.Padrao
            };

            var composicao2 = new Composicao()
            {
                ID = 2,
                IDPlaca1 = 2
            };
            var data = DateTime.Now.Date;

            placaDocumentoRepository.Setup(s => s.GetDocumentosBloqueados(data)).Returns(documentos);
            composicaoBusiness.Setup(s => s.Selecionar(documentos[0].ID)).Returns(composicao1);
            composicaoBusiness.Setup(s => s.Selecionar(documentos[1].ID)).Returns(composicao2);
            composicaoBusiness.Setup(s => s.AtualizarComposicao(composicao1, false, false, false, 0)).Returns(true);
            composicaoBusiness.Setup(s => s.AtualizarComposicao(composicao2, false, false, false, 0)).Returns(true);
            placaDocumentoRepository.Setup(s => s.Selecionar(documentos[0].ID)).Returns(placaDocumento1);
            placaDocumentoRepository.Setup(s => s.Selecionar(documentos[1].ID)).Returns(placaDocumento2);
            configBusiness.Setup(s => s.GetConfigInt(EnumConfig.habilitarVectoPlaca, It.IsAny<int>())).Returns(-1);
            configBusiness.Setup(s => s.GetConfigInt(EnumConfig.habilitarBloqueioPlaca, It.IsAny<int>())).Returns(-1);
            configBusiness.Setup(s => s.GetConfig(EnumConfig.JustificativaBloqueioAutomatico, It.IsAny<int>())).Returns("teste");
            placaBusiness.Setup(s => s.Selecionar(placaDocumento1.ID)).Returns(placa1);
            placaBusiness.Setup(s => s.Selecionar(placaDocumento2.ID)).Returns(placa2);
            composicaoBusiness.Setup(s => s.AtualizarComposicao(It.IsAny<Composicao>(), false, true, true, (int)EnumStatusComposicao.Bloqueado)).Returns(true);

            var retorno = _pdBusiness.ProcessarDocumentosVencidos(data);

            Assert.IsTrue(retorno == 0);

            Assert.IsTrue(documentos[0].TipoAlerta == 1);
            Assert.IsTrue(documentos[0].Enviado);
            Assert.IsTrue(placaDocumento1.Vencido);
            Assert.IsFalse(placaDocumento1.Bloqueado);
            Assert.IsFalse(placaDocumento1.Processado);
            Assert.IsTrue(composicao1.Justificativa == "teste");

            Assert.IsTrue(documentos[1].TipoAlerta == 1);
            Assert.IsFalse(documentos[1].Enviado);
            Assert.IsTrue(placaDocumento2.Vencido);
            Assert.IsFalse(placaDocumento2.Bloqueado);
            Assert.IsFalse(placaDocumento2.Processado);
            Assert.IsTrue(composicao2.Justificativa == "teste");
        }

        [TestMethod()]
        public void ProcessarDocumentosVencidosTest_ReprovarComposicao_Success()
        {
            var documentos = new List<PlacaDocumentoView>()
            {
                new PlacaDocumentoView()
                {
                    ID = 1,
                    IDComposicao = 1,
                    TipoAlerta = 1,
                    TipoBloqueioImediato = EnumTipoBloqueioImediato.Sim,
                    TipoAcaoVencimento = EnumTipoAcaoVencimento.Reprovar
                }
            };

            var placaDocumento = new PlacaDocumento()
            {
                ID = 1,
                Alerta1Enviado = false
            };

            var placa = new Placa()
            {
                ID = 1,
                IDPais = EnumPais.Padrao
            };

            var composicao = new Composicao()
            {
                ID = 1,
                IDPlaca1 = 1
            };

            var data = DateTime.Now.Date;

            placaDocumentoRepository.Setup(s => s.GetDocumentosBloqueados(data)).Returns(documentos);
            composicaoBusiness.Setup(s => s.Selecionar(documentos[0].ID)).Returns(composicao);
            composicaoBusiness.Setup(s => s.AtualizarComposicao(composicao, false, false, false, 0)).Returns(true);
            placaDocumentoRepository.Setup(s => s.Selecionar(documentos[0].ID)).Returns(placaDocumento);
            configBusiness.Setup(s => s.GetConfigInt(EnumConfig.habilitarVectoPlaca, It.IsAny<int>())).Returns(-1);
            configBusiness.Setup(s => s.GetConfigInt(EnumConfig.habilitarReprovaDocPlacaAutomatica, It.IsAny<int>())).Returns(-1);
            configBusiness.Setup(s => s.GetConfig(EnumConfig.JustificativaReprovaAutomatica, It.IsAny<int>())).Returns("teste");
            placaBusiness.Setup(s => s.Selecionar(placaDocumento.ID)).Returns(placa);
            composicaoBusiness.Setup(s => s.AtualizarComposicao(It.IsAny<Composicao>(), false, true, false, (int)EnumStatusComposicao.Reprovado)).Returns(true);

            var retorno = _pdBusiness.ProcessarDocumentosVencidos(data);

            Assert.IsTrue(retorno == 0);
            Assert.IsTrue(documentos[0].TipoAlerta == 1);
            Assert.IsTrue(placaDocumento.Vencido);
            Assert.IsTrue(placaDocumento.Bloqueado);
            Assert.IsTrue(placaDocumento.Processado);
            Assert.IsTrue(composicao.Justificativa == "teste");
        }

        #endregion

        #region ProcessarAlertaDocumentosComposicao

        [TestMethod]
        public void ProcessarAlertaDocumentosComposicao_NoItens_NoError()
        {
            var documentos = new List<PlacaDocumentoView>();

            var data = DateTime.Now.Date;

            placaDocumentoRepository.Setup(s => s.GetDocumentosAVencer(data)).Returns(documentos);

            var retorno = _pdBusiness.ProcessarAlertaDocumentosComposicao(data);

            Assert.IsTrue(retorno == 0);
        }

        [TestMethod]
        public void ProcessarAlertaDocumentosComposicao_NoEmail_NoError()
        {
            var documentos = new List<PlacaDocumentoView>()
            {
                new PlacaDocumentoView() { ID = 0, Operacao = "FOB", Email = "", IdEmpresa = (int)EnumEmpresa.Combustiveis, IdPais = EnumPais.Padrao }
            };

            var data = DateTime.Now.Date;

            placaDocumentoRepository.Setup(s => s.GetDocumentosAVencer(data)).Returns(documentos);

            var retorno = _pdBusiness.ProcessarAlertaDocumentosComposicao(data);

            Assert.IsTrue(retorno == 0);
        }

        [TestMethod]
        public void ProcessarAlertaDocumentosComposicao_Itens_NoError()
        {
            var documentos = new List<PlacaDocumentoView>()
            {
                new PlacaDocumentoView()
                {
                    ID = 1,
                    Operacao = "FOB",
                    Email = "email@teste.com.br",
                    IdEmpresa = (int)EnumEmpresa.Combustiveis, IdPais = EnumPais.Padrao,
                    QtdeAlertas = 1
                }
            };

            var data = DateTime.Now.Date;

            placaDocumentoRepository.Setup(s => s.GetDocumentosAVencer(data)).Returns(documentos);

            var retorno = _pdBusiness.ProcessarAlertaDocumentosComposicao(data);

            Assert.IsTrue(retorno == 0);
        }

        [TestMethod]
        public void ProcessarAlertaDocumentosComposicao_Alerta1_Sent()
        {
            var data = DateTime.Now.Date;

            var documentos = new List<PlacaDocumentoView>()
            {
                new PlacaDocumentoView()
                {
                    ID = 1,
                    Operacao = "FOB",
                    Alerta1Enviado = false,
                    DiasVencimento = 10,
                    DiasVencimentoA2 = 15,
                    DataVencimento = data.AddDays(10),
                    Email = "email@teste.com.br",
                    IdEmpresa = (int)EnumEmpresa.Combustiveis,
                    IdPais = EnumPais.Padrao,
                    QtdeAlertas = 1
                }
            };

            var placaDocumento = new PlacaDocumento()
            {
                ID = 1,
                Alerta1Enviado = false
            };

            placaDocumentoRepository.Setup(s => s.GetDocumentosAVencer(data)).Returns(documentos);
            placaDocumentoRepository.Setup(s => s.Selecionar(documentos[0].ID)).Returns(placaDocumento);

            var retorno = _pdBusiness.ProcessarAlertaDocumentosComposicao(data);

            Assert.IsTrue(retorno == 0);
            Assert.IsTrue(placaDocumento.Alerta1Enviado);
            Assert.IsFalse(placaDocumento.Alerta2Enviado);
            Assert.IsTrue(documentos[0].Enviado);
        }

        [TestMethod]
        public void ProcessarAlertaDocumentosComposicao_QtdeAlertasZerado_WithoutSentEmail()
        {
            var data = DateTime.Now.Date;

            var documentos = new List<PlacaDocumentoView>()
            {
                new PlacaDocumentoView()
                {
                    ID = 1,
                    Operacao = "FOB",
                    Alerta1Enviado = false,
                    DiasVencimento = 10,
                    DiasVencimentoA2 = 15,
                    DataVencimento = data.AddDays(10),
                    Email = "email@teste.com.br",
                    IdEmpresa = (int)EnumEmpresa.Combustiveis,
                    IdPais = EnumPais.Padrao,
                    QtdeAlertas = 0
                }
            };

            var placaDocumento = new PlacaDocumento()
            {
                ID = 1,
                Alerta1Enviado = false
            };

            placaDocumentoRepository.Setup(s => s.GetDocumentosAVencer(data)).Returns(documentos);
            placaDocumentoRepository.Setup(s => s.Selecionar(documentos[0].ID)).Returns(placaDocumento);

            var retorno = _pdBusiness.ProcessarAlertaDocumentosComposicao(data);

            Assert.IsTrue(retorno == 0);
            Assert.IsFalse(placaDocumento.Alerta1Enviado);
            Assert.IsFalse(placaDocumento.Alerta2Enviado);
            Assert.IsFalse(documentos[0].Enviado);
        }

        [TestMethod]
        public void ProcessarAlertaDocumentosComposicao_Alerta2_Sent()
        {
            var data = DateTime.Now.Date;

            var documentos = new List<PlacaDocumentoView>()
            {
                new PlacaDocumentoView()
                {
                    ID = 1,
                    Operacao = "FOB",
                    Alerta1Enviado = true,
                    Alerta2Enviado = false,
                    DiasVencimento = 10,
                    DiasVencimentoA2 = 15,
                    DataVencimento = data.AddDays(15),
                    Email = "email@teste.com.br",
                    IdEmpresa = (int)EnumEmpresa.Combustiveis,
                    IdPais = EnumPais.Padrao,
                    QtdeAlertas = 2
                }
            };

            var placaDocumento = new PlacaDocumento()
            {
                ID = 1,
                Alerta1Enviado = true,
                Alerta2Enviado = false
            };

            placaDocumentoRepository.Setup(s => s.GetDocumentosAVencer(data)).Returns(documentos);
            placaDocumentoRepository.Setup(s => s.Selecionar(documentos[0].ID)).Returns(placaDocumento);

            var retorno = _pdBusiness.ProcessarAlertaDocumentosComposicao(data);

            Assert.IsTrue(retorno == 0);
            Assert.IsTrue(placaDocumento.Alerta1Enviado);
            Assert.IsTrue(placaDocumento.Alerta2Enviado);
            Assert.IsTrue(documentos[0].Enviado);
        }

        [TestMethod]
        public void ProcessarAlertaDocumentosComposicao_ValidateEmail_Success()
        {
            var documentos = new List<PlacaDocumentoView>()
            {
                new PlacaDocumentoView() { ID = 0, Operacao = "FOB", Email = "fob@email.com", EmailCif = "cif@email.com", EmailEmpresaAmbos = "ambos@email.com", IdEmpresa = (int)EnumEmpresa.Combustiveis, IdPais = EnumPais.Padrao },
                new PlacaDocumentoView() { ID = 0, Operacao = "CIF", Email = "fob@email.com", EmailCif = "cif@email.com", EmailEmpresaAmbos = "ambos@email.com", IdEmpresa = (int)EnumEmpresa.Combustiveis, IdPais = EnumPais.Padrao },
                new PlacaDocumentoView() { ID = 0, Operacao = "CIF", Email = "fob@email.com", EmailCif = "cif@email.com", EmailEmpresaAmbos = "ambos@email.com", IdEmpresa = (int)EnumEmpresa.Ambos, IdPais = EnumPais.Padrao }
            };

            Assert.IsTrue(documentos[0].EmailEnviar == "fob@email.com");
            Assert.IsTrue(documentos[1].EmailEnviar == "cif@email.com");
            Assert.IsTrue(documentos[2].EmailEnviar == "ambos@email.com");
        }

        [TestMethod]
        public void ProcessarAlertaDocumentosComposicao_ErrorExpected()
        {
            var data = DateTime.Now.Date;

            var documentos = new List<PlacaDocumentoView>()
            {
                new PlacaDocumentoView()
                {
                    ID = 1,
                    Operacao = "FOB",
                    Alerta1Enviado = true,
                    Alerta2Enviado = false,
                    DiasVencimento = 10,
                    DiasVencimentoA2 = 15,
                    DataVencimento = data.AddDays(15),
                    Email = "email@teste.com.br",
                    IdEmpresa = (int)EnumEmpresa.Combustiveis,
                    IdPais = EnumPais.Padrao,
                    QtdeAlertas = 1
                }
            };

            var placaDocumento = new PlacaDocumento()
            {
                ID = 1,
                Alerta1Enviado = true,
                Alerta2Enviado = false
            };

            placaDocumentoRepository.Setup(s => s.GetDocumentosAVencer(data)).Returns(documentos);
            placaDocumentoRepository.Setup(s => s.Selecionar(documentos[0].ID)).Throws(new Exception());

            var retorno = _pdBusiness.ProcessarAlertaDocumentosComposicao(data);

            Assert.IsTrue(retorno == 1);
            Assert.IsTrue(documentos[0].Enviado);
        }

        #endregion

        #region ListarPlacaDocumento

        [IgnoreAttribute("Teste desligado até resolver acesso ao banco")]
        [TestMethod]
        public void ListarPlacaDocumentoTest()
        {
            var listaDocumentosPlaca = _pdBll.ListarPlacaDocumento((int)EnumTipoVeiculo.Carreta,
                (int)EnumCategoriaVeiculo.Particular, "CIF", (int)EnumEmpresa.Combustiveis,
                (int)EnumTipoProduto.ARLA);
            Assert.IsNotNull(listaDocumentosPlaca);
        }

        [IgnoreAttribute("Teste desligado até resolver a questão da ordem de execução no Jenkins")]
        [TestMethod()]
        public void ListarPlacaDocumentoPorPlacaTest()
        {
            var placaSelecionar = _placaBll.Listar().FirstOrDefault()?.PlacaVeiculo;
            var placa = _placaBll.Selecionar(p => p.PlacaVeiculo == placaSelecionar);
            Assert.IsNotNull(_pdBll.ListarPlacaDocumentoPorPlaca(placa.ID));
        }

        #endregion
    }
}