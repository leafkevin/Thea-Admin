﻿using System;
using System.Globalization;

namespace Thea.ExcelExporter;

internal static class Extensions
{
    public static string ToInvariantString(this object value) =>
        value switch
        {
            null => string.Empty,
            sbyte v => v.ToString(CultureInfo.InvariantCulture),
            byte v => v.ToString(CultureInfo.InvariantCulture),
            short v => v.ToString(CultureInfo.InvariantCulture),
            ushort v => v.ToString(CultureInfo.InvariantCulture),
            int v => v.ToString(CultureInfo.InvariantCulture),
            uint v => v.ToString(CultureInfo.InvariantCulture),
            long v => v.ToString(CultureInfo.InvariantCulture),
            ulong v => v.ToString(CultureInfo.InvariantCulture),
            float v => v.ToString("G7", CultureInfo.InvariantCulture), // Specify precision explicitly for backward compatibility
            double v => v.ToString("G15", CultureInfo.InvariantCulture), // Specify precision explicitly for backward compatibility
            decimal v => v.ToString(CultureInfo.InvariantCulture),
            TimeSpan ts => ts.ToString("c", CultureInfo.InvariantCulture),
            DateTime d => d.ToString(CultureInfo.InvariantCulture),
            bool b => b.ToString().ToLowerInvariant(),
            _ => value.ToString(),
        };    
}