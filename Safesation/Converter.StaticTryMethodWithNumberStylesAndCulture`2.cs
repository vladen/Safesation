using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;

namespace Safesation
{
    partial class Converter
    {
        private sealed class StaticTryMethodWithNumberStylesAndCulture<TSource, TTarget>
            : Converter<TSource, TTarget>
        {
            #region CONSTRUCTORS

            public StaticTryMethodWithNumberStylesAndCulture(MethodInfo method)
                : base(ConversionFeatures.CultureStyles, ConversionWays.StaticTryMethod)
            {
                Debug.Assert(method != null, "Converter method is null.");
                _method = method;
            } 

            #endregion

            #region FIELDS

            private readonly MethodInfo _method; 

            #endregion

            #region METHODS

            protected override Delegate Build()
            {
                var method = new DynamicMethod("Converter." + _method.Name, ReturnType, ParameterTypes);
                var il = method.GetILGenerator();
                var none = il.DefineLabel();
                var quit = il.DefineLabel();
                var result = il.DeclareLocal(ReturnType);
                var target = il.DeclareLocal(TargetType);
                il.BeginExceptionBlock();
                il.Emit(OpCodes.Ldarg_S, ValueArgument);
                il.Emit(OpCodes.Ldarga_S, ConversionArgument);
                il.Emit(OpCodes.Call, GetNumberStyles);
                il.Emit(OpCodes.Ldarga_S, ConversionArgument);
                il.Emit(OpCodes.Call, GetCulture);
                il.Emit(OpCodes.Ldloca_S, target);
                il.Emit(OpCodes.Call, _method);
                il.Emit(OpCodes.Ldc_I4_0);
                il.Emit(OpCodes.Ceq);
                il.Emit(OpCodes.Brtrue_S, none);
                il.Emit(OpCodes.Ldloc_S, target);
                il.Emit(OpCodes.Newobj, ConstructSome);
                il.Emit(OpCodes.Stloc_S, result);
                il.Emit(OpCodes.Leave_S, quit);
                il.MarkLabel(none);
                il.Emit(OpCodes.Ldloca_S, result);
                il.Emit(OpCodes.Initobj, ReturnType);
                il.Emit(OpCodes.Leave_S, quit);
                il.BeginCatchBlock(Types.Exception);
                il.Emit(OpCodes.Newobj, ConstructFail);
                il.Emit(OpCodes.Stloc_S, result);
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
