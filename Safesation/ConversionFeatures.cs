using System;

namespace Safesation
{
    [Flags]
    public enum ConversionFeatures
    {
        None = 0,
        Culture = 1,
        Format = 2,
        CultureFormat = Culture | Format, // 3
        Styles = 4,
        CultureStyles = Culture | Styles, // 5
        FormatStyles = Format | Styles, // 6
        CultureFormatStyles = Culture | Format | Styles // 7
    }
}