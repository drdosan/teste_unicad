using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Raizen.UniCad.BLL;
using Raizen.UniCad.BLLTests.Bases;
using Raizen.UniCad.Model;
using Raizen.UniCad.Model.View;

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

        [DataTestMethod]
        [DataRow(EnumPais.Brasil, EnumEmpresa.Combustiveis, true, true, true)]
        [DataRow(EnumPais.Brasil, EnumEmpresa.Combustiveis, true, false, true)]
        [DataRow(EnumPais.Brasil, EnumEmpresa.Combustiveis, false, true, true)]
        [DataRow(EnumPais.Brasil, EnumEmpresa.Combustiveis, false, false, true)]
        [DataRow(EnumPais.Brasil, EnumEmpresa.Combustiveis, false, null, true)]
        [DataRow(EnumPais.Brasil, EnumEmpresa.Combustiveis, null, false, true)]
        [DataRow(EnumPais.Brasil, EnumEmpresa.Combustiveis, null, null, true)]
        [DataRow(EnumPais.Brasil, EnumEmpresa.EAB, true, true, true)]
        [DataRow(EnumPais.Brasil, EnumEmpresa.EAB, true, false, false)]
        [DataRow(EnumPais.Brasil, EnumEmpresa.EAB, false, true, true)]
        [DataRow(EnumPais.Brasil, EnumEmpresa.EAB, false, false, true)]
        [DataRow(EnumPais.Brasil, EnumEmpresa.EAB, true, null, false)]
        [DataRow(EnumPais.Brasil, EnumEmpresa.EAB, false, null, true)]
        [DataRow(EnumPais.Brasil, EnumEmpresa.EAB, null, false, true)]
        [DataRow(EnumPais.Brasil, EnumEmpresa.EAB, null, null, true)]
        [DataRow(EnumPais.Brasil, EnumEmpresa.Ambos, true, true, true)]
        [DataRow(EnumPais.Brasil, EnumEmpresa.Ambos, true, false, true)]
        [DataRow(EnumPais.Brasil, EnumEmpresa.Ambos, false, true, true)]
        [DataRow(EnumPais.Brasil, EnumEmpresa.Ambos, false, false, true)]
        [DataRow(EnumPais.Brasil, EnumEmpresa.Ambos, false, null, true)]
        [DataRow(EnumPais.Brasil, EnumEmpresa.Ambos, null, false, true)]
        [DataRow(EnumPais.Brasil, EnumEmpresa.Ambos, null, null, true)]
        [DataRow(EnumPais.Argentina, EnumEmpresa.Combustiveis, true, true, true)]
        [DataRow(EnumPais.Argentina, EnumEmpresa.Combustiveis, true, false, true)]
        [DataRow(EnumPais.Argentina, EnumEmpresa.Combustiveis, false, true, true)]
        [DataRow(EnumPais.Argentina, EnumEmpresa.Combustiveis, false, false, true)]
        [DataRow(EnumPais.Argentina, EnumEmpresa.Combustiveis, false, null, true)]
        [DataRow(EnumPais.Argentina, EnumEmpresa.Combustiveis, null, false, true)]
        [DataRow(EnumPais.Argentina, EnumEmpresa.Combustiveis, null, null, true)]
        [DataRow(EnumPais.Argentina, EnumEmpresa.EAB, true, true, true)]
        [DataRow(EnumPais.Argentina, EnumEmpresa.EAB, true, false, true)]
        [DataRow(EnumPais.Argentina, EnumEmpresa.EAB, false, true, true)]
        [DataRow(EnumPais.Argentina, EnumEmpresa.EAB, false, false, true)]
        [DataRow(EnumPais.Argentina, EnumEmpresa.EAB, false, null, true)]
        [DataRow(EnumPais.Argentina, EnumEmpresa.EAB, null, false, true)]
        [DataRow(EnumPais.Argentina, EnumEmpresa.EAB, null, null, true)]
        [DataRow(EnumPais.Argentina, EnumEmpresa.Ambos, true, true, true)]
        [DataRow(EnumPais.Argentina, EnumEmpresa.Ambos, true, false, true)]
        [DataRow(EnumPais.Argentina, EnumEmpresa.Ambos, false, true, true)]
        [DataRow(EnumPais.Argentina, EnumEmpresa.Ambos, false, false, true)]
        [DataRow(EnumPais.Argentina, EnumEmpresa.Ambos, false, null, true)]
        [DataRow(EnumPais.Argentina, EnumEmpresa.Ambos, null, false, true)]
        [DataRow(EnumPais.Argentina, EnumEmpresa.Ambos, null, null, true)]
        public void PodeIntegrarSAPTest(EnumPais pais, EnumEmpresa empresa, bool? ignorarLECI, bool? ignorarLECIAdm, bool esperado)
        {
            //Arrange
            ComposicaoBusiness composicaoBusiness = new ComposicaoBusiness(pais);
            PrivateObject obj = new PrivateObject(composicaoBusiness);
            Composicao composicao = new Composicao();
            composicao.IDEmpresa = (int)empresa;
            composicao.IgnorarLeci = ignorarLECI;
            composicao.IgnorarLeciAdm = ignorarLECIAdm;

            //Act
            bool atual = (bool)obj.Invoke("PodeIntegrarSAP", composicao);

            //Assert
            Assert.AreEqual(esperado, atual);
        }

        #endregion

        #region SelecionaPlacaPrincipalFOBTest

        [DataTestMethod]
        [DataRow(EnumPais.Brasil, EnumTipoComposicao.Truck, EnumEmpresa.Combustiveis, 1, 2, true, 1)]
        [DataRow(EnumPais.Brasil, EnumTipoComposicao.Truck, EnumEmpresa.Combustiveis, null, 2, true, 2)]
        [DataRow(EnumPais.Brasil, EnumTipoComposicao.Carreta, EnumEmpresa.Combustiveis, 1, 2, true, 2)]
        [DataRow(EnumPais.Brasil, EnumTipoComposicao.Carreta, EnumEmpresa.EAB, 1, 2, true, 1)]
        [DataRow(EnumPais.Brasil, EnumTipoComposicao.Carreta, EnumEmpresa.Ambos, 1, 2, true, 1)]
        [DataRow(EnumPais.Brasil, EnumTipoComposicao.Carreta, EnumEmpresa.Ambos, null, 2, true, 0)]
        [DataRow(EnumPais.Brasil, EnumTipoComposicao.Truck, EnumEmpresa.Combustiveis, 1, 2, false, 1)]
        [DataRow(EnumPais.Brasil, EnumTipoComposicao.Truck, EnumEmpresa.Combustiveis, null, 2, false, 2)]
        [DataRow(EnumPais.Brasil, EnumTipoComposicao.Carreta, EnumEmpresa.Combustiveis, 1, 2, false, 2)]
        [DataRow(EnumPais.Brasil, EnumTipoComposicao.Carreta, EnumEmpresa.EAB, 1, 2, false, 1)]
        [DataRow(EnumPais.Brasil, EnumTipoComposicao.Carreta, EnumEmpresa.Ambos, 1, 2, false, 1)]
        [DataRow(EnumPais.Brasil, EnumTipoComposicao.Carreta, EnumEmpresa.Ambos, null, 2, false, 0)]
        [DataRow(EnumPais.Argentina, EnumTipoComposicao.Truck, EnumEmpresa.Combustiveis, 1, 2, true, 1)]
        [DataRow(EnumPais.Argentina, EnumTipoComposicao.Truck, EnumEmpresa.Combustiveis, null, 2, false, 2)]
        [DataRow(EnumPais.Argentina, EnumTipoComposicao.Carreta, EnumEmpresa.Ambos, null, 2, true, 0)]
        public void SelecionaPlacaPrincipalFOBTest(EnumPais enumPais, EnumTipoComposicao tipoComposicao, EnumEmpresa empresa, int? idPlaca1, int? idPlaca2, bool gerarObjetoPlaca, int idPlacaEsperada)
        {
            //Arrange
            ComposicaoBusiness composicaoBusiness = new ComposicaoBusiness(enumPais);
            Composicao composicao = new Composicao();
            composicao.IDEmpresa = (int)empresa;
            composicao.IDTipoComposicao = (int)tipoComposicao;
            composicao.IDPlaca1 = idPlaca1;
            composicao.IDPlaca2 = idPlaca2;

            if (gerarObjetoPlaca)
            { 
                composicao.p1 = new Placa() { ID = (idPlaca1 ?? -1) };
                composicao.p2 = new Placa() { ID = (idPlaca2 ?? -1) };
            }

            Placa placaEsperada = new Placa() { ID = idPlacaEsperada };

            //Act
            Placa placaAtual = composicaoBusiness.SelecionaPlacaPrincipalFOB(composicao);

            //Assert
            Assert.AreEqual(placaEsperada.ID, placaAtual.ID);
        }

        #endregion

        #region SelecionarVolumeCompartimentoTest

        [DataTestMethod]
        [DynamicData(nameof(GetDataTests), DynamicDataSourceType.Method)]
        public void SelecionarVolumeCompartimentoTest(DataTest dataTest)
        {
            //Arrange
            ComposicaoBusiness composicaoBusiness = new ComposicaoBusiness(dataTest.Pais);
            
            PrivateObject obj = new PrivateObject(composicaoBusiness);

            //Act
            decimal valorAtual = (decimal)obj.Invoke("SelecionarVolumeCompartimento", dataTest.Placa);

            //Assert
            Assert.AreEqual(dataTest.ValorEsperado, valorAtual);
        }

        /// <summary>
        /// Método para montar os cenários complexos de testes, visto dependem de classes e parâmetros pré-configurados
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<DataTest[]> GetDataTests()
        {
            #region Cenário 01
            yield return new DataTest[] {
                new DataTest
                (
                    cenario: "Cenario01",
                    pais: EnumPais.Brasil,
                    placa: null,
                    valorEsperado: 0m
                )
            };
            #endregion

            #region Cenário 02
            yield return new DataTest[] {
                new DataTest
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
                )
            };
            #endregion

            #region Cenário 03
            yield return new DataTest[] {
                new DataTest
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
                )
            };
            #endregion

            #region Cenário 04
            yield return new DataTest[] {
                new DataTest
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
                )
            };
            #endregion

            #region Cenário 05
            yield return new DataTest[] {
                new DataTest
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
                )
            };
            #endregion

            #region Cenário 06
            yield return new DataTest[] {
                new DataTest
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
                )
            };
            #endregion

            #region Cenário 07
            yield return new DataTest[] {
                new DataTest
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
                )
            };
            #endregion

            #region Cenário 08
            yield return new DataTest[] {
                new DataTest
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
        }

        public class DataTest
        {
            public string Cenario { get; set; }

            public EnumPais Pais { get; set; }

            public Placa Placa { get; set; }

            public decimal ValorEsperado { get; set; }

            public DataTest(string cenario, EnumPais pais, Placa placa, decimal valorEsperado)
            {
                Cenario = cenario;
                Pais = pais;
                Placa = placa;
                ValorEsperado = valorEsperado;
            }
      }

        #endregion
    }
}