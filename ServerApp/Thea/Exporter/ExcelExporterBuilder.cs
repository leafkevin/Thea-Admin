using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Thea.ExcelExporter;

public class ExcelExporterBuilder
{
    protected ExcelExporterConfiguation configuation = null;
    protected IEnumerable exportData = null;

    public ExcelExporterBuilder(string sheetName = "Sheet1", double defaultRowHeight = 15d, double defaultColumnWidth = 11.5d)
    {
        this.configuation = new ExcelExporterConfiguation
        {
            SheetName = sheetName,
            DefaultRowHeight = defaultRowHeight,
            DefaultColumnWidth = defaultColumnWidth,
            Headers = new List<ExcelColumnHeader>()
        };
    }
    public ExcelExporterBuilder AddColumnHeader(Action<ExcelColumnHeaderBuilder> initializer)
    {
        if (initializer == null) throw new ArgumentNullException(nameof(initializer));

        var headerBuilder = new ExcelColumnHeaderBuilder();
        initializer.Invoke(headerBuilder);
        configuation.Headers.Add(headerBuilder.Build());
        return this;
    }
    public ExcelExporterBuilder WithMerge(CellMergePredicate cellMergePredicate)
    {
        this.configuation.CellMergePredicate = cellMergePredicate;
        return this;
    }
    public ExcelExporterBuilder<TEntity> WithData<TEntity>(List<TEntity> exportData)
    {
        var builder = new ExcelExporterBuilder<TEntity>(this.configuation);
        builder.WithData(exportData);
        return builder;
    }
    public async Task Export(Stream outputStream)
    {
        var workbook = new Workbook();
        workbook.AddSheet(new WorkSheet
        {
            SheetName = configuation.SheetName,
            IsAllowMerge = configuation.IsAllowMerge,
            DefaultRowHeight = configuation.DefaultRowHeight,
            DefaultColumnWidth = configuation.DefaultColumnWidth,

            Headers = configuation.Headers,
            CellMergePredicate = configuation.CellMergePredicate,
            Data = this.exportData
        });
        await workbook.SaveAs(outputStream);
    }
}
public class ExcelExporterBuilder<TEntity> : ExcelExporterBuilder
{
    public ExcelExporterBuilder(string sheetName = "Sheet1", double defaultRowHeight = 15d, double defaultColumnWidth = 11.5d)
       : base(sheetName, defaultRowHeight, defaultColumnWidth) { }

    public ExcelExporterBuilder(ExcelExporterConfiguation configuation)
        => this.configuation = configuation;
    public ExcelExporterBuilder<TEntity> AddColumnHeader(Action<ExcelColumnHeaderBuilder<TEntity>> initializer)
    {
        if (initializer == null) throw new ArgumentNullException(nameof(initializer));

        var headerBuilder = new ExcelColumnHeaderBuilder<TEntity>();
        initializer.Invoke(headerBuilder);
        this.configuation.Headers.Add(headerBuilder.Build());
        return this;
    }
    public new ExcelExporterBuilder<TEntity> WithMerge(CellMergePredicate cellMergePredicate)
    {
        base.WithMerge(cellMergePredicate);
        return this;
    }
    public ExcelExporterBuilder<TEntity> WithData(List<TEntity> exportData)
    {
        this.exportData = exportData;
        return this;
    }
}
public class ExcelColumnHeaderBuilder
{
    protected ExcelColumnHeader header = new ExcelColumnHeader();
    public ExcelColumnHeaderBuilder Field(string fieldName)
    {
        this.header.Field = fieldName;
        return this;
    }
    public ExcelColumnHeaderBuilder Title(string title)
    {
        this.header.Title = title;
        return this;
    }
    public ExcelColumnHeaderBuilder Format(string format)
    {
        this.header.Format = format;
        return this;
    }
    public ExcelColumnHeaderBuilder Horizontal(CellHorizontalAlignment alignment)
    {
        this.header.HorizontalAlignment = alignment;
        return this;
    }
    public ExcelColumnHeaderBuilder Vertical(CellVerticalAlignment alignment)
    {
        this.header.VerticalAlignment = alignment;
        return this;
    }
    public ExcelColumnHeaderBuilder Width(double width)
    {
        this.header.ColumnWidth = width;
        return this;
    }
    public ExcelColumnHeaderBuilder Decorator(Func<object, object> valueDecorator)
    {
        this.header.ValueDecorator = valueDecorator;
        return this;
    }
    public ExcelColumnHeader Build()
    {
        if (string.IsNullOrEmpty(this.header.Title))
            this.header.Title = this.header.Field;
        return this.header;
    }
}
public class ExcelColumnHeaderBuilder<TEntity> : ExcelColumnHeaderBuilder
{
    public ExcelColumnHeaderBuilder<TEntity> Field<TField>(Expression<Func<TEntity, TField>> fieldExpr)
    {
        if (fieldExpr == null)
            throw new ArgumentNullException(nameof(fieldExpr));
        var memberExpr = fieldExpr.Body as MemberExpression;
        this.header.Field = memberExpr.Member.Name;
        return this;
    }
}