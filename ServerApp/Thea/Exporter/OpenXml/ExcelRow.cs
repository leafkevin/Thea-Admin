using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Thea.ExcelExporter;

public class ExcelRow
{
    private readonly Workbook parent;

    public int RowIndex { get; set; }
    public List<ExcelCell> Cells { get; set; }
    public object RowData { get; set; }
    public List<ExcelColumnHeader> Headers { get; set; }
    public ExcelCell this[string field]
    {
        get { return this.Cells.Find(f => f.Header != null && f.Header.Field == field); }
    }
    public ExcelRow(Workbook parent) => this.parent = parent;

    public bool TryGetCell(string field, out ExcelCell cell)
    {
        cell = this.Cells.Find(f => f.Header != null && f.Header.Field == field);
        return cell != null;
    }
    public ExcelRange Range(int startCellIndex, int endCellIndex)
    {
        if (this.Cells.Count <= 0)
            throw new Exception("当前行没有任何单元格");
        if (startCellIndex < 0 && startCellIndex >= this.Cells.Count)
            throw new Exception("startCellIndex值无效，必须>=0并且小于当前行单元格个数");
        if (endCellIndex < 0 && endCellIndex >= this.Cells.Count)
            throw new Exception("endCellIndex值无效，必须>=0并且小于当前行单元格个数");

        return this.Range(this.Cells[startCellIndex], this.Cells[endCellIndex]);
    }
    public ExcelRange Range(ExcelCell start, ExcelCell end)
    {
        if (start == null)
            throw new ArgumentNullException("start");
        if (end == null)
            throw new ArgumentNullException("end");

        if (start.RowIndex != this.RowIndex)
            throw new Exception("start单元格不是本行的单元格");
        if (end.RowIndex != this.RowIndex)
            throw new Exception("start单元格不是本行的单元格");
        if (start == end) return start.AsRange();
        return new ExcelRange
        {
            Start = start,
            End = end
        };
    }
    public ExcelRange AsRange()
    {
        if (this.Cells == null || this.Cells.Count == 0)
            return ExcelRange.Empty;

        return this.Range(0, this.Cells.Count - 1);
    }
    public async Task WriteHeader(StreamWriter writer)
    {
        await writer.WriteAsync($"<x:row r=\"1\" spans=\"1:{this.Headers.Count}\">");
        foreach (var header in this.Headers)
        {
            var borderId = this.parent.GetOrAddBorder(new Border { Line = 15 });
            var style = this.parent.GetOrAddStyle(new Style
            {
                BorderId = borderId,
                HorizontalAlignment = CellHorizontalAlignment.Center,
                VerticalAlignment = CellVerticalAlignment.Center
            });
            await writer.WriteAsync($"<x:c r=\"{header.Letter}1\" s=\"{style}\" t=\"s\">");
            var refIndex = this.parent.GetOrAddSharedString(header.Title);
            await writer.WriteAsync($"<x:v>{refIndex}</x:v></x:c>");
        }
        await writer.WriteAsync("</x:row>");
    }
    public async Task RenderBody(StreamWriter writer)
    {
        writer.Write($"<x:row r=\"{this.RowIndex + 1}\" spans=\"1:{this.Headers.Count}\">");
        foreach (var cellInfo in this.Cells)
        {
             await cellInfo.Render(writer);
        }
        writer.Write("</x:row>");
    }
}
