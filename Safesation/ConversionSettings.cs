using System.Diagnostics;
using System.Globalization;

namespace Safesation
{
    using Annotations;

    [DebuggerDisplay("{DebuggerDisplay}")]
    public struct ConversionSettings
    {
        #region CONSTRUCTORS

        public ConversionSettings(CultureInfo culture)
        {
            _culture = culture;
            _features = ConversionFeatures.Culture;
            _flags = 0;
            _format = null;
            _style = ConversionStyles.None;
        }

        public ConversionSettings(CultureInfo culture, DateTimeStyles styles)
        {
            _culture = culture;
            _features = ConversionFeatures.CultureStyles;
            _flags = (int) styles;
            _format = null;
            _style = ConversionStyles.DateTime;
        }

        public ConversionSettings(CultureInfo culture, NumberStyles styles)
        {
            _culture = culture;
            _features = ConversionFeatures.CultureStyles;
            _flags = (int) styles;
            _format = null;
            _style = ConversionStyles.Number;
        }

        public ConversionSettings(CultureInfo culture, string format)
        {
            _culture = culture;
            _features = ConversionFeatures.CultureFormat;
            _flags = 0;
            _format = format;
            _style = ConversionStyles.None;
        }

        public ConversionSettings(CultureInfo culture, TimeSpanStyles styles)
        {
            _culture = culture;
            _features = ConversionFeatures.CultureStyles;
            _flags = (int) styles;
            _format = null;
            _style = ConversionStyles.TimeSpan;
        }

        public ConversionSettings(CultureInfo culture, string format, DateTimeStyles styles)
        {
            _culture = culture;
            _features = ConversionFeatures.CultureFormatStyles;
            _flags = (int) styles;
            _format = format;
            _style = ConversionStyles.DateTime;
        }

        public ConversionSettings(CultureInfo culture, string format, NumberStyles styles)
        {
            _culture = culture;
            _features = ConversionFeatures.CultureFormatStyles;
            _flags = (int) styles;
            _format = format;
            _style = ConversionStyles.Number;
        }

        public ConversionSettings(CultureInfo culture, string format, TimeSpanStyles styles)
        {
            _culture = culture;
            _features = ConversionFeatures.CultureFormatStyles;
            _flags = (int) styles;
            _format = format;
            _style = ConversionStyles.TimeSpan;
        }

        public ConversionSettings(DateTimeStyles styles)
        {
            _culture = null;
            _features = ConversionFeatures.Styles;
            _flags = (int)styles;
            _format = null;
            _style = ConversionStyles.DateTime;
        }

        public ConversionSettings(NumberStyles styles)
        {
            _culture = null;
            _features = ConversionFeatures.Styles;
            _flags = (int)styles;
            _format = null;
            _style = ConversionStyles.Number;
        }

        public ConversionSettings(string format)
        {
            _culture = null;
            _features = ConversionFeatures.None;
            _flags = 0;
            _format = format;
            _style = ConversionStyles.None;
        }

        public ConversionSettings(string format, DateTimeStyles styles)
        {
            _culture = null;
            _features = ConversionFeatures.FormatStyles;
            _flags = (int)styles;
            _format = format;
            _style = ConversionStyles.DateTime;
        }

        public ConversionSettings(string format, NumberStyles styles)
        {
            _culture = null;
            _features = ConversionFeatures.FormatStyles;
            _flags = (int)styles;
            _format = format;
            _style = ConversionStyles.Number;
        }

        public ConversionSettings(string format, TimeSpanStyles styles)
        {
            _culture = null;
            _features = ConversionFeatures.FormatStyles;
            _flags = (int)styles;
            _format = format;
            _style = ConversionStyles.TimeSpan;
        }

        public ConversionSettings(TimeSpanStyles styles)
        {
            _culture = null;
            _features = ConversionFeatures.Styles;
            _flags = (int)styles;
            _format = null;
            _style = ConversionStyles.TimeSpan;
        }

        #endregion

        #region FIELDS

        private readonly CultureInfo _culture;

        private readonly ConversionFeatures _features;

        private readonly int _flags;

        private readonly string _format;

        private readonly ConversionStyles _style;

        #endregion

        #region PROPERTIES

        [UsedImplicitly]
        public CultureInfo Culture
        {
            get { return _culture; }
        }

        public DateTimeStyles DateTimeStyles
        {
            get { return _style == ConversionStyles.DateTime ? (DateTimeStyles) _flags : DateTimeStyles.None; }
        }

        [UsedImplicitly]
        internal string DebuggerDisplay
        {
            get
            {
                var styles = "None";
                switch (_style)
                {
                    case ConversionStyles.DateTime:
                        styles = DateTimeStyles.ToString();
                        break;
                    case ConversionStyles.Number:
                        styles = NumberStyles.ToString();
                        break;
                    case ConversionStyles.TimeSpan:
                        styles = TimeSpanStyles.ToString();
                        break;
                }
                switch (Features)
                {
                    case ConversionFeatures.Culture:
                        return string.Format(
                            "Conversion for '{0}' culture",
                            _culture.ThreeLetterISOLanguageName);
                    case ConversionFeatures.CultureFormat:
                        return string.Format(
                            "Conversion for '{0}' culture with '{1}' format",
                            _culture.ThreeLetterISOLanguageName, Format);
                    case ConversionFeatures.CultureFormatStyles:
                        return string.Format(
                            "Conversion for '{0}' culture with '{1}' format and '{2}' styles",
                            _culture.ThreeLetterISOLanguageName, Format, styles);
                    case ConversionFeatures.CultureStyles:
                        return string.Format(
                            "Conversion for '{0}' culture with '{1}' styles",
                            _culture.ThreeLetterISOLanguageName, styles);
                    case ConversionFeatures.FormatStyles:
                        return string.Format(
                            "Conversion with '{0}' format and '{1}' styles",
                            Format, styles);
                    case ConversionFeatures.Styles:
                        return string.Format(
                            "Conversion with '{0}' styles",
                            styles);
                    default:
                        return "Conversion";
                }
            }
        }

        public ConversionFeatures Features
        {
            get { return _features; }
        }

        public string Format
        {
            get { return _format; }
        }

        public NumberStyles NumberStyles
        {
            get { return _style == ConversionStyles.Number ? (NumberStyles) _flags : NumberStyles.Any; }
        }

        public TimeSpanStyles TimeSpanStyles
        {
            get { return _style == ConversionStyles.TimeSpan ? (TimeSpanStyles) _flags : TimeSpanStyles.None; }
        }

        #endregion
    }
}