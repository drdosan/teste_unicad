using ClosedXML.Excel;
using System;

namespace Raizen.UniCad.BLLTests.Fakes
{
    public class XLRowFake : IXLRow
    {
        public double Height { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }

        public bool IsHidden{ get { throw new NotImplementedException(); } }

        public int OutlineLevel { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }

        public IXLWorksheet Worksheet{ get { throw new NotImplementedException(); } }

        public IXLRangeAddress RangeAddress{ get { throw new NotImplementedException(); } }

        public object Value { set { throw new NotImplementedException(); } }
        public XLCellValues DataType { set { throw new NotImplementedException(); } }
        public string FormulaA1 { set { throw new NotImplementedException(); } }
        public string FormulaR1C1 { set { throw new NotImplementedException(); } }
        public IXLStyle Style { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }
        public bool ShareString { set { throw new NotImplementedException(); } }

        public IXLHyperlinks Hyperlinks{ get { throw new NotImplementedException(); } }

        public IXLConditionalFormat AddConditionalFormat()
        {
            throw new NotImplementedException();
        }

        public IXLRow AddHorizontalPageBreak()
        {
            throw new NotImplementedException();
        }

        public IXLRange AddToNamed(string rangeName)
        {
            throw new NotImplementedException();
        }

        public IXLRange AddToNamed(string rangeName, XLScope scope)
        {
            throw new NotImplementedException();
        }

        public IXLRange AddToNamed(string rangeName, XLScope scope, string comment)
        {
            throw new NotImplementedException();
        }

        public IXLRow AdjustToContents()
        {
            //Apenas para passar nos testes
            return this;
        }

        public IXLRow AdjustToContents(int startColumn)
        {
            throw new NotImplementedException();
        }

        public IXLRow AdjustToContents(int startColumn, int endColumn)
        {
            throw new NotImplementedException();
        }

        public IXLRow AdjustToContents(double minHeight, double maxHeight)
        {
            throw new NotImplementedException();
        }

        public IXLRow AdjustToContents(int startColumn, double minHeight, double maxHeight)
        {
            throw new NotImplementedException();
        }

        public IXLRow AdjustToContents(int startColumn, int endColumn, double minHeight, double maxHeight)
        {
            throw new NotImplementedException();
        }

        public IXLRange AsRange()
        {
            throw new NotImplementedException();
        }

        public IXLCell Cell(int columnNumber)
        {
            throw new NotImplementedException();
        }

        public IXLCell Cell(string columnLetter)
        {
            throw new NotImplementedException();
        }

        public int CellCount()
        {
            throw new NotImplementedException();
        }

        public IXLCells Cells(string cellsInRow)
        {
            throw new NotImplementedException();
        }

        public IXLCells Cells(int firstColumn, int lastColumn)
        {
            throw new NotImplementedException();
        }

        public IXLCells Cells(string firstColumn, string lastColumn)
        {
            throw new NotImplementedException();
        }

        public IXLCells Cells()
        {
            throw new NotImplementedException();
        }

        public IXLCells Cells(bool usedCellsOnly)
        {
            throw new NotImplementedException();
        }

        public IXLCells Cells(bool usedCellsOnly, bool includeFormats)
        {
            throw new NotImplementedException();
        }

        public IXLCells Cells(Func<IXLCell, bool> predicate)
        {
            throw new NotImplementedException();
        }

        public IXLCells CellsUsed()
        {
            throw new NotImplementedException();
        }

        public IXLCells CellsUsed(bool includeFormats)
        {
            throw new NotImplementedException();
        }

        public IXLCells CellsUsed(Func<IXLCell, bool> predicate)
        {
            throw new NotImplementedException();
        }

        public IXLCells CellsUsed(bool includeFormats, Func<IXLCell, bool> predicate)
        {
            throw new NotImplementedException();
        }

        public IXLRow Clear(XLClearOptions clearOptions = XLClearOptions.ContentsAndFormats)
        {
            throw new NotImplementedException();
        }

        public IXLRow Collapse()
        {
            throw new NotImplementedException();
        }

        public bool Contains(string rangeAddress)
        {
            throw new NotImplementedException();
        }

        public bool Contains(IXLRangeBase range)
        {
            throw new NotImplementedException();
        }

        public bool Contains(IXLCell cell)
        {
            throw new NotImplementedException();
        }

        public IXLRangeRow CopyTo(IXLCell cell)
        {
            throw new NotImplementedException();
        }

        public IXLRangeRow CopyTo(IXLRangeBase range)
        {
            throw new NotImplementedException();
        }

        public IXLRow CopyTo(IXLRow row)
        {
            throw new NotImplementedException();
        }

        public IXLPivotTable CreatePivotTable(IXLCell targetCell)
        {
            throw new NotImplementedException();
        }

        public IXLPivotTable CreatePivotTable(IXLCell targetCell, string name)
        {
            throw new NotImplementedException();
        }

        public void Delete()
        {
            throw new NotImplementedException();
        }

        public void DeleteComments()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public IXLRow Expand()
        {
            throw new NotImplementedException();
        }

        public IXLCell FirstCell()
        {
            throw new NotImplementedException();
        }

        public IXLCell FirstCellUsed()
        {
            throw new NotImplementedException();
        }

        public IXLCell FirstCellUsed(bool includeFormats)
        {
            throw new NotImplementedException();
        }

        public IXLCell FirstCellUsed(Func<IXLCell, bool> predicate)
        {
            throw new NotImplementedException();
        }

        public IXLCell FirstCellUsed(bool includeFormats, Func<IXLCell, bool> predicate)
        {
            throw new NotImplementedException();
        }

        public IXLRow Group()
        {
            throw new NotImplementedException();
        }

        public IXLRow Group(bool collapse)
        {
            throw new NotImplementedException();
        }

        public IXLRow Group(int outlineLevel)
        {
            throw new NotImplementedException();
        }

        public IXLRow Group(int outlineLevel, bool collapse)
        {
            throw new NotImplementedException();
        }

        public IXLRow Hide()
        {
            throw new NotImplementedException();
        }

        public IXLRows InsertRowsAbove(int numberOfRows)
        {
            throw new NotImplementedException();
        }

        public IXLRows InsertRowsBelow(int numberOfRows)
        {
            throw new NotImplementedException();
        }

        public bool Intersects(string rangeAddress)
        {
            throw new NotImplementedException();
        }

        public bool Intersects(IXLRangeBase range)
        {
            throw new NotImplementedException();
        }

        public bool IsEmpty()
        {
            throw new NotImplementedException();
        }

        public bool IsEmpty(bool includeFormats)
        {
            throw new NotImplementedException();
        }

        public bool IsMerged()
        {
            throw new NotImplementedException();
        }

        public IXLCell LastCell()
        {
            throw new NotImplementedException();
        }

        public IXLCell LastCellUsed()
        {
            throw new NotImplementedException();
        }

        public IXLCell LastCellUsed(bool includeFormats)
        {
            throw new NotImplementedException();
        }

        public IXLCell LastCellUsed(Func<IXLCell, bool> predicate)
        {
            throw new NotImplementedException();
        }

        public IXLCell LastCellUsed(bool includeFormats, Func<IXLCell, bool> predicate)
        {
            throw new NotImplementedException();
        }

        public IXLRange Merge()
        {
            throw new NotImplementedException();
        }

        public IXLRange Merge(bool checkIntersect)
        {
            throw new NotImplementedException();
        }

        public IXLRangeRow Row(int start, int end)
        {
            throw new NotImplementedException();
        }

        public IXLRangeRow Row(IXLCell start, IXLCell end)
        {
            throw new NotImplementedException();
        }

        public IXLRow RowAbove()
        {
            throw new NotImplementedException();
        }

        public IXLRow RowAbove(int step)
        {
            throw new NotImplementedException();
        }

        public IXLRow RowBelow()
        {
            throw new NotImplementedException();
        }

        public IXLRow RowBelow(int step)
        {
            throw new NotImplementedException();
        }

        public int RowNumber()
        {
            throw new NotImplementedException();
        }

        public IXLRangeRows Rows(string columns)
        {
            throw new NotImplementedException();
        }

        public IXLRangeRow RowUsed(bool includeFormats = false)
        {
            throw new NotImplementedException();
        }

        public void Select()
        {
            throw new NotImplementedException();
        }

        public IXLAutoFilter SetAutoFilter()
        {
            throw new NotImplementedException();
        }

        public IXLRow SetDataType(XLCellValues dataType)
        {
            throw new NotImplementedException();
        }

        public IXLDataValidation SetDataValidation()
        {
            throw new NotImplementedException();
        }

        public IXLRangeBase SetValue<T>(T value)
        {
            throw new NotImplementedException();
        }

        public IXLRow Sort()
        {
            throw new NotImplementedException();
        }

        public IXLRow SortLeftToRight(XLSortOrder sortOrder = XLSortOrder.Ascending, bool matchCase = false, bool ignoreBlanks = true)
        {
            throw new NotImplementedException();
        }

        public IXLRow Ungroup()
        {
            throw new NotImplementedException();
        }

        public IXLRow Ungroup(bool fromAll)
        {
            throw new NotImplementedException();
        }

        public IXLRow Unhide()
        {
            throw new NotImplementedException();
        }

        public IXLRange Unmerge()
        {
            throw new NotImplementedException();
        }

        IXLRangeBase IXLRangeBase.Clear(XLClearOptions clearOptions)
        {
            throw new NotImplementedException();
        }
    }
}
