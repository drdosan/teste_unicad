using Microsoft.VisualStudio.TestTools.UnitTesting;
using Raizen.UniCad.BLL.Extensions;

namespace Raizen.UniCad.BLLTests.BLL
{
    [TestClass()]
    public class StringExtensionsTest
    {
        [TestMethod]
        public void RemoverZerosAEsquerda()
        {
            var acs = "000123456";
            var tr = "TR036066";
            var cs = "cs1234567";
            var aleatorio = "@sda2dsad20023";

            Assert.AreEqual("123456", acs.RemoverZerosAEsquerda());
            Assert.AreEqual(tr, tr.RemoverZerosAEsquerda());
            Assert.AreEqual(cs, cs.RemoverZerosAEsquerda());
            Assert.AreEqual(aleatorio, aleatorio.RemoverZerosAEsquerda());
        }
    }
}
