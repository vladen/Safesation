using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;

namespace Safesation
{
    partial class Converter
    {
        private sealed class InstanceMethod<TSource, TTarget>
            : Converter<TSource, TTarget>
        {
            #region CONSTRUCTORS

            public InstanceMethod(MethodInfo method)
                : base(Categorize(method.Name), ConversionFeatures.None, ConversionWays.InstanceMethod)
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
                var quit = il.DefineLabel();
                var result = il.DeclareLocal(ReturnType);
                il.BeginExceptionBlock();
                if (SourceType.IsValueType)
                {
                    il.Emit(OpCodes.Ldarga_S, ValueArgument);
                    il.Emit(OpCodes.Call, _method);
                }
                else
                {
                    il.Emit(OpCodes.Ldarg_S, ValueArgument);
                    il.Emit(OpCodes.Callvirt, _method);
                }
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