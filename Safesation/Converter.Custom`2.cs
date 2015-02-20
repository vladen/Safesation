using System.Diagnostics;

namespace Safesation
{
    partial class Converter
    {
        private sealed class Custom<TSource, TTarget>
            : Converter<TSource, TTarget>
        {
            #region CONSTRUCTORS

            public Custom(ConversionFeatures features)
                : base(features, ConversionWays.None)
            {
                _delegate = None;
            }

            public Custom(Delegate @delegate, ConversionFeatures features)
                : base(features, ConversionWays.Custom)
            {
                Debug.Assert(@delegate != null, "Custom converter delegate is null.");
                _delegate = @delegate;
            } 

            #endregion

            #region FIELDS

            private readonly Delegate _delegate; 

            #endregion

            #region METHODS

            protected override Delegate Build()
            {
                return _delegate;
            }

            private static Option<TTarget> None(TSource source, ConversionSettings settings)
            {
                return new Option<TTarget>();
            }

            #endregion
        }
    }
}
