using System;
using FluentAssertions;
using NUnit.Framework;

namespace Safesation.Tests.ConverterTests
{
    public class Resolve
    {
        #region TESTS

        [Test]
        public void Resolve_EnumToByte_ReturnsExpectedConverter()
        {
            // Byte = (Byte) NumberStyles
            Assert(ConversionFeatures.None, Types.NumberStyles, Types.Byte, ConversionWays.Assignment);
        }

        [Test]
        public void Resolve_EnumToInt32_ReturnsExpectedConverter()
        {
            // Int32 = (Int32) NumberStyles
            Assert(ConversionFeatures.None, Types.NumberStyles, Types.Int32, ConversionWays.Assignment);
        }

        [Test]
        public void Resolve_Int32ToEnum_ReturnsExpectedConverter()
        {
            // Object Enum.ToObject(Type, Int32)
            Assert(ConversionFeatures.None, Types.Int32, Types.NumberStyles, ConversionWays.StaticMethod);
        }

        [Test]
        public void Resolve_Int32ToStringWithCulture_ReturnsExpectedConverter()
        {
            // String Int32.ToString(IFormatProvider)
            Assert(ConversionFeatures.Culture, Types.Int32, Types.String, ConversionWays.InstanceMethod);
        }

        [Test]
        public void Resolve_Int32ToStringWithCultureAndFormat_ReturnsExpectedConverter()
        {
            // String Int32.ToString(String, IFormatProvider)
            Assert(ConversionFeatures.CultureFormat, Types.Int32, Types.String, ConversionWays.InstanceMethod);
        }

        [Test]
        public void Resolve_Int64ToDateTime_ReturnsExpectedConverter()
        {
            // DateTime DateTime(Int64)
            Assert(ConversionFeatures.None, Types.Int64, Types.DateTime, ConversionWays.Constructor);
        }

        [Test]
        public void Resolve_ObjectToString_ReturnsExpectedConverter()
        {
            // String Object.ToString()
            Assert(ConversionFeatures.None, Types.Object, Types.String, ConversionWays.InstanceMethod);
        }

        [Test]
        public void Resolve_ObjectToInt32_ReturnsNull()
        {
            Safe.Converters.Resolve(new Conversion(Types.Object, Types.Int32))
                .Should().BeNull("Object can not be converted to Int32 via standard conversion");
        }

        [Test]
        public void Resolve_StringToDateTimeWithCultureFormatAndDateTimeStyles_ReturnsExpectedConverter()
        {
            // Boolean DateTime.TryParseExact(string, string, IFormatProvider, DateTimeStyles, out DateTime)
            Assert(ConversionFeatures.CultureFormatStyles, Types.String, Types.DateTime, ConversionWays.StaticTryMethod);
        }

        [Test]
        public void Resolve_StringToInt32WithCulture_ReturnsExpectedConverter()
        {
            // Int32 Int32.Parse(IFormatProvider)
            Assert(ConversionFeatures.Culture, Types.String, Types.Int32, ConversionWays.StaticMethod);
        }

        [Test]
        public void Resolve_StringToInt32WithCultureAndStyles_ReturnsExpectedConverter()
        {
            // Int32 Int32.TryParse(string, NumberStyles, IFormatProvider, out Int32)
            Assert(ConversionFeatures.CultureStyles, Types.String, Types.Int32, ConversionWays.StaticTryMethod);
        }

        [Test]
        public void Resolve_StringToObject_ReturnsExpectedConverter()
        {
            // Object = String
            Assert(ConversionFeatures.None, Types.String, Types.Object, ConversionWays.Assignment);
        }

        #endregion

        #region METHODS

        private void Assert(ConversionFeatures features, Type sourceType, Type targeType, ConversionWays way)
        {
            var converter = Safe.Converters.Resolve(new Conversion(features, sourceType, targeType));
            converter
                .Should().NotBeNull("resolved converted should be not null");
            // ReSharper disable once PossibleNullReferenceException
            converter.Conversion.Features
                .Should().Be(features, "resolved converter should have expected features");
            converter.Conversion.Source.IsAssignableFrom(sourceType)
                .Should().BeTrue("resolved converter should have expected source type");
            converter.Conversion.Target
                .Should().Be(targeType, "resolved converter should have expected target type");
            converter.Way
                .Should().Be(way, "resolved converter should use expected way");
        }

        #endregion
    }
}