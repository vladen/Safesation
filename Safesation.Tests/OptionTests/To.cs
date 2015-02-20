using System;
using System.Globalization;
using FluentAssertions;
using NUnit.Framework;

namespace Safesation.Tests.ConverterTests
{
    public class To
    {
        [Test]
        public void Convert_BooleanToString_ReturnsSome()
        {
            var values = new[] {false, true};
            foreach (var value in values)
            {
                var result = value.Safe().To<string>();
                result.IsSome
                    .Should().BeTrue("conversion result should be some");
                value.ToString().Equals(result, StringComparison.Ordinal)
                    .Should().BeTrue();
            }
        }

        [Test]
        public void Convert_DateTimeToString_ReturnsSome()
        {
            var cultures = new[]
            {
                null,
                CultureInfo.InvariantCulture,
                CultureInfo.CurrentUICulture,
                CultureInfo.GetCultureInfo(1033),
                CultureInfo.GetCultureInfo(1049)
            };
            var formats = new[] {null, "D", "d", "f", "F", "G", "g", "M", "o", "R", "s", "t", "T", "u", "U", "Y"};
            var values = new[] {DateTime.MinValue, DateTime.Now, DateTime.MaxValue};
            foreach (var value in values)
            {
                foreach (var culture in cultures)
                {
                    foreach (var format in formats)
                    {
                        var result = value.Safe().To<string>(new ConversionSettings(culture, format));
                        result.IsSome
                            .Should().BeTrue("conversion result should be some");
                        value.ToString(format, culture).Equals(result, StringComparison.Ordinal)
                            .Should().BeTrue("conversion result contain expected value");
                    }
                }
            }
        }

        [Test]
        public void Convert_NumberStylesToInt32AndBack_ReturnsSome()
        {
            var values = new[]
            {
                NumberStyles.None,
                NumberStyles.AllowLeadingWhite,
                NumberStyles.AllowTrailingWhite,
                NumberStyles.AllowLeadingSign,
                NumberStyles.AllowTrailingSign,
                NumberStyles.AllowParentheses,
                NumberStyles.AllowDecimalPoint,
                NumberStyles.AllowThousands,
                NumberStyles.AllowExponent,
                NumberStyles.AllowCurrencySymbol,
                NumberStyles.AllowHexSpecifier,
                NumberStyles.Integer,
                NumberStyles.HexNumber,
                NumberStyles.Number,
                NumberStyles.Float,
                NumberStyles.Currency,
                NumberStyles.Any
            };
            foreach (var value in values)
            {
                var @int = value.Safe().To<int>();
                @int.IsSome
                    .Should().BeTrue("conversion result should be some");
                @int.Equals((int)value)
                    .Should().BeTrue("conversion result contain expected value");
                var styles = @int.To<NumberStyles>();
                styles.IsSome
                    .Should().BeTrue("conversion result should be some");
                styles.Equals(value)
                    .Should().BeTrue("conversion result contain expected value");
            }
        }

        [Test]
        public void Convert_NumberStylesToStringAndBack_ReturnsSome()
        {
            var values = new[]
            {
                NumberStyles.None,
                NumberStyles.AllowLeadingWhite,
                NumberStyles.AllowTrailingWhite,
                NumberStyles.AllowLeadingSign,
                NumberStyles.AllowTrailingSign,
                NumberStyles.AllowParentheses,
                NumberStyles.AllowDecimalPoint,
                NumberStyles.AllowThousands,
                NumberStyles.AllowExponent,
                NumberStyles.AllowCurrencySymbol,
                NumberStyles.AllowHexSpecifier,
                NumberStyles.Integer,
                NumberStyles.HexNumber,
                NumberStyles.Number,
                NumberStyles.Float,
                NumberStyles.Currency,
                NumberStyles.Any
            };
            foreach (var value in values)
            {
                var @string = value.Safe().To<string>();
                @string.IsSome
                    .Should().BeTrue("conversion result should be some");
                @string.Equals(value.ToString())
                    .Should().BeTrue("conversion result contain expected value");
                var styles = @string.To<NumberStyles>();
                styles.IsSome
                    .Should().BeTrue("conversion result should be some");
                styles.Equals(value)
                    .Should().BeTrue("conversion result contain expected value");
            }
        }

        [Test]
        public void Convert_Int32ToString_ReturnsSome()
        {
            var cultures = new[]
            {
                null,
                CultureInfo.InvariantCulture,
                CultureInfo.CurrentUICulture,
                CultureInfo.GetCultureInfo(1033),
                CultureInfo.GetCultureInfo(1049)
            };
            var formats = new[] { null, "F", "G", "N" };
            var values = new[] { int.MinValue, 0, int.MaxValue};
            foreach (var value in values)
            {
                foreach (var culture in cultures)
                {
                    foreach (var format in formats)
                    {
                        var result = value.Safe().To<string>(new ConversionSettings(culture, format));
                        result.IsSome
                            .Should().BeTrue("conversion result should be some");
                        value.ToString(format, culture).Equals(result, StringComparison.Ordinal)
                            .Should().BeTrue("conversion result contain expected value");
                    }
                }
            }
        }

        [Test]
        public void Convert_StringToInt32_ReturnsSome()
        {
            var values = new[] { " -1","0", "2 " };
            foreach (var value in values)
            {

                var result = value.Safe().To<int>(new ConversionSettings(NumberStyles.Integer));
                result.IsSome
                    .Should().BeTrue("conversion result should be some");
                int.Parse(value, NumberStyles.Integer)
                    .Should().Be(result, "conversion result contain expected value");
            }
        }

        [Test]
        public void Convert_StringToUri_ReturnsSome()
        {
            const string uri = "http://test.org/path.file.ext";
            var result = uri.Safe().To<Uri>();
            result.IsSome
                .Should().BeTrue("conversion result should be some");
            uri.Equals(result.Value.OriginalString, StringComparison.Ordinal)
                .Should().BeTrue("conversion result contain expected value");
        }

        [Test]
        public void Convert_VariousToInt32_ReturnsSome()
        {
            var values = new object[]
            {
                (byte) 0, (decimal) 0, (double) 0, 0, (long) 0, NumberStyles.None, (short) 0, 
                (sbyte) 0, (uint) 0, (ulong) 0, (ushort) 0, "0"
            };
            foreach (var value in values)
            {
                var result = value.Safe().To<int>();
                result.IsSome
                    .Should().BeTrue("conversion result should be some");
                result.Value
                    .Should().Be(0, "conversion result contain expected value");
            }
        }

        [Test]
        public void Convert_VariousToString_ReturnsSome()
        {
            var values = new object[]
            {
                false, byte.MaxValue, DateTime.MaxValue, decimal.MaxValue, double.MaxValue, Guid.NewGuid(), int.MaxValue,
                long.MaxValue, sbyte.MaxValue, short.MaxValue, uint.MaxValue, ulong.MaxValue, ushort.MaxValue
            };
            foreach (var value in values)
            {
                var result = value.Safe().To<string>();
                var @string = value.ToString();
                result.IsSome
                    .Should().BeTrue("conversion result should be some");
                @string.Equals(result.Value, StringComparison.Ordinal)
                    .Should().BeTrue("conversion result contain expected value");
            }
        }
    }
}