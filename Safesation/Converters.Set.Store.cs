using System;
using System.Diagnostics;

namespace Safesation
{
    partial class Converters
    {
        partial class Set
        {
            private partial class Store
            {
                #region CONSTRUCTORS

                public Store()
                {
                    _capacity = InitialCapacity;
                    _indexes = new int[_capacity];
                    _entries = new Entry[_capacity];
                    for (var i = 0; i < _capacity; i++)
                    {
                        _entries[i].Next = _indexes[i] = None;
                    }
                }

                private Store(Store source)
                {
                    _capacity = Primes.Next(source._capacity);
                    _indexes = new int[_capacity];
                    _entries = new Entry[_capacity];
                    for (var i = 0; i < _capacity; i++)
                    {
                        _entries[i].Next = _indexes[i] = None;
                    }
                    for (var i = 0; i < source._capacity; i++)
                    {
                        var entry = source._indexes[i];
                        while (entry != None)
                        {
                            var item = source._entries[entry].Item;
                            var index = Hash(item)%_capacity;
                            _entries[_count] = new Entry(item, _indexes[index]);
                            _indexes[index] = _count;
                            _count++;
                            entry = source._entries[entry].Next;
                        }
                    }
                }

                #endregion

                #region FIELDS

                private readonly int _capacity;

                private volatile int _count;

                private readonly Entry[] _entries;

                private readonly int[] _indexes;

                #endregion

                #region PROPERTIES

                public int Count
                {
                    get { return _count; }
                }

                #endregion

                #region METHODS

                public Store Add(int hash, Type item)
                {
                    var store = this;
                    if (_count == _capacity)
                    {
                        store = new Store(this);
                    }
                    var index = hash % store._capacity;
                    store._entries[store._count] = new Entry(item, store._indexes[index]);
                    store._indexes[index] = store._count;
                    store._count++;
                    return store;
                }

                public int Find(int hash, Type item)
                {
                    var entries = _entries;
                    var entry = _indexes[hash % _capacity];
                    while (entry != None)
                    {
                        if (item == entries[entry].Item)
                        {
                            return entry;
                        }
                        entry = entries[entry].Next;
                    }
                    return None;
                }

                [DebuggerStepThrough]
                public int Hash(Type item)
                {
                    return item.GetHashCode() & Int32.MaxValue;
                }

                #endregion
            }
        }
    }
}
