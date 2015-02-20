using System;
using System.Reflection;

namespace Safesation
{
    public abstract class Converter<TTarget>
        : Converter
    {
        #region CONSTRUCTORS

        static Converter()
        {
            var targetType = typeof (TTarget);
            ReturnType = Types.GenericOption.MakeGenericType(targetType);
            ConstructFail = ReturnType.GetConstructor(new[] {Types.Exception});
            ConstructSome = ReturnType.GetConstructor(new[] {targetType});
        }

        protected Converter(Conversion conversion, ConversionWays way)
            : base(conversion, way)
        {
        }

        #endregion

        #region FIELDS

        protected static readonly ConstructorInfo ConstructFail;

        protected static readonly ConstructorInfo ConstructSome;

        protected static readonly Type ReturnType;

        #endregion

        #region METHODS

        public abstract Option<TTarget> Convert(object value, ConversionSettings settings); 

        #endregion
    }
}