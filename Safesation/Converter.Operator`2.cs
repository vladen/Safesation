using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;

namespace Safesation
{
    partial class Converter
    {
        private sealed class Operator<TSource, TTarget>
            : Converter<TSource, TTarget>
        {
            #region CONSTRUCTORS

            public Operator(MethodInfo method)
                : base(ConversionFeatures.None, ConversionWays.Operator)
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
                var method = new DynamicMethod("Converter.Operator", ReturnType, ParameterTypes);
                var il = method.GetILGenerator();
                var quit = il.DefineLabel();
                il.DeclareLocal(ReturnType);
                il.BeginExceptionBlock();
                il.Emit(OpCodes.Ldarg_S, ValueArgument);
                il.Emit(OpCodes.Call, _method);
                il.Emit(OpCodes.Newobj, ConstructSome);
                il.Emit(OpCodes.Stloc_S, ValueArgument);
                il.Emit(OpCodes.Leave_S, quit);
                il.BeginCatchBlock(Types.Exception);
                il.Emit(OpCodes.Newobj, ConstructFail);
                il.Emit(OpCodes.Stloc_S, ValueArgument);
                il.Emit(OpCodes.Leave_S, quit);
                il.EndExceptionBlock();
                il.MarkLabel(quit);
                il.Emit(OpCodes.Ldloc_S, ValueArgument);
                il.Emit(OpCodes.Ret);
                return method.CreateDelegate(typeof(Delegate)) as Delegate;
            } 

            #endregion
        }
    }
}