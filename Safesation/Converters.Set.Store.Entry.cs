using System;
using System.Diagnostics;

namespace Safesation
{
    partial class Converters
    {
        partial class Set
        {
            partial class Store
            {
                private struct Entry
                {
                    #region CONSTRUCTORS

                    [DebuggerStepThrough]
                    public Entry(int next)
                    {
                        Item = null;
                        Next = next;
                    }

                    [DebuggerStepThrough]
                    public Entry(Type item, int next = None)
                    {
                        Item = item;
                        Next = next;
                    }

                    #endregion

                    #region FIELDS

                    public readonly Type Item;

                    public int Next;

                    #endregion
                }
            }
        }
    }
}
