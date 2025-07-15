using ClosedXML.Excel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Raizen.UniCad.BLL;
using Raizen.UniCad.BLLTests.Bases;
using Raizen.UniCad.BLLTests.Fakes;
using Raizen.UniCad.Model;
using Raizen.UniCad.Model.View;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Raizen.UniCad.BLLTests
{

    [TestClass()]
    public class ComposicaoBusinessTests : BaseTest
    {
        private readonly ComposicaoBusiness _composicaoBll = new ComposicaoBusiness();
        private readonly ComposicaoPesquisaBusiness _composicaoPesquisaBll = new ComposicaoPesquisaBusiness();
        private readonly PlacaBusiness _placaBll = new PlacaBusiness();
        private readonly PlacaSetaBusiness _placaSetaBll = new PlacaSetaBusiness();
        private readonly PlacaDocumentoBusiness _placaDocumentoBll = new PlacaDocumentoBusiness();
        private readonly UsuarioBusiness _usuarioBll = new UsuarioBusiness();
        private readonly UsuarioTransportadoraBusiness _usuarioTransportadoraBll = new UsuarioTransportadoraBusiness();
        private readonly TransportadoraBusiness _transportadoraBll = new TransportadoraBusiness();
        //[TestMethod]
        //[TestCategory("Composicao")]
        //public void ListarVeiculoComposicao()
        //{
        //    ComposicaoBusiness compBll = new ComposicaoBusiness();
        //    var retorno = compBll.ListarVeiculoComposicao(new Model.Filtro.ComposicaoServicoFiltro());
        //    Assert.IsNotNull(retorno);
        //}

        [IgnoreAttribute("Teste desligado até resolver a questão da ordem de execução no Jenkins")]
        [TestMethod]
        [TestCategory("Composicao")]
        public void ProcessarDocumentosBloqueadoComposicaoAprovada()
        {
            //Assert.Inconclusive("Corrigir o webservice SAP para voltar a passar este teste");

            var composicao = new Composicao();
            #region criação de usuário e transportadora
            var usuario = FuncoesCompartilhadasTests.CriarUsuario("Usuário teste", "teste", "teste@teste.com",
                "Transportadora", "CIF", true, EnumEmpresa.Combustiveis);
            _usuarioBll.Adicionar(usuario);

            var transp = FuncoesCompartilhadasTests.CriarTransportadoraBrasil("34877528083", 1, "Teste Automatizado LTDA", "1234567890");
            _transportadoraBll.Adicionar(transp);

            var usuarioTransp = new UsuarioTransportadora
            {
                IDTransportadora = transp.ID,
                IDUsuario = usuario.ID
            };

            _usuarioTransportadoraBll.Adicionar(usuarioTransp);
            #endregion

            var siglaTeste = "TA1";
            var tipoDoc1 = FuncoesCompartilhadasTests.IncluirDocumento(siglaTeste, "Teste Automatizado 1", EnumCategoriaVeiculo.Particular, (int)EnumTipoBloqueioImediato.Sim);
            var tipoDoc2 = FuncoesCompartilhadasTests.IncluirDocumento(siglaTeste, "Teste Automatizado 2", EnumCategoriaVeiculo.Particular);

            var placaTruck = PlacaBusinessTests.AdicionarPlacas("CAU1000", (int)EnumTipoVeiculo.Truck, false, false);
            placaTruck.IDTransportadora = transp.ID;


            try
            {

                placaTruck.Documentos = new List<PlacaDocumentoView>
                {
                    new PlacaDocumentoView
                    {
                        DataVencimento = DateTime.Now,
                        IDTipoDocumento = tipoDoc1.ID,
                        Anexo = "teste.jpg"
                    },
                    new PlacaDocumentoView
                    {
                        DataVencimento = DateTime.Now,
                        IDTipoDocumento = tipoDoc2.ID,
                        Anexo = "teste2.jpg"
                    }
                };
                _placaBll.AdicionarPlaca(placaTruck);
                CriarComposicao(composicao, placaTruck, (int)EnumEmpresa.Ambos, "CIF",
                    (int)EnumTipoComposicao.Truck, 1, (int)EnumCategoriaVeiculo.Aluguel, "34428958000108",
                    "Teste Automatizado LTDA", 5);
                _composicaoBll.AdicionarComposicao(composicao, false);
                composicao.IDStatus = (int)EnumStatusComposicao.Aprovado;
                _composicaoBll.AtualizarComposicao(composicao, false);

                placaTruck.Documentos.ForEach(x =>
                {
                    x.DataVencimento = DateTime.Now.AddDays(-1);
                    _placaDocumentoBll.Atualizar(new PlacaDocumento
                    {
                        ID = x.ID,
                        DataVencimento = x.DataVencimento,
                        IDPlaca = x.IDPlaca,
                        Anexo = x.Anexo,
                        IDTipoDocumento = x.IDTipoDocumento

                    });
                });

                //processar documentos e bloquear documentos e composição
                //var retorno = _placaDocumentoBll.ProcessarDocumentosAVencer(DateTime.Now.ToString("yyyy-MM-dd"));
                //Assert.AreEqual(0, retorno);

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            finally
            {
                _composicaoBll.Excluir(composicao.ID);
                placaTruck.Documentos.ForEach(x => _placaDocumentoBll.Excluir(x.ID));
                _placaBll.Excluir(placaTruck.ID);
                _usuarioTransportadoraBll.Excluir(usuarioTransp);
                _usuarioBll.Excluir(usuario);
                _transportadoraBll.Excluir(transp);
                new TipoDocumentoBusiness().ExcluirLista(d => d.Sigla == siglaTeste);
            }
        }

        //[TestMethod]
        //[TestCategory("Composicao")]
        //public void AprovarComposicao()
        //{

        //    var composicao = new Composicao();
        //    var composicao2 = new Composicao();
        //    var placaCavalo = new Placa();
        //    var placaCarreta = new Placa();
        //    var placaDolly = new Placa();
        //    var placaCarreta2 = new Placa();
        //    try
        //    {
        //        placaCavalo = PlacaBusinessTests.AdicionarPlacas("CAU1000", (int)EnumTipoVeiculo.Cavalo, false, false);
        //        placaCarreta = PlacaBusinessTests.AdicionarPlacas("CAU2000", (int)EnumTipoVeiculo.Carreta, true);
        //        placaDolly = PlacaBusinessTests.AdicionarPlacas("CAU3000", (int)EnumTipoVeiculo.Dolly, false, false);
        //        placaCarreta2 = PlacaBusinessTests.AdicionarPlacas("CAU4000", (int)EnumTipoVeiculo.Carreta);

        //        CriarComposicao(composicao2, placaCavalo, placaCarreta, placaDolly, placaCarreta2, (int)EnumEmpresa.EAB, "CIF",
        //            (int)EnumTipoComposicao.BitremDolly, 18, (int)EnumCategoriaVeiculo.Aluguel, "34428958000108", "Teste Automatizado LTDA", 5);
        //        composicao2.Placa1 = placaCavalo.PlacaVeiculo;
        //        _composicaoBll.AdicionarComposicao(composicao2);

        //        _composicaoBll.ExcluirComposicao(composicao2.ID, false);

        //        placaCavalo = PlacaBusinessTests.AdicionarPlacas("CAU1000", (int)EnumTipoVeiculo.Cavalo, false, false);
        //        placaCarreta = PlacaBusinessTests.AdicionarPlacas("CAU2000", (int)EnumTipoVeiculo.Carreta, true);
        //        placaDolly = PlacaBusinessTests.AdicionarPlacas("CAU3000", (int)EnumTipoVeiculo.Dolly, false, false);
        //        placaCarreta2 = PlacaBusinessTests.AdicionarPlacas("CAU4000", (int)EnumTipoVeiculo.Carreta);

        //        CriarComposicao(composicao, placaCavalo, placaCarreta, placaDolly, placaCarreta2, (int)EnumEmpresa.EAB, "CIF",
        //            (int)EnumTipoComposicao.BitremDolly, 18, (int)EnumCategoriaVeiculo.Aluguel, "34428958000108", "Teste Automatizado LTDA", 5);

        //        var retornoAdicionar = _composicaoBll.AdicionarComposicao(composicao);
        //        Assert.IsTrue(retornoAdicionar);

        //        var comps = new PlacaBusiness().ListarPorComposicaoCapacidade(composicao);

        //        composicao.IDStatus = (int)EnumStatusComposicao.Aprovado;
        //        var retorno = _composicaoBll.AtualizarComposicao(composicao, false);

        //        composicao.IDComposicao = composicao.ID;

        //        composicao.ID = 0;
        //        CriarComposicao(composicao, placaCavalo, placaCarreta, placaDolly, placaCarreta2, (int)EnumEmpresa.Combustiveis, "CIF",
        //            (int)EnumTipoComposicao.BitremDolly, 18, (int)EnumCategoriaVeiculo.Aluguel, "34428958000108", "Teste Automatizado LTDA", 5);

        //        var retornoAdicionar2 = _composicaoBll.AdicionarComposicao(composicao,false);
        //        Assert.IsTrue(retornoAdicionar2);


        //        //teste no serviço de composição
        //        var retornoServico = _composicaoBll.ListarComposicaoServico(new Model.Filtro.ComposicaoServicoFiltro {  DataAtualizacao = DateTime.Now.Date, PlacaVeiculo = placaCavalo.PlacaVeiculo});
        //        Assert.IsNotNull(retornoServico);

        //        var file = _composicaoBll.Exportar(new Model.Filtro.ComposicaoFiltro()
        //        {
        //            IDEmpresa = composicao.IDEmpresa,
        //            IDTipoComposicao = composicao.IDTipoComposicao,
        //            IDStatus = composicao.IDStatus
        //        });

        //        composicao.IDStatus = (int)EnumStatusComposicao.Aprovado;
        //        _composicaoBll.AtualizarComposicao(composicao, true);

        //        composicao.tipoIntegracao = EnumTipoIntegracaoSAP.AprovarCheckList;
        //        _composicaoBll.AtualizarComposicao(composicao, false);

        //        composicao.IDStatus = (int)EnumStatusComposicao.Bloqueado;
        //        _composicaoBll.AtualizarComposicao(composicao, false);

        //    }
        //    catch
        //    {
        //        throw;
        //    }
        //    finally
        //    {
        //        _composicaoBll.ExcluirComposicao(composicao.ID, false);

        //        placaCarreta2.Setas.ForEach(x => _placaSetaBll.Excluir(x.ID));
        //        placaCarreta.Setas.ForEach(x => _placaSetaBll.Excluir(x.ID));

        //        _composicaoBll.Excluir(composicao.IDComposicao.Value);

        //        _placaBll.Excluir(placaCarreta2.ID);
        //        _placaBll.Excluir(placaDolly.ID);
        //        _placaBll.Excluir(placaCarreta.ID);
        //        _placaBll.Excluir(placaCavalo.ID);
        //    }

        //}
        //[TestMethod]
        //[TestCategory("Composicao")]
        //public void ComposicaoPendenteSap()
        //{
        //    var compBll = new ComposicaoBusiness();
        //    var placaBll = new PlacaBusiness();
        //    var placaSetaBll = new PlacaSetaBusiness();
        //    var composicao = new Composicao();
        //    var placaCavalo = new Placa();
        //    var placaCarreta = new Placa();
        //    var placaDolly = new Placa();
        //    var placaCarreta2 = new Placa();
        //    try
        //    {
        //        placaCavalo = PlacaBusinessTests.AdicionarPlacas("CAU1000", (int)EnumTipoVeiculo.Cavalo, false, false);
        //        placaCarreta = PlacaBusinessTests.AdicionarPlacas("CAU2000", (int)EnumTipoVeiculo.Carreta, true);
        //        placaDolly = PlacaBusinessTests.AdicionarPlacas("CAU3000", (int)EnumTipoVeiculo.Dolly, false, false);
        //        placaCarreta2 = PlacaBusinessTests.AdicionarPlacas("CAU4000", (int)EnumTipoVeiculo.Carreta);


        //        CriarComposicao(composicao, placaCavalo, placaCarreta, placaDolly, placaCarreta2, (int)EnumEmpresa.Ambos, "CIF",
        //            (int)EnumTipoComposicao.BitremDolly, 18, (int)EnumCategoriaVeiculo.Aluguel, "34428958000108", "Teste Automatizado LTDA", 5);


        //        compBll.AdicionarComposicao(composicao);

        //        var b = _placaBll.SelecionarPlacaCompleta(new Model.Filtro.ComposicaoFiltro() { Placa = "CAU1000" });
        //        Assert.IsNotNull(b);
        //        var a = _placaBll.ListarPlacaRelatorio(new Model.Filtro.ComposicaoFiltro() { Placa = "CAU1000" });
        //        Assert.IsNotNull(a);

        //        composicao.IDStatus = (int)EnumStatusComposicao.AguardandoAtualizacaoSAP;
        //        compBll.AtualizarComposicao(composicao, false);
        //        var retorno = compBll.IntegrarComposicaoPendenteSap();
        //        Assert.IsTrue(retorno);

        //    }
        //    catch
        //    {

        //    }
        //    finally
        //    {
        //        placaCarreta2.Setas.ForEach(x => placaSetaBll.Excluir(x.ID));
        //        placaCarreta.Setas.ForEach(x => placaSetaBll.Excluir(x.ID));

        //        compBll.Excluir(composicao.ID);

        //        placaBll.Excluir(placaCarreta2.ID);
        //        placaBll.Excluir(placaDolly.ID);
        //        placaBll.Excluir(placaCarreta.ID);
        //        placaBll.ExcluirPlaca(placaCavalo.ID);
        //    }

        //}

        private static void CriarComposicao(Composicao composicao, Placa placaCavalo, Placa placaCarreta, Placa placaDolly,
            Placa placaCarreta2, int idEmpresa, string operacao, int tipoComposicao, int tipoComposicaoEixo,
            int categoriaVeiculo, string cpfCnpj, string razaoSocial, double pbtc)
        {
            composicao.IDEmpresa = idEmpresa;
            composicao.Operacao = operacao;
            composicao.IDTipoComposicao = tipoComposicao;
            //composicao.IDTipoComposicaoEixo = tipoComposicaoEixo;
            composicao.IDCategoriaVeiculo = categoriaVeiculo;
            composicao.IDPlaca1 = placaCavalo.ID;
            composicao.IDPlaca2 = placaCarreta.ID;
            composicao.IDPlaca3 = placaDolly.ID;
            composicao.IDPlaca4 = placaCarreta2.ID;
            composicao.CPFCNPJ = cpfCnpj;
            composicao.RazaoSocial = razaoSocial;
            composicao.DataAtualizacao = DateTime.Now;
            composicao.IDStatus = (int)EnumStatusComposicao.EmAprovacao;
            composicao.PBTC = pbtc;
            composicao.LoginUsuario = "tr009592";
            composicao.TipoContratacao = 2;
        }

        private static void CriarComposicao(Composicao composicao, Placa placaTruck, int idEmpresa, string operacao, int tipoComposicao, int tipoComposicaoEixo,
            int categoriaVeiculo, string cpfCnpj, string razaoSocial, double pbtc)
        {
            composicao.IDEmpresa = idEmpresa;
            composicao.Operacao = operacao;
            composicao.IDTipoComposicao = tipoComposicao;
            //composicao.IDTipoComposicaoEixo = tipoComposicaoEixo;
            composicao.IDCategoriaVeiculo = categoriaVeiculo;
            composicao.IDPlaca1 = placaTruck.ID;
            composicao.CPFCNPJ = cpfCnpj;
            composicao.RazaoSocial = razaoSocial;
            composicao.DataAtualizacao = DateTime.Now;
            composicao.IDStatus = (int)EnumStatusComposicao.EmAprovacao;
            composicao.PBTC = pbtc;
            composicao.LoginUsuario = "tr009592";
            composicao.TipoContratacao = 2;
        }

        [TestMethod]
        [TestCategory("Composicao")]
        public void ExportarComposicao()
        {
            try
            {
                var file = _composicaoBll.Exportar(new Model.Filtro.ComposicaoFiltro()
                {
                    IDEmpresa = (int)EnumEmpresa.Ambos,
                    IDTipoComposicao = (int)EnumTipoComposicao.BitremDolly,
                    IDStatus = (int)EnumStatusComposicao.EmAprovacao
                });
                Assert.IsNotNull(file);
            }
            catch (Exception ex)
            {

            }
        }



        [IgnoreAttribute("Teste desligado até resolver a questão da ordem de execução no Jenkins")]
        [TestMethod()]
        [TestCategory("Composicao")]
        public void ListarComposicaoCountTest()
        {
            var ret = _composicaoPesquisaBll.ListarComposicaoCount(new Model.Filtro.ComposicaoFiltro());
            Assert.IsNotNull(ret);
        }

        [IgnoreAttribute("Teste desligado até resolver a questão da ordem de execução no Jenkins")]
        [TestMethod()]
        [TestCategory("Composicao")]
        public void ListarComposicaoTest()
        {
            var ret = _composicaoPesquisaBll.ListarComposicao(new Model.Filtro.ComposicaoFiltro(), new Framework.Models.PaginadorModel() { PaginaAtual = 1, QtdeItensPagina = 10 });
            Assert.IsNotNull(ret);
        }

        [IgnoreAttribute("Teste desligado até resolver a questão da ordem de execução no Jenkins")]
        [TestMethod()]
        [TestCategory("Composicao")]
        public void SelecionarUfCrlv()
        {
            var carreta = _composicaoBll.Selecionar(p => p.IDTipoComposicao == (int)EnumTipoComposicao.Carreta && p.IDEmpresa == (int)EnumEmpresa.Combustiveis);
            var truck = _composicaoBll.Selecionar(p => p.IDTipoComposicao == (int)EnumTipoComposicao.Truck && p.IDEmpresa == (int)EnumEmpresa.Combustiveis);
            //var eab = _composicaoBll.Selecionar(p => p.IDEmpresa == (int)EnumEmpresa.EAB);
            _composicaoBll.SelecionarUfCRLV(truck);
            _composicaoBll.SelecionarUfCRLV(carreta);
            //_composicaoBll.SelecionarUfCRLV(eab);
            Assert.IsTrue(true);
        }

        #region PodeIntegrarSAPTest

        [TestMethod]
        public void PodeIntegrarSAPTest()
        {
            /* Foi acordado junto ao time de arquitetura da Raizen o uso desta abordagem dos cenários abaixo,
               visto que o Jenkins da Raizen não aceita o uso do atributo [DataRows()] nos testes */

            #region Cenários
            List<PodeIntegrarSAPTest_Cenario> cenarios = new List<PodeIntegrarSAPTest_Cenario>()
            {
                new PodeIntegrarSAPTest_Cenario(EnumPais.Brasil, EnumEmpresa.Combustiveis, true, true, true),
                new PodeIntegrarSAPTest_Cenario(EnumPais.Brasil, EnumEmpresa.Combustiveis, true, false, true),
                new PodeIntegrarSAPTest_Cenario(EnumPais.Brasil, EnumEmpresa.Combustiveis, false, true, true),
                new PodeIntegrarSAPTest_Cenario(EnumPais.Brasil, EnumEmpresa.Combustiveis, false, false, true),
                new PodeIntegrarSAPTest_Cenario(EnumPais.Brasil, EnumEmpresa.Combustiveis, false, null, true),
                new PodeIntegrarSAPTest_Cenario(EnumPais.Brasil, EnumEmpresa.Combustiveis, null, false, true),
                new PodeIntegrarSAPTest_Cenario(EnumPais.Brasil, EnumEmpresa.Combustiveis, null, null, true),
                new PodeIntegrarSAPTest_Cenario(EnumPais.Brasil, EnumEmpresa.EAB, true, true, true),
                new PodeIntegrarSAPTest_Cenario(EnumPais.Brasil, EnumEmpresa.EAB, true, false, false),
                new PodeIntegrarSAPTest_Cenario(EnumPais.Brasil, EnumEmpresa.EAB, false, true, true),
                new PodeIntegrarSAPTest_Cenario(EnumPais.Brasil, EnumEmpresa.EAB, false, false, true),
                new PodeIntegrarSAPTest_Cenario(EnumPais.Brasil, EnumEmpresa.EAB, true, null, false),
                new PodeIntegrarSAPTest_Cenario(EnumPais.Brasil, EnumEmpresa.EAB, false, null, true),
                new PodeIntegrarSAPTest_Cenario(EnumPais.Brasil, EnumEmpresa.EAB, null, false, true),
                new PodeIntegrarSAPTest_Cenario(EnumPais.Brasil, EnumEmpresa.EAB, null, null, true),
                new PodeIntegrarSAPTest_Cenario(EnumPais.Brasil, EnumEmpresa.Ambos, true, true, true),
                new PodeIntegrarSAPTest_Cenario(EnumPais.Brasil, EnumEmpresa.Ambos, true, false, true),
                new PodeIntegrarSAPTest_Cenario(EnumPais.Brasil, EnumEmpresa.Ambos, false, true, true),
                new PodeIntegrarSAPTest_Cenario(EnumPais.Brasil, EnumEmpresa.Ambos, false, false, true),
                new PodeIntegrarSAPTest_Cenario(EnumPais.Brasil, EnumEmpresa.Ambos, false, null, true),
                new PodeIntegrarSAPTest_Cenario(EnumPais.Brasil, EnumEmpresa.Ambos, null, false, true),
                new PodeIntegrarSAPTest_Cenario(EnumPais.Brasil, EnumEmpresa.Ambos, null, null, true),
                new PodeIntegrarSAPTest_Cenario(EnumPais.Argentina, EnumEmpresa.Combustiveis, true, true, true),
                new PodeIntegrarSAPTest_Cenario(EnumPais.Argentina, EnumEmpresa.Combustiveis, true, false, true),
                new PodeIntegrarSAPTest_Cenario(EnumPais.Argentina, EnumEmpresa.Combustiveis, false, true, true),
                new PodeIntegrarSAPTest_Cenario(EnumPais.Argentina, EnumEmpresa.Combustiveis, false, false, true),
                new PodeIntegrarSAPTest_Cenario(EnumPais.Argentina, EnumEmpresa.Combustiveis, false, null, true),
                new PodeIntegrarSAPTest_Cenario(EnumPais.Argentina, EnumEmpresa.Combustiveis, null, false, true),
                new PodeIntegrarSAPTest_Cenario(EnumPais.Argentina, EnumEmpresa.Combustiveis, null, null, true),
                new PodeIntegrarSAPTest_Cenario(EnumPais.Argentina, EnumEmpresa.EAB, true, true, true),
                new PodeIntegrarSAPTest_Cenario(EnumPais.Argentina, EnumEmpresa.EAB, true, false, true),
                new PodeIntegrarSAPTest_Cenario(EnumPais.Argentina, EnumEmpresa.EAB, false, true, true),
                new PodeIntegrarSAPTest_Cenario(EnumPais.Argentina, EnumEmpresa.EAB, false, false, true),
                new PodeIntegrarSAPTest_Cenario(EnumPais.Argentina, EnumEmpresa.EAB, false, null, true),
                new PodeIntegrarSAPTest_Cenario(EnumPais.Argentina, EnumEmpresa.EAB, null, false, true),
                new PodeIntegrarSAPTest_Cenario(EnumPais.Argentina, EnumEmpresa.EAB, null, null, true),
                new PodeIntegrarSAPTest_Cenario(EnumPais.Argentina, EnumEmpresa.Ambos, true, true, true),
                new PodeIntegrarSAPTest_Cenario(EnumPais.Argentina, EnumEmpresa.Ambos, true, false, true),
                new PodeIntegrarSAPTest_Cenario(EnumPais.Argentina, EnumEmpresa.Ambos, false, true, true),
                new PodeIntegrarSAPTest_Cenario(EnumPais.Argentina, EnumEmpresa.Ambos, false, false, true),
                new PodeIntegrarSAPTest_Cenario(EnumPais.Argentina, EnumEmpresa.Ambos, false, null, true),
                new PodeIntegrarSAPTest_Cenario(EnumPais.Argentina, EnumEmpresa.Ambos, null, false, true),
                new PodeIntegrarSAPTest_Cenario(EnumPais.Argentina, EnumEmpresa.Ambos, null, null, true)
            }; 
            #endregion

            foreach (var c in cenarios)
            {
                //Arrange
                ComposicaoBusiness composicaoBusiness = new ComposicaoBusiness(c.Pais);
                PrivateObject obj = new PrivateObject(composicaoBusiness);
                Composicao composicao = new Composicao();
                composicao.IDEmpresa = (int)c.Empresa;
                composicao.IgnorarLeci = c.IgnorarLECI;
                composicao.IgnorarLeciAdm = c.IgnorarLECIAdm;

                //Act
                bool atual = (bool)obj.Invoke("PodeIntegrarSAP", composicao);

                //Assert
                Assert.AreEqual(c.Esperado, atual);
            }
        }

        private class PodeIntegrarSAPTest_Cenario
        {
            public EnumPais Pais { get; set; }
            public EnumEmpresa Empresa { get; set; }
            public bool? IgnorarLECI { get; set; }
            public bool? IgnorarLECIAdm { get; set; }
            public bool Esperado { get; set; }

            public PodeIntegrarSAPTest_Cenario(EnumPais pais, EnumEmpresa empresa, bool? ignorarLECI, bool? ignorarLECIAdm, bool esperado)
            {
                this.Pais = pais;
                this.Empresa = empresa;
                this.IgnorarLECI = ignorarLECI;
                this.IgnorarLECIAdm = ignorarLECIAdm;
                this.Esperado = esperado;
            }
        }

        #endregion

        #region SelecionaPlacaPrincipalFOBTest

        [TestMethod]       
        public void SelecionaPlacaPrincipalFOBTest()
        {
            /* Foi acordado junto ao time de arquitetura da Raizen o uso desta abordagem dos cenários abaixo,
               visto que o Jenkins da Raizen não aceita o uso do atributo [DataRows()] nos testes */

            #region Cenários
            var cenarios = new List<SelecionaPlacaPrincipalFOBTest_Cenario>()
            {
                new SelecionaPlacaPrincipalFOBTest_Cenario(EnumPais.Brasil, EnumTipoComposicao.Truck, EnumEmpresa.Combustiveis, 1, 2, true, 1),
                new SelecionaPlacaPrincipalFOBTest_Cenario(EnumPais.Brasil, EnumTipoComposicao.Truck, EnumEmpresa.Combustiveis, null, 2, true, 2),
                new SelecionaPlacaPrincipalFOBTest_Cenario(EnumPais.Brasil, EnumTipoComposicao.Carreta, EnumEmpresa.Combustiveis, 1, 2, true, 2),
                new SelecionaPlacaPrincipalFOBTest_Cenario(EnumPais.Brasil, EnumTipoComposicao.Carreta, EnumEmpresa.EAB, 1, 2, true, 1),
                new SelecionaPlacaPrincipalFOBTest_Cenario(EnumPais.Brasil, EnumTipoComposicao.Carreta, EnumEmpresa.Ambos, 1, 2, true, 1),
                new SelecionaPlacaPrincipalFOBTest_Cenario(EnumPais.Brasil, EnumTipoComposicao.Carreta, EnumEmpresa.Ambos, null, 2, true, 0),
                new SelecionaPlacaPrincipalFOBTest_Cenario(EnumPais.Brasil, EnumTipoComposicao.Truck, EnumEmpresa.Combustiveis, 1, 2, false, 1),
                new SelecionaPlacaPrincipalFOBTest_Cenario(EnumPais.Brasil, EnumTipoComposicao.Truck, EnumEmpresa.Combustiveis, null, 2, false, 2),
                new SelecionaPlacaPrincipalFOBTest_Cenario(EnumPais.Brasil, EnumTipoComposicao.Carreta, EnumEmpresa.Combustiveis, 1, 2, false, 2),
                new SelecionaPlacaPrincipalFOBTest_Cenario(EnumPais.Brasil, EnumTipoComposicao.Carreta, EnumEmpresa.EAB, 1, 2, false, 1),
                new SelecionaPlacaPrincipalFOBTest_Cenario(EnumPais.Brasil, EnumTipoComposicao.Carreta, EnumEmpresa.Ambos, 1, 2, false, 1),
                new SelecionaPlacaPrincipalFOBTest_Cenario(EnumPais.Brasil, EnumTipoComposicao.Carreta, EnumEmpresa.Ambos, null, 2, false, 0),
                new SelecionaPlacaPrincipalFOBTest_Cenario(EnumPais.Argentina, EnumTipoComposicao.Truck, EnumEmpresa.Combustiveis, 1, 2, true, 1),
                new SelecionaPlacaPrincipalFOBTest_Cenario(EnumPais.Argentina, EnumTipoComposicao.Truck, EnumEmpresa.Combustiveis, null, 2, false, 2),
                new SelecionaPlacaPrincipalFOBTest_Cenario(EnumPais.Argentina, EnumTipoComposicao.Carreta, EnumEmpresa.Ambos, null, 2, true, 0)
            }; 
            #endregion

            foreach (var c in cenarios)
            {
                //Arrange
                ComposicaoBusiness composicaoBusiness = new ComposicaoBusiness(c.Pais);
                Composicao composicao = new Composicao();
                composicao.IDEmpresa = (int)c.Empresa;
                composicao.IDTipoComposicao = (int)c.TipoComposicao;
                composicao.IDPlaca1 = c.IdPlaca1;
                composicao.IDPlaca2 = c.IdPlaca2;

                if (c.GerarObjetoPlaca)
                {
                    composicao.p1 = new Placa() { ID = (c.IdPlaca1 ?? -1) };
                    composicao.p2 = new Placa() { ID = (c.IdPlaca2 ?? -1) };
                }

                Placa placaEsperada = new Placa() { ID = c.IdPlacaEsperada };

                //Act
                Placa placaAtual = composicaoBusiness.SelecionaPlacaPrincipalFOB(composicao);

                //Assert
                Assert.AreEqual(placaEsperada.ID, placaAtual.ID); 
            }
        }

        private class SelecionaPlacaPrincipalFOBTest_Cenario
        {           
            public EnumPais Pais { get; set; }
            public EnumTipoComposicao TipoComposicao { get; set; }
            public EnumEmpresa Empresa { get; set; }
            public int? IdPlaca1 { get; set; }
            public int? IdPlaca2 { get; set; }
            public bool GerarObjetoPlaca { get; set; }
            public int IdPlacaEsperada { get; set; }

            public SelecionaPlacaPrincipalFOBTest_Cenario(EnumPais pais, EnumTipoComposicao tipoComposicao, EnumEmpresa empresa, int? idPlaca1, int? idPlaca2, bool gerarObjetoPlaca, int idPlacaEsperad)
            {
                Pais = pais;
                TipoComposicao = tipoComposicao;
                Empresa = empresa;
                IdPlaca1 = idPlaca1;
                IdPlaca2 = idPlaca2;
                GerarObjetoPlaca = gerarObjetoPlaca;
                IdPlacaEsperada = idPlacaEsperad;
            }
        }

        #endregion

        #region SelecionarVolumeCompartimentoTest

        [TestMethod]
        public void SelecionarVolumeCompartimentoTest()
        {
            /* Foi acordado junto ao time de arquitetura da Raizen o uso desta abordagem dos cenários abaixo,
               visto que o Jenkins da Raizen não aceita o uso do atributo [DataRows()] nos testes */

            #region Cenários
            var cenarios = new List<SelecionarVolumeCompartimentoTest_Cenario>()
            {
                new SelecionarVolumeCompartimentoTest_Cenario
                (
                    cenario: "Cenario01",
                    pais: EnumPais.Brasil,
                    placa: null,
                    valorEsperado: 0m
                ),

                new SelecionarVolumeCompartimentoTest_Cenario
                (
                    cenario: "Cenario02",
                    pais: EnumPais.Brasil,
                    placa: new Placa()
                    {
                        MultiSeta = false,
                        Setas = new List<PlacaSeta>()
                        {
                            new PlacaSeta()
                            {
                                VolumeCompartimento1 = 10m,
                                VolumeCompartimento2 = 20m,
                                VolumeCompartimento3 = 30m
                            }
                        }
                    },
                    valorEsperado: 60m
                ),

                new SelecionarVolumeCompartimentoTest_Cenario
                (
                    cenario: "Cenario03",
                    pais: EnumPais.Brasil,
                    placa: new Placa()
                    {
                        MultiSeta = false,
                        Setas = new List<PlacaSeta>()
                        {
                            new PlacaSeta()
                            {
                                VolumeCompartimento1 = 10m,
                                VolumeCompartimento2 = 20m,
                                VolumeCompartimento3 = null,
                                VolumeCompartimento4 = 30m,
                                VolumeCompartimento5 = null,
                                VolumeCompartimento6 = 50m,
                                VolumeCompartimento7 = 60m,
                                VolumeCompartimento8 = null,
                                VolumeCompartimento9 = 40m,
                                VolumeCompartimento10 = null
                            }
                        }
                    },
                    valorEsperado: 210m
                ),

                new SelecionarVolumeCompartimentoTest_Cenario
                (
                    cenario: "Cenario04",
                    pais: EnumPais.Brasil,
                    placa: new Placa()
                    {
                        MultiSeta = false,
                        Setas = new List<PlacaSeta>()
                        {
                            new PlacaSeta()
                            {
                                VolumeCompartimento1 = 10m,
                                VolumeCompartimento2 = 20m,
                                VolumeCompartimento3 = 30m,
                                VolumeCompartimento4 = 40m,
                                VolumeCompartimento5 = 50m,
                                VolumeCompartimento6 = 60m,
                                VolumeCompartimento7 = 70m,
                                VolumeCompartimento8 = 80m,
                                VolumeCompartimento9 = 90m,
                                VolumeCompartimento10 = 100m
                            }
                        }
                    },
                    valorEsperado: 550m
                ),

                new SelecionarVolumeCompartimentoTest_Cenario
                (
                    cenario: "Cenario05",
                    pais: EnumPais.Brasil,
                    placa: new Placa()
                    {
                        MultiSeta = true,
                        Setas = new List<PlacaSeta>()
                        {
                            new PlacaSeta()
                            {
                                CompartimentoPrincipal1 = true,
                                Compartimento1IsInativo = false,
                                VolumeCompartimento1 = 10m
                            }
                        }
                    },
                    valorEsperado: 10m
                ),

                new SelecionarVolumeCompartimentoTest_Cenario
                (
                    cenario: "Cenario06",
                    pais: EnumPais.Brasil,
                    placa: new Placa()
                    {
                        MultiSeta = true,
                        Setas = new List<PlacaSeta>()
                        {
                            new PlacaSeta()
                            {
                                CompartimentoPrincipal1 = true,
                                Compartimento1IsInativo = false,
                                VolumeCompartimento1 = 10m,
                                CompartimentoPrincipal2 = true,
                                Compartimento2IsInativo = false,
                                VolumeCompartimento2 = 20m,
                                CompartimentoPrincipal3 = true,
                                Compartimento3IsInativo = false,
                                VolumeCompartimento3 = 30m,
                                CompartimentoPrincipal4 = true,
                                Compartimento4IsInativo = false,
                                VolumeCompartimento4 = 40m,
                                CompartimentoPrincipal5 = true,
                                Compartimento5IsInativo = false,
                                VolumeCompartimento5 = 50m,
                                CompartimentoPrincipal6 = true,
                                Compartimento6IsInativo = false,
                                VolumeCompartimento6 = 60m,
                                CompartimentoPrincipal7 = true,
                                Compartimento7IsInativo = false,
                                VolumeCompartimento7 = 70m,
                                CompartimentoPrincipal8 = true,
                                Compartimento8IsInativo = false,
                                VolumeCompartimento8 = 80m,
                                CompartimentoPrincipal9 = true,
                                Compartimento9IsInativo = false,
                                VolumeCompartimento9 = 90m,
                                CompartimentoPrincipal10 = true,
                                Compartimento10IsInativo = false,
                                VolumeCompartimento10 = 100m,

                            }
                        }
                    },
                    valorEsperado: 550m
                ),

                new SelecionarVolumeCompartimentoTest_Cenario
                (
                    cenario: "Cenario07",
                    pais: EnumPais.Brasil,
                    placa: new Placa()
                    {
                        MultiSeta = true,
                        Setas = new List<PlacaSeta>()
                        {
                            new PlacaSeta()
                            {
                                CompartimentoPrincipal1 = false,
                                CompartimentoPrincipal2 = false,
                                CompartimentoPrincipal3 = false,
                                CompartimentoPrincipal4 = false,
                                CompartimentoPrincipal5 = false,
                                CompartimentoPrincipal6 = false,
                                CompartimentoPrincipal7 = false,
                                CompartimentoPrincipal8 = false,
                                CompartimentoPrincipal9 = false,
                                CompartimentoPrincipal10 = false,
                            }
                        }
                    },
                    valorEsperado: 0m
                ),

                new SelecionarVolumeCompartimentoTest_Cenario
                (
                    cenario: "Cenario08",
                    pais: EnumPais.Brasil,
                    placa: new Placa()
                    {
                        MultiSeta = true,
                        Setas = new List<PlacaSeta>()
                        {
                            new PlacaSeta()
                            {
                                CompartimentoPrincipal1 = true,
                                Compartimento1IsInativo = true,
                                CompartimentoPrincipal2 = true,
                                Compartimento2IsInativo = true,
                                CompartimentoPrincipal3 = true,
                                Compartimento3IsInativo = true,
                                CompartimentoPrincipal4 = true,
                                Compartimento4IsInativo = true,
                                CompartimentoPrincipal5 = true,
                                Compartimento5IsInativo = true,
                                CompartimentoPrincipal6 = true,
                                Compartimento6IsInativo = true,
                                CompartimentoPrincipal7 = true,
                                Compartimento7IsInativo = true,
                                CompartimentoPrincipal8 = true,
                                Compartimento8IsInativo = true,
                                CompartimentoPrincipal9 = true,
                                Compartimento9IsInativo = true,
                                CompartimentoPrincipal10 = true,
                                Compartimento10IsInativo = true,

                            }
                        }
                    },
                    valorEsperado: 0m
                )
            }; 
            #endregion

            foreach (var c in cenarios)
            {
                //Arrange
                ComposicaoBusiness composicaoBusiness = new ComposicaoBusiness(c.Pais);

                PrivateObject obj = new PrivateObject(composicaoBusiness);

                //Act
                decimal valorAtual = (decimal)obj.Invoke("SelecionarVolumeCompartimento", c.Placa);

                //Assert
                Assert.AreEqual(c.ValorEsperado, valorAtual); 
            }
        }

        public class SelecionarVolumeCompartimentoTest_Cenario
        {
            public string Cenario { get; set; }

            public EnumPais Pais { get; set; }

            public Placa Placa { get; set; }

            public decimal ValorEsperado { get; set; }

            public SelecionarVolumeCompartimentoTest_Cenario(string cenario, EnumPais pais, Placa placa, decimal valorEsperado)
            {
                Cenario = cenario;
                Pais = pais;
                Placa = placa;
                ValorEsperado = valorEsperado;
            }
        }

        #endregion

        #region CarregarCompartimentos

        [TestMethod]
        public void CarregarCompartimentosTest()
        {
            /* Foi acordado junto ao time de arquitetura da Raizen o uso desta abordagem dos cenários abaixo,
               visto que o Jenkins da Raizen não aceita o uso do atributo [DataRows()] nos testes */
            #region Cenários
            var cenarios = new List<CarregarCompartimentosTest_Cenario>()
            {
                new CarregarCompartimentosTest_Cenario
                (
                    cenario: "Cenario01",
                    pais: EnumPais.Brasil,
                    placa: null,
                    compartimentosEsperados: null
                ),

                new CarregarCompartimentosTest_Cenario
                (
                    cenario: "Cenario02",
                    pais: EnumPais.Brasil,
                    placa: new Placa()
                    {
                        MultiSeta = true,
                        Setas = new List<PlacaSeta>()
                        {
                            new PlacaSeta() { VolumeCompartimento1 = 10, LacreCompartimento1 = 100, CompartimentoPrincipal1 = true, Compartimento1IsInativo = false },
                            new PlacaSeta() { VolumeCompartimento1 = 10, LacreCompartimento1 = 100, CompartimentoPrincipal1 = true, Compartimento1IsInativo = true},
                            new PlacaSeta() { VolumeCompartimento2 = 20, LacreCompartimento2 = 200, CompartimentoPrincipal2 = true, Compartimento2IsInativo = false },
                            new PlacaSeta() { VolumeCompartimento2 = 20, LacreCompartimento2 = 200, CompartimentoPrincipal2 = true, Compartimento2IsInativo = true },
                        }
                    },
                    compartimentosEsperados: new List<CompartimentoView>()
                    {
                        new CompartimentoView()
                        {
                            seq = 1,
                            setas = new List<SetaView>()
                            {
                                new SetaView() { seq = 1, Volume = 10, Lacres = 100, Principal = true },
                            }
                        },
                        new CompartimentoView()
                        {
                            seq = 2,
                            setas = new List<SetaView>()
                            {
                                new SetaView() { seq = 3, Volume = 20, Lacres = 200, Principal = true }
                            }
                        }
                    }
                ),

                new CarregarCompartimentosTest_Cenario
                (
                    cenario: "Cenario03",
                    pais: EnumPais.Brasil,
                    placa: new Placa()
                    {
                        MultiSeta = true,
                        Setas = new List<PlacaSeta>()
                        {
                            new PlacaSeta() { VolumeCompartimento1 = 10, LacreCompartimento1 = 100, CompartimentoPrincipal1 = true, Compartimento1IsInativo = false },
                            new PlacaSeta() { VolumeCompartimento1 = 10, LacreCompartimento1 = 100, CompartimentoPrincipal1 = true, Compartimento1IsInativo = true},
                            new PlacaSeta() { VolumeCompartimento2 = 20, LacreCompartimento2 = 200, CompartimentoPrincipal2 = true, Compartimento2IsInativo = false },
                            new PlacaSeta() { VolumeCompartimento2 = 20, LacreCompartimento2 = 200, CompartimentoPrincipal2 = true, Compartimento2IsInativo = true },
                            new PlacaSeta() { VolumeCompartimento3 = 30, LacreCompartimento3 = 300, CompartimentoPrincipal3 = true, Compartimento3IsInativo = false },
                            new PlacaSeta() { VolumeCompartimento3 = 30, LacreCompartimento3 = 300, CompartimentoPrincipal3 = true, Compartimento3IsInativo = true },
                            new PlacaSeta() { VolumeCompartimento4 = 40, LacreCompartimento4 = 400, CompartimentoPrincipal4 = true, Compartimento4IsInativo = false },
                            new PlacaSeta() { VolumeCompartimento4 = 40, LacreCompartimento4 = 400, CompartimentoPrincipal4 = true, Compartimento4IsInativo = true },
                            new PlacaSeta() { VolumeCompartimento5 = 50, LacreCompartimento5 = 500, CompartimentoPrincipal5 = true, Compartimento5IsInativo = false },
                            new PlacaSeta() { VolumeCompartimento5 = 50, LacreCompartimento5 = 500, CompartimentoPrincipal5 = true, Compartimento5IsInativo = true },
                            new PlacaSeta() { VolumeCompartimento6 = 60, LacreCompartimento6 = 600, CompartimentoPrincipal6 = true, Compartimento6IsInativo = false },
                            new PlacaSeta() { VolumeCompartimento6 = 60, LacreCompartimento6 = 600, CompartimentoPrincipal6 = true, Compartimento6IsInativo = true },
                            new PlacaSeta() { VolumeCompartimento7 = 70, LacreCompartimento7 = 700, CompartimentoPrincipal7 = true, Compartimento7IsInativo = false },
                            new PlacaSeta() { VolumeCompartimento7 = 70, LacreCompartimento7 = 700, CompartimentoPrincipal7 = true, Compartimento7IsInativo = true },
                            new PlacaSeta() { VolumeCompartimento8 = 80, LacreCompartimento8 = 800, CompartimentoPrincipal8 = true, Compartimento8IsInativo = false },
                            new PlacaSeta() { VolumeCompartimento8 = 80, LacreCompartimento8 = 800, CompartimentoPrincipal8 = true, Compartimento8IsInativo = true },
                            new PlacaSeta() { VolumeCompartimento9 = 90, LacreCompartimento9 = 900, CompartimentoPrincipal9 = true, Compartimento9IsInativo = false },
                            new PlacaSeta() { VolumeCompartimento9 = 90, LacreCompartimento9 = 900, CompartimentoPrincipal9 = true, Compartimento9IsInativo = true },
                            new PlacaSeta() { VolumeCompartimento10 = 100, LacreCompartimento10 = 1000, CompartimentoPrincipal10 = true, Compartimento10IsInativo = false },
                            new PlacaSeta() { VolumeCompartimento10 = 100, LacreCompartimento10 = 1000, CompartimentoPrincipal10 = true, Compartimento10IsInativo = true },
                        }
                    },
                    compartimentosEsperados: new List<CompartimentoView>()
                    {
                        new CompartimentoView()
                        {
                            seq = 1,
                            setas = new List<SetaView>() {new SetaView() { seq = 1, Volume = 10, Lacres = 100, Principal = true } }
                        },
                        new CompartimentoView()
                        {
                            seq = 2,
                            setas = new List<SetaView>() { new SetaView() { seq = 3, Volume = 20, Lacres = 200, Principal = true } }
                        },
                        new CompartimentoView()
                        {
                            seq = 3,
                            setas = new List<SetaView>() { new SetaView() { seq = 5, Volume = 30, Lacres = 300, Principal = true } }
                        },
                        new CompartimentoView()
                        {
                            seq = 4,
                            setas = new List<SetaView>() { new SetaView() { seq = 7, Volume = 40, Lacres = 400, Principal = true } }
                        },
                        new CompartimentoView()
                        {
                            seq = 5,
                            setas = new List<SetaView>() { new SetaView() { seq = 9, Volume = 50, Lacres = 500, Principal = true } }
                        },
                        new CompartimentoView()
                        {
                            seq = 6,
                            setas = new List<SetaView>() { new SetaView() { seq = 11, Volume = 60, Lacres = 600, Principal = true } }
                        },
                        new CompartimentoView()
                        {
                            seq = 7,
                            setas = new List<SetaView>() { new SetaView() { seq = 13, Volume = 70, Lacres = 700, Principal = true } }
                        },
                        new CompartimentoView()
                        {
                            seq = 8,
                            setas = new List<SetaView>() { new SetaView() { seq = 15, Volume = 80, Lacres = 800, Principal = true } }
                        },
                        new CompartimentoView()
                        {
                            seq = 9,
                            setas = new List<SetaView>() { new SetaView() { seq = 17, Volume = 90, Lacres = 900, Principal = true } }
                        },
                        new CompartimentoView()
                        {
                            seq = 10,
                            setas = new List<SetaView>() { new SetaView() { seq = 19, Volume = 100, Lacres = 1000, Principal = true } }
                        }
                    }
                )
            }; 
            #endregion

            foreach (var c in cenarios)
            {
                //Arrange
                ComposicaoBusiness composicaoBusiness = new ComposicaoBusiness(c.Pais);
                List<CompartimentoView> compartimentosEsperados = c.CompartimentosEsperados;
                PrivateObject obj = new PrivateObject(composicaoBusiness);

                //Act
                List<CompartimentoView> compartimentos = (List<CompartimentoView>)obj.Invoke("CarregarCompartimentos", c.Placa);

                //Assert
                if (compartimentos != null)
                {
                    for (int i = 0; i < compartimentos.Count; i++)
                    {
                        var compartimento = compartimentos[i];
                        var compartimentoEsperado = compartimentosEsperados[i];

                        Assert.AreEqual(compartimentoEsperado.seq, compartimento.seq, $"Erro no campo [seq] do [{c.Cenario}]");
                        Assert.AreEqual(compartimentoEsperado.setas.Count, compartimento.setas.Count, $"Erro na contagem de setas do [{c.Cenario}]");

                        for (int j = 0; j < compartimento.setas.Count; j++)
                        {
                            var seta = compartimento.setas[j];
                            var setaEsperada = compartimentoEsperado.setas[j];

                            Assert.AreEqual(setaEsperada.seq, seta.seq, $"Erro no campo [seq] do [{c.Cenario}]");
                            Assert.AreEqual(setaEsperada.Volume, seta.Volume, $"Erro no campo [Volume] do [{c.Cenario}]");
                            Assert.AreEqual(setaEsperada.Lacres, seta.Lacres, $"Erro no campo [Lacres] do [{c.Cenario}]");
                            Assert.AreEqual(setaEsperada.Principal, seta.Principal, $"Erro no campo [Principal] do [{c.Cenario}]");
                        }
                    }
                }
                else
                    Assert.AreEqual(compartimentosEsperados, compartimentos); 
            }
        }
       
        public class CarregarCompartimentosTest_Cenario
        {
            public string Cenario { get; set; }

            public EnumPais Pais { get; set; }

            public Placa Placa { get; set; }

            public List<CompartimentoView> CompartimentosEsperados { get; set; }

            public CarregarCompartimentosTest_Cenario(string cenario, EnumPais pais, Placa placa, List<CompartimentoView> compartimentosEsperados)
            {
                Cenario = cenario;
                Pais = pais;
                Placa = placa;
                CompartimentosEsperados = compartimentosEsperados;
            }
        }

        #endregion

        #region SomarVolumesCompartimentos

        [TestMethod]
        public void SomarVolumesCompartimentosTest()
        {
            /* Foi acordado junto ao time de arquitetura da Raizen o uso desta abordagem dos cenários abaixo,
               visto que o Jenkins da Raizen não aceita o uso do atributo [DataRows()] nos testes */
            #region Cenários
            var cenarios = new List<SomarVolumesCompartimentosTest_Cenario>()
            {
                new SomarVolumesCompartimentosTest_Cenario(
                   cenario: "Cenario 1",
                   pais: EnumPais.Brasil,
                   setas: null,
                   resultadosEsperados: null
                   ),

                new SomarVolumesCompartimentosTest_Cenario(
                   cenario: "Cenario 2",
                   pais: EnumPais.Brasil,
                   setas: new List<PlacaSeta>()
                   {
                       new PlacaSeta() { VolumeCompartimento1 = 10, Compartimento1IsInativo = false },
                       new PlacaSeta() { VolumeCompartimento1 = 10, Compartimento1IsInativo = null },
                       new PlacaSeta() { VolumeCompartimento1 = null, Compartimento1IsInativo = true },
                       new PlacaSeta() { VolumeCompartimento1 = null, Compartimento1IsInativo = null }
                   },
                   resultadosEsperados: new List<decimal>()
                   {
                       10,
                       10,
                       0,
                       0
                   }),

                new SomarVolumesCompartimentosTest_Cenario(
                   cenario: "Cenario 3",
                   pais: EnumPais.Brasil,
                   setas: new List<PlacaSeta>()
                   {
                       new PlacaSeta() { VolumeCompartimento1 = 10, Compartimento1IsInativo = false },
                       new PlacaSeta() { VolumeCompartimento2 = 20, Compartimento2IsInativo = false },
                       new PlacaSeta() { VolumeCompartimento3 = 30, Compartimento3IsInativo = false },
                       new PlacaSeta() { VolumeCompartimento4 = 40, Compartimento4IsInativo = false },
                       new PlacaSeta() { VolumeCompartimento5 = 50, Compartimento5IsInativo = false },
                       new PlacaSeta() { VolumeCompartimento6 = 60, Compartimento6IsInativo = false },
                       new PlacaSeta() { VolumeCompartimento7 = 70, Compartimento7IsInativo = false },
                       new PlacaSeta() { VolumeCompartimento8 = 80, Compartimento8IsInativo = false },
                       new PlacaSeta() { VolumeCompartimento9 = 90, Compartimento9IsInativo = false },
                       new PlacaSeta() { VolumeCompartimento10 = 100, Compartimento10IsInativo = false }
                   },
                   resultadosEsperados: new List<decimal>()
                   {
                       10,
                       20,
                       30,
                       40,
                       50,
                       60,
                       70,
                       80,
                       90,
                       100
                   }),

                new SomarVolumesCompartimentosTest_Cenario(
                   cenario: "Cenario 4",
                   pais: EnumPais.Brasil,
                   setas: new List<PlacaSeta>()
                   {
                       new PlacaSeta()
                       {
                           VolumeCompartimento1 = 10, Compartimento1IsInativo = false,
                           VolumeCompartimento2 = 20, Compartimento2IsInativo = false,
                           VolumeCompartimento3 = 30, Compartimento3IsInativo = false,
                           VolumeCompartimento4 = 40, Compartimento4IsInativo = false,
                           VolumeCompartimento5 = 50, Compartimento5IsInativo = false,
                           VolumeCompartimento6 = 60, Compartimento6IsInativo = false,
                           VolumeCompartimento7 = 70, Compartimento7IsInativo = false,
                           VolumeCompartimento8 = 80, Compartimento8IsInativo = false,
                           VolumeCompartimento9 = 90, Compartimento9IsInativo = false,
                           VolumeCompartimento10 = 100, Compartimento10IsInativo = false
                       },
                   },
                   resultadosEsperados: new List<decimal>()
                   {
                       550
                   }),

                new SomarVolumesCompartimentosTest_Cenario(
                   cenario: "Cenario 5",
                   pais: EnumPais.Brasil,
                   setas: new List<PlacaSeta>()
                   {
                       new PlacaSeta()
                       {
                           VolumeCompartimento1 = null,
                           VolumeCompartimento2 = null,
                           VolumeCompartimento3 = null,
                           VolumeCompartimento4 = null,
                           VolumeCompartimento5 = null,
                           VolumeCompartimento6 = null,
                           VolumeCompartimento7 = null,
                           VolumeCompartimento8 = null,
                           VolumeCompartimento9 = null,
                           VolumeCompartimento10 = null
                       },
                   },
                   resultadosEsperados: new List<decimal>()
                   {
                       0
                   }),

                new SomarVolumesCompartimentosTest_Cenario(
                   cenario: "Cenario 6",
                   pais: EnumPais.Brasil,
                   setas: new List<PlacaSeta>()
                   {
                       new PlacaSeta()
                       {
                           VolumeCompartimento1 = 10, Compartimento1IsInativo = null,
                           VolumeCompartimento2 = 20, Compartimento2IsInativo = null,
                           VolumeCompartimento3 = 30, Compartimento3IsInativo = null,
                           VolumeCompartimento4 = 40, Compartimento4IsInativo = null,
                           VolumeCompartimento5 = 50, Compartimento5IsInativo = null,
                           VolumeCompartimento6 = 60, Compartimento6IsInativo = null,
                           VolumeCompartimento7 = 70, Compartimento7IsInativo = null,
                           VolumeCompartimento8 = 80, Compartimento8IsInativo = null,
                           VolumeCompartimento9 = 90, Compartimento9IsInativo = null,
                           VolumeCompartimento10 = 100, Compartimento10IsInativo = null
                       },
                   },
                   resultadosEsperados: new List<decimal>()
                   {
                       550
                   }),

                new SomarVolumesCompartimentosTest_Cenario(
                   cenario: "Cenario 7",
                   pais: EnumPais.Brasil,
                   setas: new List<PlacaSeta>()
                   {
                       new PlacaSeta()
                       {
                           VolumeCompartimento1 = 10, Compartimento1IsInativo = true,
                           VolumeCompartimento2 = 20, Compartimento2IsInativo = true,
                           VolumeCompartimento3 = 30, Compartimento3IsInativo = true,
                           VolumeCompartimento4 = 40, Compartimento4IsInativo = true,
                           VolumeCompartimento5 = 50, Compartimento5IsInativo = true,
                           VolumeCompartimento6 = 60, Compartimento6IsInativo = true,
                           VolumeCompartimento7 = 70, Compartimento7IsInativo = true,
                           VolumeCompartimento8 = 80, Compartimento8IsInativo = true,
                           VolumeCompartimento9 = 90, Compartimento9IsInativo = true,
                           VolumeCompartimento10 = 100, Compartimento10IsInativo = true
                       },
                   },
                   resultadosEsperados: new List<decimal>()
                   {
                       0
                   }),
            }; 
            #endregion

            foreach (var c in cenarios)
            {
                //Arrange
                ComposicaoBusiness composicaoBusiness = new ComposicaoBusiness(c.Pais);
                PrivateObject obj = new PrivateObject(composicaoBusiness);

                //Act
                List<PlacaSeta> setas = (List<PlacaSeta>)obj.Invoke("SomarVolumesCompartimentos", c.Setas);

                //Assert
                if (setas != null)
                {
                    Assert.AreEqual(c.ResultadosEsperados.Count, setas.Count, $"Falha no {c.Cenario}");

                    for (int i = 0; i < setas.Count; i++)
                    {
                        Assert.AreEqual(c.ResultadosEsperados[i], setas[i].VolumeTotalCompartimento, $"Falha no {c.Cenario}");
                    }
                }
                else
                {
                    Assert.IsNull(setas, $"Falha no {c.Cenario}");
                }
            }
        }

        private class SomarVolumesCompartimentosTest_Cenario
        {
            public SomarVolumesCompartimentosTest_Cenario(string cenario, EnumPais pais, List<PlacaSeta> setas, List<decimal> resultadosEsperados)
            {
                Setas = setas;
                Cenario = cenario;
                Pais = pais;
                ResultadosEsperados = resultadosEsperados;
            }

            public List<PlacaSeta> Setas { get; set; }
            
            public List<decimal> ResultadosEsperados { get; set; }

            public string Cenario { get; set; }

            public EnumPais Pais { get; set; }
        }

        #endregion

        #region MontarLinhasPlacas

        [TestMethod]
        public void MontarLinhasPlacasTest()
        {
            /* Foi acordado junto ao time de arquitetura da Raizen o uso desta abordagem dos cenários abaixo,
               visto que o Jenkins da Raizen não aceita o uso do atributo [DataRows()] nos testes */
            #region Cenários

            var dataTable = MontarDataTableFake();

            var cenarios = new List<MontarLinhasPlacas_Cenario>()
            {
                new MontarLinhasPlacas_Cenario(
                    cenario: "Cenário 1",
                    pais: EnumPais.Brasil,
                    worksheet: new XLWorksheetFake(),
                    linha: 1,
                    item: dataTable.Rows[0],
                    tipoDocs: new List<TipoDocumento>()
                    {
                        new TipoDocumento()
                        {
                            Sigla = "CA"
                        }
                    }),

            };

            #endregion

            foreach (var c in cenarios)
            {
                //Arrange
                ComposicaoBusiness composicaoBusiness = new ComposicaoBusiness(c.Pais);
                PrivateObject obj = new PrivateObject(composicaoBusiness);

                //Act
                obj.Invoke("MontarLinhasPlacas", c.Worksheet, c.Linha, c.Item, c.TipoDocs);

                //Assert 
                Assert.IsNotNull(c.Worksheet);
            }
        }

        private DataTable MontarDataTableFake()
        {
            DataTable table = new DataTable();
            table.Columns.Add("Dosage", typeof(int));
            table.Columns.Add("PlacaVeiculo", typeof(string));
            table.Columns.Add("Operacao", typeof(string));
            table.Columns.Add("TipoVeiculo", typeof(string));
            table.Columns.Add("Transportadora", typeof(string));
            table.Columns.Add("Renavam", typeof(string));
            table.Columns.Add("Marca", typeof(string));
            table.Columns.Add("Modelo", typeof(string));
            table.Columns.Add("Chassi", typeof(string));
            table.Columns.Add("AnoFabricacao", typeof(string));
            table.Columns.Add("AnoModelo", typeof(string));
            table.Columns.Add("Cor", typeof(string));
            table.Columns.Add("TipoRastreador", typeof(string));
            table.Columns.Add("NumeroAntena", typeof(string));
            table.Columns.Add("Versao", typeof(string));
            table.Columns.Add("CameraMonitoramento", typeof(bool));
            table.Columns.Add("BombaDescarga", typeof(bool));
            table.Columns.Add("NumeroEixos", typeof(string));
            table.Columns.Add("EixosPneusDuplos", typeof(bool));
            table.Columns.Add("NumeroEixosPneusDuplos", typeof(bool));
            table.Columns.Add("TipoProduto", typeof(string));
            table.Columns.Add("MultiSeta", typeof(bool));
            table.Columns.Add("TipoCarregamento", typeof(string));
            table.Columns.Add("CPFCNPJ", typeof(string));
            table.Columns.Add("DataNascimento", typeof(string));
            table.Columns.Add("RazaoSocial", typeof(string));
            table.Columns.Add("CategoriaVeiculo", typeof(string));
            table.Columns.Add("DataAtualizacao", typeof(string));
            table.Columns.Add("Observacao", typeof(string));
            table.Columns.Add("IDStatus", typeof(int));
            table.Columns.Add("PossuiAbs", typeof(bool));
            table.Columns.Add("Estado", typeof(string));
            table.Columns.Add("VolumeCompartimento1", typeof(string));
            table.Columns.Add("VolumeCompartimento2", typeof(string));
            table.Columns.Add("VolumeCompartimento3", typeof(string));
            table.Columns.Add("VolumeCompartimento4", typeof(string));
            table.Columns.Add("VolumeCompartimento5", typeof(string));
            table.Columns.Add("VolumeCompartimento6", typeof(string));
            table.Columns.Add("VolumeCompartimento7", typeof(string));
            table.Columns.Add("VolumeCompartimento8", typeof(string));
            table.Columns.Add("VolumeCompartimento9", typeof(string));
            table.Columns.Add("VolumeCompartimento10", typeof(string));
            table.Columns.Add("IBM", typeof(string));
            table.Columns.Add("RazaoSocialCliente", typeof(string));
            table.Columns.Add("CA", typeof(string));

            table.Rows.Add(table.NewRow());
            table.Rows[0]["IDStatus"] = 1;

            return table;
        }

        private class MontarLinhasPlacas_Cenario
        {
            public MontarLinhasPlacas_Cenario(string cenario, EnumPais pais, IXLWorksheet worksheet, int linha, DataRow item, List<TipoDocumento> tipoDocs)
            {
                Cenario = cenario;
                Pais = pais;
                Worksheet = worksheet;
                Linha = linha;
                Item = item;
                TipoDocs = tipoDocs;
            }

            public IXLWorksheet Worksheet { get; set; }
            public int Linha { get; set; }
            public DataRow Item { get; set; }
            public List<TipoDocumento> TipoDocs { get; set; }
            public string Cenario { get; set; }
            public EnumPais Pais { get; set; }
        }

        #endregion

        #region MontarLinhasComposicaoArgentina

        [TestMethod]
        public void MontarLinhasComposicaoArgentinaTest()
        {
            /* Foi acordado junto ao time de arquitetura da Raizen o uso desta abordagem dos cenários abaixo,
               visto que o Jenkins da Raizen não aceita o uso do atributo [DataRows()] nos testes */
            #region Cenários

            var cenarios = new List<MontarLinhasComposicaoArgentinaTest_Cenario>()
            {
                new MontarLinhasComposicaoArgentinaTest_Cenario(
                    cenario: "Cenário 1",
                    pais: EnumPais.Brasil,
                    linha: 1,
                    comp: new ComposicaoView()
                    {
                        Operacao = "FOB",
                        TipoComposicao = "Tipo",
                        Placa1 = "PLA0001",
                        Placa2 = "PLA0002",
                        Placa3 = "PLA0003",
                        Placa4 = "PLA0004",
                        CPFCNPJ = "CPFCNPJ",
                        RazaoSocial = "Razao Social",
                        DataAtualizacao = new DateTime(2020, 01, 02),
                        CategoriaVeiculo = "CategoriaVeiculo",
                        CategoriaVeiculoEs = "CategoriaVeiculoEs",
                        IDStatus = 1,
                        PBTC = 10,
                        NumCompartimentos = 2,
                        Bloqueado = true,
                        CapacidadeMaxima = 10
                    },
                    worksheet: new XLWorksheetFake()),

                new MontarLinhasComposicaoArgentinaTest_Cenario(
                    cenario: "Cenário 2",
                    pais: EnumPais.Argentina,
                    linha: 1,
                    comp: new ComposicaoView()
                    {
                        Operacao = "FOB",
                        TipoComposicao = "Tipo",
                        Placa1 = "PLA0001",
                        Placa2 = "PLA0002",
                        Placa3 = "PLA0003",
                        Placa4 = "PLA0004",
                        CPFCNPJ = "CPFCNPJ",
                        RazaoSocial = "Razao Social",
                        DataAtualizacao = new DateTime(2020, 01, 02),
                        CategoriaVeiculo = "CategoriaVeiculo",
                        CategoriaVeiculoEs = "CategoriaVeiculoEs",
                        IDStatus = 1,
                        PBTC = 10,
                        NumCompartimentos = 2,
                        Bloqueado = true,
                        CapacidadeMaxima = 10
                    },
                    worksheet: new XLWorksheetFake()),

                 new MontarLinhasComposicaoArgentinaTest_Cenario(
                    cenario: "Cenário 3",
                    pais: EnumPais.Brasil,
                    linha: 1,
                    comp: new ComposicaoView()
                    {
                        Operacao = "FOB",
                        TipoComposicao = "Tipo",
                        Placa1 = "PLA0001",
                        Placa2 = "PLA0002",
                        Placa3 = "PLA0003",
                        Placa4 = "PLA0004",
                        CPFCNPJ = "CPFCNPJ",
                        RazaoSocial = "Razao Social",
                        DataAtualizacao = new DateTime(2020, 01, 02),
                        CategoriaVeiculo = "CategoriaVeiculo",
                        CategoriaVeiculoEs = "CategoriaVeiculoEs",
                        IDStatus = 1,
                        PBTC = 10,
                        NumCompartimentos = 2,
                        Bloqueado = false,
                        CapacidadeMaxima = 10
                    },
                    worksheet: new XLWorksheetFake()),

                 new MontarLinhasComposicaoArgentinaTest_Cenario(
                    cenario: "Cenário 4",
                    pais: EnumPais.Argentina,
                    linha: 1,
                    comp: new ComposicaoView()
                    {
                        Operacao = "FOB",
                        TipoComposicao = "Tipo",
                        Placa1 = "PLA0001",
                        Placa2 = "PLA0002",
                        Placa3 = "PLA0003",
                        Placa4 = "PLA0004",
                        CPFCNPJ = "CPFCNPJ",
                        RazaoSocial = "Razao Social",
                        DataAtualizacao = new DateTime(2020, 01, 02),
                        CategoriaVeiculo = "CategoriaVeiculo",
                        CategoriaVeiculoEs = "CategoriaVeiculoEs",
                        IDStatus = 1,
                        PBTC = 10,
                        NumCompartimentos = 2,
                        Bloqueado = false,
                        CapacidadeMaxima = 10
                    },
                    worksheet: new XLWorksheetFake())
            };

            #endregion

            foreach (var c in cenarios)
            {
                //Arrange
                ComposicaoBusiness composicaoBusiness = new ComposicaoBusiness(c.Pais);
                PrivateObject obj = new PrivateObject(composicaoBusiness);

                //Act
                obj.Invoke("MontarLinhasComposicaoArgentina", c.Worksheet, c.Linha, c.Comp, c.Pais);

                //Assert 
                Assert.IsNotNull(c.Worksheet);
            }
        }

        private class MontarLinhasComposicaoArgentinaTest_Cenario
        {
            public MontarLinhasComposicaoArgentinaTest_Cenario(string cenario, EnumPais pais, int linha, ComposicaoView comp, IXLWorksheet worksheet)
            {
                Cenario = cenario;
                Pais = pais;
                Linha = linha;
                Comp = comp;
                Worksheet = worksheet;
            }

            public int Linha { get; set; }
            public string Cenario { get; set; }
            public EnumPais Pais { get; set; }
            public ComposicaoView Comp { get; set; }
            public IXLWorksheet Worksheet { get; set; }
    }

        #endregion

        #region MontarColunasComposicaoArgentina

        [TestMethod]
        public void MontarColunasComposicaoArgentinaTest()
        {
            /* Foi acordado junto ao time de arquitetura da Raizen o uso desta abordagem dos cenários abaixo,
               visto que o Jenkins da Raizen não aceita o uso do atributo [DataRows()] nos testes */
            #region Cenários

            var cenarios = new List<MontarColunasComposicaoArgentinaTest_Cenario>()
            {
                new MontarColunasComposicaoArgentinaTest_Cenario(
                    cenario: "Cenário 1",
                    pais: EnumPais.Brasil,
                    worksheet: new XLWorksheetFake()),

                new MontarColunasComposicaoArgentinaTest_Cenario(
                    cenario: "Cenário 2",
                    pais: EnumPais.Argentina,
                    worksheet: new XLWorksheetFake()),
            };

            #endregion

            foreach (var c in cenarios)
            {
                //Arrange
                ComposicaoBusiness composicaoBusiness = new ComposicaoBusiness(c.Pais);
                PrivateObject obj = new PrivateObject(composicaoBusiness);

                //Act
                obj.Invoke("MontarColunasComposicaoArgentina", c.Worksheet, c.Pais);

                //Assert 
                Assert.IsNotNull(c.Worksheet);
            }
        }

        private class MontarColunasComposicaoArgentinaTest_Cenario
        {
            public MontarColunasComposicaoArgentinaTest_Cenario(string cenario, EnumPais pais, IXLWorksheet worksheet)
            {
                Cenario = cenario;
                Pais = pais;
                Worksheet = worksheet;
            }

            public string Cenario { get; set; }
            public EnumPais Pais { get; set; }
            public IXLWorksheet Worksheet;
        }

        #endregion

        #region MontarLinhasComposicao

        [TestMethod]
        public void MontarLinhasComposicaoTest()
        {
            /* Foi acordado junto ao time de arquitetura da Raizen o uso desta abordagem dos cenários abaixo,
               visto que o Jenkins da Raizen não aceita o uso do atributo [DataRows()] nos testes */
            #region Cenários

            var cenarios = new List<MontarLinhasComposicaoTest_Cenario>()
            {
                new MontarLinhasComposicaoTest_Cenario(
                    cenario: "Cenário 1",
                    pais: EnumPais.Brasil,
                    linha: 1,
                    comp: new ComposicaoView()
                    {
                        Operacao = "FOB",
                        TipoComposicao = "Tipo",
                        Placa1 = "PLA0001",
                        Placa2 = "PLA0002",
                        Placa3 = "PLA0003",
                        Placa4 = "PLA0004",
                        CPFCNPJ = "CPFCNPJ",
                        RazaoSocial = "Razao Social",
                        DataNascimento = new DateTime(1980, 01, 02),
                        DataAtualizacao = new DateTime(2020, 01, 02),
                        CategoriaVeiculo = "CategoriaVeiculo",
                        CategoriaVeiculoEs = "CategoriaVeiculoEs",
                        IDStatus = 1,
                        PBTC = 10,
                        NumCompartimentos = 2,
                        Bloqueado = true,
                        CapacidadeMaxima = 10
                    },
                    worksheet: new XLWorksheetFake(),
                    usuarioExterno: true),

                new MontarLinhasComposicaoTest_Cenario(
                    cenario: "Cenário 2",
                    pais: EnumPais.Argentina,
                    linha: 1,
                    comp: new ComposicaoView()
                    {
                        Operacao = "FOB",
                        TipoComposicao = "Tipo",
                        Placa1 = "PLA0001",
                        Placa2 = "PLA0002",
                        Placa3 = "PLA0003",
                        Placa4 = "PLA0004",
                        CPFCNPJ = "CPFCNPJ",
                        DataNascimento = new DateTime(1980, 01, 02),
                        RazaoSocial = "Razao Social",
                        DataAtualizacao = new DateTime(2020, 01, 02),
                        CategoriaVeiculo = "CategoriaVeiculo",
                        CategoriaVeiculoEs = "CategoriaVeiculoEs",
                        IDStatus = 1,
                        PBTC = 10,
                        NumCompartimentos = 2,
                        Bloqueado = true,
                        CapacidadeMaxima = 10
                    },
                    worksheet: new XLWorksheetFake(),
                    usuarioExterno: true),

                 new MontarLinhasComposicaoTest_Cenario(
                    cenario: "Cenário 3",
                    pais: EnumPais.Brasil,
                    linha: 1,
                    comp: new ComposicaoView()
                    {
                        Operacao = "FOB",
                        TipoComposicao = "Tipo",
                        Placa1 = "PLA0001",
                        Placa2 = "PLA0002",
                        Placa3 = "PLA0003",
                        Placa4 = "PLA0004",
                        CPFCNPJ = "CPFCNPJ",
                        DataNascimento = new DateTime(1980, 01, 02),
                        RazaoSocial = "Razao Social",
                        DataAtualizacao = new DateTime(2020, 01, 02),
                        CategoriaVeiculo = "CategoriaVeiculo",
                        CategoriaVeiculoEs = "CategoriaVeiculoEs",
                        IDStatus = 1,
                        PBTC = 10,
                        NumCompartimentos = 2,
                        Bloqueado = false,
                        CapacidadeMaxima = 10
                    },
                    worksheet: new XLWorksheetFake(),
                    usuarioExterno: true),

                 new MontarLinhasComposicaoTest_Cenario(
                    cenario: "Cenário 4",
                    pais: EnumPais.Argentina,
                    linha: 1,
                    comp: new ComposicaoView()
                    {
                        Operacao = "FOB",
                        TipoComposicao = "Tipo",
                        Placa1 = "PLA0001",
                        Placa2 = "PLA0002",
                        Placa3 = "PLA0003",
                        Placa4 = "PLA0004",
                        CPFCNPJ = "CPFCNPJ",
                        DataNascimento = new DateTime(1980, 01, 02),
                        RazaoSocial = "Razao Social",
                        DataAtualizacao = new DateTime(2020, 01, 02),
                        CategoriaVeiculo = "CategoriaVeiculo",
                        CategoriaVeiculoEs = "CategoriaVeiculoEs",
                        IDStatus = 1,
                        PBTC = 10,
                        NumCompartimentos = 2,
                        Bloqueado = false,
                        CapacidadeMaxima = 10
                    },
                    worksheet: new XLWorksheetFake(),
                    usuarioExterno: true),

                 new MontarLinhasComposicaoTest_Cenario(
                    cenario: "Cenário 5",
                    pais: EnumPais.Argentina,
                    linha: 1,
                    comp: new ComposicaoView()
                    {
                        Operacao = "FOB",
                        TipoComposicao = "Tipo",
                        Placa1 = "PLA0001",
                        Placa2 = "PLA0002",
                        Placa3 = "PLA0003",
                        Placa4 = "PLA0004",
                        CPFCNPJ = "CPFCNPJ",
                        DataNascimento = new DateTime(1980, 01, 02),
                        RazaoSocial = "Razao Social",
                        DataAtualizacao = new DateTime(2020, 01, 02),
                        CategoriaVeiculo = "CategoriaVeiculo",
                        CategoriaVeiculoEs = "CategoriaVeiculoEs",
                        IDStatus = 1,
                        PBTC = 10,
                        NumCompartimentos = 2,
                        Bloqueado = false,
                        CapacidadeMaxima = 10
                    },
                    worksheet: new XLWorksheetFake(),
                    usuarioExterno: false)
            };

            #endregion

            foreach (var c in cenarios)
            {
                //Arrange
                ComposicaoBusiness composicaoBusiness = new ComposicaoBusiness(c.Pais);
                PrivateObject obj = new PrivateObject(composicaoBusiness);

                //Act
                obj.Invoke("MontarLinhasComposicao", c.Worksheet, c.Linha, c.Comp, c.UsuarioExterno, c.Pais);

                //Assert 
                Assert.IsNotNull(c.Worksheet);
            }
        }

        private class MontarLinhasComposicaoTest_Cenario
        {
            public MontarLinhasComposicaoTest_Cenario(string cenario, EnumPais pais, int linha, ComposicaoView comp, IXLWorksheet worksheet, bool usuarioExterno)
            {
                Cenario = cenario;
                Pais = pais;
                Linha = linha;
                Comp = comp;
                Worksheet = worksheet;
                UsuarioExterno = usuarioExterno;
            }

            public int Linha { get; set; }
            public string Cenario { get; set; }
            public EnumPais Pais { get; set; }
            public ComposicaoView Comp { get; set; }
            public IXLWorksheet Worksheet { get; set; }
            public bool UsuarioExterno { get; set; }
        }

        #endregion

        #region MontarMensagemPermissaoCarregamento


        [TestMethod]
        public void MontarMensagemPermissaoCarregamentoTest()
        {
            /* Foi acordado junto ao time de arquitetura da Raizen o uso desta abordagem dos cenários abaixo,
               visto que o Jenkins da Raizen não aceita o uso do atributo [DataRows()] nos testes */
            #region Cenários

            var cenarios = new List<MontarMensagemPermissaoCarregamentoTest_Cenario>()
            {
                 new MontarMensagemPermissaoCarregamentoTest_Cenario(
                    cenario: "Cenário 1",
                    resultadoEsperado: "<b>Seta 1</b><br/><b>Produto 1 </b><font color=\"#31698A\">Permitido</font> o carregamento do produto para seta informada.<br/><br/>",                                        
                    pais: EnumPais.Brasil,
                    permissaoCarregamento: new StringBuilder(),
                    numeroSeta: 1,
                    seta: new PlacaSeta
                    {
                        Produtos = new List<Produto>
                        {
                            new Produto
                            {
                                Situacao = (int)EnumSituacaoPlacaLimite.Permitido,
                                Nome = "Produto 1"
                            }
                        }
                    }),

                 new MontarMensagemPermissaoCarregamentoTest_Cenario(
                    cenario: "Cenário 2",
                    resultadoEsperado: "<b>Seta 1</b><br/><b>Produto 1 </b><font color=\"#31698A\">Permitido</font> o carregamento do produto para seta informada.<br/><b>Produto 2 </b><font color=\"#31698A\">Permitido</font> o carregamento do produto para seta informada.<br/><br/>",                                        
                    pais: EnumPais.Brasil,
                    permissaoCarregamento: new StringBuilder(),
                    numeroSeta: 1,
                    seta: new PlacaSeta
                    {
                        Produtos = new List<Produto>
                        {
                            new Produto
                            {
                                Situacao = (int)EnumSituacaoPlacaLimite.Permitido,
                                Nome = "Produto 1"
                            },
                            new Produto
                            {
                                Situacao = (int)EnumSituacaoPlacaLimite.Permitido,
                                Nome = "Produto 2"
                            }
                        }
                    }),

                  new MontarMensagemPermissaoCarregamentoTest_Cenario(
                    cenario: "Cenário 3",
                    resultadoEsperado: "<b>Seta 1</b><br/><b>Produto 1 </b><font color=\"#31698A\">Permitido</font> o carregamento do produto para seta informada.<br/><b>Produto 2 </b><font color=\"#CC0000\">Nao Permitido</font> o carregamento do produto para seta informada em virtude de excesso de peso.<br/><b>Produto 3 </b><font color=\"#F8551F\">No Limite</font> o carregamento do produto para seta informada.<br/><br/>",
                    pais: EnumPais.Brasil,
                    permissaoCarregamento: new StringBuilder(),
                    numeroSeta: 1,
                    seta: new PlacaSeta
                    {
                        Produtos = new List<Produto>
                        {
                            new Produto
                            {
                                Situacao = (int)EnumSituacaoPlacaLimite.Permitido,
                                Nome = "Produto 1"
                            },
                            new Produto
                            {
                                Situacao = (int)EnumSituacaoPlacaLimite.NaoPermitido,
                                Nome = "Produto 2"
                            },
                            new Produto
                            {
                                Situacao = (int)EnumSituacaoPlacaLimite.NoLimite,
                                Nome = "Produto 3"
                            }
                        }
                    }),

                  new MontarMensagemPermissaoCarregamentoTest_Cenario(
                    cenario: "Cenário 4",
                    resultadoEsperado: "<b>Flecha 1</b><br/><b>Produto 1 </b><font color=\"#31698A\">Permitido</font> cargar el producto para la flecha informada.<br/><br/>",
                    pais: EnumPais.Argentina,
                    permissaoCarregamento: new StringBuilder(),
                    numeroSeta: 1,
                    seta: new PlacaSeta
                    {
                        Produtos = new List<Produto>
                        {
                            new Produto
                            {
                                Situacao = (int)EnumSituacaoPlacaLimite.Permitido,
                                Nome = "Produto 1"
                            }                          
                        }
                    }),

                  new MontarMensagemPermissaoCarregamentoTest_Cenario(
                    cenario: "Cenário 5",
                    resultadoEsperado: "<b>Flecha 1</b><br/><b>Produto 1 </b><font color=\"#31698A\">Permitido</font> cargar el producto para la flecha informada.<br/><b>Produto 2 </b><font color=\"#31698A\">Permitido</font> cargar el producto para la flecha informada.<br/><br/>",
                    pais: EnumPais.Argentina,
                    permissaoCarregamento: new StringBuilder(),
                    numeroSeta: 1,
                    seta: new PlacaSeta
                    {
                        Produtos = new List<Produto>
                        {
                            new Produto
                            {
                                Situacao = (int)EnumSituacaoPlacaLimite.Permitido,
                                Nome = "Produto 1"
                            },
                            new Produto
                            {
                                Situacao = (int)EnumSituacaoPlacaLimite.Permitido,
                                Nome = "Produto 2"
                            }
                        }
                    }),

                  new MontarMensagemPermissaoCarregamentoTest_Cenario(
                    cenario: "Cenário 6",
                    resultadoEsperado: "<b>Flecha 1</b><br/><b>Produto 1 </b><font color=\"#31698A\">Permitido</font> cargar el producto para la flecha informada.<br/><b>Produto 2 </b><font color=\"#CC0000\">No se permite</font> cargar el producto para la flecha informada debido al exceso de peso.<br/><b>Produto 3 </b><font color=\"#F8551F\">En el limite</font> cargar el producto para la flecha informada.<br/><br/>",
                    pais: EnumPais.Argentina,
                    permissaoCarregamento: new StringBuilder(),
                    numeroSeta: 1,
                    seta: new PlacaSeta
                    {
                        Produtos = new List<Produto>
                        {
                            new Produto
                            {
                                Situacao = (int)EnumSituacaoPlacaLimite.Permitido,
                                Nome = "Produto 1"
                            },
                            new Produto
                            {
                                Situacao = (int)EnumSituacaoPlacaLimite.NaoPermitido,
                                Nome = "Produto 2"
                            },
                            new Produto
                            {
                                Situacao = (int)EnumSituacaoPlacaLimite.NoLimite,
                                Nome = "Produto 3"
                            }
                        }
                    }),
            };

            #endregion

            foreach (var c in cenarios)
            {
                //Arrange
                ComposicaoBusiness composicaoBusiness = new ComposicaoBusiness(c.Pais);
                PrivateObject obj = new PrivateObject(composicaoBusiness);

                //Act
                obj.Invoke("MontarMensagemPermissaoCarregamento", c.PermissaoCarregamento, c.NumeroSeta, c.Seta, false);
                var result = c.PermissaoCarregamento.ToString();
                
                //Assert 
                Assert.AreEqual(c.ResultadoEsperado, result, $"Erro no {c.Cenario}");
            }
        }

        private class MontarMensagemPermissaoCarregamentoTest_Cenario
        {
            public MontarMensagemPermissaoCarregamentoTest_Cenario(string cenario, EnumPais pais, StringBuilder permissaoCarregamento, int numeroSeta, PlacaSeta seta, string resultadoEsperado)
            {
                Cenario = cenario;
                Pais = pais;
                PermissaoCarregamento = permissaoCarregamento;
                NumeroSeta = numeroSeta;
                Seta = seta;
                ResultadoEsperado = resultadoEsperado;

                if (Pais == EnumPais.Brasil)
                    Idioma = "pt-BR";
                else if (Pais == EnumPais.Argentina)
                    Idioma = "es-AR";
            }

            public string Cenario { get; set; }
            public EnumPais Pais { get; set; }
            public StringBuilder PermissaoCarregamento { get; set; }
            public int NumeroSeta { get; set; }
            public PlacaSeta Seta { get; set; }
            public string ResultadoEsperado { get; set; }
            public string Idioma { get; }
        }

        #endregion

        [TestMethod]
        [TestCategory("Composicao")]
        public void NaoPodeAprovarComposicaoAutomaticamenteComPlacaAprovadaSemPendencias()
        {
            var composicao = new Composicao();
            var placaAprovada = FuncoesCompartilhadasTests.GetPlacaBrasil01(
                "ZZZ1111",
                (int)EnumTipoVeiculo.Truck,
                comSetaPadrao: false);
            placaAprovada.ID = 753329;

            composicao.IDEmpresa = 1;
            composicao.Operacao = "FOB";
            composicao.IDTipoComposicao = (int)EnumTipoComposicao.Truck;
            composicao.IDCategoriaVeiculo = (int)EnumCategoriaVeiculo.Particular;
            composicao.IDPlaca1 = placaAprovada.ID;
            composicao.Placa1 = placaAprovada.PlacaVeiculo;
            composicao.CPFCNPJ = "1111";
            composicao.RazaoSocial = "Teste";
            composicao.DataAtualizacao = DateTime.Now;
            composicao.IDStatus = (int)EnumStatusComposicao.EmAprovacao;
            composicao.PBTC = 10;
            composicao.LoginUsuario = "tr009592";
            composicao.TipoContratacao = 2;

            var podeAprovar = _composicaoBll.PodeAprovarComposicaoAutomaticamente(composicao);

            Assert.IsFalse(podeAprovar);
        }

        [TestMethod]
        [TestCategory("Composicao")]
        public void NaoPodeAprovarComposicaoComPlacaNaoAprovada()
        {
            var composicao = new Composicao();
            var placaNaoAprovada = FuncoesCompartilhadasTests.GetPlacaBrasil01(
                "0AI6232",
                (int)EnumTipoVeiculo.Truck,
                comSetaPadrao: false);
            placaNaoAprovada.ID = 707349;

            composicao.IDEmpresa = 1;
            composicao.Operacao = "FOB";
            composicao.IDTipoComposicao = (int)EnumTipoComposicao.Truck;
            composicao.IDCategoriaVeiculo = (int)EnumCategoriaVeiculo.Particular;
            composicao.IDPlaca1 = placaNaoAprovada.ID;
            composicao.Placa1 = placaNaoAprovada.PlacaVeiculo;
            composicao.CPFCNPJ = "1111";
            composicao.RazaoSocial = "Teste";
            composicao.DataAtualizacao = DateTime.Now;
            composicao.IDStatus = (int)EnumStatusComposicao.EmAprovacao;
            composicao.PBTC = 10;
            composicao.LoginUsuario = "tr009592";
            composicao.TipoContratacao = 2;

            var podeAprovar = _composicaoBll.PodeAprovarComposicaoAutomaticamente(composicao);

            Assert.IsFalse(podeAprovar);
        }

        [TestMethod]
        [TestCategory("Composicao")]
        public void NaoPodeAprovarComposicaoComPlacaComDocumentoPendente()
        {
            var composicao = new Composicao();
            var placaDocumentoPendente = FuncoesCompartilhadasTests.GetPlacaBrasil01(
                "HFD4524",
                (int)EnumTipoVeiculo.Truck,
                comSetaPadrao: false);
            placaDocumentoPendente.ID = 175;

            composicao.IDEmpresa = 1;
            composicao.Operacao = "FOB";
            composicao.IDTipoComposicao = (int)EnumTipoComposicao.Truck;
            composicao.IDCategoriaVeiculo = (int)EnumCategoriaVeiculo.Particular;
            composicao.IDPlaca1 = placaDocumentoPendente.ID;
            composicao.Placa1 = placaDocumentoPendente.PlacaVeiculo;
            composicao.CPFCNPJ = "1111";
            composicao.RazaoSocial = "Teste";
            composicao.DataAtualizacao = DateTime.Now;
            composicao.IDStatus = (int)EnumStatusComposicao.EmAprovacao;
            composicao.PBTC = 10;
            composicao.LoginUsuario = "tr009592";
            composicao.TipoContratacao = 2;

            var podeAprovar = _composicaoBll.PodeAprovarComposicaoAutomaticamente(composicao);

            Assert.IsFalse(podeAprovar);
        }
    }
}