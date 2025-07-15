using Infraestructure.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Raizen.UniCad.Model;
using System.Collections.Generic;

namespace Raizen.UniCad.BLLTests.Infraestructure
{
    [TestClass]
    public class MapperExtensionsTest
    {
        #region MotoristaPesquisaMapearTest

        [TestMethod]
        public void MotoristaPesquisaMapearTest()
        {
            /* Foi acordado junto ao time de arquitetura da Raizen o uso desta abordagem dos cenários abaixo,
               visto que o Jenkins da Raizen não aceita o uso do atributo [DataRows()] nos testes */
            #region Cenários
            var cenarios = new List<MotoristaPesquisaMapearTest_Cenario>()
            {
                new MotoristaPesquisaMapearTest_Cenario(true, EnumPais.Brasil, "José da Silva"),
                new MotoristaPesquisaMapearTest_Cenario(true, EnumPais.Argentina, "Miguelitto"),
                new MotoristaPesquisaMapearTest_Cenario(false, EnumPais.Argentina, "Miguelitto")
            }; 
            #endregion

            foreach (var c in cenarios)
            {
                //Arrange
                MotoristaPesquisa motoristaPesquisa = null;

                if (c.InstanciarMotorista)
                {
                    motoristaPesquisa = new MotoristaPesquisa()
                    {
                        IdPais = c.Pais,
                        Nome = c.Nome
                    };
                }

                //Act
                var motorista = motoristaPesquisa.Mapear();

                //Assert
                if (c.InstanciarMotorista)
                    Assert.AreEqual(c.Nome, motorista.Nome);
                else
                    Assert.IsNull(motorista);
            }
        }

        private class MotoristaPesquisaMapearTest_Cenario
        {
            public MotoristaPesquisaMapearTest_Cenario(bool instanciarMotorista, EnumPais pais, string nome)
            {
                InstanciarMotorista = instanciarMotorista;
                Pais = pais;
                Nome = nome;
            }

            public bool InstanciarMotorista { get; set; }
            public EnumPais Pais { get; set; }
            public string Nome { get; set; }
        } 

        #endregion


        #region MotoristaPesquisaListaMapearTest

        [TestMethod]
        public void MotoristaPesquisaListaMapearTest()
        {
            /* Foi acordado junto ao time de arquitetura da Raizen o uso desta abordagem dos cenários abaixo,
               visto que o Jenkins da Raizen não aceita o uso do atributo [DataRows()] nos testes */
            #region Cenários
            var cenarios = new List<MotoristaPesquisaListaMapearTest_Cenario>()
            {
                new MotoristaPesquisaListaMapearTest_Cenario(
                        cenario: "Cenario01",
                        pais: EnumPais.Brasil,
                        motoristas: null
                    ),
                new MotoristaPesquisaListaMapearTest_Cenario
                (
                    cenario: "Cenario02",
                    pais: EnumPais.Brasil,
                    motoristas: new List<MotoristaPesquisa>()
                    {
                        new MotoristaPesquisa() { IdPais = EnumPais.Brasil, Nome = "José da Silva" },
                        new MotoristaPesquisa() { IdPais = EnumPais.Argentina, Nome = "Miguelitto" },
                        new MotoristaPesquisa() { IdPais = EnumPais.Brasil, Nome = "Mariana Godoy" }
                    }
                ),
                new MotoristaPesquisaListaMapearTest_Cenario
                (
                    cenario: "Cenario03",
                    pais: EnumPais.Argentina,
                    motoristas: new List<MotoristaPesquisa>()
                    {
                        new MotoristaPesquisa() { IdPais = EnumPais.Brasil, Nome = "José da Silva" },
                        new MotoristaPesquisa() { IdPais = EnumPais.Argentina, Nome = "Miguelitto" },
                        new MotoristaPesquisa() { IdPais = EnumPais.Brasil, Nome = "Mariana Godoy" }
                    }
                )
            }; 
            #endregion

            foreach (var c in cenarios)
            {
                //Arrange

                //Act
                var motoristas = c.Motoristas.Mapear();

                //Assert
                if (c.Motoristas != null)
                {
                    for (int i = 0; i < motoristas.Count; i++)
                        Assert.AreEqual(c.Motoristas[i].Nome, motoristas[i].Nome);
                }
                else
                {
                    Assert.IsNull(motoristas);
                }
            }
        }

        private class MotoristaPesquisaListaMapearTest_Cenario
        {
            public string Cenario { get; set; }

            public EnumPais Pais { get; set; }

            public List<MotoristaPesquisa> Motoristas { get; set; }

            public MotoristaPesquisaListaMapearTest_Cenario(string cenario, EnumPais pais, List<MotoristaPesquisa> motoristas)
            {
                Cenario = cenario;
                Pais = pais;
                Motoristas = motoristas;
            }
        } 

        #endregion
    }    
}
