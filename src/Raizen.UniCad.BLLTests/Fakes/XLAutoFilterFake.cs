using ClosedXML.Excel;
using System;

namespace Raizen.UniCad.BLLTests.Fakes
{
    public class XLAutoFilterFake : IXLAutoFilter
    {
        public bool Sorted { get { throw new NotImplementedException(); }  set { throw new NotImplementedException(); }  }
        public XLSortOrder SortOrder { get { throw new NotImplementedException(); }  set { throw new NotImplementedException(); }  }
        public int SortColumn { get { throw new NotImplementedException(); }  set { throw new NotImplementedException(); }  }

        public IXLFilterColumn Column(string column)
        {
            throw new NotImplementedException();
        }

        public IXLFilterColumn Column(int column)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public IXLAutoFilter Sort(int columnToSortBy = 1, XLSortOrder sortOrder = XLSortOrder.Ascending, bool matchCase = false, bool ignoreBlanks = true)
        {
            throw new NotImplementedException();
        }
    }
}
