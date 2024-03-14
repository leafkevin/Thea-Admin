using System;
using System.IO;
using System.Threading.Tasks;

namespace Thea.ExcelExporter;

struct Font
{
    public FontVerticalAlignment VerticalAlign { get; set; } = FontVerticalAlignment.Baseline;
    public int Size { get; set; } = 11;
    public string FontName { get; set; } = "宋体";
    public FontFamily Family { get; set; } = FontFamily.Swiss;
    public string RgbColor { get; set; } = "FF000000";

    public Font() { }
    public async Task Render(StreamWriter writer)
    {
        await writer.WriteAsync("<x:font>");
        await writer.WriteAsync($"  <x:vertAlign val=\"{Enum.GetName(this.VerticalAlign).ToCamelCase()}\"/>");
        await writer.WriteAsync($"	<x:sz val=\"{this.Size}\"/>");
        await writer.WriteAsync($"	<x:color rgb=\"{this.RgbColor}\"/>");
        await writer.WriteAsync($"	<x:name val=\"{this.FontName}\"/>");
        await writer.WriteAsync($"	<x:family val=\"{(int)this.Family}\"/>");
        await writer.WriteAsync($"	<x:charset val=\"134\"/>");
        await writer.WriteAsync("</x:font>");
    }
    public override int GetHashCode() => HashCode.Combine(this.VerticalAlign, 
        this.Size, this.FontName, this.Family, this.RgbColor);
}
 