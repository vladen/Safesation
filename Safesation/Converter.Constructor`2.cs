using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;

namespace Safesation
{
    partial class Converter
    {
        private sealed class Constructor<TSource, TTarget>
            : Converter<TSource, TTarget>
        {
            #region CONSTRUCTORS

            public Constructor(ConstructorInfo constructor)
                : base(ConversionFeatures.None, ConversionWays.Constructor)
            {
                Debug.Assert(constructor != null, "Converter constructor is null.");
                _constructor = constructor;
            }

            #endregion

            #region FIELDS

            private readonly ConstructorInfo _constructor; 

            #endregion

            #region METHODS

            protected override Delegate Build()
            {
                var method = new DynamicMethod("Converter.Constructor", ReturnType, ParameterTypes);
                var il = method.GetILGenerator();
                var quit = il.DefineLabel();
                var result = il.DeclareLocal(ReturnType);
                il.BeginExceptionBlock();
                il.Emit(OpCodes.Ldarg_S, ValueArgument);
                il.Emit(OpCodes.Newobj, _constructor);
                il.Emit(OpCodes.Newobj, ConstructSome);
                il.Emit(OpCodes.Stloc_S, result);
                il.Emit(OpCodes.Leave_S, quit);
                il.BeginCatchBlock(Types.Exception);
                il.Emit(OpCodes.Newobj, ConstructFail);
                il.Emit(OpCodes.Stloc, result);
                il.Emit(OpCodes.Leave_S, quit);
                il.EndExceptionBlock();
                il.MarkLabel(quit);
                il.Emit(OpCodes.Ldloc_S, result);
                il.Emit(OpCodes.Ret);
                return method.CreateDelegate(typeof(Delegate)) as Delegate;
            } 

            #endregion
        }
    }
}