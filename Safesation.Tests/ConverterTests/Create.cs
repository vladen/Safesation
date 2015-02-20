using System;
using System.ComponentModel;
using System.Globalization;
using FluentAssertions;
using NUnit.Framework;

namespace Safesation.Tests.ConverterTests
{
    using Annotations;

    public class Create
    {
        #region TESTS

        [Test]
        public void Create_FailToString_ViaImplicitOperator_ReturnsValidConversion()
        {
            AssertFailToString(
                Converter.Create(
                    FailType.GetMethod("op_Implicit", new[] { FailType })));
        }

        [Test]
        public void Create_FailToString_ViaToStringMethod_ReturnsValidConversion()
        {
            AssertFailToString(
                Converter.Create(
                    FailType.GetMethod("ToString", Type.EmptyTypes)));
        }

        [Test]
        public void Create_FailToString_ViaToStringMethodWithCulture_ReturnsValidConversion()
        {
            AssertFailToString(
                Converter.Create(
                    FailType.GetMethod("ToString", new[] { Types.CultureInfo })));
        }

        [Test]
        public void Create_FailToString_ViaTypeConverterToMethod_ReturnsValidConversion()
        {
            AssertFailToString(
                Converter.Create(FailType, Types.String));
        }

        [Test]
        public void Create_FakeToFake_ViaAssignment_ReturnsValidConversion()
        {
            AssertFakeToFake(
                Converter.Create(FakeType, FakeType));
        }

        [Test]
        public void Create_FakeToString_ViaImplicitOperator_ReturnsValidConversion()
        {
            AssertFakeToString(
                Converter.Create(
                    FakeType.GetMethod("op_Implicit", new[] { FakeType })),
                Invocation.ImplicitOperator);
        }

        [Test]
        public void Create_FakeToString_ViaToStringMethod_ReturnsValidConversion()
        {
            AssertFakeToString(
                Converter.Create(
                    FakeType.GetMethod("ToString", Type.EmptyTypes)),
                Invocation.ToStringMethod);
        }

        [Test]
        public void Create_FakeToString_ViaToStringMethodWithCulture_ReturnsValidConversion()
        {
            AssertFakeToString(
                Converter.Create(
                    FakeType.GetMethod("ToString", new[] { Types.CultureInfo })),
                Invocation.ToStringMethodWithCulture);
        }

        [Test]
        public void Create_FakeToString_ViaToStringMethodWithFormatAndCulture_ReturnsValidConversion()
        {
            AssertFakeToString(
                Converter.Create(
                    FakeType.GetMethod("ToString", new[] { Types.String, Types.CultureInfo })),
                Invocation.ToStringMethodWithFormatAndCulture);
        }

        [Test]
        public void Create_FakeToString_ViaTypeConverterToMethod_ReturnsValidConversion()
        {
            AssertFakeToString(
                Converter.Create(FakeType, Types.String),
                Invocation.ConverterToMethod);
        }

        [Test]
        public void Create_StringToFail_ViaConstructor_ReturnsValidConversion()
        {
            AssertStringToFail(
                Converter.Create(
                    FailType.GetConstructor(new[] { Types.String })));
        }

        [Test]
        public void Create_StringToFail_ViaConstructorWithCulture_ReturnsValidConversion()
        {
            AssertStringToFail(
                Converter.Create(
                    FailType.GetConstructor(new[] { Types.String, Types.CultureInfo })));
        }

        [Test]
        public void Create_StringToFail_ViaExplicitOperator_ReturnsValidConversion()
        {
            AssertStringToFail(
                Converter.Create(
                    FailType.GetMethod("op_Explicit", new[] { Types.String })));
        }

        [Test]
        public void Create_StringToFail_ViaParseMethod_ReturnsValidConversion()
        {
            AssertStringToFail(
                Converter.Create(
                    FailType.GetMethod("Parse", new[] { Types.String })));
        }

        [Test]
        public void Create_StringToFail_ViaParseMethodWithCulture_ReturnsValidConversion()
        {
            AssertStringToFail(
                Converter.Create(
                    FailType.GetMethod("Parse", new[] { Types.String, Types.CultureInfo })));
        }

        [Test]
        public void Create_StringToFail_ViaTryParseMethod_ReturnsValidConversion()
        {
            AssertStringToFail(
                Converter.Create(
                    FailType.GetMethod("TryParse", new[] { Types.String, FailType.MakeByRefType() })));
        }

        [Test]
        public void Create_StringToFail_ViaTryParseMethodWithCulture_ReturnsValidConversion()
        {
            AssertStringToFail(
                Converter.Create(
                    FailType.GetMethod("TryParse", new[] { Types.String, Types.CultureInfo, FailType.MakeByRefType() })));
        }

        [Test]
        public void Create_StringToFail_ViaTypeConverterFromMethod_ReturnsValidConversion()
        {
            AssertStringToFail(
                Converter.Create(Types.String, FailType));
        }

        [Test]
        public void Create_StringToFake_ViaConstructor_ReturnsValidConversion()
        {
            AssertStringToFake(
                Converter.Create(
                    FakeType.GetConstructor(new[] { Types.String })),
                Invocation.Constructor);
        }

        [Test]
        public void Create_StringToFake_ViaConstructorWithCulture_ReturnsValidConversion()
        {
            AssertStringToFake(
                Converter.Create(
                    FakeType.GetConstructor(new[] { Types.String, Types.CultureInfo })),
                Invocation.ConstructorWithCulture);
        }

        [Test]
        public void Create_StringToFake_ViaExplicitOperator_ReturnsValidConversion()
        {
            AssertStringToFake(
                Converter.Create(
                    FakeType.GetMethod("op_Explicit", new[] { Types.String })),
                Invocation.ExplicitOperator);
        }

        [Test]
        public void Create_StringToFake_ViaParseMethod_ReturnsValidConversion()
        {
            AssertStringToFake(
                Converter.Create(
                    FakeType.GetMethod("Parse", new[] { Types.String })),
                Invocation.ParseMethod);
        }

        [Test]
        public void Create_StringToFake_ViaParseMethodWithCulture_ReturnsValidConversion()
        {
            AssertStringToFake(
                Converter.Create(
                    FakeType.GetMethod("Parse", new[] { Types.String, Types.CultureInfo })),
                Invocation.ParseMethodWithCulture);
        }

        [Test]
        public void Create_StringToFake_ViaParseMethodWithNumberStyles_ReturnsValidConversion()
        {
            AssertStringToFake(
                Converter.Create(
                    FakeType.GetMethod("Parse", new[] { Types.String, Types.NumberStyles })),
                Invocation.ParseMethodWithNumberStyles);
        }

        [Test]
        public void Create_StringToFake_ViaTryParseExactMethodWithFormatAndCulture_ReturnsValidConversion()
        {
            AssertStringToFake(
                Converter.Create(
                    FakeType.GetMethod("TryParseExact", new[] { Types.String, Types.String, Types.CultureInfo, FakeType.MakeByRefType() })),
                Invocation.TryParseExactMethodWithFormatAndCulture);
        }

        [Test]
        public void Create_StringToFake_ViaTryParseExactMethodWithFormatCultureAndDateTimeStyles_ReturnsValidConversion()
        {
            AssertStringToFake(
                Converter.Create(
                    FakeType.GetMethod("TryParseExact", new[] { Types.String, Types.String, Types.CultureInfo, Types.DateTimeStyles, FakeType.MakeByRefType() })),
                Invocation.TryParseExactMethodWithFormatCultureAndDateTimeStyles);
        }

        [Test]
        public void Create_StringToFake_ViaTryParseExactMethodWithFormatCultureAndTimeSpanStyles_ReturnsValidConversion()
        {
            AssertStringToFake(
                Converter.Create(
                    FakeType.GetMethod("TryParseExact", new[] { Types.String, Types.String, Types.CultureInfo, Types.TimeSpanStyles, FakeType.MakeByRefType() })),
                Invocation.TryParseExactMethodWithFormatCultureAndTimeSpanStyles);
        }

        [Test]
        public void Create_StringToFake_ViaTryParseMethod_ReturnsValidConversion()
        {
            AssertStringToFake(
                Converter.Create(
                    FakeType.GetMethod("TryParse", new[] { Types.String, FakeType.MakeByRefType() })),
                Invocation.TryParseMethod);
        }

        [Test]
        public void Create_StringToFake_ViaTryParseMethodWithCulture_ReturnsValidConversion()
        {
            AssertStringToFake(
                Converter.Create(
                    FakeType.GetMethod("TryParse", new[] { Types.String, Types.CultureInfo, FakeType.MakeByRefType() })),
                Invocation.TryParseMethodWithCulture);
        }

        [Test]
        public void Create_StringToFake_ViaTryParseMethodWithCultureAndDateTimeStyles_ReturnsValidConversion()
        {
            AssertStringToFake(
                Converter.Create(
                    FakeType.GetMethod("TryParse", new[] { Types.String, Types.CultureInfo, Types.DateTimeStyles, FakeType.MakeByRefType() })),
                Invocation.TryParseExactMethodWithCultureAndDateTimeStyles);
        }

        [Test]
        public void Create_StringToFake_ViaTryParseMethodWithNumberStylesAndCulture_ReturnsValidConversion()
        {
            AssertStringToFake(
                Converter.Create(
                    FakeType.GetMethod("TryParse", new[] { Types.String, Types.NumberStyles, Types.CultureInfo, FakeType.MakeByRefType() })),
                Invocation.TryParseMethodWithNumberStylesAndCulture);
        }

        [Test]
        public void Create_StringToFake_ViaTypeConverterFrom_ReturnsValidConversion()
        {
            AssertStringToFake(
                Converter.Create(Types.String, FakeType),
                Invocation.ConverterFromMethod);
        }

        #endregion

        #region ENUMS

        public enum Invocation
        {
            None,
            Constructor,
            ConstructorWithCulture,
            ConverterFromMethod,
            ConverterToMethod,
            ExplicitOperator,
            ImplicitOperator,
            ParseMethod,
            ParseMethodWithCulture,
            ParseMethodWithNumberStyles,
            ParseMethodWithNumberStylesAndCulture,
            ToStringMethod,
            ToStringMethodWithCulture,
            ToStringMethodWithFormatAndCulture,
            TryParseExactMethodWithCultureAndDateTimeStyles,
            TryParseExactMethodWithFormatAndCulture,
            TryParseExactMethodWithFormatCultureAndDateTimeStyles,
            TryParseExactMethodWithFormatCultureAndTimeSpanStyles,
            TryParseMethod,
            TryParseMethodWithCulture,
            TryParseMethodWithNumberStylesAndCulture
        }

        #endregion

        #region FIELDS

        private static readonly Type FailType = typeof(Fail);

        private static readonly Type FakeType = typeof(Fake);

        #endregion

        #region METHODS

        private static void AssertFailToString(Converter candidate)
        {
            var converter = candidate as Converter<Fail, string>;
            converter
                .Should().NotBeNull("converter should be assignable to 'Converter<Fail, String>'");
            // ReSharper disable once PossibleNullReferenceException
            converter.Conversion.Source
                .Should().Be(FailType, "source type should be 'Fail'");
            converter.Conversion.Target
                .Should().Be(Types.String, "target type should be 'String'");
            var source = new Fail();
            var result = converter.Convert(source, new ConversionSettings());
            result.IsFail
                .Should().BeTrue("result option should be 'Fail'");
            result.Exception
                .Should().BeOfType<FailException>("result option should contain expected exception");
        }

        private static void AssertFakeToFake(Converter candidate)
        {
            var converter = candidate as Converter<Fake, Fake>;
            converter
                .Should().NotBeNull("converter should be assignable to 'Converter<Fake, Fake>'");
            // ReSharper disable once PossibleNullReferenceException
            converter.Conversion.Source
                .Should().Be(FakeType, "source type should be 'Fake'");
            converter.Conversion.Target
                .Should().Be(FakeType, "target type should be 'Fake'");
            var source = new Fake("test");
            var result = converter.Convert(source, new ConversionSettings());
            result.IsSome
                .Should().BeTrue("result option should be 'Some'");
            source.Equals(result.Value)
                .Should().BeTrue("result option should contain expected value");
        }

        private static void AssertFakeToString(Converter candidate, Invocation invocation)
        {
            var converter = candidate as Converter<Fake, string>;
            converter
                .Should().NotBeNull("converter should be assignable to 'Converter<Fake, String>'");
            // ReSharper disable once PossibleNullReferenceException
            converter.Conversion.Source
                .Should().Be(FakeType, "source type should be 'Fake'");
            converter.Conversion.Target
                .Should().Be(Types.String, "target type should be 'String'");
            var source = new Fake("test");
            var result = converter.Convert(source, new ConversionSettings());
            result.IsSome
                .Should().BeTrue("result option should be 'Some'");
            source.Invocation
                .Should().Be(invocation, "expected invocation should be performed");
            source.Equals(result.Value)
                .Should().BeTrue("result option should contain expected value");
        }

        private static void AssertStringToFail(Converter candidate)
        {
            var converter = candidate as Converter<string, Fail>;
            converter
                .Should().NotBeNull("converter should be assignable to 'Converter<String, Fail>'");
            // ReSharper disable once PossibleNullReferenceException
            converter.Conversion.Source
                .Should().Be(Types.String, "source type should be 'String'");
            converter.Conversion.Target
                .Should().Be(FailType, "target type should be 'Fail'");
            const string source = "test";
            var result = converter.Convert(source, new ConversionSettings());
            result.IsFail
                .Should().BeTrue("result option should be 'Fail'");
            result.Exception
                .Should().BeOfType<FailException>("result option should contain expected exception");
        }

        private static void AssertStringToFake(Converter candidate, Invocation invocation)
        {
            var converter = candidate as Converter<string, Fake>;
            converter
                .Should().NotBeNull("converter should be assignable to 'Converter<String, Fake>'");
            // ReSharper disable once PossibleNullReferenceException
            converter.Conversion.Source
                .Should().Be(Types.String, "source type should be 'String'");
            converter.Conversion.Target
                .Should().Be(FakeType, "target type should be Fake");
            const string source = "test";
            var result = converter.Convert(source, new ConversionSettings());
            result.IsSome
                .Should().BeTrue("result option should be 'Some'");
            result.Value.Invocation
                .Should().Be(invocation, "expected invocation should be performed");
            result.Value.Equals(source)
                .Should().BeTrue("result option should contain expected value");
        }

        #endregion

        #region CLASSES

        [TypeConverter(typeof(FailConverter))]
        public struct Fail
            : IEquatable<Fail>
        {
            [UsedImplicitly]
            public Fail(string value)
            {
                _value = value;
                throw new FailException();
            }

            public Fail(string value, CultureInfo culture)
            {
                _value = value;
                throw new FailException();
            }

            private readonly string _value;

            [UsedImplicitly]
            public string Value
            {
                get { return _value; }
            }

            public bool Equals(Fail fail)
            {
                return string.Equals(_value, fail._value);
            }

            [UsedImplicitly]
            public bool Equals(string value)
            {
                return string.Equals(_value, value);
            }

            [UsedImplicitly]
            public static Fail Parse(string value)
            {
                throw new FailException();
            }

            [UsedImplicitly]
            public static Fail Parse(string value, CultureInfo culture)
            {
                throw new FailException();
            }

            [UsedImplicitly]
            public static bool TryParse(string value, out Fail result)
            {
                throw new FailException();
            }

            [UsedImplicitly]
            public static bool TryParse(string value, CultureInfo culture, out Fail result)
            {
                throw new FailException();
            }

            public override string ToString()
            {
                throw new FailException();
            }

            public string ToString(CultureInfo culture)
            {
                throw new FailException();
            }

            public static implicit operator string(Fail fail)
            {
                throw new FailException();
            }

            public static explicit operator Fail(string value)
            {
                throw new FailException();
            }
        }

        public class FailConverter
            : TypeConverter
        {
            public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
            {
                return sourceType == typeof(string);
            }

            public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
            {
                return destinationType == typeof(string);
            }

            public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
            {
                return new Fail((string)value, culture);
            }

            public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
            {
                return ((Fail)value).ToString(culture);
            }
        }

        public class FailException
            : ApplicationException
        {
            public FailException()
                : base("Fail")
            {
            }
        }

        [TypeConverter(typeof(FakeConverter))]
        public class Fake
            : IEquatable<Fake>
        {
            public Fake(string value)
            {
                Value = value;
                Invocation = Invocation.Constructor;
            }

            public Fake(string value, CultureInfo culture)
            {
                Value = value.ToString(culture);
                Invocation = Invocation.ConstructorWithCulture;
            }

            public string Value { get; private set; }

            public Invocation Invocation { get; set; }

            public bool Equals(Fake some)
            {
                return some != null && string.Equals(Value, some.Value);
            }

            public bool Equals(string value)
            {
                return string.Equals(Value, value);
            }

            [UsedImplicitly]
            public static Fake Parse(string value)
            {
                return new Fake(value) { Invocation = Invocation.ParseMethod };
            }

            [UsedImplicitly]
            public static Fake Parse(string value, CultureInfo culture)
            {
                return new Fake(value, culture) { Invocation = Invocation.ParseMethodWithCulture };
            }

            [UsedImplicitly]
            public static Fake Parse(string value, NumberStyles styles)
            {
                return new Fake(value) { Invocation = Invocation.ParseMethodWithNumberStyles };
            }
            
            public override string ToString()
            {
                Invocation = Invocation.ToStringMethod;
                return Value;
            }

            [UsedImplicitly]
            public string ToString(CultureInfo culture)
            {
                Invocation = Invocation.ToStringMethodWithCulture;
                return Value.ToString(culture);
            }

            [UsedImplicitly]
            public string ToString(string format, CultureInfo culture)
            {
                Invocation = Invocation.ToStringMethodWithFormatAndCulture;
                return Value.ToString(culture);
            }

            [UsedImplicitly]
            public static bool TryParse(string value, out Fake result)
            {
                result = new Fake(value) { Invocation = Invocation.TryParseMethod };
                return true;
            }

            [UsedImplicitly]
            public static bool TryParse(string value, CultureInfo culture, out Fake result)
            {
                result = new Fake(value, culture) { Invocation = Invocation.TryParseMethodWithCulture };
                return true;
            }

            [UsedImplicitly]
            public static bool TryParse(string value, CultureInfo culture, DateTimeStyles styles, out Fake result)
            {
                result = new Fake(value, culture) { Invocation = Invocation.TryParseExactMethodWithCultureAndDateTimeStyles };
                return true;
            }

            [UsedImplicitly]
            public static bool TryParse(string value, NumberStyles styles, CultureInfo culture, out Fake result)
            {
                result = new Fake(value, culture) { Invocation = Invocation.TryParseMethodWithNumberStylesAndCulture };
                return true;
            }

            [UsedImplicitly]
            public static bool TryParseExact(string value, string format, CultureInfo culture, out Fake result)
            {
                result = new Fake(value, culture) { Invocation = Invocation.TryParseExactMethodWithFormatAndCulture };
                return true;
            }

            [UsedImplicitly]
            public static bool TryParseExact(string value, string format, CultureInfo culture, DateTimeStyles styles, out Fake result)
            {
                result = new Fake(value, culture) { Invocation = Invocation.TryParseExactMethodWithFormatCultureAndDateTimeStyles };
                return true;
            }

            [UsedImplicitly]
            public static bool TryParseExact(string value, string format, CultureInfo culture, TimeSpanStyles styles, out Fake result)
            {
                result = new Fake(value, culture) { Invocation = Invocation.TryParseExactMethodWithFormatCultureAndTimeSpanStyles };
                return true;
            }

            public static implicit operator string(Fake some)
            {
                some.Invocation = Invocation.ImplicitOperator;
                return some.Value;
            }

            public static explicit operator Fake(string value)
            {
                return new Fake(value) { Invocation = Invocation.ExplicitOperator };
            }
        }

        public class FakeConverter
            : TypeConverter
        {
            public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
            {
                return sourceType == typeof(string);
            }

            public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
            {
                return destinationType == typeof(string);
            }

            public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
            {
                return new Fake((string)value, culture) { Invocation = Invocation.ConverterFromMethod };
            }

            public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
            {
                var fake = (Fake)value;
                fake.Invocation = Invocation.ConverterToMethod;
                return fake.Value;
            }
        }

        #endregion
    }
}