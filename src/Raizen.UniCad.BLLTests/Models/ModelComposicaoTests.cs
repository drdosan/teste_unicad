using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Raizen.UniCad.BLL;
using Raizen.UniCad.BLL.Interfaces;
using Raizen.UniCad.DAL.Interfaces;
using Raizen.UniCad.Model;
using Raizen.UniCad.Web.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;

namespace Raizen.UniCad.BLLTests.Models
{
    [TestClass]
    public class ModelComposicaoTests
    {
        [TestMethod]
        public void SetarTrasnportadoraComposicao_Test()
        {
            //Arrange
            var modelComposicao = new ModelComposicao();
            var placaBrasilBll = new Mock<PlacaBusiness>();
            var placaArgentinaBll = new Mock<PlacaBusiness>();

            /* Foi acordado junto ao time de arquitetura da Raizen o uso desta abordagem dos cenários abaixo,
               visto que o Jenkins da Raizen não aceita o uso do atributo [DataRows()] nos testes */
            #region Cenários

            var cenarios = new List<SetarTrasnportadoraComposicao_Cenario>()
            {
                new SetarTrasnportadoraComposicao_Cenario(
                    cenario: "1",
                    pais: EnumPais.Brasil,
                    placa1Mockada: new Placa() { PlacaBrasil = new PlacaBrasil { CPFCNPJ = "CnpjPlaca1" } },
                    placa2Mockada: new Placa() { PlacaBrasil = new PlacaBrasil { CPFCNPJ = "CnpjPlaca2" } },
                    composicao: new Composicao() { Operacao = "FOB" },
                    cnpjTransportadoraEsperada: "CnpjPlaca2"
                ),
                new SetarTrasnportadoraComposicao_Cenario(
                    cenario: "2",
                    pais: EnumPais.Brasil,
                    placa1Mockada: new Placa() { PlacaBrasil = new PlacaBrasil { CPFCNPJ = "CnpjPlaca1" } },
                    placa2Mockada: null,
                    composicao: new Composicao() { Operacao = "FOB" },
                    cnpjTransportadoraEsperada: "CnpjPlaca1"
                ),
                new SetarTrasnportadoraComposicao_Cenario(
                    cenario: "3",
                    pais: EnumPais.Brasil,
                    placa1Mockada: new Placa() { PlacaBrasil = new PlacaBrasil { CPFCNPJ = "CnpjPlaca1" } },
                    placa2Mockada: new Placa() { PlacaBrasil = new PlacaBrasil { CPFCNPJ = "CnpjPlaca2" } },
                    composicao: new Composicao() { CPFCNPJ = "CnpjOriginalComposicao", Operacao = "FOB" },
                    cnpjTransportadoraEsperada: "CnpjOriginalComposicao"
                ),
                new SetarTrasnportadoraComposicao_Cenario(
                    cenario: "4",
                    pais: EnumPais.Brasil,
                    placa1Mockada: new Placa() { PlacaBrasil = new PlacaBrasil { CPFCNPJ = "CnpjPlaca1" } },
                    placa2Mockada: new Placa() { PlacaBrasil = new PlacaBrasil { CPFCNPJ = "CnpjPlaca2" } },
                    composicao: new Composicao() { Operacao = "CIF" },
                    cnpjTransportadoraEsperada: "CnpjPlaca1"
                ),
                new SetarTrasnportadoraComposicao_Cenario(
                    cenario: "5",
                    pais: EnumPais.Brasil,
                    placa1Mockada: new Placa() { PlacaBrasil = new PlacaBrasil { CPFCNPJ = "CnpjPlaca1" } },
                    placa2Mockada: null,
                    composicao: new Composicao() { Operacao = "CIF" },
                    cnpjTransportadoraEsperada: "CnpjPlaca1"
                ),
                new SetarTrasnportadoraComposicao_Cenario(
                    cenario: "6",
                    pais: EnumPais.Brasil,
                    placa1Mockada: new Placa() { PlacaBrasil = new PlacaBrasil { CPFCNPJ = "CnpjPlaca1" } },
                    placa2Mockada: new Placa() { PlacaBrasil = new PlacaBrasil { CPFCNPJ = "CnpjPlaca2" } },
                    composicao: new Composicao() { CPFCNPJ = "CnpjOriginalComposicao", Operacao = "CIF" },
                    cnpjTransportadoraEsperada: "CnpjOriginalComposicao"
                ),
                new SetarTrasnportadoraComposicao_Cenario(
                    cenario: "7",
                    pais: EnumPais.Argentina,
                    placa1Mockada: new Placa() { PlacaArgentina = new PlacaArgentina { CUIT = "CuitPlaca1" } },
                    placa2Mockada: new Placa() { PlacaArgentina = new PlacaArgentina { CUIT = "CuitPlaca2" } },
                    composicao: new Composicao() { Operacao = "FOB" },
                    cnpjTransportadoraEsperada: "CuitPlaca2"
                ),
                new SetarTrasnportadoraComposicao_Cenario(
                    cenario: "8",
                    pais: EnumPais.Argentina,
                    placa1Mockada: new Placa() { PlacaArgentina = new PlacaArgentina { CUIT = "CuitPlaca1" } },
                    placa2Mockada: null,
                    composicao: new Composicao() { Operacao = "FOB" },
                    cnpjTransportadoraEsperada: "CuitPlaca1"
                ),
                new SetarTrasnportadoraComposicao_Cenario(
                    cenario: "9",
                    pais: EnumPais.Argentina,
                    placa1Mockada: new Placa() { PlacaArgentina = new PlacaArgentina { CUIT = "CuitPlaca1" } },
                    placa2Mockada: new Placa() { PlacaArgentina = new PlacaArgentina { CUIT = "CuitPlaca2" } },
                    composicao: new Composicao() { CPFCNPJ = "CuitOriginalComposicao", Operacao = "FOB" },
                    cnpjTransportadoraEsperada: "CuitPlaca2"
                ),
                new SetarTrasnportadoraComposicao_Cenario(
                    cenario: "10",
                    pais: EnumPais.Argentina,
                    placa1Mockada: new Placa() { PlacaArgentina = new PlacaArgentina { CUIT = "CuitPlaca1" } },
                    placa2Mockada: new Placa() { PlacaArgentina = new PlacaArgentina { CUIT = "CuitPlaca2" } },
                    composicao: new Composicao() { Operacao = "CIF" },
                    cnpjTransportadoraEsperada: "CuitPlaca1"
                ),
                new SetarTrasnportadoraComposicao_Cenario(
                    cenario: "11",
                    pais: EnumPais.Argentina,
                    placa1Mockada: new Placa() { PlacaArgentina = new PlacaArgentina { CUIT = "CuitPlaca1" } },
                    placa2Mockada: null,
                    composicao: new Composicao() { Operacao = "CIF" },
                    cnpjTransportadoraEsperada: "CuitPlaca1"
                ),
                new SetarTrasnportadoraComposicao_Cenario(
                    cenario: "12",
                    pais: EnumPais.Argentina,
                    placa1Mockada: new Placa() { PlacaArgentina = new PlacaArgentina { CUIT = "CuitPlaca1" } },
                    placa2Mockada: new Placa() { PlacaArgentina = new PlacaArgentina { CUIT = "CuitPlaca2" } },
                    composicao: new Composicao() { CPFCNPJ = "CuitOriginalComposicao", Operacao = "CIF" },
                    cnpjTransportadoraEsperada: "CuitPlaca1"
                ),
            };

            #endregion

            foreach (var c in cenarios)
            {
                //Arrange
                placaBrasilBll.Setup(p => p.ObtemPlaca1(It.IsAny<Composicao>())).Returns(c.Placa1Mockada);
                placaBrasilBll.Setup(p => p.ObtemPlaca2(It.IsAny<Composicao>())).Returns(c.Placa2Mockada);
                placaArgentinaBll.Setup(p => p.ObtemPlaca1(It.IsAny<Composicao>())).Returns(c.Placa1Mockada);
                placaArgentinaBll.Setup(p => p.ObtemPlaca2(It.IsAny<Composicao>())).Returns(c.Placa2Mockada);

                modelComposicao.Composicao = c.Composicao;
                PrivateObject obj = new PrivateObject(modelComposicao);

                //Act
                var result = obj.Invoke("SetarTrasnportadoraComposicao", c.Pais, placaBrasilBll.Object, placaArgentinaBll.Object);

                //Assert
                Assert.AreEqual(c.CnpjTransportadoraEsperada, result, $"Falha no preenchimento da Trasnportadora no cenário {c.Cenario}");
            }

            Assert.AreEqual(12, cenarios.Count, $"Falha na contagem total de cenários");
        }

        private class SetarTrasnportadoraComposicao_Cenario
        {
            public SetarTrasnportadoraComposicao_Cenario(string cenario, EnumPais pais, Placa placa1Mockada, Placa placa2Mockada, Composicao composicao, string cnpjTransportadoraEsperada)
            {
                Cenario = cenario;
                Pais = pais;
                Placa1Mockada = placa1Mockada;
                Placa2Mockada = placa2Mockada;
                CnpjTransportadoraEsperada = cnpjTransportadoraEsperada;
                Composicao = composicao;
            }

            public string Cenario { get; set; }

            public EnumPais Pais { get; set; }

            public Placa Placa1Mockada { get; set; }

            public Placa Placa2Mockada { get; set; }

            public string CnpjTransportadoraEsperada { get; set; }

            public Composicao Composicao;
        }
    }

}
