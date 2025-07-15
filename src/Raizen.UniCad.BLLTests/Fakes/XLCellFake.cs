using ClosedXML.Excel;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Raizen.UniCad.BLLTests.Fakes
{
    public class XLCellFake : IXLCell
    {
        public object Value
        {
            //Apenas para passar nos testes
            get
            {
                return string.Empty;
            }
            set
            {
                object test = value;
            }
        }

        public IXLAddress Address
        {
            get
            { throw new NotImplementedException(); }
        }

        public XLCellValues DataType { get { throw new NotImplementedException(); } set { object mock = value; } }

        public bool HasHyperlink
        {
            get
            {
                throw new NotImplementedException();

            }
        }

        public string FormulaA1 { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }
        public string FormulaR1C1 { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }
        public IXLStyle Style { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }
        public bool ShareString { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }
        public XLHyperlink Hyperlink { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }

        public IXLWorksheet Worksheet
        {
            get
            {
                throw new NotImplementedException();

            }
        }

        public IXLDataValidation DataValidation
        {
            get
            {
                throw new NotImplementedException();

            }
        }

        public IXLDataValidation NewDataValidation
        {
            get
            {
                throw new NotImplementedException();

            }
        }

        public string ValueCached
        {
            get
            {
                throw new NotImplementedException();

            }
        }

        public IXLRichText RichText
        {
            get
            {
                throw new NotImplementedException();

            }
        }

        public bool HasRichText
        {
            get
            {
                throw new NotImplementedException();

            }
        }

        public IXLComment Comment
        {
            get
            {
                throw new NotImplementedException();

            }
        }

        public bool HasComment
        {
            get
            {
                throw new NotImplementedException();

            }
        }

        public bool HasDataValidation {
            get
            {
                throw new NotImplementedException();

            }
        }

        public bool Active { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }

        public bool HasFormula{
            get
            {
                throw new NotImplementedException();

            }
        }

        public IXLRangeAddress FormulaReference { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }

        public IXLConditionalFormat AddConditionalFormat()
        {
            throw new NotImplementedException();
        }

        public IXLCell AddToNamed(string rangeName)
        {
            throw new NotImplementedException();
        }

        public IXLCell AddToNamed(string rangeName, XLScope scope)
        {
            throw new NotImplementedException();
        }

        public IXLCell AddToNamed(string rangeName, XLScope scope, string comment)
        {
            throw new NotImplementedException();
        }

        public IXLRange AsRange()
        {
            throw new NotImplementedException();
        }

        public IXLCell CellAbove()
        {
            throw new NotImplementedException();
        }

        public IXLCell CellAbove(int step)
        {
            throw new NotImplementedException();
        }

        public IXLCell CellBelow()
        {
            throw new NotImplementedException();
        }

        public IXLCell CellBelow(int step)
        {
            throw new NotImplementedException();
        }

        public IXLCell CellLeft()
        {
            throw new NotImplementedException();
        }

        public IXLCell CellLeft(int step)
        {
            throw new NotImplementedException();
        }

        public IXLCell CellRight()
        {
            throw new NotImplementedException();
        }

        public IXLCell CellRight(int step)
        {
            throw new NotImplementedException();
        }

        public IXLCell Clear(XLClearOptions clearOptions = XLClearOptions.ContentsAndFormats)
        {
            throw new NotImplementedException();
        }

        public IXLCell CopyFrom(IXLCell otherCell)
        {
            throw new NotImplementedException();
        }

        public IXLCell CopyFrom(string otherCell)
        {
            throw new NotImplementedException();
        }

        public IXLCell CopyTo(IXLCell target)
        {
            throw new NotImplementedException();
        }

        public IXLCell CopyTo(string target)
        {
            throw new NotImplementedException();
        }

        public void Delete(XLShiftDeletedCells shiftDeleteCells)
        {
            throw new NotImplementedException();
        }

        public bool GetBoolean()
        {
            throw new NotImplementedException();
        }

        public DateTime GetDateTime()
        {
            throw new NotImplementedException();
        }

        public double GetDouble()
        {
            throw new NotImplementedException();
        }

        public string GetFormattedString()
        {
            throw new NotImplementedException();
        }

        public XLHyperlink GetHyperlink()
        {
            throw new NotImplementedException();
        }

        public string GetString()
        {
            throw new NotImplementedException();
        }

        public TimeSpan GetTimeSpan()
        {
            throw new NotImplementedException();
        }

        public T GetValue<T>()
        {
            throw new NotImplementedException();
        }

        public IXLCells InsertCellsAbove(int numberOfRows)
        {
            throw new NotImplementedException();
        }

        public IXLCells InsertCellsAfter(int numberOfColumns)
        {
            throw new NotImplementedException();
        }

        public IXLCells InsertCellsBefore(int numberOfColumns)
        {
            throw new NotImplementedException();
        }

        public IXLCells InsertCellsBelow(int numberOfRows)
        {
            throw new NotImplementedException();
        }

        public IXLRange InsertData(IEnumerable data)
        {
            throw new NotImplementedException();
        }

        public IXLTable InsertTable<T>(IEnumerable<T> data)
        {
            throw new NotImplementedException();
        }

        public IXLTable InsertTable<T>(IEnumerable<T> data, bool createTable)
        {
            throw new NotImplementedException();
        }

        public IXLTable InsertTable<T>(IEnumerable<T> data, string tableName)
        {
            throw new NotImplementedException();
        }

        public IXLTable InsertTable<T>(IEnumerable<T> data, string tableName, bool createTable)
        {
            throw new NotImplementedException();
        }

        public IXLTable InsertTable(System.Data.DataTable data)
        {
            throw new NotImplementedException();
        }

        public IXLTable InsertTable(System.Data.DataTable data, bool createTable)
        {
            throw new NotImplementedException();
        }

        public IXLTable InsertTable(System.Data.DataTable data, string tableName)
        {
            throw new NotImplementedException();
        }

        public IXLTable InsertTable(System.Data.DataTable data, string tableName, bool createTable)
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

        public void Select()
        {
            throw new NotImplementedException();
        }

        public IXLCell SetActive(bool value = true)
        {
            throw new NotImplementedException();
        }

        public IXLCell SetDataType(XLCellValues dataType)
        {
            //Apenas para passar nos testes
            return this;
        }

        public IXLDataValidation SetDataValidation()
        {
            throw new NotImplementedException();
        }

        public IXLCell SetFormulaA1(string formula)
        {
            throw new NotImplementedException();
        }

        public IXLCell SetFormulaR1C1(string formula)
        {
            throw new NotImplementedException();
        }

        public IXLCell SetValue<T>(T value)
        {
            //Apenas para passar nos testes
            return this;
        }

        public bool TryGetValue<T>(out T value)
        {
            throw new NotImplementedException();
        }

        public IXLColumn WorksheetColumn()
        {
            throw new NotImplementedException();
        }

        public IXLRow WorksheetRow()
        {
            throw new NotImplementedException();
        }
    }
}
