using System;
using System.IO;
using System.Threading.Tasks;

namespace Thea.ExcelExporter;

struct Style
{
    public int XfId { get; set; } = 0;
    public int NumFmtId { get; set; } = 0;
    public int FontId { get; set; } = 0;
    public int FillId { get; set; } = 0;
    public int BorderId { get; set; } = 0;
    public CellHorizontalAlignment HorizontalAlignment { get; set; } = CellHorizontalAlignment.General;
    public CellVerticalAlignment VerticalAlignment { get; set; } = CellVerticalAlignment.Bottom;
    public bool ApplyNumberFormat { get; set; } = true;
    public bool ApplyBorder { get; set; } = true;
    public bool ApplyFill { get; set; } = true;
    public bool ApplyAlignment { get; set; } = true;
    public bool ApplyProtection { get; set; } = true;

    public Style() { }
    public async Task RenderDefine(StreamWriter writer)
    {
        await writer.WriteAsync($"<x:xf numFmtId=\"{this.NumFmtId}\" fontId=\"{this.FontId}\" fillId=\"{this.FillId}\" borderId=\"{this.BorderId}\" applyNumberFormat=\"{(this.ApplyNumberFormat ? "1" : "0")}\" applyFill=\"{(this.ApplyFill ? "1" : "0")}\" applyBorder=\"{(this.ApplyBorder ? "1" : "0")}\" applyAlignment=\"{(this.ApplyAlignment ? "1" : "0")}\" applyProtection=\"{(this.ApplyProtection ? "1" : "0")}\">");
        await writer.WriteAsync($"	<x:protection locked=\"1\" hidden=\"0\" />");
        await writer.WriteAsync($"</x:xf>");
    }
    public async Task RenderBody(StreamWriter writer)
    {
        await writer.WriteAsync($"<x:xf xfId=\"0\" numFmtId=\"{this.NumFmtId}\" fontId=\"{this.FontId}\" fillId=\"{this.FillId}\" borderId=\"{this.BorderId}\" applyNumberFormat=\"{(this.ApplyNumberFormat ? "1" : "0")}\" applyFill=\"{(this.ApplyFill ? "1" : "0")}\" applyBorder=\"{(this.ApplyBorder ? "1" : "0")}\" applyAlignment=\"{(this.ApplyAlignment ? "1" : "0")}\" applyProtection=\"{(this.ApplyProtection ? "1" : "0")}\">");
        await writer.WriteAsync($"	<x:alignment horizontal=\"{Enum.GetName(this.HorizontalAlignment).ToCamelCase()}\" vertical=\"{Enum.GetName(this.VerticalAlignment).ToCamelCase()}\" textRotation=\"0\" wrapText=\"0\" indent=\"0\" relativeIndent=\"0\" justifyLastLine=\"0\" shrinkToFit=\"0\" readingOrder=\"0\"/>");
        await writer.WriteAsync($"	<x:protection locked=\"1\" hidden=\"0\" />");
        await writer.WriteAsync($"</x:xf>");
    }
    public int GetXfsKey()
    {
        var hashCode = new HashCode();
        hashCode.Add(this.XfId);
        hashCode.Add(this.NumFmtId);
        hashCode.Add(this.FontId);
        hashCode.Add(this.FillId);
        hashCode.Add(this.BorderId);
        hashCode.Add(this.ApplyNumberFormat);
        hashCode.Add(this.ApplyBorder);
        hashCode.Add(this.ApplyFill);
        hashCode.Add(this.ApplyAlignment);
        hashCode.Add(this.ApplyProtection);
        return hashCode.ToHashCode();
    }
    public override int GetHashCode()
    {
        var hashCode = new HashCode();
        hashCode.Add(this.XfId);
        hashCode.Add(this.NumFmtId);
        hashCode.Add(this.FontId);
        hashCode.Add(this.FillId);
        hashCode.Add(this.BorderId);
        hashCode.Add(this.HorizontalAlignment);
        hashCode.Add(this.VerticalAlignment);
        hashCode.Add(this.ApplyNumberFormat);
        hashCode.Add(this.ApplyBorder);
        hashCode.Add(this.ApplyFill);
        hashCode.Add(this.ApplyAlignment);
        hashCode.Add(this.ApplyProtection);
        return hashCode.ToHashCode();
    }
}
