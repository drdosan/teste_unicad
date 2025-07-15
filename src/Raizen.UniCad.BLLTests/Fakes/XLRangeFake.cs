using ClosedXML.Excel;
using System;

namespace Raizen.UniCad.BLLTests.Fakes
{
    public class XLRangeFake : IXLRange
    {
        public IXLSortElements SortRows { get { throw new NotImplementedException(); } }

        public IXLSortElements SortColumns { get { throw new NotImplementedException(); } }

        public IXLWorksheet Worksheet { get { throw new NotImplementedException(); } }

        public IXLRangeAddress RangeAddress { get { throw new NotImplementedException(); } }

        public object Value { set { throw new NotImplementedException(); }  }
        public XLCellValues DataType { set { throw new NotImplementedException(); }  }
        public string FormulaA1 { set { throw new NotImplementedException(); }  }
        public string FormulaR1C1 { set { throw new NotImplementedException(); }  }
        public IXLStyle Style 
        {
            //Apenas para passar nos testes
            get
            {
                return new XLStyleFake();
            } 
            set 
            {
                object test = value;
            }
        }

        public bool ShareString { set { throw new NotImplementedException(); }  }

        public IXLHyperlinks Hyperlinks { get { throw new NotImplementedException(); } }

        public IXLConditionalFormat AddConditionalFormat()
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

        public IXLRange AsRange()
        {
            throw new NotImplementedException();
        }

        public IXLTable AsTable()
        {
            throw new NotImplementedException();
        }

        public IXLTable AsTable(string name)
        {
            throw new NotImplementedException();
        }

        public IXLCell Cell(int row, int column)
        {
            throw new NotImplementedException();
        }

        public IXLCell Cell(string cellAddressInRange)
        {
            throw new NotImplementedException();
        }

        public IXLCell Cell(int row, string column)
        {
            throw new NotImplementedException();
        }

        public IXLCell Cell(IXLAddress cellAddressInRange)
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

        public IXLCells Cells(string cells)
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

        public IXLRange Clear(XLClearOptions clearOptions = XLClearOptions.ContentsAndFormats)
        {
            throw new NotImplementedException();
        }

        public IXLRangeColumn Column(int column)
        {
            throw new NotImplementedException();
        }

        public IXLRangeColumn Column(string column)
        {
            throw new NotImplementedException();
        }

        public int ColumnCount()
        {
            throw new NotImplementedException();
        }

        public IXLRangeColumns Columns(Func<IXLRangeColumn, bool> predicate = null)
        {
            throw new NotImplementedException();
        }

        public IXLRangeColumns Columns(int firstColumn, int lastColumn)
        {
            throw new NotImplementedException();
        }

        public IXLRangeColumns Columns(string firstColumn, string lastColumn)
        {
            throw new NotImplementedException();
        }

        public IXLRangeColumns Columns(string columns)
        {
            throw new NotImplementedException();
        }

        public IXLRangeColumns ColumnsUsed(bool includeFormats, Func<IXLRangeColumn, bool> predicate = null)
        {
            throw new NotImplementedException();
        }

        public IXLRangeColumns ColumnsUsed(Func<IXLRangeColumn, bool> predicate = null)
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

        public IXLRange CopyTo(IXLCell target)
        {
            throw new NotImplementedException();
        }

        public IXLRange CopyTo(IXLRangeBase target)
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

        public IXLTable CreateTable()
        {
            throw new NotImplementedException();
        }

        public IXLTable CreateTable(string name)
        {
            throw new NotImplementedException();
        }

        public void Delete(XLShiftDeletedCells shiftDeleteCells)
        {
            throw new NotImplementedException();
        }

        public void DeleteComments()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            //Apenas para passar nos testes
        }

        public IXLRangeColumn FindColumn(Func<IXLRangeColumn, bool> predicate)
        {
            throw new NotImplementedException();
        }

        public IXLRangeRow FindRow(Func<IXLRangeRow, bool> predicate)
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

        public IXLRangeColumn FirstColumn(Func<IXLRangeColumn, bool> predicate = null)
        {
            throw new NotImplementedException();
        }

        public IXLRangeColumn FirstColumnUsed(bool includeFormats, Func<IXLRangeColumn, bool> predicate = null)
        {
            throw new NotImplementedException();
        }

        public IXLRangeColumn FirstColumnUsed(Func<IXLRangeColumn, bool> predicate = null)
        {
            throw new NotImplementedException();
        }

        public IXLRangeRow FirstRow(Func<IXLRangeRow, bool> predicate = null)
        {
            throw new NotImplementedException();
        }

        public IXLRangeRow FirstRowUsed(bool includeFormats, Func<IXLRangeRow, bool> predicate = null)
        {
            throw new NotImplementedException();
        }

        public IXLRangeRow FirstRowUsed(Func<IXLRangeRow, bool> predicate = null)
        {
            throw new NotImplementedException();
        }

        public IXLRangeColumns InsertColumnsAfter(int numberOfColumns)
        {
            throw new NotImplementedException();
        }

        public IXLRangeColumns InsertColumnsAfter(int numberOfColumns, bool expandRange)
        {
            throw new NotImplementedException();
        }

        public IXLRangeColumns InsertColumnsBefore(int numberOfColumns)
        {
            throw new NotImplementedException();
        }

        public IXLRangeColumns InsertColumnsBefore(int numberOfColumns, bool expandRange)
        {
            throw new NotImplementedException();
        }

        public IXLRangeRows InsertRowsAbove(int numberOfRows)
        {
            throw new NotImplementedException();
        }

        public IXLRangeRows InsertRowsAbove(int numberOfRows, bool expandRange)
        {
            throw new NotImplementedException();
        }

        public IXLRangeRows InsertRowsBelow(int numberOfRows)
        {
            throw new NotImplementedException();
        }

        public IXLRangeRows InsertRowsBelow(int numberOfRows, bool expandRange)
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

        public IXLRangeColumn LastColumn(Func<IXLRangeColumn, bool> predicate = null)
        {
            throw new NotImplementedException();
        }

        public IXLRangeColumn LastColumnUsed(bool includeFormats, Func<IXLRangeColumn, bool> predicate = null)
        {
            throw new NotImplementedException();
        }

        public IXLRangeColumn LastColumnUsed(Func<IXLRangeColumn, bool> predicate = null)
        {
            throw new NotImplementedException();
        }

        public IXLRangeRow LastRow(Func<IXLRangeRow, bool> predicate = null)
        {
            throw new NotImplementedException();
        }

        public IXLRangeRow LastRowUsed(bool includeFormats, Func<IXLRangeRow, bool> predicate = null)
        {
            throw new NotImplementedException();
        }

        public IXLRangeRow LastRowUsed(Func<IXLRangeRow, bool> predicate = null)
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

        public IXLRange Range(IXLRangeAddress rangeAddress)
        {
            throw new NotImplementedException();
        }

        public IXLRange Range(string rangeAddress)
        {
            throw new NotImplementedException();
        }

        public IXLRange Range(IXLCell firstCell, IXLCell lastCell)
        {
            throw new NotImplementedException();
        }

        public IXLRange Range(string firstCellAddress, string lastCellAddress)
        {
            throw new NotImplementedException();
        }

        public IXLRange Range(IXLAddress firstCellAddress, IXLAddress lastCellAddress)
        {
            throw new NotImplementedException();
        }

        public IXLRange Range(int firstCellRow, int firstCellColumn, int lastCellRow, int lastCellColumn)
        {
            throw new NotImplementedException();
        }

        public IXLRanges Ranges(string ranges)
        {
            throw new NotImplementedException();
        }

        public IXLRange RangeUsed()
        {
            throw new NotImplementedException();
        }

        public IXLRangeRow Row(int row)
        {
            throw new NotImplementedException();
        }

        public int RowCount()
        {
            throw new NotImplementedException();
        }

        public IXLRangeRows Rows(Func<IXLRangeRow, bool> predicate = null)
        {
            throw new NotImplementedException();
        }

        public IXLRangeRows Rows(int firstRow, int lastRow)
        {
            throw new NotImplementedException();
        }

        public IXLRangeRows Rows(string rows)
        {
            throw new NotImplementedException();
        }

        public IXLRangeRows RowsUsed(bool includeFormats, Func<IXLRangeRow, bool> predicate = null)
        {
            throw new NotImplementedException();
        }

        public IXLRangeRows RowsUsed(Func<IXLRangeRow, bool> predicate = null)
        {
            throw new NotImplementedException();
        }

        public void Select()
        {
            throw new NotImplementedException();
        }

        public IXLAutoFilter SetAutoFilter()
        {
            //Apenas para passar nos testes
            return new XLAutoFilterFake();
        }

        public IXLRange SetDataType(XLCellValues dataType)
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

        public IXLRange Sort()
        {
            throw new NotImplementedException();
        }

        public IXLRange Sort(string columnsToSortBy, XLSortOrder sortOrder = XLSortOrder.Ascending, bool matchCase = false, bool ignoreBlanks = true)
        {
            throw new NotImplementedException();
        }

        public IXLRange Sort(int columnToSortBy, XLSortOrder sortOrder = XLSortOrder.Ascending, bool matchCase = false, bool ignoreBlanks = true)
        {
            throw new NotImplementedException();
        }

        public IXLRange SortLeftToRight(XLSortOrder sortOrder = XLSortOrder.Ascending, bool matchCase = false, bool ignoreBlanks = true)
        {
            throw new NotImplementedException();
        }

        public void Transpose(XLTransposeOptions transposeOption)
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
