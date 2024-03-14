using System;

namespace Thea.ExcelExporter;

public class ExcelColumnHeader
{
    public string Field { get; set; }
    public string Title { get; set; }
    public string Letter { get; set; }
    public string Format { get; set; }
    public double? ColumnWidth { get; set; }
    public CellHorizontalAlignment? HorizontalAlignment { get; set; }
    public CellVerticalAlignment? VerticalAlignment { get; set; }
    public Func<object, object> ValueDecorator { get; set; }
    internal Func<object, object> DataFetcher { get; set; }
    public override string ToString() => this.Letter;
}
