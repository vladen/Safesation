using System.Diagnostics;

namespace Safesation
{
    partial class Converters
    {
        partial class Map
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
                    public Entry(Converter item, int next = None)
                    {
                        Item = item;
                        Next = next;
                    }

                    #endregion

                    #region FIELDS

                    public Converter Item;

                    public int Next;

                    #endregion
                }
            }
        }
    }
}