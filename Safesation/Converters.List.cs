using System;
using System.Collections;
using System.Collections.Generic;

namespace Safesation
{
    partial class Converters
    {
        private class List
            : IEnumerable<Type>
        {
            #region CONSTRUCTORS

            public List()
            {
                _items = new Type[16];
            } 

            #endregion

            #region FIELDS

            private int _count;

            private readonly Type[] _items;

            private List _next; 

            #endregion

            #region METHODS

            public List Add(Type type)
            {
                if (_count < _items.Length)
                {
                    _items[_count++] = type;
                    return this;
                }
                if (_next == null)
                {
                    _next = new List();
                }
                return _next.Add(type);
            }

            public IEnumerator<Type> GetEnumerator()
            {
                var current = this;
                do
                {
                    var count = 0;
                    while (count < current._items.Length)
                    {
                        yield return current._items[count++];
                    }
                    current = current._next;
                } while (current != null);
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            } 

            #endregion
        }
    }
}