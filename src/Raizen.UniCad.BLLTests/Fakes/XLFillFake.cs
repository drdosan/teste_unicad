using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raizen.UniCad.BLLTests.Fakes
{
    public class XLFillFake : IXLFill
    {
        public XLColor BackgroundColor { get { throw new NotImplementedException(); }  set { throw new NotImplementedException(); }  }
        public XLColor PatternColor { get { throw new NotImplementedException(); }  set { throw new NotImplementedException(); }  }
        public XLColor PatternBackgroundColor { get { throw new NotImplementedException(); }  set { throw new NotImplementedException(); }  }
        public XLFillPatternValues PatternType { get { return XLFillPatternValues.DarkDown;  } set { object mock = value; } }

        public bool Equals(IXLFill other)
        {
            throw new NotImplementedException();
        }

        public IXLStyle SetBackgroundColor(XLColor value)
        {
            //Apenas para passar nos testes
            return new XLStyleFake();
        }

        public IXLStyle SetPatternBackgroundColor(XLColor value)
        {
            //Apenas para passar nos testes
            return new XLStyleFake();
        }

        public IXLStyle SetPatternColor(XLColor value)
        {
            throw new NotImplementedException();
        }

        public IXLStyle SetPatternType(XLFillPatternValues value)
        {
            //Apenas para passar nos testes
            return new XLStyleFake();
        }
    }
}
