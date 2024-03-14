using System;
using System.IO;
using System.Threading.Tasks;

namespace Thea.ExcelExporter;

public class ExcelCell
{
    private static readonly DateTime BaseDate = new DateTime(1899, 12, 30);
    private readonly Workbook parent;

    public int RowIndex { get; set; }
    public int ColumnIndex { get; set; }
    public string CellLetter { get; set; }
    public ExcelColumnHeader Header { get; set; }
    public object Value { get; set; }
    public bool IsMerged { get; set; }
    public ExcelRange MergedRange { get; set; }
    public CellHorizontalAlignment HorizontalAlignment { get; set; }
    public CellVerticalAlignment VerticalAlignment { get; set; }
    public int BorderLine { get; set; } = 15;

    public ExcelCell(Workbook parent) => this.parent = parent;

    public ExcelRange AsRange() => new ExcelRange
    {
        Start = this,
        End = this,
        HorizontalAlignment = this.HorizontalAlignment,
        VerticalAlignment = this.VerticalAlignment
    };
    public async Task Render(StreamWriter writer)
    {
        await writer.WriteAsync($"<x:c r=\"{this.Header.Letter}{this.RowIndex + 1}\"");
        if (this.Value == null)
        {
            await writer.WriteAsync($" s=\"1\"/>");
            return;
        }

        var borderId = this.parent.GetOrAddBorder(new Border { Line = this.BorderLine });
        object cellValue = this.Value;
        if (this.Header.ValueDecorator != null)
            cellValue = this.Header.ValueDecorator(cellValue);
        this.TryGetKnownTypedValue(cellValue, out var outputValue, out var dataType, out var numFmtId);
        var styleId = this.parent.GetOrAddStyle(new Style
        {
            BorderId = borderId,
            NumFmtId = numFmtId,
            ApplyBorder = this.BorderLine == 0,
            //合并单元格后，按照设置的对齐进行排版
            HorizontalAlignment = this.HorizontalAlignment,
            VerticalAlignment = this.VerticalAlignment
        });

        await writer.WriteAsync($" s=\"{styleId}\"");
        switch (dataType)
        {
            case CellDataType.Text: await writer.WriteAsync(" t=\"s\""); break;
            case CellDataType.Number: await writer.WriteAsync(" t=\"n\""); break;
            case CellDataType.Boolean: await writer.WriteAsync(" t=\"b\""); break;
            case CellDataType.DateTime:
            case CellDataType.TimeSpan: break;
        }
        await writer.WriteAsync(">");
        await writer.WriteAsync("<x:v>");
        await writer.WriteAsync(outputValue);
        await writer.WriteAsync("</x:v>");
        await writer.WriteAsync("</x:c>");
    }
    private bool TryGetKnownTypedValue(object value, out string outputValue, out CellDataType dataType, out int numFmtId)
    {
        numFmtId = 0;
        outputValue = String.Empty;
        if (value.Equals(DBNull.Value) || value == null)
        {
            dataType = CellDataType.Text;
            return true;
        }
        if (value == null)
        {
            dataType = CellDataType.Text;
            return true;
        }
        if (value is bool b)
        {
            dataType = CellDataType.Boolean;
            outputValue = b ? "1" : "0";
            return true;
        }
        if (value is string || value is char || value is Guid)
        {
            dataType = CellDataType.Text;
            var strValue = XmlEncoder.Encode(value.ToString());
            var refIndex = this.parent.GetOrAddSharedString(strValue);
            outputValue = refIndex.ToInvariantString();
            //TODO:文本换行
            //if (parsedValue.Contains(Environment.NewLine) && !style.Alignment.WrapText)
            //    Style.Alignment.WrapText = true;               
            return true;
        }
        if (value is DateTime d && d >= BaseDate)
        {
            dataType = CellDataType.DateTime;
            outputValue = d.ToOADate().ToInvariantString();
            if (string.IsNullOrEmpty(this.Header.Format))
            {
                //d/m/yyyy
                if (d.Date == d) numFmtId = 14;
                //m/d/yyyy H:mm
                else numFmtId = 22;
            }
            else numFmtId = this.parent.GetOrAddNumberFormat(this.Header.Format);
            return true;
        }
        if (value is TimeSpan ts)
        {
            dataType = CellDataType.TimeSpan;
            outputValue = ts.TotalDays.ToInvariantString();
            if (string.IsNullOrEmpty(this.Header.Format))
            {
                //[h]:mm:ss
                numFmtId = 46;
            }
            else numFmtId = this.parent.GetOrAddNumberFormat(this.Header.Format);
            return true;
        }
        if (value.IsNumber())
        {
            numFmtId = 0;
            if ((value is double d1 && (double.IsNaN(d1) || double.IsInfinity(d1)))
                || (value is float f && (float.IsNaN(f) || float.IsInfinity(f))))
            {
                dataType = CellDataType.Text;
                outputValue = value.ToString();
            }
            else
            {
                dataType = CellDataType.Number;
                outputValue = value.ToInvariantString();
                if (!string.IsNullOrEmpty(this.Header.Format))
                    numFmtId = this.parent.GetOrAddNumberFormat(this.Header.Format);
            }
            return true;
        }
        dataType = CellDataType.Text;
        outputValue = null;
        return false;
    }
    public override string ToString() => this.CellLetter;
}
