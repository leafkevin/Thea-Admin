using System;

namespace Thea.ExcelExporter;

struct NumberFormat
{
    public int NumFmtId { get; set; }
    public string Format { get; set; }

    public override int GetHashCode() => HashCode.Combine(this.NumFmtId, this.Format);
}
