namespace Thea.ExcelExporter;

public enum CellDataType { Text, Number, Boolean, DateTime, TimeSpan }
public enum FontVerticalAlignment { Baseline, Subscript, Superscript }
public enum FontFamily { NotApplicable, Roman, Swiss, Modern, Script, Decorative }
public enum BorderStyle
{
    DashDot,
    DashDotDot,
    Dashed,
    Dotted,
    Double,
    Hair,
    Medium,
    MediumDashDot,
    MediumDashDotDot,
    MediumDashed,
    None,
    SlantDashDot,
    Thick,
    Thin
}
public enum CellHorizontalAlignment { Center, CenterContinuous, Distributed, Fill, General, Justify, Left, Right }
public enum CellVerticalAlignment { Bottom, Center, Distributed, Justify, Top }
