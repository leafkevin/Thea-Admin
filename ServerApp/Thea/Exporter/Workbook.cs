using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Thea.ExcelExporter;

public class Workbook
{
    private int index = 1;
    private readonly static UTF8Encoding encoding = new UTF8Encoding(true);
    private int lastNumFmtId = 165;
    private readonly List<WorkSheet> sheets = new();
    private readonly Dictionary<string, int> numFmtIds = new();
    private readonly Dictionary<Font, int> fontIds = new();
    private readonly Dictionary<Border, int> borderIds = new();
    private readonly Dictionary<int, (int Index, Style Style)> xfsStyles = new();
    private readonly Dictionary<Style, int> styleIds = new();

    private int RefCount { get; set; }
    private List<string> SharedStrings { get; set; } = new List<string>();
    private Dictionary<string, int> SharedStringIndices { get; set; } = new Dictionary<string, int>();
    public ReadOnlyCollection<WorkSheet> Sheets { get; private set; }

    public Workbook()
    {
        //宋体
        this.GetOrAddFont(new Font());

        //无边框
        this.GetOrAddBorder(new Border { Line = 0 });
        //有边框
        this.GetOrAddBorder(new Border { Line = 15 });

        //无边框，普通格式
        this.GetOrAddStyle(new Style { BorderId = 0, ApplyBorder = false });
        //有边框，普通格式
        this.GetOrAddStyle(new Style { BorderId = 1 });

        this.Sheets = new ReadOnlyCollection<WorkSheet>(this.sheets);
    }
    public WorkSheet AddSheet(string sheetName)
    {
        var id = this.NextId();
        var sheet = new WorkSheet
        {
            Id = id,
            RId = $"rId{id + 1}",
            SheetName = sheetName,
            Parent = this
        };
        this.sheets.Add(sheet);
        return sheet;
    }
    public void AddSheet(WorkSheet sheet)
    {
        var id = this.NextId();
        sheet.Id = id;
        sheet.RId = $"rId{id + 1}";
        this.sheets.Add(sheet);
        sheet.Parent = this;
    }
    public WorkSheet GetSheet(string sheetName) => this.sheets.Find(f => f.SheetName == sheetName);
    public async Task SaveAs(Stream stream)
    {
        if (this.sheets.Count == 0)
            throw new Exception("至少要有一个Sheet");

        using var archive = new ZipArchive(stream, ZipArchiveMode.Create, true, encoding);
        await this.RenderRels(archive);
        await this.RenderWorkbook(archive);
        await this.RenderTemplate(archive);
        await this.RenderWorkSheets(archive);
        await this.RenderStyle(archive);
        await this.RenderSharedStrings(archive);
        await this.RenderWorkbookRels(archive);
        await this.RenderContentTypes(archive);
        archive.Dispose();
    }
    public int GetOrAddSharedString(string strValue)
    {
        if (!this.SharedStringIndices.TryGetValue(strValue, out var refIndex))
        {
            refIndex = this.SharedStrings.Count;
            this.SharedStrings.Add(strValue);
            this.SharedStringIndices.Add(strValue, refIndex);
        }
        this.RefCount++;
        return refIndex;
    }
    internal int GetOrAddNumberFormat(string format)
    {
        if (string.IsNullOrEmpty(format)) return 0;
        if (!this.numFmtIds.TryGetValue(format, out var numFmtId))
        {
            numFmtId = this.lastNumFmtId;
            this.lastNumFmtId++;
            this.numFmtIds.Add(format, numFmtId);
        }
        return numFmtId;
    }
    internal int GetOrAddFont(Font font)
    {
        if (!this.fontIds.TryGetValue(font, out var fontId))
            this.fontIds.Add(font, fontId = this.fontIds.Count);
        return fontId;
    }
    internal int GetOrAddBorder(Border border)
    {
        if (!this.borderIds.TryGetValue(border, out var borderId))
            this.borderIds.Add(border, borderId = this.borderIds.Count);
        return borderId;
    }
    internal void TryAddXfsStyle(Style style)
    {
        var xfsKey = style.GetXfsKey();
        if (!this.xfsStyles.ContainsKey(xfsKey))
            this.xfsStyles.Add(xfsKey, (this.xfsStyles.Count, style));
    }
    internal int GetOrAddStyle(Style style)
    {
        this.TryAddXfsStyle(style);
        if (!this.styleIds.TryGetValue(style, out var styleId))
            this.styleIds.Add(style, styleId = this.styleIds.Count);
        return styleId;
    }
    private async Task RenderWorkbook(ZipArchive archive)
    {
        var path = "xl/workbook.xml";
        var entry = archive.CreateEntry(path);
        using var zipStream = entry.Open();
        using var writer = new StreamWriter(zipStream, encoding);

        await writer.WriteAsync("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
        await writer.WriteAsync("<x:workbook xmlns:x=\"http://schemas.openxmlformats.org/spreadsheetml/2006/main\" xmlns:r=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships\">");
        await writer.WriteAsync("<x:bookViews>");
        await writer.WriteAsync("<x:workbookView firstSheet=\"0\" activeTab=\"0\"/>");
        await writer.WriteAsync("</x:bookViews>");
        await writer.WriteAsync("<x:sheets>");

        this.sheets.ForEach(async f => await f.WriteReference(writer));

        await writer.WriteAsync("</x:sheets>");
        await writer.WriteAsync("</x:workbook>");

        await writer.FlushAsync();
        await zipStream.FlushAsync();
        writer.Close();
        zipStream.Close();
    }
    private async Task RenderWorkbookRels(ZipArchive archive)
    {
        var path = "xl/_rels/workbook.xml.rels";
        var entry = archive.CreateEntry(path);
        using var zipStream = entry.Open();
        using var writer = new StreamWriter(zipStream, encoding);

        await writer.WriteAsync("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
        await writer.WriteAsync("<Relationships xmlns=\"http://schemas.openxmlformats.org/package/2006/relationships\">");
        await writer.WriteAsync("	<Relationship Id=\"rId1\" Type=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships/theme\" Target=\"theme/theme1.xml\"/>");
        foreach (var sheet in this.sheets)
        {
            await writer.WriteAsync($"	<Relationship Id=\"{sheet.RId}\" Type=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships/worksheet\" Target=\"worksheets/sheet{sheet.Id}.xml\"/>");
        }
        await writer.WriteAsync($"	<Relationship Id=\"rId{this.sheets.Count + 2}\" Type=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships/styles\" Target=\"styles.xml\"/>");
        await writer.WriteAsync($"	<Relationship Id=\"rId{this.sheets.Count + 3}\" Type=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships/sharedStrings\" Target=\"sharedStrings.xml\"/>");
        await writer.WriteAsync("</Relationships>");

        await writer.FlushAsync();
        await zipStream.FlushAsync();
        writer.Close();
        zipStream.Close();
    }
    private async Task RenderRels(ZipArchive archive)
    {
        var path = "_rels/.rels";
        var entry = archive.CreateEntry(path);
        using var zipStream = entry.Open();
        using var writer = new StreamWriter(zipStream, encoding);

        await writer.WriteAsync("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
        await writer.WriteAsync("<Relationships xmlns=\"http://schemas.openxmlformats.org/package/2006/relationships\">");
        await writer.WriteAsync("	<Relationship Id=\"R5aab96fc61dd41e1\" Type=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships/officeDocument\" Target=\"xl/workbook.xml\"/>");
        await writer.WriteAsync("</Relationships>");

        await writer.FlushAsync();
        await zipStream.FlushAsync();
        writer.Close();
        zipStream.Close();
    }
    private async Task RenderWorkSheets(ZipArchive archive)
    {
        foreach (var sheet in this.Sheets)
        {
            await sheet.Render(archive, encoding);
        }
    }
    private async Task RenderSharedStrings(ZipArchive archive)
    {
        var path = "xl/sharedStrings.xml";
        var entry = archive.CreateEntry(path);
        using var zipStream = entry.Open();
        using var writer = new StreamWriter(zipStream, encoding);

        await writer.WriteAsync("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
        await writer.WriteAsync($"<x:sst xmlns:x=\"http://schemas.openxmlformats.org/spreadsheetml/2006/main\" count=\"{this.RefCount}\" uniqueCount=\"{this.SharedStrings.Count}\">");
        foreach (var sharedString in this.SharedStrings)
        {
            await writer.WriteAsync($"<x:si><x:t>{sharedString}</x:t></x:si>");
        }
        await writer.WriteAsync("</x:sst>");

        await writer.FlushAsync();
        await zipStream.FlushAsync();
        writer.Close();
        zipStream.Close();
    }
    private async Task RenderTemplate(ZipArchive archive)
    {
        var path = "xl/theme/theme1.xml";
        var entry = archive.CreateEntry(path);
        using var zipStream = entry.Open();
        using var writer = new StreamWriter(zipStream, encoding);

        await writer.WriteAsync("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
        await writer.WriteAsync("<a:theme xmlns:a=\"http://schemas.openxmlformats.org/drawingml/2006/main\" name=\"Office Theme​​\">");
        await writer.WriteAsync("<a:themeElements>");
        await writer.WriteAsync("	<a:clrScheme name=\"Office\">");
        await writer.WriteAsync("		<a:dk1><a:sysClr val=\"windowText\" lastClr=\"000000\"/></a:dk1>");
        await writer.WriteAsync("		<a:lt1><a:sysClr val=\"window\" lastClr=\"FFFFFF\"/></a:lt1>");
        await writer.WriteAsync("		<a:dk2><a:srgbClr val=\"44546A\"/></a:dk2>");
        await writer.WriteAsync("		<a:lt2><a:srgbClr val=\"E7E6E6\"/></a:lt2>");
        await writer.WriteAsync("		<a:accent1><a:srgbClr val=\"5B9BD5\"/></a:accent1>");
        await writer.WriteAsync("		<a:accent2><a:srgbClr val=\"ED7D31\"/></a:accent2>");
        await writer.WriteAsync("		<a:accent3><a:srgbClr val=\"A5A5A5\"/></a:accent3>");
        await writer.WriteAsync("		<a:accent4><a:srgbClr val=\"FFC000\"/></a:accent4>");
        await writer.WriteAsync("		<a:accent5><a:srgbClr val=\"4472C4\"/></a:accent5>");
        await writer.WriteAsync("		<a:accent6><a:srgbClr val=\"70AD47\"/></a:accent6>");
        await writer.WriteAsync("		<a:hlink><a:srgbClr val=\"0563C1\"/></a:hlink>");
        await writer.WriteAsync("		<a:folHlink><a:srgbClr val=\"954F72\"/></a:folHlink>");
        await writer.WriteAsync("	</a:clrScheme>");
        await writer.WriteAsync("	<a:fontScheme name=\"Office\">");
        await writer.WriteAsync("		<a:majorFont>");
        await writer.WriteAsync("			<a:latin typeface=\"Calibri Light\" panose=\"020F0302020204030204\"/>");
        await writer.WriteAsync("			<a:ea typeface=\"\"/>");
        await writer.WriteAsync("			<a:cs typeface=\"\"/>");
        await writer.WriteAsync("			<a:font script=\"Jpan\" typeface=\"游ゴシック Light\"/>");
        await writer.WriteAsync("			<a:font script=\"Hang\" typeface=\"맑은 고딕\"/>");
        await writer.WriteAsync("			<a:font script=\"Hans\" typeface=\"等线 Light\"/>");
        await writer.WriteAsync("			<a:font script=\"Hant\" typeface=\"新細明體\"/>");
        await writer.WriteAsync("			<a:font script=\"Arab\" typeface=\"Times New Roman\"/>");
        await writer.WriteAsync("			<a:font script=\"Hebr\" typeface=\"Times New Roman\"/>");
        await writer.WriteAsync("			<a:font script=\"Thai\" typeface=\"Tahoma\"/>");
        await writer.WriteAsync("			<a:font script=\"Ethi\" typeface=\"Nyala\"/>");
        await writer.WriteAsync("			<a:font script=\"Beng\" typeface=\"Vrinda\"/>");
        await writer.WriteAsync("			<a:font script=\"Gujr\" typeface=\"Shruti\"/>");
        await writer.WriteAsync("			<a:font script=\"Khmr\" typeface=\"MoolBoran\"/>");
        await writer.WriteAsync("			<a:font script=\"Knda\" typeface=\"Tunga\"/>");
        await writer.WriteAsync("			<a:font script=\"Guru\" typeface=\"Raavi\"/>");
        await writer.WriteAsync("			<a:font script=\"Cans\" typeface=\"Euphemia\"/>");
        await writer.WriteAsync("			<a:font script=\"Cher\" typeface=\"Plantagenet Cherokee\"/>");
        await writer.WriteAsync("			<a:font script=\"Yiii\" typeface=\"Microsoft Yi Baiti\"/>");
        await writer.WriteAsync("			<a:font script=\"Tibt\" typeface=\"Microsoft Himalaya\"/>");
        await writer.WriteAsync("			<a:font script=\"Thaa\" typeface=\"MV Boli\"/>");
        await writer.WriteAsync("			<a:font script=\"Deva\" typeface=\"Mangal\"/>");
        await writer.WriteAsync("			<a:font script=\"Telu\" typeface=\"Gautami\"/>");
        await writer.WriteAsync("			<a:font script=\"Taml\" typeface=\"Latha\"/>");
        await writer.WriteAsync("			<a:font script=\"Syrc\" typeface=\"Estrangelo Edessa\"/>");
        await writer.WriteAsync("			<a:font script=\"Orya\" typeface=\"Kalinga\"/>");
        await writer.WriteAsync("			<a:font script=\"Mlym\" typeface=\"Kartika\"/>");
        await writer.WriteAsync("			<a:font script=\"Laoo\" typeface=\"DokChampa\"/>");
        await writer.WriteAsync("			<a:font script=\"Sinh\" typeface=\"Iskoola Pota\"/>");
        await writer.WriteAsync("			<a:font script=\"Mong\" typeface=\"Mongolian Baiti\"/>");
        await writer.WriteAsync("			<a:font script=\"Viet\" typeface=\"Times New Roman\"/>");
        await writer.WriteAsync("			<a:font script=\"Uigh\" typeface=\"Microsoft Uighur\"/>");
        await writer.WriteAsync("			<a:font script=\"Geor\" typeface=\"Sylfaen\"/>");
        await writer.WriteAsync("		</a:majorFont>");
        await writer.WriteAsync("		<a:minorFont>");
        await writer.WriteAsync("			<a:latin typeface=\"Calibri\" panose=\"020F0502020204030204\"/>");
        await writer.WriteAsync("			<a:ea typeface=\"\"/>");
        await writer.WriteAsync("			<a:cs typeface=\"\"/>");
        await writer.WriteAsync("			<a:font script=\"Jpan\" typeface=\"游ゴシック\"/>");
        await writer.WriteAsync("			<a:font script=\"Hang\" typeface=\"맑은 고딕\"/>");
        await writer.WriteAsync("			<a:font script=\"Hans\" typeface=\"等线\"/>");
        await writer.WriteAsync("			<a:font script=\"Hant\" typeface=\"新細明體\"/>");
        await writer.WriteAsync("			<a:font script=\"Arab\" typeface=\"Arial\"/>");
        await writer.WriteAsync("			<a:font script=\"Hebr\" typeface=\"Arial\"/>");
        await writer.WriteAsync("			<a:font script=\"Thai\" typeface=\"Tahoma\"/>");
        await writer.WriteAsync("			<a:font script=\"Ethi\" typeface=\"Nyala\"/>");
        await writer.WriteAsync("			<a:font script=\"Beng\" typeface=\"Vrinda\"/>");
        await writer.WriteAsync("			<a:font script=\"Gujr\" typeface=\"Shruti\"/>");
        await writer.WriteAsync("			<a:font script=\"Khmr\" typeface=\"DaunPenh\"/>");
        await writer.WriteAsync("			<a:font script=\"Knda\" typeface=\"Tunga\"/>");
        await writer.WriteAsync("			<a:font script=\"Guru\" typeface=\"Raavi\"/>");
        await writer.WriteAsync("			<a:font script=\"Cans\" typeface=\"Euphemia\"/>");
        await writer.WriteAsync("			<a:font script=\"Cher\" typeface=\"Plantagenet Cherokee\"/>");
        await writer.WriteAsync("			<a:font script=\"Yiii\" typeface=\"Microsoft Yi Baiti\"/>");
        await writer.WriteAsync("			<a:font script=\"Tibt\" typeface=\"Microsoft Himalaya\"/>");
        await writer.WriteAsync("			<a:font script=\"Thaa\" typeface=\"MV Boli\"/>");
        await writer.WriteAsync("			<a:font script=\"Deva\" typeface=\"Mangal\"/>");
        await writer.WriteAsync("			<a:font script=\"Telu\" typeface=\"Gautami\"/>");
        await writer.WriteAsync("			<a:font script=\"Taml\" typeface=\"Latha\"/>");
        await writer.WriteAsync("			<a:font script=\"Syrc\" typeface=\"Estrangelo Edessa\"/>");
        await writer.WriteAsync("			<a:font script=\"Orya\" typeface=\"Kalinga\"/>");
        await writer.WriteAsync("			<a:font script=\"Mlym\" typeface=\"Kartika\"/>");
        await writer.WriteAsync("			<a:font script=\"Laoo\" typeface=\"DokChampa\"/>");
        await writer.WriteAsync("			<a:font script=\"Sinh\" typeface=\"Iskoola Pota\"/>");
        await writer.WriteAsync("			<a:font script=\"Mong\" typeface=\"Mongolian Baiti\"/>");
        await writer.WriteAsync("			<a:font script=\"Viet\" typeface=\"Arial\"/>");
        await writer.WriteAsync("			<a:font script=\"Uigh\" typeface=\"Microsoft Uighur\"/>");
        await writer.WriteAsync("			<a:font script=\"Geor\" typeface=\"Sylfaen\"/>");
        await writer.WriteAsync("		</a:minorFont>");
        await writer.WriteAsync("	</a:fontScheme>");
        await writer.WriteAsync("	<a:fmtScheme name=\"Office\">");
        await writer.WriteAsync("		<a:fillStyleLst>");
        await writer.WriteAsync("			<a:solidFill><a:schemeClr val=\"phClr\"/></a:solidFill>");
        await writer.WriteAsync("			<a:gradFill rotWithShape=\"1\">");
        await writer.WriteAsync("				<a:gsLst>");
        await writer.WriteAsync("					<a:gs pos=\"0\">");
        await writer.WriteAsync("						<a:schemeClr val=\"phClr\">");
        await writer.WriteAsync("							<a:lumMod val=\"110000\"/>");
        await writer.WriteAsync("							<a:satMod val=\"105000\"/>");
        await writer.WriteAsync("							<a:tint val=\"67000\"/>");
        await writer.WriteAsync("						</a:schemeClr>");
        await writer.WriteAsync("					</a:gs>");
        await writer.WriteAsync("					<a:gs pos=\"50000\">");
        await writer.WriteAsync("						<a:schemeClr val=\"phClr\">");
        await writer.WriteAsync("							<a:lumMod val=\"105000\"/>");
        await writer.WriteAsync("							<a:satMod val=\"103000\"/>");
        await writer.WriteAsync("							<a:tint val=\"73000\"/>");
        await writer.WriteAsync("						</a:schemeClr>");
        await writer.WriteAsync("					</a:gs>");
        await writer.WriteAsync("					<a:gs pos=\"100000\">");
        await writer.WriteAsync("						<a:schemeClr val=\"phClr\">");
        await writer.WriteAsync("							<a:lumMod val=\"105000\"/>");
        await writer.WriteAsync("							<a:satMod val=\"109000\"/>");
        await writer.WriteAsync("							<a:tint val=\"81000\"/>");
        await writer.WriteAsync("						</a:schemeClr>");
        await writer.WriteAsync("					</a:gs>");
        await writer.WriteAsync("				</a:gsLst>");
        await writer.WriteAsync("				<a:lin ang=\"5400000\" scaled=\"0\"/>");
        await writer.WriteAsync("			</a:gradFill>");
        await writer.WriteAsync("			<a:gradFill rotWithShape=\"1\">");
        await writer.WriteAsync("				<a:gsLst>");
        await writer.WriteAsync("					<a:gs pos=\"0\">");
        await writer.WriteAsync("						<a:schemeClr val=\"phClr\">");
        await writer.WriteAsync("							<a:satMod val=\"103000\"/>");
        await writer.WriteAsync("							<a:lumMod val=\"102000\"/>");
        await writer.WriteAsync("							<a:tint val=\"94000\"/>");
        await writer.WriteAsync("						</a:schemeClr>");
        await writer.WriteAsync("					</a:gs>");
        await writer.WriteAsync("					<a:gs pos=\"50000\">");
        await writer.WriteAsync("						<a:schemeClr val=\"phClr\">");
        await writer.WriteAsync("							<a:satMod val=\"110000\"/>");
        await writer.WriteAsync("							<a:lumMod val=\"100000\"/>");
        await writer.WriteAsync("							<a:shade val=\"100000\"/>");
        await writer.WriteAsync("						</a:schemeClr>");
        await writer.WriteAsync("					</a:gs>");
        await writer.WriteAsync("					<a:gs pos=\"100000\">");
        await writer.WriteAsync("						<a:schemeClr val=\"phClr\">");
        await writer.WriteAsync("							<a:lumMod val=\"99000\"/>");
        await writer.WriteAsync("							<a:satMod val=\"120000\"/>");
        await writer.WriteAsync("							<a:shade val=\"78000\"/>");
        await writer.WriteAsync("						</a:schemeClr>");
        await writer.WriteAsync("					</a:gs>");
        await writer.WriteAsync("				</a:gsLst>");
        await writer.WriteAsync("				<a:lin ang=\"5400000\" scaled=\"0\"/>");
        await writer.WriteAsync("			</a:gradFill>");
        await writer.WriteAsync("		</a:fillStyleLst>");
        await writer.WriteAsync("		<a:lnStyleLst>");
        await writer.WriteAsync("			<a:ln w=\"6350\" cap=\"flat\" cmpd=\"sng\" algn=\"ctr\">");
        await writer.WriteAsync("				<a:solidFill><a:schemeClr val=\"phClr\"/></a:solidFill>");
        await writer.WriteAsync("				<a:prstDash val=\"solid\"/>");
        await writer.WriteAsync("				<a:miter lim=\"800000\"/>");
        await writer.WriteAsync("			</a:ln>");
        await writer.WriteAsync("			<a:ln w=\"12700\" cap=\"flat\" cmpd=\"sng\" algn=\"ctr\">");
        await writer.WriteAsync("				<a:solidFill><a:schemeClr val=\"phClr\"/></a:solidFill>");
        await writer.WriteAsync("				<a:prstDash val=\"solid\"/>");
        await writer.WriteAsync("				<a:miter lim=\"800000\"/>");
        await writer.WriteAsync("			</a:ln>");
        await writer.WriteAsync("			<a:ln w=\"19050\" cap=\"flat\" cmpd=\"sng\" algn=\"ctr\">");
        await writer.WriteAsync("				<a:solidFill><a:schemeClr val=\"phClr\"/></a:solidFill>");
        await writer.WriteAsync("				<a:prstDash val=\"solid\"/>");
        await writer.WriteAsync("				<a:miter lim=\"800000\"/>");
        await writer.WriteAsync("			</a:ln>");
        await writer.WriteAsync("		</a:lnStyleLst>");
        await writer.WriteAsync("		<a:effectStyleLst>");
        await writer.WriteAsync("			<a:effectStyle><a:effectLst/></a:effectStyle>");
        await writer.WriteAsync("			<a:effectStyle><a:effectLst/></a:effectStyle>");
        await writer.WriteAsync("			<a:effectStyle>");
        await writer.WriteAsync("				<a:effectLst>");
        await writer.WriteAsync("					<a:outerShdw blurRad=\"57150\" dist=\"19050\" dir=\"5400000\" algn=\"ctr\" rotWithShape=\"0\">");
        await writer.WriteAsync("						<a:srgbClr val=\"000000\"><a:alpha val=\"63000\"/></a:srgbClr>");
        await writer.WriteAsync("					</a:outerShdw>");
        await writer.WriteAsync("				</a:effectLst>");
        await writer.WriteAsync("			</a:effectStyle>");
        await writer.WriteAsync("		</a:effectStyleLst>");
        await writer.WriteAsync("		<a:bgFillStyleLst>");
        await writer.WriteAsync("			<a:solidFill><a:schemeClr val=\"phClr\"/></a:solidFill>");
        await writer.WriteAsync("			<a:solidFill>");
        await writer.WriteAsync("				<a:schemeClr val=\"phClr\">");
        await writer.WriteAsync("					<a:tint val=\"95000\"/>");
        await writer.WriteAsync("					<a:satMod val=\"170000\"/>");
        await writer.WriteAsync("				</a:schemeClr>");
        await writer.WriteAsync("			</a:solidFill>");
        await writer.WriteAsync("			<a:gradFill rotWithShape=\"1\">");
        await writer.WriteAsync("				<a:gsLst>");
        await writer.WriteAsync("					<a:gs pos=\"0\">");
        await writer.WriteAsync("						<a:schemeClr val=\"phClr\">");
        await writer.WriteAsync("							<a:tint val=\"93000\"/>");
        await writer.WriteAsync("							<a:satMod val=\"150000\"/>");
        await writer.WriteAsync("							<a:shade val=\"98000\"/>");
        await writer.WriteAsync("							<a:lumMod val=\"102000\"/>");
        await writer.WriteAsync("						</a:schemeClr>");
        await writer.WriteAsync("					</a:gs>");
        await writer.WriteAsync("					<a:gs pos=\"50000\">");
        await writer.WriteAsync("						<a:schemeClr val=\"phClr\">");
        await writer.WriteAsync("							<a:tint val=\"98000\"/>");
        await writer.WriteAsync("							<a:satMod val=\"130000\"/>");
        await writer.WriteAsync("							<a:shade val=\"90000\"/>");
        await writer.WriteAsync("							<a:lumMod val=\"103000\"/>");
        await writer.WriteAsync("						</a:schemeClr>");
        await writer.WriteAsync("					</a:gs>");
        await writer.WriteAsync("					<a:gs pos=\"100000\">");
        await writer.WriteAsync("						<a:schemeClr val=\"phClr\">");
        await writer.WriteAsync("							<a:shade val=\"63000\"/>");
        await writer.WriteAsync("							<a:satMod val=\"120000\"/>");
        await writer.WriteAsync("						</a:schemeClr>");
        await writer.WriteAsync("					</a:gs>");
        await writer.WriteAsync("				</a:gsLst>");
        await writer.WriteAsync("				<a:lin ang=\"5400000\" scaled=\"0\"/>");
        await writer.WriteAsync("			</a:gradFill>");
        await writer.WriteAsync("		</a:bgFillStyleLst>");
        await writer.WriteAsync("	</a:fmtScheme>");
        await writer.WriteAsync("</a:themeElements>");
        await writer.WriteAsync("<a:objectDefaults/>");
        await writer.WriteAsync("<a:extraClrSchemeLst/>");
        await writer.WriteAsync("<a:extLst>");
        await writer.WriteAsync("	<a:ext uri=\"{05A4C25C-085E-4340-85A3-A5531E510DB2}\">");
        await writer.WriteAsync("		<thm15:themeFamily xmlns:thm15=\"http://schemas.microsoft.com/office/thememl/2012/main\" name=\"Office Theme\" id=\"{62F939B6-93AF-4DB8-9C6B-D6C7DFDC589F}\" vid=\"{4A3C46E8-61CC-4603-A589-7422A47A8E4A}\"/>");
        await writer.WriteAsync("	</a:ext>");
        await writer.WriteAsync("</a:extLst>");
        await writer.WriteAsync("</a:theme>");

        await writer.FlushAsync();
        await zipStream.FlushAsync();
        writer.Close();
        zipStream.Close();
    }
    private async Task RenderStyle(ZipArchive archive)
    {
        var path = "xl/styles.xml";
        var entry = archive.CreateEntry(path);
        using var zipStream = entry.Open();
        using var writer = new StreamWriter(zipStream, encoding);

        await writer.WriteAsync("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
        await writer.WriteAsync("<x:styleSheet xmlns:x=\"http://schemas.openxmlformats.org/spreadsheetml/2006/main\">");

        //数字格式
        await writer.WriteAsync($"<x:numFmts count=\"{this.numFmtIds.Count + 1}\">");
        await writer.WriteAsync("	<x:numFmt numFmtId=\"0\" formatCode=\"\"/>");
        var sortedNumFmtIds = this.numFmtIds.OrderBy(f => f.Value);
        foreach (var numFmtId in sortedNumFmtIds)
        {
            await writer.WriteAsync($"	<x:numFmt numFmtId=\"{numFmtId.Value}\" formatCode=\"{numFmtId.Key}\"/>");
        }
        await writer.WriteAsync("</x:numFmts>");

        //字体
        await writer.WriteAsync($"<x:fonts count=\"{this.fontIds.Count}\">");
        var sortedFontIds = this.fontIds.OrderBy(f => f.Value);
        foreach (var fontId in sortedFontIds)
        {
            await fontId.Key.Render(writer);
        }
        await writer.WriteAsync("</x:fonts>");
        await writer.WriteAsync("<x:fills count=\"2\">");
        await writer.WriteAsync("	<x:fill><patternFill patternType=\"none\"/></x:fill>");
        await writer.WriteAsync("	<x:fill><patternFill patternType=\"gray125\"/></x:fill>");
        await writer.WriteAsync("</x:fills>");

        //边框
        await writer.WriteAsync($"<x:borders count=\"{this.borderIds.Count}\">");
        var sortedBorderIds = this.borderIds.OrderBy(f => f.Value);
        foreach (var borderId in sortedBorderIds)
        {
            await borderId.Key.Render(writer);
        }
        await writer.WriteAsync("</x:borders>");

        await writer.WriteAsync($"<x:cellStyleXfs count=\"{this.xfsStyles.Count}\">");
        var sortedXfsStyles = this.xfsStyles.OrderBy(f => f.Value.Index);
        foreach (var xfsStyle in sortedXfsStyles)
        {
            await xfsStyle.Value.Style.RenderDefine(writer);
        }
        await writer.WriteAsync("</x:cellStyleXfs>");

        await writer.WriteAsync($"<x:cellXfs count=\"{this.styleIds.Count}\">");
        var sortedStyleIds = this.styleIds.OrderBy(f => f.Value);
        foreach (var styleId in sortedStyleIds)
        {
            await styleId.Key.RenderBody(writer);
        }
        await writer.WriteAsync("</x:cellXfs>");

        await writer.WriteAsync("<x:cellStyles count=\"1\">");
        await writer.WriteAsync("	<x:cellStyle name=\"Normal\" xfId=\"0\" builtinId=\"0\"/>");
        await writer.WriteAsync("</x:cellStyles>");
        await writer.WriteAsync("</x:styleSheet>");

        await writer.FlushAsync();
        await zipStream.FlushAsync();
        writer.Close();
        zipStream.Close();
    }
    private async Task RenderContentTypes(ZipArchive archive)
    {
        var path = "[Content_Types].xml";
        var entry = archive.CreateEntry(path);
        using var zipStream = entry.Open();
        using var writer = new StreamWriter(zipStream, encoding);

        await writer.WriteAsync("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
        await writer.WriteAsync("<Types xmlns=\"http://schemas.openxmlformats.org/package/2006/content-types\">");
        await writer.WriteAsync("	<Default Extension=\"xml\" ContentType=\"application/vnd.openxmlformats-officedocument.spreadsheetml.sheet.main+xml\"/>");
        await writer.WriteAsync("	<Default Extension=\"rels\" ContentType=\"application/vnd.openxmlformats-package.relationships+xml\"/>");
        await writer.WriteAsync("	<Override PartName=\"/xl/workbook.xml\" ContentType=\"application/vnd.openxmlformats-officedocument.spreadsheetml.sheet.main+xml\"/>");
        await writer.WriteAsync("	<Override PartName=\"/xl/worksheets/sheet1.xml\" ContentType=\"application/vnd.openxmlformats-officedocument.spreadsheetml.worksheet+xml\"/>");
        await writer.WriteAsync("	<Override PartName=\"/xl/theme/theme1.xml\" ContentType=\"application/vnd.openxmlformats-officedocument.theme+xml\"/>");
        await writer.WriteAsync("	<Override PartName=\"/xl/styles.xml\" ContentType=\"application/vnd.openxmlformats-officedocument.spreadsheetml.styles+xml\"/>");
        await writer.WriteAsync("	<Override PartName=\"/xl/sharedStrings.xml\" ContentType=\"application/vnd.openxmlformats-officedocument.spreadsheetml.sharedStrings+xml\"/>");
        await writer.WriteAsync("</Types>");

        await writer.FlushAsync();
        await zipStream.FlushAsync();
        writer.Close();
        zipStream.Close();
    }
    private int NextId() => index++;
}
