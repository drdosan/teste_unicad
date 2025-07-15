using ClosedXML.Excel;
using System;

namespace Raizen.UniCad.BLLTests.Fakes
{
    public class XLWorksheetFake : IXLWorksheet
    {
        public XLWorkbook Workbook { get { throw new NotImplementedException(); } }

        public double ColumnWidth { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }
        public double RowHeight { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }
        public string Name { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }
        public int Position { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }

        public IXLPageSetup PageSetup { get { throw new NotImplementedException(); } }

        public IXLOutline Outline { get { throw new NotImplementedException(); } }

        public IXLNamedRanges NamedRanges { get { throw new NotImplementedException(); } }

        public IXLSheetView SheetView { get { throw new NotImplementedException(); } }

        public IXLTables Tables { get { throw new NotImplementedException(); } }

        public IXLDataValidations DataValidations { get { throw new NotImplementedException(); } }

        public XLWorksheetVisibility Visibility { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }

        public IXLSheetProtection Protection { get { throw new NotImplementedException(); } }

        public IXLSortElements SortRows { get { throw new NotImplementedException(); } }

        public IXLSortElements SortColumns { get { throw new NotImplementedException(); } }

        public bool ShowFormulas { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }
        public bool ShowGridLines { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }
        public bool ShowOutlineSymbols { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }
        public bool ShowRowColHeaders { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }
        public bool ShowRuler { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }
        public bool ShowWhiteSpace { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }
        public bool ShowZeros { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }
        public XLColor TabColor { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }
        public bool TabSelected { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }
        public bool TabActive { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }

        public IXLPivotTables PivotTables { get { throw new NotImplementedException(); } }

        public bool RightToLeft { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }

        public IXLBaseAutoFilter AutoFilter { get { throw new NotImplementedException(); } }

        public IXLRanges MergedRanges { get { throw new NotImplementedException(); } }

        public IXLConditionalFormats ConditionalFormats { get { throw new NotImplementedException(); } }

        public IXLRanges SelectedRanges { get { throw new NotImplementedException(); } }

        public IXLCell ActiveCell { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }
        public string Author { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }

        public IXLWorksheet Worksheet { get { throw new NotImplementedException(); } }

        public IXLRangeAddress RangeAddress { get { throw new NotImplementedException(); } }

        public object Value { set { throw new NotImplementedException(); } }
        public XLCellValues DataType { set { throw new NotImplementedException(); } }
        public string FormulaA1 { set { throw new NotImplementedException(); } }
        public string FormulaR1C1 { set { throw new NotImplementedException(); } }
        public IXLStyle Style { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }
        public bool ShareString { set { throw new NotImplementedException(); } }

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

        public IXLCell Cell(int row, int column)
        {
            //apenas para passar nos testes
            return new XLCellFake() { Value = string.Empty };
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

        public IXLRangeBase Clear(XLClearOptions clearOptions = XLClearOptions.ContentsAndFormats)
        {
            throw new NotImplementedException();
        }

        public IXLWorksheet CollapseColumns()
        {
            throw new NotImplementedException();
        }

        public IXLWorksheet CollapseColumns(int outlineLevel)
        {
            throw new NotImplementedException();
        }

        public IXLWorksheet CollapseRows()
        {
            throw new NotImplementedException();
        }

        public IXLWorksheet CollapseRows(int outlineLevel)
        {
            throw new NotImplementedException();
        }

        public IXLColumn Column(int column)
        {
            throw new NotImplementedException();
        }

        public IXLColumn Column(string column)
        {
            throw new NotImplementedException();
        }

        public int ColumnCount()
        {
            throw new NotImplementedException();
        }

        public IXLColumns Columns()
        {
            throw new NotImplementedException();
        }

        public IXLColumns Columns(string columns)
        {
            throw new NotImplementedException();
        }

        public IXLColumns Columns(string firstColumn, string lastColumn)
        {
            throw new NotImplementedException();
        }

        public IXLColumns Columns(int firstColumn, int lastColumn)
        {
            throw new NotImplementedException();
        }

        public IXLColumns ColumnsUsed(bool includeFormats = false, Func<IXLColumn, bool> predicate = null)
        {
            throw new NotImplementedException();
        }

        public IXLColumns ColumnsUsed(Func<IXLColumn, bool> predicate)
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

        public IXLWorksheet CopyTo(string newSheetName)
        {
            throw new NotImplementedException();
        }

        public IXLWorksheet CopyTo(string newSheetName, int position)
        {
            throw new NotImplementedException();
        }

        public IXLWorksheet CopyTo(XLWorkbook workbook, string newSheetName)
        {
            throw new NotImplementedException();
        }

        public IXLWorksheet CopyTo(XLWorkbook workbook, string newSheetName, int position)
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

        public object Evaluate(string expression)
        {
            throw new NotImplementedException();
        }

        public IXLWorksheet ExpandColumns()
        {
            throw new NotImplementedException();
        }

        public IXLWorksheet ExpandColumns(int outlineLevel)
        {
            throw new NotImplementedException();
        }

        public IXLWorksheet ExpandRows()
        {
            throw new NotImplementedException();
        }

        public IXLWorksheet ExpandRows(int outlineLevel)
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

        public IXLColumn FirstColumn()
        {
            throw new NotImplementedException();
        }

        public IXLColumn FirstColumnUsed()
        {
            throw new NotImplementedException();
        }

        public IXLColumn FirstColumnUsed(bool includeFormats)
        {
            throw new NotImplementedException();
        }

        public IXLRow FirstRow()
        {
            throw new NotImplementedException();
        }

        public IXLRow FirstRowUsed()
        {
            throw new NotImplementedException();
        }

        public IXLRow FirstRowUsed(bool includeFormats)
        {
            throw new NotImplementedException();
        }

        public IXLWorksheet Hide()
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

        public IXLColumn LastColumn()
        {
            throw new NotImplementedException();
        }

        public IXLColumn LastColumnUsed()
        {
            throw new NotImplementedException();
        }

        public IXLColumn LastColumnUsed(bool includeFormats)
        {
            throw new NotImplementedException();
        }

        public IXLRow LastRow()
        {
            throw new NotImplementedException();
        }

        public IXLRow LastRowUsed()
        {
            throw new NotImplementedException();
        }

        public IXLRow LastRowUsed(bool includeFormats)
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

        public IXLNamedRange NamedRange(string rangeName)
        {
            throw new NotImplementedException();
        }

        public IXLPivotTable PivotTable(string name)
        {
            throw new NotImplementedException();
        }

        public IXLSheetProtection Protect()
        {
            throw new NotImplementedException();
        }

        public IXLSheetProtection Protect(string password)
        {
            throw new NotImplementedException();
        }

        public IXLRange Range(IXLRangeAddress rangeAddress)
        {
            throw new NotImplementedException();
        }

        public IXLRange Range(string rangeAddress)
        {
            if (rangeAddress == "A1:J1")
                return new XLRangeFake();

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
            //Apenas para passar nos testes
            return new XLRangeFake();
        }

        public IXLRanges Ranges(string ranges)
        {
            throw new NotImplementedException();
        }

        public IXLRange RangeUsed()
        {
            throw new NotImplementedException();
        }

        public IXLRange RangeUsed(bool includeFormats)
        {
            throw new NotImplementedException();
        }

        public IXLRow Row(int row)
        {
            //Apenas para passar nos testes
            return new XLRowFake();
        }

        public int RowCount()
        {
            throw new NotImplementedException();
        }

        public IXLRows Rows()
        {
            throw new NotImplementedException();
        }

        public IXLRows Rows(string rows)
        {
            throw new NotImplementedException();
        }

        public IXLRows Rows(int firstRow, int lastRow)
        {
            throw new NotImplementedException();
        }

        public IXLRows RowsUsed(bool includeFormats = false, Func<IXLRow, bool> predicate = null)
        {
            throw new NotImplementedException();
        }

        public IXLRows RowsUsed(Func<IXLRow, bool> predicate)
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

        public IXLDataValidation SetDataValidation()
        {
            throw new NotImplementedException();
        }

        public IXLWorksheet SetRightToLeft()
        {
            throw new NotImplementedException();
        }

        public IXLWorksheet SetRightToLeft(bool value)
        {
            throw new NotImplementedException();
        }

        public IXLWorksheet SetShowFormulas()
        {
            throw new NotImplementedException();
        }

        public IXLWorksheet SetShowFormulas(bool value)
        {
            throw new NotImplementedException();
        }

        public IXLWorksheet SetShowGridLines()
        {
            throw new NotImplementedException();
        }

        public IXLWorksheet SetShowGridLines(bool value)
        {
            throw new NotImplementedException();
        }

        public IXLWorksheet SetShowOutlineSymbols()
        {
            throw new NotImplementedException();
        }

        public IXLWorksheet SetShowOutlineSymbols(bool value)
        {
            throw new NotImplementedException();
        }

        public IXLWorksheet SetShowRowColHeaders()
        {
            throw new NotImplementedException();
        }

        public IXLWorksheet SetShowRowColHeaders(bool value)
        {
            throw new NotImplementedException();
        }

        public IXLWorksheet SetShowRuler()
        {
            throw new NotImplementedException();
        }

        public IXLWorksheet SetShowRuler(bool value)
        {
            throw new NotImplementedException();
        }

        public IXLWorksheet SetShowWhiteSpace()
        {
            throw new NotImplementedException();
        }

        public IXLWorksheet SetShowWhiteSpace(bool value)
        {
            throw new NotImplementedException();
        }

        public IXLWorksheet SetShowZeros()
        {
            throw new NotImplementedException();
        }

        public IXLWorksheet SetShowZeros(bool value)
        {
            throw new NotImplementedException();
        }

        public IXLWorksheet SetTabActive()
        {
            throw new NotImplementedException();
        }

        public IXLWorksheet SetTabActive(bool value)
        {
            throw new NotImplementedException();
        }

        public IXLWorksheet SetTabColor(XLColor color)
        {
            throw new NotImplementedException();
        }

        public IXLWorksheet SetTabSelected()
        {
            throw new NotImplementedException();
        }

        public IXLWorksheet SetTabSelected(bool value)
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

        public IXLTable Table(int index)
        {
            throw new NotImplementedException();
        }

        public IXLTable Table(string name)
        {
            throw new NotImplementedException();
        }

        public IXLWorksheet Unhide()
        {
            throw new NotImplementedException();
        }

        public IXLRange Unmerge()
        {
            throw new NotImplementedException();
        }

        public IXLSheetProtection Unprotect()
        {
            throw new NotImplementedException();
        }

        public IXLSheetProtection Unprotect(string password)
        {
            throw new NotImplementedException();
        }
    }
}
