using Microsoft.VisualStudio.TestTools.UnitTesting;
using Raizen.UniCad.Model;
using Raizen.UniCad.Web.Models;
using System.Collections.Generic;

namespace Raizen.UniCad.BLLTests.Models
{
    [TestClass]
    public class ModelUsuarioTests
    {
        [TestMethod]
        public void PerfilExternoAutorizado_Test()
        {
            /* Foi acordado junto ao time de arquitetura da Raizen o uso desta abordagem dos cenários abaixo,
               visto que o Jenkins da Raizen não aceita o uso do atributo [DataRows()] nos testes */
            #region Cenários

            var cenarios = new List<PerfilExternoAutorizado_Cenario>()
            {
                new PerfilExternoAutorizado_Cenario(
                    cenario: "Cenário 1",
                    usuario: new Usuario { Perfil = EnumPerfil.TRANSPORTADORA},
                    resultadoEsperado: true),

                new PerfilExternoAutorizado_Cenario(
                    cenario: "Cenário 2",
                    usuario: new Usuario { Perfil = EnumPerfil.TRANSPORTADORA_ARGENTINA },
                    resultadoEsperado: true),

                new PerfilExternoAutorizado_Cenario(
                    cenario: "Cenário 3",
                    usuario: new Usuario { Perfil = EnumPerfil.CLIENTE_EAB },
                    resultadoEsperado: true),

                new PerfilExternoAutorizado_Cenario(
                    cenario: "Cenário 4",
                    usuario: new Usuario { Perfil = EnumPerfil.CLIENTE_ACS },
                    resultadoEsperado: true),

                new PerfilExternoAutorizado_Cenario(
                    cenario: "Cenário 5",
                    usuario: new Usuario { Perfil = EnumPerfil.CLIENTE_ACS_ARGENTINA },
                    resultadoEsperado: true),

                new PerfilExternoAutorizado_Cenario(
                    cenario: "Cenário 6",
                    usuario: new Usuario { Perfil = EnumPerfil.QUALITY },
                    resultadoEsperado: true),
            };

            #endregion

            foreach (var c in cenarios)
            {
                //Arrange
                ModelUsuario modelUsuario = new ModelUsuario();
                modelUsuario.Usuario = c.Usuario;
                PrivateObject obj = new PrivateObject(modelUsuario);

                //Act
                bool result = (bool)obj.Invoke("PerfilExternoAutorizado");

                //Assert 
                Assert.AreEqual(c.ResultadoEsperado, result, $"Erro no cenário {c.Cenario}");
            }
        }

        private class PerfilExternoAutorizado_Cenario
        {
            public PerfilExternoAutorizado_Cenario(string cenario, bool resultadoEsperado, Usuario usuario)
            {
                Cenario = cenario;
                ResultadoEsperado = resultadoEsperado;
                Usuario = usuario;
            }

            public string Cenario { get; set; }

            public bool ResultadoEsperado { get; set; }

            public Usuario Usuario { get; set; }

        }
    }
}
