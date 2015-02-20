using System.Reflection.Emit;

namespace Safesation
{
    partial class Converter
    {
        private sealed class EnumParseMethod<TSource, TTarget>
            : Converter<TSource, TTarget>
        {
            #region CONSTRUCTORS

            public EnumParseMethod()
                : base(ConversionFeatures.None, ConversionWays.StaticMethod)
            {
            }

            #endregion

            #region METHODS

            protected override Delegate Build()
            {
                var method = new DynamicMethod("Converter.Enum", ReturnType, ParameterTypes);
                var il = method.GetILGenerator();
                var quit = il.DefineLabel();
                var result = il.DeclareLocal(ReturnType);
                il.BeginExceptionBlock();
                il.Emit(OpCodes.Ldtoken, TargetType);
                il.Emit(OpCodes.Call, GetTypeFromHandle);
                il.Emit(OpCodes.Ldarg_S, ValueArgument);
                il.Emit(OpCodes.Ldc_I4_1);
                il.Emit(OpCodes.Call, Types.Enum.GetMethod("Parse", new[] { Types.Type, Types.String, Types.Boolean }));
                il.Emit(OpCodes.Unbox_Any, TargetType);
                il.Emit(OpCodes.Newobj, ConstructSome);
                il.Emit(OpCodes.Stloc_S, result);
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