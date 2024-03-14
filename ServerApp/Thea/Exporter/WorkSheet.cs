using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Thea.ExcelExporter;

public class WorkSheet
{
    private readonly List<string> mergedLetters = new();
    private readonly Dictionary<string, ExcelRange> lastMergedRanges = new();

    public int Id { get; internal set; }
    public string RId { get; internal set; }
    public string SheetName { get; set; }
    public double DefaultRowHeight { get; set; } = 15d;
    public double DefaultColumnWidth { get; set; } = 11.5d;
    public CellHorizontalAlignment HorizontalAlignment { get; set; } = CellHorizontalAlignment.General;
    public CellVerticalAlignment VerticalAlignment { get; set; } = CellVerticalAlignment.Center;
    public string Dimension { get; set; }
    public int RowCount { get; set; }
    public bool IsAllowMerge { get; set; }
    public List<ExcelColumnHeader> Headers { get; set; }
    public CellMergePredicate CellMergePredicate { get; set; }
    public Workbook Parent { get; internal set; }
    internal IEnumerable Data { get; set; }

    internal async Task WriteReference(StreamWriter writer)
    {
        await writer.WriteAsync($"<x:sheet name=\"{this.SheetName}\" sheetId=\"{this.Id}\" r:id=\"{this.RId}\"/>");
    }
    internal async Task Render(ZipArchive archive, UTF8Encoding encoding)
    {
        this.Initialize();
        var path = $"xl/worksheets/sheet{this.Id}.xml";
        var entry = archive.CreateEntry(path);
        using var zipStream = entry.Open();
        using var writer = new StreamWriter(zipStream, encoding);

        await WriteHeader(writer);
        await WriteBody(writer);
        await WriteFooter(writer);

        await writer.FlushAsync();
        await zipStream.FlushAsync();
        writer.Close();
        zipStream.Close();
    }
    private void Initialize()
    {
        string letter = null;
        for (int i = 0; i < this.Headers.Count; i++)
        {
            if (i > 26)
            {
                var first = 65 + i / 26;
                var second = i % 26;
                letter = $"A1:{(char)first}{(char)second}";
            }
            else letter = $"{(char)(i + 65)}";
            this.Headers[i].Letter = letter;
        }
        if (this.IsAllowMerge && this.CellMergePredicate == null)
            throw new Exception("当前设置为可以合并单元格，断言CellMergePredicate未设置");

        if (this.Data is IList list)
            this.RowCount = list.Count;
        else if (this.Data is Array array)
            this.RowCount = array.Length;
        else if (this.Data is ICollection collection)
            this.RowCount = collection.Count;
        else
        {
            var index = 0;
            foreach (var item in this.Data)
                index++;
            this.RowCount = index;
        }
        this.Dimension = $"A1:{letter}{this.RowCount + 1}";

        foreach (var entity in this.Data)
        {
            foreach (var header in this.Headers)
            {
                this.BuildGetFunc(entity, header);
            }
            break;
        }
    }
    private async Task WriteHeader(StreamWriter writer)
    {
        await writer.WriteAsync("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
        await writer.WriteAsync("<x:worksheet xmlns:r=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships\" xmlns:x=\"http://schemas.openxmlformats.org/spreadsheetml/2006/main\">");
        await writer.WriteAsync($"<x:dimension ref=\"{this.Dimension}\"/>");
        await writer.WriteAsync("<x:sheetViews>");
        await writer.WriteAsync("<x:sheetView workbookViewId=\"0\"/>");
        await writer.WriteAsync("</x:sheetViews>");
        await writer.WriteAsync($"<x:sheetFormatPr defaultRowHeight=\"{this.DefaultRowHeight}\" defaultColWidth=\"{this.DefaultColumnWidth}\"/>");
        if (this.Headers.Exists(f => f.ColumnWidth.HasValue))
        {
            int minIndex = 0, index = 0;
            await writer.WriteAsync("<x:cols>");
            ExcelColumnHeader header = null;
            while (index < this.Headers.Count)
            {
                header = this.Headers[index];
                if (!header.ColumnWidth.HasValue)
                {
                    index++;
                    continue;
                }
                await writer.WriteAsync($"<x:col min=\"{minIndex + 1}\" max=\"{index + 1}\" width=\"{header.ColumnWidth}\" style=\"0\" customWidth=\"1\"/>");
                index++;
                minIndex = index;
            }
            if (minIndex < index)
                await writer.WriteAsync($"<x:col min=\"{minIndex + 1}\" max=\"{index}\" width=\"{this.DefaultColumnWidth}\" style=\"0\" customWidth=\"1\"/>");

            await writer.WriteAsync("</x:cols>");
        }
        await writer.WriteAsync("<x:sheetData>");
    }
    private async Task WriteBody(StreamWriter writer)
    {
        var headerRow = this.CreateHeaderRow(1);
        await headerRow.WriteHeader(writer);

        int rowIndex = 1;
        ExcelRow lastRow = null;
        foreach (var entity in this.Data)
        {
            var row = this.CreateDataRow(rowIndex, entity);
            //处理列合并
            this.MergeCells(row, lastRow);

            //当前行不输出，只输出前行
            if (lastRow != null)
                await lastRow.RenderBody(writer);

            lastRow = row;
            rowIndex++;
        }
        //先统计一下已经合并的单元格
        this.StatisticsMergedCells();
        //再输出最后一行
        if (lastRow != null)
            await lastRow.RenderBody(writer);
    }
    private async Task WriteFooter(StreamWriter writer)
    {
        await writer.WriteLineAsync("	</x:sheetData>");
        if (this.mergedLetters.Count > 0)
        {
            await writer.WriteAsync($"<x:mergeCells count=\"{this.mergedLetters.Count}\">");
            foreach (var mergedLetter in this.mergedLetters)
            {
                await writer.WriteAsync($"<x:mergeCell ref=\"{mergedLetter}\" />");
            }
            await writer.WriteAsync("</x:mergeCells>");
        }
        await writer.WriteAsync("<x:printOptions horizontalCentered=\"0\" verticalCentered=\"0\" headings=\"0\" gridLines=\"0\"/>");
        await writer.WriteAsync("<x:pageMargins left=\"0.75\" right=\"0.75\" top=\"0.75\" bottom=\"0.5\" header=\"0.5\" footer=\"0.75\"/>");
        await writer.WriteAsync("<x:pageSetup paperSize=\"1\" scale=\"100\" pageOrder=\"downThenOver\" orientation=\"default\" blackAndWhite=\"0\" draft=\"0\" cellComments=\"none\" errors=\"displayed\"/>");
        await writer.WriteAsync("<x:headerFooter/>");
        await writer.WriteAsync("<x:tableParts count=\"0\"/>");
        await writer.WriteAsync("</x:worksheet>");
    }
    private ExcelRow CreateHeaderRow(int rowIndex)
    {
        var row = new ExcelRow(this.Parent)
        {
            RowIndex = rowIndex,
            Headers = this.Headers,
            Cells = new List<ExcelCell>()
        };

        //处理当前行各列样式
        for (int index = 0; index < this.Headers.Count; index++)
        {
            var header = this.Headers[index];
            row.Cells.Add(new ExcelCell(this.Parent)
            {
                RowIndex = rowIndex,
                ColumnIndex = index,
                CellLetter = $"{header.Letter}{rowIndex + 1}",
                Header = header
            });
        }
        return row;
    }
    private ExcelRow CreateDataRow(int rowIndex, object entity)
    {
        var row = new ExcelRow(this.Parent)
        {
            RowIndex = rowIndex,
            Headers = this.Headers,
            Cells = new List<ExcelCell>(),
            RowData = entity,
        };

        //处理当前行各列样式
        var mergedKeys = this.lastMergedRanges.Keys.ToList();
        for (int index = 0; index < this.Headers.Count; index++)
        {
            var header = this.Headers[index];
            var objValue = header.DataFetcher.Invoke(entity);
            var cell = new ExcelCell(this.Parent)
            {
                RowIndex = rowIndex,
                ColumnIndex = index,
                CellLetter = $"{header.Letter}{rowIndex + 1}",
                HorizontalAlignment = header.HorizontalAlignment ?? this.HorizontalAlignment,
                VerticalAlignment = header.VerticalAlignment ?? this.VerticalAlignment,
                Header = header,
                Value = objValue
            };
            row.Cells.Add(cell);
        }
        return row;
    }
    private void MergeCells(ExcelRow row, ExcelRow lastRow)
    {
        if (!this.IsAllowMerge) return;
        if (this.CellMergePredicate == null) return;
        if (this.CellMergePredicate(row, lastRow, out var ranges))
        {
            //合并的时候，处理合并单元格的边框
            foreach (var range in ranges)
            {
                var startRowIndex = range.Start.RowIndex;
                var endRownIndex = range.End.RowIndex;
                if (endRownIndex == startRowIndex) continue;

                var startColumnIndex = range.Start.ColumnIndex;
                var endColumnIndex = range.End.ColumnIndex;

                //存在，则合并
                if (this.lastMergedRanges.TryGetValue(range.ColumnLetters, out var mergedRange))
                {
                    mergedRange.Merge(range);

                    //处理单元格样式
                    for (int i = startColumnIndex; i <= endColumnIndex; i++)
                    {
                        var curCell = row.Cells[i] as ExcelCell;

                        //处理列合并单元格边框
                        if (i > startColumnIndex)
                        {
                            //不是第一个，就去掉左边
                            curCell.BorderLine = curCell.BorderLine & 14;
                            curCell.IsMerged = true;
                            curCell.MergedRange = mergedRange;
                        }
                        if (i < endColumnIndex)
                        {
                            //不是最后一个，就去掉右边
                            curCell.BorderLine = curCell.BorderLine & 11;
                            curCell.IsMerged = true;
                            curCell.MergedRange = mergedRange;
                        }

                        //处理行合并单元格
                        if (lastRow != null)
                        {
                            var lastCell = lastRow.Cells[i] as ExcelCell;

                            //上一行单元格，去掉下边
                            lastCell.BorderLine = lastCell.BorderLine & 7;
                            lastCell.IsMerged = true;
                            lastCell.MergedRange = mergedRange;

                            if (range.HorizontalAlignment.HasValue)
                                lastCell.HorizontalAlignment = range.HorizontalAlignment.Value;
                            if (range.VerticalAlignment.HasValue)
                                lastCell.VerticalAlignment = range.VerticalAlignment.Value;
                        }

                        //当前行单元格，去掉上边
                        curCell.BorderLine = curCell.BorderLine & 13;
                        curCell.IsMerged = true;
                        curCell.MergedRange = mergedRange;

                        if (range.HorizontalAlignment.HasValue)
                            curCell.HorizontalAlignment = range.HorizontalAlignment.Value;
                        if (range.VerticalAlignment.HasValue)
                            curCell.VerticalAlignment = range.VerticalAlignment.Value;
                    }
                }
                //不存在，则添加
                else this.lastMergedRanges.Add(range.ColumnLetters, mergedRange = range);
            }
            //去掉不再合并的单元格
            var exceptedKeys = ranges.Select(f => f.ColumnLetters);
            this.StatisticsMergedCells(exceptedKeys);
        }
        else
        {
            //上一行，没有合并的单元格，直接返回
            if (this.lastMergedRanges.Count <= 0) return;

            //不合并的时候，统计并终止所有合并单元格
            this.StatisticsMergedCells();
        }
    }
    private void StatisticsMergedCells(IEnumerable<string> exceptedKeys = null)
    {
        foreach (var mergedKey in this.lastMergedRanges.Keys)
        {
            if (exceptedKeys != null && exceptedKeys.Contains(mergedKey)) continue;
            if (this.lastMergedRanges.Remove(mergedKey, out var range))
                this.mergedLetters.Add(range.Range);
        }
    }
    private void BuildGetFunc(object entity, ExcelColumnHeader header)
    {
        if (entity is IDictionary<string, object>)
        {
            Func<object, object> func = obj =>
            {
                if (obj is IDictionary<string, object> dict)
                {
                    if (dict.TryGetValue(header.Field, out var value))
                        return value;
                }
                return null;
            };
            header.DataFetcher = func;
        }
        else
        {
            var type = entity.GetType();
            var property = type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).FirstOrDefault(f => f.Name == header.Field);
            if (property == null)
                throw new Exception($"属性{header.Field}不存在");

            var methodInfo = property.GetGetMethod();
            var objExpr = Expression.Parameter(typeof(object), "obj");
            var entityExpr = Expression.Variable(type, "entity");
            var returnLabel = Expression.Label(typeof(object));
            var blockExpr = Expression.Block(new ParameterExpression[] { entityExpr },
                Expression.Assign(entityExpr, Expression.Convert(objExpr, type)),
                Expression.Return(returnLabel, Expression.Convert(Expression.Call(entityExpr, methodInfo), typeof(object))),
                Expression.Label(returnLabel, Expression.Convert(Expression.Constant(null), typeof(object)))
            );
            var func = Expression.Lambda<Func<object, object>>(blockExpr, objExpr).Compile();
            header.DataFetcher = func;
        }
    }
}
