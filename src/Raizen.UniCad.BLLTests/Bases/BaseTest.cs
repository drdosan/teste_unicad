using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Raizen.UniCad.BLLTests.Bases
{
    [TestClass]
    public abstract class BaseTest
    {
        // Hack para forçar a carga da dependência do SqlProviderServices
        // Mais informações: http://robsneuron.blogspot.com.br/2013/11/entity-framework-upgrade-to-6.html
        private static readonly Type _dependency = typeof(System.Data.Entity.SqlServer.SqlProviderServices);

        protected BaseTest() { }
    }
}