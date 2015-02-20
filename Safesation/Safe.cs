using System;

namespace Safesation
{
    public static class Safe
    {
        private static Converters _converters;

        public static Converters Converters
        {
            get { return _converters = (_converters ?? new Converters()); }
            set { _converters = value ?? new Converters(); }
        }

        public static IOption Option()
        {
            return new Option<object>();
        }

        public static Option<T> Option<T>()
        {
            return new Option<T>();
        }

        public static IOption Option(Type type)
        {
            if (type == null)
            {
                return null;
            }
            // ReSharper disable once PossibleNullReferenceException
            return Types.GenericOption
                .MakeGenericType(type)
                .GetConstructor(Type.EmptyTypes)
                .Invoke(new object[0]) as IOption;
        }

        public static IOption Option(Type type, Exception exception)
        {
            if (type == null)
            {
                return null;
            }
            // ReSharper disable once PossibleNullReferenceException
            return Types.GenericOption
                .MakeGenericType(type)
                .GetConstructor(new[] {Types.Exception})
                .Invoke(new object[] {exception}) as IOption;
        }

        public static Option<T> Option<T>(T value)
        {
            return new Option<T>(value);
        }
    }
}