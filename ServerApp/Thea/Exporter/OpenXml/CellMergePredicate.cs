using System.Collections.Generic;

namespace Thea.ExcelExporter;

public delegate bool CellMergePredicate(ExcelRow curRow, ExcelRow lastRow, out List<ExcelRange> ranges);
