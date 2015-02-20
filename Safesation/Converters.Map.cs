using System;
using System.Diagnostics;

namespace Safesation
{
    using Annotations;

    partial class Converters
    {
        [DebuggerDisplay("{DebuggerDisplay}")]
        private sealed partial class Map
        {
            #region CONSTRUCTORS

            public Map()
            {
                _store = new Store();
                _sync = new object();
            }

            #endregion

            #region CONSTANTS

            private const int InitialCapacity = 97;

            private const int None = -1;

            #endregion

            #region FIELDS

            private volatile Store _store;

            private readonly object _sync;

            #endregion

            #region PROPERTIES

            public int Count
            {
                get { return _store.Count; }
            }

            [UsedImplicitly]
            internal string DebuggerDisplay
            {
                get { return string.Format("Map of converters, {0} item(s)", _store.Count); }
            }

            #endregion

            #region METHODS

            public Converter GetOrAdd(Conversion key, [NotNull]Func<Conversion, Converter> add)
            {
                if (add == null) throw new ArgumentNullException("add");
                var hash = Store.Hash(key);
                var value = default(Converter);
                var index = _store.Find(hash, key, ref value);
                if (index != None)
                {
                    return value;
                }
                lock (_sync)
                {
                    index = _store.Find(hash, key, ref value);
                    if (index != None)
                    {
                        return value;
                    }
                    value = add(key);
                    _store = _store.Add(hash, value);
                    return value;
                }
            }

            [Pure]
            public bool TryGet(Conversion key, out Converter value)
            {
                value = null;
                return _store.Find(Store.Hash(key), key, ref value) != None;
            }

            public bool TryAddOrUpdate(Conversion key, [NotNull]Func<Converter, bool> check, Converter value)
            {
                if (check == null) throw new ArgumentNullException("check");
                lock (_sync)
                {
                    var hash = Store.Hash(key);
                    var existing = default(Converter);
                    var index = _store.Find(hash, key, ref existing);
                    while (true)
                    {
                        if (index == None)
                        {
                            _store = _store.Add(hash, value);
                            return true;
                        }
                        if (!check(existing))
                        {
                            return false;
                        }
                        _store = _store.Update(index, value);
                        return true;
                    }
                }
            }

            #endregion
        }
    }
}