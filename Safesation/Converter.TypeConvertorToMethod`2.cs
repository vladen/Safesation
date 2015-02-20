using System.Reflection.Emit;

namespace Safesation
{
    partial class Converter
    {
        private sealed class TypeConvertorToMethod<TSource, TTarget>
            : Converter<TSource, TTarget>
        {
            #region CONSTRUCTORS

            public TypeConvertorToMethod()
                : base(ConversionFeatures.Culture, ConversionWays.TypeConverter)
            {
            }

            #endregion

            #region METHODS

            protected override Delegate Build()
            {
                var method = new DynamicMethod("Converter.TypeConverter.ConvertTo", ReturnType, ParameterTypes);
                var il = method.GetILGenerator();
                var quit = il.DefineLabel();
                var result = il.DeclareLocal(ReturnType);
                il.BeginExceptionBlock();
                il.Emit(OpCodes.Ldtoken, SourceType);
                il.Emit(OpCodes.Call, GetTypeFromHandle);
                il.Emit(OpCodes.Call, GetConverter);
                il.Emit(OpCodes.Ldnull);
                il.Emit(OpCodes.Ldarga_S, ConversionArgument);
                il.Emit(OpCodes.Call, GetCulture);
                il.Emit(OpCodes.Ldarg_S, ValueArgument);
                if (SourceType.IsValueType)
                {
                    il.Emit(OpCodes.Box, SourceType);
                }
                il.Emit(OpCodes.Ldtoken, TargetType);
                il.Emit(OpCodes.Call, GetTypeFromHandle);
                il.Emit(OpCodes.Callvirt, ConvertTo);
                il.Emit(TargetType.IsValueType ? OpCodes.Unbox_Any : OpCodes.Castclass, TargetType);
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