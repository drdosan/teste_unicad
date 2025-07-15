using Microsoft.VisualStudio.TestTools.UnitTesting;
using Raizen.UniCad.BLL.Extensions;
using System;
using System.Reflection;

namespace Raizen.UniCad.BLLTests
{
    [TestClass]
    public class PropertyExtensionTest
    {
        class ClasseExemplo
        {
            public int PropInt { get; set; }
            
            public short PropShort { get; set; }
            
            public long PropLong { get; set; }

            public decimal ProdDecimal { get; set; }
            
            public double PropDouble { get; set; }
            
            public string PropString { get; set; }
            
            public DateTime PropDateTime { get; set; }

            public TimeSpan PropTimeSpan { get; set; }

            public bool PropBool{ get; set; }

            public object PropObject { get; set; }

        }

        [TestMethod]
        public void SetDefaultsTest()
        {
            var classeExemplo = new ClasseExemplo();

            classeExemplo.SetGetDefaults();

            foreach (PropertyInfo p in classeExemplo.GetType().GetProperties())
                Assert.AreEqual(p.GetDefaultValue(), p.GetValue(classeExemplo));
        }
    }
}
