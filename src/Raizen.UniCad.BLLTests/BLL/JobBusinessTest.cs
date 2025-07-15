using Microsoft.VisualStudio.TestTools.UnitTesting;
using Raizen.UniCad.BLL;
using Raizen.UniCad.Model;
using System;
using System.Collections.Generic;

namespace Raizen.UniCad.BLLTests
{
    [TestClass]
    public class JobBusinessTest
    {
        [TestMethod]
        public void CorrigirID_Test()
        {
            /* Foi acordado junto ao time de arquitetura da Raizen o uso desta abordagem dos cenários abaixo,
               visto que o Jenkins da Raizen não aceita o uso do atributo [DataRows()] nos testes */
            #region Cenários

            var cenarios = new List<CorrigirID_Cenario>()
            {
                new CorrigirID_Cenario(
                    cenario: "0",
                    idJobEsperado: -1,
                    nomeJobEsperado: "NOME_QUALQUER_JOB_INEXISTENTE",
                    job: new Job
                    {
                        Nome = "NOME_QUALQUER_JOB_INEXISTENTE"
                    }
                ),

                new CorrigirID_Cenario(
                    cenario: "1",
                    idJobEsperado: (int)EnumJob.Cliente,
                    nomeJobEsperado: "CLIENTE",
                    job: new Job
                    {
                        Nome = "CLIENTE"
                    }
                ),

                new CorrigirID_Cenario(
                    cenario: "2",
                    idJobEsperado: (int)EnumJob.Transportadora,
                    nomeJobEsperado: "TRANSPORTADORA",
                    job: new Job
                    {
                        Nome = "TRANSPORTADORA"
                    }
                ),

                new CorrigirID_Cenario(
                    cenario: "3",
                    idJobEsperado: (int)EnumJob.Documento,
                    nomeJobEsperado: "DOCUMENTO",
                    job: new Job
                    {
                        Nome = "DOCUMENTO"
                    }
                ),

                new CorrigirID_Cenario(
                    cenario: "4",
                    idJobEsperado: (int)EnumJob.Importacao,
                    nomeJobEsperado: "IMPORTACAO",
                    job: new Job
                    {
                        Nome = "IMPORTACAO"
                    }
                ),

                new CorrigirID_Cenario(
                    cenario: "5",
                    idJobEsperado: (int)EnumJob.IntegrarComposicaoPendenteSAP,
                    nomeJobEsperado: "PENDENTE_SAP",
                    job: new Job
                    {
                        Nome = "PENDENTE_SAP"
                    }
                ),

                new CorrigirID_Cenario(
                    cenario: "6",
                    idJobEsperado: (int)EnumJob.ExcluirArquivos,
                    nomeJobEsperado: "EXCLUIR_ARQUIVOS",
                    job: new Job
                    {
                        Nome = "EXCLUIR_ARQUIVOS"
                    }
                ),

                new CorrigirID_Cenario(
                    cenario: "7",
                    idJobEsperado: (int)EnumJob.SincronizarMotoritas,
                    nomeJobEsperado: "SINCRONIZAR_MOTORITAS_SAP",
                    job: new Job
                    {
                        Nome = "SINCRONIZAR_MOTORITAS_SAP"
                    }
                ),

                new CorrigirID_Cenario(
                    cenario: "8",
                    idJobEsperado: (int)EnumJob.EnviarEmailTeste,
                    nomeJobEsperado: "ENVIAR_EMAIL_TESTE",
                    job: new Job
                    {
                        Nome = "ENVIAR_EMAIL_TESTE"
                    }
                ),

                new CorrigirID_Cenario(
                    cenario: "9",
                    idJobEsperado: (int)EnumJob.ComposicaoDocumentoAlerta,
                    nomeJobEsperado: "COMPOSICAO_DOCUMENTO_ALERTA",
                    job: new Job
                    {
                        Nome = "COMPOSICAO_DOCUMENTO_ALERTA"
                    }
                ),

                new CorrigirID_Cenario(
                    cenario: "10",
                    idJobEsperado: (int)EnumJob.ComposicaoDocumento,
                    nomeJobEsperado: "COMPOSICAO_DOCUMENTO_VENCIDO",
                    job: new Job
                    {
                        Nome = "COMPOSICAO_DOCUMENTO_VENCIDO"
                    }
                ),

                new CorrigirID_Cenario(
                    cenario: "11",
                    idJobEsperado: (int)EnumJob.MotoristaDocumentoAlerta,
                    nomeJobEsperado: "MOTORISTA_DOCUMENTO_ALERTA",
                    job: new Job
                    {
                        Nome = "MOTORISTA_DOCUMENTO_ALERTA"
                    }
                ),

                new CorrigirID_Cenario(
                    cenario: "12",
                    idJobEsperado: (int)EnumJob.MotoristaDocumentoVencido,
                    nomeJobEsperado: "MOTORISTA_DOCUMENTO_VENCIDO",
                    job: new Job
                    {
                        Nome = "MOTORISTA_DOCUMENTO_VENCIDO"
                    }
                ),
            };

            #endregion

            foreach (var c in cenarios)
            {
                //Arrange
                var jobBusiness = new JobBusiness();

                //Act
                jobBusiness.CorrigirID(c.Job);

                //Assert
                Assert.AreEqual(c.IdJobEsperado, c.Job.ID, $"Falha ao comparar IDs no cenário {c.Cenario}");
                Assert.AreEqual(c.NomeJobEsperado, c.Job.Nome, $"Falha ao comparar Nomes no cenário {c.Cenario}");
            }
        }

        private class CorrigirID_Cenario
        {
            public CorrigirID_Cenario(string cenario, Job job, int idJobEsperado, string nomeJobEsperado)
            {
                Cenario = cenario;
                Job = job;
                IdJobEsperado = idJobEsperado;
                NomeJobEsperado = nomeJobEsperado;
            }

            public string Cenario { get; internal set; }

            public Job Job { get; internal set; }

            public int IdJobEsperado { get; set; }

            public string NomeJobEsperado { get; internal set; }
        }
    }
}
