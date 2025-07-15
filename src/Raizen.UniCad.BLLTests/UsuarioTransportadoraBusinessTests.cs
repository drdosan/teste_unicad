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
    public class UsuarioTransportadoraBusinessTests : BaseTest
    {
        readonly UsuarioTransportadoraBusiness bll = new UsuarioTransportadoraBusiness();
        [TestMethod()]
        public void ListarTransportadorasPorUsuarioTest()
        {
            var lista = bll.Listar(p => p.IDTransportadora > 100 && p.IDTransportadora < 120);
            var idu = lista.First().IDUsuario;
            var user = new UsuarioBusiness().Listar(p => p.ID == idu).First();

            var result = bll.ListarTransportadorasPorUsuario(user.ID, user.IDEmpresa);
            Assert.IsNotNull(result);
        }
    }
}