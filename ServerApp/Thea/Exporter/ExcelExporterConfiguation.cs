using System.Collections.Generic;

namespace Thea.ExcelExporter;

public class ExcelExporterConfiguation
{
    public string SheetName { get; set; } = "Sheet1";
    public bool IsAllowMerge { get; set; }
    public double DefaultRowHeight { get; set; } = 15d;
    public double DefaultColumnWidth { get; set; } = 11.5d;
    public CellHorizontalAlignment DefaultHorizontalAlignment { get; set; } = CellHorizontalAlignment.General;
    public CellVerticalAlignment DefaultVerticalAlignment { get; set; } = CellVerticalAlignment.Center;
    public List<ExcelColumnHeader> Headers { get; set; }
    public CellMergePredicate CellMergePredicate { get; set; }
}