using System.Reflection.Emit;

namespace Safesation
{
    partial class Converter
    {
        private sealed class Assignment<TSource, TTarget>
            : Converter<TSource, TTarget>
        {
            #region CONSTRUCTORS

            public Assignment()
                : base(ConversionFeatures.None, ConversionWays.Assignment)
            {
            } 

            #endregion

            #region METHODS

            protected override Delegate Build()
            {
                var result = new DynamicMethod("Converter.Assignment", ReturnType, ParameterTypes);
                var il = result.GetILGenerator();
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Newobj, ConstructSome);
                il.Emit(OpCodes.Ret);
                return result.CreateDelegate(typeof(Delegate)) as Delegate;
            } 

            #endregion
        }
    }
}
