using ClosedXML.Excel;
using System;

namespace Raizen.UniCad.BLLTests.Fakes
{
    public class XLBorderFake : IXLBorder
    {
        public XLBorderStyleValues OutsideBorder { set { throw new NotImplementedException(); }  }
        public XLColor OutsideBorderColor { set { throw new NotImplementedException(); }  }
        public XLBorderStyleValues InsideBorder { set { throw new NotImplementedException(); }  }
        public XLColor InsideBorderColor { set { throw new NotImplementedException(); }  }
        public XLBorderStyleValues LeftBorder { get { throw new NotImplementedException(); }  set { object mock = value; } }
        public XLColor LeftBorderColor { get { throw new NotImplementedException(); }  set { throw new NotImplementedException(); }  }
        public XLBorderStyleValues RightBorder { get { throw new NotImplementedException(); }  set { object mock = value; } }
        public XLColor RightBorderColor { get { throw new NotImplementedException(); }  set { throw new NotImplementedException(); }  }
        public XLBorderStyleValues TopBorder { get { throw new NotImplementedException(); }  set { object mock = value; } }
        public XLColor TopBorderColor { get { throw new NotImplementedException(); }  set { throw new NotImplementedException(); }  }
        public XLBorderStyleValues BottomBorder { get { throw new NotImplementedException(); }  set { object mock = value; } }
        public XLColor BottomBorderColor { get { throw new NotImplementedException(); }  set { throw new NotImplementedException(); }  }
        public bool DiagonalUp { get { throw new NotImplementedException(); }  set { throw new NotImplementedException(); }  }
        public bool DiagonalDown { get { throw new NotImplementedException(); }  set { throw new NotImplementedException(); }  }
        public XLBorderStyleValues DiagonalBorder { get { throw new NotImplementedException(); }  set { throw new NotImplementedException(); }  }
        public XLColor DiagonalBorderColor { get { throw new NotImplementedException(); }  set { throw new NotImplementedException(); }  }

        public bool Equals(IXLBorder other)
        {
            throw new NotImplementedException();
        }

        public IXLStyle SetBottomBorder(XLBorderStyleValues value)
        {
            throw new NotImplementedException();
        }

        public IXLStyle SetBottomBorderColor(XLColor value)
        {
            throw new NotImplementedException();
        }

        public IXLStyle SetDiagonalBorder(XLBorderStyleValues value)
        {
            throw new NotImplementedException();
        }

        public IXLStyle SetDiagonalBorderColor(XLColor value)
        {
            throw new NotImplementedException();
        }

        public IXLStyle SetDiagonalDown()
        {
            throw new NotImplementedException();
        }

        public IXLStyle SetDiagonalDown(bool value)
        {
            throw new NotImplementedException();
        }

        public IXLStyle SetDiagonalUp()
        {
            throw new NotImplementedException();
        }

        public IXLStyle SetDiagonalUp(bool value)
        {
            throw new NotImplementedException();
        }

        public IXLStyle SetInsideBorder(XLBorderStyleValues value)
        {
            throw new NotImplementedException();
        }

        public IXLStyle SetInsideBorderColor(XLColor value)
        {
            throw new NotImplementedException();
        }

        public IXLStyle SetLeftBorder(XLBorderStyleValues value)
        {
            throw new NotImplementedException();
        }

        public IXLStyle SetLeftBorderColor(XLColor value)
        {
            throw new NotImplementedException();
        }

        public IXLStyle SetOutsideBorder(XLBorderStyleValues value)
        {
            throw new NotImplementedException();
        }

        public IXLStyle SetOutsideBorderColor(XLColor value)
        {
            throw new NotImplementedException();
        }

        public IXLStyle SetRightBorder(XLBorderStyleValues value)
        {
            throw new NotImplementedException();
        }

        public IXLStyle SetRightBorderColor(XLColor value)
        {
            throw new NotImplementedException();
        }

        public IXLStyle SetTopBorder(XLBorderStyleValues value)
        {
            throw new NotImplementedException();
        }

        public IXLStyle SetTopBorderColor(XLColor value)
        {
            throw new NotImplementedException();
        }
    }
}
