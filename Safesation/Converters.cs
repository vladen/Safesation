using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace Safesation
{
    using Annotations;

    [DebuggerDisplay("{DebuggerDisplay}")]
    public sealed partial class Converters
    {
        #region CONSTRUCTORS

        public Converters()
        {
            _map = new Map();
            _set = new Set();
        }

        #endregion

        #region FIELDS

        private readonly Map _map;

        private readonly Set _set;

        #endregion

        #region PROPERTIES

        public int Count
        {
            get { return _map.Count; }
        }

        [UsedImplicitly]
        internal string DebuggerDisplay
        {
            get { return string.Format("Converters, {0} item(s)", Count); }
        }

        #endregion

        #region METHODS

        private Converter Discover(Conversion conversion)
        {
            Converter converter;
            if ((Register(conversion.Source) | Register(conversion.Target)) && _map.TryGet(conversion, out converter))
            {
                return converter;
            }
            var category = conversion.Category;
            var features = conversion.Features;
            var target = conversion.Target;
            var source = conversion.Source.BaseType;
            while (true)
            {
                while (source != null)
                {
                    if (_map.TryGet(new Conversion(category, features, source, target), out converter))
                    {
                        return converter;
                    }
                    source = source.BaseType;
                }
                if (features == ConversionFeatures.None)
                {
                    break;
                }
                if ((features & ConversionFeatures.Styles) != 0)
                {
                    features = features & ~ConversionFeatures.Styles;
                }
                else if ((features & ConversionFeatures.Format) != 0)
                {
                    features = features & ~ConversionFeatures.Format;
                }
                else features = features & ~ConversionFeatures.Culture;
                source = conversion.Source;
            }
            return Converter.Create(conversion.Source, conversion.Target);
        }

        public bool Register(Converter converter)
        {
            if (converter == null)
            {
                return false;
            }
            var registered = _map.TryAddOrUpdate(converter.Conversion, converter.IsBetterThan, converter);
            Debug.WriteLine(converter.DebuggerDisplay + " was " + (registered ? "registered" : "skipped") + ".");
            return registered;
        }

        public bool Register<TSource, TTarget>(Func<TSource, TTarget> @delegate)
        {
            return Register(
                Converter.Create<TSource, TTarget>(
                    (value, settings) => new Option<TTarget>(@delegate(value)),
                    ConversionFeatures.None));
        }

        public bool Register<TSource, TTarget>(Func<TSource, CultureInfo, TTarget> @delegate)
        {
            return Register(
                Converter.Create<TSource, TTarget>(
                    (value, settings) => new Option<TTarget>(@delegate(value, settings.Culture)),
                    ConversionFeatures.Culture));
        }

        public bool Register<TSource, TTarget>(Func<TSource, string, TTarget> @delegate)
        {
            return Register(
                Converter.Create<TSource, TTarget>(
                    (value, settings) => new Option<TTarget>(@delegate(value, settings.Format)),
                    ConversionFeatures.Format));
        }

        public bool Register<TSource, TTarget>(Func<TSource, CultureInfo, string, TTarget> @delegate)
        {
            return Register(
                Converter.Create<TSource, TTarget>(
                    (value, settings) => new Option<TTarget>(@delegate(value, settings.Culture, settings.Format)),
                    ConversionFeatures.CultureFormat));
        }

        private bool Register(List types)
        {
            var registered = false;
            foreach (var type in types)
            {
                if (type != null && _set.TryAdd(type))
                {
                    foreach (var converter in Scan(type))
                    {
                        if (converter != null)
                        {
                            registered |= Register(converter);
                            types = types.Add(converter.Conversion.Source).Add(converter.Conversion.Target);
                        }
                    }
                    Debug.WriteLine("Type '" + type.Namespace + "." + type.Name + "' was registered.");
                    types = types.Add(type.BaseType);
                }
            }
            return registered;
        }

        public bool Register(Type type)
        {
            return Register(new List().Add(type));
        }

        public bool Register(params Type[] types)
        {
            return Register(types.Aggregate(new List(), (list, type) => list.Add(type)));
        }

        [CanBeNull]
        public Converter Resolve(Conversion conversion)
        {
            return _map.GetOrAdd(conversion, Discover);
        }

        private static IEnumerable<Converter> Scan(Type type)
        {
            var constructors = type.GetConstructors(BindingFlags.Public | BindingFlags.Instance);
            foreach (var constructor in constructors)
            {
                yield return Converter.Create(constructor);
            }
            var methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static);
            for (var i = 0; i < methods.Length; i++)
            {
                var method = methods[i];
                if (method.DeclaringType == type)
                {
                    yield return Converter.Create(method);
                }
            }
            // todo: special case for arrays
            if (type.IsArray)
            {
                yield break;
            }
            var interfaces = type.GetInterfaces();
            for (var i = 0; i < interfaces.Length; i++)
            {
                var @interface = interfaces[i];
                methods = type.GetInterfaceMap(@interface).TargetMethods;
                for (var j = 0; j < methods.Length; j++)
                {
                    var method = methods[j];
                    if (method.DeclaringType == type)
                    {
                        yield return Converter.Create(method);
                    }
                }
            }
        }

        #endregion
    }
}