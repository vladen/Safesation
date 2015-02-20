using System;
using System.Diagnostics;

namespace Safesation
{
    partial class Converters
    {
        partial class Map
        {
            private sealed partial class Store
            {
                #region CONSTRUCTORS

                public Store()
                {
                    _capacity = InitialCapacity;
                    _indexes = new int[_capacity];
                    _entries = new Entry[_capacity];
                    _version = int.MinValue;
                    for (var i = 0; i < _capacity; i++)
                    {
                        _indexes[i] = None;
                        _entries[i] = new Entry(None);
                    }
                }

                private Store(Store source)
                {
                    _capacity = Primes.Next(source._capacity);
                    _indexes = new int[_capacity];
                    _entries = new Entry[_capacity];
                    _version = source._version;
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
                            var index = Hash(item.Conversion)%_capacity;
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

                private volatile int _version;

                #endregion

                #region PROPERTIES

                public int Count
                {
                    get { return _count; }
                }

                #endregion

                #region METHODS

                public Store Add(int hash, Converter value)
                {
                    var store = this;
                    if (_count == _capacity)
                    {
                        store = new Store(this);
                    }
                    var index = hash % store._capacity;
                    store._entries[store._count] = new Entry(value, store._indexes[index]);
                    store._indexes[index] = store._count;
                    store._count++;
                    store._version++;
                    return store;
                }

                public int Find(int hash, Conversion key, ref Converter value)
                {
                    var entries = _entries;
                    var index = _indexes[hash % _capacity];
                    while (index != None)
                    {
                        value = entries[index].Item;
                        if (key.Equals(value.Conversion))
                        {
                            return index;
                        }
                        index = entries[index].Next;
                    }
                    return None;
                }

                [DebuggerStepThrough]
                public static int Hash(Conversion key)
                {
                    return key.Hash & Int32.MaxValue;
                }

                public Store Update(int index, Converter value)
                {
                    _entries[index].Item = value;
                    return this;
                }

                #endregion
            }
        }
    }
}