using Microsoft.VisualStudio.TestTools.UnitTesting;
using Raizen.UniCad.BLL;
using Raizen.UniCad.BLLTests.Bases;

namespace Raizen.UniCad.BLLTests
{
    [TestClass()]
    public class ClienteAcsBusinessTests : BaseTest
    {
        readonly ClienteAcsBusiness bll = new ClienteAcsBusiness();
        //[TestMethod()]
        //public void AutenticarTest()
        //{
        //    string Usuario, tokenSSo;
        //    var retorno = this.bll.Autenticar("Dv", "Token", out Usuario, out tokenSSo, Model.EnumPais.Brasil);
        //    Assert.IsNotNull(retorno);
        //}

        //[TestMethod()]
        //public void LoginUserSystemTest()
        //{
        //    var retorno = this.bll.LoginUserSystem("ClienteAcsUnicadServico", "Raizen@17", "UNICA");
        //    Assert.IsNotNull(retorno);
        //}
    }
}