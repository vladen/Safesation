using System;
using System.ComponentModel;
using System.Globalization;

namespace Safesation
{
    internal static class Types
    {
        #region CONSTRUCTORS

        static Types()
        {
            Boolean = typeof(Boolean);
            Byte = typeof(Byte);
            ConversionSettings = typeof(ConversionSettings);
            CultureInfo = typeof(CultureInfo);
            DateTime = typeof (DateTime);
            DateTimeStyles = typeof(DateTimeStyles);
            Enum = typeof(Enum);
            Exception = typeof(Exception);
            GenericNullable = typeof(Nullable<>);
            GenericOption = typeof (Option<>);
            Int16 = typeof(Int16);
            Int32 = typeof(Int32);
            Int64 = typeof(Int64);
            NumberStyles = typeof(NumberStyles);
            Object = typeof(Object);
            RuntimeTypeHandle = typeof(RuntimeTypeHandle);
            SByte = typeof(SByte);
            String = typeof(String);
            TimeSpanStyles = typeof (TimeSpanStyles);
            Type = typeof(Type);
            TypeConverter = typeof(TypeConverter);
            TypeDescriptor = typeof(TypeDescriptor);
            TypeDescriptorContextInterface = typeof(ITypeDescriptorContext);
            UInt16 = typeof(UInt16);
            UInt32 = typeof(UInt32);
            UInt64 = typeof(UInt64);
            Void = typeof(void);
            EnumBase = new[]
            {
                Byte, Int16, Int32, Int64, SByte, UInt16, UInt32, UInt64
            };
        } 

        #endregion

        #region FIELDS

        public static readonly Type Boolean;

        public static readonly Type Byte;

        public static readonly Type ConversionSettings;

        public static readonly Type CultureInfo;

        public static readonly Type DateTime;

        public static readonly Type DateTimeStyles;

        public static readonly Type Enum;

        public static readonly Type[] EnumBase;

        public static readonly Type Exception;

        public static readonly Type GenericNullable;

        public static readonly Type GenericOption;

        public static readonly Type Int16;

        public static readonly Type Int32;

        public static readonly Type Int64;

        public static readonly Type NumberStyles;

        public static readonly Type Object;

        public static readonly Type RuntimeTypeHandle;

        public static readonly Type SByte;

        public static readonly Type String;

        public static readonly Type TimeSpanStyles;

        public static readonly Type Type;

        public static readonly Type TypeConverter;

        public static readonly Type TypeDescriptor;

        public static readonly Type TypeDescriptorContextInterface;

        public static readonly Type UInt16;

        public static readonly Type UInt32;

        public static readonly Type UInt64;

        public static readonly Type Void; 

        #endregion

        #region METHODS

        public static bool IsEnumBase(Type type)
        {
            if (Array.IndexOf(EnumBase, type) >= 0)
                return true;
            return false;
        }

        public static bool IsNullable(Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == GenericNullable;
        }

        #endregion
    }
}
