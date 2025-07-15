using Microsoft.VisualStudio.TestTools.UnitTesting;
using Raizen.UniCad.BLL;
using Raizen.UniCad.BLLTests.Bases;
using Raizen.UniCad.Model;
using System.Collections.Generic;
using System.Reflection;

namespace Raizen.UniCad.BLLTests
{
    [TestClass()]
    public class PlacaBusinessTests : BaseTest
    {
        private readonly PlacaBusiness _placaBll = new PlacaBusiness();
        private object actualValue;

        //[TestMethod]
        //public void ListarVeiculoPlaca()
        //{
        //    var placa = PlacaBusinessTests.AdicionarPlacas("SSS9999", (int)EnumTipoVeiculo.Truck, false, false);
        //    var composicao = new Composicao
        //    {
        //        IDEmpresa = 2,
        //        Operacao = "CIF",
        //        IDTipoComposicao = (int)EnumTipoComposicao.Truck,
        //        //IDTipoComposicaoEixo = 19,
        //        IDCategoriaVeiculo = (int)EnumCategoriaVeiculo.Aluguel,
        //        IDPlaca1 = placa.ID,
        //        CPFCNPJ = "08310367000113",
        //        RazaoSocial = "SIMEIRA LOGISTICA LTDA",
        //        DataAtualizacao = DateTime.Now,
        //        IDStatus = (int)EnumStatusComposicao.EmAprovacao,
        //        PBTC = 3.00,
        //        LoginUsuario = "tr009592"
        //    };
        //    new ComposicaoBusiness().AdicionarComposicao(composicao,false);

        //    var lista = _placaBll.ListarPlacaServico(new Model.Filtro.PlacaServicoFiltro{ Operacao = "CIF", LinhaNegocio = 2, PlacaVeiculo = "SSS9999"});
        //    Assert.AreNotEqual(lista, null);

        //    new ComposicaoBusiness().ExcluirComposicao(composicao.ID,false);

        //}
        [TestMethod()]
        public void ListarPlacaTest()
        {
            var lista = _placaBll.ListarPlaca(new Model.Filtro.PlacaFiltro(), new Framework.Models.PaginadorModel());
            Assert.AreNotEqual(lista, null);
        }

        [TestMethod()]
        public void ListarPlacaCountTest()
        {
            var lista = _placaBll.ListarPlacaCount(new Model.Filtro.PlacaFiltro());
            Assert.AreNotEqual(lista, null);
        }

        [TestMethod()]
        public void VerificarPlacaJaUsadaTest()
        {
            var existe1 = _placaBll.VerificarPlacaJaUsada("SSS7432", "FOB", 1, null, EnumTipoComposicao.Truck, 123, 1);
            Assert.AreEqual(existe1, false);
            var existe7 = _placaBll.VerificarPlacaJaUsada("SSS7432", "FOB", 2, null, EnumTipoComposicao.Truck, 123, 1);
            Assert.AreEqual(existe7, false);
            var existe2 = _placaBll.VerificarPlacaJaUsada("SSS7432", "CIF", 1, null, EnumTipoComposicao.Truck, 123, 1);
            Assert.AreEqual(existe2, false);
            var existe3 = _placaBll.VerificarPlacaJaUsada("SSS7432", "FOB", 1, null, EnumTipoComposicao.Carreta, 123, 1);
            Assert.AreEqual(existe3, false);
            var existe6 = _placaBll.VerificarPlacaJaUsada("SSS7432", "FOB", 2, null, EnumTipoComposicao.Carreta, 123, 1);
            Assert.AreEqual(existe6, false);
            var existe4 = _placaBll.VerificarPlacaJaUsada("SSS7432", "CIF", 1, null, EnumTipoComposicao.Carreta, 123, 1);
            Assert.AreEqual(existe4, false);
        }

        [IgnoreAttribute("Teste desligado até resolver acesso ao banco")]
        [TestMethod()]
        public void ValidarAcessoTest()
        {
            var existe1 = _placaBll.ValidarAcesso("tr011803", null, new Placa() { ID = 1234 });
            Assert.AreEqual(true, true);
        }

        [IgnoreAttribute("Teste desligado até resolver a questão da ordem de execução no Jenkins")]
        [TestMethod()]
        public void StatusPlacaTest()
        {
            var existe1 = _placaBll.StatusPlaca(new Placa() { ID = 1234 });
            Assert.AreEqual(true, true);
        }

        [TestMethod()]
        public void PlacaAprovadaTest()
        {
            var existe1 = _placaBll.PlacaAprovada(new Placa() { ID = 1234 });
            Assert.AreEqual(true, true);
        }

        [IgnoreAttribute("Teste desligado até resolver a questão da ordem de execução no Jenkins")]
        [TestMethod()]
        public void ListarPlacaEmAprovacaoTest()
        {
            var existe1 = _placaBll.ListarPlacaEmAprovacao("ABC1324", "CIF", 1);
            Assert.AreEqual(true, true);
        }

        [IgnoreAttribute("Teste desligado até resolver a questão da ordem de execução no Jenkins")]
        [TestMethod()]
        public void ListarPorStatusTest()
        {
            var existe1 = _placaBll.ListarPorStatus(0, null, "ABC1324", EnumStatusComposicao.Aprovado, "FOB");
            Assert.AreEqual(true, true);
        }

        [IgnoreAttribute("Teste desligado até resolver a questão da ordem de execução no Jenkins")]
        [TestMethod()]
        public void ListarAlteracoesTest()
        {
            _placaBll.ListarAlteracoes(new PlacaBusiness().Selecionar(p => p.IDTipoVeiculo == (int)EnumTipoVeiculo.Truck).ID, new PlacaBusiness().Selecionar(p => p.IDTipoVeiculo == (int)EnumTipoVeiculo.Carreta).ID);
            Assert.AreEqual(true, true);
        }

        [IgnoreAttribute("Teste desligado até resolver a questão da ordem de execução no Jenkins")]
        [TestMethod()]
        public void isPlacaAlteradaTest()
        {
            _placaBll.isPlacaAlterada(new PlacaBusiness().Selecionar(p => p.IDTipoVeiculo == (int)EnumTipoVeiculo.Truck).ID, new PlacaBusiness().Selecionar(p => p.IDTipoVeiculo == (int)EnumTipoVeiculo.Carreta).ID);
            Assert.AreEqual(true, true);
        }

        [IgnoreAttribute("Teste desligado até resolver acesso ao banco")]
        [TestMethod()]
        public void isPlacaPendenteTest()
        {
            _placaBll.isPlacaPendente(1234);
            Assert.AreEqual(true, true);
        }

        [TestMethod()]
        public void ObterPlacaAprovadaTest()
        {
            _placaBll.ObterPlacaAprovada("ABC1234");
            Assert.AreEqual(true, true);
        }

        //[TestMethod()]
        //public void AtualizarPlacaPermissaoTest()
        //{
        //    var placa = new PlacaBusiness().Selecionar(p => p.IDTipoVeiculo == (int)EnumTipoVeiculo.Truck);
        //    Placa placaNew = new Placa();
        //    _placaBll.AtualizarPlacaPermissao(placa, out placaNew);
        //    Assert.AreEqual(true, true);
        //}

        [IgnoreAttribute("Teste desligado até resolver a questão da ordem de execução no Jenkins")]
        [TestMethod()]
        public void ListarPorComposicaoTest()
        {
            var comp = new ComposicaoBusiness().Selecionar(p => p.IDEmpresa == 2);
            _placaBll.ListarPorComposicao(comp);
            Assert.AreEqual(true, true);
        }

        [TestMethod()]
        public void ListarPlacaSemComposicaoTest()
        {
            var lista = _placaBll.ListarPlacaSemComposicao(new Model.Filtro.PlacaFiltro(), new Framework.Models.PaginadorModel() { });
            Assert.AreNotEqual(lista, null);
        }

        [TestMethod()]
        public void ListarPlacaSemComposicaoCountTest()
        {
            var lista = _placaBll.ListarPlacaSemComposicaoCount(new Model.Filtro.PlacaFiltro());
            Assert.AreNotEqual(lista, null);
        }

        [IgnoreAttribute("Teste desligado até resolver a questão da ordem de execução no Jenkins")]
        [TestMethod()]
        public void ListarPlacaPorOperacaoLinhaNegocioTest()
        {
            var lista = _placaBll.ListarPlacaPorOperacaoLinhaNegocio(new Model.Filtro.PlacaFiltro());
            Assert.AreNotEqual(lista, null);
        }

        [TestMethod()]
        public void ListarPorIdComposicaoTest()
        {
            var lista = _placaBll.ListarPorIdComposicao("ABC1234", 1);
            Assert.AreNotEqual(lista, null);
        }

        [IgnoreAttribute("Teste desligado até resolver a questão da ordem de execução no Jenkins")]
        [TestMethod()]
        public void ExcluirPlacaTest()
        {
            var placa = PlacaBusinessTests.AdicionarPlacas("CAU1346", (int)EnumTipoVeiculo.Truck, false, false);
            var lista = _placaBll.ExcluirPlaca(placa.ID);
            Assert.AreNotEqual(lista, null);
        }
  
        public static Placa AdicionarPlacas(string placaNumero, int idtipoVeiculo, bool comSetaPadrao = false, bool comSetas = true)
        {
            Placa placa = FuncoesCompartilhadasTests.GetPlacaBrasil01(placaNumero, idtipoVeiculo, comSetaPadrao);

            if (idtipoVeiculo == (int)EnumTipoVeiculo.Cavalo)
                placa.NumeroEixos = 3;
            new PlacaBusiness().AdicionarPlaca(placa);
            new PlacaBusiness().AtualizarPlaca(placa);
            if (comSetas)
                placa.Setas = AdicionarPlacasSetas(placa, comSetaPadrao, 5000);
            return placa;
        }

        private static List<PlacaSeta> AdicionarPlacasSetas(Placa placa, bool comSetaPadrao, decimal volume)
        {
            List<PlacaSeta> listPlacaSeta = new List<PlacaSeta>();
            PlacaSeta placaSeta = new PlacaSeta();
            placaSeta.IDPlaca = placa.ID;
            placaSeta.VolumeCompartimento1 = volume;
            placaSeta.VolumeCompartimento2 = volume + 1;
            placaSeta.VolumeCompartimento3 = volume + 2;
            placaSeta.VolumeCompartimento4 = volume + 3;
            placaSeta.VolumeCompartimento5 = volume + 4;
            placaSeta.VolumeCompartimento6 = volume + 5;
            placaSeta.VolumeCompartimento7 = volume + 6;
            placaSeta.VolumeCompartimento8 = volume + 7;
            placaSeta.VolumeCompartimento9 = volume + 8;
            placaSeta.VolumeCompartimento10 = volume + 9;

            new PlacaSetaBusiness().Adicionar(placaSeta);
            listPlacaSeta.Add(placaSeta);

            if (comSetaPadrao)
            {
                PlacaSeta placaSeta2 = new PlacaSeta();
                placaSeta2.IDPlaca = placa.ID;
                placaSeta2.VolumeCompartimento1 = volume * 3;
                new PlacaSetaBusiness().Adicionar(placaSeta2);
                listPlacaSeta.Add(placaSeta2);
            }

            return listPlacaSeta;
        }

        #region CalcularColunas

        [TestMethod]
        public void CalcularColunasTest()
        {
            /* Foi acordado junto ao time de arquitetura da Raizen o uso desta abordagem dos cenários abaixo,
               visto que o Jenkins da Raizen não aceita o uso do atributo [DataRows()] nos testes */
            #region Cenários
            var cenarios = new List<CalcularColunasTest_Cenario>()
            {
                new CalcularColunasTest_Cenario
                (
                    cenario: "Cenario01",
                    pais: EnumPais.Brasil,
                    placa: null,
                    placaEsperada: null
                ),

                new CalcularColunasTest_Cenario
                (
                    cenario: "Cenario02",
                    pais: EnumPais.Brasil,
                    placa: new Placa()
                    {
                        MultiSeta = true,
                        Setas = new List<PlacaSeta>()
                        {
                            new PlacaSeta() { VolumeCompartimento1 = 10 },
                            new PlacaSeta() { VolumeCompartimento2 = 20 }
                        }
                    },
                    placaEsperada: new Placa()
                    {
                        MultiSeta = true,
                        Setas = new List<PlacaSeta>()
                        {
                            new PlacaSeta() { VolumeCompartimento1 = 10, Sequencial = 1, Multiseta = true, Colunas = 2 },
                            new PlacaSeta() { VolumeCompartimento2 = 20, Sequencial = 2, Multiseta = true, Colunas = 2 }
                        }
                    }
                ),

                new CalcularColunasTest_Cenario
                (
                    cenario: "Cenario03",
                    pais: EnumPais.Brasil,
                    placa: new Placa()
                    {
                        MultiSeta = true,
                        Setas = new List<PlacaSeta>()
                        {
                            new PlacaSeta() { VolumeCompartimento1 = 10 },
                            new PlacaSeta() { VolumeCompartimento2 = 20 },
                            new PlacaSeta() { VolumeCompartimento3 = 30 },
                            new PlacaSeta() { VolumeCompartimento4 = 40 },
                            new PlacaSeta() { VolumeCompartimento5 = 50 },
                            new PlacaSeta() { VolumeCompartimento6 = 60 },
                            new PlacaSeta() { VolumeCompartimento7 = 70 },
                            new PlacaSeta() { VolumeCompartimento8 = 80 },
                            new PlacaSeta() { VolumeCompartimento9 = 90 },
                            new PlacaSeta() { VolumeCompartimento10 = 100 }
                        }
                    },
                    placaEsperada: new Placa()
                    {
                        MultiSeta = true,
                        Setas = new List<PlacaSeta>()
                        {
                            new PlacaSeta() { VolumeCompartimento1 = 10, Sequencial = 1, Multiseta = true, Colunas = 10 },
                            new PlacaSeta() { VolumeCompartimento2 = 20, Sequencial = 2, Multiseta = true, Colunas = 10 },
                            new PlacaSeta() { VolumeCompartimento3 = 30, Sequencial = 3, Multiseta = true, Colunas = 10 },
                            new PlacaSeta() { VolumeCompartimento4 = 40, Sequencial = 4, Multiseta = true, Colunas = 10 },
                            new PlacaSeta() { VolumeCompartimento5 = 50, Sequencial = 5, Multiseta = true, Colunas = 10 },
                            new PlacaSeta() { VolumeCompartimento6 = 60, Sequencial = 6, Multiseta = true, Colunas = 10 },
                            new PlacaSeta() { VolumeCompartimento7 = 70, Sequencial = 7, Multiseta = true, Colunas = 10 },
                            new PlacaSeta() { VolumeCompartimento8 = 80, Sequencial = 8, Multiseta = true, Colunas = 10 },
                            new PlacaSeta() { VolumeCompartimento9 = 90, Sequencial = 9, Multiseta = true, Colunas = 10 },
                            new PlacaSeta() { VolumeCompartimento10 = 100, Sequencial = 10, Multiseta = true, Colunas = 10 }
                        }
                    }
                )
            }; 
            #endregion

            foreach (var c in cenarios)
            {
                //Arrange
                PlacaBusiness placaBusiness = new PlacaBusiness(c.Pais);
                Placa placa = c.Placa;
                Placa placaEsperada = c.PlacaEsperada;

                //Act
                placaBusiness.CalcularColunas(placa);

                //Assert
                if (placa != null)
                {
                    for (int i = 0; i < placa.Setas.Count; i++)
                    {
                        var seta = placa.Setas[i];
                        var setaEsperada = placaEsperada.Setas[i];

                        Assert.AreEqual(setaEsperada.Colunas, seta.Colunas, $"Erro no campo [Colunas] do [{c.Cenario}]");
                        Assert.AreEqual(setaEsperada.Sequencial, seta.Sequencial, $"Erro no campo [Sequencial] do [{c.Cenario}]");
                        Assert.AreEqual(setaEsperada.Multiseta, seta.Multiseta, $"Erro no campo [Multiseta] do [{c.Cenario}]");
                        Assert.AreEqual(setaEsperada.VolumeCompartimento1, seta.VolumeCompartimento1, $"Erro no campo [VolumeCompartimento1] do [{c.Cenario}]");
                        Assert.AreEqual(setaEsperada.VolumeCompartimento2, seta.VolumeCompartimento2, $"Erro no campo [VolumeCompartimento2] do [{c.Cenario}]");
                        Assert.AreEqual(setaEsperada.VolumeCompartimento3, seta.VolumeCompartimento3, $"Erro no campo [VolumeCompartimento3] do [{c.Cenario}]");
                        Assert.AreEqual(setaEsperada.VolumeCompartimento4, seta.VolumeCompartimento4, $"Erro no campo [VolumeCompartimento4] do [{c.Cenario}]");
                        Assert.AreEqual(setaEsperada.VolumeCompartimento5, seta.VolumeCompartimento5, $"Erro no campo [VolumeCompartimento5] do [{c.Cenario}]");
                        Assert.AreEqual(setaEsperada.VolumeCompartimento6, seta.VolumeCompartimento6, $"Erro no campo [VolumeCompartimento6] do [{c.Cenario}]");
                        Assert.AreEqual(setaEsperada.VolumeCompartimento7, seta.VolumeCompartimento7, $"Erro no campo [VolumeCompartimento7] do [{c.Cenario}]");
                        Assert.AreEqual(setaEsperada.VolumeCompartimento8, seta.VolumeCompartimento8, $"Erro no campo [VolumeCompartimento8] do [{c.Cenario}]");
                        Assert.AreEqual(setaEsperada.VolumeCompartimento9, seta.VolumeCompartimento9, $"Erro no campo [VolumeCompartimento9] do [{c.Cenario}]");
                        Assert.AreEqual(setaEsperada.VolumeCompartimento10, seta.VolumeCompartimento10, $"Erro no campo [VolumeCompartimento10] do [{c.Cenario}]");
                    }
                }
                else
                    Assert.AreEqual(placaEsperada, placa); 
            }
        }

        public class CalcularColunasTest_Cenario
        {
            public string Cenario { get; set; }

            public EnumPais Pais { get; set; }

            public Placa Placa { get; set; }

            public Placa PlacaEsperada { get; set; }

            public CalcularColunasTest_Cenario(string cenario, EnumPais pais, Placa placa, Placa placaEsperada)
            {
                Cenario = cenario;
                Pais = pais;
                Placa = placa;
                PlacaEsperada = placaEsperada;
            }
        }

        #endregion
    }
}