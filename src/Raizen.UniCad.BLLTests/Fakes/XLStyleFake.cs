using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raizen.UniCad.BLLTests.Fakes
{
        public class XLStyleFake : IXLStyle
    {
        public IXLAlignment Alignment { get { return new XLAlignmentFake(); }  set { object mock = value; }  }
        public IXLBorder Border { get { return new XLBorderFake(); }  set { object mock = value; }  }
        public IXLFill Fill { get { return new XLFillFake(); }  set { object mock = value; }  }
        public IXLFont Font { get { return new XLFontFake(); }  set { object mock = value; }  }
        public IXLNumberFormat NumberFormat { get { throw new NotImplementedException(); }  set { throw new NotImplementedException(); }  }

        public IXLNumberFormat DateFormat { get { throw new NotImplementedException(); } }

        public IXLProtection Protection { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }

        public bool Equals(IXLStyle other)
        {
            throw new NotImplementedException();
        }
    }
}
