using System;
using System.IO;
using System.Threading.Tasks;

namespace Thea.ExcelExporter;

struct Border
{
    public BorderStyle Style { get; set; } = BorderStyle.Thin;
    public string RgbColor { get; set; } = "FF000000";
    public int Line { get; set; } = 0;

    public Border() { }
    public async Task Render(StreamWriter writer)
    {
        await writer.WriteAsync("<x:border diagonalUp=\"0\" diagonalDown=\"0\">");
        await writer.WriteAsync($"	<x:left style=\"{this.GetStyleByLine(1)}\"><x:color indexed=\"64\"/></x:left>");
        await writer.WriteAsync($"	<x:right style=\"{this.GetStyleByLine(4)}\"><x:color rgb=\"{this.RgbColor}\"/></x:right>");
        await writer.WriteAsync($"	<x:top style=\"{this.GetStyleByLine(2)}\"><x:color rgb=\"{this.RgbColor}\"/></x:top>");
        await writer.WriteAsync($"	<x:bottom style=\"{this.GetStyleByLine(8)}\"><x:color rgb=\"{this.RgbColor}\"/></x:bottom>");
        await writer.WriteAsync($"	<x:diagonal style=\"none\"><x:color rgb=\"{this.RgbColor}\"/></x:diagonal>");
        await writer.WriteAsync("</x:border>");
    }
    public override int GetHashCode() => HashCode.Combine(this.Style, this.RgbColor, this.Line);
    private string GetStyleByLine(int direction)
    {
        if ((this.Line & direction) == direction)
            return Enum.GetName(this.Style).ToCamelCase();
        return "none";
    }
}
