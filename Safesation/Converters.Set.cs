using System;
using System.Diagnostics;

namespace Safesation
{
    using Annotations;

    partial class Converters
    {
        [DebuggerDisplay("{DebuggerDisplay}")]
        private sealed partial class Set
        {
            #region CONSTRUCTORS

            public Set()
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

            [UsedImplicitly]
            internal string DebuggerDisplay
            {
                get { return string.Format("Set of types, {0} item(s)", _store.Count); }
            }

            #endregion

            #region METHODS

            public bool TryAdd(Type item)
            {
                var hash = _store.Hash(item);
                var index = _store.Find(hash, item);
                if (index != None)
                {
                    return false;
                }
                lock (_sync)
                {
                    index = _store.Find(hash, item);
                    if (index != None)
                    {
                        return false;
                    }
                    _store = _store.Add(hash, item);
                    return true;
                }
            }

            #endregion
        }
    }
}
