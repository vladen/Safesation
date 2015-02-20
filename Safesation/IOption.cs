using System;

namespace Safesation
{
    public interface IOption
    {
        Exception Exception { get; }

        bool IsFail { get; }

        bool IsNone { get; }

        bool IsSome { get; }

        object Value { get; }
    }
}
