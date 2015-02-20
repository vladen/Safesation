using System;
using System.Diagnostics;

namespace Safesation
{
    using Static = Safe;

    public static class Extensions
    {
        #region METHODS

        public static Option<T> As<T>(IOption option)
        {
            if (!option.IsSome)
            {
                return new Option<T>(option.Exception);
            }
            try
            {
                return new Option<T>((T) option.Value);
            }
            catch (Exception exception)
            {
                return new Option<T>(exception);
            }
        }

        public static IOption Catch(this IOption option, Action<Exception> @catch)
        {
            if (@catch == null || option == null || !option.IsFail)
            {
                return option;
            }
            try
            {
                @catch(option.Exception);
                return new Option<object>();
            }
            catch (Exception exception)
            {
                return new Option<object>(option.Exception, exception);
            }
        }

        public static IOption Catch<TException>(this IOption option, Action<TException> @catch)
            where TException : Exception
        {
            if (@catch == null || option == null || !option.IsFail)
            {
                return option;
            }
            try
            {
                var exception = option.Exception as TException;
                if (exception != null)
                {
                    @catch(exception);
                    return new Option<object>();
                }
            }
            catch (Exception exception)
            {
                return new Option<object>(option.Exception, exception);
            }
            return option;
        }

        public static int CompareTo<T>(this Option<T> left, Option<T> right)
            where T : IComparable<T>
        {
            return left.IsSome
                ? right.IsSome
                    ? left.Value.CompareTo(right.Value)
                    : int.MinValue
                : right.IsSome
                    ? int.MaxValue
                    : 0;
        }

        public static bool Equals<T>(this Option<T> left, T right)
            where T : IEquatable<T>
        {
            return left.IsSome && left.Value.Equals(right);
        }

        public static object Otherwise(this IOption option, object otherwise)
        {
            if (option == null)
            {
                return otherwise;
            }
            return option.IsNone ? otherwise : option.Value;
        }

        public static Option<T> Safe<T>(this T value)
        {
            return new Option<T>(value);
        }

        public static Option<TTarget> ToSpecial<TTarget>(this IOption option, string category, ConversionSettings settings)
        {
            if (option == null)
            {
                return new Option<TTarget>();
            }
            if (option.IsSome)
            {
                var value = option.Value;
                var source = value.GetType();
                var target = typeof (TTarget);
                var conversion = new Conversion(category, settings.Features, source, target);
                var converter = Static.Converters.Resolve(conversion) as Converter<TTarget>;
                if (converter != null)
                {
                    Debug.WriteLine(
                        "Converting from '{0}.{1}' to '{2}.{3}' using untyped '{4}'.",
                        source.Namespace, source.Name, target.Namespace, target.Name, converter.DebuggerDisplay);
                    return converter.Convert(value, settings);
                }
                Debug.WriteLine(
                    "Converting from '{0}.{1}' to '{2}.{3}' using 'None'.",
                    source.Namespace, source.Name, target.Namespace, target.Name);
            }
            else if (option.IsFail)
            {
                return new Option<TTarget>(option.Exception);
            }
            return new Option<TTarget>();
        }

        public static IOption ToSpecial(this IOption option, Type target, string category, ConversionSettings settings)
        {
            if (option == null)
            {
                return Static.Option(target);
            }
            if (option.IsSome)
            {
                var value = option.Value;
                var source = value.GetType();
                var conversion = new Conversion(category, settings.Features, source, target);
                var converter = Static.Converters.Resolve(conversion);
                if (converter != null)
                {
                    Debug.WriteLine(
                        "Converting from '{0}.{1}' to '{2}.{3}' using untyped '{4}'.",
                        source.Namespace, source.Name, target.Namespace, target.Name, converter.DebuggerDisplay);
                    return converter.Convert(value, settings);
                }
                Debug.WriteLine(
                    "Converting from '{0}.{1}' to '{2}.{3}' using 'None'.",
                    source.Namespace, source.Name, target.Namespace, target.Name);
            }
            else if (option.IsFail)
            {
                return Static.Option(target, option.Exception);
            }
            return Static.Option(target);
        }

        #endregion
    }
}