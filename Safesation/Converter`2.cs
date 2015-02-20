using System;
using System.Diagnostics;

namespace Safesation
{
    using Annotations;

    public abstract class Converter<TSource, TTarget>
        : Converter<TTarget>
    {
        #region CONSTRUCTORS

        static Converter()
        {
            SourceType = typeof (TSource);
            ParameterTypes = new[] { SourceType, Types.ConversionSettings };
            TargetType = typeof(TTarget);
        }

        protected Converter(ConversionFeatures features, ConversionWays way)
            : base(new Conversion(null, features, SourceType, TargetType), way)
        {
        }

        protected Converter(string category, ConversionFeatures features, ConversionWays way)
            : base(new Conversion(category, features, SourceType, TargetType), way)
        {
        }

        #endregion

        #region DELEGATES

        public delegate Option<TTarget> Delegate(TSource value, ConversionSettings settings);

        #endregion

        #region FIELDS

        private volatile Delegate _delegate;

        protected static readonly Type[] ParameterTypes;

        protected new static readonly Type SourceType;

        protected new static readonly Type TargetType;

        #endregion

        #region METHODS

        protected abstract Delegate Build();

        [CanBeNull]
        protected static string Categorize([NotNull]string name)
        {
            if (name.StartsWith("From", StringComparison.Ordinal))
            {
                var category = name.Substring(4);
                if (category.EndsWith(SourceType.Name))
                {
                    category = category.Substring(0, category.Length - SourceType.Name.Length);
                    if (category.Length == 0)
                    {
                        category = null;
                    }
                }
                return category;
            }
            if (name.StartsWith("To", StringComparison.Ordinal))
            {
                var category = name.Substring(2);
                if (category.EndsWith(TargetType.Name))
                {
                    category = category.Substring(0, category.Length - TargetType.Name.Length);
                    if (category.Length == 0)
                    {
                        category = null;
                    }
                }
                return category;
            }
            return null;
        }

        public override Option<TTarget> Convert(object value, ConversionSettings settings)
        {
            Debug.Assert(value is TSource, "Value to be converted is not assignable to " + typeof(TSource).Name + ".");
            Delegate @delegate;
            if (!TryBuild(out @delegate))
            {
                return new Option<TTarget>();
            }
            try
            {
                return @delegate.Invoke((TSource)value, settings);
            }
            catch (Exception exception)
            {
                return new Option<TTarget>(exception);
            }
        }

        public override IOption Convert<T>(T value, ConversionSettings settings)
        {
            Debug.Assert(value is TSource, "Value to be converted is not assignable to " + typeof(TSource).Name + ".");
            Delegate @delegate;
            if (!TryBuild(out @delegate))
            {
                return new Option<TTarget>();
            }
            try
            {
                return @delegate.Invoke((TSource)(object)value, settings);
            }
            catch (Exception exception)
            {
                return new Option<TTarget>(exception);
            }
        }

        public Option<TTarget> Convert(TSource value, ConversionSettings settings)
        {
            Delegate @delegate;
            if (!TryBuild(out @delegate))
            {
                return new Option<TTarget>();
            }
            try
            {
                return @delegate.Invoke(value, settings);
            }
            catch (Exception exception)
            {
                return new Option<TTarget>(exception);
            }
        }

        public bool TryBuild(out Delegate @delegate)
        {
            // todo: possibly synchronize
            if (_delegate == null)
            {
                _delegate = Build();
                Debug.WriteLine(DebuggerDisplay + " was built.");
                Debug.Assert(_delegate != null, "Converter delegate is null.");
            }
            @delegate = _delegate;
            return true;
        }

        #endregion
    }
}