using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Raizen.UniCad.BLL;
using Raizen.UniCad.BLLTests.Bases;
using System.Web.Routing;
using Raizen.UniCad.Web.Util;
using Raizen.UniCad.Web.Models.Filtros;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace Raizen.UniCad.BLLTests
{
    [TestClass()]
    public class ClienteBusinessTests : BaseTest
    {
        readonly ClienteBusiness bll = new ClienteBusiness();

        [IgnoreAttribute("Teste desligado até resolver a questão da ordem de execução no Jenkins")]
        [TestMethod()]
        public void ImportarTest()
        {
            List<String> imbs = new List<string>();
            imbs.Add("0000000078");

            Assert.AreEqual(bll.Importar(null, imbs, Model.EnumEmpresa.Combustiveis), 0);
            Assert.AreEqual(bll.Importar(null, imbs, Model.EnumEmpresa.EAB), 0);
        }

        [IgnoreAttribute("Teste desligado até resolver a questão da ordem de execução no Jenkins")]
        [TestMethod()]
        public void ListarClientesTest()
        {
            var result = bll.ListarClientes(new Model.Filtro.ClienteFiltro());
            Assert.IsNotNull(result);
        }

        //[TestMethod()]
        //public void ListarClientes2Test()
        //{

        //    Assert.IsTrue(true);
        //}
    }
}