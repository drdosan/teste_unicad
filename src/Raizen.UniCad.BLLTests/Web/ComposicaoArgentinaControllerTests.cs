using Microsoft.VisualStudio.TestTools.UnitTesting;
using Raizen.UniCad.Model;
using Raizen.UniCad.Web.Controllers;
using Raizen.UniCad.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raizen.UniCad.BLLTests.Web
{
    [TestClass]
    public class ComposicaoArgentinaControllerTests
    {
        [TestMethod]
        public void ValidaTipoComposicao_Test()
        {
            /* Foi acordado junto ao time de arquitetura da Raizen o uso desta abordagem dos cenários abaixo,
               visto que o Jenkins da Raizen não aceita o uso do atributo [DataRows()] nos testes */
            #region Cenários

            var mensagemErro = "El tipo de vehículo no coincide con la posición de la placa en la composición!";

            var cenarios = new List<ComposicaoArgentinaControllerTests_Cenario>()
            {
                #region SemirremolqueChico

		        new ComposicaoArgentinaControllerTests_Cenario(
                    cenario: "Cenário 1",
                    numeroPlaca: (int)EnumPlaca.Placa1,
                    tipoComposicao: (int)EnumTipoComposicao.SemirremolqueChico,
                    modelPlaca: new ModelPlaca()
                    {
                        Placa = new Placa()
                        {
                            IDTipoVeiculo = (int)EnumTipoVeiculo.Carreta
                        }
                    },
                    mensagemEsperadaId: EnumMensagemPlaca.SemMensagemTratadaNoJs,
                    mensagemEsperadaTexto: mensagemErro),

                new ComposicaoArgentinaControllerTests_Cenario(
                    cenario: "Cenário 2",
                    numeroPlaca: (int)EnumPlaca.Placa1,
                    tipoComposicao: (int)EnumTipoComposicao.SemirremolqueChico,
                    modelPlaca: new ModelPlaca()
                    {
                        Placa = new Placa()
                        {
                            IDTipoVeiculo = (int)EnumTipoVeiculo.Tractor
                        }
                    },
                    mensagemEsperadaId: EnumMensagemPlaca.SemMensagemTratadaNoJs,
                    mensagemEsperadaTexto: null),

                new ComposicaoArgentinaControllerTests_Cenario(
                    cenario: "Cenário 3",
                    numeroPlaca: (int)EnumPlaca.Placa2,
                    tipoComposicao: (int)EnumTipoComposicao.SemirremolqueChico,
                    modelPlaca: new ModelPlaca()
                    {
                        Placa = new Placa()
                        {
                            IDTipoVeiculo = (int)EnumTipoVeiculo.Carreta
                        }
                    },
                    mensagemEsperadaId: EnumMensagemPlaca.SemMensagemTratadaNoJs,
                    mensagemEsperadaTexto: mensagemErro),

                new ComposicaoArgentinaControllerTests_Cenario(
                    cenario: "Cenário 4",
                    numeroPlaca: (int)EnumPlaca.Placa2,
                    tipoComposicao: (int)EnumTipoComposicao.SemirremolqueChico,
                    modelPlaca: new ModelPlaca()
                    {
                        Placa = new Placa()
                        {
                            IDTipoVeiculo = (int)EnumTipoVeiculo.Semiremolque
                        }
                    },
                    mensagemEsperadaId: EnumMensagemPlaca.SemMensagemTratadaNoJs,
                    mensagemEsperadaTexto: null), 

	            #endregion

                #region SemirremolqueGrande

                new ComposicaoArgentinaControllerTests_Cenario(
                    cenario: "Cenário 5",
                    numeroPlaca: (int)EnumPlaca.Placa1,
                    tipoComposicao: (int)EnumTipoComposicao.SemirremolqueGrande,
                    modelPlaca: new ModelPlaca()
                    {
                        Placa = new Placa()
                        {
                            IDTipoVeiculo = (int)EnumTipoVeiculo.Carreta
                        }
                    },
                    mensagemEsperadaId: EnumMensagemPlaca.SemMensagemTratadaNoJs,
                    mensagemEsperadaTexto: mensagemErro),

                new ComposicaoArgentinaControllerTests_Cenario(
                    cenario: "Cenário 6",
                    numeroPlaca: (int)EnumPlaca.Placa1,
                    tipoComposicao: (int)EnumTipoComposicao.SemirremolqueGrande,
                    modelPlaca: new ModelPlaca()
                    {
                        Placa = new Placa()
                        {
                            IDTipoVeiculo = (int)EnumTipoVeiculo.Tractor
                        }
                    },
                    mensagemEsperadaId: EnumMensagemPlaca.SemMensagemTratadaNoJs,
                    mensagemEsperadaTexto: null),

                new ComposicaoArgentinaControllerTests_Cenario(
                    cenario: "Cenário 7",
                    numeroPlaca: (int)EnumPlaca.Placa2,
                    tipoComposicao: (int)EnumTipoComposicao.SemirremolqueGrande,
                    modelPlaca: new ModelPlaca()
                    {
                        Placa = new Placa()
                        {
                            IDTipoVeiculo = (int)EnumTipoVeiculo.Carreta
                        }
                    },
                    mensagemEsperadaId: EnumMensagemPlaca.SemMensagemTratadaNoJs,
                    mensagemEsperadaTexto: mensagemErro),

                new ComposicaoArgentinaControllerTests_Cenario(
                    cenario: "Cenário 8",
                    numeroPlaca: (int)EnumPlaca.Placa2,
                    tipoComposicao: (int)EnumTipoComposicao.SemirremolqueGrande,
                    modelPlaca: new ModelPlaca()
                    {
                        Placa = new Placa()
                        {
                            IDTipoVeiculo = (int)EnumTipoVeiculo.Semiremolque
                        }
                    },
                    mensagemEsperadaId: EnumMensagemPlaca.SemMensagemTratadaNoJs,
                    mensagemEsperadaTexto: null),

                #endregion

                #region Escalado

                new ComposicaoArgentinaControllerTests_Cenario(
                    cenario: "Cenário 9",
                    numeroPlaca: (int)EnumPlaca.Placa1,
                    tipoComposicao: (int)EnumTipoComposicao.Escalado,
                    modelPlaca: new ModelPlaca()
                    {
                        Placa = new Placa()
                        {
                            IDTipoVeiculo = (int)EnumTipoVeiculo.Carreta
                        }
                    },
                    mensagemEsperadaId: EnumMensagemPlaca.SemMensagemTratadaNoJs,
                    mensagemEsperadaTexto: mensagemErro),

                new ComposicaoArgentinaControllerTests_Cenario(
                    cenario: "Cenário 10",
                    numeroPlaca: (int)EnumPlaca.Placa1,
                    tipoComposicao: (int)EnumTipoComposicao.Escalado,
                    modelPlaca: new ModelPlaca()
                    {
                        Placa = new Placa()
                        {
                            IDTipoVeiculo = (int)EnumTipoVeiculo.Tractor
                        }
                    },
                    mensagemEsperadaId: EnumMensagemPlaca.SemMensagemTratadaNoJs,
                    mensagemEsperadaTexto: null),

                new ComposicaoArgentinaControllerTests_Cenario(
                    cenario: "Cenário 11",
                    numeroPlaca: (int)EnumPlaca.Placa2,
                    tipoComposicao: (int)EnumTipoComposicao.Escalado,
                    modelPlaca: new ModelPlaca()
                    {
                        Placa = new Placa()
                        {
                            IDTipoVeiculo = (int)EnumTipoVeiculo.Carreta
                        }
                    },
                    mensagemEsperadaId: EnumMensagemPlaca.SemMensagemTratadaNoJs,
                    mensagemEsperadaTexto: mensagemErro),

                new ComposicaoArgentinaControllerTests_Cenario(
                    cenario: "Cenário 12",
                    numeroPlaca: (int)EnumPlaca.Placa2,
                    tipoComposicao: (int)EnumTipoComposicao.Escalado,
                    modelPlaca: new ModelPlaca()
                    {
                        Placa = new Placa()
                        {
                            IDTipoVeiculo = (int)EnumTipoVeiculo.Semiremolque
                        }
                    },
                    mensagemEsperadaId: EnumMensagemPlaca.SemMensagemTratadaNoJs,
                    mensagemEsperadaTexto: null),

                #endregion

                #region BitrenChico

                new ComposicaoArgentinaControllerTests_Cenario(
                    cenario: "Cenário 13",
                    numeroPlaca: (int)EnumPlaca.Placa1,
                    tipoComposicao: (int)EnumTipoComposicao.BitrenChico,
                    modelPlaca: new ModelPlaca()
                    {
                        Placa = new Placa()
                        {
                            IDTipoVeiculo = (int)EnumTipoVeiculo.Carreta
                        }
                    },
                    mensagemEsperadaId: EnumMensagemPlaca.SemMensagemTratadaNoJs,
                    mensagemEsperadaTexto: mensagemErro),

                new ComposicaoArgentinaControllerTests_Cenario(
                    cenario: "Cenário 14",
                    numeroPlaca: (int)EnumPlaca.Placa1,
                    tipoComposicao: (int)EnumTipoComposicao.BitrenChico,
                    modelPlaca: new ModelPlaca()
                    {
                        Placa = new Placa()
                        {
                            IDTipoVeiculo = (int)EnumTipoVeiculo.Tractor
                        }
                    },
                    mensagemEsperadaId: EnumMensagemPlaca.SemMensagemTratadaNoJs,
                    mensagemEsperadaTexto: null),

                new ComposicaoArgentinaControllerTests_Cenario(
                    cenario: "Cenário 15",
                    numeroPlaca: (int)EnumPlaca.Placa2,
                    tipoComposicao: (int)EnumTipoComposicao.BitrenChico,
                    modelPlaca: new ModelPlaca()
                    {
                        Placa = new Placa()
                        {
                            IDTipoVeiculo = (int)EnumTipoVeiculo.Carreta
                        }
                    },
                    mensagemEsperadaId: EnumMensagemPlaca.SemMensagemTratadaNoJs,
                    mensagemEsperadaTexto: mensagemErro),

                new ComposicaoArgentinaControllerTests_Cenario(
                    cenario: "Cenário 16",
                    numeroPlaca: (int)EnumPlaca.Placa2,
                    tipoComposicao: (int)EnumTipoComposicao.BitrenChico,
                    modelPlaca: new ModelPlaca()
                    {
                        Placa = new Placa()
                        {
                            IDTipoVeiculo = (int)EnumTipoVeiculo.Semiremolque
                        }
                    },
                    mensagemEsperadaId: EnumMensagemPlaca.SemMensagemTratadaNoJs,
                    mensagemEsperadaTexto: null),

                new ComposicaoArgentinaControllerTests_Cenario(
                    cenario: "Cenário 17",
                    numeroPlaca: (int)EnumPlaca.Placa3,
                    tipoComposicao: (int)EnumTipoComposicao.BitrenChico,
                    modelPlaca: new ModelPlaca()
                    {
                        Placa = new Placa()
                        {
                            IDTipoVeiculo = (int)EnumTipoVeiculo.Carreta
                        }
                    },
                    mensagemEsperadaId: EnumMensagemPlaca.SemMensagemTratadaNoJs,
                    mensagemEsperadaTexto: mensagemErro),

                new ComposicaoArgentinaControllerTests_Cenario(
                    cenario: "Cenário 18",
                    numeroPlaca: (int)EnumPlaca.Placa3,
                    tipoComposicao: (int)EnumTipoComposicao.BitrenChico,
                    modelPlaca: new ModelPlaca()
                    {
                        Placa = new Placa()
                        {
                            IDTipoVeiculo = (int)EnumTipoVeiculo.Semiremolque
                        }
                    },
                    mensagemEsperadaId: EnumMensagemPlaca.SemMensagemTratadaNoJs,
                    mensagemEsperadaTexto: null),

                #endregion

                #region BitrenGrande

                new ComposicaoArgentinaControllerTests_Cenario(
                    cenario: "Cenário 19",
                    numeroPlaca: (int)EnumPlaca.Placa1,
                    tipoComposicao: (int)EnumTipoComposicao.BitrenGrande,
                    modelPlaca: new ModelPlaca()
                    {
                        Placa = new Placa()
                        {
                            IDTipoVeiculo = (int)EnumTipoVeiculo.Carreta
                        }
                    },
                    mensagemEsperadaId: EnumMensagemPlaca.SemMensagemTratadaNoJs,
                    mensagemEsperadaTexto: mensagemErro),

                new ComposicaoArgentinaControllerTests_Cenario(
                    cenario: "Cenário 20",
                    numeroPlaca: (int)EnumPlaca.Placa1,
                    tipoComposicao: (int)EnumTipoComposicao.BitrenGrande,
                    modelPlaca: new ModelPlaca()
                    {
                        Placa = new Placa()
                        {
                            IDTipoVeiculo = (int)EnumTipoVeiculo.Tractor
                        }
                    },
                    mensagemEsperadaId: EnumMensagemPlaca.SemMensagemTratadaNoJs,
                    mensagemEsperadaTexto: null),

                new ComposicaoArgentinaControllerTests_Cenario(
                    cenario: "Cenário 21",
                    numeroPlaca: (int)EnumPlaca.Placa2,
                    tipoComposicao: (int)EnumTipoComposicao.BitrenGrande,
                    modelPlaca: new ModelPlaca()
                    {
                        Placa = new Placa()
                        {
                            IDTipoVeiculo = (int)EnumTipoVeiculo.Carreta
                        }
                    },
                    mensagemEsperadaId: EnumMensagemPlaca.SemMensagemTratadaNoJs,
                    mensagemEsperadaTexto: mensagemErro),

                new ComposicaoArgentinaControllerTests_Cenario(
                    cenario: "Cenário 22",
                    numeroPlaca: (int)EnumPlaca.Placa2,
                    tipoComposicao: (int)EnumTipoComposicao.BitrenGrande,
                    modelPlaca: new ModelPlaca()
                    {
                        Placa = new Placa()
                        {
                            IDTipoVeiculo = (int)EnumTipoVeiculo.Semiremolque
                        }
                    },
                    mensagemEsperadaId: EnumMensagemPlaca.SemMensagemTratadaNoJs,
                    mensagemEsperadaTexto: null),

                new ComposicaoArgentinaControllerTests_Cenario(
                    cenario: "Cenário 23",
                    numeroPlaca: (int)EnumPlaca.Placa3,
                    tipoComposicao: (int)EnumTipoComposicao.BitrenGrande,
                    modelPlaca: new ModelPlaca()
                    {
                        Placa = new Placa()
                        {
                            IDTipoVeiculo = (int)EnumTipoVeiculo.Carreta
                        }
                    },
                    mensagemEsperadaId: EnumMensagemPlaca.SemMensagemTratadaNoJs,
                    mensagemEsperadaTexto: mensagemErro),

                new ComposicaoArgentinaControllerTests_Cenario(
                    cenario: "Cenário 24",
                    numeroPlaca: (int)EnumPlaca.Placa3,
                    tipoComposicao: (int)EnumTipoComposicao.BitrenGrande,
                    modelPlaca: new ModelPlaca()
                    {
                        Placa = new Placa()
                        {
                            IDTipoVeiculo = (int)EnumTipoVeiculo.Semiremolque
                        }
                    },
                    mensagemEsperadaId: EnumMensagemPlaca.SemMensagemTratadaNoJs,
                    mensagemEsperadaTexto: null),

                #endregion
            };

            #endregion

            foreach (var c in cenarios)
            {
                //Arrange
                ComposicaoArgentinaController composicaoArgentinaController = new ComposicaoArgentinaController();
                PrivateObject obj = new PrivateObject(composicaoArgentinaController);

                //Act
                obj.Invoke("ValidaTipoComposicao", c.NumeroPlaca, c.TipoComposicao, c.ModelPlaca);

                //Assert
                Assert.AreEqual(c.MensagemEsperadaTexto, c.ModelPlaca.Mensagem, $"Falha no {c.Cenario}");
                Assert.AreEqual(c.MensagemEsperadaId, c.ModelPlaca.MensagemId, $"Falha no {c.Cenario}");
            }
        }

        private class ComposicaoArgentinaControllerTests_Cenario
        {
            public ComposicaoArgentinaControllerTests_Cenario(string cenario, int numeroPlaca, int? tipoComposicao, ModelPlaca modelPlaca, string mensagemEsperadaTexto, EnumMensagemPlaca? mensagemEsperadaId)
            {
                Cenario = cenario;
                NumeroPlaca = numeroPlaca;
                TipoComposicao = tipoComposicao;
                ModelPlaca = modelPlaca;
                MensagemEsperadaTexto = mensagemEsperadaTexto;
                MensagemEsperadaId = mensagemEsperadaId;
            }

            public string Cenario { get; set; }

            public int NumeroPlaca { get; set; }

            public int? TipoComposicao { get; set; }

            public ModelPlaca ModelPlaca { get; set; }

            public string MensagemEsperadaTexto { get; set; }

            public EnumMensagemPlaca? MensagemEsperadaId { get; set; }
        }
    }
}
