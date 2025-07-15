using Microsoft.VisualStudio.TestTools.UnitTesting;
using Raizen.UniCad.BLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Raizen.UniCad.BLLTests.Bases;

namespace Raizen.UniCad.BLLTests
{
    [TestClass()]
    public class UsuarioClienteBusinessTests : BaseTest
    {
        readonly UsuarioClienteBusiness bll = new UsuarioClienteBusiness();

        [IgnoreAttribute("Teste desligado até resolver acesso ao banco")]
        [TestMethod()]
        public void ListarClientesPorUsuarioTest()
        {
            var lista = bll.Listar();
            var idu = lista.Last().IDUsuario;
            var user = new UsuarioBusiness().Listar(p => p.ID == idu).First();

            var result = bll.ListarClientesPorUsuario(user.ID, null);
            Assert.IsNotNull(result);
        }
    }
}