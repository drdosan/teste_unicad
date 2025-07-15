using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raizen.UniCad.BLLTests.Fakes
{
    public class XLFontFake : IXLFont
    {
        public bool Bold { get { return true; } set { var a = value; } }
        public bool Italic { get { throw new NotImplementedException(); }  set { throw new NotImplementedException(); }  }
        public XLFontUnderlineValues Underline { get { throw new NotImplementedException(); }  set { throw new NotImplementedException(); }  }
        public bool Strikethrough { get { throw new NotImplementedException(); }  set { throw new NotImplementedException(); }  }
        public XLFontVerticalTextAlignmentValues VerticalAlignment { get { throw new NotImplementedException(); }  set { throw new NotImplementedException(); }  }
        public bool Shadow { get { throw new NotImplementedException(); }  set { throw new NotImplementedException(); }  }
        public double FontSize { get { throw new NotImplementedException(); }  set { throw new NotImplementedException(); }  }
        public XLColor FontColor { get { throw new NotImplementedException(); }  set { throw new NotImplementedException(); }  }
        public string FontName { get { throw new NotImplementedException(); }  set { throw new NotImplementedException(); }  }
        public XLFontFamilyNumberingValues FontFamilyNumbering { get { throw new NotImplementedException(); }  set { throw new NotImplementedException(); }  }

        public bool Equals(IXLFont other)
        {
            throw new NotImplementedException();
        }

        public IXLStyle SetBold()
        {
            throw new NotImplementedException();
        }

        public IXLStyle SetBold(bool value)
        {
            throw new NotImplementedException();
        }

        public IXLStyle SetFontColor(XLColor value)
        {
            return new XLStyleFake();
        }

        public IXLStyle SetFontFamilyNumbering(XLFontFamilyNumberingValues value)
        {
            throw new NotImplementedException();
        }

        public IXLStyle SetFontName(string value)
        {
            throw new NotImplementedException();
        }

        public IXLStyle SetFontSize(double value)
        {
            throw new NotImplementedException();
        }

        public IXLStyle SetItalic()
        {
            throw new NotImplementedException();
        }

        public IXLStyle SetItalic(bool value)
        {
            throw new NotImplementedException();
        }

        public IXLStyle SetShadow()
        {
            throw new NotImplementedException();
        }

        public IXLStyle SetShadow(bool value)
        {
            throw new NotImplementedException();
        }

        public IXLStyle SetStrikethrough()
        {
            throw new NotImplementedException();
        }

        public IXLStyle SetStrikethrough(bool value)
        {
            throw new NotImplementedException();
        }

        public IXLStyle SetUnderline()
        {
            throw new NotImplementedException();
        }

        public IXLStyle SetUnderline(XLFontUnderlineValues value)
        {
            throw new NotImplementedException();
        }

        public IXLStyle SetVerticalAlignment(XLFontVerticalTextAlignmentValues value)
        {
            throw new NotImplementedException();
        }
    }
}
