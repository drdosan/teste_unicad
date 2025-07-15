using Microsoft.VisualStudio.TestTools.UnitTesting;
using Raizen.Framework.UserSystem.Client;
using Raizen.UniCad.BLL;
using Raizen.UniCad.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Raizen.UniCad.BLLTests;
using Raizen.UniCad.BLLTests.Bases;

namespace Raizen.UniCad.BLLTests
{
    [TestClass()]
    public class UsuarioBusinessTests : BaseTest
    {
        private readonly UsuarioBusiness _usuarioBll = new UsuarioBusiness();
        
        [IgnoreAttribute("Teste desligado até resolver a questão da ordem de execução no Jenkins")]
        [TestMethod()]
        public void ListarUsuarioTest()
        {
            var ret = _usuarioBll.ListarUsuario(new Model.Filtro.UsuarioFiltro(), new Framework.Models.PaginadorModel());
            Assert.IsNotNull(ret);
        }

        [IgnoreAttribute("Teste desligado até resolver a questão da ordem de execução no Jenkins")]
        [TestMethod()]
        public void ListarUsuarioCountTest()
        {
            var ret = _usuarioBll.ListarUsuarioCount(new Model.Filtro.UsuarioFiltro());
            Assert.IsNotNull(ret);
        }

        [IgnoreAttribute("Teste desligado até resolver a questão da ordem de execução no Jenkins")]
        [TestMethod()]
        public void AdicionarUsuarioTest()
        {
            var usuario = FuncoesCompartilhadasTests.CriarUsuario("teste usuario", "teste", "teste@teste.com",
                "Administrador - TI", "CIF", true, EnumEmpresa.Combustiveis);

            _usuarioBll.AdicionarUsuario(usuario, false);

            _usuarioBll.AtualizarUsuario(usuario, false);

            var msg = _usuarioBll.ResetarSenha(usuario.ID);
            Assert.IsNotNull(msg);            
            _usuarioBll.ExcluirLista(p => p.ID == usuario.ID);
            Assert.IsTrue(true);
        }
    }
}