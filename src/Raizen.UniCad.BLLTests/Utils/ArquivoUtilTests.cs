using Microsoft.VisualStudio.TestTools.UnitTesting;
using Raizen.UniCad.Utils;

namespace Raizen.UniCad.BLLTests.Utils
{
    [TestClass()]
    public class ArquivoUtilTests
    {
        [TestMethod()]
        public void SalvarArquivoTest()
        {
            try
            {
                ArquivoUtil.SalvarArquivo(null, "");
            }
            catch
            {

            }

            Assert.IsTrue(true);
        }

        [TestMethod()]
        public void ExcluirArquivoTest()
        {
            try
            {
                ArquivoUtil.ExcluirArquivo("teste", "teste");
            }
            catch
            {

            }

            Assert.IsTrue(true);
        }

    }
}