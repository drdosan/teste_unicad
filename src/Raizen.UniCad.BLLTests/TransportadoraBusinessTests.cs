using Microsoft.VisualStudio.TestTools.UnitTesting;
using Raizen.UniCad.BLL;
using Raizen.UniCad.BLLTests.Bases;

namespace Raizen.UniCad.BLLTests
{
    [TestClass()]
    public class TransportadoraBusinessTests : BaseTest
    {
        readonly TransportadoraBusiness _bll = new TransportadoraBusiness();
        
        [IgnoreAttribute("Teste desligado até resolver a questão da ordem de execução no Jenkins")]
        [TestMethod()]
        public void ImportarTest()
        {
            Assert.AreEqual(_bll.Importar(null, Model.EnumEmpresa.Combustiveis), 0);
            Assert.AreEqual(_bll.Importar(null, Model.EnumEmpresa.EAB), 0);
        }
    }
}