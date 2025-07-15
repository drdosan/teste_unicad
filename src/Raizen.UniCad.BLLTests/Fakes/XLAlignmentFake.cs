using ClosedXML.Excel;
using System;

namespace Raizen.UniCad.BLLTests.Fakes
{
    public class XLAlignmentFake : IXLAlignment
    {
        public XLAlignmentHorizontalValues Horizontal { get { throw new NotImplementedException(); }  set { throw new NotImplementedException(); }  }
        public XLAlignmentVerticalValues Vertical { get { throw new NotImplementedException(); }  set { throw new NotImplementedException(); }  }
        public int Indent { get { throw new NotImplementedException(); }  set { throw new NotImplementedException(); }  }
        public bool JustifyLastLine { get { throw new NotImplementedException(); }  set { throw new NotImplementedException(); }  }
        public XLAlignmentReadingOrderValues ReadingOrder { get { throw new NotImplementedException(); }  set { throw new NotImplementedException(); }  }
        public int RelativeIndent { get { throw new NotImplementedException(); }  set { throw new NotImplementedException(); }  }
        public bool ShrinkToFit { get { throw new NotImplementedException(); }  set { throw new NotImplementedException(); }  }
        public int TextRotation { get { throw new NotImplementedException(); }  set { throw new NotImplementedException(); }  }
        public bool WrapText { get { throw new NotImplementedException(); }  set { throw new NotImplementedException(); }  }
        public bool TopToBottom { get { throw new NotImplementedException(); }  set { throw new NotImplementedException(); }  }

        public bool Equals(IXLAlignment other)
        {
            throw new NotImplementedException();
        }

        public IXLStyle SetHorizontal(XLAlignmentHorizontalValues value)
        {
            //Apenas para passar nos testes
            return new XLStyleFake();
        }

        public IXLStyle SetIndent(int value)
        {
            throw new NotImplementedException();
        }

        public IXLStyle SetJustifyLastLine()
        {
            throw new NotImplementedException();
        }

        public IXLStyle SetJustifyLastLine(bool value)
        {
            throw new NotImplementedException();
        }

        public IXLStyle SetReadingOrder(XLAlignmentReadingOrderValues value)
        {
            throw new NotImplementedException();
        }

        public IXLStyle SetRelativeIndent(int value)
        {
            throw new NotImplementedException();
        }

        public IXLStyle SetShrinkToFit()
        {
            throw new NotImplementedException();
        }

        public IXLStyle SetShrinkToFit(bool value)
        {
            throw new NotImplementedException();
        }

        public IXLStyle SetTextRotation(int value)
        {
            throw new NotImplementedException();
        }

        public IXLStyle SetTopToBottom()
        {
            throw new NotImplementedException();
        }

        public IXLStyle SetTopToBottom(bool value)
        {
            throw new NotImplementedException();
        }

        public IXLStyle SetVertical(XLAlignmentVerticalValues value)
        {
            throw new NotImplementedException();
        }

        public IXLStyle SetWrapText()
        {
            throw new NotImplementedException();
        }

        public IXLStyle SetWrapText(bool value)
        {
            throw new NotImplementedException();
        }
    }
}
