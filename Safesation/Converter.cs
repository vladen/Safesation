using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Text;

namespace Safesation
{
    using Annotations;

    [DebuggerDisplay("{DebuggerDisplay}")]
    public abstract partial class Converter
    {
        #region CONSTRUCTORS

        static Converter()
        {
            GetCulture = Types.ConversionSettings.GetProperty("Culture").GetGetMethod();
            GetDateTimeStyles = Types.ConversionSettings.GetProperty("DateTimeStyles").GetGetMethod();
            GetFormat = Types.ConversionSettings.GetProperty("Format").GetGetMethod();
            GetNumberStyles = Types.ConversionSettings.GetProperty("NumberStyles").GetGetMethod();
            GetTimeSpanStyles = Types.ConversionSettings.GetProperty("TimeSpanStyles").GetGetMethod();
            ValueArgument = 0;
            ConversionArgument = 1;
            GetTypeFromHandle = Types.Type.GetMethod("GetTypeFromHandle", new[] {Types.RuntimeTypeHandle});
            ConvertFrom = Types.TypeConverter.GetMethod("ConvertFrom",
                new[] {Types.TypeDescriptorContextInterface, Types.CultureInfo, Types.Object});
            ConvertTo = Types.TypeConverter.GetMethod("ConvertTo",
                new[] {Types.TypeDescriptorContextInterface, Types.CultureInfo, Types.Object, Types.Type});
            GetConverter = Types.TypeDescriptor.GetMethod("GetConverter", new[] {Types.Type});
        }

        protected Converter(Conversion conversion, ConversionWays way)
        {
            Conversion = conversion;
            Way = way;
        }

        #endregion

        #region FIELDS

        private static readonly byte ConversionArgument;

        private static readonly MethodInfo ConvertFrom;

        private static readonly MethodInfo ConvertTo;

        private static readonly MethodInfo GetConverter;

        private static readonly MethodInfo GetCulture;

        private static readonly MethodInfo GetDateTimeStyles;

        private static readonly MethodInfo GetFormat;

        private static readonly MethodInfo GetNumberStyles;

        private static readonly MethodInfo GetTimeSpanStyles;

        private static readonly MethodInfo GetTypeFromHandle;

        private static readonly byte ValueArgument;

        #endregion

        #region PROPERTIES

        public Conversion Conversion { get; private set; }

        internal string DebuggerDisplay
        {
            get
            {
                var builder = new StringBuilder("Converter from '")
                    .Append(Conversion.Source.Namespace).Append(".").Append(Conversion.Source.Name).Append("' to '")
                    .Append(Conversion.Target.Namespace).Append(".").Append(Conversion.Target.Name);
                if (Conversion.Category != null)
                {
                    builder.Append(", ").Append(Conversion.Category);
                }
                builder.Append("' via '").Append(Way);
                if (Conversion.Features != ConversionFeatures.None)
                {
                    builder.Append("' featured with '").Append(Conversion.Features);
                }
                return builder.Append("'").ToString();
            }
        }

        public ConversionWays Way { get; private set; }

        #endregion

        #region METHODS

        private static Converter Construct(Type converterType, Type sourceType, Type targetType)
        {
            return converterType
                .MakeGenericType(sourceType, targetType)
                .GetConstructors()[0]
                .Invoke(new object[0]) as Converter;
        }

        private static Converter Construct(Type converterType, Type sourceType, Type targetType, ConstructorInfo constructor)
        {
            return converterType
                .MakeGenericType(sourceType, targetType)
                .GetConstructors()[0]
                .Invoke(new object[] {constructor}) as Converter;
        }

        private static Converter Construct(Type converterType, Type sourceType, Type targetType, MethodInfo method)
        {
            return converterType
                .MakeGenericType(sourceType, targetType)
                .GetConstructors()[0]
                .Invoke(new object[] {method}) as Converter;
        }

        public abstract IOption Convert<T>(T value, ConversionSettings settings);

        [CanBeNull]
        public static Converter Create(ConstructorInfo constructor)
        {
            if (constructor == null || constructor.IsGenericMethodDefinition)
            {
                return null;
            }
            var targetType = constructor.DeclaringType;
            if (!IsValidGenericTypeParameter(targetType))
            {
                return null;
            }
            var parameters = constructor.GetParameters();
            if (parameters.Length == 0)
            {
                return null;
            }
            var sourceType = parameters[0].ParameterType;
            if (!IsValidGenericTypeParameter(sourceType))
            {
                return null;
            }
            if (parameters.Length == 1)
            {
                // TTarget .ctor(TSource)
                return Construct(
                    typeof (Constructor<,>), sourceType, targetType, constructor);
            }
            if (parameters.Length == 2 && parameters[1].ParameterType.IsAssignableFrom(Types.CultureInfo))
            {
                // TTarget .ctor(TSource, CultureInfo)
                return Construct(
                    typeof(ConstructorWithCulture<,>), sourceType, targetType, constructor);
            }
            return null;
        }

        [CanBeNull]
        public static Converter Create<TSource, TTarget>(Converter<TSource, TTarget>.Delegate @delegate, ConversionFeatures features)
        {
            return @delegate == null ? null : new Custom<TSource, TTarget>(@delegate, features);
        }

        [CanBeNull]
        public static Converter Create(MethodInfo method)
        {
            if (method == null
                || method.IsGenericMethodDefinition
                || method.ReturnType == Types.Void)
            {
                return null;
            }
            if (method.IsStatic)
            {
                return CreateFromStaticMethod(method);
            }
            return CreateFromInstanceMethod(method);
        }

        [CanBeNull]
        public static Converter Create(Type sourceType, Type targetType)
        {
            if (sourceType == null || targetType == null)
            {
                return null;
            }
            if (targetType.IsAssignableFrom(sourceType)
                || (sourceType.IsEnum && Types.IsEnumBase(targetType)))
            {
                return Construct(
                    typeof(Assignment<,>), sourceType, targetType);
            }
            if (targetType.IsEnum)
            {
                if (Types.IsEnumBase(sourceType))
                {
                    return Construct(
                        typeof(EnumToObjectMethod<,>), sourceType, targetType);
                }
                if (Types.String == sourceType)
                {
                    return Construct(
                        typeof(EnumParseMethod<,>), sourceType, targetType);
                }
            }
            // todo: special case for nullable types
            if (TypeDescriptor.GetConverter(targetType).CanConvertFrom(sourceType))
            {
                return Construct(
                    typeof(TypeConvertorFromMethod<,>), sourceType, targetType);
            }
            if (TypeDescriptor.GetConverter(sourceType).CanConvertTo(targetType))
            {
                return Construct(
                    typeof(TypeConvertorToMethod<,>), sourceType, targetType);
            }
            return null;
        }

        [CanBeNull]
        private static Converter CreateFromInstanceMethod(MethodInfo method)
        {
            var name = method.Name;
            var dot = name.LastIndexOf(".", StringComparison.OrdinalIgnoreCase) + 1;
            if (dot > 1 && dot < name.Length)
            {
                name = name.Substring(dot);
            }
            if (name.StartsWith("To", StringComparison.Ordinal))
            {
                var sourceType = method.DeclaringType;
                if (!IsValidGenericTypeParameter(sourceType))
                {
                    return null;
                }
                var targetType = method.ReturnType;
                if (sourceType == targetType || !IsValidGenericTypeParameter(targetType))
                {
                    return null;
                }
                var parameters = method.GetParameters();
                if (parameters.Length == 0)
                {
                    // TTarget TSource.To...()
                    return Construct(
                        typeof (InstanceMethod<,>), sourceType, targetType, method);
                }
                if (parameters.Length == 1)
                {
                    if (parameters[0].ParameterType.IsAssignableFrom(Types.CultureInfo))
                    {
                        // TTarget TSource.To...(CultureInfo)
                        return Construct(
                            typeof (InstanceMethodWithCulture<,>), sourceType, targetType, method);
                    }
                    if (parameters[0].ParameterType.IsAssignableFrom(Types.String))
                    {
                        // TTarget TSource.To...(string)
                        return Construct(
                            typeof(InstanceMethodWithFormat<,>), sourceType, targetType, method);
                    }
                }
                if (parameters.Length == 2)
                {
                    if (parameters[0].ParameterType.IsAssignableFrom(Types.String)
                        && parameters[1].ParameterType.IsAssignableFrom(Types.CultureInfo))
                    {
                        // TTarget TSource.To...(string, CultureInfo)
                        return Construct(
                            typeof(InstanceMethodWithFormatAndCulture<,>), sourceType, targetType, method);
                    }
                }
            }
            return null;
        }

        [CanBeNull]
        private static Converter CreateFromStaticMethod(MethodInfo method)
        {
            var parameters = method.GetParameters();
            if (parameters.Length == 0)
            {
                return null;
            }
            var sourceType = parameters[0].ParameterType;
            if (!IsValidGenericTypeParameter(sourceType))
            {
                return null;
            }
            Type targetType;
            if (method.Name.Equals("TryParse", StringComparison.Ordinal)
                || method.Name.Equals("TryParseExact", StringComparison.Ordinal))
            {
                var lastParameter = parameters[parameters.Length - 1];
                if (method.ReturnType != Types.Boolean || !lastParameter.IsOut)
                {
                    return null;
                }
                targetType = lastParameter.ParameterType.GetElementType();
                if (!IsValidGenericTypeParameter(targetType))
                {
                    return null;
                }
                if (parameters.Length == 2)
                {
                    // bool TryParse(TSource, out TTraget)
                    return Construct(
                        typeof(StaticTryMethod<,>), sourceType, targetType, method);
                }
                if (parameters.Length == 3)
                {
                    if (parameters[1].ParameterType.IsAssignableFrom(Types.CultureInfo))
                    {
                        // bool TryParse(TSource, IFormatProvider, out TTraget)
                        return Construct(
                            typeof(StaticTryMethodWithCulture<,>), sourceType, targetType, method);
                    }
                    return null;
                }
                if (parameters.Length == 4)
                {
                    if (parameters[1].ParameterType.IsAssignableFrom(Types.CultureInfo)
                        && parameters[2].ParameterType.IsAssignableFrom(Types.DateTimeStyles))
                    {
                        // bool TryParse(TSource, IFormatProvider, DateTimeStyles, out TTraget)
                        return Construct(
                            typeof (StaticTryMethodWithCultureAndDateTimeStyles<,>), sourceType, targetType, method);
                    }
                    if (parameters[1].ParameterType.IsAssignableFrom(Types.NumberStyles)
                        && parameters[2].ParameterType.IsAssignableFrom(Types.CultureInfo))
                    {
                        // bool TryParse(TSource, NumberStyles, IFormatProvider, out TTraget)
                        return Construct(
                            typeof(StaticTryMethodWithNumberStylesAndCulture<,>), sourceType, targetType, method);
                    }
                    if (parameters[1].ParameterType.IsAssignableFrom(Types.String)
                        && parameters[2].ParameterType.IsAssignableFrom(Types.CultureInfo))
                    {
                        // bool TryParse(TSource, string, IFormatProvider, out TTraget)
                        return Construct(
                            typeof(StaticTryMethodWithFormatAndCulture<,>), sourceType, targetType, method);
                    }
                    return null;
                }
                if (parameters.Length == 5)
                {
                    if (parameters[1].ParameterType.IsAssignableFrom(Types.String)
                        && parameters[2].ParameterType.IsAssignableFrom(Types.CultureInfo)
                        && parameters[3].ParameterType.IsAssignableFrom(Types.DateTimeStyles))
                    {
                        // bool TryParseExact(TSource, string, IFormatProvider, DateTimeStyles, out TTraget)
                        return Construct(
                            typeof(StaticTryMethodWithFormatCultureAndDateTimeStyles<,>), sourceType, targetType, method);
                    }
                    if (parameters[1].ParameterType.IsAssignableFrom(Types.String)
                        && parameters[2].ParameterType.IsAssignableFrom(Types.CultureInfo)
                        && parameters[3].ParameterType.IsAssignableFrom(Types.TimeSpanStyles))
                    {
                        // bool TryParseExact(TSource, string, IFormatProvider, DateTimeStyles, out TTraget)
                        return Construct(
                            typeof(StaticTryMethodWithFormatCultureAndTimeSpanStyles<,>), sourceType, targetType, method);}
                }
                return null;
            }
            targetType = method.ReturnType;
            if (!IsValidGenericTypeParameter(targetType))
            {
                return null;
            }
            if (method.Name.Equals("op_Implicit", StringComparison.Ordinal))
            {
                // implicit operator TTarget(TSource)
                return Construct(
                    typeof (Operator<,>), sourceType, targetType, method);
            }
            if (method.Name.Equals("op_Explicit", StringComparison.Ordinal))
            {
                // explicit operator TTarget(TSource)
                return Construct(
                    typeof(Operator<,>), sourceType, targetType, method);
            }
            if (method.Name.Equals("Parse", StringComparison.Ordinal) 
                || method.Name.StartsWith("From", StringComparison.Ordinal) 
                || method.Name.StartsWith("To", StringComparison.Ordinal))
            {
                if (parameters.Length == 1)
                {
                    // TTarget Parse(TSource)
                    return Construct(
                        typeof (StaticMethod<,>), sourceType, targetType, method);
                }
                if (parameters.Length == 2)
                {
                    if (parameters[1].ParameterType.IsAssignableFrom(Types.CultureInfo))
                    {
                        // TTarget Parse(TSource, IFormatProvider)
                        return Construct(
                            typeof (StaticMethodWithCulture<,>), sourceType, targetType, method);
                    }
                    if (parameters[1].ParameterType.IsAssignableFrom(Types.NumberStyles))
                    {
                        // TTarget Parse(TSource, NumberStyles)
                        return Construct(
                            typeof(StaticMethodWithNumberStyles<,>), sourceType, targetType, method);
                    }
                }
            }
            return null;
        }

        internal bool IsBetterThan(Converter converter)
        {
            return Way < converter.Way;
        }

        [DebuggerStepThrough]
        private static bool IsValidGenericTypeParameter(Type type)
        {
            return !type.IsPointer && !type.IsByRef && type != Types.Void;
        }

        public static Converter None(ConversionFeatures features, Type sourceType, Type targetType)
        {
            return typeof (Custom<,>)
                .MakeGenericType(sourceType, targetType)
                .GetConstructors()[0]
                .Invoke(new object[] {features}) as Converter;
        }

        #endregion
    }
}