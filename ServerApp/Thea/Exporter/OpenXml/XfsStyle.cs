using System;
using System.IO;
using System.Threading.Tasks;

namespace Thea.ExcelExporter;

struct XfsStyle
{
    public int XfId { get; set; } = 0;
    public int NumFmtId { get; set; } = 0;
    public int FontId { get; set; } = 0;
    public int FillId { get; set; } = 0;
    public int BorderId { get; set; } = 0;
    public bool ApplyNumberFormat { get; set; } = true;
    public bool ApplyBorder { get; set; } = true;
    public bool ApplyFill { get; set; } = true;
    public bool ApplyAlignment { get; set; } = true;
    public bool ApplyProtection { get; set; } = true;

    public XfsStyle() { }
    public async Task Render(StreamWriter writer)
    {
        await writer.WriteAsync($"<x:xf numFmtId=\"{this.NumFmtId}\" fontId=\"{this.FontId}\" fillId=\"{this.FillId}\" borderId=\"{this.BorderId}\" applyNumberFormat=\"{(this.ApplyNumberFormat ? "1" : "0")}\" applyFill=\"{(this.ApplyFill ? "1" : "0")}\" applyBorder=\"{(this.ApplyBorder ? "1" : "0")}\" applyAlignment=\"{(this.ApplyAlignment ? "1" : "0")}\" applyProtection=\"{(this.ApplyProtection ? "1" : "0")}\">");
        await writer.WriteAsync($"	<x:protection locked=\"1\" hidden=\"0\" />");
        await writer.WriteAsync($"</x:xf>");
    }
    public override int GetHashCode()
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
}
