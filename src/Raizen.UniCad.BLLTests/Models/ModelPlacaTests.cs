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
    public class ModelPlacaTests
    {        
        [TestMethod]
        public void Validate_Test()
        {
            //Arrange
            //var placaBusiness = new Mock<PlacaBusiness>(); 
            /* Não foi possível utilizar a abordagem com Mock aqui, visto que não consegui injetar este objeto mockado dentro do ModelPlaca.
             * Portanto, para não perder os testes desta regras, resolvi utilizada a abordagem comum do sistema, ou seja, acessando o BD */

            //Utilizei este WHERE só para diminiuir o range de registros, já que não havia um TOP 1 disponivel no repositório
            var usuario = new UsuarioBusiness().Listar(u => u.ID < 50)[0]; 
            var trasnportadora = new TransportadoraBusiness().Listar(u => u.ID < 50)[0];

            /* Foi acordado junto ao time de arquitetura da Raizen o uso desta abordagem dos cenários abaixo,
               visto que o Jenkins da Raizen não aceita o uso do atributo [DataRows()] nos testes */
            #region Cenários

            var cenarios = new List<ModelPlacaValidate_Cenario>()
            {
                new ModelPlacaValidate_Cenario(
                    cenario: "1",
                    modeloValido: true,
                    mensagemErroEsperada: null,
                    modelPlaca: new ModelPlaca()
                    {
                        IdPais = (int)EnumPais.Argentina,
                        Placa = FuncoesCompartilhadasTests.GetPlacaArgentina01("CHZ1010", (int)EnumTipoVeiculo.Carreta, true, usuario.ID, trasnportadora.ID)
                    },
                    placaSelecionada: FuncoesCompartilhadasTests.GetPlacaArgentina01("CHZ1010", (int)EnumTipoVeiculo.Carreta, true, usuario.ID, trasnportadora.ID)
                ),

                new ModelPlacaValidate_Cenario(
                    cenario: "2",
                    modeloValido: false,
                    mensagemErroEsperada: "Usuario no registrado en UNICAD, contacte a la persona responsablel",
                    modelPlaca: new ModelPlaca()
                    {
                        IdPais = (int)EnumPais.Argentina,
                        Placa = FuncoesCompartilhadasTests.GetPlacaArgentina01("CHZ1010", (int)EnumTipoVeiculo.Carreta, true, 0, trasnportadora.ID)
                    },
                    placaSelecionada: FuncoesCompartilhadasTests.GetPlacaArgentina01("CHZ1010", (int)EnumTipoVeiculo.Carreta, true, 0, trasnportadora.ID)
                ),

                new ModelPlacaValidate_Cenario(
                    cenario: "3",
                    modeloValido: false,
                    mensagemErroEsperada: "Es necesario vincular un transportista.",
                    modelPlaca: new ModelPlaca()
                    {
                        IdPais = (int)EnumPais.Argentina,
                        Placa = FuncoesCompartilhadasTests.GetPlacaArgentina01("CHZ1010", (int)EnumTipoVeiculo.Carreta, true, usuario.ID, null, "CIF")
                    },
                    placaSelecionada: FuncoesCompartilhadasTests.GetPlacaArgentina01("CHZ1010", (int)EnumTipoVeiculo.Carreta, true, usuario.ID, null, "CIF")
                )

                //TODO: Não deu tempo de implementar os testes dos demais cenários. Fazê-lo a partir daqui...
            };

            #endregion

            foreach (var c in cenarios)
            {
                //Arrange
                var placaBusiness = new PlacaBusiness(c.ModelPlaca.Placa.IDPais);
                if (c.PlacaSelecionada != null)
                    placaBusiness.AdicionarPlaca(c.PlacaSelecionada);

                //Act
                var result = c.ModelPlaca.Validate(new System.ComponentModel.DataAnnotations.ValidationContext(c.ModelPlaca)).ToList();

                //Remove os registros de testes inseridos
                if (c.PlacaSelecionada != null)
                    placaBusiness.ExcluirPlaca(c.PlacaSelecionada.ID);

                //Assert
                Assert.AreEqual(c.ModeloValido, !result.Any(), $"Falha na contagem de erros no cenário {c.Cenario}");

                if (result.Count == 1)
                    Assert.AreEqual(c.MensagemErroEsperada, result[0].ErrorMessage, $"Falha na descrição do erro no cenário {c.Cenario}");
            }
        }

        private class ModelPlacaValidate_Cenario
        {
            public ModelPlacaValidate_Cenario(string cenario, ModelPlaca modelPlaca, bool modeloValido, Placa placaSelecionada, string mensagemErroEsperada)
            {
                Cenario = cenario;
                ModelPlaca = modelPlaca;
                ModeloValido = modeloValido;
                PlacaSelecionada = placaSelecionada;
                MensagemErroEsperada = mensagemErroEsperada;
            }

            public string Cenario { get; internal set; }

            public ModelPlaca ModelPlaca { get; internal set; }

            public bool ModeloValido { get; internal set; }

            public Placa PlacaSelecionada { get; internal set; }

            public string MensagemErroEsperada { get; set; }
        }
    }

}
