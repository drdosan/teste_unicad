using Microsoft.VisualStudio.TestTools.UnitTesting;
using Raizen.UniCad.BLL.Extensions;
using Raizen.UniCad.Model;
using Raizen.UniCad.Model.Filtro;
using Raizen.UniCad.Model.View;
using Raizen.UniCad.Web.Models;
using System.IO;
using System.Reflection;
using System.Web;

namespace Raizen.UniCad.BLLTests
{
    [TestClass]
    public class ModelFillTests
    {
        #region Commons

        private static void AssertModel(object model)
        {
            foreach (PropertyInfo p in model.GetType().GetProperties())
                if (p.GetSetMethod() != null)
                    Assert.AreEqual(p.GetDefaultValue(), p.GetValue(model), $"Erro na verificação da propriedade [{p.Name}]");
        }

        #endregion

        #region Models

        [TestMethod]
        public void AgendamentoChecklistTest()
        {
            //Arrange
            var model = new AgendamentoChecklist();

            //Act
            model.SetGetDefaults();

            //Assert
            AssertModel(model);
        }

        [TestMethod]
        public void AgendamentoTerminalTest()
        {
            //Arrange
            var model = new AgendamentoTerminal();

            //Act
            model.SetGetDefaults();

            //Assert
            AssertModel(model);
        }


        [TestMethod]
        public void AgendamentoTerminalHorarioTest()
        {
            //Arrange
            var model = new AgendamentoTerminalHorario();

            //Act
            model.SetGetDefaults();

            //Assert
            AssertModel(model);
        }

        [TestMethod]
        public void AgendamentoTreinamentoTest()
        {
            //Arrange
            var model = new AgendamentoTreinamento();

            //Act
            model.SetGetDefaults();

            //Assert
            AssertModel(model);
        }

        [TestMethod]
        public void CategoriaVeiculoTest()
        {
            //Arrange
            var model = new CategoriaVeiculo();

            //Act
            model.SetGetDefaults();

            //Assert
            AssertModel(model);
        }

        [TestMethod]
        public void ChecklistComposicaoTest()
        {
            //Arrange
            var model = new ChecklistComposicao();

            //Act
            model.SetGetDefaults();

            //Assert
            AssertModel(model);
        }

        [TestMethod]
        public void ClienteTest()
        {
            //Arrange
            var model = new Cliente();

            //Act
            model.SetGetDefaults();

            //Assert
            AssertModel(model);
        }

        [TestMethod]
        public void ComposicaoTest()
        {
            //Arrange
            var model = new Composicao();

            //Act
            model.SetGetDefaults();

            //Assert
            AssertModel(model);
        }

        [TestMethod]
        public void ComposicaoEixoTest()
        {
            //Arrange
            var model = new ComposicaoEixo();

            //Act
            model.SetGetDefaults();

            //Assert
            AssertModel(model);
        }

        [TestMethod]
        public void ConfiguracaoTest()
        {
            //Arrange
            var model = new Configuracao();

            //Act
            model.SetGetDefaults();

            //Assert
            AssertModel(model);
        }

        [TestMethod]
        public void EmpresaTest()
        {
            //Arrange
            var model = new Empresa();

            //Act
            model.SetGetDefaults();

            //Assert
            AssertModel(model);
        }

        [TestMethod]
        public void EstadoTest()
        {
            //Arrange
            var model = new Estado();

            //Act
            model.SetGetDefaults();

            //Assert
            AssertModel(model);
        }

        [TestMethod]
        public void HistoricoTreinamentoTeoricoMotoristaTest()
        {
            //Arrange
            var model = new HistoricoTreinamentoTeoricoMotorista();

            //Act
            model.SetGetDefaults();

            //Assert
            AssertModel(model);
        }

        [TestMethod]
        public void HistorioAtivarMotoristaTest()
        {
            //Arrange
            var model = new HistorioAtivarMotorista();

            //Act
            model.SetGetDefaults();

            //Assert
            AssertModel(model);
        }

        [TestMethod]
        public void HistorioBloqueioComposicaoTest()
        {
            //Arrange
            var model = new HistorioBloqueioComposicao();

            //Act
            model.SetGetDefaults();

            //Assert
            AssertModel(model);
        }

        [TestMethod]
        public void HistorioBloqueioMotoristaTest()
        {
            //Arrange
            var model = new HistorioBloqueioMotorista();

            //Act
            model.SetGetDefaults();

            //Assert
            AssertModel(model);
        }

        [TestMethod]
        public void JobTest()
        {
            //Arrange
            var model = new Job();

            //Act
            model.SetGetDefaults();

            //Assert
            AssertModel(model);
        }

        [TestMethod]
        public void LogDocumentosTest()
        {
            //Arrange
            var model = new LogDocumentos();

            //Act
            model.SetGetDefaults();

            //Assert
            AssertModel(model);
        }

        [TestMethod]
        public void LogExecucaoJobTest()
        {
            //Arrange
            var model = new LogExecucaoJob();

            //Act
            model.SetGetDefaults();

            //Assert
            AssertModel(model);
        }

        [TestMethod]
        public void MotoristaTest()
        {
            //Arrange
            var model = new Motorista();

            //Act
            model.SetGetDefaults();

            //Assert
            AssertModel(model);
        }

        [TestMethod]
        public void MotoristaArgentinaTest()
        {
            //Arrange
            var model = new MotoristaArgentina();

            //Act
            model.SetGetDefaults();

            //Assert
            AssertModel(model);
        }

        [TestMethod]
        public void MotoristaBrasilTest()
        {
            //Arrange
            var model = new MotoristaBrasil();

            //Act
            model.SetGetDefaults();

            //Assert
            AssertModel(model);
        }

        [TestMethod]
        public void MotoristaClienteTest()
        {
            //Arrange
            var model = new MotoristaCliente();

            //Act
            model.SetGetDefaults();

            //Assert
            AssertModel(model);
        }

        [TestMethod]
        public void MotoristaDocumentoTest()
        {
            //Arrange
            var model = new MotoristaDocumento();

            //Act
            model.SetGetDefaults();

            //Assert
            AssertModel(model);
        }

        [TestMethod]
        public void MotoristaPesquisaTest()
        {
            //Arrange
            var model = new MotoristaPesquisa();

            //Act
            model.SetGetDefaults();

            //Assert
            AssertModel(model);
        }

        [TestMethod]
        public void MotoristaTipoComposicaoTest()
        {
            //Arrange
            var model = new MotoristaTipoComposicao();

            //Act
            model.SetGetDefaults();

            //Assert
            AssertModel(model);
        }

        [TestMethod]
        public void MotoristaTipoProdutoTest()
        {
            //Arrange
            var model = new MotoristaTipoProduto();

            //Act
            model.SetGetDefaults();

            //Assert
            AssertModel(model);
        }

        [TestMethod]
        public void MotoristaTreinamentoTerminalTest()
        {
            //Arrange
            var model = new MotoristaTreinamentoTerminal();

            //Act
            model.SetGetDefaults();

            //Assert
            AssertModel(model);
        }

        [TestMethod]
        public void PaisTest()
        {
            //Arrange
            var model = new Pais();

            //Act
            model.SetGetDefaults();

            //Assert
            AssertModel(model);
        }

        [TestMethod]
        public void PerfilTest()
        {
            //Arrange
            var model = new Perfil();

            //Act
            model.SetGetDefaults();

            //Assert
            AssertModel(model);
        }

        [TestMethod]
        public void PlacaTest()
        {
            //Arrange
            var model = new Placa();

            //Act
            model.SetGetDefaults();

            //Assert
            AssertModel(model);
        }

        [TestMethod]
        public void PlacaArgentinaTest()
        {
            //Arrange
            var model = new PlacaArgentina();

            //Act
            model.SetGetDefaults();

            //Assert
            AssertModel(model);
        }

        [TestMethod]
        public void PlacaBrasilTest()
        {
            //Arrange
            var model = new PlacaBrasil();

            //Act
            model.SetGetDefaults();

            //Assert
            AssertModel(model);
        }

        [TestMethod]
        public void PlacaClienteTest()
        {
            //Arrange
            var model = new PlacaCliente();

            //Act
            model.SetGetDefaults();

            //Assert
            AssertModel(model);
        }

        [TestMethod]
        public void PlacaDocumentoTest()
        {
            //Arrange
            var model = new PlacaDocumento();

            //Act
            model.SetGetDefaults();

            //Assert
            AssertModel(model);
        }

        [TestMethod]
        public void PlacaSetaTest()
        {
            //Arrange
            var model = new PlacaSeta();

            //Act
            model.SetGetDefaults();

            //Assert
            AssertModel(model);
        }

        [TestMethod]
        public void PlacaSetaConstrutorTest()
        {
            //Arrange
            var colunas = 1;
            var sequencial = 2;
            var multiSeta = true;
            var model = new PlacaSeta(colunas, sequencial, multiSeta);

            //Act

            //Assert
            Assert.AreEqual(model.Colunas, colunas);
            Assert.AreEqual(model.Sequencial, sequencial);
            Assert.AreEqual(model.Multiseta, multiSeta);
        }

        [TestMethod]
        public void ProdutoTest()
        {
            //Arrange
            var model = new Produto();

            //Act
            model.SetGetDefaults();

            //Assert
            AssertModel(model);
        }

        [TestMethod]
        public void SincronizacaoMotoristasTest()
        {
            //Arrange
            var model = new SincronizacaoMotoristas();

            //Act
            model.SetGetDefaults();

            //Assert
            AssertModel(model);
        }

        [TestMethod]
        public void TerminalTest()
        {
            //Arrange
            var model = new Terminal();

            //Act
            model.SetGetDefaults();

            //Assert
            AssertModel(model);
        }

        [TestMethod]
        public void TerminalEmpresaTest()
        {
            //Arrange
            var model = new TerminalEmpresa();

            //Act
            model.SetGetDefaults();

            //Assert
            AssertModel(model);
        }

        [TestMethod]
        public void TipoAgendaTest()
        {
            //Arrange
            var model = new TipoAgenda();

            //Act
            model.SetGetDefaults();

            //Assert
            AssertModel(model);
        }

        [TestMethod]
        public void TipoCarregamentoTest()
        {
            //Arrange
            var model = new TipoCarregamento();

            //Act
            model.SetGetDefaults();

            //Assert
            AssertModel(model);
        }

        [TestMethod]
        public void TipoComposicaoTest()
        {
            //Arrange
            var model = new TipoComposicao();

            //Act
            model.SetGetDefaults();

            //Assert
            AssertModel(model);
        }

        [TestMethod]
        public void TipoDocumentoTest()
        {
            //Arrange
            var model = new TipoDocumento();

            //Act
            model.SetGetDefaults();

            //Assert
            AssertModel(model);
        }

        [TestMethod]
        public void TipoDocumentoTipoComposicaoTest()
        {
            //Arrange
            var model = new TipoDocumentoTipoComposicao();

            //Act
            model.SetGetDefaults();

            //Assert
            AssertModel(model);
        }

        [TestMethod]
        public void TipoDocumentoTipoComposicaoPlacaViewTest()
        {
            //Arrange
            var model = new TipoDocumentoTipoComposicaoPlacaView();

            //Act
            model.SetGetDefaults();

            //Assert
            AssertModel(model);
        }

        [TestMethod]
        public void TipoDocumentoTipoProdutoTest()
        {
            //Arrange
            var model = new TipoDocumentoTipoProduto();

            //Act
            model.SetGetDefaults();

            //Assert
            AssertModel(model);
        }

        [TestMethod]
        public void TipoDocumentoTipoVeiculoTest()
        {
            //Arrange
            var model = new TipoDocumentoTipoVeiculo();

            //Act
            model.SetGetDefaults();

            //Assert
            AssertModel(model);
        }

        [TestMethod]
        public void TipoProdutoTest()
        {
            //Arrange
            var model = new TipoProduto();

            //Act
            model.SetGetDefaults();

            //Assert
            AssertModel(model);
        }

        [TestMethod]
        public void TipoVeiculoTest()
        {
            //Arrange
            var model = new TipoVeiculo();

            //Act
            model.SetGetDefaults();

            //Assert
            AssertModel(model);
        }

        [TestMethod]
        public void TransportadoraTest()
        {
            //Arrange
            var model = new Transportadora();

            //Act
            model.SetGetDefaults();

            //Assert
            AssertModel(model);
        }

        [TestMethod]
        public void UsuarioTest()
        {
            //Arrange
            var model = new Usuario();

            //Act
            model.SetGetDefaults();

            //Assert
            AssertModel(model);
        }

        [TestMethod]
        public void UsuarioClienteTest()
        {
            //Arrange
            var model = new UsuarioCliente();

            //Act
            model.SetGetDefaults();

            //Assert
            AssertModel(model);
        }

        [TestMethod]
        public void UsuarioTransportadoraTest()
        {
            //Arrange
            var model = new UsuarioTransportadora();

            //Act
            model.SetGetDefaults();

            //Assert
            AssertModel(model);
        }

        #endregion

        #region Filtros

        [TestMethod]
        public void AgendamentoChecklistFiltroTest()
        {
            //Arrange
            var model = new AgendamentoChecklistFiltro();

            //Act
            model.SetGetDefaults();

            //Assert
            AssertModel(model);
        }

        [TestMethod]
        public void AgendamentoTerminalFiltroTest()
        {
            //Arrange
            var model = new AgendamentoTerminalFiltro();

            //Act
            model.SetGetDefaults();

            //Assert
            AssertModel(model);
        }

        [TestMethod]
        public void AgendamentoTerminalHorarioFiltroTest()
        {
            //Arrange
            var model = new AgendamentoTerminalHorarioFiltro();

            //Act
            model.SetGetDefaults();

            //Assert
            AssertModel(model);
        }

        [TestMethod]
        public void AgendamentoTreinamentoFiltroTest()
        {
            //Arrange
            var model = new AgendamentoTreinamentoFiltro();

            //Act
            model.SetGetDefaults();

            //Assert
            AssertModel(model);
        }


        [TestMethod]
        public void ClienteFiltroTest()
        {
            //Arrange
            var model = new ClienteFiltro();

            //Act
            model.SetGetDefaults();

            //Assert
            AssertModel(model);
        }

        [TestMethod]
        public void ComposicaoFiltroTest()
        {
            //Arrange
            var model = new ComposicaoFiltro();

            //Act
            model.SetGetDefaults();

            //Assert
            AssertModel(model);
        }

        [TestMethod]
        public void ComposicaoServicoFiltroTest()
        {
            //Arrange
            var model = new ComposicaoServicoFiltro();

            //Act
            model.SetGetDefaults();

            //Assert
            AssertModel(model);
        }

        [TestMethod]
        public void ConfiguracaoFiltroTest()
        {
            //Arrange
            var model = new ConfiguracaoFiltro();

            //Act
            model.SetGetDefaults();

            //Assert
            AssertModel(model);
        }

        [TestMethod]
        public void ExemploFiltroTest()
        {
            //Arrange
            var model = new ExemploFiltro();

            //Act
            model.SetGetDefaults();

            //Assert
            AssertModel(model);
        }

        [TestMethod]
        public void JobFiltroTest()
        {
            //Arrange
            var model = new JobFiltro();

            //Act
            model.SetGetDefaults();

            //Assert
            AssertModel(model);
        }

        [TestMethod]
        public void LogDocumentosFiltroTest()
        {
            //Arrange
            var model = new LogDocumentosFiltro();

            //Act
            model.SetGetDefaults();

            //Assert
            AssertModel(model);
        }

        [TestMethod]
        public void LogFiltroTest()
        {
            //Arrange
            var model = new LogFiltro();

            //Act
            model.SetGetDefaults();

            //Assert
            AssertModel(model);
        }

        [TestMethod]
        public void MotoristaFiltroTest()
        {
            //Arrange
            var model = new MotoristaFiltro();

            //Act
            model.SetGetDefaults();

            //Assert
            AssertModel(model);
        }

        [TestMethod]
        public void MotoristaServicoFiltroTest()
        {
            //Arrange
            var model = new MotoristaServicoFiltro();

            //Act
            model.SetGetDefaults();

            //Assert
            AssertModel(model);
        }

        [TestMethod]
        public void PlacaFiltroTest()
        {
            //Arrange
            var model = new PlacaFiltro();

            //Act
            model.SetGetDefaults();

            //Assert
            AssertModel(model);
        }

        [TestMethod]
        public void PlacaServicoFiltroTest()
        {
            //Arrange
            var model = new PlacaServicoFiltro();

            //Act
            model.SetGetDefaults();

            //Assert
            AssertModel(model);
        }

        [TestMethod]
        public void ProdutoFiltroTest()
        {
            //Arrange
            var model = new ProdutoFiltro();

            //Act
            model.SetGetDefaults();

            //Assert
            AssertModel(model);
        }

        [TestMethod]
        public void SincronizacaoMotoristasFiltroTest()
        {
            //Arrange
            var model = new SincronizacaoMotoristasFiltro();

            //Act
            model.SetGetDefaults();

            //Assert
            AssertModel(model);
        }

        [TestMethod]
        public void TerminalEmpresaFiltroTest()
        {
            //Arrange
            var model = new TerminalEmpresaFiltro();

            //Act
            model.SetGetDefaults();

            //Assert
            AssertModel(model);
        }

        [TestMethod]
        public void TerminalFiltroTest()
        {
            //Arrange
            var model = new TerminalFiltro();

            //Act
            model.SetGetDefaults();

            //Assert
            AssertModel(model);
        }

        [TestMethod]
        public void TipoAgendaFiltroTest()
        {
            //Arrange
            var model = new TipoAgendaFiltro();

            //Act
            model.SetGetDefaults();

            //Assert
            AssertModel(model);
        }

        [TestMethod]
        public void TipoDocumentoFiltroTest()
        {
            //Arrange
            var model = new TipoDocumentoFiltro();

            //Act
            model.SetGetDefaults();

            //Assert
            AssertModel(model);
        }

        [TestMethod]
        public void TransportadoraFiltroTest()
        {
            //Arrange
            var model = new TransportadoraFiltro();

            //Act
            model.SetGetDefaults();

            //Assert
            AssertModel(model);
        }

        [TestMethod]
        public void UsuarioFiltroTest()
        {
            //Arrange
            var model = new UsuarioFiltro();

            //Act
            model.SetGetDefaults();

            //Assert
            AssertModel(model);
        }

        #endregion

        #region Views

        [TestMethod]
        public void AgendamentoChecklistViewTest()
        {
            //Arrange
            var model = new AgendamentoChecklistView();

            //Act
            model.SetGetDefaults();

            //Assert
            AssertModel(model);
        }

        [TestMethod]
        public void AgendamentoTerminalHorarioViewTest()
        {
            //Arrange
            var model = new AgendamentoTerminalHorarioView();

            //Act
            model.SetGetDefaults();

            //Assert
            AssertModel(model);
        }

        [TestMethod]
        public void AgendamentoTerminalViewTest()
        {
            //Arrange
            var model = new AgendamentoTerminalView();

            //Act
            model.SetGetDefaults();

            //Assert
            AssertModel(model);
        }

        [TestMethod]
        public void AgendamentoTreinamentoRetornoViewTest()
        {
            //Arrange
            var model = new AgendamentoTreinamentoRetornoView();

            //Act
            model.SetGetDefaults();

            //Assert
            AssertModel(model);
        }

        [TestMethod]
        public void AgendamentoTreinamentoViewTest()
        {
            //Arrange
            var model = new AgendamentoTreinamentoView();

            //Act
            model.SetGetDefaults();

            //Assert
            AssertModel(model);
        }

        [TestMethod]
        public void CapacidadeSetaViewTest()
        {
            //Arrange
            var model = new CapacidadeSetaView();

            //Act
            model.SetGetDefaults();

            //Assert
            AssertModel(model);
        }

        [TestMethod]
        public void CarteirinhaViewTest()
        {
            //Arrange
            var model = new CarteirinhaView();

            //Act
            model.SetGetDefaults();

            //Assert
            AssertModel(model);
        }

        [TestMethod]
        public void ClienteTransportadoraViewTest()
        {
            //Arrange
            var model = new ClienteTransportadoraView();

            //Act
            model.SetGetDefaults();

            //Assert
            AssertModel(model);
        }

        [TestMethod]
        public void CompartimentoViewTest()
        {
            //Arrange
            var model = new CompartimentoView();

            //Act
            model.SetGetDefaults();

            //Assert
            AssertModel(model);
        }

        [TestMethod]
        public void SetaViewTest()
        {
            //Arrange
            var model = new SetaView();

            //Act
            model.SetGetDefaults();

            //Assert
            AssertModel(model);
        }

        [TestMethod]
        public void ComposicaoServicoViewTest()
        {
            //Arrange
            var model = new ComposicaoServicoView();

            //Act
            model.SetGetDefaults();

            //Assert
            AssertModel(model);
        }

        [TestMethod]
        public void ComposicaoViewTest()
        {
            //Arrange
            var model = new ComposicaoView();

            //Act
            model.SetGetDefaults();

            //Assert
            AssertModel(model);
        }

        [TestMethod]
        public void ControlePresencaMotoristaViewTest()
        {
            //Arrange
            var model = new ControlePresencaMotoristaView();

            //Act
            model.SetGetDefaults();

            //Assert
            AssertModel(model);
        }

        [TestMethod]
        public void EasyQueryViewTest()
        {
            //Arrange
            var model = new EasyQueryView();

            //Act
            model.SetGetDefaults();

            //Assert
            AssertModel(model);
        }

        [TestMethod]
        public void ExemploViewTest()
        {
            //Arrange
            var model = new ExemploView();

            //Act
            model.SetGetDefaults();

            //Assert
            AssertModel(model);
        }

        [TestMethod]
        public void LogDocumentosViewTest()
        {
            //Arrange
            var model = new LogDocumentosView();

            //Act
            model.SetGetDefaults();

            //Assert
            AssertModel(model);
        }

        [TestMethod]
        public void MotoristaAlteradoViewTest()
        {
            //Arrange
            var model = new MotoristaAlteradoView();

            //Act
            model.SetGetDefaults();

            //Assert
            AssertModel(model);
        }

        [TestMethod]
        public void MotoristaClienteViewTest()
        {
            //Arrange
            var model = new MotoristaClienteView();

            //Act
            model.SetGetDefaults();

            //Assert
            AssertModel(model);
        }

        [TestMethod]
        public void MotoristaDocumentoServicoViewTest()
        {
            //Arrange
            var model = new MotoristaDocumentoServicoView();

            //Act
            model.SetGetDefaults();

            //Assert
            AssertModel(model);
        }

        [TestMethod]
        public void MotoristaDocumentoViewTest()
        {
            //Arrange
            var model = new MotoristaDocumentoView();

            //Act
            model.SetGetDefaults();

            //Assert
            AssertModel(model);
        }

        [DataTestMethod]
        [DataRow("FOB", "motorista@email.com", "transportadora@email.com", "motorista@email.com")]
        [DataRow("CIF", "motorista@email.com", "transportadora@email.com", "transportadora@email.com")]
        public void MotoristaDocumentoViewEmailEnviarTest(string operacao, string emailMotorista, string emailTranportadora, string emailEsperado)
        {
            //Arrange
            var model = new MotoristaDocumentoView
            {
                Operacao = operacao,
                Email = emailMotorista,
                EmailTransportadora = emailTranportadora
            };

            //Act
            var emailAtual = model.EmailEnviar;

            //Assert
            Assert.AreEqual(emailEsperado, emailAtual);
        }

        [DataTestMethod]
        [DataRow(EnumPais.Brasil, "cpf287", "dni345", "cpf287")]
        [DataRow(EnumPais.Argentina, "cpf287", "dni345", "dni345")]
        public void MotoristaDocumentoViewDocumentoIdentificacaoTest(EnumPais pais, string cpf, string dni, string documentoEsperado)
        {
            //Arrange
            var model = new MotoristaDocumentoView
            {
                IdPais = pais,
                CPF = cpf,
                DNI = dni
            };

            //Act
            var documentoAtual = model.DocumentoIdentificacao;

            //Assert
            Assert.AreEqual(documentoEsperado, documentoAtual);
        }

        [TestMethod]
        public void MotoristaPermissaoServicoViewTest()
        {
            //Arrange
            var model = new MotoristaPermissaoServicoView();

            //Act
            model.SetGetDefaults();

            //Assert
            AssertModel(model);
        }

        [TestMethod]
        public void MotoristaServicoViewTest()
        {
            //Arrange
            var model = new MotoristaServicoView();

            //Act
            model.SetGetDefaults();

            //Assert
            AssertModel(model);
        }

        [TestMethod]
        public void MotoristaTreinamentoPraticoViewTest()
        {
            //Arrange
            var model = new MotoristaTreinamentoPraticoServicoView();

            //Act
            model.SetGetDefaults();

            //Assert
            AssertModel(model);
        }

        [TestMethod]
        public void MotoristaViewTest()
        {
            //Arrange
            var model = new MotoristaView();

            //Act
            model.SetGetDefaults();

            //Assert
            AssertModel(model);
        }

        [TestMethod]
        public void PlacaAlteradaViewTest()
        {
            //Arrange
            var model = new PlacaAlteradaView();

            //Act
            model.SetGetDefaults();

            //Assert
            AssertModel(model);
        }

        [TestMethod]
        public void PlacaClienteViewTest()
        {
            //Arrange
            var model = new PlacaClienteView();

            //Act
            model.SetGetDefaults();

            //Assert
            AssertModel(model);
        }        

        [TestMethod]
        public void PlacaClienteViewConstructorTest()
        {
            //Arrange
            var cpfcnpj = "cpf124";
            var razaoSocial = "Jose da Silva";
            var ibm = "ibm654";
            var id = 234;
            var cliente = new Cliente() { RazaoSocial = razaoSocial, CNPJCPF = cpfcnpj, IBM = ibm, ID = id };
            var model = new PlacaClienteView(cliente);

            //Act

            //Assert
            Assert.AreEqual($"{cliente.IBM} - {cliente.CNPJCPF} - {cliente.RazaoSocial}", model.RazaoSocial);
            Assert.AreEqual(id, model.IDCliente);
        }

        [TestMethod]
        public void PlacaCompartimentoServicoViewTest()
        {
            //Arrange
            var model = new PlacaCompartimentoServicoView();

            //Act
            model.SetGetDefaults();

            //Assert
            AssertModel(model);
        }

        [TestMethod]
        public void PlacaDocumentoServicoViewTest()
        {
            //Arrange
            var model = new PlacaDocumentoServicoView();

            //Act
            model.SetGetDefaults();

            //Assert
            AssertModel(model);
        }

        [TestMethod]
        public void PlacaDocumentoViewTest()
        {
            //Arrange
            var model = new PlacaDocumentoView();

            //Act
            model.SetGetDefaults();

            //Assert
            AssertModel(model);
        }

        [DataTestMethod]
        [DataRow("FOB", EnumEmpresa.EAB, "fob@email.com", "cif@email.com", "ambos@email.com", "fob@email.com")]
        [DataRow("FOB", EnumEmpresa.Combustiveis, "fob@email.com", "cif@email.com", "ambos@email.com", "fob@email.com")]
        [DataRow("FOB", EnumEmpresa.Ambos, "fob@email.com", "cif@email.com", "ambos@email.com", "fob@email.com")]
        [DataRow("CIF", EnumEmpresa.EAB, "fob@email.com", "cif@email.com", "ambos@email.com", "cif@email.com")]
        [DataRow("CIF", EnumEmpresa.Combustiveis, "fob@email.com", "cif@email.com", "ambos@email.com", "cif@email.com")]
        [DataRow("CIF", EnumEmpresa.Ambos, "fob@email.com", "cif@email.com", "ambos@email.com", "ambos@email.com")]

        public void PlacaDocumentoViewEmailEnviarTest(string operacao, EnumEmpresa empresa, string emailFOB, string emailCIF, string emailAMBOS, string emailEsperado)
        {
            //Arrange
            var model = new PlacaDocumentoView
            {
                Operacao = operacao,
                IdEmpresa = (int)empresa,
                Email = emailFOB,
                EmailCif = emailCIF,
                EmailEmpresaAmbos = emailAMBOS
            };

            //Act
            var emailAtual = model.EmailEnviar;

            //Assert
            Assert.AreEqual(emailEsperado, emailAtual);
        }

        [TestMethod]
        public void PlacaPermissaoServicoViewTest()
        {
            //Arrange
            var model = new PlacaPermissaoServicoView();

            //Act
            model.SetGetDefaults();

            //Assert
            AssertModel(model);
        }

        [TestMethod]
        public void PlacaServicoViewTest()
        {
            //Arrange
            var model = new PlacaServicoView();

            //Act
            model.SetGetDefaults();

            //Assert
            AssertModel(model);
        }

        [TestMethod]
        public void PlacaSetaViewTest()
        {
            //Arrange
            var model = new PlacaSetaView();

            //Act
            model.SetGetDefaults();

            //Assert
            AssertModel(model);
        }

        [TestMethod]
        public void PlacaViewTest()
        {
            //Arrange
            var model = new PlacaView();

            //Act
            model.SetGetDefaults();

            //Assert
            AssertModel(model);
        }

        [TestMethod]
        public void PlacaViewConstructorTest()
        {
            //Arrange
            var numeroEixos = 10;
            var numeroEixosDistanciados = 20;
            var numeroEixosPneusDuplos = 30;
            var categoriaVeiculo = EnumCategoriaVeiculo.Aluguel;
            var pais = EnumPais.Brasil;
            var model = new PlacaView(numeroEixos, numeroEixosDistanciados, numeroEixosPneusDuplos, categoriaVeiculo, pais);

            //Act

            //Assert
            Assert.AreEqual(numeroEixos, model.NumeroEixos);
            Assert.AreEqual(numeroEixosDistanciados, model.NumeroEixosDistanciados);
            Assert.AreEqual(numeroEixosPneusDuplos, model.NumeroEixosPneusDuplos);
            Assert.AreEqual(categoriaVeiculo, (EnumCategoriaVeiculo)model.IDCategoriaVeiculo);
            Assert.AreEqual(pais, (EnumPais)model.IdPais);
        }

        [TestMethod]
        public void PopupQuestionModelViewTest()
        {
            //Arrange
            var model = new PopupQuestionModelView();

            //Act
            model.SetGetDefaults();

            //Assert
            AssertModel(model);
        }

        [TestMethod]
        public void ProdutoViewTest()
        {
            //Arrange
            var model = new ProdutoView();

            //Act
            model.SetGetDefaults();

            //Assert
            AssertModel(model);
        }

        [TestMethod]
        public void SincronizacaoMotoritasViewTest()
        {
            //Arrange
            var model = new SincronizacaoMotoritasView();

            //Act
            model.SetGetDefaults();

            //Assert
            AssertModel(model);
        }

        [TestMethod]
        public void TerminalTreinamentoViewTest()
        {
            //Arrange
            var model = new TerminalTreinamentoView();

            //Act
            model.SetGetDefaults();

            //Assert
            AssertModel(model);
        }

        [TestMethod]
        public void TipoAgendaViewTest()
        {
            //Arrange
            var model = new TipoAgendaView();

            //Act
            model.SetGetDefaults();

            //Assert
            AssertModel(model);
        }

        [TestMethod]
        public void TipoDocumentoTipoProdutoViewTest()
        {
            //Arrange
            var model = new TipoDocumentoTipoProdutoView();

            //Act
            model.SetGetDefaults();

            //Assert
            AssertModel(model);
        }

        [TestMethod]
        public void TipoDocumentoTipoVeiculoViewTest()
        {
            //Arrange
            var model = new TipoDocumentoTipoVeiculoView();

            //Act
            model.SetGetDefaults();

            //Assert
            AssertModel(model);
        }

        [TestMethod]
        public void TipoDocumentoViewTest()
        {
            //Arrange
            var model = new TipoDocumentoView();

            //Act
            model.SetGetDefaults();

            //Assert
            AssertModel(model);
        }

        [TestMethod]
        public void TreinamentoViewTest()
        {
            //Arrange
            var model = new TreinamentoView();

            //Act
            model.SetGetDefaults();

            //Assert
            AssertModel(model);
        }

        [TestMethod]
        public void UsuarioClienteViewTest()
        {
            //Arrange
            var model = new UsuarioClienteView();

            //Act
            model.SetGetDefaults();

            //Assert
            AssertModel(model);
        }

        [TestMethod]
        public void UsuarioTransportadoraViewTest()
        {
            //Arrange
            var model = new UsuarioTransportadoraView();

            //Act
            model.SetGetDefaults();

            //Assert
            AssertModel(model);
        }

        [TestMethod]
        public void VisualizarCapacidadeSetaViewTest()
        {
            //Arrange
            var model = new VisualizarCapacidadeSetaView();

            //Act
            model.SetGetDefaults();

            //Assert
            AssertModel(model);
        }

        #endregion

        #region Others

        [DataTestMethod]
        [DataRow(true, true, "Mensagem em português", "Mensaje en español", "pt-BR", "Mensagem em português")]
        [DataRow(true, true, "Mensagem em português", "Mensaje en español", "es-AR", "Mensaje en español")]
        [DataRow(true, true, "Mensagem em português", "Mensaje en español", "outraLinguagem", "Mensagem em português")]
        [DataRow(true, true, "Mensagem em português", "Mensaje en español", null, "Mensagem em português")]
        [DataRow(true, false, "Mensagem em português", "Mensaje en español", null, "Mensagem em português")]
        [DataRow(false, false, "Mensagem em português", "Mensaje en español", null, "Mensagem em português")]
        public void DescricaoPorLinguaAttributeTest(bool gerarHttpContext, bool adicionaCookie, string msgPortugues, string msgEspanhol, string idioma, string msgEsperada)
        {
            //Arrange
            if (gerarHttpContext)
            {
                HttpContext.Current = new HttpContext(
                    new HttpRequest("", "http://tempuri.org", ""),
                    new HttpResponse(new StringWriter()));

                if (adicionaCookie)
                    HttpContext.Current.Request.Cookies.Add(new HttpCookie("SSO_IDIOMA", idioma));
            }

            //Act
            var model = new DescricaoPorLinguaAttribute(msgPortugues, msgEspanhol);

            //Assert
            Assert.AreEqual(msgEsperada, model.Description);
        }

        [TestMethod]
        public void EnumPerfilTest()
        {
            //Arrange
            
            //Act

            //Assert
            Assert.IsNotNull(EnumPerfil.CLIENTE_ACS);
            Assert.IsNotNull(EnumPerfil.CLIENTE_ACS_ARGENTINA);
        }

        [TestMethod]
        public void PlacaCloneTest()
        {
            //Arrange
            Placa placa = new Placa();

            //Act
            object placaClonada = placa.Clone();

            //Assert
            Assert.IsNotNull(placaClonada);
        }

        #endregion

        #region Web

        [TestMethod]
        public void AutenticarCsOnlineModelTest()
        {
            //Arrange
            var model = new AutenticarCsOnlineModel();

            //Act
            model.SetGetDefaults();

            //Assert
            AssertModel(model);
        }

        [TestMethod]
        public void BaseModelTest()
        {
            //Arrange
            var model = new BaseModel();

            //Act
            model.SetGetDefaults();

            //Assert
            AssertModel(model);
        }

        [TestMethod]
        public void ItemMenuViewModelTest()
        {
            //Arrange
            var model = new ItemMenuViewModel();

            //Act
            model.SetGetDefaults();

            //Assert
            AssertModel(model);
        }

        [TestMethod]
        public void ModelAgendamentoChecklistTest()
        {
            //Arrange
            var model = new ModelAgendamentoChecklist();

            //Act
            model.SetGetDefaults();

            //Assert
            AssertModel(model);
        }

        [TestMethod]
        public void ModelAgendamentoTerminalTest()
        {
            //Arrange
            var model = new ModelAgendamentoTerminal();

            //Act
            model.SetGetDefaults();

            //Assert
            AssertModel(model);
        }

        [TestMethod]
        public void ModelAgendamentoTreinamentoTest()
        {
            //Arrange
            var model = new ModelAgendamentoTreinamento();

            //Act
            model.SetGetDefaults();

            //Assert
            AssertModel(model);
        }

        [TestMethod]
        public void ModelComposicaoTest()
        {
            //Arrange
            var model = new ModelComposicao();

            //Act
            model.SetGetDefaults();

            //Assert
            AssertModel(model);
        }

        [TestMethod]
        public void ModelConfiguracaoTest()
        {
            //Arrange
            var model = new ModelConfiguracao();

            //Act
            model.SetGetDefaults();

            //Assert
            AssertModel(model);
        }

        [TestMethod]
        public void ModelControleAgendamentosTest()
        {
            //Arrange
            var model = new ModelControleAgendamentos();

            //Act
            model.SetGetDefaults();

            //Assert
            AssertModel(model);
        }

        [TestMethod]
        public void ModelErroTest()
        {
            //Arrange
            var model = new ModelErro();

            //Act
            model.SetGetDefaults();

            //Assert
            AssertModel(model);
        }

        [TestMethod]
        public void ModelHomeTest()
        {
            //Arrange
            var model = new ModelHome();

            //Act
            model.SetGetDefaults();

            //Assert
            AssertModel(model);
        }

        [TestMethod]
        public void ModelJobTest()
        {
            //Arrange
            var model = new ModelJob();

            //Act
            model.SetGetDefaults();

            //Assert
            AssertModel(model);
        }

        [TestMethod]
        public void ModelLogTest()
        {
            //Arrange
            var model = new ModelLog();

            //Act
            model.SetGetDefaults();

            //Assert
            AssertModel(model);
        }

        [TestMethod]
        public void ModelLogDocumentosTest()
        {
            //Arrange
            var model = new ModelLogDocumentos();

            //Act
            model.SetGetDefaults();

            //Assert
            AssertModel(model);
        }

        [TestMethod]
        public void ModelMotoristaTest()
        {
            //Arrange
            var model = new ModelMotorista();

            //Act
            model.SetGetDefaults();

            //Assert
            AssertModel(model);
        }

        [TestMethod]
        public void ModelMotoristaArgentinaTest()
        {
            //Arrange
            var model = new ModelMotoristaArgentina();

            //Act
            model.SetGetDefaults();

            //Assert
            AssertModel(model);
        }

        [TestMethod]
        public void ModelPlacaTest()
        {
            //Arrange
            var model = new ModelPlaca();

            //Act
            model.SetGetDefaults();

            //Assert
            AssertModel(model);
        }

        [TestMethod]
        public void ModelProdutoTest()
        {
            //Arrange
            var model = new ModelProduto();

            //Act
            model.SetGetDefaults();

            //Assert
            AssertModel(model);
        }

        [TestMethod]
        public void ModelSincronizacaoMotoritasTest()
        {
            //Arrange
            var model = new ModelSincronizacaoMotoritas();

            //Act
            model.SetGetDefaults();

            //Assert
            AssertModel(model);
        }

        [TestMethod]
        public void ModelTerminalTest()
        {
            //Arrange
            var model = new ModelTerminal();

            //Act
            model.SetGetDefaults();

            //Assert
            AssertModel(model);
        }


        [TestMethod]
        public void ModelTerminalEmpresaTest()
        {
            //Arrange
            var model = new ModelTerminalEmpresa();

            //Act
            model.SetGetDefaults();

            //Assert
            AssertModel(model);
        }

        [TestMethod]
        public void ModelTipoAgendaTest()
        {
            //Arrange
            var model = new ModelTipoAgenda();

            //Act
            model.SetGetDefaults();

            //Assert
            AssertModel(model);
        }

        [TestMethod]
        public void ModelTipoProdutoTest()
        {
            //Arrange
            var model = new ModelTipoDocumento();

            //Act
            model.SetGetDefaults();

            //Assert
            AssertModel(model);
        }

        [TestMethod]
        public void ModelUsuarioTest()
        {
            //Arrange
            var model = new ModelUsuario();

            //Act
            model.SetGetDefaults();

            //Assert
            AssertModel(model);
        }

       

        #endregion
    }
}