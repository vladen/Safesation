using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;

namespace Safesation
{
    using Annotations;

    [DebuggerDisplay("{DebuggerDisplay}")]
    public struct Option<T>
        : IEquatable<Option<T>>, IOption
    {
        #region CONSTRUCTORS

        public Option(Exception exception)
        {
            _exception = exception;
            _type = exception == null ? None : Fail;
            _value = default(T);
        }

        public Option(params Exception[] exceptions)
        {
            _exception = null;
            _value = default(T);
            if (exceptions.Length == 1)
            {
                _exception = exceptions[0];
            }
            else
            {
                var list = new List<Exception>(exceptions.Length);
                for (var i = 0; i < exceptions.Length; i++)
                {
                    var exception = exceptions[i];
                    if (exception != null)
                    {
                        var aggregate = exception as AggregateException;
                        if (aggregate == null)
                        {
                            list.Add(exception);
                        }
                        else list.AddRange(aggregate.InnerExceptions);
                    }
                }
                if (list.Count > 0)
                {
                    _exception = new AggregateException(exceptions);
                }
            }
            _type = _exception == null ? Fail : None;
        }

        public Option(T value)
        {
            var type = typeof(T);
            if (type.IsValueType)
            {
                _exception = null;
                _type = Types.IsNullable(type) && Equals(value, null) ? None : Some;
            }
            else
            {
                _exception = value as Exception;
                if (_exception == null)
                {
                    _type = Equals(value, null) || Equals(value, DBNull.Value) ? None : Some;
                }
                else
                {
                    _type = Fail;
                }
            }
            _value = value;
        }

        #endregion

        #region CONSTANTS

        private const int Fail = 1;

        private const int None = 0;

        private const int Some = 2;

        #endregion

        #region FIELDS

        private readonly Exception _exception;

        private readonly int _type;

        private readonly T _value;

        #endregion

        #region PROPETIES

        [UsedImplicitly]
        internal string DebuggerDisplay
        {
            get
            {
                var type = typeof (T);
                switch (_type)
                {
                    case Some:
                        return string.Format(
                            "Some '{0}.{1}': {2}", 
                            type.Namespace, type.Name, _value);
                    case Fail:
                        return string.Format(
                            "Fail '{0}.{1}': {2}",
                            type.Namespace, type.Name, _exception.Message);
                    default:
                        return string.Format(
                            "None '{0}.{1}'",
                            type.Namespace, type.Name);
                }
            }
        }

        public Exception Exception
        {
            get { return _exception; }
        }

        public bool IsFail
        {
            get { return _type == Fail; }
        }

        public bool IsNone
        {
            get { return _type == None; }
        }

        public bool IsSome
        {
            get { return _type == Some; }
        }

        public T Value
        {
            get
            {
                if (_type == Some)
                {
                    return _value;
                }
                if (_type == Fail)
                {
                    throw new InvalidOperationException("This instance contains exception.", _exception);
                }
                throw new InvalidOperationException("This instance does not contain value.");
            }
        }

        object IOption.Value
        {
            get { return Value; }
        }

        #endregion

        #region METHODS

        [Pure]
        public Option<T> Catch(Action<Exception> @catch)
        {
            if (@catch == null || _type != Fail)
            {
                return this;
            }
            try
            {
                @catch(_exception);
                return new Option<T>();
            }
            catch (Exception exception)
            {
                return new Option<T>(_exception, exception);
            }
        }

        [Pure]
        public Option<T> Catch<TException>(Action<TException> @catch)
            where TException : Exception
        {
            if (@catch == null || _type != Fail)
            {
                return this;
            }
            try
            {
                var exception = _exception as TException;
                if (exception != null)
                {
                    @catch(exception);
                    return new Option<T>();
                }
            }
            catch (Exception exception)
            {
                return new Option<T>(_exception, exception);
            }
            return this;
        }

        [Pure]
        public Option<T> Check(Func<T, bool> check)
        {
            if (check == null || _type != Some)
            {
                return this;
            }
            try
            {
                return check(_value) ? this : new Option<T>();
            }
            catch (Exception exception)
            {
                return new Option<T>(exception);
            }
        }

        [Pure]
        public override bool Equals(object value)
        {
            if (value is Option<T>)
            {
                return Equals((Option<T>) value);
            }
            if (_type == Some)
            {
                if (value is T)
                {
                    var equatable = _value as IEquatable<T>;
                    if (equatable == null)
                    {
                        return _value.Equals(value);
                    }
                    return equatable.Equals(value);
                }
                return false;
            }
            if (_type == Fail)
            {
                return _exception == value;
            }
            return value == null || Equals(value, DBNull.Value);
        }

        [Pure]
        public bool Equals(Option<T> option)
        {
            if (_type == Some && option._type == Some)
            {
                return _value.Equals(option._value);
            }
            if (_type == Fail && option._type == Fail)
            {
                return _exception == option._exception;
            }
            return _type == None && option._type == None;
        }

        [Pure]
        public bool Equals(T value)
        {
            if (_type == Some)
            {
                return _value.Equals(value);
            }
            if (_type == Fail)
            {
                return _exception == value as Exception;
            }
            return false;
        }

        [Pure]
        public override int GetHashCode()
        {
            return IsSome
                ? _value.GetHashCode()
                : IsFail
                    ? _exception.GetHashCode()
                    : 0;
        }

        [Pure]
        public Option<T> Match(Action none)
        {
            return Match(null, none);
        }

        [Pure]
        public Option<T> Match(Action<T> some = null, Action none = null)
        {
            try
            {
                if (_type == Some)
                {
                    if (some != null)
                    {
                        some(_value);
                    }
                }
                else if (_type == None)
                {
                    if (none != null)
                    {
                        none();
                    }
                }
            }
            catch (Exception exception)
            {
                return new Option<T>(exception);
            }
            return this;
        }

        [Pure]
        public Option<TResult> Match<TResult>(Func<TResult> none)
        {
            return Match(null, none);
        }

        [Pure]
        public Option<TResult> Match<TResult>(Func<T, TResult> some = null, Func<TResult> none = null)
        {
            try
            {
                if (_type == Some)
                {
                    if (some != null)
                    {
                        return new Option<TResult>(some(_value));
                    }
                }
                else if (_type == None)
                {
                    if (none != null)
                    {
                        return new Option<TResult>(none());
                    }
                }
            }
            catch (Exception exception)
            {
                return new Option<TResult>(exception);
            }
            return new Option<TResult>(_exception);
        }

        [Pure]
        public T Otherwise(T otherwise)
        {
            return _type == Some ? Value : otherwise;
        }

        public Option<TResult> Switch<TResult>(Func<Branch<T>, Branch<T, TResult>> @switch)
        {
            if (@switch == null)
            {
                return new Option<TResult>(_exception);
            }
            try
            {
                return @switch(new Branch<T>()).Invoke(this);
            }
            catch (Exception exception)
            {
                return new Option<TResult>(exception, _exception);
            }
        }
            
        [Pure]
        public Option<T> Throw()
        {
            if (_type == Fail)
            {
                throw _exception;
            }
            return this;
        }

        [Pure]
        public Option<TTarget> To<TTarget>()
        {
            return ToSpecial<TTarget>(null, new ConversionSettings());
        }

        [Pure]
        public Option<TTarget> To<TTarget>(CultureInfo culture)
        {
            return ToSpecial<TTarget>(null, new ConversionSettings(culture));
        }

        [Pure]
        public Option<TTarget> To<TTarget>(CultureInfo culture, string format)
        {
            return ToSpecial<TTarget>(null, new ConversionSettings(culture, format));
        }

        [Pure]
        public Option<TTarget> To<TTarget>(CultureInfo culture, string format, DateTimeStyles styles)
        {
            return ToSpecial<TTarget>(null, new ConversionSettings(culture, format, styles));
        }

        [Pure]
        public Option<TTarget> To<TTarget>(CultureInfo culture, string format, NumberStyles styles)
        {
            return ToSpecial<TTarget>(null, new ConversionSettings(culture, format, styles));
        }

        [Pure]
        public Option<TTarget> To<TTarget>(CultureInfo culture, string format, TimeSpanStyles styles)
        {
            return ToSpecial<TTarget>(null, new ConversionSettings(culture, format, styles));
        }

        [Pure]
        public Option<TTarget> To<TTarget>(CultureInfo culture, DateTimeStyles styles)
        {
            return ToSpecial<TTarget>(null, new ConversionSettings(culture, styles));
        }

        [Pure]
        public Option<TTarget> To<TTarget>(CultureInfo culture, NumberStyles styles)
        {
            return ToSpecial<TTarget>(null, new ConversionSettings(culture, styles));
        }

        [Pure]
        public Option<TTarget> To<TTarget>(CultureInfo culture, TimeSpanStyles styles)
        {
            return ToSpecial<TTarget>(null, new ConversionSettings(culture, styles));
        }

        [Pure]
        public Option<TTarget> To<TTarget>(string format)
        {
            return ToSpecial<TTarget>(null, new ConversionSettings(format));
        }

        [Pure]
        public Option<TTarget> To<TTarget>(string format, DateTimeStyles styles)
        {
            return ToSpecial<TTarget>(null, new ConversionSettings(format, styles));
        }

        [Pure]
        public Option<TTarget> To<TTarget>(string format, NumberStyles styles)
        {
            return ToSpecial<TTarget>(null, new ConversionSettings(format, styles));
        }

        [Pure]
        public Option<TTarget> To<TTarget>(string format, TimeSpanStyles styles)
        {
            return ToSpecial<TTarget>(null, new ConversionSettings(format, styles));
        }

        [Pure]
        public Option<TTarget> To<TTarget>(ConversionSettings settings)
        {
            return ToSpecial<TTarget>(null, settings);
        }

        [Pure]
        public Option<TTarget> To<TTarget>(DateTimeStyles styles)
        {
            return ToSpecial<TTarget>(null, new ConversionSettings(styles));
        }

        [Pure]
        public Option<TTarget> To<TTarget>(NumberStyles styles)
        {
            return ToSpecial<TTarget>(null, new ConversionSettings(styles));
        }

        [Pure]
        public Option<TTarget> To<TTarget>(TimeSpanStyles styles)
        {
            return ToSpecial<TTarget>(null, new ConversionSettings(styles));
        }

        [Pure]
        public IOption To(Type target, ConversionSettings settings)
        {
            return ToSpecial(target, null, settings);
        }

        [Pure]
        public Option<TTarget> ToSpecial<TTarget>(string category)
        {
            return ToSpecial<TTarget>(category, new ConversionSettings());
        }

        [Pure]
        public Option<TTarget> ToSpecial<TTarget>(string category, CultureInfo culture)
        {
            return ToSpecial<TTarget>(category, new ConversionSettings(culture));
        }

        [Pure]
        public Option<TTarget> ToSpecial<TTarget>(string category, CultureInfo culture, string format)
        {
            return ToSpecial<TTarget>(category, new ConversionSettings(culture, format));
        }

        [Pure]
        public Option<TTarget> ToSpecial<TTarget>(string category, CultureInfo culture, string format, DateTimeStyles styles)
        {
            return ToSpecial<TTarget>(category, new ConversionSettings(culture, format, styles));
        }

        [Pure]
        public Option<TTarget> ToSpecial<TTarget>(string category, CultureInfo culture, string format, NumberStyles styles)
        {
            return ToSpecial<TTarget>(category, new ConversionSettings(culture, format, styles));
        }

        [Pure]
        public Option<TTarget> ToSpecial<TTarget>(string category, CultureInfo culture, string format, TimeSpanStyles styles)
        {
            return ToSpecial<TTarget>(category, new ConversionSettings(culture, format, styles));
        }

        [Pure]
        public Option<TTarget> ToSpecial<TTarget>(string category, CultureInfo culture, DateTimeStyles styles)
        {
            return ToSpecial<TTarget>(category, new ConversionSettings(culture, styles));
        }

        [Pure]
        public Option<TTarget> ToSpecial<TTarget>(string category, CultureInfo culture, NumberStyles styles)
        {
            return ToSpecial<TTarget>(category, new ConversionSettings(culture, styles));
        }

        [Pure]
        public Option<TTarget> ToSpecial<TTarget>(string category, CultureInfo culture, TimeSpanStyles styles)
        {
            return ToSpecial<TTarget>(category, new ConversionSettings(culture, styles));
        }

        [Pure]
        public Option<TTarget> ToSpecial<TTarget>(string category, string format)
        {
            return ToSpecial<TTarget>(category, new ConversionSettings(format));
        }

        [Pure]
        public Option<TTarget> ToSpecial<TTarget>(string category, string format, DateTimeStyles styles)
        {
            return ToSpecial<TTarget>(category, new ConversionSettings(format, styles));
        }

        [Pure]
        public Option<TTarget> ToSpecial<TTarget>(string category, string format, NumberStyles styles)
        {
            return ToSpecial<TTarget>(category, new ConversionSettings(format, styles));
        }

        [Pure]
        public Option<TTarget> ToSpecial<TTarget>(string category, string format, TimeSpanStyles styles)
        {
            return ToSpecial<TTarget>(category, new ConversionSettings(format, styles));
        }

        [Pure]
        public Option<TTarget> ToSpecial<TTarget>(string category, ConversionSettings settings)
        {
            if (_type == Some)
            {
                var source = _value.GetType();
                var target = typeof(TTarget);
                var conversion = new Conversion(category, settings.Features, source, target);
                var converter = Safe.Converters.Resolve(conversion);
                var typed = converter as Converter<T, TTarget>;
                if (typed != null)
                {
                    Debug.WriteLine(
                        "Converting from '{0}.{1}' to '{2}.{3}' using typed '{4}'.",
                        source.Namespace, source.Name, target.Namespace, target.Name, converter.DebuggerDisplay);
                    return typed.Convert(_value, settings);
                }
                var untyped = converter as Converter<TTarget>;
                if (untyped != null)
                {
                    Debug.WriteLine(
                        "Converting from '{0}.{1}' to '{2}.{3}' using untyped '{4}'.",
                        source.Namespace, source.Name, target.Namespace, target.Name, converter.DebuggerDisplay);
                    return untyped.Convert(_value, settings);
                }
                Debug.WriteLine(
                    "Converting from '{0}.{1}' to '{2}.{3}' using 'None'.",
                    source.Namespace, source.Name, target.Namespace, target.Name);
            }
            else if (_type == Fail)
            {
                return new Option<TTarget>(_exception);
            }
            return new Option<TTarget>();
        }

        [Pure]
        public Option<TTarget> ToSpecial<TTarget>(string category, DateTimeStyles styles)
        {
            return ToSpecial<TTarget>(category, new ConversionSettings(styles));
        }

        [Pure]
        public Option<TTarget> ToSpecial<TTarget>(string category, NumberStyles styles)
        {
            return ToSpecial<TTarget>(category, new ConversionSettings(styles));
        }

        [Pure]
        public Option<TTarget> ToSpecial<TTarget>(string category, TimeSpanStyles styles)
        {
            return ToSpecial<TTarget>(category, new ConversionSettings(styles));
        }

        [Pure]
        public IOption ToSpecial(Type target, string category, ConversionSettings settings)
        {
            if (_type == Some)
            {
                var source = _value.GetType();
                var conversion = new Conversion(category, settings.Features, source, target);
                var converter = Safe.Converters.Resolve(conversion);
                if (converter != null)
                {
                    Debug.WriteLine(
                        "Converting from '{0}.{1}' to '{2}.{3}' using untyped '{4}'.",
                        source.Namespace, source.Name, target.Namespace, target.Name, converter.DebuggerDisplay);
                    return converter.Convert(_value, settings);
                }
                Debug.WriteLine(
                    "Converting from '{0}.{1}' to '{2}.{3}' using 'None'.",
                    source.Namespace, source.Name, target.Namespace, target.Name);
            }
            else if (_type == Fail)
            {
                return Safe.Option(target, _exception);
            }
            return Safe.Option(target);
        }

        [Pure]
        public override string ToString()
        {
            if (IsSome)
            {
                return _value.ToString();
            }
            if (IsFail)
            {
                return _exception.ToString();
            }
            return String.Empty;
        }

        #endregion

        #region OPERATORS

        public static implicit operator T(Option<T> safe)
        {
            return safe.Value;
        }

        public static bool operator true(Option<T> safe)
        {
            return safe._type == Some;
        }

        public static bool operator false(Option<T> safe)
        {
            return safe._type != Some;
        }

        public static bool operator ==(Option<T> left, Option<T> right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Option<T> left, Option<T> right)
        {
            return !left.Equals(right);
        }

        #endregion
    }
}